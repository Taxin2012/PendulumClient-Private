﻿using System;
using PendulumClient.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Core.Styles;
using Object = UnityEngine.Object;

namespace PendulumClient.ButtonAPIV2
{
    public class APIV2_MenuButton : APIV2_MenuElement
    {
        private static GameObject cached_buttonprefab;

        private static GameObject ButtonPrefab
        {
            get
            {
                if (cached_buttonprefab == null)
                {
                    cached_buttonprefab = NewQuickMenu.Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Dashboard/ScrollRect").GetComponent<ScrollRect>().content
                        .Find("Buttons_QuickActions/Button_Respawn").gameObject;
                }
                return cached_buttonprefab;
            }
        }

        private readonly TextMeshProUGUI _text;

        public string Text
        {
            get => _text.text;
            set => _text.SetText(value);
        }

        private readonly Button _button;
        public bool Interactable
        {
            get => _button.interactable;
            set => _button.interactable = value;
        }

        public APIV2_MenuButton(string text, string tooltip, Action onClick, Transform parent, Sprite sprite = null) : base(ButtonPrefab, parent,
            $"Button_{text}")
        {
            _text = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            _text.text = text;
            _text.richText = true;
            if (sprite == null)
            {
                _text.fontSize = 35;
                _text.enableAutoSizing = true;
                _text.color = ColorModule.ColorModule.CachedTextColor;
                _text.m_fontColor = ColorModule.ColorModule.CachedTextColor;
                _text.m_htmlColor = ColorModule.ColorModule.CachedTextColor;
                _text.margin = new Vector4(20f, 0f, 20f, 0f);
                //_text.color = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
                //_text.m_fontColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
                //_text.m_htmlColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
                _text.transform.localPosition = new Vector3(_text.transform.localPosition.x, -30f);

                var layoutElement = RectTransform.Find("Background").gameObject.AddComponent<LayoutElement>();
                layoutElement.ignoreLayout = true;

                var horizontalLayout = GameObject.AddComponent<HorizontalLayoutGroup>();
                horizontalLayout.padding.right = 10;
                horizontalLayout.padding.left = 10;
                var styleElement = _text.GetComponent<StyleElement>();
                styleElement.field_Public_String_1 = "H1";
                Object.DestroyImmediate(RectTransform.Find("Icon").gameObject);
            }
            else
            {
                var iconImage = RectTransform.Find("Icon").GetComponent<Image>();
                iconImage.sprite = sprite;
                iconImage.overrideSprite = sprite;
            }

            Object.DestroyImmediate(RectTransform.Find("Icon_Secondary").gameObject);
            Object.DestroyImmediate(RectTransform.Find("Badge_Close").gameObject);
            Object.DestroyImmediate(RectTransform.Find("Badge_MMJump").gameObject);

            var uiTooltip = GameObject.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>();
            uiTooltip.field_Public_String_0 = tooltip;
            uiTooltip.field_Public_String_1 = tooltip;

            _button = GameObject.GetComponent<Button>();
            _button.onClick = new Button.ButtonClickedEvent();
            _button.onClick.AddListener(new Action(onClick));
        }

        public static APIV2_MenuButton Create(string text, string tooltip, Action onClick, Transform parent, Sprite sprite = null)
        {
            return new APIV2_MenuButton(text, tooltip, onClick, parent, sprite);
        }
    }
}
