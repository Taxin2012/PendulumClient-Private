using System;
using UnityEngine;

namespace PendulumClient.ButtonAPIV2
{
    public interface APIV2_ButtonPageInterface
    {
        APIV2_MenuButton AddButton(string text, string tooltip, Action onClick, Sprite sprite = null);
        APIV2_ToggleButton AddToggle(string text, string tooltip, Action<bool> onToggle, bool defaultValue = false);
        //APIV2_ToggleButton AddToggle(string text, string tooltip, bool configValue);
        APIV2_MenuPage AddMenuPage(string text, string tooltip = "", Sprite sprite = null);
        APIV2_CategoryPage AddCategoryPage(string text, string tooltip = "", Sprite sprite = null);
        APIV2_MenuPage GetMenuPage(string name);
        APIV2_CategoryPage GetCategoryPage(string name);
    }
}
