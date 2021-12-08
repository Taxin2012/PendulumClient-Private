using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MelonLoader;
using UnityStandardAssets.Effects;
using PendulumClient.Main;
using UnhollowerRuntimeLib;

namespace PendulumClient.ColorModule
{
    public static class ColorModule
    {

        //Credits: Meep#0231

        private static IList<Text> CachedText;

        private static IList<Transform> CachedTransforms;

        public static Color CachedColor = new Color(ModPrefs.GetFloat(ColorSettings.SettingsCategory, ColorSettings.ColorR), ModPrefs.GetFloat(ColorSettings.SettingsCategory, ColorSettings.ColorG), ModPrefs.GetFloat(ColorSettings.SettingsCategory, ColorSettings.ColorB), ModPrefs.GetFloat(ColorSettings.SettingsCategory, ColorSettings.ColorA));

        public static Color CachedTextColor = new Color(1f, 1f, 1f, 1f);

        public static Color DefaultMenuColorInverted = new Color(0.9468627451f, 0.64490196078f, 0.62921568627f, 1f);

        public static Sprite PendClientSprite;

        /*public static void Update()
        {
            if (!Initialised)
            {
                if (Wrappers.GetQuickMenu() != null)
                {
                    CleanupUI();
                    SetColorTheme(Color.green, Color.white, 0.5f);
                    Initialised = true;
                }
            }
        }
        public static void Start()
        {
            new Thread(() =>
            {
                for (; ; )
                {
                    Thread.Sleep(5000);
                    Update();
                }
            }).Start();
        }*/

        public static string ColorToHex(Color c)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
        }
        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }

        public static void CleanupUI()
        {
            Color textColor = CachedColor;
            GameObject gameObject = GameObject.Find("MenuContent");
            if (ColorModule.CachedTransforms == null) ColorModule.CachedTransforms = gameObject.GetComponentsInChildren<Transform>(true);
            if (ColorModule.CachedText == null) ColorModule.CachedText = gameObject.GetComponentsInChildren<Text>(true);

            gameObject.transform.Find("Screens").Find("Worlds").Find("Vertical Scroll View").GetComponentsInChildren<Text>().ToList<Text>().ForEach(delegate (Text x)
            {
                x.color = textColor;
            });
            gameObject.transform.Find("Screens").Find("Avatar").Find("Vertical Scroll View").GetComponentsInChildren<Text>().ToList<Text>().ForEach(delegate (Text x)
            {
                x.color = textColor;
            });
            for (int i = 0; i < ColorModule.CachedText.Count; i++)
            {
                Color color = CachedTextColor;
                Color color2 = CachedColor;
                Color clear = Color.clear;
                if (ColorModule.CachedText[i].color == color || ColorModule.CachedText[i].color == color2 || ColorModule.CachedText[i].color == clear || ColorModule.CachedText[i].text == "You Are In" || ColorModule.CachedText[i].text == "Edit Status")
                {
                    ColorModule.CachedText[i].color = textColor;
                }
            }
            for (int j = 0; j < ColorModule.CachedTransforms.Count; j++)
            {
                Transform transform = ColorModule.CachedTransforms[j];
                Text text = (transform != null) ? transform.GetComponent<Text>() : null;
                if (text != null)
                {
                    text.color = textColor;
                }
            }
        }

        public static void SetColorSlider(float colorHue, float colorSaturation, float colorValue, float textColorHue, float textColorSaturation, float textColorValue, float opac)
        {
            Color color = Color.HSVToRGB(colorHue, colorSaturation, colorValue);
            Color textColor = Color.HSVToRGB(textColorHue, textColorSaturation, textColorValue);
            SetColorTheme();
        }

        internal static void ColorOfTypeIfExists(GameObject parent, string contains, Color clr, float opacityToUse, bool useImg)
        {
            clr.a = opacityToUse;
            (from x in parent.GetComponentsInChildren<Transform>(true)
             where x.name.Contains(contains)
             select x).ToList<Transform>().ForEach(delegate (Transform x)
             {
                 if (useImg)
                 {
                     Image image = (x != null) ? x.GetComponent<Image>() : null;
                     if (image != null)
                     {
                         image.color = clr;
                         return;
                     }
                 }
                 else
                 {
                     Text text = (x != null) ? x.GetComponent<Text>() : null;
                     if (text != null)
                     {
                         text.color = clr;
                     }
                 }
             });
            clr.a = 1f;
            //Credits: Magic3000
        }


        public static void SetColorThemeV2()
        {
            PendulumClientMain.UIColorsSetup = true;

            Color color = CachedColor;
            Color color2 = CachedTextColor;
            Color color3 = DefaultMenuColorInverted;
            ColorBlock colorBlock = default(ColorBlock);
            colorBlock.colorMultiplier = 1f;
            colorBlock.disabledColor = Color.grey;
            colorBlock.highlightedColor = color * 1.5f;
            colorBlock.normalColor = color / 1.5f;
            colorBlock.pressedColor = Color.grey * 1.5f;
            colorBlock.fadeDuration = 0.1f;
            ColorBlock colors = colorBlock;

            

            GameObject.Find("_Application/CursorManager/MouseArrow/VRCUICursorIcon").GetComponent<VRC.UI.CursorIcon>().field_Public_SpriteRenderer_0.color = color;

            var gameObject1 = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)");
            var QMStyleEngine = gameObject1.GetComponent<VRC.UI.Core.Styles.StyleEngine>().field_Private_List_1_ElementStyle_0;

            /*try
            {
                foreach (VRC.UI.Core.Styles.StyleElement Element in gameObject1.GetComponentsInChildren<VRC.UI.Core.Styles.StyleElement>(true))
                {
                    if (Element.field_Private_Selectable_0 != null) Element.field_Private_Selectable_0.colors = colors;
                }
            }
            catch (Exception e)
            {
                PendulumLogger.LogError("MenuColor Error: " + e);
            }

            //PendulumLogger.Log("ObjName" + GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickLinks/Button_Worlds").name);//.GetComponent<VRC.UI.Core.Styles.StyleElement>().field_Private_Selectable_0.colors = colors;
            */
            /*try
            {
                foreach (Button button2 in gameObject1.GetComponentsInChildren<Button>(true))
                {
                    if (button2.gameObject != null && button2.transform != null)
                    {
                        if (button2.gameObject.name != "rColorButton" && button2.gameObject.name != "gColorButton" && button2.gameObject.name != "bColorButton" && button2.gameObject.name != "ColorPickPreviewButton")// && button2.transform.parent.name != "Menu_Notifications" && button2.transform.parent.parent.name != "Menu_QM_Emojis")
                        {
                            if (button2.colors != null || button2.colors != default(ColorBlock))
                            {
                                button2.colors = colors;
                            }
                        }
                        if (button2.transform.Find("Icon").gameObject != null)
                        {
                            if (button2.transform.Find("Icon").gameObject.GetComponent<Image>() != null)
                            {
                                button2.transform.Find("Icon").gameObject.GetComponent<Image>().color = color2;
                            }
                        }
                        if (button2.transform.Find("Text_H4").gameObject != null)
                        {
                            if (button2.transform.Find("Text_H4").gameObject.GetComponent<TMPro.TextMeshProUGUI>() != null)
                            {
                                button2.transform.Find("Text_H4").gameObject.GetComponent<TMPro.TextMeshProUGUI>().color = color2;
                            }
                        }
                        if (button2.transform.Find("Icon_On").gameObject != null)
                        {
                            if (button2.transform.Find("Icon_On").gameObject.GetComponent<Image>() != null)
                            {
                                button2.transform.Find("Icon_On").gameObject.GetComponent<Image>().color = color2;
                            }
                        }
                        if (button2.transform.Find("Icon_Off").gameObject != null)
                        {
                            if (button2.transform.Find("Icon_Off").gameObject.GetComponent<Image>() != null)
                            {
                                button2.transform.Find("Icon_Off").gameObject.GetComponent<Image>().color = color2;
                            }
                        }
                        if (button2.transform.parent.gameObject != null)
                        {
                            if (button2.transform.Find("Icon").gameObject != null && (button2.transform.parent.name == "Wing_Left" || button2.transform.parent.name == "Wing_Left"))
                            {
                                if (button2.transform.Find("Icon").gameObject.GetComponent<Image>() != null)
                                {
                                    button2.transform.Find("Icon").gameObject.GetComponent<Image>().color = color2;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PendulumLogger.LogError("MenuColor Error: " + e);
            }*/
            PendulumClientMain.UIColorsSetup = true;
        }
        public static void SetColorTheme()
        {
            Color color = CachedColor;
            Color color2 = CachedTextColor;
            Color color3 = DefaultMenuColorInverted;
            ColorBlock colorBlock = default(ColorBlock);
            colorBlock.colorMultiplier = 1f;
            colorBlock.disabledColor = Color.grey;
            colorBlock.highlightedColor = color * 1.5f;
            colorBlock.normalColor = color / 1.5f;
            colorBlock.pressedColor = Color.grey * 1.5f;
            colorBlock.fadeDuration = 0.1f;
            ColorBlock colors = colorBlock;
            GameObject gameObject = GameObject.Find("MenuContent");

            var PendSpriteReq = PendulumClientMain.myAssetBundle.LoadAssetAsync("pendclientsprite.png", Il2CppType.Of<Sprite>());
            PendClientSprite = PendSpriteReq.asset.Cast<Sprite>();
            try
            {
                gameObject.transform.Find("Backdrop/Backdrop/Background").GetComponent<Image>().color = new Color(color.r, color.g, color.b, color.a / 3f);
                gameObject.transform.Find("Screens/UserInfo/User Panel/PanelHeaderBackground").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Description_SafetyLevel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/ON").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/ON/TopPanel_SafetyLevel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/ON").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/ON/TopPanel_SafetyLevel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/ON").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/ON/TopPanel_SafetyLevel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Maxiumum/ON").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Maxiumum/ON/TopPanel_SafetyLevel").GetComponent<Image>().color = color;
                foreach (Text text in gameObject.transform.Find("Popups/InputPopup/Keyboard/Keys").GetComponentsInChildren<Text>(true))
                {
                    text.color = color;
                }
                foreach (Text text2 in gameObject.transform.Find("Popups/InputKeypadPopup/Keyboard/Keys").GetComponentsInChildren<Text>(true))
                {
                    text2.color = color;
                }
                gameObject.transform.Find("Popups/InputKeypadPopup/Rectangle").GetComponent<Image>().color = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f);
                gameObject.transform.Find("Popups/InputKeypadPopup/Rectangle/Panel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/InputKeypadPopup/InputField").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/StandardPopupV2/Popup/Panel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/StandardPopupV2/Popup/BorderImage").GetComponent<Image>().color = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f);
                gameObject.transform.Find("Popups/InputPopup/InputField").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/StandardPopup/Rectangle").GetComponent<Image>().color = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f);
                gameObject.transform.Find("Popups/StandardPopup/MidRing").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/StandardPopup/InnerDashRing").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/StandardPopup/RingGlow").GetComponent<Image>().color = Color.white;
                gameObject.transform.Find("Popups/UpdateStatusPopup/Popup/Panel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/UpdateStatusPopup/Popup/BorderImage").GetComponent<Image>().color = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f);
                gameObject.transform.Find("Popups/UpdateStatusPopup/Popup/InputFieldStatus").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/AdvancedSettingsPopup/Popup/Panel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/AdvancedSettingsPopup/Popup/BorderImage").GetComponent<Image>().color = color2;
                gameObject.transform.Find("Popups/AddToPlaylistPopup/Popup/Panel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/AddToPlaylistPopup/Popup/BorderImage").GetComponent<Image>().color = color2;
                gameObject.transform.Find("Popups/BookmarkFriendPopup/Popup/Panel (2)").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/BookmarkFriendPopup/Popup/BorderImage").GetComponent<Image>().color = color2;
                gameObject.transform.Find("Popups/EditPlaylistPopup/Popup/Panel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/EditPlaylistPopup/Popup/BorderImage").GetComponent<Image>().color = color2;
                gameObject.transform.Find("Popups/PerformanceSettingsPopup/Popup/Panel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/PerformanceSettingsPopup/Popup/BorderImage").GetComponent<Image>().color = color2;
                gameObject.transform.Find("Popups/AlertPopup/Lighter").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/RoomInstancePopup/Popup/Panel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/RoomInstancePopup/Popup/BorderImage").GetComponent<Image>().color = color2;
                gameObject.transform.Find("Popups/RoomInstancePopup/Popup/BorderImage (1)").GetComponent<Image>().color = color2;
                gameObject.transform.Find("Popups/ReportWorldPopup/Popup/Panel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/ReportWorldPopup/Popup/BorderImage").GetComponent<Image>().color = color2;
                gameObject.transform.Find("Popups/ReportUserPopup/Popup/Panel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/ReportUserPopup/Popup/BorderImage").GetComponent<Image>().color = color2;
                gameObject.transform.Find("Popups/SearchOptionsPopup/Popup/Panel (1)").GetComponent<Image>().color = color;
                gameObject.transform.Find("Popups/SearchOptionsPopup/Popup/BorderImage").GetComponent<Image>().color = color2;
                gameObject.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>().color = Color.white * 2f;
                gameObject.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>().color = Color.white * 2f;
                gameObject.transform.Find("Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>().color = Color.white * 2f;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked").GetComponentInChildren<MeshRenderer>().material.SetTexture("_Tex", new Texture2D(32, 32));
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked").GetComponentInChildren<MeshRenderer>().material.SetColor("_Tint", new Color(0f, 0f, 0f));
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponentInChildren<ParticleSystem>().startColor = new Color(1f, 1f, 1f, 0.5f);
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponentInChildren<ParticleSystem>().startSize = 0.1f;
                //GameObject.Find("UserInterface/MenuContent/Screens/Social/Current Status/StatusButton").GetComponent<Button>().colors = colors;
                /*GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/ICON/iconFrame").GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", new Texture2D(32, 32));
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/ICON/iconFrame").GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", color);
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainFrame").GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", new Texture2D(32, 32));
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/SCREEN/mainFrame").GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", color);
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/TITLE/titleFrame").GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", new Texture2D(32, 32));
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel/InfoPanel_Template_ANIM/TITLE/titleFrame").GetComponentInChildren<MeshRenderer>().material.SetColor("_Color", color);*/

                //GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingInfoPanel").gameObject.SetActive(false);
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/MirroredElements").gameObject.SetActive(false);
                //GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel").gameObject.transform.position += new Vector3(0f, 0.5f, 0.5f);
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>().sprite = new Sprite();
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>().color = color / 2f;
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_User_Hover/_ToolTipPanel/InfoIcon").GetComponent<Image>().sprite = PendClientSprite;
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_Invite/_ToolTipPanel/InfoIcon (1)").GetComponent<Image>().sprite = PendClientSprite;
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_ToolTip/_ToolTipPanel/InfoIcon").GetComponent<Image>().sprite = PendClientSprite;
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_User_Selected/_UserBio/InfoIcon").GetComponent<Image>().sprite = PendClientSprite;
                //GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>().material.shader = Shader.Find("Unlit/Color");
                //GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>().material.renderQueue = -2147483647;
                //GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").layer = -2147483647;
                //GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>().transform.localScale += new Vector3(0f, 4f, -1f);
                // GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel").GetComponentInChildren<BoxCollider>().size += new Vector3(0f, 2f, 0f);
                //GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/GoButton").gameObject.transform.position += new Vector3(0f, -0.5f, 0f);

                //bye bye vrc+ lol
                
                var VRCP1 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/HeaderContainer/VRCPlusBanner");
                var VRCP1Panel = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/HeaderContainer/VRCPlusBanner/Panel");
                var VRCP2 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/VRCPlusMiniBanner");
                var VRCP2Image = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/VRCPlusMiniBanner/Image");
                //Component.Destroy(VRCP1.GetComponent<TweenButton>());
                Component.Destroy(VRCP1.GetComponent<Button>());
                Component.Destroy(VRCP1.GetComponent<Collider>());
                Component.Destroy(VRCP1Panel.GetComponent<Image>());

                //Component.Destroy(VRCP2.GetComponent<TweenButton>());
                Component.Destroy(VRCP2.GetComponent<Button>());
                Component.Destroy(VRCP2.GetComponent<Collider>());
                Component.Destroy(VRCP2Image.GetComponent<Image>());

                //enable vrcat
                var catparent = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu").transform;
                var cat = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/VRCPlusThankYou");
                var catchar = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/VRCPlusThankYou/ThankYou Character");
                var gencat = GameObject.Instantiate(cat, catparent);
                gencat.name = "penis vagina";
                gencat.SetActiveRecursively(true);
                var catobj = gencat.transform.Find("ThankYou Character").gameObject;

                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("rob swire");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("buy pendulum client");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("what da dog doin");
                //catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("buy bread client");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("bruh moment");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add(":moyai:");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("corbin is a simp");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("shut up corbin");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("xaio paio");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("innit bruv");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("corbin is a simp");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("me when the");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("that moment when you");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("corbin is a simp");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("3 quid");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("ok newgen");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("venezuelans be like");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("you're going to brazil");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("bonjor");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("why you actin sus");
                catobj.GetComponent<VRCPlusThankYou>().field_Public_List_1_String_1.Add("Issues? ask the CEO");

                //make sure clock is ontop of cat cause rape
                PendulumClientMain.SetupMenuClock();

                //notifacations dont like this one
                //GameObject.Destroy(VRCP1);
                //GameObject.Destroy(VRCP2);

                var ReportAbuseButton = GameObject.Find("UserInterface/QuickMenu/UserInteractMenu/ReportAbuseButton");
                GameObject.DestroyImmediate(ReportAbuseButton);

                var bruhmome = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/VRC+PageTab/Button/Text");
                bruhmome.GetComponent<Text>().text = "VRC-";
                var bruhmome2 = GameObject.Find("UserInterface/MenuContent/Screens/VRC+/TitlePanel/TitleText");
                var bruhmomeP = GameObject.Find("UserInterface/MenuContent/Screens/VRC+/TitlePanel");
                bruhmome2.GetComponent<Text>().text = "VRChat Minus";
                var bruhmome3 = GameObject.Instantiate(bruhmome2, bruhmomeP.transform);
                bruhmome3.name = "lemon";
                bruhmome3.GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, -420f);
                bruhmome3.GetComponent<Text>().text = "NOBODY CARES NIGGA";
                
                var VRCP3 = GameObject.Find("UserInterface/MenuContent/Screens/VRC+/Subscription");
                GameObject.Destroy(VRCP3);

                var BruhAction = new System.Action(() =>
                {
                    PendulumClientMain.Errormsg("Bruh", "Bruh");
                });
                var TaC = GameObject.Find("UserInterface/MenuContent/Screens/VRC+/TitlePanel/TermsAndConditions");
                var TaCButton = TaC.GetComponent<Button>();
                TaCButton.onClick = new Button.ButtonClickedEvent();
                TaCButton.onClick.AddListener(BruhAction);

                /*var MainPanel = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel");
                var GoButtonText = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ButtonMiddle/Text");
                var ParentTransform = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/GoButton/Text").transform;
                GameObject MainText = GameObject.Instantiate(GoButtonText.gameObject, ParentTransform);
                GameObject CreatorText = GameObject.Instantiate(GoButtonText.gameObject, ParentTransform);

                MainText.name = "GeneratedText";
                MainText.GetComponent<Text>().text = "Welcome To Pendulum Client!";
                MainText.GetComponent<Text>().material.renderQueue = 2147483647;
                MainText.GetComponent<Text>().rectTransform.sizeDelta += new Vector2(100f, 0f);
                MainText.layer = 2147483647;
                MainText.transform.position += new Vector3(0f, 0.75f, 0f);
                MainText.SetActiveRecursively(true);
                MainText.transform.parent = MainPanel.transform;

                CreatorText.name = "CreatorTagText";
                CreatorText.GetComponent<Text>().text = "By Kyran and Corbinss";
                CreatorText.GetComponent<Text>().material.renderQueue = 2147483647;
                CreatorText.GetComponent<Text>().rectTransform.sizeDelta += new Vector2(100f, 0f);
                CreatorText.layer = 2147483647;
                CreatorText.transform.position += new Vector3(0f, 0.25f, 0f);
                CreatorText.SetActiveRecursively(true);
                CreatorText.transform.parent = MainPanel.transform;*/

                //System.Diagnostics.Debugger.IsAttached

                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_Lighting (1)/Point light").GetComponentInChildren<Light>().color = color / 2f;
                GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud/ReticleParent/Reticle").GetComponentInChildren<Image>().color = new Color(color.r, color.g, color.b, color.a * 2f);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/Panel_Header").GetComponentInChildren<Image>().color = new Color(color.r / 1.5f, color.g / 1.5f, color.b / 1.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/AudioDevicePanel/Panel_Header").GetComponentInChildren<Image>().color = new Color(color.r / 1.5f, color.g / 1.5f, color.b / 1.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/MousePanel/Panel_Header").GetComponentInChildren<Image>().color = new Color(color.r / 1.5f, color.g / 1.5f, color.b / 1.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/HeightPanel/Panel_Header").GetComponentInChildren<Image>().color = new Color(color.r / 1.5f, color.g / 1.5f, color.b / 1.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VoiceOptionsPanel/Panel_Header").GetComponentInChildren<Image>().color = new Color(color.r / 1.5f, color.g / 1.5f, color.b / 1.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/Panel_Header (1)").GetComponentInChildren<Image>().color = new Color(color.r / 1.5f, color.g / 1.5f, color.b / 1.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/Panel_Header Top").GetComponentInChildren<Image>().color = new Color(color.r / 1.5f, color.g / 1.5f, color.b / 1.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/Panel_Header Side").GetComponentInChildren<Image>().color = new Color(color.r / 2f, color.g / 2f, color.b / 2f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeMaster").GetComponent<Image>().color = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeUi").GetComponent<Image>().color = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameWorld").GetComponent<Image>().color = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameVoice").GetComponent<Image>().color = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameAvatars").GetComponent<Image>().color = new Color(color.r / 2.5f, color.g / 2.5f, color.b / 2.5f, color.a);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeMaster/FillArea/Fill").GetComponentInChildren<Image>().color = color2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeUi/FillArea/Fill").GetComponentInChildren<Image>().color = color2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameWorld/FillArea/Fill").GetComponentInChildren<Image>().color = color2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameVoice/FillArea/Fill").GetComponentInChildren<Image>().color = color2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameAvatars/FillArea/Fill").GetComponentInChildren<Image>().color = color2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeMaster/SliderLabel").GetComponentInChildren<Text>().color = color * 0f;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeUi/SliderLabel").GetComponentInChildren<Text>().color = color * 0f;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameWorld/SliderLabel").GetComponentInChildren<Text>().color = color * 0f;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameVoice/SliderLabel").GetComponentInChildren<Text>().color = color * 0f;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameAvatars/SliderLabel").GetComponentInChildren<Text>().color = color * 0f;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/txt_LOADING").GetComponentInChildren<Text>().color = color;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/txt_Percent").GetComponentInChildren<Text>().color = color;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/txt_LOADING_Size").GetComponentInChildren<Text>().color = color;
                //find_all_child_objects(GameObject.Find("_Application/CursorManager"));
                //GameObject.Find("_Application/CursorManager/BlueFireballMouse/Ball").GetComponent<ParticleSystem>().startColor = color;
                GameObject.Find("_Application/CursorManager/BlueFireballMouse/Ball").GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", color);
                //PendulumLogger.Log("Ball Shader: " + GameObject.Find("_Application/CursorManager/BlueFireballMouse/Ball").GetComponent<ParticleSystemRenderer>().material.shader.name);
                //GameObject.Find("_Application/CursorManager/BlueFireballMouse/Glow").GetComponent<ParticleSystem>().startColor = color;
                GameObject.Find("_Application/CursorManager/BlueFireballMouse/Glow").GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", color);
               // GameObject.Find("_Application/CursorManager/BlueFireballMouse/Trail").GetComponent<ParticleSystem>().startColor = color;
                GameObject.Find("_Application/CursorManager/BlueFireballMouse/Trail").GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", color);

                GameObject.Find("UserInterface/PlayerDisplay/BlackFade").GetComponent<VRCUiBackgroundFade>().field_Public_Boolean_0 = false;
            }
            catch (Exception ex)
            {
                PendulumLogger.Log("Error1: " + ex.ToString());
            }
            try
            {
                Transform transform = gameObject.transform.Find("Popups/InputPopup");
                color2.a = 0.8f;
                transform.Find("Rectangle").GetComponent<Image>().color = color;
                color2.a = 0.5f;
                color.a = 0.8f;
                transform.Find("Rectangle/Panel").GetComponent<Image>().color = color;
                color.a = 0.5f;
                Transform transform2 = gameObject.transform.Find("Backdrop/Header/Tabs/ViewPort/Content/Search");
                transform2.Find("SearchTitle").GetComponent<Text>().color = color;
                transform2.Find("InputField").GetComponent<Image>().color = color;
            }
            catch (Exception ex2)
            {
                PendulumLogger.Log("Error2: " + ex2.ToString());
            }
            try
            {
                colorBlock = default(ColorBlock);
                colorBlock.colorMultiplier = 2f;
                colorBlock.disabledColor = Color.grey;
                colorBlock.highlightedColor = color2;
                colorBlock.normalColor = color;
                colorBlock.pressedColor = Color.gray;
                colorBlock.fadeDuration = 0.1f;
                ColorBlock colors2 = colorBlock;

                ColorBlock colorBlock2 = default(ColorBlock);
                colorBlock2.colorMultiplier = 1f;
                colorBlock2.disabledColor = Color.grey;
                colorBlock2.highlightedColor = Color.white;
                colorBlock2.normalColor = Color.white;
                colorBlock2.pressedColor = Color.gray;
                colorBlock2.fadeDuration = 0.1f;
                ColorBlock colors3 = colorBlock2;

                gameObject.GetComponentsInChildren<Transform>(true).FirstOrDefault((Transform x) => x.name == "Row:4 Column:0").GetComponent<Button>().colors = colors;
                color2.a = 1f;
                colors2.normalColor = color2;
                foreach (Slider slider in gameObject.GetComponentsInChildren<Slider>(true))
                {
                    slider.colors = colors2;
                }
                color2.a = 0.5f;
                colors2.normalColor = color;
                foreach (Button button in gameObject.GetComponentsInChildren<Button>(true))
                {
                    button.colors = colors;
                }
                gameObject = GameObject.Find("QuickMenu");
                GameObject gameObject2 = GameObject.Find("MenuContent/Screens");
                foreach (Button button2 in gameObject.GetComponentsInChildren<Button>(true))
                {
                    if (button2.gameObject.name != "rColorButton" && button2.gameObject.name != "gColorButton" && button2.gameObject.name != "bColorButton" && button2.gameObject.name != "ColorPickPreviewButton" && button2.transform.parent.name != "SocialNotifications" && button2.transform.parent.parent.name != "EmojiMenu")
                    {
                        button2.colors = colors;
                    }
                }
                foreach (UiToggleButton uiToggleButton in gameObject.GetComponentsInChildren<UiToggleButton>(true))
                {
                    foreach (Image image in uiToggleButton.GetComponentsInChildren<Image>(true))
                    {
                        image.color = color;
                    }
                }
                foreach (Slider slider2 in gameObject.GetComponentsInChildren<Slider>(true))
                {
                    slider2.colors = colors2;
                    foreach (Image image2 in slider2.GetComponentsInChildren<Image>(true))
                    {
                        image2.color = color;
                    }
                }
                foreach (Toggle toggle in gameObject.GetComponentsInChildren<Toggle>(true))
                {
                    toggle.colors = colors2;
                    foreach (Image image3 in toggle.GetComponentsInChildren<Image>(true))
                    {
                        image3.color = color;
                    }
                }
                foreach (Text text in gameObject2.GetComponentsInChildren<Text>(true))
                {
                    text.color = Color.white;
                }

                Color SolidWhite = new Color(1f, 1f, 1f, 1f);
                Color TransparentWhite = new Color(1f, 1f, 1f, 0.33f);
                Color PopupBG = new Color(color.r / 2f, color.g / 2f, color.b / 2f, 1f);
                Color MenuBG = new Color(color.r / 2f, color.g / 2f, color.b / 2f, 0.15f);

                GameObject.Find("UserInterface/MenuContent/Popups/AdvancedSettingsPopup/Popup").GetComponentInChildren<Image>().color = PopupBG;
                GameObject.Find("UserInterface/MenuContent/Popups/PerformanceSettingsPopup/Popup").GetComponentInChildren<Image>().color = PopupBG;
                GameObject.Find("UserInterface/MenuContent/Popups/ReportWorldPopup/Popup/BorderImage").GetComponent<Image>().color = PopupBG;
                GameObject.Find("UserInterface/MenuContent/Popups/AddToPlaylistPopup/Popup/BorderImage").GetComponent<Image>().color = PopupBG;
                GameObject.Find("UserInterface/MenuContent/Popups/ReportUserPopup/Popup/BorderImage").GetComponent<Image>().color = PopupBG;
                GameObject.Find("UserInterface/MenuContent/Popups/BookmarkFriendPopup/Popup/BorderImage").GetComponent<Image>().color = PopupBG;
                GameObject.Find("UserInterface/MenuContent/Popups/EditPlaylistPopup/Popup/BorderImage").GetComponent<Image>().color = PopupBG;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_EditBindings").GetComponent<Button>().colors = colors3;
                //GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_AdvancedOptions/Image_NEW").transform.localScale = new Vector3(0f, 0f, 0f);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_EditBindings/Image_NEW").transform.localScale = new Vector3(0f, 0f, 0f);
                //GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/TitlePanel/Image").transform.localScale = new Vector3(0f, 0f, 0f);
                //GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/TitlePanel/Button_PerformanceOptions/Image_NEW").transform.localScale = new Vector3(0f, 0f, 0f);
                //GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/SafetyPageTab/Image (1)").transform.localScale = new Vector3(0f, 0f, 0f);
                //GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/User Panel/Playlists/PlaylistsButton/Image/Icon_New").transform.localScale = new Vector3(0f, 0f, 0f);
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_Background/Panel").GetComponent<Image>().sprite = new Sprite();
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_Background/Panel").GetComponent<Image>().color = MenuBG;
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_Background/Panel").transform.localScale += new Vector3(0.5f, 0f, 0f);

                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_User_Hover/Panel").GetComponent<Image>().sprite = new Sprite();
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_User_Hover/Panel").GetComponent<Image>().color = MenuBG;
                //GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_User_Hover/Panel").transform.localScale += new Vector3(0.5f, 0f, 0f);
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_User_Selected/Panel").GetComponent<Image>().sprite = new Sprite();
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_User_Selected/Panel").GetComponent<Image>().color = MenuBG;
                //GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_User_Selected/Panel").transform.localScale += new Vector3(0.5f, 0f, 0f);
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_ToolTip/Panel").GetComponent<Image>().sprite = new Sprite();
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_ToolTip/Panel").GetComponent<Image>().color = MenuBG;
                //GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_ToolTip/Panel").transform.localScale += new Vector3(0.5f, 0f, 0f);
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_Invite/Panel").GetComponent<Image>().sprite = new Sprite();
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_Invite/Panel").GetComponent<Image>().color = MenuBG;
                //GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_Invite/Panel").transform.localScale += new Vector3(0.5f, 0f, 0f);

                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar/Panel").GetComponent<Image>().sprite = new Sprite();
                GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar/Panel").GetComponent<Image>().color = MenuBG;
               // GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar/Panel").transform.localScale += new Vector3(0.5f, 0f, 0f);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/TitlePanel/Button_PerformanceOptions").GetComponentInChildren<Button>().colors = colors3;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_AdvancedOptions").GetComponentInChildren<Button>().colors = colors3;
                GameObject.Find("UserInterface/MenuContent/Screens/Social/UserProfileAndStatusSection/Status/EditStatusButton/Text").GetComponent<Text>().color = Color.white;
                GameObject.Find("UserInterface/MenuContent/Screens/Social/UserProfileAndStatusSection/ViewProfileButton/Text").GetComponent<Text>().color = Color.white;
                var hudRoot = GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud").gameObject;
                var inviteObj = hudRoot.transform.Find("NotificationDotParent/InviteDot").gameObject;
                //var inviteReqObj = hudRoot.transform.Find("NotificationDotParent/InviteRequestDot").gameObject;
                var notificationObj = hudRoot.transform.Find("NotificationDotParent/NotificationDot").gameObject;
                var voteKickObj = hudRoot.transform.Find("NotificationDotParent/VoteKickDot").gameObject;
                var friendRequestObj = hudRoot.transform.Find("NotificationDotParent/FriendRequestDot").gameObject;
                var voiceDotParent = hudRoot.transform.Find("VoiceDotParent").gameObject;
                var voiceDotObj = hudRoot.transform.Find("VoiceDotParent/VoiceDot").gameObject;
                var voiceDotDisabledObj = hudRoot.transform.Find("VoiceDotParent/VoiceDotDisabled").gameObject;
                var voicePushToTalkKeybd = hudRoot.transform.Find("VoiceDotParent/PushToTalkKeybd").gameObject;
                var voicePushToTalkXboxj = hudRoot.transform.Find("VoiceDotParent/PushToTalkXbox").gameObject;
                if (inviteObj != null && inviteObj.GetComponent<Image>().color != CachedColor) inviteObj.GetComponent<Image>().color = CachedColor;
                //if (inviteReqObj != null && inviteReqObj.GetComponent<Image>().color != CachedColor) inviteReqObj.GetComponent<Image>().color = CachedColor;
                if (voteKickObj != null && voteKickObj.GetComponent<Image>().color != CachedColor) voteKickObj.GetComponent<Image>().color = CachedColor;
                if (friendRequestObj != null && friendRequestObj.GetComponent<Image>().color != CachedColor) friendRequestObj.GetComponent<Image>().color = CachedColor;

                AssetBundleRequest TalkReq = PendulumClientMain.myAssetBundle.LoadAssetAsync("talkiconuncolored.png", Il2CppType.Of<Sprite>());
                Sprite TalkLogoWhite = TalkReq.asset.Cast<Sprite>();
                if (TalkLogoWhite != null)
                {   
                    if (voiceDotObj != null && voiceDotObj.GetComponent<Image>().color != CachedColor) voiceDotObj.GetComponent<Image>().color = CachedColor;
                    if (voiceDotObj != null && voiceDotObj.GetComponent<CanvasRenderer>().GetColor() != CachedColor) voiceDotObj.GetComponent<CanvasRenderer>().SetColor(CachedColor);
                    voiceDotParent.GetComponent<HudVoiceIndicator>().field_Private_Image_0.sprite = TalkLogoWhite;
                    if (voiceDotParent != null && voiceDotParent.GetComponent<HudVoiceIndicator>().field_Private_Color_0 != SolidWhite) voiceDotParent.GetComponent<HudVoiceIndicator>().field_Private_Color_0 = CachedColor;
                    //voiceDotParent.GetComponent<HudVoiceIndicator>().field_Private_Image_1.sprite = TalkLogoWhite;
                    if (voiceDotParent != null && voiceDotParent.GetComponent<HudVoiceIndicator>().field_Private_Color_1 != TransparentWhite) voiceDotParent.GetComponent<HudVoiceIndicator>().field_Private_Color_1 = new Color(CachedColor.r, CachedColor.g, CachedColor.b, CachedColor.a / 5f);

                    if (voiceDotDisabledObj != null && voiceDotDisabledObj.GetComponents<Image>()[0].color != new Color(CachedColor.r, CachedColor.g, CachedColor.b, CachedColor.a / 2.5f)) voiceDotDisabledObj.GetComponents<Image>()[0].color = new Color(CachedColor.r, CachedColor.g, CachedColor.b, CachedColor.a / 2.5f);
                    //if (voiceDotDisabledObj != null && voiceDotDisabledObj.GetComponent<CanvasRenderer>().GetColor() != new Color(CachedColor.r, CachedColor.g, CachedColor.b, CachedColor.a / 2.5f)) voiceDotDisabledObj.GetComponent<CanvasRenderer>().SetColor(new Color(CachedColor.r, CachedColor.g, CachedColor.b, CachedColor.a / 2.5f)); 

                    GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud/VoiceDotParent/VoiceDotDisabled").GetComponent<FadeCycleEffect>().enabled = false;

                    if (Shader.Find("VRChat/GalacticUi") != null)
                    {
                        //PendulumLogger.Log("VRChat/GalacticUi Detected");
                        //PendulumLogger.Log("Hud Icon Shader: {0}", .name);
                        GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud/VoiceDotParent/VoiceDotDisabled").GetComponent<Image>().material.shader = Shader.Find("VRChat/GalacticUi");
                    }
                }
                PendulumClientMain.UIColorsSetup = true;
            }
            catch (Exception e)
            {
                PendulumLogger.Log("ColorModuleException: " + e.ToString());
            }
            try
            {
                VRCUiCursorManager.field_Private_Static_VRCUiCursorManager_0.field_Public_VRCUiCursor_0.field_Public_Color_0 = color;
                VRCUiCursorManager.field_Private_Static_VRCUiCursorManager_0.field_Public_VRCUiCursor_1.field_Public_Color_0 = color;
                VRCUiCursorManager.field_Private_Static_VRCUiCursorManager_0.field_Public_VRCUiCursor_2.field_Public_Color_0 = color;
                VRCUiCursorManager.field_Private_Static_VRCUiCursorManager_0.field_Public_VRCUiCursor_3.field_Public_Color_0 = color;
                VRCUiCursorManager.field_Private_Static_VRCUiCursorManager_0.field_Public_VRCUiCursor_4.field_Public_Color_0 = color;
            }
            catch (Exception)
            {
            }
        }
        public static string object_path(GameObject obj)
        {
            string text = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                text = "/" + obj.name + text;
            }
            return text;
        }

        public static void find_all_child_objects(GameObject obj)
        {
            PendulumLogger.Log(object_path(obj));
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                find_all_child_objects(obj.transform.GetChild(i).gameObject);
            }
        }
    }
}