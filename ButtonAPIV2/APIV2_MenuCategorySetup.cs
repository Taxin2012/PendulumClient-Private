using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace PendulumClient.ButtonAPIV2
{
    public class APIV2_MenuHeader : APIV2_MenuElement
    {
        private static GameObject cached_headerprefab;
        private static GameObject HeaderPrefab
        {
            get
            {
                if (cached_headerprefab == null)
                {
                    cached_headerprefab = NewQuickMenu.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Dashboard/ScrollRect").GetComponent<ScrollRect>().content
                        .Find("Header_QuickActions").gameObject;
                }
                return cached_headerprefab;
            }
        }

        private readonly TextMeshProUGUI _text;
        public string Title
        {
            get => _text.text;
            set => _text.text = value;
        }

        public APIV2_MenuHeader(string title, Transform parent) : base(HeaderPrefab, (parent == null ? HeaderPrefab.transform.parent : parent), $"Header_{title}")
        {
            _text = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            _text.text = title;
            _text.richText = true;

            _text.transform.parent.GetComponent<HorizontalLayoutGroup>().childControlWidth = true;
        }

        protected APIV2_MenuHeader(GameObject original, Transform parent, Vector3 pos, string name, bool defaultState = true) : base(original, parent, pos, name, defaultState) { }
        protected APIV2_MenuHeader(GameObject original, Transform parent, string name, bool defaultState = true) : base(original, parent, name, defaultState) { }
    }
    public class APIV2_MenuButtonContainer : APIV2_MenuElement
    {
        private static GameObject cached_containerprefab;
        private static GameObject ContainerPrefab
        {
            get
            {
                if (cached_containerprefab == null)
                {
                    cached_containerprefab = NewQuickMenu.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Dashboard/ScrollRect").GetComponent<ScrollRect>().content
                        .Find("Buttons_QuickActions").gameObject;
                }
                return cached_containerprefab;
            }
        }

        public APIV2_MenuButtonContainer(string name, Transform parent = null) : base(ContainerPrefab, (parent == null ? ContainerPrefab.transform.parent : parent), $"Buttons_{name}")
        {
            foreach (var obj in RectTransform)
            {
                var control = obj.Cast<Transform>();

                if (control == null)
                    continue;

                Object.Destroy(control.gameObject);
            }
            var gridLayout = GameObject.GetComponent<GridLayoutGroup>();
            gridLayout.childAlignment = TextAnchor.UpperLeft;
            gridLayout.padding.top = 8;
            gridLayout.padding.left = 64;
        }
    }

    public class APIV2_MenuCategory : APIV2_ButtonPageInterface
    {
        public APIV2_MenuHeader Header;
        private readonly APIV2_MenuButtonContainer _buttonContainer;

        private readonly List<APIV2_MenuPage> _subMenuPages = new List<APIV2_MenuPage>();
        private readonly List<APIV2_CategoryPage> _subCategoryPages = new List<APIV2_CategoryPage>();

        public string Name { get; }

        public string Title
        {
            get => Header.Title;
            set => Header.Title = value;
        }

        public bool Active
        {
            get => _buttonContainer.GameObject.activeInHierarchy;
            set
            {
                Header.Active = value;
                _buttonContainer.Active = value;
            }
        }

        public APIV2_MenuCategory(string title, Transform parent = null)
        {
            Name = APIV2_MenuElement.CleanName(title);
            Header = new APIV2_MenuHeader(title, parent);
            _buttonContainer = new APIV2_MenuButtonContainer(Name, parent);
        }

        public APIV2_MenuButton AddSpacer(Sprite sprite = null)
        {
            var spacer = AddButton(string.Empty, string.Empty, null, sprite);
            spacer.GameObject.name = "Button_Spacer";
            spacer.Background.gameObject.SetActive(false);
            return spacer;
        }
        public APIV2_MenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null)
        {
            var button = new APIV2_MenuButton(text, tooltip, onClick, _buttonContainer.RectTransform, sprite);
            return button;
        }

        public APIV2_ToggleButton AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false)
        {
            var toggle = new APIV2_ToggleButton(text, tooltip, onToggle, _buttonContainer.RectTransform, defaultValue);
            return toggle;
        }

        /*public APIV2_ToggleButton AddToggle(string text, string tooltip, bool configValue)
        {
            var toggle = new APIV2_ToggleButton(text, tooltip, configValue, _buttonContainer.RectTransform, configValue);
            return toggle;
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

        public RectTransform RectTransform => _buttonContainer.RectTransform;

        public APIV2_MenuPage GetMenuPage(string name)
        {
            return _subMenuPages.FirstOrDefault(m => m.Name == APIV2_MenuElement.CleanName($"Menu_{name}"));
        }

        public APIV2_CategoryPage GetCategoryPage(string name)
        {
            return _subCategoryPages.FirstOrDefault(m => m.Name == APIV2_MenuElement.CleanName($"Menu_{name}"));
        }
    }
}
