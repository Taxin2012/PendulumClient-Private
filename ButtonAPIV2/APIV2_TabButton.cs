using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements.Controls;

using MenuTab = MonoBehaviourPublicStInBoGaObObUnique;

namespace PendulumClient.ButtonAPIV2
{
    public class APIV2_TabButton : APIV2_MenuElement
    {
        private static GameObject cached_tabbuttonprefab;
        private static GameObject TabButtonPrefab
        {
            get
            {
                if (cached_tabbuttonprefab == null)
                {
                    cached_tabbuttonprefab = NewQuickMenu.Instance.field_Public_GameObject_5;
                }
                return cached_tabbuttonprefab;
            }
        }

        protected APIV2_TabButton(string name, string tooltip, string pageName, Sprite sprite) : base(TabButtonPrefab, TabButtonPrefab.transform.parent, $"Page_{name}")
        {
            if (GameObject.GetComponent<MonoBehaviourPublicIPointerClickHandlerIEventSystemHandlerVoPoOnVoPoVoPoVoPoVo0>() != null)
            {
                UnityEngine.Object.DestroyImmediate(GameObject.GetComponent<MonoBehaviourPublicIPointerClickHandlerIEventSystemHandlerVoPoOnVoPoVoPoVoPoVo0>()); //double click camera
            }

            var menuTab = GameObject.transform.GetComponent<MenuTab>();
            menuTab.field_Public_String_0 = "PendulumClientPage_" + pageName;
            menuTab.field_Private_MenuStateController_0 = NewQuickMenu.Instance.field_Protected_MenuStateController_0;

            var button = GameObject.GetComponent<Button>();
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(new Action(menuTab.ShowTabContent));

            var uiTooltip = GameObject.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>();
            uiTooltip.field_Public_String_0 = tooltip;
            uiTooltip.field_Public_String_1 = tooltip;

            var iconImage = RectTransform.Find("Icon").GetComponent<Image>();
            iconImage.sprite = sprite;
            iconImage.overrideSprite = sprite;
        }

        public static APIV2_TabButton Create(string name, string tooltip, string pageName, Sprite sprite)
        {
            return new APIV2_TabButton(name, tooltip, pageName, sprite);
        }
    }
}
