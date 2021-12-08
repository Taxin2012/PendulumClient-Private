using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnhollowerRuntimeLib;
using Object = UnityEngine.Object;
using PendulumClient.Wrapper;

namespace PendulumClient.ButtonAPIInc
{
    public static class ButtonAPI
    {
        public static Transform InstantiateGameobject(string type)
        {
            var quickMenu = Wrappers.GetQuickMenu();
            switch(type)
            {
                default:
                    return Object.Instantiate<GameObject>(Wrappers.GetVRCUiPageManager().transform.Find("MenuContent/Screens/Settings/AudioDevicePanel/LevelText").gameObject).transform;
                case "back":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("CameraMenu/BackButton").gameObject).transform;
                case "nameplates":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("UIElementsMenu/ToggleNameplatesButton").gameObject).transform;
                case "block1":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("NotificationInteractMenu/BlockButton").gameObject).transform;
                case "next":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("QuickMenu_NewElements/_CONTEXT/QM_Context_User_Selected/NextArrow_Button").gameObject).transform;
                case "prev":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("QuickMenu_NewElements/_CONTEXT/QM_Context_User_Selected/PreviousArrow_Button").gameObject).transform;
                case "emojimenu":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("EmojiMenu").gameObject).transform;
                case "userinteractmenu":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("UserInteractMenu").gameObject).transform;
                case "block":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("UserInteractMenu/BlockButton").gameObject).transform;
                case "menu":
                    return Object.Instantiate<GameObject>(quickMenu.transform.Find("CameraMenu").gameObject).transform;
            }
        }
        private static string ConvertToType(this ButtonType type)
        {
            switch(type)
            {
                case ButtonType.Default:
                    return "back";
                case ButtonType.Toggle:
                    return "block";
                case ButtonType.Menu:
                    return "menu";
                case ButtonType.Social:
                    return "social";
                case ButtonType.Text:
                    return "text";

            }
            return "block";
        }
        public static GameObject CreateButton(ButtonType type, string text, string tooltip, Color textColor, Color backgroundColor, float x_pos, float y_pos, Transform parent, Action listener = null, Action SecondListener = null)
        {
            return null;

            Transform transform = InstantiateGameobject(type.ConvertToType());
            var quickMenu = Wrappers.GetQuickMenu();

            float num = quickMenu.transform.Find("CameraMenu/BackButton").localPosition.x;
            float num2 = quickMenu.transform.Find("CameraMenu/BackButton").localPosition.y;

            //transform.localPosition = new Vector3(num, num2, 0f);
            if (type == ButtonType.Toggle || type == ButtonType.EnabledToggle)
            {
                if (parent.gameObject.name == "ShortcutMenu")
                {
                    transform.localPosition += new Vector3(x_pos * 315f, (y_pos + 1f) * 420f, 0f);
                }
                else
                {
                    transform.localPosition += new Vector3((x_pos + 1f) * 420f, (y_pos + 1f) * 420f, 0f);
                }
            }
            else
            {
                transform.localPosition += new Vector3(x_pos * 420f, y_pos * 420f, 0f);
            }

            transform.SetParent(parent, false);

            switch(type)
            {
                case ButtonType.Toggle:
                    var EnableButton = transform.Find("Toggle_States_Visible/OFF").gameObject;
                    var DisableButton = transform.Find("Toggle_States_Visible/ON").gameObject;

                    if (text == "Moderations" && tooltip == "Toggle PCM on and off")
                    {
                        EnableButton.GetComponentsInChildren<Text>()[0].text = $"{text} ON";
                        DisableButton.GetComponentsInChildren<Text>()[0].text = $"{text} ON";
                        var fontSize = EnableButton.GetComponentsInChildren<Text>()[0].fontSize;

                        EnableButton.GetComponentsInChildren<Text>()[1].text = $"{text} OFF";
                        DisableButton.GetComponentsInChildren<Text>()[1].text = $"{text} OFF";
                    }
                    else
                    {
                        EnableButton.GetComponentsInChildren<Text>()[0].text = $"{text} OFF";
                        DisableButton.GetComponentsInChildren<Text>()[0].text = $"{text} OFF";
                        var fontSize = EnableButton.GetComponentsInChildren<Text>()[0].fontSize;

                        EnableButton.GetComponentsInChildren<Text>()[1].text = $"{text} ON";
                        DisableButton.GetComponentsInChildren<Text>()[1].text = $"{text} ON";
                    }
                    EnableButton.GetComponentsInChildren<Text>()[1].resizeTextForBestFit = true;
                    DisableButton.GetComponentsInChildren<Text>()[1].resizeTextForBestFit = true;
                    EnableButton.GetComponentsInChildren<Text>()[1].resizeTextMaxSize = 256;
                    DisableButton.GetComponentsInChildren<Text>()[1].resizeTextMaxSize = 256;

                    EnableButton.GetComponentInChildren<Image>().color = backgroundColor;
                    DisableButton.GetComponentInChildren<Image>().color = backgroundColor;

                    transform.transform.GetComponent<UiTooltip>().field_Public_String_0 = tooltip;
                    transform.transform.GetComponent<UiTooltip>().field_Public_String_1 = tooltip;

                    transform.transform.GetComponent<UiTooltip>().SetToolTipBasedOnToggle();

                    transform.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();

                    transform.GetComponent<Button>().onClick.AddListener(new Action(() =>
                    {
                        if (EnableButton.activeSelf)
                        {
                            SecondListener.Invoke();
                            EnableButton.SetActive(false);
                            DisableButton.SetActive(true);
                        }
                        else
                        {
                            listener.Invoke();
                            DisableButton.SetActive(false);
                            EnableButton.SetActive(true);
                        }
                    }));
                    break;
                case ButtonType.EnabledToggle:
                    var EnableButton2 = transform.Find("Toggle_States_Visible/OFF").gameObject;
                    var DisableButton2 = transform.Find("Toggle_States_Visible/ON").gameObject;

                    EnableButton2.GetComponentsInChildren<Text>()[0].text = $"{text} OFF";
                    DisableButton2.GetComponentsInChildren<Text>()[0].text = $"{text} OFF";
                    var fontSize2 = EnableButton2.GetComponentsInChildren<Text>()[0].fontSize;

                    EnableButton2.GetComponentsInChildren<Text>()[1].text = $"{text} ON";
                    DisableButton2.GetComponentsInChildren<Text>()[1].text = $"{text} ON";
                    EnableButton2.GetComponentsInChildren<Text>()[1].resizeTextForBestFit = true;
                    DisableButton2.GetComponentsInChildren<Text>()[1].resizeTextForBestFit = true;
                    EnableButton2.GetComponentsInChildren<Text>()[1].resizeTextMaxSize = 256;
                    DisableButton2.GetComponentsInChildren<Text>()[1].resizeTextMaxSize = 256;

                    EnableButton2.GetComponentInChildren<Image>().color = backgroundColor;
                    DisableButton2.GetComponentInChildren<Image>().color = backgroundColor;

                    EnableButton2.SetActive(true);
                    DisableButton2.SetActive(false);
                    listener.Invoke();

                    transform.transform.GetComponent<UiTooltip>().field_Public_String_0 = tooltip;
                    transform.transform.GetComponent<UiTooltip>().field_Public_String_1 = tooltip;

                    transform.transform.GetComponent<UiTooltip>().SetToolTipBasedOnToggle();

                    transform.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();

                    transform.GetComponent<Button>().onClick.AddListener(new Action(() =>
                    {
                        if (EnableButton2.activeSelf)
                        {
                            SecondListener.Invoke();
                            EnableButton2.SetActive(false);
                            DisableButton2.SetActive(true);
                        }
                        else
                        {
                            listener.Invoke();
                            DisableButton2.SetActive(false);
                            EnableButton2.SetActive(true);
                        }
                    }));
                    break;
                case ButtonType.Text:
                    transform.GetComponentInChildren<Text>().text = text;
                    transform.GetComponentInChildren<UiTooltip>().field_Public_String_0 = tooltip;
                    transform.GetComponentInChildren<Text>().color = textColor;
                    transform.GetComponentInChildren<Image>().color = backgroundColor;
                    transform.GetComponentInChildren<Image>().enabled = false;

                    transform.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                    transform.GetComponent<Button>().onClick.AddListener(listener);
                    break;

                case ButtonType.Default:
                    transform.GetComponentInChildren<Text>().text = text;
                    transform.GetComponentInChildren<UiTooltip>().field_Public_String_0 = tooltip;
                    transform.GetComponentInChildren<Text>().color = textColor;
                    transform.GetComponentInChildren<Image>().color = backgroundColor;

                    transform.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                    transform.GetComponent<Button>().onClick.AddListener(listener);
                    break;

                case ButtonType.Menu:
                    break; //to-do, menu support
            }
            return transform.gameObject;
        }
        public static Text make_slider(Transform parent, Action<float> act, float bx, float by, string text, float def, float max, float min, int negate)
        {
            var btn = CreateButton(ButtonType.Default, "slider_element_" + bx + by, "", Color.white, Color.white, bx, by, parent, null); 
            btn.SetActive(false);
            var slider = UnityEngine.Object.Instantiate<Transform>(Wrappers.GetVRCUiManager().field_Public_GameObject_0.transform.Find("Screens/Settings/AudioDevicePanel/VolumeSlider"), parent);
            slider.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            slider.transform.localPosition = btn.gameObject.transform.localPosition; 
            slider.transform.localPosition -= new Vector3(0, negate);
            slider.GetComponentInChildren<RectTransform>().anchorMin += new Vector2(0.06f, 0f);
            slider.GetComponentInChildren<RectTransform>().anchorMax += new Vector2(0.1f, 0f);
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().onValueChanged = new UnityEngine.UI.Slider.SliderEvent();
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().value = def;
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = max;
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().minValue = min;
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().onValueChanged.AddListener(DelegateSupport.ConvertDelegate<UnityAction<float>>(act));
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().UpdateVisuals();
            var txt = new GameObject("Text"); txt.transform.SetParent(parent, false);
            GameObject.Destroy(slider.transform.Find("Fill Area/Label").gameObject);
            slider.transform.Find("Fill Area/Fill").gameObject.GetComponent<Image>().color = new Color(ColorModule.ColorModule.CachedColor.r, ColorModule.ColorModule.CachedColor.g, ColorModule.ColorModule.CachedColor.b, 1f);
            var txt_component = txt.AddComponent<Text>();
            txt_component.font = Resources.GetBuiltinResource<Font>("Arial.ttf"); 
            txt_component.fontSize = 64; 
            txt_component.text = text;
            txt_component.transform.localPosition = slider.transform.localPosition;
            txt_component.transform.localPosition += new Vector3(txt_component.fontSize * text.Count() / 3.75f, 75);
            txt_component.enabled = true;
            txt_component.GetComponent<RectTransform>().sizeDelta = new Vector2(txt_component.fontSize * text.Count(), 100);
            txt_component.alignment = TextAnchor.MiddleLeft;
            return txt_component;
        }

        public static Text make_slider_int(Transform parent, Action<float> act, float bx, float by, string text, int def, int max, int min, int negate)
        {
            var btn = CreateButton(ButtonType.Default, "slider_element_" + bx + by, "", Color.white, Color.white, bx, by, parent, null);
            btn.SetActive(false);
            var slider = UnityEngine.Object.Instantiate<Transform>(Wrappers.GetVRCUiManager().field_Public_GameObject_0.transform.Find("Screens/Settings/AudioDevicePanel/VolumeSlider"), parent);
            slider.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            slider.transform.localPosition = btn.gameObject.transform.localPosition;
            slider.transform.localPosition -= new Vector3(0, negate);
            slider.GetComponentInChildren<RectTransform>().anchorMin += new Vector2(0.06f, 0f);
            slider.GetComponentInChildren<RectTransform>().anchorMax += new Vector2(0.1f, 0f);
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().onValueChanged = new UnityEngine.UI.Slider.SliderEvent();
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().value = def;
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().maxValue = max;
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().minValue = min;
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().wholeNumbers = true;
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().onValueChanged.AddListener(DelegateSupport.ConvertDelegate<UnityAction<float>>(act));
            slider.GetComponentInChildren<UnityEngine.UI.Slider>().UpdateVisuals();
            GameObject.Destroy(slider.transform.Find("Fill Area/Label").gameObject);
            slider.transform.Find("Fill Area/Fill").gameObject.GetComponent<Image>().color = new Color(ColorModule.ColorModule.CachedColor.r, ColorModule.ColorModule.CachedColor.g, ColorModule.ColorModule.CachedColor.b, 1f);
            var txt = new GameObject("Text"); txt.transform.SetParent(parent, false);
            var txt_component = txt.AddComponent<Text>();
            txt_component.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            txt_component.fontSize = 64;
            txt_component.text = text;
            txt_component.transform.localPosition = slider.transform.localPosition;
            txt_component.transform.localPosition += new Vector3(txt_component.fontSize * text.Count() / 3.75f, 75);
            txt_component.enabled = true;
            txt_component.GetComponent<RectTransform>().sizeDelta = new Vector2(txt_component.fontSize * text.Count(), 100);
            txt_component.alignment = TextAnchor.MiddleLeft;
            return txt_component;
        }
    }
}
