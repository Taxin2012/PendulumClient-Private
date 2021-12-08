using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;

using Object = UnityEngine.Object;

namespace PendulumClient.ButtonAPIV2
{
    class NewQuickMenu
    {
        private static VRC.UI.Elements.QuickMenu cached_quickmenuinstance;
        public static VRC.UI.Elements.QuickMenu Instance
        {
            get
            {
                if (cached_quickmenuinstance == null)
                {
                    cached_quickmenuinstance = Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>();
                }
                return cached_quickmenuinstance;
            }
        }
        private static Sprite cached_oniconsprite;
        public static Sprite OnIconSprite
        {
            get
            {
                if (cached_oniconsprite == null)
                {
                    cached_oniconsprite = Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Notifications/Panel_NoNotifications_Message/Icon").GetComponent<Image>().sprite;
                }
                return cached_oniconsprite;
            }
        }
        private static GameObject cached_toggleprefab;
        public static GameObject TogglePrefab
        {
            get
            {
                if (cached_toggleprefab == null)
                {
                    cached_toggleprefab = Instance.field_Public_Transform_0
                        .Find("Window/QMParent/Menu_Settings/Panel_QM_ScrollRect").GetComponent<ScrollRect>().content
                        .Find("Buttons_UI_Elements_Row_1/Button_ToggleQMInfo").gameObject;
                }
                return cached_toggleprefab;
            }
        }
    }
}
