using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using PendulumClient.Main;
using PendulumClient.ColorModule;
using VRC.UI.Core.Styles;
using UnityEngine.Networking;
using UnhollowerRuntimeLib;

namespace PendulumClient.ColorModuleV2
{
    public static class CMV2_ColorModule
    {
        private static Sprite NewButton = null;
        private static List<string> GrayscaleSprites = new List<string>();
        public static void SetupColors()
        {
            PendulumClientMain.UIColorsSetup = true;

            Color color = ColorModule.ColorModule.CachedColor;
            Color color2 = ColorModule.ColorModule.CachedTextColor;
            Color color3 = ColorModule.ColorModule.DefaultMenuColorInverted;
            ColorBlock colorBlock = default(ColorBlock);
            colorBlock.colorMultiplier = 1f;
            colorBlock.disabledColor = Color.grey;
            colorBlock.highlightedColor = color * 1.5f;
            colorBlock.normalColor = color / 1.5f;
            colorBlock.pressedColor = Color.grey * 1.5f;
            colorBlock.fadeDuration = 0.1f;
            ColorBlock colors = colorBlock;

            //var gameObject1 = GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)");
            //var QMStyleEngine = gameObject1.GetComponent<VRC.UI.Core.Styles.StyleEngine>().field_Private_List_1_ElementStyle_0;

            var LoadRequest1 = PendulumClientMain.myAssetBundle.LoadAssetAsync("Button_White.png", Il2CppType.Of<Sprite>());
            if (LoadRequest1 != null && LoadRequest1.isDone)
            {
                NewButton = LoadRequest1.asset.Cast<Sprite>();
                NewButton.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            }

            var ColorManager = new CMV2_ColorManager(color, color2);
            var ColorStyle = new CMV2_OverridesStyles("MenuColor");
            var OverrideList = new List<CMV2_OverrideObject>();
            OverrideList.Add(new CMV2_OverrideObject(".BackgroundLayer1", "image-color: $BG$;"));
            OverrideList.Add(new CMV2_OverrideObject(".BackgroundLayer2", "image-color: $ACCT$;"));
            OverrideList.Add(new CMV2_OverrideObject(".BackgroundButton", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".BackgroundButton:hover", "image-color: $HIGH$;"));
            OverrideList.Add(new CMV2_OverrideObject(".BackgroundButton:disabled", "image-color: $DARK$;"));
            OverrideList.Add(new CMV2_OverrideObject(".BackgroundMissingInfo", "image-color: $BG$;"));
            OverrideList.Add(new CMV2_OverrideObject(".BackgroundWing", "image-color: $BG$;"));
            OverrideList.Add(new CMV2_OverrideObject(".WingListItemBackground", "image-color: $BG$;"));
            OverrideList.Add(new CMV2_OverrideObject(".WingListItemBackground:hover", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".ButtonWing", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".ButtonWing:hover:enabled", "image-color: $HIGH$;"));
            OverrideList.Add(new CMV2_OverrideObject(".ButtonWing:hover:disabled", "image-color: $BG$;"));
            OverrideList.Add(new CMV2_OverrideObject(".ButtonWing:active:enabled", "image-color: $HIGH$;"));
            OverrideList.Add(new CMV2_OverrideObject(".ButtonWing:disabled", "image-color: $BG$;"));
            OverrideList.Add(new CMV2_OverrideObject(".WingDropdownItemBackground", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".WingDropdown", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".WingDropdown:hover", "image-color: $HIGH$;"));
            OverrideList.Add(new CMV2_OverrideObject(".ButtonSquareQM", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".ButtonSquareQM:hover:enabled", "image-color: $HIGH$;"));
            OverrideList.Add(new CMV2_OverrideObject(".TabBottom", "image-color: $BG$;\nheight: 95;"));
            OverrideList.Add(new CMV2_OverrideObject(".TabBottom:selected", "image-color: $HIGH$;"));
            OverrideList.Add(new CMV2_OverrideObject(".MicToggle", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".SafeModeToggle", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".WingTabLeft", "image-color: $XTRADARK$;"));
            OverrideList.Add(new CMV2_OverrideObject(".WingTabLeft:hover", "image-color: $DARK$;"));
            OverrideList.Add(new CMV2_OverrideObject(".WingTabRight", "image-color: $XTRADARK$;"));
            OverrideList.Add(new CMV2_OverrideObject(".WingTabRight:hover", "image-color: $DARK$;"));
            OverrideList.Add(new CMV2_OverrideObject(".WingCircleButton", "image-color: $DARK$;"));
            OverrideList.Add(new CMV2_OverrideObject(".WingCircleButton:hover", "image-color: $BG$;"));
            OverrideList.Add(new CMV2_OverrideObject(".H1", "image-color: $TEXTHI$;"));
            OverrideList.Add(new CMV2_OverrideObject(".H2", "image-color: $TEXT$;"));
            OverrideList.Add(new CMV2_OverrideObject(".H3", "image-color: $TEXTHI$;"));
            OverrideList.Add(new CMV2_OverrideObject(".H4", "image-color: $TEXT$;"));
            OverrideList.Add(new CMV2_OverrideObject(".Icon", "image-color: $TEXTHI$;"));
            OverrideList.Add(new CMV2_OverrideObject(".Icon_Alt", "image-color: $TEXTHI$;"));
            OverrideList.Add(new CMV2_OverrideObject(".Separator", "image-color: $XTRADARK$;"));
            OverrideList.Add(new CMV2_OverrideObject(".SliderBackground", "image-color: $DARK$;"));
            OverrideList.Add(new CMV2_OverrideObject(".ElementClass_Scrollbar_HandleImage", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".ElementClass_Slider_HandleImage", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".Slider:hover>.ElementClass_Slider_HandleImage", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".Slider:active>.ElementClass_Slider_HandleImage", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".SliderFill", "image-color: $BASE$;"));
            OverrideList.Add(new CMV2_OverrideObject(".Slider:hover>.SliderFill", "image-color: $HIGH$;"));
            OverrideList.Add(new CMV2_OverrideObject(".Slider:active>.SliderFill", "image-color: $ACCT$;"));
            OverrideList.Add(new CMV2_OverrideObject(".BadgeJump", "image-color: $ACCT$;"));
            foreach (CMV2_OverrideObject obj in OverrideList)
            {
                ColorStyle.ParseOverride(obj.Selector, obj.Body);
            }
            ColorStyle.BackupDefaultStyle();
            //ColorStyle.OverrideSprite("Button_Default", NewButton);
            GrayscaleSprites.Add("images/background_layer_01");
            GrayscaleSprites.Add("images/background_layer_02_lines");
            GrayscaleSprites.Add("images/backgroundbottom");
            GrayscaleSprites.Add("images/backgroundoutline");
            GrayscaleSprites.Add("images/backgroundtop");
            GrayscaleSprites.Add("images/button_default");
            GrayscaleSprites.Add("images/button_hover");
            GrayscaleSprites.Add("images/dotted_panel");
            GrayscaleSprites.Add("images/notif_tab_marker");
            GrayscaleSprites.Add("images/notif_tab_marker_active");
            GrayscaleSprites.Add("images/notif_tab_marker_hover");
            GrayscaleSprites.Add("images/page_backdrop");
            GrayscaleSprites.Add("images/page_tab_backdrop");
            GrayscaleSprites.Add("images/page_tab_backdrop_hover");
            GrayscaleSprites.Add("images/panel_2_modal");
            GrayscaleSprites.Add("images/sliderfill");
            GrayscaleSprites.Add("images/tabbottom");
            GrayscaleSprites.Add("images/tableft");
            GrayscaleSprites.Add("images/tabright");
            GrayscaleSprites.Add("images/wing_item_backdrop");
            GrayscaleSprites.Add("images/wing_item_backdrop_hover");
            GrayscaleSprites.Add("images/safemode");
            //GrayscaleSprites.Add("Images/Icons/JumpToMM".ToLower());
            ColorStyle.ApplySpriteOverrides(GrayscaleSprites);
            ColorStyle.UpdateStylesForSpriteOverrides();
            ColorStyle.ApplyOverrides(ColorManager);
            foreach (var styleElement in ColorStyle.myStyleEngine.GetComponentsInChildren<StyleElement>(true))
                styleElement.Method_Protected_Void_0();
            ExtraStuff();
        }
        public static void LoadingScreenStuff()
        {
            try
            {
                if (File.Exists("PendulumClient/MenuMusic/LoginMusic.wav"))
                {
                    var uwr = UnityWebRequest.Get($"file://{Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/LoginMusic.wav")}");
                    uwr.SendWebRequest();
                    if (!uwr.isDone)
                    {
                        while (true)
                        {
                            System.Threading.Thread.Sleep(1);
                            if (uwr.isDone)
                            {
                                break;
                            }
                        }
                    }
                    var audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(uwr.downloadHandler, uwr.url, false, false, AudioType.UNKNOWN);

                    var audioSource = GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>();
                    audioSource.Stop();
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
                else if (File.Exists("PendulumClient/MenuMusic/LoginMusic.ogg"))
                {
                    var uwr = UnityWebRequest.Get($"file://{Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/LoginMusic.ogg")}");
                    uwr.SendWebRequest();
                    if (!uwr.isDone)
                    {
                        while (true)
                        {
                            System.Threading.Thread.Sleep(1);
                            if (uwr.isDone)
                            {
                                break;
                            }
                        }
                    }
                    var audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(uwr.downloadHandler, uwr.url, false, false, AudioType.UNKNOWN);

                    var audioSource = GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>();
                    audioSource.Stop();
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
                if (File.Exists("PendulumClient/MenuMusic/LoadingMusic.wav"))
                {
                    var uwr = UnityWebRequest.Get($"file://{Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/LoadingMusic.wav")}");
                    uwr.SendWebRequest();
                    if (!uwr.isDone)
                    {
                        while (true)
                        {
                            System.Threading.Thread.Sleep(1);
                            if (uwr.isDone)
                            {
                                break;
                            }
                        }
                    }
                    var audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(uwr.downloadHandler, uwr.url, false, false, AudioType.UNKNOWN);

                    var audioSource = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>();
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                        audioSource.clip = audioClip;
                        audioSource.Play();
                    }
                    else
                    {
                        audioSource.clip = audioClip;
                    }
                }
                else if (File.Exists("PendulumClient/MenuMusic/LoadingMusic.ogg"))
                {
                    var uwr = UnityWebRequest.Get($"file://{Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/LoadingMusic.ogg")}");
                    uwr.SendWebRequest();
                    if (!uwr.isDone)
                    {
                        while (true)
                        {
                            System.Threading.Thread.Sleep(1);
                            if (uwr.isDone)
                            {
                                break;
                            }
                        }
                    }
                    var audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(uwr.downloadHandler, uwr.url, false, false, AudioType.UNKNOWN);

                    var audioSource = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>();
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                        audioSource.clip = audioClip;
                        audioSource.Play();
                    }
                    else
                    {
                        audioSource.clip = audioClip;
                    }
                }
            }
            catch (Exception e)
            {
                PendulumLogger.LogErrorSevere("Failed to load MenuMusic: " + e.ToString());
            }

            try
            {
                var color = ColorModule.ColorModule.CachedColor;
                var color2 = ColorModule.ColorModule.CachedTextColor;
                ColorBlock colorBlock = default(ColorBlock);
                colorBlock.colorMultiplier = 1f;
                colorBlock.disabledColor = Color.grey;
                colorBlock.highlightedColor = color * 1.5f;
                colorBlock.normalColor = color / 1.5f;
                colorBlock.pressedColor = Color.grey * 1.5f;
                colorBlock.fadeDuration = 0.1f;
                ColorBlock colors = colorBlock;

                GameObject.Find("_Application/CursorManager/MouseArrow/VRCUICursorIcon").GetComponent<VRC.UI.CursorIcon>().field_Public_SpriteRenderer_0.color = color;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_Lighting (1)/Point light").GetComponentInChildren<Light>().color = color / 2f;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/txt_LOADING").GetComponentInChildren<Text>().color = color;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/txt_Percent").GetComponentInChildren<Text>().color = color;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Loading Elements/txt_LOADING_Size").GetComponentInChildren<Text>().color = color;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/MirroredElements").gameObject.SetActive(false);
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>().sprite = new Sprite();
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Panel_Backdrop").GetComponent<Image>().color = color / 2f;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked").GetComponentInChildren<MeshRenderer>().material.SetTexture("_Tex", new Texture2D(32, 32));
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked").GetComponentInChildren<MeshRenderer>().material.SetColor("_Tint", new Color(0f, 0f, 0f));
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponentInChildren<ParticleSystem>().startColor = new Color(1f, 1f, 1f, 0.5f);
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_FX_ParticleBubbles/FX_snow").GetComponentInChildren<ParticleSystem>().startSize = 0.1f;
                var newdecosprite = CMV2_SpriteUtil.GetGrayscaledSprite(GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>().sprite, true);
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>().sprite = newdecosprite;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>().sprite = newdecosprite;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Left").GetComponent<Image>().color = color;
                GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ProgressPanel/Parent_Loading_Progress/Decoration_Right").GetComponent<Image>().color = color;
                foreach(Button btn in GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/").GetComponentsInChildren<Button>())
                {
                    btn.colors = colors;
                }
            }
            catch (Exception e)
            {
                PendulumLogger.LogError("ColorSetup Error 2: " + e);
            }
        }

        public static void ChangeHudReticle()
        {
            GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud/ReticleParent/Reticle").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorHighlight;
        }
        public static void ChangeDebugPanel()
        {
            GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/QMNotificationsArea/DebugInfoPanel/Panel/Background").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
        }
        public static void ExtraStuff()
        {
            var CachedColor = ColorModule.ColorModule.CachedColor;
            Color SolidWhite = new Color(1f, 1f, 1f, 1f);
            Color TransparentWhite = new Color(1f, 1f, 1f, 0.33f);
            var hudRoot = GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud").gameObject;
            var inviteObj = hudRoot.transform.Find("NotificationDotParent/InviteDot").gameObject;
            //var inviteReqObj = hudRoot.transform.Find("NotificationDotParent/InviteRequestDot").gameObject;
            var notificationObj = hudRoot.transform.Find("NotificationDotParent/NotificationDot").gameObject;
            var voteKickObj = hudRoot.transform.Find("NotificationDotParent/VoteKickDot").gameObject;
            var friendRequestObj = hudRoot.transform.Find("NotificationDotParent/FriendRequestDot").gameObject;
            var voiceDotParent = hudRoot.transform.Find("VoiceDotParent").gameObject;
            var voiceDotObj = hudRoot.transform.Find("VoiceDotParent/VoiceDot").gameObject;
            var voiceDotDisabledObj = hudRoot.transform.Find("VoiceDotParent/VoiceDotDisabled").gameObject;
            //var voicePushToTalkKeybd = hudRoot.transform.Find("VoiceDotParent/PushToTalkKeybd").gameObject;
            //var voicePushToTalkXboxj = hudRoot.transform.Find("VoiceDotParent/PushToTalkXbox").gameObject;
            /*if (inviteObj != null && inviteObj.GetComponent<Image>().color != CachedColor) inviteObj.GetComponent<Image>().color = CachedColor;
            //if (inviteReqObj != null && inviteReqObj.GetComponent<Image>().color != CachedColor) inviteReqObj.GetComponent<Image>().color = CachedColor;
            if (voteKickObj != null && voteKickObj.GetComponent<Image>().color != CachedColor) voteKickObj.GetComponent<Image>().color = CachedColor;
            if (friendRequestObj != null && friendRequestObj.GetComponent<Image>().color != CachedColor) friendRequestObj.GetComponent<Image>().color = CachedColor;*/


            AssetBundleRequest TalkReq = PendulumClientMain.myAssetBundle.LoadAssetAsync("talkiconuncolored.png", Il2CppType.Of<Sprite>());
            if (TalkReq != null)
            {
                Sprite TalkLogoWhite = TalkReq.asset.Cast<Sprite>();
                if (voiceDotObj != null && voiceDotObj.GetComponent<Image>().color != CachedColor) voiceDotObj.GetComponent<Image>().color = CachedColor;
                //voiceDotObj.GetComponent<Image>().sprite = TalkLogoWhite;
                var hvi = UnityEngine.Object.FindObjectOfType<HudVoiceIndicator>();
                //if (voiceDotObj != null && voiceDotObj.GetComponent<CanvasRenderer>().GetColor() != CachedColor) voiceDotObj.GetComponent<CanvasRenderer>().SetColor(CachedColor);
                hvi.field_Private_Image_0.sprite = TalkLogoWhite;
                if (voiceDotDisabledObj != null && hvi.field_Private_Color_0 != SolidWhite) hvi.field_Private_Color_0 = CachedColor;
                //hvi.field_Private_Image_1.sprite = TalkLogoWhite;
                if (voiceDotDisabledObj != null && hvi.field_Private_Color_1 != TransparentWhite) hvi.field_Private_Color_1 = new Color(CachedColor.r, CachedColor.g, CachedColor.b, CachedColor.a / 5f);

                if (voiceDotDisabledObj != null && voiceDotDisabledObj.GetComponent<Image>().color != new Color(CachedColor.r, CachedColor.g, CachedColor.b, CachedColor.a / 2.5f)) voiceDotDisabledObj.GetComponents<Image>()[0].color = new Color(CachedColor.r, CachedColor.g, CachedColor.b, CachedColor.a / 2.5f);
                //if (voiceDotDisabledObj != null && voiceDotDisabledObj.GetComponent<CanvasRenderer>().GetColor() != new Color(CachedColor.r, CachedColor.g, CachedColor.b, CachedColor.a / 2.5f)) voiceDotDisabledObj.GetComponent<CanvasRenderer>().SetColor(new Color(CachedColor.r, CachedColor.g, CachedColor.b, CachedColor.a / 2.5f)); 

                GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud/VoiceDotParent/VoiceDotDisabled").GetComponent<FadeCycleEffect>().enabled = false;

                /*if (Shader.Find("VRChat/GalacticUi") != null)
                {
                    //PendulumLogger.Log("VRChat/GalacticUi Detected");
                    //PendulumLogger.Log("Hud Icon Shader: {0}", .name);
                    GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud/VoiceDotParent/VoiceDotDisabled").GetComponent<Image>().material.shader = Shader.Find("VRChat/GalacticUi");
                }*/
            }
            try
            {
                var color = ColorModule.ColorModule.CachedColor;
                var color2 = ColorModule.ColorModule.CachedTextColor;
                ColorBlock colorBlock = default(ColorBlock);
                colorBlock.colorMultiplier = 1f;
                colorBlock.disabledColor = new Color(color.r * 0.3f, color.g *0.3f, color.b * 0.3f, 1);
                colorBlock.highlightedColor = color * 1.5f;
                colorBlock.normalColor = color * 0.9f;
                colorBlock.pressedColor = Color.grey * 1.5f;
                colorBlock.selectedColor = color * 1.5f;
                colorBlock.fadeDuration = 0.1f;
                ColorBlock colors = colorBlock;
                ColorBlock colorBlock2 = default(ColorBlock);
                colorBlock2.colorMultiplier = 1f;
                colorBlock2.disabledColor = new Color(color.r * 0.3f, color.g * 0.3f, color.b * 0.3f, 1);
                colorBlock2.highlightedColor = color * 1.5f;
                colorBlock2.normalColor = color * 0.9f;
                colorBlock2.pressedColor = Color.grey * 1.5f;
                colorBlock2.selectedColor = color;
                colorBlock2.fadeDuration = 0.1f;
                ColorBlock colorsbaseselection = colorBlock2;
                ColorBlock colorBlock3 = default(ColorBlock);
                colorBlock3.colorMultiplier = 1f;
                colorBlock3.disabledColor = new Color(color.r * 0.3f, color.g * 0.3f, color.b * 0.3f, 1);
                colorBlock3.highlightedColor = color * 1.5f;
                colorBlock3.normalColor = color * 0.7f;
                colorBlock3.pressedColor = Color.grey * 1.5f;
                colorBlock3.selectedColor = color;
                colorBlock3.fadeDuration = 0.1f;
                ColorBlock colorsdarknormal = colorBlock3;
                ChangeHudReticle();
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel/Panel_Header").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/AudioDevicePanel/Panel_Header").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/MousePanel/Panel_Header").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/HeightPanel/Panel_Header").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VoiceOptionsPanel/Panel_Header").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel/Panel_Header (1)").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/UserVolumeOptions/Panel_Header (1)").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/UserVolumeOptions").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDark2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDark2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/OtherOptionsPanel").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDark2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VoiceOptionsPanel").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDark2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/MousePanel").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDark2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/AudioDevicePanel").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDark2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/ComfortSafetyPanel").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDark2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/HeightPanel").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDark2;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/TitlePanel").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/Panel_Header Top").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/Panel_Header Side").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeMaster").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeUi").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameWorld").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameVoice").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameAvatars").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeMaster/FillArea/Fill").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorBase;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeUi/FillArea/Fill").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorBase;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameWorld/FillArea/Fill").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorBase;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameVoice/FillArea/Fill").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorBase;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameAvatars/FillArea/Fill").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorBase;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeMaster/SliderLabel").GetComponentInChildren<Text>().color = color * 0.5f;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeUi/SliderLabel").GetComponentInChildren<Text>().color = color * 0.5f;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameWorld/SliderLabel").GetComponentInChildren<Text>().color = color * 0.5f;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameVoice/SliderLabel").GetComponentInChildren<Text>().color = color * 0.5f;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/VolumePanel/VolumeGameAvatars/SliderLabel").GetComponentInChildren<Text>().color = color * 0.5f;

                GameObject.Find("UserInterface/MenuContent/Screens/Settings/AudioDevicePanel/VolumeSlider").GetComponentInChildren<Image>().color = color * 0.5f;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/AudioDevicePanel/VolumeSlider/Fill Area/Fill").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorBase;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/AudioDevicePanel/SelectPrevMic").GetComponentInChildren<Button>().colors = colors;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/AudioDevicePanel/SelectNextMic").GetComponentInChildren<Button>().colors = colors;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/AudioDevicePanel/VolumeDisplay/Background").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;

                GameObject.Find("UserInterface/MenuContent/Screens/Settings/MousePanel/SensitivitySlider/Background").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/MousePanel/SensitivitySlider/Fill Area/Fill").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorBase;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/MousePanel/SensitivitySlider/Handle Slide Area/Handle").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorHighlight;

                var newsprite = GameObject.Find("UserInterface/MenuContent/Screens/Settings/Footer/Exit").GetComponent<Button>().image.sprite;//CMV2_SpriteUtil.GetGrayscaledSprite(GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_AdvancedOptions").GetComponent<Image>().sprite, true);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_AdvancedOptions").GetComponent<Button>().image.sprite = newsprite;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_AdvancedOptions").GetComponent<Image>().sprite = newsprite;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_AdvancedOptions").GetComponent<Image>().color = Color.white;
                //var newsprite2 = CMV2_SpriteUtil.GetGrayscaledSprite(GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_EditBindings").GetComponent<Image>().sprite, true);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_EditBindings").GetComponent<Button>().image.sprite = newsprite;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_EditBindings").GetComponent<Image>().sprite = newsprite;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_EditBindings").GetComponent<Image>().color = Color.white;
                GameObject.Destroy(GameObject.Find("UserInterface/MenuContent/Screens/Settings/Button_EditBindings/Image_NEW"));
                //var newsprite3 = CMV2_SpriteUtil.GetGrayscaledSprite(GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/TitlePanel/Button_PerformanceOptions").GetComponent<Image>().sprite, true);
                GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/TitlePanel/Button_PerformanceOptions").GetComponent<Button>().image.sprite = newsprite;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/TitlePanel/Button_PerformanceOptions").GetComponent<Image>().sprite = newsprite;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/TitlePanel/Button_PerformanceOptions").GetComponent<Image>().color = Color.white;

                GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/Button_Reset").GetComponent<Button>().image.sprite = newsprite;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/Button_Reset").GetComponent<Image>().sprite = newsprite;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/Button_Reset").GetComponent<Image>().color = Color.white;

                GameObject.Find("UserInterface/MenuContent/Screens/Avatar/TitlePanel (1)").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                GameObject.Find("UserInterface/MenuContent/Screens/Avatar/TitlePanel (1)").GetComponentInChildren<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;

                var MenuContentHeader = GameObject.Find("UserInterface/MenuContent/Backdrop/Header");
                if (MenuContentHeader != null)
                {
                    MenuContentHeader.SetActive(true);
                }


                var MMTabParent = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content");
                //var VRCPlusOnClick = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/VRC+PageTab/Button").GetComponent<Button>().onClick;
                var GalleryOnClick = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/GalleryTab/Button").GetComponent<Button>().onClick;
                var OldGalleryButton = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/GalleryTab/Button");

                var OldVRCPlusButton = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/VRC+PageTab");
                var NewVRCPlusButton = GameObject.Instantiate(OldVRCPlusButton, MMTabParent.transform);
                NewVRCPlusButton.name = "PendVRCPlusButton";
                //NewVRCPlusButton.GetComponentInChildren<Button>().gameObject.GetComponent<LayoutElement>().minWidth = 140;
                NewVRCPlusButton.GetComponent<LayoutElement>().minWidth = 140;

                var OldWorldsButton = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/WorldsPageTab");
                var NewWorldsButton = GameObject.Instantiate(OldWorldsButton, MMTabParent.transform);
                NewWorldsButton.name = "PendWorldsButton";

                var OldAvatarButton = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/AvatarPageTab");
                var NewAvatarButton = GameObject.Instantiate(OldAvatarButton, MMTabParent.transform);
                NewAvatarButton.name = "PendAvatarButton";

                var OldSocialButton = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/SocialPageTab");
                var NewSocialButton = GameObject.Instantiate(OldSocialButton, MMTabParent.transform);
                NewSocialButton.name = "PendSocialButton";

                var OldSettingsButton = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/SettingsPageTab");
                var NewSettingsButton = GameObject.Instantiate(OldSettingsButton, MMTabParent.transform);
                NewSettingsButton.name = "PendSettingsButton";

                var OldSafetyButton = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/SafetyPageTab");
                var NewSafetyButton = GameObject.Instantiate(OldSafetyButton, MMTabParent.transform);
                NewSafetyButton.name = "PendSafetyButton";

                //var OldSearchButton = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/Search");
                //var NewSearchButton = GameObject.Instantiate(OldSearchButton, MMTabParent.transform);
                //NewSearchButton.name = "PendSearchButton";

                GameObject.Destroy(OldVRCPlusButton);
                GameObject.Destroy(OldWorldsButton);
                GameObject.Destroy(OldAvatarButton);
                GameObject.Destroy(OldSocialButton);
                GameObject.Destroy(OldSettingsButton);
                GameObject.Destroy(OldSafetyButton);
                //MonoBehaviour2PublicVo1
                //GameObject.Destroy(OldSearchButton);
                GameObject.Destroy(NewVRCPlusButton.transform.Find("Image_NEW").gameObject);

                var termsbutton = GameObject.Find("UserInterface/MenuContent/Screens/VRC+/TitlePanel/TermsAndConditions");
                var GalleryButton = GameObject.Instantiate(termsbutton, GameObject.Find("UserInterface/MenuContent/Screens/VRC+/TitlePanel").transform);
                GalleryButton.name = "VRCPlusGalleryButton";
                GalleryButton.GetComponentInChildren<Text>().text = "Gallery";
                GalleryButton.transform.localPosition = new Vector3(-600, 0, 0);
                GalleryButton.GetComponent<Button>().onClick = GalleryOnClick;
                var pagetab = GalleryButton.AddComponent<VRCUiPageTab>();
                pagetab.field_Public_String_0 = "WORLDS";
                pagetab.field_Public_String_1 = "UserInterface/MenuContent/Screens/Gallery";
                pagetab.field_Public_TabContext_0 = VRCUiPageTabManager.TabContext.Everywhere;

                //GameObject.Find("UserInterface/MenuContent/Backdrop/Header").GetComponent<VRCUiPageHeader>().field_Public_GameObject_0 = GalleryButton;
                var LElement = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/Search").GetComponent<LayoutElement>();
                LElement.preferredWidth = 360;
                LElement.m_PreferredWidth = 360;
                LElement.minWidth = 360;
                LElement.m_MinWidth = 360;
                LElement.ignoreLayout = true;
                GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/Search").transform.localPosition = new Vector3(540f, 0f, -0.0264f);

                var NewPendImage = GameObject.Instantiate(GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/Search/InputField/Image"), GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content").transform);
                NewPendImage.name = "PendIcon";
                NewPendImage.GetComponent<Image>().sprite = JoinNotifier.JoinNotifierMod.PendulumSprite;
                NewPendImage.AddComponent<LayoutElement>();
                NewPendImage.GetComponent<LayoutElement>().ignoreLayout = true;
                NewPendImage.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
                NewPendImage.transform.localPosition = new Vector3(705f, 0f, -3.7f);

                GameObject.Destroy(OldGalleryButton);
                GameObject.Destroy(OldGalleryButton.transform.parent.Find("Image_NEW").gameObject);
                Component.Destroy(OldGalleryButton.transform.parent.gameObject.GetComponent<LayoutElement>());
                Component.Destroy(OldGalleryButton.transform.parent.gameObject.GetComponent<Image>());

                foreach (Button btn in GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content").GetComponentsInChildren<Button>(true))
                {
                    btn.colors = colors;
                }
                foreach (UiSafetyFeatureToggle toggle in GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/_SafetyMatrix/_Toggles").GetComponentsInChildren<UiSafetyFeatureToggle>())
                {
                    if (toggle.gameObject.name.StartsWith("Toggle_"))
                    {
                        toggle.field_Private_Color_0 = color;
                        toggle.field_Private_Color_2 = color;
                        toggle.gameObject.transform.Find("Toggle_Image/On").GetComponent<RawImage>().color = color;
                    }
                }
                GameObject gameObject = GameObject.Find("UserInterface/MenuContent");
                foreach (Text text in gameObject.transform.Find("Screens").GetComponentsInChildren<Text>(true))
                {
                    text.color = color2;
                }
                foreach (TMPro.TextMeshPro text in gameObject.transform.Find("Screens").GetComponentsInChildren<TMPro.TextMeshPro>(true))
                {
                    text.color = color2;
                }
                foreach (TMPro.TextMeshProUGUI text in gameObject.transform.Find("Screens").GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
                {
                    text.color = color2;
                }
                foreach (Button btn in gameObject.transform.Find("Screens").GetComponentsInChildren<Button>(true))
                {
                    btn.colors = colors;
                }
                foreach (Toggle toggle in gameObject.transform.Find("Screens/Settings").GetComponentsInChildren<Toggle>(true))
                {
                    if (toggle.transform.Find("Background") != null)
                    {
                        toggle.transform.Find("Background").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                        if (toggle.transform.Find("Background/Checkmark") != null)
                        {
                            toggle.transform.Find("Background/Checkmark").GetComponent<Image>().color = color;
                        }
                    }
                }
                NewVRCPlusButton.GetComponentInChildren<Button>().colors = colorsbaseselection;
                GameObject.Find("UserInterface/MenuContent/Screens/Settings_Safety/Button_Reset").GetComponent<Button>().colors = colorsdarknormal;
                gameObject.transform.Find("Backdrop/Backdrop/Background").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorBase;
                gameObject.transform.Find("Screens/UserInfo/User Panel/PanelHeaderBackground").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                gameObject.transform.Find("Screens/Settings_Safety/_Description_SafetyLevel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_SafeMode/ON").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_SafeMode/OFF").GetComponent<Image>().color = Color.black;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_SafeMode/ON/TopPanel_SafetyLevel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/ON").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/OFF").GetComponent<Image>().color = Color.black;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Custom/ON/TopPanel_SafetyLevel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/ON").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/OFF").GetComponent<Image>().color = Color.black;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_None/ON/TopPanel_SafetyLevel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/ON").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/OFF").GetComponent<Image>().color = Color.black;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Normal/ON/TopPanel_SafetyLevel").GetComponent<Image>().color = color;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Maxiumum/ON").GetComponent<Image>().color = CMV2_ColorManager.C_MenuColorDarklight;
                gameObject.transform.Find("Screens/Settings_Safety/_Buttons_SafetyLevel/Button_Maxiumum/OFF").GetComponent<Image>().color = Color.black;
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
            }
            catch (Exception e)
            {
                PendulumLogger.LogError("ColorSetup Error 1: " + e);
            }
        }
    }
}
