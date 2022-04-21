using System;
using System.Collections.Generic;
using System.Linq;
using PendulumClient.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using VRC.UI.Elements.Menus;
using Object = UnityEngine.Object;

using LaunchPadQMMenu = MonoBehaviour1PublicBuGaBuGaBuBuBuBuBuBuUnique;
namespace PendulumClient.ButtonAPIV2
{
    public class APIV2_CategoryPage : APIV2_MenuElement
    {
        private static GameObject cached_menuprefab;

        private static GameObject MenuPrefab
        {
            get
            {
                if (cached_menuprefab == null)
                {
                    cached_menuprefab = NewQuickMenu.Instance.field_Public_Transform_0.Find("Window/QMParent/Menu_Dashboard").gameObject;
                }
                return cached_menuprefab;
            }
        }

        private static int SiblingIndex => NewQuickMenu.Instance.field_Public_Transform_0.Find("Window/QMParent/Modal_AddMessage").GetSiblingIndex();

        private readonly List<APIV2_MenuCategory> _categories = new List<APIV2_MenuCategory>();

        public event Action OnOpen;
        private readonly string _menuName;
        private readonly bool _isRoot;

        private readonly Transform _container;

        public UIPage UiPage { get; }

        public APIV2_CategoryPage(string text, bool isRoot = false) : base(MenuPrefab, MenuPrefab.transform.parent, $"Menu_PendulumClient_{text}", false)
        {
            Object.DestroyImmediate(GameObject.GetComponent<LaunchPadQMMenu>());

            RectTransform.SetSiblingIndex(SiblingIndex);

            _menuName = CleanName(text);
            _isRoot = isRoot;
            var headerTransform = RectTransform.GetChild(0);
            Object.DestroyImmediate(headerTransform.Find("RightItemContainer/Button_QM_Expand").gameObject);

            var titleText = headerTransform.GetComponentInChildren<TextMeshProUGUI>();
            titleText.text = text;
            titleText.richText = true;


            if (!_isRoot)
            {
                var backButton = headerTransform.GetComponentInChildren<Button>(true);
                backButton.gameObject.SetActive(true);
            }

            _container = RectTransform.GetComponentInChildren<ScrollRect>().content;
            foreach (var obj in _container)
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

        public APIV2_MenuCategory AddCategory(string title, bool centered = false)
        {
            var existingCategory = GetCategory(title);
            if (existingCategory != null)
            {
                return existingCategory;
            }

            var category = new APIV2_MenuCategory(title, _container, centered);
            _categories.Add(category);
            return category;
        }

        public APIV2_MenuCategory GetCategory(string name)
        {
            return _categories.FirstOrDefault(c => c.Name == CleanName(name));
        }

        public static APIV2_CategoryPage Create(string text, bool isRoot)
        {
            return new APIV2_CategoryPage(text, isRoot);
        }
    }
}
