using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using VRC.UI.Elements.Menus;
using Object = UnityEngine.Object;
using PendulumClient.UI;

namespace PendulumClient.ButtonAPIV2
{
    public class APIV2_MenuPage : APIV2_MenuElement, APIV2_ButtonPageInterface
    {
        private static GameObject cached_menuprefab;

        private static GameObject MenuPrefab
        {
            get
            {
                if (cached_menuprefab == null)
                {
                    cached_menuprefab = NewQuickMenu.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_DevTools").gameObject;
                }
                return cached_menuprefab;
            }
        }
        private static int SiblingIndex => NewQuickMenu.Instance.field_Public_Transform_0.Find("Window/QMParent/Modal_AddMessage").GetSiblingIndex();

        private readonly List<APIV2_MenuPage> _subMenuPages = new List<APIV2_MenuPage>();
        private readonly List<APIV2_CategoryPage> _subCategoryPages = new List<APIV2_CategoryPage>();

        public event Action OnOpen;
        private readonly string _menuName;
        private readonly bool _isRoot;

        private readonly Transform _container;

        public UIPage UiPage { get; }

        public APIV2_MenuPage(string text, bool isRoot = false, bool isgrid = false, string displaytext = "") : base(MenuPrefab, MenuPrefab.transform.parent, $"Menu_PendulumClient_{text}", false)
        {
            Object.DestroyImmediate(GameObject.GetComponent<DevMenu>());

            RectTransform.SetSiblingIndex(SiblingIndex);

            _menuName = CleanName(text);
            _isRoot = isRoot;
            var headerTransform = RectTransform.GetChild(0);
            var titleText = headerTransform.GetComponentInChildren<TextMeshProUGUI>();
            if (string.IsNullOrEmpty(displaytext))
                titleText.text = text;
            else
                titleText.text = displaytext;
            titleText.richText = true;

            if (!_isRoot)
            {
                var backButton = headerTransform.GetComponentInChildren<Button>(true);
                backButton.gameObject.SetActive(true);
            }

            headerTransform.name = $"Header_{_menuName}";

            var buttonContainer = RectTransform.Find("Scrollrect/Viewport/VerticalLayoutGroup/Buttons");
            foreach (var obj in buttonContainer)
            {
                var control = obj.Cast<Transform>();
                if (control == null)
                {
                    continue;
                }
                Object.Destroy(control.gameObject);
            }

            // Set up UIPage
            UiPage = GameObject.AddComponent<UIPage>();
            UiPage.field_Public_String_0 = "PendulumClientPage_" + _menuName;
            UiPage.field_Private_Boolean_1 = true;
            UiPage.field_Protected_MenuStateController_0 = NewQuickMenu.Instance.field_Protected_MenuStateController_0;
            UiPage.field_Private_List_1_UIPage_0 = new Il2CppSystem.Collections.Generic.List<UIPage>();
            UiPage.field_Private_List_1_UIPage_0.Add(UiPage);

            // Get scroll stuff
            var scrollRect = RectTransform.Find("Scrollrect").GetComponent<ScrollRect>();
            _container = scrollRect.content;

            if (isgrid)
            {
                // copy properties of old grid layout
                var gridLayoutGroup = _container.Find("Buttons").GetComponent<GridLayoutGroup>();

                Object.DestroyImmediate(_container.GetComponent<VerticalLayoutGroup>());
                var glp = _container.gameObject.AddComponent<GridLayoutGroup>();
                glp.spacing = gridLayoutGroup.spacing;
                glp.cellSize = gridLayoutGroup.cellSize;
                glp.constraint = gridLayoutGroup.constraint;
                glp.constraintCount = gridLayoutGroup.constraintCount;
                glp.startAxis = gridLayoutGroup.startAxis;
                glp.startCorner = gridLayoutGroup.startCorner;
                glp.childAlignment = TextAnchor.UpperLeft;
                glp.padding = gridLayoutGroup.padding;
                glp.padding.top = 8;
                glp.padding.left = 64;
            }
            else
            {
                _container.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
                _container.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = true;
                _container.GetComponent<VerticalLayoutGroup>().childScaleHeight = true;
                _container.GetComponent<VerticalLayoutGroup>().padding.bottom = 10;
                _container.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            // delete components we're not using
            Object.DestroyImmediate(_container.Find("Buttons").gameObject);
            Object.DestroyImmediate(_container.Find("Spacer_8pt").gameObject);

            // Fix scrolling
            var scrollbar = scrollRect.transform.Find("Scrollbar");
            scrollbar.gameObject.SetActive(true);

            scrollRect.enabled = true;
            scrollRect.verticalScrollbar = scrollbar.GetComponent<Scrollbar>();
            scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
            scrollRect.viewport.GetComponent<RectMask2D>().enabled = true;

            NewQuickMenu.Instance.field_Protected_MenuStateController_0.field_Private_Dictionary_2_String_UIPage_0.Add(UiPage.field_Public_String_0, UiPage);

            if (isRoot)
            {
                var rootPages = NewQuickMenu.Instance.field_Protected_MenuStateController_0.field_Public_ArrayOf_UIPage_0.ToList();
                rootPages.Add(UiPage);
                NewQuickMenu.Instance.field_Protected_MenuStateController_0.field_Public_ArrayOf_UIPage_0 = rootPages.ToArray();
            }
        }

        public void Open()
        {
            if (_isRoot)
            {
                NewQuickMenu.Instance.field_Protected_MenuStateController_0.SwitchToRootPage("PendulumClientPage_" + _menuName);
            }
            else
            {
                NewQuickMenu.Instance.field_Protected_MenuStateController_0.PushPage("PendulumClientPage_" + _menuName);
            }

            OnOpen?.Invoke();
        }

        public APIV2_MenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null)
        {
            return new APIV2_MenuButton(text, tooltip, onClick, _container, sprite);
        }

        public APIV2_ToggleButton AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false)
        {
            return new APIV2_ToggleButton(text, tooltip, onToggle, _container);
        }

        /*public APIV2_ToggleButton AddToggle(string text, string tooltip, bool configValue)
        {
            return new APIV2_ToggleButton(text, tooltip, configValue, _container, configValue);
        }*/

        public APIV2_MenuPage AddMenuPage(string text, string tooltip = "", Sprite sprite = null, bool isgrid = false)
        {
            var existingPage = GetMenuPage(text);
            if (existingPage != null)
            {
                return existingPage;
            }

            var menu = new APIV2_MenuPage(text, false, isgrid);
            AddButton(text, string.IsNullOrEmpty(tooltip) ? $"Open the {text} menu" : tooltip, menu.Open, sprite);
            _subMenuPages.Add(menu);
            return menu;
        }

        public APIV2_CategoryPage AddCategoryPage(string text, string tooltip = "", Sprite sprite = null)
        {
            var existingPage = GetCategoryPage(text);
            if (existingPage != null)
            {
                return existingPage;
            }

            var menu = new APIV2_CategoryPage(text);
            AddButton(text, string.IsNullOrEmpty(tooltip) ? $"Open the {text} menu" : tooltip, menu.Open, sprite);
            _subCategoryPages.Add(menu);
            return menu;
        }

        public APIV2_MenuPage GetMenuPage(string name)
        {
            return _subMenuPages.FirstOrDefault(m => m.Name == CleanName($"Menu_PendulumClient_{name}"));
        }

        public APIV2_CategoryPage GetCategoryPage(string name)
        {
            return _subCategoryPages.FirstOrDefault(m => m.Name == CleanName($"Menu_PendulumClient_{name}"));
        }

        public static APIV2_MenuPage Create(string text, bool isRoot)
        {
            return new APIV2_MenuPage(text, isRoot);
        }
    }
}
