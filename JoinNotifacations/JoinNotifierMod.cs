using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using MelonLoader;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.UI;
using PendulumClient.ButtonAPIInc;
using PendulumClient.Wrapper;
using VRC;
using VRC.Core;
using Object = UnityEngine.Object;
using Player = VRC.Player;
using PendulumClient.Main;
using PendulumClient;

namespace JoinNotifier
{
    public class JoinNotifierMod : VRCMod
    {
        public static float NameTextUpdate = 0f;
        public static float OneTimeLoop = 0f;
        private static float ApplyNameTextColorsLoop = 0f;
        private static float SelfLogoPositioningLoop = 0f;

        public static bool PlayerManagerSetup = false;
        public static bool LoopOn = false;
        public static bool IsCEO = false;
        private static bool ApplyTextColors = false;
        private static bool LogoPositioning = false;

        private Image myJoinImage;
        private Image myLeaveImage;
        private AudioSource myJoinSource;
        private AudioSource myLeaveSource;
        private Text myJoinText;
        private Text myLeaveText;

        public static string plrid = "";

        public static Color KyranColor = new Color(1f, 0.35f, 0f);
        public static Color CorbinColor = Color.cyan;
        public static Color MooniColor = new Color(0.5f, 0.25f, 1f);

        //public const string BupperUID = "usr_bb365107-c1d1-4bf5-bd56-e63fd02060b1";
        //public const string BupperUID2 = "usr_035cbd45-e44b-4a06-a6e0-8250d0d4b2c9";

        public const string CorbinUID = "usr_70758d81-b4a8-4543-b033-f6b4bc97d165";
        public const string KyranUID1 = "usr_ba0606b7-927b-4c14-be7b-54d9d158c8e4";
        public const string KyranUID2 = "usr_bf196e57-9b62-46b6-a52d-73e264356e43";//one month "usr_07990006-1338-489f-802f-45455146f332";
        public const string GrubbieUID = "usr_6ae7e5da-94d2-48d5-8f4d-d1ba861f67bb";
        public const string MooniUID = "usr_71704e0d-ac55-4e40-8368-725e7c69466b";
        //public const string CermetUID = "usr_e0c7a496-ecd1-4158-873a-481be21cc5be";

        private static int myLastLevelLoad;
        private static bool myObservedLocalPlayerJoin;

        private Sprite myJoinSprite;
        public static Sprite PendulumSprite;
        private AudioClip myJoinClip;
        private AudioClip myLeaveClip;


        public static Sprite DevOutline = null;
        public static Sprite DevIconOutline = null;

        private Sprite KyranPFP;
        private Sprite CorbinPFP;
        private Sprite MooniPFP;

        private Player DevPlayer;

        //old unused accs
        //"usr_bb365107-c1d1-4bf5-bd56-e63fd02060b1",
        //"usr_ba0606b7-927b-4c14-be7b-54d9d158c8e4",
        //"usr_035cbd45-e44b-4a06-a6e0-8250d0d4b2c9",
        //"usr_643f4cb6-0c19-4413-937a-243cd2274518",
        //"usr_0678ce30-346e-41fa-a4cd-eead85cea457"

        public static List<string> DevUserIDs = new List<string>()
        {
            "usr_bf196e57-9b62-46b6-a52d-73e264356e43", //me
            "usr_70758d81-b4a8-4543-b033-f6b4bc97d165", //corbin
            "usr_71704e0d-ac55-4e40-8368-725e7c69466b" //other corbin
        };
        public override string Name
        {
            get
            {
                return "JoinNotifier";
            }
        }
        public override string Description
        {
            get
            {
                return "dicknballs";
            }
        }

        public override void OnStart()
        {
            //PendulumLogger.Log("ApplicationStart");
            MelonCoroutines.Start(InitThings());
        }

        public override void OnUpdate()
        {
            if (ApplyTextColors == true)
            {
                ApplyNameTextColorsLoop += Time.deltaTime;
            }

            if (ApplyNameTextColorsLoop >= 5f)
            {
                //ApplyNameTextColors();
                if (!string.IsNullOrEmpty(plrid))
                {
                    CheckInRoomList(plrid);
                }
                ApplyNameTextColorsLoop = 0f;
                ApplyTextColors = false;
            }

            /*if (LogoPositioning == true)
            {
                SelfLogoPositioningLoop += Time.deltaTime;
            }

            if (SelfLogoPositioningLoop >= 1f)
            {
                var PendLogo = DevPlayer.transform.Find("Canvas - Profile (1)/Frames/Panel - PendulumClientLogo");
                PendLogo.position += new Vector3(-0.045f, -0.039f, 0.25f);
                SelfLogoPositioningLoop = 0f;
                LogoPositioning = false;
            }
            /*NameTextUpdate += Time.deltaTime;
            if (KyranICI == true)
            {
                var Kyran = PlayerWrappers.GetPlayer("usr_e1f9673b-158a-403d-b897-e28aa47c9028");
                var MainCanvas = Kyran.gameObject.transform.Find("Canvas - Profile (1)");
                MainCanvas.Find("Frames/Panel - GeneratedVIPBadge").gameObject.SetActive(true);
                MainCanvas.Find("Text/Text - GeneratedVIPTag").gameObject.SetActive(true);
                MainCanvas.Find("Text/Text - GeneratedVIPTag Drop").gameObject.SetActive(true);
                PendulumLogger.Log("Plate Updated.");
            }
            if (KyranICI2 == true)
            {
                var Kyran = PlayerWrappers.GetPlayer("usr_1a4a7781-8e3c-4c85-b6a5-3eaa8e2a9a7a");
                var MainCanvas = Kyran.gameObject.transform.Find("Canvas - Profile (1)");
                MainCanvas.Find("Frames/Panel - GeneratedVIPBadge").gameObject.SetActive(true);
                MainCanvas.Find("Text/Text - GeneratedVIPTag").gameObject.SetActive(true);
                MainCanvas.Find("Text/Text - GeneratedVIPTag Drop").gameObject.SetActive(true);
                PendulumLogger.Log("Plate Updated.");
            }

            if(NameTextUpdate >= 5f && PlayerManagerSetup == true)
            {
                //ApplyNameTextColors();
                NameTextUpdate = 0f;
            }*/
        }

        public IEnumerator InitThings()
        {
            //PendulumLogger.Log("Waiting for Managers.");
            
            while (ReferenceEquals(NetworkManager.field_Internal_Static_NetworkManager_0, null)) yield return null;
            while (ReferenceEquals(VRCAudioManager.field_Private_Static_VRCAudioManager_0, null)) yield return null;
            while (ReferenceEquals(VRCUiManager.field_Private_Static_VRCUiManager_0, null)) yield return null;
            GameObject.Find("UserInterface/PlayerDisplay/BlackFade").GetComponent<VRCUiBackgroundFade>().field_Public_Boolean_0 = false;

            //while (ReferenceEquals(PlayerManager.field_Private_Static_PlayerManager_0, null)) yield return null;
            //while (ReferenceEquals(PendulumClientMain.LogoButtonSetup, false)) yield return null;

            PendulumLogger.EventLog("VRCUiManager init");
            PendulumLogger.Log("Start JoinNotifier init.");
            
            try
            {
                NetworkManagerHooks.Initialize();
            }
            catch (Exception e)
            {
                PendulumLogger.LogErrorSevere("NetworkManagerHook init failed: " + e.ToString());
            }

            if (PendulumClientMain.myAssetBundle == null)
            {
                PendulumLogger.LogErrorSevere("AssetBundle Not Found!");
            }

            try
            {
                var iconLoadRequest = PendulumClientMain.myAssetBundle.LoadAssetAsync("JoinIcon.png", Il2CppType.Of<Sprite>());
                //while (!iconLoadRequest.isDone) yield return null;
                myJoinSprite = iconLoadRequest.asset.Cast<Sprite>();
                myJoinSprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                var JoinSoundLoadRequest = PendulumClientMain.myAssetBundle.LoadAssetAsync("Chime.wav", Il2CppType.Of<AudioClip>());
                //while (!JoinSoundLoadRequest.isDone) yield return null;
                myJoinClip = JoinSoundLoadRequest.asset.Cast<AudioClip>();
                myJoinClip.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                var LeaveSoundLoadRequest = PendulumClientMain.myAssetBundle.LoadAssetAsync("DoorClose.wav", Il2CppType.Of<AudioClip>());
                //while (!LeaveSoundLoadRequest.isDone) yield return null;
                myLeaveClip = LeaveSoundLoadRequest.asset.Cast<AudioClip>();
                myLeaveClip.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                var KyranPFPRequest = PendulumClientMain.myAssetBundle.LoadAssetAsync("kyran_pfp.png", Il2CppType.Of<Sprite>());
                //while (!KyranPFPRequest.isDone) yield return null;
                KyranPFP = KyranPFPRequest.asset.Cast<Sprite>();
                KyranPFP.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                var CorbinPFPRequest = PendulumClientMain.myAssetBundle.LoadAssetAsync("corbin_pfp.png", Il2CppType.Of<Sprite>());
                //while (!CorbinPFPRequest.isDone) yield return null;
                CorbinPFP = CorbinPFPRequest.asset.Cast<Sprite>();
                CorbinPFP.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                var MooniPFPRequest = PendulumClientMain.myAssetBundle.LoadAssetAsync("mooni_pfp.png", Il2CppType.Of<Sprite>());
                //while (!MooniPFPRequest.isDone) yield return null;
                MooniPFP = MooniPFPRequest.asset.Cast<Sprite>();
                MooniPFP.hideFlags |= HideFlags.DontUnloadUnusedAsset;

                var DevCircleLogo = PendulumClientMain.myAssetBundle.LoadAsset_Internal("DevIconOutline", Il2CppType.Of<Sprite>());
                var DevIconReq = PendulumClientMain.myAssetBundle.LoadAsset_Internal("DevIcon", Il2CppType.Of<Sprite>());
                Sprite DevIcon = DevIconReq.Cast<Sprite>();
                Sprite DevIconOut = DevCircleLogo.Cast<Sprite>();
                DevIcon.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                DevIconOut.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                DevOutline = DevIcon;
                DevIconOutline = DevIconOut;
            }
            catch (Exception e)
            {
                PendulumLogger.LogErrorSevere("LoadJoinNotiferAssets failed: " + e.ToString());
            }

            /*if (PendulumClientMain.myAssetBundle == null)
            {
                PendulumLogger.LogError("AssetBundle Error");
            }

            var iconLoadRequest = PendulumClientMain.myAssetBundle.LoadAsset<Sprite>("JoinIcon.png");
            if (iconLoadRequest != null)
            {
                myJoinSprite.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                myJoinSprite = iconLoadRequest;
            }
            else
            {
                PendulumLogger.LogError("JoinIcon Error");
            }
            var JoinSoundLoadRequest = PendulumClientMain.myAssetBundle.LoadAsset<AudioClip>("Chime.wav");
            if (JoinSoundLoadRequest != null)
            {
                myJoinClip.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                myJoinClip = JoinSoundLoadRequest;
            }
            else
            {
                PendulumLogger.LogError("Chime Error");
            }
            var LeaveSoundLoadRequest = PendulumClientMain.myAssetBundle.LoadAsset<AudioClip>("DoorClose.wav");
            if (LeaveSoundLoadRequest != null)
            {
                myLeaveClip.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                myLeaveClip = LeaveSoundLoadRequest;
            }
            else
            {
                PendulumLogger.LogError("DoorClose Error");
            }*/

            try
            {
                CreateGameObjects();
            }
            catch (Exception e)
            {
                PendulumLogger.LogErrorSevere("CreateGameObjects failed: " + e.ToString());
            }

            //PendulumLogger.Log("ButtonAmountV2: " + Resources.FindObjectsOfTypeAll<VRC.UI.Elements.QuickMenu>()[0].gameObject.GetComponentsInChildren<Button>().Length);
            PendulumLogger.Log("JoinNotifier HUD Created!");

            PlayerManagerSetup = true;
            
            NetworkManagerHooks.OnJoin += OnPlayerJoined;
            NetworkManagerHooks.OnLeave += OnPlayerLeft;
        }

        public static void ApplyNameTextColors()
        {
            if (PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0 != null)
            {
                var AllPlayers = PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
                foreach (Player player in AllPlayers)
                {
                    if (player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject.transform.localScale.x > 0f)
                    {
                        if (player.prop_APIUser_0 != null && player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - NamePlate").GetComponent<Image>() != null && player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject != null)
                        {
                            var JoinedPlayer = player.prop_APIUser_0;
                            if (JoinedPlayer.last_platform != "android")
                            {
                                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                                NewNameText.name = "Text - GeneratedNameTagColor";
                                NewNameText.gameObject.SetActive(true);
                                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                                NewNameText.GetComponent<Text>().color = player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - NamePlate").gameObject.GetComponent<Image>().color;
                            }
                            else
                            {
                                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                                NewNameText.name = "Text - GeneratedNameTagColor";
                                NewNameText.gameObject.SetActive(true);
                                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                                NewNameText.GetComponent<Text>().color = Color.red;
                            }
                        }
                    }
                }
            }
        }

        public override void OnModSettingsApplied()
        {
            //PendulumLogger.Log("Applying Colors.");
            if (myJoinSource != null)
            {
                myJoinSource.volume = JoinNotifierSettings.GetSoundVolume();
                myJoinSource.outputAudioMixerGroup = JoinNotifierSettings.GetUseUiMixer() ? VRCAudioManager.field_Private_Static_VRCAudioManager_0.field_Public_AudioMixerGroup_1 : null;
            }

            if (myLeaveSource != null)
            {
                myLeaveSource.volume = JoinNotifierSettings.GetSoundVolume();
                myLeaveSource.outputAudioMixerGroup = JoinNotifierSettings.GetUseUiMixer() ? VRCAudioManager.field_Private_Static_VRCAudioManager_0.field_Public_AudioMixerGroup_1 : null;
            }

            if (myJoinImage != null)
                myJoinImage.color = JoinNotifierSettings.GetJoinIconColor();
            
            if (myLeaveImage != null)
                myLeaveImage.color = JoinNotifierSettings.GetLeaveIconColor();
            
            if (myJoinText != null)
            {
                myJoinText.fontSize = JoinNotifierSettings.GetTextSize;
                myJoinText.color = JoinNotifierSettings.GetJoinIconColor();
            }

            if (myLeaveText != null)
            {
                myLeaveText.fontSize = JoinNotifierSettings.GetTextSize;
                myLeaveText.color = JoinNotifierSettings.GetLeaveIconColor();
            }
            //PendulumLogger.Log("Colors Applied!");
        }

        private Image CreateNotifierImage(string name, float offset, Color colorTint)
        {
            var hudRoot = GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud");
            var requestedParent = hudRoot.transform.Find("NotificationDotParent");
            var indicator = Object.Instantiate(hudRoot.transform.Find("NotificationDotParent/NotificationDot").gameObject, requestedParent, false).Cast<GameObject>();
            indicator.name = "NotifyDot-" + name;
            indicator.SetActive(true);
            indicator.transform.localPosition += Vector3.right * -150f;
            indicator.transform.localPosition += Vector3.up * offset;
            var image = indicator.GetComponent<Image>();
            image.sprite = myJoinSprite;

            image.enabled = false;
            image.color = colorTint;

            return image;
        }

        private Text CreateTextNear(Image image, float offset, TextAnchor alignment)
        {
            var gameObject = new GameObject(image.gameObject.name + "-text");
            gameObject.AddComponent<Text>();
            gameObject.transform.SetParent(image.transform, false);
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = Vector3.up * offset;
            var text = gameObject.GetComponent<Text>();
            text.color = image.color;
            text.fontStyle = FontStyle.Bold;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.alignment = alignment;
            text.fontSize = JoinNotifierSettings.GetTextSize;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            gameObject.SetActive(true);
            return text;
        }

        private AudioSource CreateAudioSource(AudioClip clip, GameObject parent)
        {
            var source = parent.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialize = false;
            source.volume = JoinNotifierSettings.GetSoundVolume();
            source.loop = false;
            source.playOnAwake = false;
            if (JoinNotifierSettings.GetUseUiMixer())
                source.outputAudioMixerGroup = VRCAudioManager.field_Private_Static_VRCAudioManager_0.field_Public_AudioMixerGroup_1;
            return source;
        }

        private void CreateGameObjects()
        {
            if (myJoinImage != null) return;

            var hudRoot = GameObject.Find("UserInterface/UnscaledUI/HudContent/Hud");
            if (hudRoot == null)
            {
                PendulumLogger.LogErrorSevere("Error Creating JoinNotifier HUD");
                return;
            }
            
            PendulumLogger.Log("Creating JoinNotifier HUD");
//            var pathToThing = "UserInterface/UnscaledUI/HudContent/Hud/NotificationDotParent/NotificationDot";
            myJoinImage = CreateNotifierImage("join", 110f, JoinNotifierSettings.GetJoinIconColor());
            myJoinSource = CreateAudioSource(myJoinClip, myJoinImage.gameObject);
            myJoinText = CreateTextNear(myJoinImage, 110f, TextAnchor.LowerLeft);
            
            myLeaveImage = CreateNotifierImage("leave", 220f, JoinNotifierSettings.GetLeaveIconColor());
            myLeaveSource = CreateAudioSource(myLeaveClip, myLeaveImage.gameObject);
            myLeaveText = CreateTextNear(myLeaveImage, 110f, TextAnchor.LowerLeft);
        }

        public override void OnLevelWasInitialized(int level)
        {
            //PendulumLogger.Log("Scene Load (" + level + ")");
            
            myLastLevelLoad = Environment.TickCount;
            myObservedLocalPlayerJoin = false;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == '-')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static void CheckInRoomList(string username)
        {
            if (GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/InRoom/ViewPort/Content"))
            {
                var InRoomListObject = GameObject.Find("UserInterface/MenuContent/Screens/Social/Vertical Scroll View/Viewport/Content/InRoom/ViewPort/Content");
                var InRoomListCount = InRoomListObject.transform.childCount;
                var InRoomList = new List<GameObject>();
                for (int i = 0; i < InRoomListCount; i++)
                {
                    InRoomList.Add(InRoomListObject.transform.GetChild(i).gameObject);
                }
                foreach (var child in InRoomList)
                {
                    if (child.transform.Find("Background/ElementImageShape/ElementImage/Panel/TitleText") != null && child.name != "Image")
                    {
                        var TextComp = child.transform.Find("Background/ElementImageShape/ElementImage/Panel/TitleText").gameObject.GetComponent<Text>();
                        if (TextComp.text == RemoveSpecialCharacters(username))
                        {
                            GameObject.Destroy(child);
                        }
                        PendulumLogger.Log(TextComp.text);
                        PendulumLogger.Log(RemoveSpecialCharacters(username));
                    }
                    else
                    {
                        //PendulumLogger.Log(child.name);
                    }
                }
            }
        }

        public void OnPlayerJoined(Player player)
        {
            var JoinedPlayer = player.prop_APIUser_0;
            bool IsCEO = JoinedPlayer.id == GrubbieUID;
            if (LovesPhotonBots.Contains(JoinedPlayer.id))
            {
                GameObject.Destroy(player.gameObject);
                return;
            }
            if (APIUser.CurrentUser.id == JoinedPlayer.id)
            {
                myObservedLocalPlayerJoin = true;
                myLastLevelLoad = Environment.TickCount;
                //ApplyTextColors = true;
                /*player.gameObject.transform.Find("Canvas - Profile (1)").gameObject.SetActiveRecursively(true);
                player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - Debug Drop").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - Debug").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Images/Panel - Badges/Favorite").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - VIPBadge").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - Debug").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - Debug/Text").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - Debug/Background").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - Debug/Background").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - StatusBadge").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - StatusTag").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - StatusTag Drop").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - VIPTag").gameObject.SetActive(false);
                player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - VIPTag Drop").gameObject.SetActive(false);
                var MainCanvas = player.gameObject.transform.Find("Canvas - Profile (1)");
                GameObject OrgNametag = MainCanvas.Find("Frames/Panel - NamePlate").gameObject;
                JoiningPlayerList.RemoveRange(0, JoiningPlayerList.Count);
                if (JoinedPlayer.tags.Contains("system_legend"))
                {
                    OrgNametag.GetComponent<Image>().color = new Color(0f, 0.3f, 0.5f);
                }
                else if (player.prop_APIUser_0.hasLegendTrustLevel)
                {
                    OrgNametag.GetComponent<Image>().color = new Color(0.7f, 1f, 0f);
                }
                else if (player.prop_APIUser_0.hasVeteranTrustLevel)
                {
                    OrgNametag.GetComponent<Image>().color = new Color(0.5f, 0f, 1f);
                }
                else if (player.prop_APIUser_0.hasTrustedTrustLevel)
                {
                    OrgNametag.GetComponent<Image>().color = new Color(0.894f, 0.43f, 0.263f);
                }
                else if (player.prop_APIUser_0.hasKnownTrustLevel)
                {
                    OrgNametag.GetComponent<Image>().color = new Color(0.1647f, 0.8117f, 0.3568f);
                }
                else if (player.prop_APIUser_0.hasBasicTrustLevel)
                {
                    OrgNametag.GetComponent<Image>().color = new Color(0.0863f, 0.470f, 1f);
                }
                else if (player.prop_APIUser_0.isUntrusted)
                {
                    OrgNametag.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f);
                }*/

                /*if (DevUserIDs.Contains(JoinedPlayer.id))
                {
                    LogoPositioning = true;
                    DevPlayer = player;
                }*/
            }

            /*AssetBundleRequest NoTalkSprite = myAssetBundle.LoadAssetAsync("NamePlateSilent.png", Il2CppType.Of<Sprite>());
            AssetBundleRequest TalkingSprite = myAssetBundle.LoadAssetAsync("NamePlateTalk.png", Il2CppType.Of<Sprite>());
            Sprite NoTalkNameplate = NoTalkSprite.asset.Cast<Sprite>();
            Sprite TalkingNameplate = TalkingSprite.asset.Cast<Sprite>();
            if (NoTalkNameplate != null)
            {
                var OrgNameplate = player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - NamePlate").gameObject;
                var UnmutedNameplate = player.gameObject.transform.Find("Canvas - Profile (1)/Images/Panel - SpeechStatus/Unmuted").gameObject;
                var FramesTransform = player.gameObject.transform.Find("Canvas - Profile (1)/Frames").transform;
                GameObject GeneratedNP = GameObject.Instantiate(OrgNameplate, FramesTransform);
                GeneratedNP.name = "Panel - GeneratedNamePlate";
                GeneratedNP.GetComponent<Image>().sprite = NoTalkNameplate;
                OrgNameplate.transform.localScale = new Vector3(0f, 0f, 0f);
                OrgNameplate.SetActive(false);
                UnmutedNameplate.transform.localScale = new Vector3(0f, 0f, 0f);
                UnmutedNameplate.SetActive(false);
                //player.gameObject.transform.Find("Canvas - Profile (1)/Images/Panel - SpeechStatus/Unmuted").gameObject.GetComponent<Image>().sprite = TalkingNameplate;
            }*/

            if (player._vrcplayer.prop_ApiAvatar_0.id == "avtr_742010e6-370c-4dbd-a134-7bdde0e42d0d")
            {
                GameObject.Destroy(player.gameObject);
            }

            if (player.prop_APIUser_0.id == CorbinUID)
            {
                var TagColor = CorbinColor;
                var Parent = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
                var NametagBG = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Main/Background").gameObject;
                var StatsBG = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Quick Stats").gameObject;
                var NameIcon = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Icon").gameObject;
                var GenIcon = GameObject.Instantiate(NameIcon, Parent);
                GenIcon.name = "GeneratedIcon";
                if (CorbinPFP != null)
                {
                    GameObject.Destroy(GenIcon.transform.Find("Initials").gameObject);
                    GenIcon.transform.Find("Background").gameObject.GetComponent<Image>().sprite = DevOutline;
                    GenIcon.transform.Find("Background").gameObject.GetComponent<Image>().color = TagColor;
                    GenIcon.transform.Find("User Image").gameObject.GetComponent<RawImage>().texture = CorbinPFP.texture;
                    GenIcon.transform.Find("User Image").gameObject.SetActive(true);
                    var bgclone = GameObject.Instantiate(GenIcon.transform.Find("Background").gameObject, GenIcon.transform);
                    bgclone.name = "GeneratedBackground";
                    bgclone.GetComponent<Image>().sprite = DevIconOutline;
                    bgclone.GetComponent<Image>().color = TagColor;
                }
                GenIcon.SetActive(true);
                if (DevOutline != null)
                {
                    var threesliceNT = NametagBG.GetComponent<ImageThreeSlice>();
                    var threesliceQS = StatsBG.GetComponent<ImageThreeSlice>();
                    threesliceNT._sprite = DevOutline;
                    threesliceNT.color = TagColor;
                    threesliceQS._sprite = DevOutline;
                    threesliceQS.color = TagColor;
                }
            }

            if (player.prop_APIUser_0.id == KyranUID2)
            {
                var TagColor = KyranColor;
                var Parent = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
                var NametagBG = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Main/Background").gameObject;
                var StatsBG = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Quick Stats").gameObject;
                var NameIcon = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Icon").gameObject;
                var GenIcon = GameObject.Instantiate(NameIcon, Parent);
                GenIcon.name = "GeneratedIcon";
                if (KyranPFP != null)
                {
                    GameObject.Destroy(GenIcon.transform.Find("Initials").gameObject);
                    GenIcon.transform.Find("Background").gameObject.GetComponent<Image>().sprite = DevOutline;
                    GenIcon.transform.Find("Background").gameObject.GetComponent<Image>().color = TagColor;
                    GenIcon.transform.Find("User Image").gameObject.GetComponent<RawImage>().texture = KyranPFP.texture;
                    GenIcon.transform.Find("User Image").gameObject.SetActive(true);
                    var bgclone = GameObject.Instantiate(GenIcon.transform.Find("Background").gameObject, GenIcon.transform);
                    bgclone.name = "GeneratedBackground";
                    bgclone.GetComponent<Image>().sprite = DevIconOutline;
                    bgclone.GetComponent<Image>().color = TagColor;
                }
                GenIcon.SetActive(true);
                if (DevOutline != null)
                {
                    var threesliceNT = NametagBG.GetComponent<ImageThreeSlice>();
                    var threesliceQS = StatsBG.GetComponent<ImageThreeSlice>();
                    threesliceNT._sprite = DevOutline;
                    threesliceNT.color = TagColor;
                    threesliceQS._sprite = DevOutline;
                    threesliceQS.color = TagColor;
                }
            }

            if (player.prop_APIUser_0.id == MooniUID)
            {
                var TagColor = MooniColor;
                var Parent = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
                var NametagBG = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Main/Background").gameObject;
                var StatsBG = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Quick Stats").gameObject;
                var NameIcon = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Icon").gameObject;
                var GenIcon = GameObject.Instantiate(NameIcon, Parent);
                GenIcon.name = "GeneratedIcon";
                if (MooniPFP != null)
                {
                    GameObject.Destroy(GenIcon.transform.Find("Initials").gameObject);
                    GenIcon.transform.Find("Background").gameObject.GetComponent<Image>().sprite = DevOutline;
                    GenIcon.transform.Find("Background").gameObject.GetComponent<Image>().color = TagColor;
                    GenIcon.transform.Find("User Image").gameObject.GetComponent<RawImage>().texture = MooniPFP.texture;
                    GenIcon.transform.Find("User Image").gameObject.SetActive(true);
                    var bgclone = GameObject.Instantiate(GenIcon.transform.Find("Background").gameObject, GenIcon.transform);
                    bgclone.name = "GeneratedBackground";
                    bgclone.GetComponent<Image>().sprite = DevIconOutline;
                    bgclone.GetComponent<Image>().color = TagColor;
                }
                GenIcon.SetActive(true);
                if (DevOutline != null)
                {
                    var threesliceNT = NametagBG.GetComponent<ImageThreeSlice>();
                    var threesliceQS = StatsBG.GetComponent<ImageThreeSlice>();
                    threesliceNT._sprite = DevOutline;
                    threesliceNT.color = TagColor;
                    threesliceQS._sprite = DevOutline;
                    threesliceQS.color = TagColor;
                }
            }

            if (!DevUserIDs.Contains(player.field_Private_APIUser_0.id))
            {
                if (player._vrcplayer.field_Public_PlayerNameplate_0 != null)
                {
                    var textcont = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Main/Text Container").gameObject;
                    var orgname = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Main/Text Container/Name").gameObject;
                    var newname = GameObject.Instantiate(orgname, textcont.transform);
                    newname.name = "GeneratedName";
                    newname.SetActive(true);
                    newname.GetComponent<TMPro.TextMeshProUGUI>().color = PendulumClientMain.GetTrustColor(player.prop_APIUser_0);
                    newname.GetComponent<TMPro.TextMeshProUGUI>().text = player.prop_APIUser_0.displayName;
                    GameObject.Destroy(orgname);//orgname.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
                    if (APIUser.IsFriendsWith(player.prop_APIUser_0.id))
                    {
                        var content = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
                        var icons = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Icon").gameObject;
                        var newicons = GameObject.Instantiate(icons, content);
                        newicons.name = "GeneratedIcons";
                        newicons.SetActive(true);
                        for (int i = 0; i < newicons.transform.childCount; ++i)
                        {
                            //PendulumLogger.Log("Icon: " + newicons.transform.GetChild(i).gameObject.name);
                            GameObject.Destroy(newicons.transform.GetChild(i).gameObject);
                        }
                        var orgficon = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Icon/Friend Icon").gameObject;
                        var newficon = GameObject.Instantiate(orgficon, newicons.transform);
                        newficon.name = "GeneratedFriendIcon";
                        newficon.SetActive(true);
                        if (!player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Icon/User Image").gameObject.activeSelf)
                        {
                            newficon.GetComponent<RectTransform>().anchoredPosition += new Vector2(50, 0);
                            //orgficon.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
                        }
                    }
                    //var gplate = GameObject.Instantiate(plate);
                    //nametext.color = PendulumClientMain.GetTrustColor(player.prop_APIUser_0);
                }
            }
            else
            {
                if (player._vrcplayer.field_Public_PlayerNameplate_0 != null)
                {
                    var textcont = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Main/Text Container").gameObject;
                    var orgname = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Main/Text Container/Name").gameObject;
                    var newname = GameObject.Instantiate(orgname, textcont.transform);
                    newname.name = "GeneratedName";
                    newname.SetActive(true);
                    newname.GetComponent<TMPro.TextMeshProUGUI>().color = Color.white;//PendulumClientMain.GetTrustColor(player.prop_APIUser_0);
                    newname.GetComponent<TMPro.TextMeshProUGUI>().text = player.prop_APIUser_0.displayName;
                    GameObject.Destroy(orgname);//orgname.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
                    //PendulumLogger.Log("Layer: " + newname.layer);
                    if (APIUser.IsFriendsWith(player.prop_APIUser_0.id))
                    {
                        var content = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents");
                        var icons = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Icon").gameObject;
                        var newicons = GameObject.Instantiate(icons, content);
                        newicons.name = "GeneratedIcons";
                        newicons.SetActive(true);
                        for (int i = 0; i < newicons.transform.childCount; ++i)
                        {
                            //PendulumLogger.Log("Icon: " + newicons.transform.GetChild(i).gameObject.name);
                            GameObject.Destroy(newicons.transform.GetChild(i).gameObject);
                        }
                        var orgficon = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Icon/Friend Icon").gameObject;
                        var newficon = GameObject.Instantiate(orgficon, newicons.transform);
                        newficon.name = "GeneratedFriendIcon";
                        newficon.SetActive(true);
                        newficon.GetComponent<RectTransform>().anchoredPosition += new Vector2(50, 0);
                        orgficon.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
                    }
                    //var gplate = GameObject.Instantiate(plate);
                    //nametext.color = PendulumClientMain.GetTrustColor(player.prop_APIUser_0);
                }
            }

            /*if (JoinedPlayer.tags.Contains("system_legend") && !DevUserIDs.Contains(JoinedPlayer.id) && !IsCEO)
            {
                var TagColor = new Color(0f, 0.3f, 0.5f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                NewNameText.GetComponent<Text>().color = TagColor;
            }
            else if (player.prop_APIUser_0.hasLegendTrustLevel && !DevUserIDs.Contains(JoinedPlayer.id) && !IsCEO)
            {
                var TagColor = new Color(0.7f, 1f, 0f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                NewNameText.GetComponent<Text>().color = TagColor;
            }
            else if (player.prop_APIUser_0.hasVeteranTrustLevel && !DevUserIDs.Contains(JoinedPlayer.id) && !IsCEO)
            {
                var TagColor = new Color(0.5f, 0f, 1f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                NewNameText.GetComponent<Text>().color = TagColor;
            }
            else if (player.prop_APIUser_0.hasTrustedTrustLevel && !DevUserIDs.Contains(JoinedPlayer.id) && !IsCEO)
            {
                var TagColor = new Color(0.894f, 0.43f, 0.263f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                NewNameText.GetComponent<Text>().color = TagColor;
            }
            else if (player.prop_APIUser_0.hasKnownTrustLevel && !DevUserIDs.Contains(JoinedPlayer.id) && !IsCEO)
            {
                var TagColor = new Color(0.1647f, 0.8117f, 0.3568f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                NewNameText.GetComponent<Text>().color = TagColor;
            }
            else if (player.prop_APIUser_0.hasBasicTrustLevel && !DevUserIDs.Contains(JoinedPlayer.id) && !IsCEO)
            {
                var TagColor = new Color(0.0863f, 0.470f, 1f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                NewNameText.GetComponent<Text>().color = TagColor;
            }
            else if (player.prop_APIUser_0.isUntrusted && !DevUserIDs.Contains(JoinedPlayer.id) && !IsCEO)
            {
                var TagColor = new Color(0.8f, 0.8f, 0.8f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                NewNameText.GetComponent<Text>().color = TagColor;
            }
            if (BupperUID == JoinedPlayer.id || BupperUID2 == JoinedPlayer.id)
            {
                var TagColor = new Color(0.3f, 0.55f, 1f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - NamePlate").gameObject.GetComponent<Image>().color = TagColor;
                var MainCanvas = player.gameObject.transform.Find("Canvas - Profile (1)");
                GameObject VIPFrame = MainCanvas.Find("Frames/Panel - VIPBadge").gameObject;
                GameObject OrgNametag = MainCanvas.Find("Frames/Panel - NamePlate").gameObject;
                GameObject StatusFrame = MainCanvas.Find("Frames/Panel - StatusBadge").gameObject;
                GameObject Frames = MainCanvas.Find("Frames").gameObject;
                GameObject VIPText = MainCanvas.Find("Text/Text - VIPTag").gameObject;
                GameObject VIPTextBG = MainCanvas.Find("Text/Text - VIPTag Drop").gameObject;
                GameObject Text = MainCanvas.Find("Text").gameObject;
                GameObject TextBadgesPanel = MainCanvas.Find("Text/Panel - Badges (1)").gameObject;
                GameObject TrustText = MainCanvas.Find("Text/Panel - Badges (1)/TrustRankText").gameObject;
                GameObject TrustTextBG = MainCanvas.Find("Text/Panel - Badges (1)/TrustRankText/Overlay").gameObject;
                GameObject NewVIPFrame = GameObject.Instantiate(VIPFrame, Frames.transform);
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                GameObject NewStatusFrame = GameObject.Instantiate(StatusFrame, Frames.transform);
                NewStatusFrame.name = "Panel - PendulumClientLogo";
                NewStatusFrame.GetComponent<Image>().color = Color.white;
                NewStatusFrame.GetComponent<Image>().sprite = PendulumSprite;
                NewStatusFrame.transform.localScale += new Vector3(-0.35f, 0f);
                GameObject GeneratedNameplateColor = MainCanvas.Find("Text/Text - GeneratedNameTagColor").gameObject;
                NewVIPFrame.name = "Panel - GeneratedVIPBadge";
                NewVIPFrame.GetComponent<Image>().sprite = OrgNametag.GetComponent<Image>().sprite;
                GameObject NewVIPTextBG = GameObject.Instantiate(VIPTextBG, Text.transform);
                NewVIPTextBG.name = "Text - GeneratedVIPTag Drop";
                GameObject NewVIPText = GameObject.Instantiate(VIPText, Text.transform);
                NewVIPText.name = "Text - GeneratedVIPTag";
                if (APIUser.CurrentUser.id != BupperUID)
                {
                    GameObject NewTrustText = GameObject.Instantiate(TrustTextBG, TrustText.transform);
                    GameObject NewTrustTextBG = GameObject.Instantiate(TrustTextBG, TrustText.transform);
                    NewTrustText.name = "GeneratedTrustRankText";
                    NewTrustText.transform.position += new Vector3(0f, -0.065f, 0f);
                    NewTrustText.GetComponent<Text>().color = TagColor;
                    NewTrustText.GetComponent<Text>().text = "Kyran's Son";
                    NewTrustText.GetComponentInChildren<Text>().text = "Kyran's Son";
                    NewTrustText.SetActiveRecursively(true);
                    NewTrustTextBG.name = "GeneratedTrustBG";
                    NewTrustTextBG.transform.position += new Vector3(0f, -0.065f, 0f);
                    NewTrustTextBG.GetComponent<Text>().color = Color.black;
                    NewTrustTextBG.GetComponent<Text>().text = "Kyran's Son";
                }
                TrustText.GetComponent<Text>().color = Color.black;
                NewVIPFrame.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.SetActiveRecursively(true);
                NewStatusFrame.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.GetComponent<Text>().text = "RETARD";
                NewVIPTextBG.gameObject.SetActiveRecursively(true);
                NewVIPTextBG.gameObject.GetComponent<Text>().text = "RETARD";
                NewVIPFrame.GetComponent<Image>().color = TagColor;
                TrustTextBG.GetComponent<Text>().color = TagColor;
                GeneratedNameplateColor.GetComponent<Text>().color = TagColor;
                //GameObject NewTrustText2 = GameObject.Instantiate(TrustTextBG, TrustText.transform);
                //GameObject.Destroy(NewTrustText2.transform.Find("GeneratedTrustRankText").gameObject);
                //GameObject.Destroy(NewTrustText2.transform.Find("GeneratedTrustBG").gameObject);
                /*NewTrustText2.name = "GenTrustRankText22";
                NewTrustText2.GetComponent<Text>().color = new Color(0f, 0f, 0f, 0f);
                NewTrustText2.GetComponentInChildren<Text>().color = Color.black;
                NewTrustText2.GetComponentInChildren<Text>().text = "Friend (Known User)";*/
            //NewTrustText2.transform.Find("Overlay").GetComponentInChildren<Text>().color = Color.black;
            //NewTrustText2.transform.Find("Overlay").gameObject.SetActive(true);
            //}

            /*if (CorbinUID == JoinedPlayer.id)
            {
                var TagColor = Color.cyan;
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - NamePlate").gameObject.GetComponent<Image>().color = TagColor;
                var MainCanvas = player.gameObject.transform.Find("Canvas - Profile (1)");
                GameObject VIPFrame = MainCanvas.Find("Frames/Panel - VIPBadge").gameObject;
                GameObject StatusFrame = MainCanvas.Find("Frames/Panel - StatusBadge").gameObject;
                GameObject OrgNametag = MainCanvas.Find("Frames/Panel - NamePlate").gameObject;
                GameObject Frames = MainCanvas.Find("Frames").gameObject;
                GameObject VIPText = MainCanvas.Find("Text/Text - VIPTag").gameObject;
                GameObject VIPTextBG = MainCanvas.Find("Text/Text - VIPTag Drop").gameObject;
                GameObject Text = MainCanvas.Find("Text").gameObject;
                GameObject NewVIPFrame = GameObject.Instantiate(VIPFrame, Frames.transform);
                NewVIPFrame.name = "Panel - GeneratedVIPBadge";
                NewVIPFrame.GetComponent<Image>().sprite = OrgNametag.GetComponent<Image>().sprite;
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                NewNameText.GetComponent<Text>().color = TagColor;
                GameObject NewStatusFrame = GameObject.Instantiate(StatusFrame, Frames.transform);
                NewStatusFrame.name = "Panel - PendulumClientLogo";
                NewStatusFrame.GetComponent<Image>().color = Color.white;
                NewStatusFrame.GetComponent<Image>().sprite = PendulumSprite;
                NewStatusFrame.transform.localScale += new Vector3(-0.35f, 0f);
                GameObject NewVIPTextBG = GameObject.Instantiate(VIPTextBG, Text.transform);
                NewVIPTextBG.name = "Text - GeneratedVIPTag Drop";
                GameObject NewVIPText = GameObject.Instantiate(VIPText, Text.transform);
                NewVIPText.name = "Text - GeneratedVIPTag";
                NewVIPFrame.gameObject.SetActiveRecursively(true);
                NewStatusFrame.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.GetComponent<Text>().text = "SIMP";
                NewVIPTextBG.gameObject.SetActiveRecursively(true);
                NewVIPTextBG.gameObject.GetComponent<Text>().text = "SIMP";
                NewVIPFrame.GetComponent<Image>().color = TagColor;
            }

            /*if (CermetUID == JoinedPlayer.id)
            {
                var TagColor = new Color(1f, 0.75f, 0.913f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - NamePlate").gameObject.GetComponent<Image>().color = TagColor;
                var MainCanvas = player.gameObject.transform.Find("Canvas - Profile (1)");
                GameObject VIPFrame = MainCanvas.Find("Frames/Panel - VIPBadge").gameObject;
                GameObject OrgNametag = MainCanvas.Find("Frames/Panel - NamePlate").gameObject;
                GameObject StatusFrame = MainCanvas.Find("Frames/Panel - StatusBadge").gameObject;
                GameObject Frames = MainCanvas.Find("Frames").gameObject;
                GameObject VIPText = MainCanvas.Find("Text/Text - VIPTag").gameObject;
                GameObject VIPTextBG = MainCanvas.Find("Text/Text - VIPTag Drop").gameObject;
                GameObject Text = MainCanvas.Find("Text").gameObject;
                GameObject NewVIPFrame = GameObject.Instantiate(VIPFrame, Frames.transform);
                NewVIPFrame.name = "Panel - GeneratedVIPBadge";
                NewVIPFrame.GetComponent<Image>().sprite = OrgNametag.GetComponent<Image>().sprite;
                GameObject NewStatusFrame = GameObject.Instantiate(StatusFrame, Frames.transform);
                NewStatusFrame.name = "Panel - PendulumClientLogo";
                NewStatusFrame.GetComponent<Image>().color = Color.white;
                NewStatusFrame.GetComponent<Image>().sprite = PendulumSprite;
                NewStatusFrame.transform.localScale += new Vector3(-0.35f, 0f);
                GameObject NewVIPTextBG = GameObject.Instantiate(VIPTextBG, Text.transform);
                NewVIPTextBG.name = "Text - GeneratedVIPTag Drop";
                GameObject NewVIPText = GameObject.Instantiate(VIPText, Text.transform);
                NewVIPText.name = "Text - GeneratedVIPTag";
                NewVIPFrame.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.SetActiveRecursively(true);
                NewStatusFrame.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.GetComponent<Text>().text = "Dev";
                NewVIPTextBG.gameObject.SetActiveRecursively(true);
                NewVIPTextBG.gameObject.GetComponent<Text>().text = "Dev";
                NewVIPFrame.GetComponent<Image>().color = TagColor;
            }*/

            /*if (KyranUID1 == JoinedPlayer.id)
            {
                //KyranICI = true;
                var TagColor = new Color(1f, 0.3f, 0f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - NamePlate").gameObject.GetComponent<Image>().color = TagColor;
                var MainCanvas = player.gameObject.transform.Find("Canvas - Profile (1)");
                GameObject VIPFrame = MainCanvas.Find("Frames/Panel - VIPBadge").gameObject;
                GameObject OrgNametag = MainCanvas.Find("Frames/Panel - NamePlate").gameObject;
                GameObject StatusFrame = MainCanvas.Find("Frames/Panel - StatusBadge").gameObject;
                GameObject Frames = MainCanvas.Find("Frames").gameObject;
                GameObject VIPText = MainCanvas.Find("Text/Text - VIPTag").gameObject;
                GameObject VIPTextBG = MainCanvas.Find("Text/Text - VIPTag Drop").gameObject;
                GameObject Text = MainCanvas.Find("Text").gameObject;
                GameObject TrustText = MainCanvas.Find("Text/Panel - Badges (1)/TrustRankText").gameObject;
                GameObject TrustTextBG = MainCanvas.Find("Text/Panel - Badges (1)/TrustRankText/Overlay").gameObject;
                GameObject NewVIPFrame = GameObject.Instantiate(VIPFrame, Frames.transform);
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                GameObject NewStatusFrame = GameObject.Instantiate(StatusFrame, Frames.transform);
                NewStatusFrame.name = "Panel - PendulumClientLogo";
                NewStatusFrame.GetComponent<Image>().color = Color.white;
                NewStatusFrame.GetComponent<Image>().sprite = PendulumSprite;
                NewStatusFrame.transform.localScale += new Vector3(-0.35f, 0f);
                GameObject GeneratedNameplateColor = MainCanvas.Find("Text/Text - GeneratedNameTagColor").gameObject;
                NewVIPFrame.name = "Panel - GeneratedVIPBadge";
                NewVIPFrame.GetComponent<Image>().sprite = OrgNametag.GetComponent<Image>().sprite;
                GameObject NewVIPTextBG = GameObject.Instantiate(VIPTextBG, Text.transform);
                NewVIPTextBG.name = "Text - GeneratedVIPTag Drop";
                GameObject NewVIPText = GameObject.Instantiate(VIPText, Text.transform);
                NewVIPText.name = "Text - GeneratedVIPTag";
                TrustText.GetComponent<Text>().color = Color.black;
                if (APIUser.CurrentUser.id != KyranUID1)
                {
                    GameObject NewTrustText = GameObject.Instantiate(TrustTextBG, TrustText.transform);
                    GameObject NewTrustTextBG = GameObject.Instantiate(TrustTextBG, TrustText.transform);
                    NewTrustText.name = "GeneratedTrustRankText";
                    NewTrustText.transform.position += new Vector3(0f, -0.065f, 0f);
                    NewTrustText.GetComponent<Text>().color = TagColor;
                    NewTrustText.GetComponent<Text>().text = "Client Developer";
                    NewTrustText.GetComponentInChildren<Text>().text = "Client Developer";
                    NewTrustText.SetActiveRecursively(true);
                    NewTrustTextBG.name = "GeneratedTrustBG";
                    NewTrustTextBG.transform.position += new Vector3(0f, -0.065f, 0f);
                    NewTrustTextBG.GetComponent<Text>().color = Color.black;
                    NewTrustTextBG.GetComponent<Text>().text = "Client Developer";
                }
                TrustTextBG.GetComponent<Text>().color = TagColor;
                GeneratedNameplateColor.GetComponent<Text>().color = TagColor;
                NewVIPFrame.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.SetActiveRecursively(true);
                NewStatusFrame.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.GetComponent<Text>().text = "DEV";
                NewVIPTextBG.gameObject.SetActiveRecursively(true);
                NewVIPTextBG.gameObject.GetComponent<Text>().text = "DEV";
                NewVIPFrame.GetComponent<Image>().color = TagColor;
            }

            if (KyranUID2 == JoinedPlayer.id)
            {
                //KyranICI2 = true;
                var TagColor = new Color(1f, 0.3f, 0f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - NamePlate").gameObject.GetComponent<Image>().color = TagColor;
                var MainCanvas = player.gameObject.transform.Find("Canvas - Profile (1)");
                GameObject VIPFrame = MainCanvas.Find("Frames/Panel - VIPBadge").gameObject;
                GameObject OrgNametag = MainCanvas.Find("Frames/Panel - NamePlate").gameObject;
                GameObject StatusFrame = MainCanvas.Find("Frames/Panel - StatusBadge").gameObject;
                GameObject Frames = MainCanvas.Find("Frames").gameObject;
                GameObject VIPText = MainCanvas.Find("Text/Text - VIPTag").gameObject;
                GameObject VIPTextBG = MainCanvas.Find("Text/Text - VIPTag Drop").gameObject;
                GameObject Text = MainCanvas.Find("Text").gameObject;
                GameObject TrustText = MainCanvas.Find("Text/Panel - Badges (1)/TrustRankText").gameObject;
                GameObject TrustTextBG = MainCanvas.Find("Text/Panel - Badges (1)/TrustRankText/Overlay").gameObject;
                GameObject NewVIPFrame = GameObject.Instantiate(VIPFrame, Frames.transform);
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                GameObject NewStatusFrame = GameObject.Instantiate(StatusFrame, Frames.transform);
                NewStatusFrame.name = "Panel - PendulumClientLogo";
                NewStatusFrame.GetComponent<Image>().color = Color.white;
                NewStatusFrame.GetComponent<Image>().sprite = PendulumSprite;
                NewStatusFrame.transform.localScale += new Vector3(-0.35f, 0f);
                GameObject GeneratedNameplateColor = MainCanvas.Find("Text/Text - GeneratedNameTagColor").gameObject;
                NewVIPFrame.name = "Panel - GeneratedVIPBadge";
                NewVIPFrame.GetComponent<Image>().sprite = OrgNametag.GetComponent<Image>().sprite;
                GameObject NewVIPTextBG = GameObject.Instantiate(VIPTextBG, Text.transform);
                NewVIPTextBG.name = "Text - GeneratedVIPTag Drop";
                GameObject NewVIPText = GameObject.Instantiate(VIPText, Text.transform);
                NewVIPText.name = "Text - GeneratedVIPTag";
                TrustText.GetComponent<Text>().color = Color.black;
                if (APIUser.CurrentUser.id != KyranUID2)
                {
                    GameObject NewTrustText = GameObject.Instantiate(TrustTextBG, TrustText.transform);
                    GameObject NewTrustTextBG = GameObject.Instantiate(TrustTextBG, TrustText.transform);
                    NewTrustText.name = "GeneratedTrustRankText";
                    NewTrustText.transform.position += new Vector3(0f, -0.065f, 0f);
                    NewTrustText.GetComponent<Text>().color = TagColor;
                    NewTrustText.GetComponent<Text>().text = "Client Developer";
                    NewTrustText.GetComponentInChildren<Text>().text = "Client Developer";
                    NewTrustText.SetActiveRecursively(true);
                    NewTrustTextBG.name = "GeneratedTrustBG";
                    NewTrustTextBG.transform.position += new Vector3(0f, -0.065f, 0f);
                    NewTrustTextBG.GetComponent<Text>().color = Color.black;
                    NewTrustTextBG.GetComponent<Text>().text = "Client Developer";
                }
                TrustTextBG.GetComponent<Text>().color = TagColor;
                GeneratedNameplateColor.GetComponent<Text>().color = TagColor;
                NewVIPFrame.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.SetActiveRecursively(true);
                NewStatusFrame.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.GetComponent<Text>().text = "DEV";
                NewVIPTextBG.gameObject.SetActiveRecursively(true);
                NewVIPTextBG.gameObject.GetComponent<Text>().text = "DEV";
                NewVIPFrame.GetComponent<Image>().color = TagColor;
            }

            if (JoinedPlayer.id == GrubbieUID)
            {
                var TagColor = new Color(1f, 1f, 1f);
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - NamePlate").gameObject.GetComponent<Image>().color = TagColor;
                var MainCanvas = player.gameObject.transform.Find("Canvas - Profile (1)");
                GameObject VIPFrame = MainCanvas.Find("Frames/Panel - VIPBadge").gameObject;
                GameObject OrgNametag = MainCanvas.Find("Frames/Panel - NamePlate").gameObject;
                GameObject StatusFrame = MainCanvas.Find("Frames/Panel - StatusBadge").gameObject;
                GameObject Frames = MainCanvas.Find("Frames").gameObject;
                GameObject VIPText = MainCanvas.Find("Text/Text - VIPTag").gameObject;
                GameObject VIPTextBG = MainCanvas.Find("Text/Text - VIPTag Drop").gameObject;
                GameObject Text = MainCanvas.Find("Text").gameObject;
                GameObject TrustText = MainCanvas.Find("Text/Panel - Badges (1)/TrustRankText").gameObject;
                GameObject TrustTextBG = MainCanvas.Find("Text/Panel - Badges (1)/TrustRankText/Overlay").gameObject;
                GameObject NewVIPFrame = GameObject.Instantiate(VIPFrame, Frames.transform);
                var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent.transform);
                NewNameText.name = "Text - GeneratedNameTagColor";
                NewNameText.gameObject.SetActive(true);
                PlayerNameText.gameObject.transform.localScale = new Vector3(0f, 0f, 0f);
                GameObject NewStatusFrame = GameObject.Instantiate(StatusFrame, Frames.transform);
                NewStatusFrame.name = "Panel - PendulumClientLogo";
                NewStatusFrame.GetComponent<Image>().color = Color.white;
                NewStatusFrame.GetComponent<Image>().sprite = PendulumSprite;
                NewStatusFrame.transform.localScale += new Vector3(-0.35f, 0f);
                GameObject GeneratedNameplateColor = MainCanvas.Find("Text/Text - GeneratedNameTagColor").gameObject;
                NewVIPFrame.name = "Panel - GeneratedVIPBadge";
                NewVIPFrame.GetComponent<Image>().sprite = OrgNametag.GetComponent<Image>().sprite;
                GameObject NewVIPTextBG = GameObject.Instantiate(VIPTextBG, Text.transform);
                NewVIPTextBG.name = "Text - GeneratedVIPTag Drop";
                GameObject NewVIPText = GameObject.Instantiate(VIPText, Text.transform);
                NewVIPText.name = "Text - GeneratedVIPTag";
                TrustText.GetComponent<Text>().color = Color.black;
                if (APIUser.CurrentUser.id != GrubbieUID)
                {
                    GameObject NewTrustText = GameObject.Instantiate(TrustTextBG, TrustText.transform);
                    GameObject NewTrustTextBG = GameObject.Instantiate(TrustTextBG, TrustText.transform);
                    NewTrustText.name = "GeneratedTrustRankText";
                    NewTrustText.GetComponent<RectTransform>().position += new Vector3(0f, -0.065f, 0f);
                    NewTrustText.GetComponent<Text>().color = TagColor;
                    NewTrustText.GetComponent<Text>().text = "CEO of Pendulum Client";
                    NewTrustText.GetComponentInChildren<Text>().text = "CEO of Pendulum Client";
                    NewTrustText.SetActiveRecursively(true);
                    NewTrustTextBG.name = "GeneratedTrustBG";
                    NewTrustTextBG.GetComponent<RectTransform>().position += new Vector3(0f, -0.065f, 0f);
                    NewTrustTextBG.GetComponent<Text>().color = Color.black;
                    NewTrustTextBG.GetComponent<Text>().text = "CEO of Pendulum Client";
                }
                TrustTextBG.GetComponent<Text>().color = TagColor;
                GeneratedNameplateColor.GetComponent<Text>().color = TagColor;
                NewVIPFrame.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.SetActiveRecursively(true);
                NewStatusFrame.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.GetComponent<Text>().text = "CEO";
                NewVIPTextBG.gameObject.SetActiveRecursively(true);
                NewVIPTextBG.gameObject.GetComponent<Text>().text = "CEO";
                NewVIPFrame.GetComponent<Image>().color = TagColor;
            }

            if (JoinedPlayer.last_platform == "android")
            {
                PlayerWrappers.GetPlayer(JoinedPlayer.id).field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0f, 0f, 0f));
                var MainCanvas = player.gameObject.transform.Find("Canvas - Profile (1)");
                GameObject Text = MainCanvas.Find("Text").gameObject;
                GameObject VIPText = MainCanvas.Find("Text/Text - VIPTag").gameObject;
                GameObject VIPTextBG = MainCanvas.Find("Text/Text - VIPTag Drop").gameObject;
                GameObject NamePlateText = MainCanvas.Find("Text/Text - NameTag").gameObject;
                NamePlateText.GetComponent<Text>().color = Color.red;
                GameObject NewVIPTextBG = GameObject.Instantiate(VIPTextBG, Text.transform);
                NewVIPTextBG.name = "Text - GeneratedVIPTag Drop";
                GameObject NewVIPText = GameObject.Instantiate(VIPText, Text.transform);
                NewVIPText.name = "Text - GeneratedVIPTag";
                NewVIPText.gameObject.SetActiveRecursively(true);
                NewVIPText.gameObject.GetComponent<Text>().text = "Quest";
                NewVIPText.gameObject.GetComponent<Text>().color = Color.red;
                NewVIPTextBG.gameObject.SetActiveRecursively(true);
                NewVIPTextBG.gameObject.GetComponent<Text>().text = "Quest";
            }

            //AddPlayerToJoiningList(player);*/

            //PendulumClientMain.UpdatePlayerList();

            if (false)//player.prop_APIUser_0.id == APIUser.CurrentUser.id)//(player.gameObject.transform.Find("Player Nameplate"))
            {
                var nameplate = player.gameObject.transform.Find("Player Nameplate/Canvas/Nameplate/Contents/Main/Dev Circle");
                var namebg = nameplate;//.transform.Find("Canvas").gameObject;
                Assembly assemcs = typeof(VRCPlayer).Assembly;
                foreach (var SelType in assemcs.GetTypes())
                {
                    foreach (var Property in SelType.GetProperties())
                    {
                        if (Property.PropertyType == typeof(Il2CppSystem.Type))
                        {
                            try
                            {
                                if (Il2CppType.From(SelType) != null)
                                {
                                    if (namebg.GetComponent(Il2CppType.From(SelType)) != null)
                                    {
                                        PendulumLogger.Log("NameplateComponentName: " + SelType.FullName);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                            }
                        }
                    }
                }
            }

            if (JoinedPlayer.id != APIUser.CurrentUser.id)
            {
                if (JoinedPlayer.last_platform == "android")
                {
                    PendulumLogger.Log(ConsoleColor.DarkGray, "[Q] " + JoinedPlayer.displayName + " Joined." + " (" + JoinedPlayer.id + ")");
                }
                else
                {
                    PendulumLogger.JoinLog(JoinedPlayer.displayName + " (" + JoinedPlayer.id + ")");
                }
            }

            //PendulumClientMain.DebugLogPlayerJoinLeave(player, "Joined", true);
            if (PendulumClient.ButtonAPIV2.MenuSetup.PlayerESPon)
            {
                if (player.gameObject.transform.Find("SelectRegion") != null)
                {
                    Wrappers.EnableOutline(player.gameObject.transform.Find("SelectRegion").gameObject.GetComponent<MeshRenderer>(), PendulumClient.ColorModule.ColorModule.CachedColor);
                }
            }

            if (!myObservedLocalPlayerJoin || Environment.TickCount - myLastLevelLoad < 5_000) return;
            if (!JoinNotifierSettings.ShouldNotifyInCurrentInstance()) return;
            var playerName = JoinedPlayer.displayName ?? "!null!";
            MelonCoroutines.Start(BlinkIconCoroutine(myJoinImage));
               myJoinSource.Play();
                if (player.prop_APIUser_0.last_platform == "android")
                {
                    myJoinText.color = new Color(0.25f, 0.25f, 0.25f);
                }
                else if (!string.IsNullOrEmpty(player.prop_APIUser_0.friendKey) || APIUser.IsFriendsWith(player.prop_APIUser_0.id) || player.prop_APIUser_0.isFriend)
                {
                    myJoinText.color = new Color(1f, 0.8f, 0f);
                }
                else
                {
                    myJoinText.color = JoinNotifierSettings.GetJoinIconColor();
                }
            MelonCoroutines.Start(ShowName(myJoinText, playerName));
        }
        
        public void OnPlayerLeft(Player player)
        {
            var LeftPlayer = player.prop_APIUser_0;

            if (LeftPlayer.id != APIUser.CurrentUser.id)
            {
                PendulumLogger.LeaveLog(LeftPlayer.displayName);
            }
            if (LeftPlayer.id == PendulumClientMain.StoredUserID)
            {
                PendulumClientMain.DebugLogPlayerJoinLeave(player, "Left", false);
            }
            else
            {
                PendulumClientMain.DebugLogPlayerJoinLeave(player, "Left", false);
            }
            if (LeftPlayer.id == APIUser.CurrentUser.id) return;
            //PendulumClientMain.UpdatePlayerList();
            if (!JoinNotifierSettings.ShouldNotifyInCurrentInstance()) return;
            if (Environment.TickCount - myLastLevelLoad < 5_000) return;
            var playerName = player.prop_APIUser_0.displayName ?? "!null!";
            MelonCoroutines.Start(BlinkIconCoroutine(myLeaveImage));
            myLeaveSource.Play();
            MelonCoroutines.Start(ShowName(myLeaveText, playerName));
        }

        public IEnumerator ShowName(Text text, string name)
        {
            var currentText = text.text ?? "";
            currentText = currentText.Length == 0 ? name : currentText + "\n" + name;
            text.text = currentText;
            yield return new WaitForSeconds(3);
            currentText = text.text;
            currentText = currentText.Replace(name, "").Trim('\n');
            text.text = currentText;
        }

        public IEnumerator BlinkIconCoroutine(Image imageToBlink)
        {
            for (var i = 0; i < 3; i++)
            {
                imageToBlink.enabled = true;
                yield return new WaitForSeconds(.5f);
                imageToBlink.enabled = false;
                yield return new WaitForSeconds(.5f);
            }
        }

        public static void AddPlayerToJoiningList(Player player)
        {
            if (player.prop_APIUser_0.id != APIUser.CurrentUser.id)
            {
                JoiningPlayerList.Add(player);
            }
        }

        public static System.Collections.Generic.List<Player> JoiningPlayerList = new System.Collections.Generic.List<Player>();

        public static bool KyranICI = false;

        public static bool KyranICI2 = false;

        public static string foldername = "PendulumClient";

        //public static AssetBundle myAssetBundle2 = AssetBundle.LoadFromFile(foldername + "/MenuMusic/menumusic.assetbundle");

        public static List<string> LovesPhotonBots = new List<string>()
        {
            "usr_68318415-6a0f-4f33-b737-f6f6f0373968",
            "usr_a7ea6435-ec4c-467f-9e25-a34778f81a99",
            "usr_93d7c31d-206f-4f1a-80d2-041698fb69c4",
            "usr_dd757b6d-77e9-42e0-8d21-7ed9055ae1c0",
            "usr_7a799b5e-0d54-40e4-a95a-e92501211346",
            "usr_9141fdf0-ffda-4e90-9f33-c8087fda396d",
            "usr_aaa0d2e0-594a-48d4-be07-a17e83b88a30",
            "usr_fe69b5e2-5a39-49c1-8abc-7b1344df0732",
            "usr_4044ecd7-1aa2-4612-bb0e-ecdaf090e773",
            "usr_2ddbff5d-5eed-4572-86c0-caf9afeb95df",
            "usr_211a104d-9155-41c8-96c2-f844b689c1e0",
            "usr_1c8e1b72-dca0-46bc-939b-c7a78dc438d2",
            "usr_eff31487-89a0-4f8d-a432-996e5a92c5dc",
            "usr_3df51a70-0d2f-472e-b3b4-713527e5fdcc",
            "usr_d5ad4ee6-712a-4c4d-8059-8972170736aa",
            "usr_dec069f1-9be3-4424-aeb4-e8d2869801d5",
            "usr_fcc72fe8-3bf8-4a4e-8d1b-93662106fad2",
            "usr_ad3df2c4-d4e4-4172-a7bc-35e6f83adefa",
            "usr_2ee87f81-7017-4dee-9d92-1147d69545be",
            "usr_ea81a0f9-0650-419f-9c87-1f07693bf128",
            "usr_d5a66b1b-819f-498d-a4cc-36f8e3bb85ac",
            "usr_7df3457b-32c3-44a3-8436-84deab973df0",
            "usr_97b002cc-1892-46ba-9768-90e35156ca8d",
            "usr_851c4c08-2ea9-4fdd-8655-1402c1a6abd9",
            "usr_0d252a05-60af-43b1-af43-b13db71685ec",

            //DayClientPhotonBots
            "usr_be0ef246-3ce2-4b74-a194-8d18e8745b19",
            "usr_dd077c53-509c-4659-8194-da6404f59373",
            "usr_1cb06c3e-0a8a-4601-b000-c411fa3ef216",
            "usr_b10d0ab5-cc48-41c4-8448-f590939afea1",
            "usr_8661344f-4822-4246-b6a7-804f31690d51",
            "usr_765f21f6-bd02-4571-aa35-4a6f39163ffd",
            "usr_ad15b7fa-05fe-4e81-8ac6-28f218318bde",

            "usr_1e6946f1-b0e1-4592-9d88-426ee0e08176",
            "usr_3e525a6a-7483-4029-8830-03958f263e36",
            "usr_a880ae1f-cd6f-4117-8750-addff07bdddb",
            "usr_404ae99f-6029-48ee-9a3d-b20d1063008b",
            "usr_67a30389-a12f-4ed5-891b-9c3cbe8281cd"
        };

    }
}