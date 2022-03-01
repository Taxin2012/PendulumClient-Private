using System;
using PendulumClient.UI;
using PendulumClient.UnityExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using ToggleIcon = MonoBehaviourPublicToAwVoBoVoOnBoVoBoVoUnique;//VRC.UI.Elements.Controls.ToggleIcon;

namespace PendulumClient.ButtonAPIV2
{
    public class APIV2_ToggleButton : APIV2_MenuElement
    {
        private readonly Toggle _toggleComponent;
        private readonly ToggleIcon _toggleIcon;

        public bool Interactable
        {
            get => _toggleComponent.interactable;
            set => _toggleComponent.interactable = value;
        }

        private bool _valueHolder;

        public APIV2_ToggleButton(string text, string tooltip, Action<bool> onToggle, Transform parent, bool defaultvalue = false) : base(NewQuickMenu.TogglePrefab, parent, $"Button_Toggle{text}")
        {
            var iconOn = RectTransform.Find("Icon_On").GetComponent<Image>();
            iconOn.sprite = NewQuickMenu.OnIconSprite;

            _toggleIcon = GameObject.GetComponent<ToggleIcon>();

            var tmp = GameObject.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = text;
            tmp.richText = true;
            tmp.color = ColorModule.ColorModule.CachedTextColor;
            tmp.m_fontColor = ColorModule.ColorModule.CachedTextColor;
            tmp.m_htmlColor = ColorModule.ColorModule.CachedTextColor;
            //tmp.color = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
            //tmp.m_fontColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);
            //tmp.m_htmlColor = new Color(0.4157f, 0.8902f, 0.9765f, 1f);

            var uiTooltip = GameObject.GetComponent<VRC.UI.Elements.Tooltips.UiToggleTooltip>();
            uiTooltip.field_Public_String_0 = tooltip;
            uiTooltip.field_Public_String_1 = tooltip;

            _toggleComponent = GameObject.GetComponent<Toggle>();
            _toggleComponent.onValueChanged = new Toggle.ToggleEvent();
            _toggleComponent.onValueChanged.AddListener(new Action<bool>(_toggleIcon.OnValueChanged));
            _toggleComponent.onValueChanged.AddListener(new Action<bool>(onToggle));
            _toggleIcon.field_Public_Toggle_0 = _toggleComponent;
            uiTooltip.field_Public_Toggle_0 = _toggleComponent;

            Toggle(defaultvalue, false);

            if (defaultvalue)
            {
                onToggle.Invoke(true);
                _toggleIcon.OnValueChanged(true);
            }

            /*EnableDisableListener.RegisterSafe();
            var listener = GameObject.AddComponent<EnableDisableListener>();
            listener.OnEnableEvent += UpdateToggleIfNeeded;*/
        }


        public void Toggle(bool value, bool callback = true, bool updateVisually = false)
        {
            _valueHolder = value;
            _toggleComponent.Set(value, callback);
            if (updateVisually)
            {
                UpdateToggleIfNeeded();
            }
        }

        private void UpdateToggleIfNeeded()
        {
            _toggleIcon.OnValueChanged(_valueHolder);
        }
    }
}
