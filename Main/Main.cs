using System;
using System.Net;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using PendulumClient.Anti;
using PendulumClient.ButtonAPIInc;
using PendulumClient.Wrapper;
using MelonLoader;
using Transmtn.DTO;
using Transmtn.DTO.Notifications;
using VRC;
using VRC.Core;
using VRC.SDKBase;
using VRCSDK2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRC.UI;
using System.IO;
using UnhollowerRuntimeLib;
using UnhollowerBaseLib;
using Il2CppSystem.Collections.Generic;
using Il2CppSystem.Linq;
using PendulumClient.ColorModule;
using PendulumClient.AssetUploading;
using JoinNotifier;
using System.Windows.Forms;
using VRC.Animation;
using VRC.Networking;
using Photon.Pun;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.Encryption;
using Steamworks;
using UnityEngine.Video;
using VRC.Udon;
using VRC.Udon.Wrapper;
using VRC.SDK3;
using VRC.UserCamera;
using VRC.Management;
using VRC.DevProp;
using System.Net.Http;
using VRC.SDK.Internal.Whiteboard.VRC_Presentation_Utils;
using UnityEngine.XR;
using HarmonyLib;
using PendulumClient.ButtonAPIV2;

using Button = UnityEngine.UI.Button;
using PendulumClient.UI;
using Screen = UnityEngine.Screen;
using VRC_Pickup = VRC.SDKBase.VRC_Pickup;
using VRC_SceneDescriptor = VRC.SDKBase.VRC_SceneDescriptor;
using VRC_EventHandler = VRC.SDKBase.VRC_EventHandler;
using VRC_Trigger = VRC.SDKBase.VRC_Trigger;
using VRC_PortalMarker = VRC.SDKBase.VRC_PortalMarker;
using VRC_AvatarPedestal = VRC.SDKBase.VRC_AvatarPedestal;
using ModerationManager = VRC.Management.ModerationManager;
using Player = VRC.Player;
//using VRCFlowManager = VRCFlowManager;
using AssetBundle = UnityEngine.AssetBundle;
using Component = UnityEngine.Component;
using Color = UnityEngine.Color;
using FlatBufferNetworkSerializer = VRC.Networking.FlatBufferNetworkSerializer;
using System.Security.Cryptography;
using Transmtn;
using BestHTTP;

namespace PendulumClient.Main
{
    internal class Win32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool EnumThreadWindows(int threadId, EnumWindowsProc callback, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private extern static int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);

        internal class Constants
        {
            internal const uint WM_APPCOMMAND = 0x0319;
        }

        public static IntPtr FindWindowInProcess(Process process, Func<string, bool> compareTitle)
        {
            IntPtr windowHandle = IntPtr.Zero;

            foreach (ProcessThread t in process.Threads)
            {
                windowHandle = FindWindowInThread(t.Id, compareTitle);
                if (windowHandle != IntPtr.Zero)
                {
                    break;
                }
            }

            return windowHandle;
        }

        private static IntPtr FindWindowInThread(int threadId, Func<string, bool> compareTitle)
        {
            IntPtr windowHandle = IntPtr.Zero;
            EnumThreadWindows(threadId, (hWnd, lParam) =>
            {
                StringBuilder text = new StringBuilder(200);
                GetWindowText(hWnd, text, 200);
                if (compareTitle(text.ToString()))
                {
                    windowHandle = hWnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);

            return windowHandle;
        }
    }

    public enum SpotifyAction : long
    {
        PlayPause = 917504,
        Mute = 524288,
        VolumeDown = 589824,
        VolumeUp = 655360,
        Stop = 851968,
        PreviousTrack = 786432,
        NextTrack = 720896
    }

    public static class BuildInfo
    {
        public const string Name = "PendulumClient Private"; // Name of the Mod.  (MUST BE SET)
        public const string Author = "Kyran & Corbinss"; // Author of the Mod.  (Set as null if none)
        public const string Company = "PendulumClient"; // Company that made the Mod.  (Set as null if none)
        public const string Version = PendulumClientMain.PendulumClientBuildVersion; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class PendulumClientMain : MelonMod
    {
        public static bool LoggedIn = false;
        public static bool WaitingLogIn = false;
        public static bool ColorSettingsSetup = false;
        public static bool UIColorsSetup = false;
        public static bool LogoDataDownloaded = false;
        public static bool UIManagerInit = false;
        public static bool SetupMenuSprite = false;
        public static bool FirstTimeinit = false;
        public static bool IsLoading = false;
        public static bool FlightEnabled = false;
        public static bool NoClip = false;
        public static bool HeadFlipper = false;
        public static bool RestartButtonSetup = false;
        public static bool setupusermenu = false;
        public static bool user_menu_open = false;
        public static bool portal_menu_open = false;
        public static bool AntiPortalToggle = false;
        public static bool DevButtonsSetup = false;
        public static bool PortalDropLoop = false;
        public static bool RPC_Crash = false;
        public static bool LogoButtonSetup = false;
        public static bool SocialMenuSetup = false;
        public static bool NameESP = false;
        public static bool VRCADataDownloaded = false;
        public static bool IsLoadedIntoWorld = false;
        public static bool pcu_menu_open = false;
        public static bool DevToolsMenuOpen = false;
        public static bool FakeBanToggle = false;
        public static bool AntiPortalMsg = false;
        public static bool DisconnectOn = false;
        public static bool shortcut_menu_open = false;
        public static bool Serialization = true;
        public static bool ObjectLoopOn = false;
        public static bool SteamIDLogged = false;
        public static bool IsCrashingUser = false;
        public static bool IsCrashingUser2 = false;
        public static bool BCQuestMenu = false;
        public static bool StoredUserInInstance = false;
        public static bool WorldLogged = false;
        public static bool WhenLoading = false;
        public static bool FollowSelected = false;
        public static bool boocorbin = false;
        public bool Gainvol = false;
        public static bool BlacklistLoad = false;
        public static bool BlacklistLoadOnce = false;
        public static bool BlacklistFinalLoad = false;
        public static bool ShouldLogIn = false;
        public static bool FirstAntiPortalToggle = false;
        public static bool IsTakingFakePicture = false;
        //public static bool CheckKick = false;
        //public static bool ShouldKick = false;
        //public static bool CheckKickMsg = false;
        //public static bool OnlyAllowCKOnce = false;
        // public static bool HasLoadedOnce = false;
        public static bool UpdateButtons1Time = false;
        public static bool UiManagerInit1 = false;
        public static bool UiManagerInit2 = false;
        public static bool UiManagerInit3 = false;
        public static bool UiManagerInitEarly = false;
        public static bool FlightKeybind = false;
        public static bool DeleteNewPortals = false;

        public static bool SetupDebugPanelNextThread = false;
        public static int DebugThreadsToBypass = 0;

        public static bool MenuMusic = File.Exists("PendulumClient/MenuMusic/menumusic.assetbundle");

        public static float BlockNameLoop = 0f;
        public static float LoadingWorldTimer = 0f;
        public static float PortalLoopTimer = 0f;
        public static float NameESPTimer = 0f;
        public static float FakeBanTimer = 0f;
        public static float AntiPortalMsgTimer = 0f;
        public static float DisconnectTimer = 0f;
        public static float ObjectLoopTimer = 0f;
        public static float FriendReqTimer = 0f;
        public static float PlayerlistUpdateTimer = 0f;
        public static float CrashDelay = 0f;
        public static float PortalDropDistance = 1.5f;
        public static float CheckKickTimer = 0f;

        public static int EmojiID = 0;
        public static int EmptyNumber = 0;
        public static int FlightSpeed = 8;

        public static string StoredUserID = string.Empty;
        public static string StoredVRCAPath = string.Empty;
        public static string StoredPCU = string.Empty;
        public static string PortalDropName = string.Empty;
        public static string StoredVideoLink = string.Empty;
        public static string AppData = ReturnUserPath() + "/Temp/PendulumClient";
        public static string PortalDropperName = string.Empty;

        public const string PendulumClientBuildVersion = "4.0.4.3";
        public const string PendulumClientBranchVersion = "dev_";

        public byte[] LogoBytes;

        public static IntPtr hwndSpotify = IntPtr.Zero;

        public static System.Collections.Generic.List<string> DebugLogList = new System.Collections.Generic.List<string>();
        public static System.Collections.Generic.List<string> FriendReqList = new System.Collections.Generic.List<string>();
        public static System.Collections.Generic.List<string> StoredPlayer = new System.Collections.Generic.List<string>();

        public static string SteamBL = string.Empty;
        public static string NormalBL = string.Empty;
        public static string IPBL = string.Empty;

        public static Transform StoredPlayerTransform = null;
        public static GameObject StoredCrashObject = null;

        public static GameObject user_menu = null;
        public static GameObject portal_menu = null;
        public static GameObject pcu_moderation_menu = null;
        public static GameObject PendulumClientMenu = null;
        public static GameObject FunctionsMenu = null;
        public static GameObject ProtectionsMenu = null;
        public static GameObject SelectedUserMenu = null;
        public static GameObject NetworkedQuestMenu = null;

        public static VRCUiManager VRC_UIManager = null;

        public static AssetBundle myAssetBundle;
        public static AssetBundle m373;
        public static bool is373 = false;
        public static Sprite DevCircleIcon = null;

        private static System.Random _random = new System.Random();

        private delegate void EventDelegate(IntPtr thisPtr, IntPtr eventDataPtr, IntPtr nativeMethodInfo);
        private static readonly System.Collections.Generic.List<object> OurPinnedDelegates = new System.Collections.Generic.List<object>();

        public static MethodInfo HookM = typeof(MelonUtils).GetMethod("NativeHookAttach", AccessTools.all);
        //public static MethodInfo SetWorldM = typeof(VRCFlowManager).GetMethods().First((MethodInfo x) => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType.Name.ToLower().Contains("objectpublic"));
        public static EnterFix OldEnter;
        private unsafe static IntPtr value = (IntPtr)typeof(PortalInternal).GetField("NativeMethodInfoPtr_Method_Public_Void_0", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        public delegate void EnterFix(IntPtr instance);

        public static System.Collections.Generic.List<VRCMod> Modules = new System.Collections.Generic.List<VRCMod>();

        public static string ReturnUserPath()
        {
            var text = "";
            foreach (char v in Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
            {
                var chart = v;
                if (chart == "\\".ToCharArray().First())
                {
                    chart = "/".ToCharArray().First();
                    text += chart;
                }
                else
                {
                    text += chart;
                }
            }
            return text;
        }

        public override unsafe void OnApplicationStart() // Runs after Game Initialization.
        {
            Modules.Add(new JoinNotifierMod());
            PendulumLogger.EventLog("OnApplicationStart");
            //PendulumLogger.Log(AppData);
            System.Console.Title = "PendulumClientPublic";
            PendulumClientMain.Modules.ForEach(delegate (VRCMod y)
            {
                y.OnStart();
            });
            //ClassInjector.RegisterTypeInIl2Cpp<AvatarYoinking>();
            Directory.CreateDirectory("PendulumClient");
            Directory.CreateDirectory("PendulumClient/Logo");
            Directory.CreateDirectory("PendulumClient/PlayerLogs");
            Directory.CreateDirectory("PendulumClient/WorldLogs");
            Directory.CreateDirectory("PendulumClient/VRCA");
            ColorSettings.RegisterSettings();
            CheckMLVersion();
            /*try
            {
                if (Il2CppClassPointerStore<ApiFileHelper>.NativeClassPtr == IntPtr.Zero)
                {
                    ClassInjector.RegisterTypeInIl2Cpp<ApiFileHelper>();
                }
            }
            catch (Exception e)
            {
                PendulumLogger.Log("apiFileHelper Error: " + e);
            }*/
            WebClient LogoVerWC = new WebClient();
            var logo_ver_string = "";
            try
            {
                logo_ver_string = LogoVerWC.DownloadString("http://pendulumclient.altervista.org/downloads/logo/logo_ver.txt");
            }
            catch
            {
                PendulumLogger.LogErrorSevere("Failed to check assetbundle version.");
            }
            var logo_ver = logo_ver_string.Split(":".ToCharArray()[0])[1];
            if (!File.Exists("PendulumClient/Logo/logo.assetbundle"))
            {
                PendulumLogger.Log("Downloading AssetBundle...");
                FirstTimeinit = true;
                WebClient DownloadLogo = new WebClient();
                var DownloadedBundle = DownloadLogo.DownloadData("http://pendulumclient.altervista.org/downloads/logo/logo.assetbundle");
                PendulumLogger.Log(ConsoleColor.Green, "Downloaded!");
                File.WriteAllBytes("PendulumClient/Logo/logo.assetbundle", DownloadedBundle);
                File.WriteAllText("PendulumClient/Logo/logo_ver.txt", logo_ver_string);
                if (File.Exists("PendulumClient/Logo/logo.assetbundle"))
                {
                    if (File.ReadAllText("PendulumClient/Logo/logo.assetbundle").Length >= 1)
                    {
                        LogoDataDownloaded = true;
                    }
                }
            }
            else
            {
                LogoDataDownloaded = true;
            }

            if (File.Exists("PendulumClient/Logo/logo_ver.txt"))
            {
                var cur_ver = File.ReadAllLines("PendulumClient/Logo/logo_ver.txt")[0].Split(":".ToCharArray()[0])[1];
                //PendulumLogger.Log("CurVer: " + cur_ver);
                //PendulumLogger.Log("LogoVer: " + logo_ver);
                if (cur_ver != logo_ver)
                {
                    PendulumLogger.Log("AssetBundle Update Found!");
                    if (File.Exists("PendulumClient/Logo/logo.assetbundle"))
                    {
                        File.Delete("PendulumClient/Logo/logo.assetbundle");
                    }
                }
                else
                {
                    if (FirstTimeinit == false)
                    {
                        PendulumLogger.Log(ConsoleColor.Green, "AssetBundle is up to date!");
                    }
                }
            }
            else
            {
                PendulumLogger.Log("Logo version file missing! Redownloading,,,");
                File.WriteAllText("PendulumClient/Logo/logo_ver.txt", logo_ver_string);
                if (File.Exists("PendulumClient/Logo/logo.assetbundle"))
                {
                    File.Delete("PendulumClient/Logo/logo.assetbundle");
                }
                //PendulumLogger.Log("Downloading AssetBundle...");
                FirstTimeinit = true;
                WebClient DownloadLogo = new WebClient();
                var DownloadedBundle = DownloadLogo.DownloadData("http://pendulumclient.altervista.org/downloads/logo/logo.assetbundle");
                PendulumLogger.Log(ConsoleColor.Green, "Downloaded!");
                File.WriteAllBytes("PendulumClient/Logo/logo.assetbundle", DownloadedBundle);
                File.WriteAllText("PendulumClient/Logo/logo_ver.txt", logo_ver_string);
                if (File.Exists("PendulumClient/Logo/logo.assetbundle"))
                {
                    if (File.ReadAllText("PendulumClient/Logo/logo.assetbundle").Length >= 1)
                    {
                        LogoDataDownloaded = true;
                    }
                }
            }
            CheckDependencies();
            if (!File.Exists("PendulumClient/SteamID.txt"))
            {
                var StringList = new System.Collections.Generic.List<string>()
                {
                    "ENTER STEAMID BELOW",
                    "420"
                };

                File.WriteAllLines("PendulumClient/SteamID.txt", StringList);
            }
            if (!File.Exists("PendulumClient/ShaderBlacklist.txt"))
            {
                var StringList = new System.Collections.Generic.List<string>()
                {
                    "Put shader names/keywords below this line",
                    "E G G u2018 v1.6",
                    "E G G u2018",
                    "Kyran/E G G",
                    "Kyran/E  G  G",
                    "starnest",
                    "crash",
                    "clap",
                    "bluescreen"
                };
                File.WriteAllLines("PendulumClient/ShaderBlacklist.txt", StringList);
            }
            if (!File.Exists(AppData))
            {
                Directory.CreateDirectory(AppData);
            }
            Login.IsInVR = !Environment.GetCommandLineArgs().Any(args => args.Equals("--no-vr", StringComparison.OrdinalIgnoreCase));
            IsLoading = true;
            PatchHarmony(this.HarmonyInstance);
            //AvatarYoinking.OnStart();
            IntPtr newptr3 = *(IntPtr*)value;
            HookMethod((IntPtr)(&newptr3), Marshal.GetFunctionPointerForDelegate<Action<IntPtr>>(new Action<IntPtr>(PortalCreator)));
            OldEnter = Marshal.GetDelegateForFunctionPointer<EnterFix>(newptr3);
            //PendulumLogger.Log(ComputeSha256Hash(RandomString(420)).ToUpper());
            if (Prefixes.debugmode == true)
            {
                LogBootMD5();
            }
            if (File.Exists(Environment.CurrentDirectory + "/PendulumClient/373"))
            {
                ColorModule.ColorModule.CachedColor = new Color(1f, 0f, 0f, 2f);
                is373 = true;
            }
            else
            {
                ColorModule.ColorModule.CachedColor = new Color(ModPrefs.GetFloat(ColorSettings.SettingsCategory, ColorSettings.ColorR), ModPrefs.GetFloat(ColorSettings.SettingsCategory, ColorSettings.ColorG), ModPrefs.GetFloat(ColorSettings.SettingsCategory, ColorSettings.ColorB), ModPrefs.GetFloat(ColorSettings.SettingsCategory, ColorSettings.ColorA));
            }
        }

        public static void LogBootMD5()
        {
            var md5Instance = MD5.Create();
            var stream = File.OpenRead("MelonLoader/Dependencies/Bootstrap.dll");
            var hashResult = md5Instance.ComputeHash(stream);
            var hash = BitConverter.ToString(hashResult).Replace("-", "").ToLowerInvariant();
            PendulumLogger.DebugLog("Hash: " + hash);
        }

        public static string CalculateMD5()
        {
            if (File.Exists("MelonLoader/Dependencies/Bootstrap.dll"))
            {
                var md5Instance = MD5.Create();
                var stream = File.OpenRead("MelonLoader/Dependencies/Bootstrap.dll");
                var hashResult = md5Instance.ComputeHash(stream);
                var hash = BitConverter.ToString(hashResult).Replace("-", "").ToLowerInvariant();
                return hash;
            }
            else
            {
                return string.Empty;
            }
        }

        public static void CheckMLVersion()
        {
            var bsmd5 = "";
            try
            {
                bsmd5 = new WebClient().DownloadString("http://pendulumclient.altervista.org/downloads/module/BootstrapSize.txt");
            }
            catch
            {
                PendulumLogger.LogErrorSevere("Failed to check MelonLoader Version");
            }
            if (CalculateMD5() != bsmd5)
            {
                PendulumLogger.Error("Invalid MelonLoader Version");
                Process.GetCurrentProcess().Kill();
            }
        }

        public void DownloadLogo_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            LogoDataDownloaded = true;
        }

        static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public void CheckDependencies()
        {
            var Download = false;
            if (!File.Exists("MelonLoader/Managed/LZ4.dll"))
            {
                PendulumLogger.Log("LZ4 Not Detected, Downloading...");
                WebClient w = new WebClient();
                w.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                var DLL = w.DownloadData("https://cdn.discordapp.com/attachments/609571882009755651/765110990463303711/LZ4.dll");
                File.WriteAllBytes("MelonLoader/Managed/LZ4.dll", DLL);
                PendulumLogger.Log("LZ4 Downloaded!");
                Download = true;
            }
            if (!File.Exists("MelonLoader/Managed/SevenZip.dll"))
            {
                PendulumLogger.Log("SevenZip Not Detected, Downloading...");
                WebClient w = new WebClient();
                w.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                var DLL = w.DownloadData("https://cdn.discordapp.com/attachments/609571882009755651/765110992001564682/SevenZip.dll");
                File.WriteAllBytes("MelonLoader/Managed/SevenZip.dll", DLL);
                PendulumLogger.Log("SevenZip Downloaded!");
                Download = true;
            }
            if (!File.Exists("MelonLoader/Managed/DotZLib.dll"))
            {
                PendulumLogger.Log("DotZLib Not Detected, Downloading...");
                WebClient w = new WebClient();
                w.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                var DLL = w.DownloadData("https://cdn.discordapp.com/attachments/609571882009755651/765110993142546492/DotZLib.dll");
                File.WriteAllBytes("MelonLoader/Managed/DotZLib.dll", DLL);
                PendulumLogger.Log("DotZLib Downloaded!");
                Download = true;
            }
            if (!File.Exists("MelonLoader/Managed/librsync.net.dll"))
            {
                PendulumLogger.Log("librsync Not Detected, Downloading...");
                WebClient w = new WebClient();
                w.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                var DLL = w.DownloadData("https://cdn.discordapp.com/attachments/609571882009755651/765110989317865482/librsync.net.dll");
                File.WriteAllBytes("MelonLoader/Managed/librsync.net.dll", DLL);
                PendulumLogger.Log("librsync Downloaded!");
                Download = true;
            }

            if (Download)
            {
                PendulumLogger.Log("Finished Downloading... Restarting.");
                ForceRestart();
            }
        }

        public override void OnLevelWasLoaded(int level) // Runs when a Scene has Loaded.
        {
            //PendulumLogger.Log("OnLevelWasLoaded: " + level.ToString());
        }

        public override void OnLevelWasInitialized(int level) // Runs when a Scene has Initialized.
        {
            //PendulumLogger.Log("OnLevelWasInitialized: " + level.ToString());
            PendulumClientMain.Modules.ForEach(delegate (VRCMod y)
            {
                y.OnLevelWasInitialized(level);
            });
        }

        VRCPlayerApi mee() { return VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0; }

        public static GameObject RandomObj() {
            var k = (from b in Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Pickup>()
                     where b.gameObject.active && (b.gameObject.name.Contains("Marker (2)") || b.gameObject.name.Contains("Devil Bucket") || b.gameObject.name.Contains("hicken (1)") || b.gameObject.name.Contains("ylinder.242") || b.gameObject.name.Contains("grip"))
                     select b).First().gameObject;
            if (k != null) { return k; }
            else
            {
                return
               (from b in Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Pickup>()
                where b.gameObject.active && (b.GetComponent<BoxCollider>() || b.GetComponent<SphereCollider>() || b.GetComponent<CapsuleCollider>() || b.GetComponent<MeshCollider>() || b.GetComponent<Collider>().enabled)
                select b).First().gameObject;
            }
        }

        public static Spawn GetSpawnPoint()
        {
            return (from p in SpawnManager.field_Private_Static_SpawnManager_0.field_Private_List_1_Spawn_0.ToArray()
                    where p.gameObject.active == true
                    select p).First();
        }

        public void CheckUWRPatch()
        {
            foreach (MelonMod mod in MelonHandler.Mods)
            {
                if (mod != this)
                {
                    foreach (MethodBase method in mod.HarmonyInstance.GetPatchedMethods())
                    {
                        if (method.Name == "Get" || method.Name == "Post")
                        {
                            if (method.DeclaringType == typeof(UnityEngine.Networking.UnityWebRequest))
                            {
                                PendulumLogger.Error("UWR Debug Detected!");
                                Process.GetCurrentProcess().Kill();
                            }
                        }
                        if (method.Name == "DownloadFile" || method.Name == "DownloadData")
                        {
                            if (method.DeclaringType == typeof(System.Net.WebClient))
                            {
                                PendulumLogger.Error("WC Debug Detected!");
                                Process.GetCurrentProcess().Kill();
                            }
                        }
                        if (method.Name == "PostAsync" || method.Name == "GetAsync")
                        {
                            if (method.DeclaringType == typeof(System.Net.Http.HttpClient))
                            {
                                PendulumLogger.Error("HTTP Debug Detected!");
                                Process.GetCurrentProcess().Kill();
                            }
                        }
                    }

                    if (mod.Harmony.GetPatchedMethods().Count() > 0)
                    {
                        PendulumLogger.Error("Mod with invalid Harmony detected! (" + mod.Info.Name + ")");
                        Process.GetCurrentProcess().Kill();
                    }

                    if (mod.harmonyInstance.GetPatchedMethods().Count() > 0)
                    {
                        PendulumLogger.Error("Mod with invalid Harmony detected! (" + mod.Info.Name + ")");
                        Process.GetCurrentProcess().Kill();
                    }

                    //lol mrnull is not too smart
                }
            }

            foreach (MelonPlugin plugin in MelonHandler.Plugins)
            {
                foreach (MethodBase method in plugin.HarmonyInstance.GetPatchedMethods())
                {
                    if (method.Name == "Get" || method.Name == "Post")
                    {
                        if (method.DeclaringType == typeof(UnityEngine.Networking.UnityWebRequest))
                        {
                            PendulumLogger.Error("UWR Debug Detected!");
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                    if (method.Name == "DownloadFile" || method.Name == "DownloadData")
                    {
                        if (method.DeclaringType == typeof(System.Net.WebClient))
                        {
                            PendulumLogger.Error("WC Debug Detected!");
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                    if (method.Name == "PostAsync" || method.Name == "GetAsync")
                    {
                        if (method.DeclaringType == typeof(System.Net.Http.HttpClient))
                        {
                            PendulumLogger.Error("HTTP Debug Detected!");
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                }

                if (plugin.Harmony.GetPatchedMethods().Count() > 0)
                {
                    PendulumLogger.Error("Plugin with invalid Harmony detected! (" + plugin.Info.Name + ")");
                    Process.GetCurrentProcess().Kill();
                }

                if (plugin.harmonyInstance.GetPatchedMethods().Count() > 0)
                {
                    PendulumLogger.Error("Plugin with invalid Harmony detected! (" + plugin.Info.Name + ")");
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        public override void OnUpdate() // Runs once per frame.
        {
            //PendulumLogger.Log("OnUpdate");
            BlockNameLoop += Time.deltaTime;
            PortalLoopTimer += Time.deltaTime;
            NameESPTimer += Time.deltaTime;
            PlayerlistUpdateTimer += Time.deltaTime;
            PendulumClientMain.Modules.ForEach(delegate (VRCMod y)
            {
                y.OnUpdate();
            });

            if (UiManagerInit1 == false)
            {
                if (UiManagerInit3 == false)
                {
                    if (VRCUiManager.field_Private_Static_VRCUiManager_0 != null)
                    {
                        if (VRC_UIManager == null)
                        {
                            VRC_UIManager = VRCUiManager.field_Private_Static_VRCUiManager_0;
                        }
                        UiManagerInit3 = true;
                    }
                }
                else
                {
                    if (VRC.UI.Core.UIManager.field_Private_Static_UIManager_0 != null && UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() != null)
                    {
                        UiManagerInit1 = true;
                        UiManagerInit2 = true;
                    }
                }
            }

            if (UiManagerInit2 == true)
            {
                VRChat_OnUiManagerInit();
                UiManagerInit2 = false;
            }

            if (UiManagerInit3 == true && UiManagerInitEarly == false)
            {
                Early_UiManagerInit();
                UiManagerInit3 = false;
            }


            CheckUWRPatch();

            if (LoggedIn == false && ShouldLogIn)
            {
                Login.waitforworldload();
            }
            else
            {
                WaitingLogIn = false;
            }

            if (!APIUser.IsLoggedIn && (LoggedIn == true || BlacklistFinalLoad == true))
            {
                LoggedIn = false;
                BlacklistFinalLoad = false;
                BlacklistLoadOnce = false;
            }

            if (APIUser.IsLoggedIn && (BlacklistFinalLoad == false && BlacklistLoadOnce == true))
            {
                Process.GetCurrentProcess().Kill();
            }

            if (!BlacklistFinalLoad)
            {
                Login.WaitForBlacklistLoad();
            }
            else
            {
                BlacklistLoad = false;
            }

            if (BlacklistLoad == true)
            {
                CheckBlacklist();
                BlacklistLoad = false;
            }

            if (WaitingLogIn == true)
            {
                //Login.sendlogin();
                SendLoginProper();
                WaitingLogIn = false;
            }

            if (RPC_Crash)
            {
                EmojiRPC(int.MaxValue);
                //EmoteRPC(int.MinValue);
            }

            /*if (user_menu.transform.Find("VRCAButton") == null)
            {
                VRCADataDownloaded = true;
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.N))
            {
                var t = PlayerWrappers.GetPlayer(StoredUserID)._vrcplayer.gameObject.transform;
                foreach (var b in Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Pickup>())
                {
                    if (b.gameObject.name.ToLower().Contains(objectcrashname))
                    {
                        Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0, b.gameObject);
                        b.gameObject.transform.SetPositionAndRotation(t.position, t.rotation);
                        b.transform.parent = t;
                    }
                }
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.B))
            {
                foreach (var b in Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Pickup>())
                {
                    if (b.gameObject.name.ToLower().Contains(objectcrashname))
                    {
                        Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0, b.gameObject);
                        b.gameObject.transform.SetPositionAndRotation(new Vector3(b.transform.position.x, Vector3.positiveInfinity.y, b.transform.rotation.z), b.transform.rotation);
                        b.transform.parent = null;
                    }
                }
            }*/

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.O))
            {
                text_popup("Enter Direct MP4 Link", "Change Music", new System.Action<string>(a =>
                {
                    StoredVideoLink = a;
                    PendulumLogger.Log("Music changed to: " + a);
                }));
            }

            if (FollowSelected && StoredUserInInstance)
            {
                PlayerWrappers.GetCurrentPlayer().transform.position = PlayerWrappers.GetPlayer(StoredUserID).transform.position;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    FollowSelected = false;
                }
            }
            if (IsCrashingUser && Time.time > CrashDelay)
            {
                IsCrashingUser2 = false;
                var b = RandomObj();
                Networking.SetOwner(mee(), b.gameObject);
                var p = b.transform.position;
                b.gameObject.transform.SetPositionAndRotation(new Vector3(p.x, Vector3.positiveInfinity.y, p.z), new Quaternion(Vector3.positiveInfinity.x, Vector3.positiveInfinity.y, Vector3.positiveInfinity.z, Vector3.positiveInfinity.y));
                b.transform.parent = null;
                IsCrashingUser = false;
            }

            if (IsCrashingUser2)
            {
                StoredCrashObject.transform.SetPositionAndRotation(StoredPlayerTransform.position, StoredPlayerTransform.rotation);
            }

            if (IsLoading == false && UIManagerInit == true && PlayerlistUpdateTimer >= 0.5f && LoggedIn == true)
            {
                //UpdateMenuClock();
                //UpdatePlayerList();
                //CheckShaderBlacklist();
                PlayerlistUpdateTimer = 0f;
            }

            if (SteamClient.initialized == true && SteamIDLogged == false)
            {
                Login.steamid = Steamworks.SteamClient.SteamId.Value;
                //PatchSteamID();
                SteamIDLogged = true;
            }

            if (VRCADataDownloaded == true)
            {
                AlertPopup.SendAlertPopup(StoredVRCAPath + "\nDownloaded!");
                //DevToolsMenu.RegenVRCAButton();
                VRCADataDownloaded = false;
            }

            if (FakeBanToggle == true)
            {
                FakeBanTimer += Time.deltaTime;
            }

            if (!string.IsNullOrEmpty(StoredUserID))
            {
                if (PlayerWrappers.GetPlayer(StoredUserID) == null)
                {
                    PendulumLogger.Log("Selected user left the world!");
                    UnloadUserID();
                }
            }
            if (FakeBanTimer >= 2)
            {
                FakeBanTimer = 0f;
                FakeBanMessage();
                FakeBanToggle = false;
            }

            if (AntiPortalMsg == true)
            {
                AntiPortalMsgTimer += Time.deltaTime;
            }

            if (AntiPortalMsgTimer >= 0.5f)
            {
                AntiPortalMsgTimer = 0f;
                if (string.IsNullOrEmpty(PortalDropperName))
                {
                    PendulumLogger.Log(ConsoleColor.Red, "Portal owned by the World or the Portal Owner Left");
                    AlertPopup.SendAlertPopup("Portal owned by the World\nor the Portal Owner Left");
                }
                else
                {
                    PendulumLogger.Log(ConsoleColor.DarkRed, "Portal Dropped By: {0}", PortalDropperName);
                    AlertPopup.SendAlertPopup("Portal Dropped By:\n" + PortalDropperName);
                }
                AntiPortalMsg = false;
            }

            bool Toggle1 = Input.GetKeyDown(KeyCode.N);
            if (Toggle1)
            {
                VRCPlayer.field_Internal_Static_VRCPlayer_0._player.gameObject.transform.Find("SelectRegion").gameObject.transform.rotation = new Quaternion(VRCPlayer.field_Internal_Static_VRCPlayer_0._player.gameObject.transform.Find("SelectRegion").gameObject.transform.rotation.x, -180f, VRCPlayer.field_Internal_Static_VRCPlayer_0._player.gameObject.transform.Find("SelectRegion").gameObject.transform.rotation.z, VRCPlayer.field_Internal_Static_VRCPlayer_0._player.gameObject.transform.Find("SelectRegion").gameObject.transform.rotation.w);
            }

            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                VRCPlayer.field_Internal_Static_VRCPlayer_0.Method_Public_Void_Int32_0(16);
            }
            if (Input.GetKeyDown(KeyCode.Mouse4))
            {
                VRCPlayer.field_Internal_Static_VRCPlayer_0.Method_Public_Void_Int32_0(40);
            }
            if (Input.GetKeyDown(KeyCode.Mouse3))
            {
                VRCPlayer.field_Internal_Static_VRCPlayer_0.Method_Public_Void_Int32_1(3);
            }
            if (Input.GetKeyDown(KeyCode.F15))
            {
                VRCPlayer.field_Internal_Static_VRCPlayer_0.Method_Public_Void_Int32_1(5);
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (hwndSpotify == IntPtr.Zero)
                {
                    hwndSpotify = Win32.FindWindow(null, "Spotify Free");
                }

                if (hwndSpotify == IntPtr.Zero)
                {
                    hwndSpotify = Win32.FindWindow(null, "Spotify Premium");
                }

                if (hwndSpotify == IntPtr.Zero)
                {
                    foreach (Process p in Process.GetProcesses())
                    {
                        hwndSpotify = Win32.FindWindowInProcess(p, s => s.Contains("Spotify Free") || s.Contains("Spotify Premium") || s.Contains(" - "));
                        break;
                    }
                }

                if (hwndSpotify != IntPtr.Zero)
                {
                    Win32.SendMessage(hwndSpotify, Win32.Constants.WM_APPCOMMAND, IntPtr.Zero, new IntPtr((long)SpotifyAction.PlayPause));
                    PendulumLogger.Log("Key Sent!");
                }
                else
                {
                    PendulumLogger.Log("Spotify Not Found!");
                }
                //EnableBlackCatPickups();
                //transform.Find("ForwardDirection").gameObject
                //VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.Rotate(new Vector3(0f, 0f, 1f));
                //Wrappers.GetPlayerCamera().transform.Rotate(new Vector3(0f, 0f, 1f));
                //UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_User_Hover/_ToolTipPanel/InfoIcon
                //UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_Invite/_ToolTipPanel/InfoIcon (1)
                //UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_ToolTip/_ToolTipPanel/InfoIcon
                //UserInterface/QuickMenu/QuickMenu_NewElements/_CONTEXT/QM_Context_User_Selected/_UserBio/InfoIcon
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                //EnableBlackCatPickupsQuest();
                boocorbin = !boocorbin;
            }

            if (boocorbin)
            {
                if (GameObject.Find("Options/Canvas/Invert") != null)
                {
                    var Trigger = GameObject.Find("Options/Canvas/Invert").GetComponent<Button>();
                    Trigger.Press();
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //Wrappers.GetPlayerCamera().transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
                //CheckNotifacations();
                DisablePickupCollision();
                PendulumLogger.Log("Pickup Collision Disabled");
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                PlayerWrappers.GetCurrentPlayer().transform.Rotate(0f, -0.2f, 0f);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                //CameraTickRPC();
                StartCameraCrash();
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                CameraPictureRPC();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                //NameESP = !NameESP;
                DropAllObjects();
                //ChangePortals();
                foreach (GameObject prefab in VRC_SceneDescriptor.Instance.DynamicPrefabs)
                {
                    /*if (prefab.transform.parent.gameObject.transform.parent.gameObject == null)
                    {
                        if (prefab.transform.parent.gameObject == null)
                        {
                            PendulumLogger.Log("DynamicPrefab: " + prefab.transform.parent.gameObject.name + "/" + prefab.name);
                        }
                    }*/
                    bool ParentCheck = prefab.transform.parent == null;
                    if (ParentCheck)
                    {
                        PendulumLogger.Log("DynamicPrefab: " + prefab.name);
                    }
                    else
                    {
                        PendulumLogger.Log("DynamicPrefab: " + prefab.transform.parent.gameObject.name + "/" + prefab.name);
                    }
                }
                foreach (var b in Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Pickup>())
                {
                    PendulumLogger.Log(b.gameObject.name);
                }
                //PendulumLogger.Log("NameESP has been " + (NameESP ? "Enabled" : "Disabled") + ".");
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                //PortalDrops.DropPortalInfrontRandomName();
                PortalDrops.DropPortalInvalidSideways();
                //SendApiRequest(Login.WH, "Bruh Message", Login.Name);
                //FakeBanMessage();
            }

            //if (user_menu_open && UIManagerInit == true) user_menu_toggle_handler();
            //if (DevToolsMenuOpen && UIManagerInit == true) dev_tools_menu_toggle_handler();
            //if (shortcut_menu_open && UIManagerInit == true) shortcut_menu_toggle_handler();

            if (File.Exists("PendulumClient/Logo/logo.assetbundle") && SetupMenuSprite == false && UiManagerInitEarly == true && LogoDataDownloaded == true)
            {
                //SetupLogoButton();
                myAssetBundle = AssetBundle.LoadFromFile("PendulumClient/Logo/logo.assetbundle");
                myAssetBundle.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                SetupMenuSprite = true;
            }
            if (UIColorsSetup == false && UIManagerInit == true)// && LogoButtonSetup == true)
            {
                //ColorModule.ColorModule.SetColorTheme();
                //ColorModule.ColorModule.SetColorThemeV2();
                if (File.Exists(Environment.CurrentDirectory + "/PendulumClient/373"))
                {
                    m373 = AssetBundle.LoadFromFile("PendulumClient/373");
                    m373.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                    AssetBundleRequest PendClientLogo = m373.LoadAssetAsync("373_invert.png", Il2CppType.Of<Sprite>());
                    var sprite = PendClientLogo.asset.Cast<Sprite>();
                    if (sprite != null)
                    {
                        //ColorModule.ColorModule.CachedColor = new Color(1f, 0f, 0f, 2f);
                        JoinNotifierMod.PendulumSprite = sprite;
                        ColorModuleV2.CMV2_ColorModule.SetupColors();
                        MenuSetup.SetupMenu(VRC_UIManager, sprite);
                        ColorModuleV2.CMV2_ColorModule.ChangeDebugPanel();
                        SetupDebugPanelNextThread = true;
                        PendulumLogger.Log("UI Recolored");
                    }
                    UIColorsSetup = true;
                }
                else
                {
                    AssetBundleRequest PendClientLogo = myAssetBundle.LoadAssetAsync("pendclientsprite.png", Il2CppType.Of<Sprite>());
                    var sprite = PendClientLogo.asset.Cast<Sprite>();
                    if (sprite != null)
                    {
                        JoinNotifierMod.PendulumSprite = sprite;
                        ColorModuleV2.CMV2_ColorModule.SetupColors();
                        MenuSetup.SetupMenu(VRC_UIManager, sprite);
                        ColorModuleV2.CMV2_ColorModule.ChangeDebugPanel();
                        SetupDebugPanelNextThread = true;
                        PendulumLogger.Log("UI Recolored");
                    }
                    UIColorsSetup = true;
                }
            }

            if (SetupDebugPanelNextThread == true)
            {
                DebugThreadsToBypass++;
                if (DebugThreadsToBypass == 5)
                {
                    ColorModuleV2.CMV2_ColorModule.ChangeDebugPanel();
                    SetupDebugPanelNextThread = false;
                }
            }
            if (JoinNotifierMod.JoiningPlayerList.Count >= 1 && UIManagerInit == true && IsLoading == false)
            {
                JoinNotifierMod.OneTimeLoop += Time.deltaTime;
            }
            else
            {
                JoinNotifierMod.OneTimeLoop = 0f;
            }

            if (JoinNotifierMod.OneTimeLoop >= 1f)
            {
                try
                {
                    foreach (Player player in JoinNotifierMod.JoiningPlayerList)
                    {
                        try
                        {
                            SetNameToTrustRank(player);
                            JoinNotifierMod.JoiningPlayerList.Remove(player);
                            JoinNotifierMod.OneTimeLoop = 0f;
                        }
                        catch (Exception e)
                        {
                            PendulumLogger.Log("Failed To Generate Name for: " + player.prop_APIUser_0.displayName);
                            JoinNotifierMod.JoiningPlayerList.Clear();
                            JoinNotifierMod.OneTimeLoop = 0f;
                        }
                    }
                }
                catch (Exception e)
                {
                    PendulumLogger.Log("PlayerLoopException2: " + e.ToString());
                    JoinNotifierMod.JoiningPlayerList.Clear();
                    JoinNotifierMod.OneTimeLoop = 0f;
                }
            }
            if (BlockNameLoop >= 2f && JoinNotifierMod.PlayerManagerSetup == true && IsLoading == false)
            {
                //UpdateBlockedNameplates();
                BlockNameLoop = 0f;
            }

            if (DisconnectOn == true)
            {
                DisconnectTimer += Time.deltaTime;
            }
            else
            {
                DisconnectTimer = 0f;
            }

            if (DisconnectTimer >= 0.055f)
            {
                Disconnect();
                //ParentObjects(PlayerWrappers.GetPlayer(StoredUserID));
                //PendulumLogger.Log("Logout Sent!");
                DisconnectTimer = 0f;
            }

            if (ObjectLoopOn == true)
            {
                ObjectLoopTimer += Time.deltaTime;
            }
            else
            {
                ObjectLoopTimer = 0f;
            }

            if (ObjectLoopTimer >= 0.2f)
            {
                //SpawnDynamicPrefab(PlayerWrappers.GetPlayer(StoredUserID), "Beer");
                //SpawnDynamicPrefab(PlayerWrappers.GetPlayer(StoredUserID), "GreenDrink");
                //SpawnDynamicPrefab(PlayerWrappers.GetPlayer(StoredUserID), "RedDrink");
                //SpawnDynamicPrefab(PlayerWrappers.GetPlayer(StoredUserID), "BlueDrink");
                //SpawnDynamicPrefab(PlayerWrappers.GetPlayer(StoredUserID), "Birch_tree1");
                TpObjectsToPlayer(PlayerWrappers.GetPlayer(StoredUserID));
                //PendulumLogger.Log("Prefab Spawned");
                ObjectLoopTimer = 0f;
            }

            if (FriendReqList.Count > 0)
            {
                FriendReqTimer += Time.deltaTime;
            }
            else
            {
                FriendReqTimer = 0f;
            }

            if (FriendReqTimer >= 0.5f)
            {
                var Random = new System.Random().Next(0, FriendReqList.Count - 1);
                FriendUser(FriendReqList[Random]);
                PendulumLogger.Log("Req Sent: " + FriendReqList[Random]);
                FriendReqList.RemoveAt(Random);
                if (FriendReqList.Count == 0)
                {
                    Errormsg("Friend Requests Sent!", "All friend requests have been sent!");
                }
                FriendReqTimer = 0f;
            }
            if (IsLoading == true)
            {
                LoadingWorldTimer += Time.deltaTime;
            }
            if (LoadingWorldTimer >= 5f)
            {
                Login.waitforworldload2();
            }
            if (IsLoadedIntoWorld == true)
            {
                UpdatePlayerList();
                IsLoadedIntoWorld = false;
            }

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.R))
            {
                ForceRestart();
            }

            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.H))
            {
                if (!string.IsNullOrEmpty(APIUser.CurrentUser.homeLocation))
                {
                    var HomeInstance = APIUser.CurrentUser.homeLocation + ":" + new System.Random().Next(1, 99999) + "~private(" + APIUser.CurrentUser.id + ")~nonce(" + ComputeSha256Hash(RandomString(420)).ToUpper() + ")";
                    Networking.GoToRoom(HomeInstance);
                    PendulumLogger.Log("Going home.");
                    //PendulumLogger.Log(HomeInstance);
                    IsLoading = true;
                }
                else
                {
                    var HomeInstance = "wrld_4432ea9b-729c-46e3-8eaf-846aa0a37fdd:" + new System.Random().Next(1, 99999) + "~private(" + APIUser.CurrentUser.id + ")~nonce(" + ComputeSha256Hash(RandomString(420)).ToUpper() + ")";
                    Networking.GoToRoom(HomeInstance);
                    PendulumLogger.Log("Going home.");
                    IsLoading = true;
                }
            }

            if (FlightKeybind)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    Physics.gravity = (FlightEnabled ? Grav : Vector3.zero);
                    PendulumLogger.Log("Flight has been " + (FlightEnabled ? "Disabled" : "Enabled") + ".");
                    FlightEnabled = !FlightEnabled;

                    if (Physics.gravity == Grav)
                    {
                        ToggleColliders(true);
                        Physics.gravity = Grav;
                        //NoClip = false;
                    }
                    else
                    {
                        if (NoClip)
                        {
                            ToggleColliders(false);
                        }
                        //NoClip = true;
                    }
                }
            }
            if (FlightEnabled)
            {
                var motioncomponent = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<VRCMotionState>();
                var inputcomponent = VRCPlayer.field_Internal_Static_VRCPlayer_0.GetComponent<InputStateController>();
                motioncomponent.Method_Public_Void_0();
                inputcomponent.Method_Public_Void_0();
                GameObject playerCamera = Wrappers.GetPlayerCamera();
                float num = Input.GetKey(KeyCode.LeftShift) ? FlightSpeed * 2 : FlightSpeed;
                VRCPlayer currentPlayer = PlayerWrappers.GetCurrentPlayer();
                if (Input.GetKey(KeyCode.W))
                {
                    currentPlayer.transform.position += playerCamera.transform.forward * num * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    currentPlayer.transform.position += playerCamera.transform.right * -1f * num * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    currentPlayer.transform.position += playerCamera.transform.forward * -1f * num * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    currentPlayer.transform.position += playerCamera.transform.right * num * Time.deltaTime;
                }
                if (Input.GetKey(KeyCode.Space))
                {
                    currentPlayer.transform.position += currentPlayer.transform.up * num * Time.deltaTime * 2f;
                }
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    currentPlayer.transform.position += currentPlayer.transform.up * -1f * num * Time.deltaTime * 2f;
                }
                if (System.Math.Abs(Input.GetAxis("Vertical")) > 0f)
                {
                    currentPlayer.transform.position += playerCamera.transform.forward * num * Time.deltaTime * Input.GetAxis("Vertical") * 2f;
                }
                if (System.Math.Abs(Input.GetAxis("Horizontal")) > 0f)
                {
                    currentPlayer.transform.position += playerCamera.transform.right * num * Time.deltaTime * Input.GetAxis("Horizontal") * 2f;
                }
                if (System.Math.Abs(Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical")) > 0f)
                {
                    currentPlayer.transform.position += currentPlayer.transform.up * num * Time.deltaTime * Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical") * 2f;
                }
                if (NoClip == true)
                {
                    Vector3 vector = currentPlayer.gameObject.transform.position - VRCTrackingManager.Method_Public_Static_Vector3_0();
                    Quaternion quaternion = currentPlayer.gameObject.transform.rotation * Quaternion.Inverse(VRCTrackingManager.Method_Public_Static_Quaternion_1());
                    VRCTrackingManager.Method_Public_Static_Void_Vector3_Quaternion_1(vector, quaternion);
                }
            }
            bool ToggleHeadFlipper = Input.GetKeyDown(KeyCode.Tab);
            if (ToggleHeadFlipper)
            {
                GetInstance().GetComponentInChildren<NeckMouseRotator>().field_Public_NeckRange_0 = new VRC.DataModel.NeckRange(float.MinValue, float.MaxValue, 0f);
                /*HeadFlipper = !HeadFlipper;
                if (HeadFlipper && UIManagerInit == true)
                {
                    GetInstance().GetComponentInChildren<MonoBehaviourPublicObSiBoSiVeBoQuVeBoSiUnique>().field_Public_NeckRange_0 = new VRC.DataModel.NeckRange(float.MinValue, float.MaxValue, 0f);
                }
                else if (!HeadFlipper && UIManagerInit == true)
                {
                    GetInstance().GetComponentInChildren<MonoBehaviourPublicObSiBoSiVeBoQuVeBoSiUnique>().field_Public_NeckRange_0 = new VRC.DataModel.NeckRange(0f, 0f, 0f);
                    //GetInstance().GetComponentInChildren<NeckMouseRotator>().field_Public_Vector3_0 = new Vector3(0f, 0f, 0f);
                }*/
            }

            if (PortalLoopTimer >= 5f && PortalDropLoop == true)
            {
                PortalDrops.DropPortalOnUserID(StoredUserID);
                PortalLoopTimer = 0f;
            }
            if (false)//(Input.GetKeyDown(KeyCode.K))
            {
                text_popup("Enter AvatarID.", "Change Pedestals", new System.Action<string>((a) =>
                {
                    ChangeAvatarPedestals(a);
                }));
            }

            if (UIManagerInit == true)
            {
                /*if (Wrappers.GetQuickMenu().prop_Boolean_0 == true && IsLoading == false)
                {
                    var PlayerCount = GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar/PlayerCountText");
                    var GetAllPlayers = PlayerWrappers.GetAllPlayers()._size;
                    PlayerCount.GetComponent<Text>().text = "Players: " + GetAllPlayers;
                    var QuestCount = GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar/QuestCountText");
                    var GetAllQuestUsers = PlayerWrappers.GetAllQuestUsers()._size;
                    QuestCount.GetComponent<Text>().text = "Quest Users: " + GetAllQuestUsers;
                }*/
            }
            bool keyShift = Input.GetKey(KeyCode.LeftShift);
            if (keyShift)
            {
                bool keyDownG = Input.GetKeyDown(KeyCode.G);
                if (keyDownG)
                {
                    bool gainvol = this.Gainvol;
                    VRCPlayer currentPlayer = PlayerWrappers.GetCurrentPlayer();
                    if (gainvol)
                    {
                        PendulumLogger.Log("Mic Gain Off");
                        this.Gainvol = false;
                        //USpeaker.field_Internal_Static_Single_0 = 1f;
                        //USpeaker.field_Internal_Static_Single_1 = 1f;
                        //USpeaker.field_Internal_Static_Single_2 = 1f;
                        USpeaker.field_Internal_Static_Single_3 = 1f;
                        //currentPlayer.field_Private_USpeaker_0.CurrentBitrate = EnumPublicSealedvaBi15BiBiBiBiBiBiBiUnique.BitRate_48K;
                        AlertPopup.SendAlertPopup("Mic Gain Off");
                    }
                    else
                    {
                        PendulumLogger.Log("Mic Gain On");
                        this.Gainvol = true;
                        //USpeaker.field_Internal_Static_Single_0 = float.MaxValue;
                        //USpeaker.field_Internal_Static_Single_1 = float.MaxValue;
                        //USpeaker.field_Internal_Static_Single_2 = float.MaxValue;
                        USpeaker.field_Internal_Static_Single_3 = float.MaxValue;
                        //currentPlayer.field_Private_USpeaker_0.CurrentBitrate = EnumPublicSealedvaBi15BiBiBiBiBiBiBiUnique.BitRate_8K;
                        AlertPopup.SendAlertPopup("Mic Gain On");
                    }
                }
            }
        }

        public void SetupLogoButton()
        {
            if (FirstTimeinit == true)
            {
                //PendulumLogger.Log("logo.assetbundle Downloaded!");
            }
            Transform transform3 = Wrappers.GetQuickMenu().transform.Find("ShortcutMenu");
            GameObject gameObject3 = ButtonAPI.CreateButton(ButtonType.Default, "Pendulum Client\n" + PendulumClientBranchVersion + " v" + PendulumClientBuildVersion, "Pendulum Client, By Kyran And Corbinss.", UnityEngine.Color.white, UnityEngine.Color.white, -4f, 4f, transform3, delegate
            {
                Process.Start("https://discord.gg/kRgtZRJ");
            });
            var GoHomeButton = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/GoHomeButton");
            GameObject.Destroy(GoHomeButton);
            GameObject NewHomeButton = ButtonAPI.CreateButton(ButtonType.Default, "Go Home", "Go to Your Home World", UnityEngine.Color.white, UnityEngine.Color.white, -3f, 2f, transform3, delegate
            {
                if (!string.IsNullOrEmpty(APIUser.CurrentUser.homeLocation))
                {
                    var HomeInstance = APIUser.CurrentUser.homeLocation + ":" + new System.Random().Next(1, 99999) + "~private(" + APIUser.CurrentUser.id + ")~nonce(" + ComputeSha256Hash(RandomString(420)).ToUpper() + ")";
                    Networking.GoToRoom(HomeInstance);
                    IsLoading = true;
                }
                else
                {
                    var HomeInstance = "wrld_4432ea9b-729c-46e3-8eaf-846aa0a37fdd:" + new System.Random().Next(1, 99999) + "~private(" + APIUser.CurrentUser.id + ")~nonce(" + ComputeSha256Hash(RandomString(420)).ToUpper() + ")";
                    Networking.GoToRoom(HomeInstance);
                    PendulumLogger.Log("Going home.");
                    IsLoading = true;
                }
            });
            //AssetBundleRequest PendClientLogo = myAssetBundle.LoadAssetAsync("pendclientsprite.png", Il2CppType.Of<Sprite>());
            AssetBundleRequest PendClientLogo = myAssetBundle.LoadAssetAsync("PW3Row.png", Il2CppType.Of<Sprite>());
            AssetBundleRequest SpriteShader = myAssetBundle.LoadAssetAsync("bruh.shader", Il2CppType.Of<Shader>());
            Sprite PendClientSprite = PendClientLogo.asset.Cast<Sprite>();
            Shader LogoSpriteShader = SpriteShader.asset.Cast<Shader>();
            if (PendClientSprite != null)
            {
                Material logoMaterial = new Material(gameObject3.gameObject.GetComponent<UnityEngine.UI.Image>().material);
                logoMaterial.shader = LogoSpriteShader;
                logoMaterial.SetTexture("_MainTex", PendClientSprite.texture);
                logoMaterial.SetInt("_ColumnsX", 11);
                logoMaterial.SetInt("_RowsY", 3);
                logoMaterial.SetFloat("_AnimationSpeed", 25f);
                gameObject3.gameObject.GetComponent<UnityEngine.UI.Image>().material = logoMaterial;
                gameObject3.gameObject.GetComponent<UnityEngine.UI.Image>().sprite = new Sprite();
            }
            else
            {
                PendulumLogger.Log("MenuSprite Failed to Initalize.");
            }
            //var Vec2 = new Vector2(0.1f, 0.1f);
            gameObject3.GetComponent<UnityEngine.UI.Image>().rectTransform.sizeDelta -= new Vector2(0.1f, 0.1f);
            var Orgtext = gameObject3.GetComponentInChildren<Text>().gameObject;
            var NewText1 = GameObject.Instantiate(Orgtext, gameObject3.transform);
            NewText1.name = "BlackText1";
            var NewText2 = GameObject.Instantiate(Orgtext, gameObject3.transform);
            NewText2.name = "BlackText2";
            var NewText3 = GameObject.Instantiate(Orgtext, gameObject3.transform);
            NewText3.name = "BlackText3";
            var NewText4 = GameObject.Instantiate(Orgtext, gameObject3.transform);
            NewText4.name = "BlackText4";
            var NewText = GameObject.Instantiate(Orgtext, gameObject3.transform);
            NewText.name = "WhiteText";
            Orgtext.GetComponent<Text>().color = UnityEngine.Color.black;
            //NewText1.GetComponent<Text>().color = UnityEngine.Color.black;
            //Orgtext.GetComponent<Text>().transform.localScale += new Vector3(0f, 0.02f);
            NewText1.GetComponent<Text>().color = UnityEngine.Color.black;
            NewText1.GetComponent<Text>().transform.position += new Vector3(0.0015f, 0f);
            NewText2.GetComponent<Text>().color = UnityEngine.Color.black;
            NewText2.GetComponent<Text>().transform.position += new Vector3(-0.0015f, 0f);
            NewText3.GetComponent<Text>().color = UnityEngine.Color.black;
            NewText3.GetComponent<Text>().transform.position += new Vector3(0f, 0.0015f);
            NewText4.GetComponent<Text>().color = UnityEngine.Color.black;
            NewText4.GetComponent<Text>().transform.position += new Vector3(0f, -0.0015f);
            LogoButtonSetup = true;
        }

        public static bool CheckValidCollider(Collider collider, Component org)
        {
            Il2CppArrayBase<Collider> colliderlist = null;
            try
            {
                colliderlist = GameObject.Find("UserInterface").GetComponentsInChildren<Collider>();
            }
            catch (Exception) { }
            if (colliderlist != null)
            {
                foreach (Collider qmcollider in colliderlist)
                {
                    try
                    {
                        if (qmcollider != null && qmcollider != new Collider())
                        {
                            if (qmcollider == collider)
                                return false;
                        }
                    }
                    catch (Exception) { return false; }
                }
            }
            try
            {
                return collider.GetComponent<PlayerSelector>() == null && collider.GetComponent<VRC_Trigger>() == null && collider.GetComponent<VRCSDK2.VRC_UiShape>() == null && collider.GetComponent<VRC.SDKBase.VRC_Pickup>() == null && collider.GetComponent<QuickMenu>() == null && collider.GetComponent<VRC.UI.Elements.QuickMenu>() == null && collider.GetComponent<VRC_StationInternal>() == null && collider.GetComponent<VRC.SDKBase.VRC_AvatarPedestal>() == null && collider != org;
            }
            catch (Exception) { return false; }
        }
        public static void ToggleColliders(bool toggle)
        {
            Collider[] array = UnityEngine.Object.FindObjectsOfType<Collider>();
            Component component = PlayerWrappers.GetCurrentPlayer().GetComponents(Il2CppType.Of<Collider>()).FirstOrDefault<Component>();
            foreach (Collider collider in array)
            {
                if (CheckValidCollider(collider, component))
                {
                    try
                    {
                        collider.enabled = toggle;
                    }
                    catch (Exception) { }
                }
            }
        }

        public static Vector3 Grav = Physics.gravity;

        
        public static void SpawnDynamicPrefab(Player player, string prefab)
        {
            GameObject Prefab = Networking.Instantiate(VRC_EventHandler.VrcBroadcastType.Always, prefab, player.transform.position, player.transform.rotation);
            Prefab.name = "InstantiatedPrefab";
        }

        public static void delete_dynamic_prefabs()
        {
            (
             from pobject in Resources.FindObjectsOfTypeAll<VRC_Pickup>()
             where pobject.gameObject.activeInHierarchy && pobject.name == "InstantiatedPrefab"
             select pobject).ToList().ForEach(p =>
             {
                 UnityEngine.Object.Destroy(p.transform.root.gameObject);
             });
        }
        public override void OnFixedUpdate() // Can run multiple times per frame. Mostly used for Physics.
        {
            //PendulumLogger.Log("OnFixedUpdate");
            if (RPC_Crash)
            {
                EmojiRPC(int.MaxValue);
                //EmoteRPC(int.MinValue);
            }
        }

        public override void OnLateUpdate() // Runs once per frame after OnUpdate and OnFixedUpdate have finished.
        {
            //PendulumLogger.Log("OnLateUpdate");
            if (RPC_Crash)
            {
                EmojiRPC(int.MaxValue);
                //EmoteRPC(int.MinValue);
            }
        }

        public override void OnGUI() // Can run multiple times per frame. Mostly used for Unity's IMGUI.
        {
            //PendulumLogger.Log("OnGUI");
            if (RPC_Crash)
            {
                EmojiRPC(int.MaxValue);
                //EmoteRPC(int.MinValue);
            }
        }

        public override void OnApplicationQuit() // Runs when the Game is told to Close.
        {
            //PendulumLogger.Log("OnApplicationQuit");
        }

        public override void OnModSettingsApplied() // Runs when Mod Preferences get saved to UserData/modprefs.ini.
        {
            //PendulumLogger.Log("OnModSettingsApplied");
            PendulumClientMain.Modules.ForEach(delegate (VRCMod y)
            {
                y.OnModSettingsApplied();
            });
        }

        public static void ConvertAvtrID()
        {
            var AvatarID = "avtr_2cbf0316-3f99-4177-a7d3-f03fca8cbc9a";
        }
        public void Early_UiManagerInit()
        {
            UnityEngine.Application.targetFrameRate = ModPrefs.GetInt("PendulumClient", "FPSCap");
            var MenuStyle = ModPrefs.GetInt("PendulumClient", "MenuStyle");
            bool AnitPortalDefaultEnabled = ModPrefs.GetBool("PendulumClient", "AntiPortal");
            ForceRestartButton();
            //ColorModuleV2.CMV2_ColorModule.ExtraStuff();

            UiManagerInitEarly = true;
        }
        public void VRChat_OnUiManagerInit()
        {
            UIManagerInit = true;

            //SetupPlayerCounter();
            //TrustRankButtonPositioning();

            /*
            Transform transform3 = Wrappers.GetQuickMenu().transform.Find("ShortcutMenu");
            bool flag8 = transform3 != null;
            if (flag8)
            {
                GameObject gameObject1 = ButtonAPI.CreateButton(ButtonType.Toggle, "Flight", "Enable/Disable Flight", UnityEngine.Color.white, UnityEngine.Color.white, -4f, 1f, transform3, delegate
                {
                    FlightEnabled = true;
                    Physics.gravity = Vector3.zero;
                    ToggleColliders(false);
                    PendulumLogger.Log("Flight has been " + (FlightEnabled ? "Enabled" : "Disabled") + ".");
                }, delegate
                {
                    FlightEnabled = false;
                    Physics.gravity = Grav;
                    ToggleColliders(true);
                    PendulumLogger.Log("Flight has been " + (FlightEnabled ? "Enabled" : "Disabled") + ".");
                });

                if (AnitPortalDefaultEnabled == true)
                {
                    GameObject gameObjectantiportal = ButtonAPI.CreateButton(ButtonType.EnabledToggle, "AntiPortal", "Toggle if you can go in portals or not", UnityEngine.Color.white, UnityEngine.Color.white, -4f, 0f, transform3, delegate
                    {
                        if (FirstAntiPortalToggle == false)
                        {
                            AntiPortalToggle = true;
                            FirstAntiPortalToggle = true;
                        }
                        else
                        {
                            AntiPortalToggle = true;
                            PendulumLogger.Log("Anti Portal Enabled! Saving Config...");
                            AlertPopup.SendAlertPopup("Anti Portal Enabled!");
                            ModPrefs.SetBool("PendulumClient", "AntiPortal", true);
                            ModPrefs.SaveConfig();
                        }
                    }, delegate
                    {
                        AntiPortalToggle = false;
                        PendulumLogger.Log("Anti Portal Disabled! Saving Config...");
                        AlertPopup.SendAlertPopup("Anti Portal Disabled!");
                        ModPrefs.SetBool("PendulumClient", "AntiPortal", false);
                        ModPrefs.SaveConfig();
                    });
                }
                else
                {
                    GameObject gameObjectantiportal = ButtonAPI.CreateButton(ButtonType.Toggle, "AntiPortal", "Toggle if you can go in portals or not", UnityEngine.Color.white, UnityEngine.Color.white, -4f, 0f, transform3, delegate
                    {
                        AntiPortalToggle = true;
                        PendulumLogger.Log("Anti Portal Enabled! Saving Config...");
                        AlertPopup.SendAlertPopup("Anti Portal Enabled!");
                        ModPrefs.SetBool("PendulumClient", "AntiPortal", true);
                        ModPrefs.SaveConfig();
                    }, delegate
                    {
                        AntiPortalToggle = false;
                        PendulumLogger.Log("Anti Portal Disabled! Saving Config...");
                        AlertPopup.SendAlertPopup("Anti Portal Disabled!");
                        ModPrefs.SetBool("PendulumClient", "AntiPortal", false);
                        ModPrefs.SaveConfig();
                    });
                }
                GameObject SelectSelf = ButtonAPI.CreateButton(ButtonType.Default, "", "Select Yourself.", UnityEngine.Color.white, UnityEngine.Color.white, -3f, 4f, transform3, delegate
                {
                    Wrappers.GetQuickMenu().Method_Public_Void_Player_0(VRCPlayer.field_Internal_Static_VRCPlayer_0._player);
                });
                SelectSelf.GetComponent<Image>().enabled = false;
                //SelectSelf.GetComponentInChildren<Text>().enabled = false;
            }

            var userinteractmenu = Wrappers.GetQuickMenu().transform.Find("UserInteractMenu");
            if (userinteractmenu != null && setupusermenu == false)
            {
                user_menu = blankmenupage("selection_menu");

                var backbutton = ButtonAPI.CreateButton(ButtonType.Default, "Back", "Go Back.", UnityEngine.Color.yellow, UnityEngine.Color.white, 1f, 1f, user_menu.transform,
                new Action(() =>
                {
                    user_menu.SetActive(false);
                    userinteractmenu.gameObject.SetActive(true);

                }),
                new Action(() =>
                {

                }));

                if (MenuStyle <= 0)
                {
                    var menubutton = ButtonAPI.CreateButton(ButtonType.Default, "Options", "Selected User Options.", UnityEngine.Color.white, UnityEngine.Color.white, 0f, 0f, userinteractmenu,
                    new Action(() =>
                    {
                        user_menu.SetActive(true);
                        userinteractmenu.gameObject.SetActive(false);
                        user_menu_open = true;
                    }),
                    new Action(() =>
                    {

                    }));
                }
                if (MenuStyle == 1)
                {
                    var menubutton = ButtonAPI.CreateButton(ButtonType.Default, "Options", "Selected User Options.", UnityEngine.Color.white, UnityEngine.Color.white, 0f, 1f, userinteractmenu,
                    new Action(() =>
                    {
                        user_menu.SetActive(true);
                        userinteractmenu.gameObject.SetActive(false);
                        user_menu_open = true;
                    }),
                    new Action(() =>
                    {

                    }));
                    menubutton.transform.localScale = new Vector2(1f, 0.45f);
                    menubutton.transform.localPosition += new Vector3(0f, 95f, 0f);
                    menubutton.transform.Find("Text").gameObject.transform.localScale = new Vector2(1f, 2.1f);

                    var dropportalbutton = ButtonAPI.CreateButton(ButtonType.Default, "Drop Portal", "Drop Portal on Selected Player.", UnityEngine.Color.white, UnityEngine.Color.white, 0f, 1f, userinteractmenu,
                    new Action(() =>
                    {
                        var selectedplayer = Wrappers.GetQuickMenu().GetSelectedPlayer();
                        PortalDrops.DropPortalOnPlayer(selectedplayer);
                        AlertPopup.SendAlertPopup("Dropped Portal on " + selectedplayer.prop_APIUser_0.displayName);
                    }),
                    new Action(() =>
                    {

                    }));
                    dropportalbutton.transform.localScale = new Vector2(1f, 0.45f);
                    dropportalbutton.transform.localPosition += new Vector3(0f, -95f, 0f);
                    dropportalbutton.transform.Find("Text").gameObject.transform.localScale = new Vector2(1f, 2.1f);
                }
                if (MenuStyle == 2)
                {
                    var menubutton = ButtonAPI.CreateButton(ButtonType.Default, "Options", "Selected User Options.", UnityEngine.Color.white, UnityEngine.Color.white, 0f, 1f, userinteractmenu,
                    new Action(() =>
                    {
                        user_menu.SetActive(true);
                        userinteractmenu.gameObject.SetActive(false);
                        user_menu_open = true;
                    }),
                    new Action(() =>
                    {

                    }));
                    menubutton.transform.localScale = new Vector2(1f, 0.45f);
                    menubutton.transform.localPosition += new Vector3(0f, 95f, 0f);
                    menubutton.transform.Find("Text").gameObject.transform.localScale = new Vector2(1f, 2.1f);

                    var dropportalbutton = ButtonAPI.CreateButton(ButtonType.Default, "Teleport", "Teleport to Selected Player.", UnityEngine.Color.white, UnityEngine.Color.white, 0f, 1f, userinteractmenu,
                    new Action(() =>
                    {
                        var selectedplayer = Wrappers.GetQuickMenu().GetSelectedPlayer();
                        var currentplayer = PlayerWrappers.GetCurrentPlayer();
                        currentplayer.transform.position = selectedplayer.transform.position;
                        AlertPopup.SendAlertPopup("Teleported to " + selectedplayer.prop_APIUser_0.displayName);
                        CloseMenu();
                    }),
                    new Action(() =>
                    {

                    }));
                    dropportalbutton.transform.localScale = new Vector2(1f, 0.45f);
                    dropportalbutton.transform.localPosition += new Vector3(0f, -95f, 0f);
                    dropportalbutton.transform.Find("Text").gameObject.transform.localScale = new Vector2(1f, 2.1f);
                }
                if (MenuStyle >= 3)
                {
                    var menubutton = ButtonAPI.CreateButton(ButtonType.Default, "Options", "Selected User Options.", UnityEngine.Color.white, UnityEngine.Color.white, 0f, 1f, userinteractmenu,
                    new Action(() =>
                    {
                        user_menu.SetActive(true);
                        userinteractmenu.gameObject.SetActive(false);
                        user_menu_open = true;
                    }),
                    new Action(() =>
                    {

                    }));
                    menubutton.transform.localScale = new Vector2(1f, 0.45f);
                    menubutton.transform.localPosition += new Vector3(0f, 95f, 0f);
                    menubutton.transform.Find("Text").gameObject.transform.localScale = new Vector2(1f, 2.1f);
                    var dropportalbutton = ButtonAPI.CreateButton(ButtonType.Default, "Clone Avatar", "Force Clone the Selected Players Avatar (If Public).", UnityEngine.Color.white, UnityEngine.Color.white, 0f, 1f, userinteractmenu,
                    new Action(() =>
                    {
                        var selectedplayer = Wrappers.GetQuickMenu().GetSelectedPlayer();
                        APIForceClone();
                    }),
                    new Action(() =>
                    {

                    }));
                    dropportalbutton.transform.localScale = new Vector2(1f, 0.45f);
                    dropportalbutton.transform.localPosition += new Vector3(0f, -95f, 0f);
                    dropportalbutton.transform.Find("Text").gameObject.transform.localScale = new Vector2(1f, 2.1f);
                }
                DevToolsMenu.SetupOptionsMenu();
                DevToolsMenu.SetupDevToolsMenu();
                //SetupDeclineButton();
            }
            bool flag9 = SocialMenuSetup;
            if (!flag9)
            {
                var screensmenu = GameObject.Find("Screens").transform.Find("UserInfo");
                if (!SocialMenuSetup && screensmenu != null && Wrappers.GetQuickMenu().transform.Find("QuickMenu_NewElements/_InfoBar/EarlyAccessText") != null)
                {
                    social_btn(1f, 0f, "Force Request", new Action(() =>
                    {
                        SendInviteReqSocial();
                    }));
                    social_btn(0f, -1f, "Teleport", new Action(() =>
                    {
                        TPToPlayerSocial();
                    }));

                    social_btn(1f, -1f, "Clone Avatar", new Action(() =>
                    {
                        SocialForceClone();
                    }));

                    /*social_btn(0f, 0f, "Force VTK", new Action(() =>
                    {
                        SendVoteKickSocial();
                    }));


                    social_btn(4.66f, 8.34f, "Message", new Action(() =>
                    {
                        SocialMSG();
                    }));

                    social_btn(9.89f, -1.25f, "Force Invite", new Action(() =>
                    {
                        SocialInvite();
                    }));
                    social_btn(2f, 0f, "FriendReq", new Action(() =>
                    {
                        SendFriendReqSocial();
                    }));
                    social_btn(2f, -1f, "Object Crash", new Action(() =>
                    {
                        var menu = GameObject.Find("Screens").transform.Find("UserInfo");
                        var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
                        var player = PlayerWrappers.GetPlayer(userInfo.field_Public_APIUser_0.id);
                        if (player != null)
                        {
                            CloseMenu();
                            ParentObjects(player);
                        }
                        else
                        {
                            Errormsg("Player not Found!", "The selected player is not in the instance");
                        }
                    }));
                }
                var SocialMenu = GameObject.Find("UserInterface/MenuContent/Screens/Social");
                if (SocialMenu != null)
                {
                    social_screen_button(-1f, 1f, "Rejoin World", new Action(() =>
                    {
                        RejoinWorld();
                    }));
                    social_screen_button(-1f, 0f, "Copy InstanceID", new Action(() =>
                    {
                        CopyInstanceID();
                    }));
                    social_screen_button(-1f, -1f, "Go to InstanceID", new Action(() =>
                    {
                        text_popup("Enter InstanceID", "Go", new System.Action<string>((a) =>
                        {
                            if (a.Contains(":") && a.Contains("wrld_"))
                            {
                                GotoInstance(a);
                            }
                            else
                            {
                                Errormsg("Not a valid Instance.", "InstanceID is invalid or doesn't exist");
                            }
                        }));
                    }));
                    social_screen_button(-1f, -2f, "Change Pedestals", new Action(() =>
                    {
                        text_popup("Enter AvatarID", "Change Pedestals", new System.Action<string>((a) =>
                        {
                            if (a.Contains("avtr_"))
                            {
                                ChangeAvatarPedestals(a);
                            }
                            else
                            {
                                Errormsg("Not a valid Avatar.", "AvatarID is invalid or doesn't exist");
                            }
                        }));
                    }));
                    social_screen_button(-1f, -3f, "Change Avatar By ID", new Action(() =>
                    {
                        text_popup("Enter AvatarID", "Change Avatar", new System.Action<string>((a) =>
                        {
                            if (a.Contains("avtr_"))
                            {
                                ChangeAvatar(a);
                            }
                            else
                            {
                                Errormsg("Not a valid Avatar.", "AvatarID is invalid or doesn't exist");
                            }
                        }));
                    }));
                    social_screen_button(-1f, -4f, "[DISABLED]", new Action(() =>
                    {
                        //SendFriendReqSocial();
                    }));
                    social_screen_button(-1f, -5f, "Friend All", new Action(() =>
                    {
                        StartFreindReqAll();
                    }));
                    social_screen_button(-1f, -6f, "Invite All", new Action(() =>
                    {
                        InviteAll();
                    }));
                    social_screen_button(-1f, -7f, "Request All", new Action(() =>
                    {
                        InviteRequestAll();
                    }));
                }
            }*/
        }

        public static void CopyInstanceID()
        {
            /*var World = RoomManager.field_Internal_Static_ApiWorld_0;
            var WorldID = World.id;
            var InstanceID = World.currentInstanceIdWithTags;
            Clipboard.SetText(WorldID + ":" + InstanceID);
            Errormsg("Text Copied!", "InstanceID Copied to Clipboard!");*/
        }

        public static void GotoInstance(string instance)
        {
            Networking.GoToRoom(instance);
        }

        public static void GoHome()
        {
            if (!string.IsNullOrEmpty(APIUser.CurrentUser.homeLocation))
            {
                var HomeInstance = APIUser.CurrentUser.homeLocation + ":" + new System.Random().Next(1, 99999) + "~private(" + APIUser.CurrentUser.id + ")~nonce(" + PendulumClientMain.RandomInstance8Char() + PendulumClientMain.RandomInstance4Char() + PendulumClientMain.RandomInstance4Char() + PendulumClientMain.RandomInstance4Char() + PendulumClientMain.RandomInstance12Char() + ")";
                Networking.GoToRoom(HomeInstance);
                //PendulumLogger.Log(HomeInstance);
                PendulumClientMain.IsLoading = true;
            }
            else
            {
                var HomeInstance = "wrld_4432ea9b-729c-46e3-8eaf-846aa0a37fdd:" + new System.Random().Next(1, 99999) + "~private(" + APIUser.CurrentUser.id + ")~nonce(" + PendulumClientMain.RandomInstance8Char() + PendulumClientMain.RandomInstance4Char() + PendulumClientMain.RandomInstance4Char() + PendulumClientMain.RandomInstance4Char() + PendulumClientMain.RandomInstance12Char() + ")";
                Networking.GoToRoom(HomeInstance);
                PendulumClientMain.IsLoading = true;
            }
        }
        public static void RejoinWorld()
        {
            var instance = RoomManager.field_Internal_Static_ApiWorld_0;
            var localplr = APIUser.CurrentUser.instanceId;
            Networking.GoToRoom($"{instance.id}:{localplr}");
            //Networking.GoToRoom(RoomManager.field_Internal_Static_ApiWorld_0.id + ":" + RoomManager.field_Internal_Static_ApiWorldInstance_0.idWithTags);
            /*bool fail = false;
            string name = "";
            System.Action<string> NameSuccess = (string c) =>
            {
                name = c;
                PendulumLogger.Log("Name: " + c);
            };
            System.Action<string> NameFail = (string c) =>
            {
                fail = true;
                PendulumLogger.Log("NameFail: " + c);
            };
            RoomManager.field_Internal_Static_ApiWorldInstance_0.GetShortName(NameSuccess, NameFail);
            if (fail == false || name == "")
            {
                System.Action<string> IDSuccess = (string c) =>
                {
                    PendulumLogger.Log("ID: " + c);
                };
                System.Action<string> IDFail = (string c) =>
                {
                    PendulumLogger.Log("IDFail: " + c);
                };
                ApiWorldInstance.GetInstanceIdFromShortName(name, IDSuccess, IDFail);
            }*/
        }

        public static void ChangeAvatar(string avatarid)
        {
            ChangeToAvatar(avatarid);
        }


        public static void FriendUser(string userid)
        {
            VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Notification.Create(userid, Notification.NotificationType.FRIEND_REQUEST, "why cant we be friends", null));
        }

        public static void InviteAll()
        {
            if (PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0 != null)
            {
                foreach (Player player in PlayerWrappers.GetAllPlayers())
                {
                    //VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Invite.Create(player.prop_APIUser_0.id, "", new Location(RoomManager.field_Internal_Static_ApiWorld_0.id, new Transmtn.DTO.Instance(RoomManager.field_Internal_Static_ApiWorld_0.currentInstanceIdWithTags, "", "", "", "", false)), RoomManager.field_Internal_Static_ApiWorld_0.name));
                }
                Errormsg("Invites Sent!", "Invites Sent!");
            }
        }

        public static void InviteRequestAll()
        {
            if (PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0 != null)
            {
                foreach (Player player in PlayerWrappers.GetAllPlayers())
                {
                    VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Notification.Create(player.prop_APIUser_0.id, Notification.NotificationType.REQUEST_INVITE, "lemme in bitch", null));
                }
                Errormsg("Requests Sent!", "Invite Requests Sent!");
            }
        }

        public static void ChangePortals()
        {
            foreach (PortalInternal p in Resources.FindObjectsOfTypeAll<PortalInternal>())
            {
                Networking.RPC(RPC.Destination.AllBufferOne, p.gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
                    {
                        (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                        (Il2CppSystem.String)"YoinkedPortal\0",
                        new Il2CppSystem.Int32
                        {
                            m_value = -420
                        }.BoxIl2CppObject()
                    });
                PendulumLogger.Log("Portal Changed! " + p.gameObject.name);
            }
        }

        public static GameObject UserUiPrefab = null;
        public static IEnumerator DeleteUser(float waittime)
        {
            yield return new WaitForSeconds(waittime);
            if (UserUiPrefab != null)
            {
                UserUiPrefab.SetActive(false);
                //PendulumLogger.Log("Deleted!");
            }
            else
            {
                //PendulumLogger.Log("that shit null lol");
            }
        }

        public static Color GetTrustColor(APIUser apiUser)
        {
            switch (PlayerWrappers.GetUserTrustRank(apiUser))
            {
                case PlayerWrappers.TrustRank.Admin:
                    return new Color(0.5f, 0f, 0f);

                case PlayerWrappers.TrustRank.Legendary:
                    return new Color(0f, 0.3f, 0.5f);

                case PlayerWrappers.TrustRank.Veteran:
                    return new Color(0.7f, 1f, 0f);

                case PlayerWrappers.TrustRank.Trusted:
                    return new Color(0.5f, 0f, 1f);

                case PlayerWrappers.TrustRank.Known:
                    return new Color(0.894f, 0.43f, 0.263f);

                case PlayerWrappers.TrustRank.User:
                    return new Color(0.1647f, 0.8117f, 0.3568f);

                case PlayerWrappers.TrustRank.New:
                    return new Color(0.0863f, 0.470f, 1f);

                case PlayerWrappers.TrustRank.Visitor:
                    return new Color(0.8f, 0.8f, 0.8f);
            }
            return new Color(0.4139273f, 0.8854161f, 0.9705882f);
        }

        public static string GetTrustRankString(APIUser apiUser)
        {
            switch (PlayerWrappers.GetUserTrustRank(apiUser))
            {
                case PlayerWrappers.TrustRank.Admin:
                    return "Admin";

                case PlayerWrappers.TrustRank.Legendary:
                    return "Legendary User";

                case PlayerWrappers.TrustRank.Veteran:
                    return "Veteran User";

                case PlayerWrappers.TrustRank.Trusted:
                    return "Trusted User";

                case PlayerWrappers.TrustRank.Known:
                    return "Known User";

                case PlayerWrappers.TrustRank.User:
                    return "User";

                case PlayerWrappers.TrustRank.New:
                    return "New User";

                case PlayerWrappers.TrustRank.Visitor:
                    return "Visitor";
            }
            return "Visitor";
        }
        public static void PatchHarmony(HarmonyLib.Harmony instance)
        {
            var AntiPortalMethod1 = "Method_Private_Void_1";
            var AntiPortalMethod2 = "Method_Private_Void_0";
            var EventMethod1 = "Method_Public_Void_Player_VrcEvent_VrcBroadcastType_Int32_Single_";
            var RoomMethod1 = "Method_Public_Static_Boolean_ApiWorld_ApiWorldInstance_String_Int32_0";
            var WorldTriggerMethod1 = "InternalTriggerEvent";
            var GoToRoomMethod = "GoToRoom";
            var NotiMethod = "Send";
            var PingMethod = "Method_Public_Static_Int32_1";
            var AvatarChangeMethod = "Method_Internal_Void_ApiAvatar_0";

            var TestMethod = "_EnterNewWorldInstanceWithTags_b__0";
            var TestMethod2 = "Method_Protected_Void_String_0";

            var ModMethod1 = "_IsKickedFromWorld_b__0";
            var ModMethod2 = "_GetPlayerModerationsOfType_b__0";
            var ModMethod3 = "_GetModerationsOfType_b__0";
            var ModMethod4 = "_GetModerationOfType_b__0";
            var ModMethod5 = "_get_IsBannedFromPublicOnly_b__78_0";
            var ModMethod6 = "_get_IsBanned_b__76_0";
            var ModMethod7 = "_ReadReplyPlayerMods_b__37_0";
            var ModMethod8 = "_ReceiveModeration_b__0";
            var ModMethod9 = "_ReceiveModeration_b__1";
            var ModMethod10 = "_FetchModerations_b__9";
            var ModMethod11 = "_FetchModerations_b__6";
            var ModMethod12 = "_FetchModerations_b__8";
            var ModMethod13 = "_FetchModerations_b__5";
            var ModMethod14 = "_FetchModerations_b__2";
            var ModMethod15 = "_FetchModerations_b__4";
            var ModMethod16 = "_FetchModerations_b__0";
            var ModMethod17 = "_FetchModerations_b__1";
            var ModMethod18 = "_FetchModerations_b__3";
            var ModMethod19 = "_FetchModerations_b__7";

            //var AntiPortalPatch = "falsePatch";
            var EventPatch = "patch__3";
            var RoomPatch = "patch__1";
            var TriggerPatch = "patch__4";
            var GoToRoomPatch = "patch__5";
            var AntiKickPatch = "patch__6";
            var NotiPatch = "patch__10";
            var PingPatch = "patch__ping";
            //var AvatarChangePatch = "patch__avatar__logging";

            var ApiModPatch = "patch__7";
            var ApiPlrModPatch = "patch__8";
            var ApiModContPatch = "patch__9";
            var ApiModEnumPatch = "patch__enum__1";
            var ApiPlrModEnumPatch = "patch__enum__2";
            var ResultFalsePatch = "result__false";

            var UiListPatch = "UIList__Hook";
            var AvatarChangePatch = "AvatarChange__Hook";
            var AvatarLoadPatch = "AvatarLoad__Hook";
            var UserInfoPatch = "UserInfo__Hook";
            var HWIDMethod = "GetDeviceUniqueIdentifier";
            var HWIDPatch = "HWID__Hook";


            var original2 = typeof(PortalInternal).GetMethod(AntiPortalMethod1); //DestroyPortal
            var original3 = typeof(PortalInternal).GetMethod(AntiPortalMethod2); //DestroyPortalSpawn
            var original28 = typeof(VRC_EventDispatcherRFC).GetMethod("Method_Public_Void_Player_VrcEvent_VrcBroadcastType_Int32_Single_0");//GetMethods().Where(m => m.Name.Contains(EventMethod1));
            var original4 = typeof(RoomManager).GetMethod(RoomMethod1);
            var original5 = typeof(VRC_EventHandler).GetMethod(WorldTriggerMethod1);
            var original6 = typeof(Networking).GetMethod(GoToRoomMethod);
            var original7 = typeof(PostOffice).GetMethod(NotiMethod);
            //var original8 = typeof(PhotonNetwork).GetMethod(PingMethod);
            var original9 = typeof(VRCPlayer).GetMethod(AvatarChangeMethod);

            var Hook1 = typeof(UiUserList).GetMethod(nameof(UiUserList.Method_Protected_Virtual_Void_VRCUiContentButton_Object_0));
            var Hook1Patch = typeof(Prefixes).GetMethod(UiListPatch);
            var Hook2 = typeof(AssetBundleDownloadManager).GetMethods().Where(mi => mi.GetParameters().Length == 1 && mi.GetParameters().First().ParameterType == typeof(ApiAvatar) && mi.ReturnType == typeof(void));
            var Hook2Patch = typeof(Prefixes).GetMethod(AvatarChangePatch);
            var Hook3 = typeof(VRCAvatarManager.AvatarCreationCallback).GetMethod(nameof(VRCAvatarManager.AvatarCreationCallback.Invoke));//typeof(VRCAvatarManager.MulticastDelegateNPublicSealedVoGaVRBoUnique).GetMethod(nameof(VRCAvatarManager.MulticastDelegateNPublicSealedVoGaVRBoUnique.Invoke));
            var Hook3Patch = typeof(Prefixes).GetMethod(AvatarLoadPatch);
            var Hook4 = typeof(PageUserInfo).GetMethod(nameof(PageUserInfo.Method_Public_Void_APIUser_InfoType_ListType_0));
            var Hook4Patch = typeof(Prefixes).GetMethod(UserInfoPatch);
            var Hook5 = typeof(SystemInfo).GetMethod(HWIDMethod);
            var Hook5Patch = typeof(Prefixes).GetMethod(HWIDPatch);

            var Hook6 = typeof(AssetBundleDownloadManager).GetMethod("Method_Internal_Void_ApiWorld_PDM_0");
            var Hook6Patch = typeof(Prefixes).GetMethod("WorldDownload__Hook");

            var NamePlateHook1 = typeof(PlayerNameplate).GetMethod("Method_Private_Void_Single_Boolean_0");
            //var NamePlateHook2 = typeof(PlayerNameplate).GetMethod("Method_Private_Void_Single_Boolean_PDM_0");
            //var NamePlateHook3 = typeof(PlayerNameplate).GetMethod("Method_Private_Void_Single_Boolean_PDM_1");
            var NamePlatePatch1 = typeof(Prefixes).GetMethod("FloatPatch");

            var Hook7 = typeof(FlatBufferNetworkSerializer).GetMethod(nameof(FlatBufferNetworkSerializer.Method_Public_Void_EventData_0));
            var Hook7V2 = typeof(Photon.Realtime.LoadBalancingClient).GetMethod(nameof(Photon.Realtime.LoadBalancingClient.OnEvent));
            var Hook7V3 = typeof(VRCNetworkingClient).GetMethod(nameof(VRCNetworkingClient.OnEvent));
            //var Hook7V4 = typeof(VRC_EventLog.MonoBehaviour1NPublicObPrPrPrUnique).GetMethod("OnEvent");
            //var Hook7V3List = typeof(VRC_EventLog.MonoBehaviour1NPublicObPrPrPrUnique).GetMethods().Where(mi => mi.GetParameters().Length == 1 && mi.GetParameters().First().ParameterType == typeof(EventData) && mi.Name.StartsWith("Method_Public_Virtual_Final_New_Void_EventData_"));
            var Hook7Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.PhotonEvents));

            var Hook8 = typeof(VRC.UI.Core.Styles.StyleEngine).GetMethod("Method_Public_Void_ElementStyle_String_0");
            var Hook8Patch = typeof(Prefixes).GetMethod("ElementStyle__Hook");

            var Hook9 = typeof(VRC.UI.Elements.QuickMenu).GetMethod("OnDisable");
            var Hook9PrePatch = typeof(Prefixes).GetMethod("prepatch__QMOnClose");
            var Hook9PostPatch = typeof(Prefixes).GetMethod("postpatch__QMOnClose");

            var Hook10 = typeof(VRC.UI.Elements.QuickMenu).GetMethod("OnEnable");
            var Hook10PrePatch = typeof(Prefixes).GetMethod("prepatch__QMOnOpen");
            var Hook10PostPatch = typeof(Prefixes).GetMethod("postpatch__QMOnOpen");

            var Hook11 = typeof(PortalInternal).GetMethod("ConfigurePortal");
            var Hook11PrePatch = typeof(Prefixes).GetMethod("prepatch__PortalAwake");
            var Hook11PostPatch = typeof(Prefixes).GetMethod("postpatch__PortalAwake");

            //var testoriginal = typeof(VRCFlowManager.ObjectNPrivateSealedStObAcStOb1Ac1StStUnique).GetMethod(TestMethod);
            //var testoriginal2 = typeof(VRCFlowManager).GetMethod(TestMethod2);
            //moderation manager patches
            var ModOrg1 = typeof(ModerationManager.ObjectNPrivateSealedStStUnique).GetMethod(ModMethod1);
            //var ModOrg2 = typeof(ModerationManager.ObjectNPrivateSealedMoVoBomAp2).GetMethod(ModMethod2);
            //var ModOrg3 = typeof(ModerationManager.ObjectNPrivateSealedMoVoBomAp1).GetMethod(ModMethod3);
            //var ModOrg4 = typeof(ModerationManager.ObjectNPrivateSealedMoVoBomAp0).GetMethod(ModMethod4);
            var ModOrg5 = typeof(ModerationManager.__c).GetMethod(ModMethod5);
            var ModOrg6 = typeof(ModerationManager.__c).GetMethod(ModMethod6);
            var ModOrg7 = typeof(ModerationManager.__c).GetMethod(ModMethod7);
            //var ModOrg8 = typeof(ModerationManager.ObjectNPrivateSealedStAc1ApUnique).GetMethod(ModMethod8);
            //var ModOrg9 = typeof(ModerationManager.ObjectNPrivateSealedStAc1ApUnique).GetMethod(ModMethod9);
            var ModOrg10 = typeof(ModerationManager.ObjectNPrivateSealedApVoBomAp1).GetMethod(ModMethod10);
            //var ModOrg11 = typeof(ModerationManager.ObjectNPrivateSealedIE1ApLiVo1ApBotmm1).GetMethod(ModMethod11);
            //var ModOrg12 = typeof(ModerationManager.ObjectNPrivateSealedIE1ApLiVo1ApBotmm1).GetMethod(ModMethod12);
            var ModOrg13 = typeof(ModerationManager.ObjectNPrivateSealedApVoBomAp0).GetMethod(ModMethod13);
            //var ModOrg14 = typeof(ModerationManager.ObjectNPrivateSealedIE1ApLiVo1ApBotmm0).GetMethod(ModMethod14);
            //var ModOrg15 = typeof(ModerationManager.ObjectNPrivateSealedIE1ApLiVo1ApBotmm0).GetMethod(ModMethod15);
            var ModOrg16 = typeof(ModerationManager.ObjectNPrivateSealedObAcUnique).GetMethod(ModMethod16);
            var ModOrg17 = typeof(ModerationManager.ObjectNPrivateSealedObAcUnique).GetMethod(ModMethod17);
            var ModOrg18 = typeof(ModerationManager.ObjectNPrivateSealedObAcUnique).GetMethod(ModMethod18);
            var ModOrg19 = typeof(ModerationManager.ObjectNPrivateSealedObAcUnique).GetMethod(ModMethod19);

            var falseprefix = typeof(Prefixes).GetMethods().Where(m => m.GetParameters().Count() == 0).First();
            var CustomRPCPrefix = typeof(Prefixes).GetMethod(EventPatch);
            var RoomPrefix = typeof(Prefixes).GetMethod(RoomPatch);
            var triggerprefix = typeof(Prefixes).GetMethod(TriggerPatch);
            var GoToRoomPrefix = typeof(Prefixes).GetMethod(GoToRoomPatch);
            var AntiKickPrefix = typeof(Prefixes).GetMethod(AntiKickPatch);
            var NotiPrefix = typeof(Prefixes).GetMethod(NotiPatch);
            var PingPrefix = typeof(Prefixes).GetMethod(PingPatch);
            var AvatarChangePrefix = typeof(Prefixes).GetMethod(AvatarChangePatch);

            var ApiModBoolPrefix = typeof(Prefixes).GetMethod(ApiModPatch);
            var ApiPlrModBoolPrefix = typeof(Prefixes).GetMethod(ApiPlrModPatch);
            var ApiModContPrefix = typeof(Prefixes).GetMethod(ApiModContPatch);
            var ApiModEnumPrefix = typeof(Prefixes).GetMethod(ApiModEnumPatch);
            var ApiPlrModEnumPrefix = typeof(Prefixes).GetMethod(ApiPlrModEnumPatch);
            var ResultFalsePrefix = typeof(Prefixes).GetMethod(ResultFalsePatch);



            instance.Patch(original2, new HarmonyMethod(falseprefix), null, null);
            instance.Patch(original3, new HarmonyMethod(falseprefix), null, null);
            //foreach (var method in original28)
            //{
            instance.Patch(original28, new HarmonyMethod(CustomRPCPrefix), null, null);
            //}
            //instance.Patch(original4, new HarmonyMethod(RoomPrefix), null, null);
            instance.Patch(original5, new HarmonyMethod(triggerprefix), null, null);
            instance.Patch(original7, new HarmonyMethod(NotiPrefix), null, null);
            instance.Patch(NamePlateHook1, new HarmonyMethod(NamePlatePatch1), null, null);
            //instance.Patch(NamePlateHook2, new HarmonyMethod(NamePlatePatch1), null, null);
            //instance.Patch(NamePlateHook3, new HarmonyMethod(NamePlatePatch1), null, null);
            //instance.Patch(testoriginal, new HarmonyMethod(GoToRoomPrefix), null, null);
            /*instance.Patch(ModOrg1, new HarmonyMethod(AntiKickPrefix), null, null);
            instance.Patch(ModOrg2, new HarmonyMethod(ApiPlrModBoolPrefix), null, null);
            instance.Patch(ModOrg3, new HarmonyMethod(ApiModBoolPrefix), null, null);
            instance.Patch(ModOrg4, new HarmonyMethod(ApiModBoolPrefix), null, null);
            instance.Patch(ModOrg5, new HarmonyMethod(ApiModBoolPrefix), null, null);
            instance.Patch(ModOrg6, new HarmonyMethod(ApiModBoolPrefix), null, null);
            //instance.Patch(ModOrg7, new HarmonyMethod(ResultFalsePrefix), null, null);
            instance.Patch(ModOrg8, new HarmonyMethod(falseprefix), null, null);
            instance.Patch(ModOrg9, new HarmonyMethod(ApiModContPrefix), null, null);
            /*instance.Patch(ModOrg10, new HarmonyMethod(ApiPlrModBoolPrefix), null, null);
            instance.Patch(ModOrg11, new HarmonyMethod(ApiPlrModBoolPrefix), null, null);
            instance.Patch(ModOrg12, new HarmonyMethod(ApiPlrModBoolPrefix), null, null);
            instance.Patch(ModOrg13, new HarmonyMethod(ApiModBoolPrefix), null, null);
            instance.Patch(ModOrg14, new HarmonyMethod(ApiModBoolPrefix), null, null);
            instance.Patch(ModOrg15, new HarmonyMethod(ApiModBoolPrefix), null, null);
            instance.Patch(ModOrg16, new HarmonyMethod(ApiModEnumPrefix), null, null);
            instance.Patch(ModOrg17, new HarmonyMethod(ApiPlrModEnumPrefix), null, null);*/
            /*instance.Patch(ModOrg18, new HarmonyMethod(ApiModBoolPrefix), null, null);
            instance.Patch(ModOrg19, new HarmonyMethod(ApiPlrModBoolPrefix), null, null);*/
            //instance.Patch(original8, new HarmonyMethod(PingPrefix), null, null);
            instance.Patch(Hook6, new HarmonyMethod(Hook6Patch));
            instance.Patch(Hook7, new HarmonyMethod(Hook7Patch));
            instance.Patch(Hook7V2, new HarmonyMethod(Hook7Patch));
            //instance.Patch(Hook7V3, new HarmonyMethod(Hook7Patch));
            /*foreach (var method in Hook7V3List)
            {
                instance.Patch(method, new HarmonyMethod(Hook7Patch));
            }*/
            instance.Patch(Hook8, new HarmonyMethod(Hook8Patch));
            instance.Patch(Hook9, new HarmonyMethod(Hook9PrePatch), new HarmonyMethod(Hook9PostPatch));
            instance.Patch(Hook10, new HarmonyMethod(Hook10PrePatch), new HarmonyMethod(Hook10PostPatch));
            instance.Patch(Hook11, new HarmonyMethod(Hook11PrePatch), new HarmonyMethod(Hook11PostPatch));
            instance.Patch(typeof(CameraUtil._TakeScreenShot_d__5).GetMethod("MoveNext"), new HarmonyMethod(AccessTools.Method(typeof(Prefixes), nameof(Prefixes.patch__camera))));
            //instance.Patch(original9, new HarmonyMethod(AvatarChangePrefix), null, null);
            instance.Patch(Hook1, new HarmonyMethod(Hook1Patch), null, null);
            //instance.Patch(Hook2, new HarmonyMethod(Hook2Patch), null, null);
            foreach (var method in Hook2)
            {
                instance.Patch(method, new HarmonyMethod(Hook2Patch), null, null);
            }
            instance.Patch(Hook3, new HarmonyMethod(Hook3Patch), null, null);
            instance.Patch(Hook4, new HarmonyMethod(Hook4Patch), null, null);
            instance.Patch(Hook5, new HarmonyMethod(Hook5Patch), null, null);
            //instance.Patch(testoriginal2, new HarmonyMethod(GoToRoomPrefix), null, null);


            var methods = instance.GetPatchedMethods();
            foreach (var method in methods)
            {
                if (!string.IsNullOrEmpty(method.Name) && !method.Name.ToLower().Contains("get_") && !method.Name.ToLower().Contains("set_")) PendulumLogger.Log(ConsoleColor.DarkGray, "[Harmony] Patched Method: {0}", method.Name);
            }
            PendulumLogger.Log(ConsoleColor.Green, "[Harmony] Patched Harmony Instance!");

            if (FirstTimeinit == true && LogoDataDownloaded == true)
            {
                PendulumLogger.Log("First Time Initalization Detected.");
                PendulumLogger.Log("Restarting game to apply AssetBundle.");
                ForceRestart();
            }
            //SetupEventPatches();
        }
        public static void SetupEventPatches()
        {
            foreach (var nestedType in typeof(VRC_EventLog).GetNestedTypes())
            {
                foreach (var methodInfo in nestedType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!methodInfo.Name.StartsWith("Method_Public_Virtual_Final_New_Void_EventData_")) continue;

                    unsafe
                    {
                        var originalMethodPtr = *(IntPtr*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(methodInfo).GetValue(null);

                        EventDelegate originalDelegate = null;

                        void OnEventDelegate(IntPtr thisPtr, IntPtr eventDataPtr, IntPtr nativeMethodInfo)
                        {
                            if (eventDataPtr == IntPtr.Zero)
                            {
                                originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                                return;
                            }

                            var eventData = new EventData(eventDataPtr);

                            if (eventData.Code != 6 && eventData.Code != 9)
                            {
                                originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                                return;
                            }

                            if (!IsRPCBad(eventData))
                                originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                        }

                        var patchDelegate = new EventDelegate(OnEventDelegate);
                        OurPinnedDelegates.Add(patchDelegate);

                        MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPtr), Marshal.GetFunctionPointerForDelegate(patchDelegate));
                        originalDelegate = Marshal.GetDelegateForFunctionPointer<EventDelegate>(originalMethodPtr);
                    }
                }
            }
            unsafe
            {
                var originalMethodPtr = *(IntPtr*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(VRCNetworkingClient).GetMethod(nameof(VRCNetworkingClient.OnEvent))).GetValue(null);

                EventDelegate originalDelegate = null;

                void OnEventDelegate(IntPtr thisPtr, IntPtr eventDataPtr, IntPtr nativeMethodInfo)
                {
                    if (eventDataPtr == IntPtr.Zero)
                    {
                        originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                        return;
                    }

                    var eventData = new EventData(eventDataPtr);

                    if (!ShouldBlockEvent(eventData))
                        originalDelegate(thisPtr, eventDataPtr, nativeMethodInfo);
                }

                var patchDelegate = new EventDelegate(OnEventDelegate);
                OurPinnedDelegates.Add(patchDelegate);

                MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPtr), Marshal.GetFunctionPointerForDelegate(patchDelegate));
                originalDelegate = Marshal.GetDelegateForFunctionPointer<EventDelegate>(originalMethodPtr);
            }
        }
        public static bool IsRPCBad(EventData data)
        {
            return data.Code == 9;
        }
        public static bool ShouldBlockEvent(EventData data)
        {
            return data.Code == 9;
        }
        public static void PatchOldHarmony()
        {
            /*try
            {
                PendulumLogger.Log(ConsoleColor.DarkYellow, "[Harmony] Generating new Harmony Instance.");
                var harmony = HarmonyInstance.Create("PendulumClient.HarmonyInstance.V2");
                //var original1 = typeof(ModerationManager).GetMethod("KickUserRPC");
                var original2 = typeof(PortalInternal).GetMethod("Method_Private_Void_1"); //DestroyPortal
                var original3 = typeof(PortalInternal).GetMethod("Method_Private_Void_0"); //DestroyPortalSpawn
                //var original3 = typeof(ModerationManager).GetMethod("Method_Public_Boolean_String_1"); //BlockedEitherWay
                var original4 = typeof(VRC_EventHandler).GetMethod("InternalTriggerEvent");
                //var original5 = typeof(ModerationManager).GetMethod("Method_Private_ApiModeration_ModerationType_0");
                //var original6 = typeof(ModerationManager).GetMethod("ReceiveVoteToKickInitiation");
                //var original7 = typeof(ModerationManager).GetMethod("Method_Public_Boolean_String_String_String_0"); //IsKickedFromWorld
                //var original8 = typeof(ModerationManager).GetMethod("Method_Public_Boolean_String_String_String_1"); //PublicOnlyBannedFromWorld
                //var original9 = typeof(ModerationManager).GetMethod("Method_Public_Void_APIUser_String_APIUser_PDM_0"); //UserKickUser
                //var original10 = typeof(ModerationManager).GetMethod("Method_Public_Boolean_APIUser_0"); //TryKick
                //var original11 = typeof(ModerationManager).GetMethod("Method_Public_Void_String_String_Boolean_PDM_0"); //SendVoteToKick
                //var original12 = typeof(ModerationManager).GetMethod("Method_Private_Boolean_Boolean_0"); //IsKicked
                //var original13 = typeof(ModerationManager).GetMethod("ShowUserAvatarChangedRPC");
                //var original14 = typeof(ModerationManager).GetMethod("MuteChangeRPC");
                //var original15 = typeof(ModerationManager).GetMethod("FriendStateChangeRPC");
                //var original16 = typeof(ModerationManager).GetMethod("ResetShowUserAvatarToDefaultRPC");
                //var original17 = typeof(ModerationManager).GetMethod("ModForceOffMicRPC")
                //var original18 = typeof(PhotonAnimatorView).GetMethod("OnPhotonSerializeView");
                //var original19 = typeof(PhotonView).GetMethod("Method_Public_Void_ObjectPublicQu1ObByObBoInBoBoUnique_ValueTypePublicSealedInObPhDoInUnique_PDM_0");
                //var original20 = typeof(PhotonView).GetMethod("Method_Public_Void_ObjectPublicQu1ObByObBoInBoBoUnique_ValueTypePublicSealedInObPhDoInUnique_PDM_1");
                //var original21 = typeof(PhotonView).GetMethod("Method_Public_Void_ObjectPublicQu1ObByObBoInBoBoUnique_ValueTypePublicSealedInObPhDoInUnique_PDM_2");
                //var original22 = typeof(PhotonView).GetMethod("Method_Public_Void_ObjectPublicQu1ObByObBoInBoBoUnique_ValueTypePublicSealedInObPhDoInUnique_PDM_3");
                //var original23 = typeof(PhotonView).GetMethod("Method_Public_Void_ObjectPublicQu1ObByObBoInBoBoUnique_ValueTypePublicSealedInObPhDoInUnique_PDM_4");
                //var original24 = typeof(PhotonView).GetMethod("Method_Public_Void_ObjectPublicQu1ObByObBoInBoBoUnique_ValueTypePublicSealedInObPhDoInUnique_PDM_5");
                //var original25 = typeof(NotificationManager).GetMethod("Method_Public_Void_Notification_1");
                //var FlowManager1 = typeof(VRCFlowManager).GetMethod("Method_Public_Boolean_String_Boolean_0");
                //var FlowManager2 = typeof(VRCFlowManager).GetMethod("Method_Private_Boolean_String_String_Boolean_0");
                //var FlowManager3 = typeof(VRCFlowManager).GetMethod("Method_Private_Boolean_String_Int32_PDM_0");
                var original26 = typeof(VRC_EventHandler).GetMethods();
                var original27 = typeof(PhotonNetwork).GetMethod("Method_Private_Static_Void_EventData_PDM_0");
                //var original28 = typeof(VRC_EventDispatcherRFC).GetMethod("Method_Public_Void_Player_VrcEvent_VrcBroadcastType_Int32_Single_0");
                //var CustomEvents = typeof(VRC_EventDispatcherRFC).GetMethod("Method_Public_Boolean_Player_VrcEvent_VrcBroadcastType_0");
                //var ApiModeration = typeof(ApiModeration).GetMethod("SendModeration");
                //var ApiPlayerModeration = typeof(ApiPlayerModeration).GetMethod("SendModeration");
                var kickprefix = typeof(Prefixes).GetMethod("KickUserPatch");
                var falseprefix = typeof(Prefixes).GetMethod("falsePatch");
                var blockprefix = typeof(Prefixes).GetMethod("BlockStateChangePrefix");
                var blockprefix2 = typeof(Prefixes).GetMethod("IsBlockedPrefix");
                var triggerprefix = typeof(Prefixes).GetMethod("TriggerEvent");
                var vtkinitprefix = typeof(Prefixes).GetMethod("VoteToKickInit");
                var triplestringsprefix = typeof(Prefixes).GetMethod("TStringOutput");
                var triplestringsprefixf = typeof(Prefixes).GetMethod("TStringOutputFalse");
                var userkickuserprefix = typeof(Prefixes).GetMethod("UserKickUserPatch");
                var trykickprefix = typeof(Prefixes).GetMethod("TryKickPatch");
                var sendvotekickprefix = typeof(Prefixes).GetMethod("SendVoteKickPatch");
                var iskickedprefix = typeof(Prefixes).GetMethod("IsKickedPatch");
                var avatarstateprefix = typeof(Prefixes).GetMethod("AvatarStateChangePrefix");
                var mutestateprefix = typeof(Prefixes).GetMethod("MuteStateChangePrefix");
                var friendstateprefix = typeof(Prefixes).GetMethod("FriendStateChangePrefix");
                var CustomRPCPrefix = typeof(Prefixes).GetMethod("CustomRPCPatch");
                var ForceMicOffPrefix = typeof(Prefixes).GetMethod("ForceMicOffPrefix");
                var SerializePrefix = typeof(Prefixes).GetMethod("SerializePrefix");
                var NotifacationPrefix = typeof(Prefixes).GetMethod("NotifacationPatch");
                var EventPrefix = typeof(Prefixes).GetMethod("EventPatch");
                var ApiPlayerModerationPatch = typeof(Prefixes).GetMethod("PlayerModerationPatch");
                var ApiModerationPatch = typeof(Prefixes).GetMethod("ApiModerationPrefix");
                //var RecieveEventPrefix = typeof(Prefixes).GetMethod("RecieveEventPatch");
                var FlowManagerPrefix1 = typeof(Prefixes).GetMethod("FlowManagerPatch1");
                var FlowManagerPrefix2 = typeof(Prefixes).GetMethod("FlowManagerPatch2");
                var FlowManagerPrefix3 = typeof(Prefixes).GetMethod("FlowManagerPatch3");

                //harmony.Patch(original1, new HarmonyMethod(kickprefix), null, null);
                //harmony.Patch(original28, new HarmonyMethod(CustomRPCPrefix), null, null);
                //harmony.Patch(original2, new HarmonyMethod(falseprefix), null, null);
                //harmony.Patch(original3, new HarmonyMethod(falseprefix), null, null);
                //harmony.Patch(original4, new HarmonyMethod(triggerprefix), null, null);
                //harmony.Patch(original5, new HarmonyMethod(ApiModerationPatch), null, null);
                //harmony.Patch(original6, new HarmonyMethod(vtkinitprefix), null, null);
                //harmony.Patch(original13, new HarmonyMethod(avatarstateprefix), null, null);
                //harmony.Patch(original14, new HarmonyMethod(mutestateprefix), null, null);
                //harmony.Patch(original15, new HarmonyMethod(friendstateprefix), null, null);
                //harmony.Patch(original7, new HarmonyMethod(triplestringsprefix), null, null);
                //harmony.Patch(original8, new HarmonyMethod(triplestringsprefixf), null, null);
                //harmony.Patch(original9, new HarmonyMethod(userkickuserprefix), null, null);
                //harmony.Patch(original10, new HarmonyMethod(trykickprefix), null, null);
                //harmony.Patch(original11, new HarmonyMethod(sendvotekickprefix), null, null);
                //harmony.Patch(original12, new HarmonyMethod(iskickedprefix), null, null);
                //harmony.Patch(original16, new HarmonyMethod(CustomRPCPrefix), null, null);
                //harmony.Patch(original17, new HarmonyMethod(ForceMicOffPrefix), null, null);
                //harmony.Patch(original18, new HarmonyMethod(SerializePrefix), null, null);
                //harmony.Patch(original19, new HarmonyMethod(SerializePrefix), null, null);
                //harmony.Patch(original21, new HarmonyMethod(SerializePrefix), null, null);
                //harmony.Patch(original22, new HarmonyMethod(SerializePrefix), null, null);
                //harmony.Patch(original23, new HarmonyMethod(SerializePrefix), null, null);
                //harmony.Patch(original24, new HarmonyMethod(SerializePrefix), null, null);
                //harmony.Patch(original25, new HarmonyMethod(NotifacationPrefix), null, null);
                //harmony.Patch(ApiPlayerModeration, new HarmonyMethod(ApiPlayerModerationPatch), null, null);
                //harmony.Patch(ApiModeration, new HarmonyMethod(ApiModerationPatch), null, null);
                //harmony.Patch(FlowManager1, new HarmonyMethod(FlowManagerPrefix1), null, null);
                //harmony.Patch(FlowManager2, new HarmonyMethod(FlowManagerPrefix2), null, null);
                //harmony.Patch(FlowManager3, new HarmonyMethod(FlowManagerPrefix3), null, null);

                /*foreach (MethodInfo method in original26)
                {
                    if (method.Name == "TriggerEvent")
                    {
                        if (method.GetParameters()[2].Name == "instagatorId")
                        {
                            harmony.Patch(method, new HarmonyMethod(EventPrefix), null, null);
                        }
                    }
                }

                var methods = harmony.GetPatchedMethods();
                foreach (var method in methods)
                {
                    if (!string.IsNullOrEmpty(method.Name) && !method.Name.ToLower().Contains("get") && !method.Name.ToLower().Contains("set")) PendulumLogger.Log(ConsoleColor.DarkGray, "[Harmony] Patched Method: {0}", method.Name);
                }
                PendulumLogger.Log(ConsoleColor.Green, "[Harmony] Patched Harmony Instance!");
                if (FirstTimeinit == true && LogoDataDownloaded == true)
                {
                    PendulumLogger.Log("First Time Initalization Detected.");
                    PendulumLogger.Log("Restarting game to apply AssetBundle.");
                    ForceRestart();
                }

            }
            catch (Exception arg)
            {
                PendulumLogger.Log(ConsoleColor.Red, "[Harmony] Error while patching Harmony: {0}", arg);
            }*/
        }

        public void PatchSteamID()
        {
            var harmony = this.HarmonyInstance; //HarmonyInstance.Create("PendulumClient.SteamIDInstance");
            var original25 = typeof(ISteamUser).GetMethod("GetSteamID");
            var SteamIDPrefix = typeof(Prefixes).GetMethod("patch__anti__steam");
            harmony.Patch(original25, new HarmonyMethod(SteamIDPrefix), null, null);
        }

        private static VRC_EventHandler bruhhandler1;
        public static void PendulumClientUserModeration(string userid, string moderationtype)
        {
            string output = "PCM:" + userid + ":" + moderationtype;

            if (bruhhandler1 == null) bruhhandler1 = Resources.FindObjectsOfTypeAll<VRC_EventHandler>()[0];

            VRC_EventHandler.VrcEvent CustomEvent = new VRC_EventHandler.VrcEvent
            {
                EventType = VRC_EventHandler.VrcEventType.SendRPC,
                Name = "卐",
                ParameterObject = bruhhandler1.gameObject,
                ParameterInt = int.MinValue,
                ParameterFloat = float.MinValue,
                ParameterString = output
            };
            bruhhandler1.TriggerEvent(CustomEvent, VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered, VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject, 0f);
        }

        private static VRC_EventHandler bruhhandler2;
        public static void PendulumClientVideoPlayerModeration(string videolink)
        {
            string output = "PCM:VOID_MC:" + videolink;
            if (bruhhandler2 == null)
                bruhhandler2 = Resources.FindObjectsOfTypeAll<VRC_EventHandler>()[0];
            VRC_EventHandler.VrcEvent CustomEvent = new VRC_EventHandler.VrcEvent
            {
                EventType = VRC_EventHandler.VrcEventType.ActivateCustomTrigger,
                Name = "",
                ParameterObject = bruhhandler2.gameObject,
                ParameterInt = -99,
                ParameterFloat = -99f,
                ParameterString = output,
                ParameterBoolOp = VRC_EventHandler.VrcBooleanOp.Unused,
                ParameterBytes = new Il2CppStructArray<byte>(0),
                ParameterBool = false,
            };
            bruhhandler2.TriggerEvent(CustomEvent, VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered, VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject, 0f);
        }

        public void SetNameToTrustRank(Player player)
        {
            /*if (player._vrcplayer.prop_Int16_0 == 0 && player.prop_APIUser_0.id != APIUser.CurrentUser.id)
            {
                GameObject.Destroy(player.field_Private_USpeaker_0.gameObject);
            }*/
            if (player.prop_APIUser_0 != null && player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - NamePlate").GetComponent<Image>() != null && player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject != null)
            {
                var JoinedPlayer = player.prop_APIUser_0;
                if (JoinedPlayer.last_platform != "android")
                {
                    var PlayerNameText = player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject;
                    var TextTransformParent = player.gameObject.transform.Find("Canvas - Profile (1)/Text");
                    GameObject NewNameText = GameObject.Instantiate(PlayerNameText, TextTransformParent);
                    NewNameText.name = "Text - GeneratedNameTagColor";
                    NewNameText.gameObject.SetActiveRecursively(true);
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
                    NewNameText.GetComponent<Text>().color = UnityEngine.Color.red;
                }
            }
        }

        public static void ForceRestartButton()
        {
            ColorBlock colorBlock = default(ColorBlock);
            colorBlock.colorMultiplier = 1f;
            colorBlock.disabledColor = UnityEngine.Color.grey;
            colorBlock.highlightedColor = ColorModule.ColorModule.CachedColor * 1.5f;
            colorBlock.normalColor = ColorModule.ColorModule.CachedColor / 1.5f;
            colorBlock.pressedColor = UnityEngine.Color.grey * 1.5f;
            colorBlock.fadeDuration = 0.1f;
            ColorBlock colors = colorBlock;

            var MidButton = GameObject.Find("MenuContent/Popups/LoadingPopup/ButtonMiddle").gameObject;
            var ButtonParent = GameObject.Find("MenuContent/Popups/LoadingPopup").gameObject;
            MidButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 0f);//new Vector2(0f, 62f);
            var RestartButton = UnityEngine.Object.Instantiate(MidButton, ButtonParent.transform, true).GetComponentInChildren<Button>();
            RestartButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(400f, 0f);//new Vector2(0f, -248f);
            RestartButton.gameObject.SetActive(true);
            RestartButton.name = "RestartButton";
            RestartButton.GetComponentInChildren<Text>().text = "Restart";
            RestartButton.GetComponent<Button>().colors = colors;
            RestartButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            RestartButton.GetComponent<Button>().onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(new Action(delegate ()
            {
                ForceRestart();
            })));

            var ExitButton = UnityEngine.Object.Instantiate(MidButton, ButtonParent.transform, true).GetComponentInChildren<Button>();
            ExitButton.GetComponent<RectTransform>().anchoredPosition += new Vector2(-400f, 0f);//new Vector2(0f, -124f);
            ExitButton.gameObject.SetActive(true);
            ExitButton.name = "ExitButton";
            ExitButton.GetComponentInChildren<Text>().text = "Exit";
            ExitButton.GetComponent<Button>().colors = colors;
            ExitButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            ExitButton.GetComponent<Button>().onClick.AddListener(DelegateSupport.ConvertDelegate<UnityAction>(new Action(delegate ()
            {
                Process.GetCurrentProcess().Kill();
            })));

            //MidButton.GetComponent<BoxCollider>().size = new Vector3(300f, 300f, 1);
            ColorModuleV2.CMV2_ColorModule.LoadingScreenStuff();

            RestartButtonSetup = true;
        }

        public static void UpdateLoadingScreenButtons()
        {
            if (GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ExitButton") == null)
                return;

            if (GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/RestartButton") == null)
                return;

            var ExitButton = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/ExitButton");
            var RestartButton = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/RestartButton");

            ExitButton.GetComponent<BoxCollider>().center = new Vector3(0f, 0.25f, 0f);
            RestartButton.GetComponent<BoxCollider>().center = new Vector3(0f, 0.5f, 0f);
            
            ExitButton.GetComponent<BoxCollider>().bounds.Expand(new Vector3(0f, 0.25f, 0f));
            RestartButton.GetComponent<BoxCollider>().bounds.Expand(new Vector3(0f, 0.5f, 0f));
            ExitButton.GetComponent<BoxCollider>().bounds.Expand(1f);
        }

        public static void ForceRestart()
        {
            GoHome();
            try
            {
                Process.Start(Environment.CurrentDirectory + "\\VRChat.exe", Environment.CommandLine.ToString());
            }
            catch (Exception)
            {
                new Exception();
            }
            Process.GetCurrentProcess().Kill();
        }

        private static VRC_EventHandler bruhhandler;
        public static void SendBruhEvent()
        {
            if (bruhhandler == null)
                bruhhandler = Resources.FindObjectsOfTypeAll<VRC_EventHandler>()[0];
            VRC_EventHandler.VrcEvent e = new VRC_EventHandler.VrcEvent
            {
                EventType = VRC_EventHandler.VrcEventType.ActivateCustomTrigger,
                Name = "",
                ParameterObject = bruhhandler.gameObject,
                ParameterInt = -99,
                ParameterFloat = -99f,
                ParameterString = "bruh",
                ParameterBoolOp = VRC_EventHandler.VrcBooleanOp.Unused,
                ParameterBytes = new Il2CppStructArray<byte>(0),
                ParameterBool = false,
            };
            bruhhandler.TriggerEvent(e, VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered, VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject, 0f);
        }

        private VRC_EventHandler handler;
        public void Disconnect()
        {
            if (handler == null)
                handler = Resources.FindObjectsOfTypeAll<VRC_EventHandler>()[0];

            VRC_EventHandler.VrcEvent vrcEvent = new VRC_EventHandler.VrcEvent
            {
                EventType = VRC_EventHandler.VrcEventType.AddHealth,
                Name = "AddHealth",
                ParameterObject = handler.gameObject,
                ParameterInt = 1,
                ParameterFloat = 0f,
                ParameterString = $"<color=red>Bye Bye</color> | <color=cyan>{RandomString(666)}</color>",
                ParameterBoolOp = VRC_EventHandler.VrcBooleanOp.Unused,
                ParameterBytes = new Il2CppStructArray<byte>(0),
            };
            handler.TriggerEvent(vrcEvent, VRC_EventHandler.VrcBroadcastType.Always, PlayerWrappers.GetAllPlayers()[new System.Random().Next(PlayerWrappers.GetAllPlayers()._size - 1)].gameObject, 0f);
        }

        private static VRC_EventHandler handler2;
        public static void Tpspam(Player player)
        {
            if (handler2 == null)
                handler2 = Resources.FindObjectsOfTypeAll<VRC_EventHandler>()[0];

            VRC_EventHandler.VrcEvent vrcEvent = new VRC_EventHandler.VrcEvent
            {
                EventType = VRC_EventHandler.VrcEventType.TeleportPlayer,
                Name = "TeleportPlayer",
                ParameterObject = handler2.gameObject,
                ParameterInt = int.MaxValue,
                ParameterFloat = float.MaxValue,
                ParameterString = $"<color=red>Bye Bye</color> | <color=cyan>{RandomString2(666)}</color>",
                ParameterBoolOp = VRC_EventHandler.VrcBooleanOp.Unused,
                ParameterBytes = new Il2CppStructArray<byte>(0),
            };
            handler2.TriggerEvent(vrcEvent, VRC_EventHandler.VrcBroadcastType.Always, player.gameObject, 0f);
        }

        private static string RandomString(int length)
        {
            var Chars = "abcdefghlijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789".ToArray();
            string returnstring = "";
            System.Random random = new System.Random(new System.Random().Next(length));
            for (int i = 0; i < length; i++)
                returnstring += Chars[random.Next(Chars.Length)];
            return returnstring;
        }

        private static string RandomString2(int length)
        {
            var Chars = "abcdefghlijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789".ToArray();
            string returnstring = "";
            System.Random random = new System.Random(new System.Random().Next(length));
            for (int i = 0; i < length; i++)
                returnstring += Chars[random.Next(Chars.Length)];
            return returnstring;
        }

        public static void TpObjectsToPlayer(Player player, string name1 = null, string name2 = null)
        {
            foreach (VRC_Pickup pickup in GameObject.FindObjectsOfType<VRC_Pickup>())
            {
                if (string.IsNullOrEmpty(name1) && string.IsNullOrEmpty(name2))
                {
                    Networking.SetOwner(PlayerWrappers.GetCurrentPlayer().field_Private_VRCPlayerApi_0, pickup.gameObject);
                    pickup.transform.SetPositionAndRotation(player.gameObject.transform.position, player.gameObject.transform.rotation);
                }
                else if (string.IsNullOrEmpty(name2))
                {
                    if (pickup.gameObject.name.Contains(name1))
                    {
                        Networking.SetOwner(PlayerWrappers.GetCurrentPlayer().field_Private_VRCPlayerApi_0, pickup.gameObject);
                        pickup.transform.SetPositionAndRotation(player.gameObject.transform.position, player.gameObject.transform.rotation);
                    }
                }
                else
                {
                    if (pickup.gameObject.name.Contains(name1) || pickup.gameObject.name.Contains(name2))
                    {
                        Networking.SetOwner(PlayerWrappers.GetCurrentPlayer().field_Private_VRCPlayerApi_0, pickup.gameObject);
                        pickup.transform.SetPositionAndRotation(player.gameObject.transform.position, player.gameObject.transform.rotation);
                    }
                }
            }
        }

        public static void DropAllObjects()
        {
            foreach (VRC_Pickup pickup in GameObject.FindObjectsOfType<VRC_Pickup>())
            {
                Networking.RPC(RPC.Destination.AllBufferOne, pickup.transform.gameObject, "Drop", null);
                Networking.RPC(RPC.Destination.AllBufferOne, pickup.gameObject, "Drop", null);
            }
        }

        public static GameObject blankmenupage(string name)
        {
            var qmenu = Wrappers.GetQuickMenu();
            var menutocopy = qmenu.transform.Find("ShortcutMenu");
            var tfmMenu = UnityEngine.Object.Instantiate<GameObject>(menutocopy.gameObject).transform;
            tfmMenu.transform.name = name;
            for (var i = 0; i < tfmMenu.childCount; i++) GameObject.Destroy(tfmMenu.GetChild(i).gameObject);
            tfmMenu.SetParent(qmenu.transform, false);
            tfmMenu.gameObject.SetActive(false);
            return tfmMenu.gameObject;
        }

        public static Player GetPlayerByID(int plrid)
        {
            return PlayerManager.Method_Public_Static_Player_Int32_0(plrid);
        }
        public static int delete_portals()
        {
            var num = 0;
            foreach (PortalInternal p in UnityEngine.Object.FindObjectsOfType<PortalInternal>())
            {
                if (p.GetComponent<PhotonView>() != null)
                {
                    if (GetPlayerByID(p.prop_Int32_0) != null)
                    {
                        Player player = GetPlayerByID(p.prop_Int32_0);
                        PendulumLogger.Log("Deleted portal owned by: " + player.prop_APIUser_0.displayName);
                    }
                    else
                    {
                        PendulumLogger.Log("Deleted portal with null owner");
                    }
                    UnityEngine.Object.Destroy(p.transform.root.gameObject);
                    num++;
                }
            }
            return num;
        }

        public static void delete_portal(PortalInternal p)
        {
            if (p.GetComponent<PhotonView>() != null)
            {
                if (GetPlayerByID(p.prop_Int32_0) != null)
                {
                    Player player = GetPlayerByID(p.prop_Int32_0);
                    PendulumLogger.Log("Deleted portal owned by: " + player.prop_APIUser_0.displayName);
                }
                else
                {
                    PendulumLogger.Log("Deleted portal with null owner");
                }
                UnityEngine.Object.Destroy(p.transform.root.gameObject);
            }
        }

        public static void HookMethod(IntPtr OldMethod, IntPtr NewMethod)
        {
            HookM.Invoke(null, new object[]
            {
                OldMethod,
                NewMethod
            });
        }

        public static void user_menu_toggle_handler()
        {
            var qm = Wrappers.GetQuickMenu();
            if (qm != null && qm.prop_Boolean_0 == false)
            {
                user_menu.SetActive(false);
                portal_menu.SetActive(false);
                pcu_moderation_menu.SetActive(false);

                //VRCUiManager.prop_VRCUiManager_0.Method_Internal_Boolean_0();

                if (qm != null)
                {
                    try
                    {
                        var o = Wrappers.GetQuickMenu().transform.Find("UserInteractMenu");
                        o.gameObject.SetActive(false);
                    }
                    catch
                    (Exception e)
                    {
                        PendulumLogger.Log("Exception Caught: " + e);
                    }
                }
            }
        }

        public static void dev_tools_menu_toggle_handler()
        {
            var qm = Wrappers.GetQuickMenu();
            if (qm != null && qm.prop_Boolean_0 == false)
            {
                PendulumClientMenu.SetActive(false);
                SelectedUserMenu.SetActive(false);
                FunctionsMenu.SetActive(false);
                ProtectionsMenu.SetActive(false);

                //VRCUiManager.prop_VRCUiManager_0.Method_Internal_Boolean_0();

                if (qm != null)
                {
                    try
                    {
                        //var o = Wrappers.GetQuickMenu().transform.Find("ModerationMenu");
                        PendulumClientMenu.SetActive(false);
                        SelectedUserMenu.SetActive(false);
                        FunctionsMenu.SetActive(false);
                        ProtectionsMenu.SetActive(false);
                    }
                    catch
                    (Exception e)
                    {
                        PendulumLogger.Log("Exception Caught: " + e);
                    }
                }
            }
        }

        public static void shortcut_menu_toggle_handler()
        {
            var qm = Wrappers.GetQuickMenu();
            if (qm != null && qm.prop_Boolean_0 == false)
            {
                Wrappers.GetQuickMenu().transform.Find("ShortcutMenu").gameObject.SetActive(false);

                //VRCUiManager.prop_VRCUiManager_0.Method_Internal_Boolean_0();

                if (qm != null)
                {
                    try
                    {
                        var o = Wrappers.GetQuickMenu().transform.Find("ShortcutMenu").gameObject;
                        o.SetActive(false);
                    }
                    catch
                    (Exception e)
                    {
                        PendulumLogger.Log("Exception Caught: " + e);
                    }
                }
            }
        }

        public static void SaveUserID(APIUser apiuser)
        {
            if (apiuser.id != APIUser.CurrentUser.id) StoredUserID = apiuser.id; PendulumLogger.Log("Set Selected User to: {0} ({1})", apiuser.displayName, apiuser.id);
            StoredUserInInstance = true;
            AlertPopup.SendAlertPopup("Selected:\n" + apiuser.displayName);
            UpdatePlayerList();
        }

        public static void UnloadUserID()
        {
            StoredUserInInstance = false;
            StoredUserID = string.Empty;
        }

        public static void SaveUserIDSocial(string username, string userid)
        {
            var User = new System.Collections.Generic.List<string>();
            User.Add(userid);
            User.Add(username);
            if (PlayerWrappers.GetPlayer(userid) != null)
            {
                StoredUserInInstance = true;
                StoredUserID = userid;
            }
            else
            {
                StoredUserInInstance = false;
                StoredPlayer = User;
            }
        }

        public static string GetRegion(ApiWorldInstance instance)
        {
            var region = "";
            if (instance.region == NetworkRegion.US_West)
            {
                region = "usw";
            }
            if (instance.region == NetworkRegion.US_East)
            {
                region = "us";
            }
            if (instance.region == NetworkRegion.Europe)
            {
                region = "eu";
            }
            if (instance.region == NetworkRegion.Japan)
            {
                region = "jp";
            }
            return region;
        }
        public static void PortalCreator(IntPtr instance)
        {
            bool flag = IntPtr.Zero != instance;
            if (flag)
            {
                if (AntiPortalToggle == true)
                {
                    PortalInternal portalInternal = new PortalInternal(instance);
                    if (PlayerManager.Method_Public_Static_Player_Int32_0(portalInternal.prop_Int32_0) != null)
                    {
                        if (AntiPortalMsg == false)
                        {
                            Player player = PlayerManager.Method_Public_Static_Player_Int32_0(portalInternal.prop_Int32_0);
                            PortalDropperName = player.prop_APIUser_0.displayName;
                            AntiPortalMsg = true;
                        }
                        //PendulumLogger.Log(ConsoleColor.DarkRed, "Portal Dropped By: {0}", player.prop_APIUser_0.displayName);
                        //AlertPopup.SendAlertPopup("Portal Dropped By:\n" + player.prop_APIUser_0.displayName);
                    }
                    else
                    {
                        //PendulumLogger.Log(ConsoleColor.Red, "Portal owned by the World or the Portal Owner Left");
                        //AlertPopup.SendAlertPopup("Portal owned by the World\nor the Portal Owner Left");
                        PortalDropperName = string.Empty;
                        AntiPortalMsg = true;
                    }
                }
                else
                {
                    IsLoading = true;
                    OldEnter(instance);
                }
            }
        }

        public static void EmojiRPC(int i)
        {
            try
            {
                var x = new Il2CppSystem.Int32();
                x.m_value = i;
                var obj = x.BoxIl2CppObject();
                Networking.RPC(0, PlayerWrappers.GetCurrentPlayer().gameObject, "SpawnEmojiRPC", new Il2CppSystem.Object[]
                {
                    obj,
                });
            }
            catch
            {
            }
        }
        public static void EmoteRPC(int i)
        {
            try
            {
                var x = new Il2CppSystem.Int32();
                x.m_value = i;
                var obj = x.BoxIl2CppObject();
                Networking.RPC(0, PlayerWrappers.GetCurrentPlayer().gameObject, "PlayEmoteRPC", new Il2CppSystem.Object[]
                {
                    obj,
                });
            }
            catch
            {
            }
        }

        private void SocialInvite()
        {
            var ff = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = ff.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            var a = RoomManager.field_Internal_Static_ApiWorld_0.id;
            var b = "";//RoomManager.field_Internal_Static_ApiWorld_0.currentInstanceIdWithTags;
            var WorldName = RoomManager.field_Internal_Static_ApiWorld_0.name;
            if (userInfo != null)
            {
                VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Invite.Create(userInfo.field_Private_APIUser_0.id, "", new Location(a, new Transmtn.DTO.Instance(b, "", "", "", "", false)), WorldName));
                PendulumLogger.Log(a + ":" + b);
                Errormsg("Sent Invite!", "Invite sent to: " + userInfo.field_Private_APIUser_0.displayName + "!");
            };

        }

        private void SocialMSG()
        {
            var ff = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = ff.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            text_popup("Enter text to send to " + userInfo.field_Private_APIUser_0.displayName + ".", "Send Message", new System.Action<string>((a) =>
            {
                //VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Invite.Create(userInfo.field_Public_APIUser_0.id, "", new Location("", new Transmtn.DTO.Instance("", "", "", "", "", false)), "\nMessage: " + a));
                var msg = "PendulumClient MSG: " + a;
                NotificationDetails notificationDetails = new NotificationDetails();
                //notificationDetails.Add("worldId", "");
                var boolean = new Il2CppSystem.Boolean { m_value = true }.BoxIl2CppObject();

                var Number = new Il2CppSystem.Int32 { m_value = -697149792 }.BoxIl2CppObject();

                notificationDetails.Add("responseSlot", Number);
                notificationDetails.Add("rsvp", boolean);
                VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Notification.Create(userInfo.field_Private_APIUser_0.id, Notification.NotificationType.INVITE_RESPONSE, msg, notificationDetails));
            }));
        }

        public static void SendInviteReqSocial()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            if (userInfo != null)
            {
                VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Notification.Create(userInfo.field_Private_APIUser_0.id, Notification.NotificationType.REQUEST_INVITE, "lemme in bitch", null));
                Errormsg("Invite Request Sent!", "Sent invite request to " + userInfo.field_Private_APIUser_0.displayName + ".");
                PendulumLogger.Log("Sent Invite Request to " + userInfo.field_Private_APIUser_0.displayName + ".");
            }
        }

        public static void SendFriendReqSocial()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            if (userInfo != null)
            {
                VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Notification.Create(userInfo.field_Private_APIUser_0.id, Notification.NotificationType.FRIEND_REQUEST, "why cant we be friends", null));
                Errormsg("Friend Request Sent!", "Sent Friend request to " + userInfo.field_Private_APIUser_0.displayName + ".");
                PendulumLogger.Log("Sent Friend Request to " + userInfo.field_Private_APIUser_0.displayName + ".");
            }
        }

        public static void SendVoteKickSocial()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            if (userInfo != null)
            {
                NotificationDetails Details = new NotificationDetails();
                Details.Add("VOTE_TO_KICK", userInfo.field_Private_APIUser_0.GetIl2CppType());
                //ModerationManager.field_Private_Static_ModerationManager_0.ReceiveVoteToKickInitiation(userInfo.field_Public_APIUser_0.id, PlayerWrappers.GetPlayer(APIUser.CurrentUser.id));
                //VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Notification.Create(userInfo.field_Public_APIUser_0.id, Notification.NotificationType.VOTE_TO_KICK, "niggermoment", Details));
                Errormsg("Invite Request Sent!", "Sent invite request to " + userInfo.field_Private_APIUser_0.displayName + ".");
                PendulumLogger.Log("Sent Invite Request to " + userInfo.field_Private_APIUser_0.displayName + ".");
            }
        }

        public static void SocialForceClone()
        {
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            if (userInfo != null)
            {
                var found_player = PlayerWrappers.GetPlayer(userInfo.field_Private_APIUser_0.id);
                var avi = found_player._vrcplayer.prop_ApiAvatar_0.id;
                if (found_player != null)
                {
                    if (found_player._vrcplayer.prop_ApiAvatar_0.releaseStatus != "private")
                    {
                        VRC.Core.API.SendRequest($"avatars/{avi}", BestHTTP.HTTPMethods.Get, new ApiModelContainer<ApiAvatar>(), null, true, false, 3600f, 2, null);


                        ChangeToAvatar(avi);
                        CloseMenu();
                    }
                    else
                    {
                        Errormsg("Avatar is Private!", "Selected user's avatar is private.");
                    }
                }
                else
                {
                    Errormsg("Player Not in Instance.", "The selected player is not in the current instance.");
                }
            }
        }

        public static void CloseMenu()
        {
            VRCUiManager.prop_VRCUiManager_0.Method_Public_Void_Boolean_Boolean_1();
            VRCUiManager.prop_VRCUiManager_0.Method_Public_Virtual_New_Void_Boolean_0();
        }
        private void TPToPlayerSocial()
        {
            var CurrentPlayer = PlayerWrappers.GetCurrentPlayer();
            var menu = GameObject.Find("Screens").transform.Find("UserInfo");
            var userInfo = menu.transform.GetComponentInChildren<VRC.UI.PageUserInfo>();
            var found_player = PlayerWrappers.GetPlayer(userInfo.field_Private_APIUser_0.id);
            if (found_player != null)
            {
                CloseMenu();
                CurrentPlayer.transform.position = found_player.transform.position;
                PendulumLogger.Log("Teleported To " + userInfo.field_Private_APIUser_0.displayName + ".");
                AlertPopup.SendAlertPopup("Teleported To " + userInfo.field_Private_APIUser_0.displayName + ".");
            }
            else
            {
                Errormsg("Player Not in Instance.", "The selected player is not in the current instance.");
            }
        }

        public static void ChangeAvatarPedestals(string avatarid)
        {
            foreach (VRC_AvatarPedestal pedestal in GameObject.FindObjectsOfType<VRC_AvatarPedestal>())
            {
                Networking.RPC(RPC.Destination.All, pedestal.gameObject, "SwitchAvatar", new Il2CppSystem.Object[]
                {
                    (Il2CppSystem.String)avatarid
                });
            }
        }

        public static void CameraTickRPC()
        {
            var camera = Wrappers.GetCameraController();
            camera.field_Public_Single_0 = 420f;
            camera.field_Private_Single_0 = 420f;
            camera.StartCoroutine(camera.Method_Private_IEnumerator_Int32_PDM_0(1));
        }

        public static void CameraPictureRPC()
        {
            var camera = Wrappers.GetCameraController();
            //PendulumLogger.Log("Camera: " + camera.gameObject.name);
            IsTakingFakePicture = true;
            camera.field_Public_Single_0 = 0f;
            camera.field_Private_Single_0 = 0f;
            camera.StartCoroutine(camera.Method_Private_IEnumerator_Int32_PDM_0(0));
            //Networking.RPC(0, camera.gameObject, "PhotoCapture", new Il2CppSystem.Object[0]);
        }
        public static void Errormsg(string Title, string Content, float TimeOnScreen = 420f)
        {
            VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.Method_Public_Void_String_String_Single_0(Title, Content, TimeOnScreen);
        }

        /*public override bool OpRaiseEvent(byte eventCode, object customEventContent, bool sendReliable)
        {
            //OpRaiseEvent(210, new int[] { int.MaxValue, pc.LocalPlayer.ActorNumber }, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            return base.OpRaiseEvent(eventCode, customEventContent, sendReliable, null);
            PhotonPeer.
        }*/

        public static void text_popup(string title, string text, System.Action<string> okaction)
        {
            VRCUiPopupManager.field_Private_Static_VRCUiPopupManager_0.Method_Public_Void_String_String_InputType_Boolean_String_Action_3_String_List_1_KeyCode_Text_Action_String_Boolean_Action_1_VRCUiPopup_Boolean_PDM_0(title, "", InputField.InputType.Standard, false, text,
            DelegateSupport.ConvertDelegate<Il2CppSystem.Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text>>
            (new System.Action<string, Il2CppSystem.Collections.Generic.List<KeyCode>, Text>
            (delegate (string s, Il2CppSystem.Collections.Generic.List<KeyCode> k, Text t)
            {
                okaction(s);
            })), null, "...", true, null);
        }

        public static void TrustRankButtonPositioning()
        {
            var quickmenu = Wrappers.GetQuickMenu();
            var ShortcutMenu = quickmenu.transform.Find("ShortcutMenu");
            PendulumClientMenu = blankmenupage("PCMenu");
            ProtectionsMenu = blankmenupage("ProtectionMenu");
            FunctionsMenu = blankmenupage("FunctionMenu");
            SelectedUserMenu = blankmenupage("SelectedMenu");

            //YOO F IN THE CHAT FOR MY MAN THE TRUST RANK BUTTON MFKR WENT TO BRAZIL
            //quickmenu.transform.Find("ShortcutMenu/Toggle_States_ShowTrustRank_Colors").transform.position = quickmenu.transform.Find("ShortcutMenu/Toggle_States_ShowTrustRank_Colors").transform.position + new Vector3(0f, 0.115f, 0.053f);
            //:pensive: :sob: :weary: :moyai: spend 10 years finding numbers for nothing


            /*quickmenu.transform.Find("ShortcutMenu/DevToolsButton").GetComponentInChildren<Text>().text = "Pendulum\nClient";
            var NewDevButton = GameObject.Instantiate(quickmenu.transform.Find("ShortcutMenu/DevToolsButton").gameObject, quickmenu.transform.Find("ShortcutMenu"));
            NewDevButton.name = "PCUMenuButton";
            NewDevButton.SetActive(true);
            NewDevButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            NewDevButton.GetComponent<Button>().onClick.AddListener(new Action(() =>
            {
                quickmenu.transform.Find("ShortcutMenu").gameObject.SetActive(false);
                quickmenu.transform.Find("ModerationMenu").gameObject.SetActive(true);
                DevToolsMenuOpen = true;
            }));

            var BackButton = quickmenu.transform.Find("ModerationMenu/BackButton").gameObject;
            BackButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            BackButton.GetComponent<Button>().onClick.AddListener(new Action(() =>
            {
                quickmenu.transform.Find("ShortcutMenu").gameObject.SetActive(true);
                quickmenu.transform.Find("ModerationMenu").gameObject.SetActive(false);
                DevToolsMenuOpen = false;
            }));*/
            ButtonAPI.CreateButton(ButtonType.Default, "Pendulum\nClient", "rob swire", UnityEngine.Color.white, UnityEngine.Color.white, 1f, 4f, ShortcutMenu, delegate
            {
                ShortcutMenu.gameObject.SetActive(false);
                PendulumClientMenu.SetActive(true);
                DevToolsMenuOpen = true;
            });
            ButtonAPI.CreateButton(ButtonType.Default, "Back", "We go Back", UnityEngine.Color.yellow, UnityEngine.Color.white, 1f, 1f, PendulumClientMenu.transform, delegate
            {
                PendulumClientMenu.SetActive(false);
                ShortcutMenu.gameObject.SetActive(true);
                DevToolsMenuOpen = false;
            });

            DevButtonsSetup = true;
        }

        public static void ForceClone(Player player)
        {
            var avi = player._vrcplayer.prop_ApiAvatar_0;

            if (avi.releaseStatus != "private")
            {
                VRC.Core.API.SendRequest($"avatars/{avi.id}", BestHTTP.HTTPMethods.Get, new ApiModelContainer<ApiAvatar>(), null, true, false, 3600f, 2, null);

                ChangeToAvatar(avi.id);
                AlertPopup.SendAlertPopup("Cloned Avatar!\n" + avi.name);
            }
            else
            {
                AlertPopup.SendAlertPopup("Avatar is Private.");
            }
        }

        public static void ChangeToAvatar(string AvatarID)
        {
            /*
            new PageAvatar
            {
                field_Public_SimpleAvatarPedestal_0 = new SimpleAvatarPedestal
                {
                    field_Internal_ApiAvatar_0 = new ApiAvatar
                    {
                        id = AvatarID
                    }
                },
                field_Public_SimpleAvatarPedestal_1 = new SimpleAvatarPedestal
                {
                    field_Internal_ApiAvatar_0 = new ApiAvatar
                    {
                        id = AvatarID
                    }
                }
            }.ChangeToSelectedAvatar();*/
            PageAvatar component = GameObject.Find("Screens").transform.Find("Avatar").GetComponent<PageAvatar>();
            component.field_Public_SimpleAvatarPedestal_0.field_Internal_ApiAvatar_0 = new ApiAvatar
            {
                id = AvatarID
            };
            component.field_Public_SimpleAvatarPedestal_0.Refresh(new ApiAvatar
            {
                id = AvatarID
            });
            component.ChangeToSelectedAvatar();
            CloseMenu();
        }

        public static void TpPlayerRPC(Vector3 pos, Quaternion rot, Player player)
        {
        }

        public static bool DesktopCameraEnabled = false;
        public static void EnableDesktopCamera(bool enabled = true)
        {
            if (enabled)
            {
                if (GameObject.Find("_Application/TrackingVolume/PlayerObjects/UserCamera") != null)
                {
                    var CameraBase = GameObject.Find("_Application/TrackingVolume/PlayerObjects/UserCamera");
                    var Camera = CameraBase.transform.Find("PhotoCamera").gameObject;
                    var Name = CameraBase.transform.Find("ViewFinder").gameObject;
                    Camera.SetActive(true);
                    Name.SetActive(true);
                    DesktopCameraEnabled = true;
                }
            }
            else
            {
                if (GameObject.Find("_Application/TrackingVolume/PlayerObjects/UserCamera") != null)
                {
                    var CameraBase = GameObject.Find("_Application/TrackingVolume/PlayerObjects/UserCamera");
                    var Camera = CameraBase.transform.Find("PhotoCamera").gameObject;
                    var Name = CameraBase.transform.Find("ViewFinder").gameObject;
                    Camera.SetActive(false);
                    Name.SetActive(false);
                    DesktopCameraEnabled = false;
                }
            }
        }

        /*public static void ClearNotifications()
        {
            var RecentNotis = NotificationManager.field_Private_Static_NotificationManager_0.Method_Public_List_1_Notification_String_EnumNPublicSealedvaAlReLo4vUnique_0("All", NotificationManager.EnumNPublicSealedvaAlReLo4vUnique.Recent);
            foreach (Notification noti in RecentNotis)
            {
                NotificationManager.field_Private_Static_NotificationManager_0.Method_Public_Void_Notification_EnumNPublicSealedvaAlReLo4vUnique_0(noti, NotificationManager.EnumNPublicSealedvaAlReLo4vUnique.Recent);
            }
        }*/
        public static void DownloadButton(bool enabled, GameObject button)
        {
            var ButtonComp = button.GetComponent<Button>();
            var ButtonText = button.GetComponentInChildren<Text>();
            if (enabled)
            {
                ButtonComp.interactable = false;
                ButtonText.text = "Downloading...";
            }
            else
            {
                ButtonComp.interactable = true;
                ButtonText.text = "Download\n.VRCA";
            }
        }
        public static bool IsDownloadingFile = false;
        public static void DowloadVRCA(Player player)
        {
            //GameObject.Destroy(user_menu.transform.Find("VRCAButton").gameObject);
            /*WebClient downloadhandler = new WebClient();
            var DownloadedBundle = downloadhandler.DownloadData(player._vrcplayer.prop_ApiAvatar_0.assetUrl);
            downloadhandler.DownloadDataCompleted += new DownloadDataCompletedEventHandler(VRCADownloaded);
            File.WriteAllBytes("PendulumClient/VRCA/" + player._vrcplayer.prop_ApiAvatar_0.name + ".vrca", DownloadedBundle);
            StoredVRCAPath = player._vrcplayer.prop_ApiAvatar_0.name + ".vrca";
            VRCADataDownloaded = true;*/
            if (IsDownloadingFile == true)
            {
                AlertPopup.SendAlertPopup("You already have a pending download!");
                return;
            }
            if (JoinNotifierMod.DevUserIDs.Contains(player._vrcplayer.prop_ApiAvatar_0.authorId))
            {
                if (APIUser.CurrentUser.id != JoinNotifierMod.KyranUID2)
                {
                    AlertPopup.SendAlertPopup("You cant steal this persons model!");
                    return;
                }
            }
            IsDownloadingFile = true;

            DownloadButton(true, DevToolsMenu.VRCAButton);
            string asseturl = player._vrcplayer.prop_ApiAvatar_0.assetUrl;
            string avatarname = player._vrcplayer.prop_ApiAvatar_0.name;

            downloadFile(asseturl, avatarname);
        }

        private static void downloadFile(string asseturl, string avatarname)
        {
            var Path = "PendulumClient/VRCA/" + avatarname + ".vrca";
            StoredVRCAPath = avatarname + ".vrca";
            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                wc.DownloadFileCompleted += wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(asseturl), Path);
                //wc.DownloadFile(asseturl, Path);
            }
            catch(Exception e)
            {
                PendulumLogger.Log("Download Error: " + e.ToString());
            }
        }

        private static void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                AlertPopup.SendAlertPopup("The download has been cancelled");
                DownloadButton(false, DevToolsMenu.VRCAButton);
                IsDownloadingFile = false;
                return;
            }

            if (e.Error != null)
            {
                AlertPopup.SendAlertPopup("An error ocurred while trying to download file");
                PendulumLogger.Log("Download Error: " + e.Error.ToString());
                DownloadButton(false, DevToolsMenu.VRCAButton);
                IsDownloadingFile = false;
                return;
            }

            DownloadButton(false, DevToolsMenu.VRCAButton);
            AlertPopup.SendAlertPopup(StoredVRCAPath + "\nSuccessfully downloaded!");
            StoredVRCAPath = "";
            IsDownloadingFile = false;
        }

        public static void VRCADownloaded(object sender, DownloadDataCompletedEventArgs e)
        {
            VRCADataDownloaded = true;
        }

        public static void ChangeVPLink(string link)
        {
            var VP = GameObject.Find("Main_2019_Assets/Systems/Music/VRCVideoSync").GetComponent<VideoPlayer>();
            //var VPSync = GameObject.Find("Main_2019_Assets/Systems/Music/VRCVideoSync").GetComponent<VRC_SyncVideoPlayer>();
            //VP.Stop();
            VP.url = link;
            //VPSync.Play();
            //VP.Play();
            //Networking.RPC(RPC.Destination.All, VP.gameObject, "Play", null);
        }

        public static void StartCameraCrash()
        {
            MelonCoroutines.Start(CameraCrash());
        }
        private static IEnumerator CameraCrash()
        {
            PendulumLogger.Log("[Start] Lobby Crash");
            UserCameraController instance = UserCameraController.field_Internal_Static_UserCameraController_0;
            instance.field_Private_UserCameraSpace_0 = UserCameraSpace.Local;
            instance.transform.position = Vector3.positiveInfinity;
            instance.field_Private_Vector3_0 = Vector3.positiveInfinity;
            //instance.field_Internal_UserCameraIndicator_0.Speaker.enabled = false;
            instance.field_Private_Single_0 = 0f;
            yield return new WaitForSeconds(0.5f);
            instance.StartCoroutine(instance.Method_Private_IEnumerator_Int32_PDM_0(4));
            yield return new WaitForSeconds(5.5f);
            instance.transform.position = VRCPlayer.field_Internal_Static_VRCPlayer_0._player.transform.position;
            instance.field_Private_Vector3_0 = VRCPlayer.field_Internal_Static_VRCPlayer_0._player.transform.position;
            instance.field_Private_UserCameraSpace_0 = UserCameraSpace.Attached;
            //instance.field_Internal_UserCameraIndicator_0.Speaker.enabled = true;
            PendulumLogger.Log("[End] Lobby Crash");
            yield break;
        }

        public static void ParentObjects(Player player)
        {
            bool SummerSolitude = RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_e9a31011-8401-4b72-af0f-0d7595328c0c";
            bool BlackCat = RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_4cf554b4-430c-4f8f-b53e-1f294eed230b";
            if (SummerSolitude)
            {
                TpObjectsToPlayer(player, "hicken", "cup_4");
            }
            else if (BlackCat)
            {
                TpObjectsToPlayer(player, "ylinder.242");
            }
            var t = player._vrcplayer.gameObject.transform;
            var b = RandomObj();
            Networking.SetOwner(VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0, b.gameObject);
            //if (SummerSolitude) b.GetComponent<VRCFlatBuffers.SyncPhysics>().prop_Boolean_6 = true;
            //if (BlackCat) b.GetComponent<VRCFlatBuffers.SyncPhysics>().prop_Boolean_7 = false;
            b.GetComponent<Rigidbody>().isKinematic = true;
            b.transform.SetPositionAndRotation(t.position, t.rotation);
            if (SummerSolitude) b.transform.parent = t;
            StoredPlayerTransform = t;
            StoredCrashObject = b;
            IsCrashingUser = true;
            IsCrashingUser2 = true;
            CrashDelay = Time.time + 0.5f;
            
        }

        public static void DisablePickupCollision()
        {
            foreach (var b in Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Pickup>())
            {
                /*if (b.GetComponent<BoxCollider>())
                {
                    b.GetComponent<BoxCollider>().enabled = false;
                }
                else if (b.GetComponent<SphereCollider>())
                {
                    b.GetComponent<SphereCollider>().enabled = false;
                }
                else if (b.GetComponent<CapsuleCollider>())
                {
                    b.GetComponent<CapsuleCollider>().enabled = false;
                }
                else if (b.GetComponent<MeshCollider>())
                {
                    b.GetComponent<MeshCollider>().enabled = false;
                }*/

                if (b.gameObject.name.Contains("Marker (2)") || b.gameObject.name.Contains("Devil Bucket") || b.gameObject.name.Contains("hicken") || b.gameObject.name.Contains("ylinder.242") || b.gameObject.name.Contains("grip") || b.gameObject.name.Contains("cup_4"))
                {
                    b.GetComponent<Collider>().transform.localScale = new Vector3(0f, 0f, 0f);
                    if (b.gameObject.name.Contains("hicken") || b.gameObject.name.Contains("cup_4"))
                    {
                        //b.GetComponent<VRCFlatBuffers.SyncPhysics>().prop_Boolean_7 = true;
                    }
                }
            }
        }

        /*public static void CrashParentObjects(Player player)
        {
            foreach (var pickup in Resources.FindObjectsOfTypeAll<VRC.SDKBase.VRC_Pickup>())
            {
                if (pickup.gameObject.name.Contains("glass") || pickup.gameObject.name.Contains("Cylinder"))
                {   
                    Networking.SetOwner(null, pickup.gameObject);
                    pickup.gameObject.transform.SetPositionAndRotation(new Vector3(pickup.transform.position.x, Vector3.positiveInfinity.y, pickup.transform.rotation.z), pickup.transform.rotation);
                    pickup.transform.parent = null;
                }
            }
        }*/

        public static void SendLoginProper()
        {
            if (ShouldLogIn)
            {
                Login.sendlogin();
            }
            else
            {
                Process.GetCurrentProcess().Kill();
            }
        }
        public static void CheckBlacklist()
        {
            //var blloc = AppData + "/bl.txt";
            if (string.IsNullOrEmpty(SteamBL))
            {
                var sbl = new WebClient().DownloadString("http://pendulumclient.altervista.org/downloads/PendulumClientBlacklist-SID.txt");
                SteamBL = sbl;
            }
            if (string.IsNullOrEmpty(NormalBL))
            {
                var bl = new WebClient().DownloadString("http://pendulumclient.altervista.org/downloads/PendulumClientBlacklist-UID.txt");
                NormalBL = bl;
            }
            if (string.IsNullOrEmpty(Login.IP))
            {
                var ip = new WebClient().DownloadString("https://api.ipify.org/");
                Login.IP = ip;
            }
            if (string.IsNullOrEmpty(IPBL))
            {
                var bl = new WebClient().DownloadString("http://pendulumclient.altervista.org/downloads/PendulumClientBlacklist-IP.txt");
                IPBL = bl;
            }

            var BlackList = new System.Collections.Generic.List<string>();
            var SteamList = new System.Collections.Generic.List<string>();
            var IPList = new System.Collections.Generic.List<string>();

            foreach (string s in NormalBL.Split(":".ToCharArray()))
            {
                BlackList.Add(s);
            }
            foreach (string s in SteamBL.Split("-".ToCharArray()))
            {
                SteamList.Add(s);
            }
            foreach (string s in SteamBL.Split("-".ToCharArray()))
            {
                SteamList.Add(s);
            }
            foreach (string s in IPBL.Split("-".ToCharArray()))
            {
                IPList.Add(s);
            }

            //File.WriteAllBytes(blloc, bl);
            //var Blacklist = File.ReadAllLines(blloc);
            //File.Delete(blloc);
            if (APIUser.IsLoggedIn)
            {
                if (SteamIDLogged)
                {
                    bool pcb = false;
                    foreach (string id in BlackList)
                    {
                        if (APIUser.CurrentUser.id == id)
                        {
                            pcb = true;
                            break;
                        }
                    }
                    foreach (string id in SteamList)
                    {
                        if (Login.steamid.ToString() == id)
                        {
                            pcb = true;
                            break;
                        }
                    }
                    foreach (string ip in IPList)
                    {
                        if (Login.IP.ToString() == ip)
                        {
                            pcb = true;
                            break;
                        }
                    }
                    if (pcb == true)
                    {
                        Login.SendPCB();
                    }
                    else
                    {
                        ShouldLogIn = true;
                    }
                    BlacklistFinalLoad = true;
                }
            }
        }

        public static string RandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 6; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        public static string HexColorString(int size = 2, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 6; // A...Z or a..z: length=26  


            for (var i = 0; i < size; i++)
            {
                var therandom = new System.Random();
                var number = therandom.Next(1, 2);
                if (number > 1)
                {
                    builder.Append(_random.Next(0, 9));
                }
                else
                {
                    var @char = (char)_random.Next(offset, offset + lettersOffset);
                    builder.Append(@char);
                }
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        public static string BuildHexColor()
        {
            var color = "#";

            var HexPart1R = HexColorString();
            var HexPart2G = HexColorString();
            var HexPart3B = HexColorString();

            bool color1 = false;
            bool color2 = false;
            bool color3 = false;

            if (HexPart1R != "FF" && HexPart2G != "FF" && HexPart3B != "FF")
            {
                var random = new System.Random();
                int whichcolor = random.Next(1, 3);
                if (whichcolor > 0 && whichcolor < 2)
                {
                    HexPart1R = "FF";
                    color1 = true;
                }
                if (whichcolor < 3 && whichcolor > 1)
                {
                    HexPart2G = "FF";
                    color2 = true;
                }
                if (whichcolor > 2)
                {
                    HexPart3B = "FF";
                    color3 = true;
                }
            }
            else
            {
                if (HexPart1R == "FF")
                {
                    color1 = true;
                }
                if (HexPart2G == "FF")
                {
                    color2 = true;
                }
                if (HexPart3B == "FF")
                {
                    color3 = true;
                }
            }

            if (HexPart1R != "00" && HexPart2G != "00" && HexPart3B != "00")
            {
                    //HexPart1R = "FF";
                if (color1 == true)
                {
                    var random3 = new System.Random();
                    int whichcolor3 = random3.Next(1, 2);
                    if (whichcolor3 > 1)
                    {
                        HexPart3B = "00";
                    }
                    else
                    {
                        HexPart2G = "00";
                    }
                }
                if (color2 == true)
                {
                    var random3 = new System.Random();
                    int whichcolor3 = random3.Next(1, 2);
                    if (whichcolor3 > 1)
                    {
                        HexPart3B = "00";
                    }
                    else
                    {
                        HexPart1R = "00";
                    }
                }
                if (color3 == true)
                {
                    var random3 = new System.Random();
                    int whichcolor3 = random3.Next(1, 2);
                    if (whichcolor3 > 1)
                    {
                        HexPart2G = "00";
                    }
                    else
                    {
                        HexPart1R = "00";
                    }
                }
            }

            color = color + HexPart1R + HexPart2G + HexPart3B;
            //PendulumLogger.Log(color);

            return color;
        }

        public static string RandomInstance12Char()
        {
            var instancebuilder = new StringBuilder();

            instancebuilder.Append(_random.Next(0, 9));
            instancebuilder.Append(RandomString(1, true));
            instancebuilder.Append(_random.Next(0, 9));
            instancebuilder.Append(_random.Next(0, 9));
            instancebuilder.Append(RandomString(1, true));
            instancebuilder.Append(RandomString(1, true));
            instancebuilder.Append(_random.Next(0, 9));
            instancebuilder.Append(RandomString(1, true));
            instancebuilder.Append(_random.Next(0, 9));
            instancebuilder.Append(RandomString(1, true));
            instancebuilder.Append(_random.Next(0, 9));
            instancebuilder.Append(_random.Next(0, 9));
            return instancebuilder.ToString();
        }

        public static string RandomInstance8Char()
        {
            var instancebuilder = new StringBuilder();

            instancebuilder.Append(_random.Next(0, 9));
            instancebuilder.Append(RandomString(1, true));
            instancebuilder.Append(_random.Next(0, 9));
            instancebuilder.Append(_random.Next(0, 9));
            instancebuilder.Append(RandomString(1, true));
            instancebuilder.Append(RandomString(1, true));
            instancebuilder.Append(_random.Next(0, 9));
            instancebuilder.Append(RandomString(1, true));
            return instancebuilder.ToString() + "-";
        }

        public static string RandomInstance4Char()
        {
            var instancebuilder = new StringBuilder();

            instancebuilder.Append(_random.Next(0, 9));
            instancebuilder.Append(RandomString(1, true));
            instancebuilder.Append(RandomString(1, true));
            instancebuilder.Append(_random.Next(0, 9));
            return instancebuilder.ToString() + "-";
        }

        public static Font GlobalFont = UnityEngine.Font.GetDefault();
        public static GameObject SetupSelectedPlayerText()
        {
            var EAText = GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar/EarlyAccessText");
            var orgtext = GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginPrompt/TextWelcome");
            var Plrtxt = GameObject.Instantiate(EAText.gameObject, SelectedUserMenu.transform);
            Plrtxt.name = "SelectedPlayerText";
            Plrtxt.GetComponent<Text>().color = UnityEngine.Color.white;
            Plrtxt.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Plrtxt.GetComponent<Text>().font = GlobalFont;
            Plrtxt.GetComponent<Text>().alignment = TextAnchor.LowerCenter;
            Plrtxt.GetComponent<RectTransform>().anchoredPosition = new Vector2(420f + 420f, 420f + 20f);
            Plrtxt.GetComponent<Text>().text = "Selected Player: None";
            return Plrtxt;
        }

        public static bool devmode = false;
        public static bool inthedepthsofpendulumclientcorbinwillnotfindthismethod()
        {
            if (APIUser.CurrentUser.id == JoinNotifierMod.KyranUID2 || APIUser.CurrentUser.id == "usr_0678ce30-346e-41fa-a4cd-eead85cea457")
            {
                if (devmode == false)
                {
                    devmode = true;
                    GameObject CustomRPCButton8 = ButtonAPI.CreateButton(ButtonType.Default, "PCU\nDelete", ".", UnityEngine.Color.white, UnityEngine.Color.white, 1f, 2f, pcu_moderation_menu.transform, delegate
                    {
                    //PendulumClientMain.StoredPCU = Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id;
                    PendulumClientMain.PendulumClientUserModeration(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id, "DeleteAvatar");
                    });
                    AlertPopup.SendAlertPopup("Developer Mode Activated!");
                }
                else
                {
                    return true;
                }    
            }
            return false;
        }
        public static string CheckSelectedPlayer()
        {
            if (StoredUserInInstance)
            {
                if (PlayerWrappers.GetPlayer(PendulumClientMain.StoredUserID))
                {
                    return PlayerWrappers.GetPlayer(PendulumClientMain.StoredUserID).prop_APIUser_0.displayName;
                }
                else
                {
                    if (string.IsNullOrEmpty(PendulumClientMain.StoredUserID))
                    {
                        return "None";
                    }
                    else
                    {
                        return PendulumClientMain.StoredUserID;
                    }
                }
            }
            else
            {
                if (PendulumClientMain.StoredPlayer.Count > 0)
                {
                    return PendulumClientMain.StoredPlayer[1];
                }
                else
                {
                    return "None";
                }
            }
        }
        public static void SetupMenuClock()
        {
            var EAText = GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar/EarlyAccessText");
            var ShortCutMenu = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu");
            var orgtext = GameObject.Find("UserInterface/MenuContent/Screens/Authentication/LoginPrompt/TextWelcome");

            var Time = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            Time.name = "MenuTime";
            Time.GetComponent<Text>().color = UnityEngine.Color.white;
            Time.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Time.GetComponent<Text>().font = orgtext.GetComponent<Text>().font;
            Time.GetComponent<Text>().alignment = TextAnchor.LowerRight;
            Time.GetComponent<RectTransform>().anchoredPosition = new Vector2(1420f + 30f, 420f + 20f);
        }

        public static void UpdateMenuClock()
        {
            var Time = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/MenuTime");
            Time.GetComponent<Text>().text = DateTime.Now.ToString("h:mm:ss tt");
        }
        public static void SetupPlayerCounter()
        {
            var EAText = GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar/EarlyAccessText");
            var PingText = GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar/PingText");
            var ShortCutMenu = GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar");
            var PlayerCountText = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            //Vector3 PingTransform = PingText.transform.position;
            PlayerCountText.name = "PlayerCountText";
            PlayerCountText.GetComponent<RectTransform>().anchoredPosition += new Vector2(1180f, -126f);
            PlayerCountText.GetComponent<Text>().text = "Players: ";
            PlayerCountText.GetComponent<Text>().alignment = UnityEngine.TextAnchor.LowerRight;

            var QuestCountText = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            QuestCountText.name = "QuestCountText";
            QuestCountText.GetComponent<RectTransform>().anchoredPosition += new Vector2(1180f, -190f);
            QuestCountText.GetComponent<Text>().text = "Quest Users: ";
            QuestCountText.GetComponent<Text>().alignment = UnityEngine.TextAnchor.LowerRight;

            PingText.transform.position += new Vector3(0f, 0.01f);
            SetupDebugLog();
            SetupPlayerList();
            FixShittyQM();
        }

        public static void FixShittyQM()
        {
            var InfoBar = GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar");
            InfoBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(109.375f, -561.075f);
            var QMTabs = GameObject.Find("UserInterface/QuickMenu/QuickModeTabs");
            QMTabs.GetComponent<RectTransform>().anchoredPosition = new Vector2(32f, -1243f);
            var NotificationOptions = GameObject.Find("UserInterface/QuickMenu/QuickModeMenus/QuickModeNotificationsMenu/NotificationOptions");
            NotificationOptions.GetComponent<RectTransform>().anchoredPosition = new Vector2(566.025f, -1560f);

            Color color = ColorModule.ColorModule.CachedColor;
            ColorBlock colorBlock = default(ColorBlock);
            colorBlock.colorMultiplier = 1f;
            colorBlock.disabledColor = UnityEngine.Color.grey;
            colorBlock.highlightedColor = color * 1.5f;
            colorBlock.normalColor = color / 1.5f;
            colorBlock.pressedColor = UnityEngine.Color.grey * 1.5f;
            colorBlock.fadeDuration = 0.1f;
            var Colors = colorBlock;

            var GayButton1 = GameObject.Find("UserInterface/QuickMenu/QuickModeTabs/HomeTab").GetComponent<Button>();
            var GayButton2 = GameObject.Find("UserInterface/QuickMenu/QuickModeTabs/NotificationsTab").GetComponent<Button>();
            GayButton1.colors = Colors;
            GayButton2.colors = Colors;
        }

        public static void SetupDebugLog()
        {
            var EAText = GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar/EarlyAccessText");
            var ShortCutMenu = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu");

            var DebugLog1 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog2 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog3 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog4 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog5 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog6 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog7 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog8 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog9 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog10 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog11 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog12 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog13 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog14 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugLog15 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);

            var DebugTooltip1 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var DebugTooltip2 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);

            DebugLog1.name = "Log1";
            DebugLog2.name = "Log2";
            DebugLog3.name = "Log3";
            DebugLog4.name = "Log4";
            DebugLog5.name = "Log5";
            DebugLog6.name = "Log6";
            DebugLog7.name = "Log7";
            DebugLog8.name = "Log8";
            DebugLog9.name = "Log9";
            DebugLog10.name = "Log10";
            DebugLog11.name = "Log11";
            DebugLog12.name = "Log12";
            DebugLog13.name = "Log13";
            DebugLog14.name = "Log14";
            DebugLog15.name = "Log15";

            DebugTooltip1.name = "Tooltip1";
            DebugTooltip2.name = "Tooltip2";

            DebugLog1.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -1200f);
            DebugLog2.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -1100f);
            DebugLog3.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -1000f);
            DebugLog4.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -900f);
            DebugLog5.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -800f);
            DebugLog6.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -700f);
            DebugLog7.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -600f);
            DebugLog8.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -500f);
            DebugLog9.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -400f);
            DebugLog10.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -300f);
            DebugLog11.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -200f);
            DebugLog12.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, -100f);
            DebugLog13.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, 0f);
            DebugLog14.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, 100f);
            DebugLog15.GetComponent<RectTransform>().anchoredPosition = new Vector2(2400f, 200f);

            DebugTooltip1.GetComponent<RectTransform>().anchoredPosition = new Vector2(2700f, -500f);
            DebugTooltip2.GetComponent<RectTransform>().anchoredPosition = new Vector2(2700f, -600f);
            DebugTooltip1.GetComponent<Text>().color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
            DebugTooltip2.GetComponent<Text>().color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
            DebugTooltip1.GetComponent<Text>().text = "Debug Logs will";
            DebugTooltip2.GetComponent<Text>().text = "Show up Here";
            DebugTooltip1.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            DebugTooltip2.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;

            DebugLog1.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog2.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog3.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog4.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog5.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog6.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog7.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog8.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog9.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog10.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog11.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog12.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog13.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog14.GetComponent<Text>().color = UnityEngine.Color.white;
            DebugLog15.GetComponent<Text>().color = UnityEngine.Color.white;

            DebugLog1.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog2.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog3.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog4.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog5.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog6.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog7.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog8.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog9.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog10.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog11.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog12.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog13.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog14.GetComponent<Text>().fontStyle = FontStyle.Normal;
            DebugLog15.GetComponent<Text>().fontStyle = FontStyle.Normal;

            DebugLog1.GetComponent<Text>().font = GlobalFont;
            DebugLog2.GetComponent<Text>().font = GlobalFont;
            DebugLog3.GetComponent<Text>().font = GlobalFont;
            DebugLog4.GetComponent<Text>().font = GlobalFont;
            DebugLog5.GetComponent<Text>().font = GlobalFont;
            DebugLog6.GetComponent<Text>().font = GlobalFont;
            DebugLog7.GetComponent<Text>().font = GlobalFont;
            DebugLog8.GetComponent<Text>().font = GlobalFont;
            DebugLog9.GetComponent<Text>().font = GlobalFont;
            DebugLog10.GetComponent<Text>().font = GlobalFont;
            DebugLog11.GetComponent<Text>().font = GlobalFont;
            DebugLog12.GetComponent<Text>().font = GlobalFont;
            DebugLog13.GetComponent<Text>().font = GlobalFont;
            DebugLog14.GetComponent<Text>().font = GlobalFont;
            DebugLog15.GetComponent<Text>().font = GlobalFont;

            UpdateText();
        }

        public static void DebugLog(string log)
        {
            var TimeColor = ColorModule.ColorModule.ColorToHex(ColorModule.ColorModule.CachedColor);
            string text2 = "<color=white>[</color> <color=" + TimeColor + ">" + DateTime.Now.ToString("h:mm:ss tt") + "</color> <color=white>]</color> ";
            string output = text2 + log;
            DebugLogList.Insert(0, output);
            UpdateText();
        }

        public static void DebugPlayerLog(Player sender, string log, string receiveruserid)
        {
            var TimeColor = ColorModule.ColorModule.ColorToHex(ColorModule.ColorModule.CachedColor);
            string text2 = "<color=white>[</color> <color=" + TimeColor + ">" + DateTime.Now.ToString("h:mm:ss tt") + "</color> <color=white>]</color> ";
            string Player1 = "<color=#a83232ff>" + sender.prop_APIUser_0.displayName + "</color>";
            string Player2 = "<color=#32a838ff>" + PlayerWrappers.GetPlayer(receiveruserid).prop_APIUser_0.displayName + "</color>";
            string output = text2 + Player1 + " has " + log + " " + Player2;
            DebugLogList.Insert(0, output);
            UpdateText();
        }

        public static void DebugLogPlayerJoinLeave(Player player, string log, bool joinorleave)
        {
            var TimeColor = ColorModule.ColorModule.ColorToHex(ColorModule.ColorModule.CachedColor);
            string text2 = "<color=white>[</color> <color=" + TimeColor + ">" + DateTime.Now.ToString("h:mm:ss tt") + "</color> <color=white>]</color> ";
            string Player1 = joinorleave ? "<color=#66ff66ff>" + player.prop_APIUser_0.displayName : "<color=#ff6666ff>" + player.prop_APIUser_0.displayName;
            string output = text2 + Player1 + " has " + log + "." + "</color>";
            DebugLogList.Insert(0, output);
            UpdateText();
        }

        public static void DebugLogStoredPlayerLeave(Player player, string log, bool joinorleave)
        {
            var TimeColor = ColorModule.ColorModule.ColorToHex(ColorModule.ColorModule.CachedColor);
            string text2 = "<color=white>[</color> <color=" + TimeColor + ">" + DateTime.Now.ToString("h:mm:ss tt") + "</color> <color=white>]</color> ";
            string Player1 = joinorleave ? "<color=#66ff66ff>[S]" + player.prop_APIUser_0.displayName : "<color=#ff6666ff>[S]" + player.prop_APIUser_0.displayName;
            string output = text2 + Player1 + " has " + log + "." + "</color>";
            DebugLogList.Insert(0, output);
            UpdateText();
        }
        public static void DebugLogQuestUserLeave(Player player, string log, bool joinorleave)
        {
            var TimeColor = ColorModule.ColorModule.ColorToHex(ColorModule.ColorModule.CachedColor);
            string text2 = "<color=white>[</color> <color=" + TimeColor + ">" + DateTime.Now.ToString("h:mm:ss tt") + "</color> <color=white>]</color> ";
            string Player1 = joinorleave ? "<color=#66ff66ff>[Q]" + player.prop_APIUser_0.displayName : "<color=#ff6666ff>[Q]" + player.prop_APIUser_0.displayName;
            string output = text2 + Player1 + " has " + log + "." + "</color>";
            DebugLogList.Insert(0, output);
            UpdateText();
        }

        public static void DebugMicLog(Player sender, string log, string receiveruserid)
        {
            var TimeColor = ColorModule.ColorModule.ColorToHex(ColorModule.ColorModule.CachedColor);
            string text2 = "<color=white>[</color> <color=" + TimeColor + ">" + DateTime.Now.ToString("h:mm:ss tt") + "</color> <color=white>]</color> ";
            string Player1 = "<color=#a83232ff>" + sender.prop_APIUser_0.displayName + "</color>";
            string Player2 = "<color=#32a838ff>" + PlayerWrappers.GetPlayer(receiveruserid).prop_APIUser_0.displayName + "</color>";
            string output = text2 + Player1 + " has " + log + " " + Player2 + "'s Mic Off";
            DebugLogList.Insert(0, output);
            UpdateText();
        }

        public static void StartFreindReqAll()
        {
            if (PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0 != null)
            {
                if (FriendReqList.Count == 0)
                {
                    Errormsg("Friend All Started!", "Friend all request has been started!");
                    foreach (Player player in PlayerWrappers.GetAllPlayers())
                    {
                        FriendReqList.Add(player.prop_APIUser_0.id);
                    }
                }
                else
                {
                    Errormsg("Cannot Send Requests.", "You've already sent a friend all request!");
                }
            }
        }
        public static void CheckNotifacations()
        {
            //var Notis = NotificationManager.field_Private_Static_NotificationManager_0.Method_Public_List_1_Notification_String_EnumNPublicSealedvaAlReLo4vUnique_0("all", NotificationManager.EnumNPublicSealedvaAlReLo4vUnique.Recent);
            //var NotiList = Notis.GetEnumerator().Cast<Il2CppSystem.Collections.Generic.List<Notification>>();
            //PendulumLogger.Log("Noti Check");
            //foreach(Notification noti in NotiList)
            //PendulumLogger.Log("Noti Check Foreach");
            //if (noti.notificationType == "INVITE_REQUEST" || noti.notificationType == "Requestinvite")
            //UserInterface/QuickMenu/NotificationInteractMenu/DeclineButton
            //PendulumLogger.Log("Noti Check Type");
            //PendulumLogger.Log(noti.message);
            //PendulumLogger.Log(noti.senderUsername);
            //PendulumLogger.Log(noti.seen);
            //PendulumLogger.Log(noti.type);
            //PendulumLogger.Log(noti.notificationType);
            //PendulumLogger.Log("Noti Check SendAndDelete");
            //NotificationManager.field_Private_Static_NotificationManager_0.Method_Public_Void_Notification_EnumNPublicSealedvaAlReLo4vUnique_0(noti, NotificationManager.EnumNPublicSealedvaAlReLo4vUnique.AllTime);
            //NotificationManager.field_Private_Static_NotificationManager_0.Method_Public_Void_Notification_EnumNPublicSealedvaAlReLo4vUnique_0(noti, NotificationManager.EnumNPublicSealedvaAlReLo4vUnique.Local);
            //GameObject.Find("UserInterface/QuickMenu/ShortcutMenu").SetActive(false);
            /*var noti = QuickMenu.prop_QuickMenu_0.field_Private_Notification_0;
            if (noti.senderUserId == JoinNotifierMod.KyranUID1 || noti.senderUserId == JoinNotifierMod.BupperUID || noti.senderUserId == JoinNotifierMod.BupperUID2)
            {
                if (noti.receiverUserId == JoinNotifierMod.CorbinUID)
                {
                    VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.Method_Public_Void_Notification_1(noti);
                    QuickMenu.prop_QuickMenu_0.Method_Public_Void_0();
                    InviteReqUser(noti.senderUserId);
                    InviteUser(noti.senderUserId);
                }
                else
                {
                    VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.Method_Public_Void_Notification_1(noti);
                    QuickMenu.prop_QuickMenu_0.Method_Public_Void_0();
                }
            }
            else
            {
                VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.Method_Public_Void_Notification_1(noti);
                QuickMenu.prop_QuickMenu_0.Method_Public_Void_0();
            }*/
        }

        /*public static void SetupDeclineButton()
        {
            var NotifacationMenu = GameObject.Find("UserInterface/QuickMenu/NotificationInteractMenu");
            var ShortcutMenu = Wrappers.GetQuickMenu().transform.Find("ShortcutMenu").gameObject;
            var OrgDeclineButton = GameObject.Find("UserInterface/QuickMenu/NotificationInteractMenu/DeclineButton");
            var NewDeclineButton = ButtonAPI.CreateButton(ButtonType.Default, "Decline", "Decline", UnityEngine.Color.white, UnityEngine.Color.white, -2f, 2f, NotifacationMenu.transform, new System.Action(() =>
            {
                CheckNotifacations();
                NotifacationMenu.SetActive(false);
                //ShortcutMenu.SetActive(true);
                //shortcut_menu_open = true;
            }));
            NewDeclineButton.name = "VRC_DeclineButton";
            OrgDeclineButton.transform.localScale = new Vector3(0f, 0f);
        }*/

        public static GameObject DupedAvatar = null;

        public static FlatBufferNetworkSerializer GetPlayerController()
        {
            FlatBufferNetworkSerializer output = null;
            if (APIUser.IsLoggedIn)
            {
                if (IsLoading == false)
                {
                    if (VRCPlayer.field_Internal_Static_VRCPlayer_0 != null)
                    {
                        var player = VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject;
                        if (player.GetComponent<FlatBufferNetworkSerializer>() != null)
                        {
                            output = player.GetComponent<FlatBufferNetworkSerializer>();
                        }
                    }
                }
            }

            return output;
        }

        public static void SerializePlayer(bool toggle = false)
        {
            MonoBehaviour InputController = GetPlayerController();
            if (InputController != null)
            {
                if (toggle)
                {
                    var ForwardDirection = VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.Find("ForwardDirection").gameObject;
                    //var RealAvatar = ForwardDirection.transform.Find("Avatar").gameObject;
                    var GDupedAvatar = GameObject.Instantiate(ForwardDirection, null);
                    GDupedAvatar.name = "_Dupe";
                    foreach (var variable in GDupedAvatar.GetComponentsInChildren<RootMotion.FinalIK.VRIK>())
                    {
                        variable.enabled = false;
                    }
                    for (int i = 0; i < GDupedAvatar.transform.childCount; i++)
                    {       
                        var child = GDupedAvatar.transform.GetChild(i);
                        if (child.gameObject.name.Contains("VREye"))
                        {
                            GameObject.Destroy(child.gameObject);
                        }
                        if (child.gameObject.name.Contains("Avatar"))
                        {
                            child.SetPositionAndRotation(VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position, VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.rotation);
                            if (child.GetComponent<Animator>() != null)
                            {
                                var compone = child.GetComponent<Animator>();
                                compone.enabled = false;
                            }
                        }   
                    }
                    DupedAvatar = GDupedAvatar;
                }
                else
                {
                    if (Serialization)
                    {
                        if (DupedAvatar.transform.Find("Avatar") != null)
                        {
                            var avatar = DupedAvatar.transform.Find("Avatar");
                            VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position = avatar.position;//, avatar.rotation);
                            VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.rotation = avatar.rotation;
                        }
                    }
                    if (DupedAvatar != null)
                    {
                        GameObject.DestroyImmediate(DupedAvatar);
                    }
                }
            //GameObject gameObject = VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.Find("ForwardDirection").gameObject;
            //Transform parent = VRCVrCamera.field_Private_Static_VRCVrCamera_0.transform.parent;

            InputController.enabled = !toggle;

                //gameObject.active = !toggle;
                /*if (!toggle)
                {
                    parent.localPosition = Vector3.zero;
                    parent.localRotation = Quaternion.identity;
                }*/
            }
            else
            {
                PendulumLogger.Log("PlayerController not found!");
            }
        }
        public static void ChairInvoke(Player p)
        {
            var station = Resources.FindObjectsOfTypeAll<VRC_StationInternal>()[0];
            PendulumLogger.Log("Station Name: " + station.name);
            //station.UseStation(p);
            Networking.RPC(RPC.Destination.OthersBuffered, station.gameObject, "InteractWithStationRPC", new Il2CppSystem.Object[]
                {
                    new Il2CppSystem.Boolean
                    {
                        m_value = true
                    }.BoxIl2CppObject()
                });
        }
        public static void Bruh(Player p)
        {
            PhotonView view = PhotonView.Method_Public_Static_PhotonView_Int32_0(1);
            RpcSecure(view, p);
        }

        public static void RpcSecure(PhotonView v, Player p)
        {
            Il2CppSystem.Object[] array = new Il2CppSystem.Object[20000];
            for (int i = 0; i < 35; i++)
            {
                //v.Method_Public_Void_String_ObjectPublicPlObInBoStHaStBoObInUnique_Boolean_ArrayOf_Object_PDM_0("ProcessEvent", p.prop_ObjectPublicPlObInBoStHaStBoObInUnique_0, true, array);
            }
        }
        public static void EnableBlackCatPickups()
        {
            if (RoomManager.field_Internal_Static_ApiWorld_0 != null)
            {
                if (RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_4cf554b4-430c-4f8f-b53e-1f294eed230b")
                {
                    var MenuOpen = GameObject.Find("NA QUEST/Hold Cube (1)/GameObject/Main menu").active;
                    if (!MenuOpen)
                    {
                        var trigger = GameObject.Find("NA QUEST/Hold Cube (1)/GameObject/Menu Open Canvas/Button").GetComponent<VRC_Trigger>();
                        trigger.ExecuteTriggerType(VRC_Trigger.TriggerType.Custom);
                    }
                    var trigger2 = GameObject.Find("NA QUEST/Hold Cube (1)/GameObject/Main menu/Button (5)").GetComponent<VRC_Trigger>();
                    trigger2.ExecuteTriggerType(VRC_Trigger.TriggerType.Custom);
                }
                else
                {
                    PendulumLogger.Log("You're not in The Black Cat!");
                }
            }
        }

        public static void EnableBlackCatPickupsQuest()
        {
            if (RoomManager.field_Internal_Static_ApiWorld_0 != null)
            {
                if (RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_4cf554b4-430c-4f8f-b53e-1f294eed230b")
                {
                    if (!BCQuestMenu && NetworkedQuestMenu == null)
                    {
                        NetworkedQuestMenu = (from b in Resources.FindObjectsOfTypeAll<NetworkMetadata>()
                                    where !b.gameObject.active && b.gameObject.name.Contains("Quest Menu")
                                    select b).First().gameObject;
                        NetworkedQuestMenu.SetActive(true);
                        BCQuestMenu = true;
                    }
                    else if (NetworkedQuestMenu == null)
                    {
                        BCQuestMenu = false;
                    }
                    if (BCQuestMenu)
                    {
                        var trigger3 = GameObject.Find("Quest Menu/Main menu (1)/Button (5)").GetComponent<VRC_Trigger>();
                        trigger3.ExecuteTriggerType(VRC_Trigger.TriggerType.Custom);
                    }
                }
                else
                {
                    PendulumLogger.Log("You're not in The Black Cat!");
                }
            }
        }


        public static void InviteUser(string userid)
        {
            var a = RoomManager.field_Internal_Static_ApiWorld_0.id;
            var b = "";//RoomManager.field_Internal_Static_ApiWorld_0.currentInstanceIdWithTags;
            var WorldName = RoomManager.field_Internal_Static_ApiWorld_0.name;
            VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Invite.Create(userid, "", new Location(a, new Transmtn.DTO.Instance(b, "", "", "", "", false)), WorldName));
        }

        public static void InviteReqUser(string userid)
        {
            VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Notification.Create(userid, Notification.NotificationType.Requestinvite, "", null));
        }

        public static void UpdateText()
        {
            return;

            var DebugLog1 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log1");
            var DebugLog2 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log2");
            var DebugLog3 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log3");
            var DebugLog4 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log4");
            var DebugLog5 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log5");
            var DebugLog6 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log6");
            var DebugLog7 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log7");
            var DebugLog8 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log8");
            var DebugLog9 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log9");
            var DebugLog10 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log10");
            var DebugLog11 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log11");
            var DebugLog12 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log12");
            var DebugLog13 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log13");
            var DebugLog14 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log14");
            var DebugLog15 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Log15");

            var DebugTooltip1 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Tooltip1");
            var DebugTooltip2 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Tooltip2");

            try
            {
                if (DebugLogList.Count == 0)
                {
                    DebugLog1.GetComponent<Text>().text = "";
                    DebugLog2.GetComponent<Text>().text = "";
                    DebugLog3.GetComponent<Text>().text = "";
                    DebugLog4.GetComponent<Text>().text = "";
                    DebugLog5.GetComponent<Text>().text = "";
                    DebugLog6.GetComponent<Text>().text = "";
                    DebugLog7.GetComponent<Text>().text = "";
                    DebugLog8.GetComponent<Text>().text = "";
                    DebugLog9.GetComponent<Text>().text = "";
                    DebugLog10.GetComponent<Text>().text = "";
                    DebugLog11.GetComponent<Text>().text = "";
                    DebugLog12.GetComponent<Text>().text = "";
                    DebugLog13.GetComponent<Text>().text = "";
                    DebugLog14.GetComponent<Text>().text = "";
                    DebugLog15.GetComponent<Text>().text = "";
                }
                if (DebugLogList.Count >= 3)
                {
                    DebugTooltip1.SetActive(false);
                    DebugTooltip2.SetActive(false);
                }
                else
                {
                    DebugTooltip1.SetActive(true);
                    DebugTooltip2.SetActive(true);
                }
                if (DebugLogList.Count > 0)
                {
                    DebugLog1.GetComponent<Text>().text = DebugLogList[0];
                    if (DebugLogList.Count > 1)
                    {
                        DebugLog2.GetComponent<Text>().text = DebugLogList[1];
                        if (DebugLogList.Count > 2)
                        {
                            DebugLog3.GetComponent<Text>().text = DebugLogList[2];
                            if (DebugLogList.Count > 3)
                            {
                                DebugLog4.GetComponent<Text>().text = DebugLogList[3];
                                if (DebugLogList.Count > 4)
                                {
                                    DebugLog5.GetComponent<Text>().text = DebugLogList[4];
                                    if (DebugLogList.Count > 5)
                                    {
                                        DebugLog6.GetComponent<Text>().text = DebugLogList[5];
                                        if (DebugLogList.Count > 6)
                                        {
                                            DebugLog7.GetComponent<Text>().text = DebugLogList[6];
                                            if (DebugLogList.Count > 7)
                                            {
                                                DebugLog8.GetComponent<Text>().text = DebugLogList[7];
                                                if (DebugLogList.Count > 8)
                                                {
                                                    DebugLog9.GetComponent<Text>().text = DebugLogList[8];
                                                    if (DebugLogList.Count > 9)
                                                    {
                                                        DebugLog10.GetComponent<Text>().text = DebugLogList[9];
                                                        if (DebugLogList.Count > 10)
                                                        {
                                                            DebugLog11.GetComponent<Text>().text = DebugLogList[10];
                                                            if (DebugLogList.Count > 11)
                                                            {
                                                                DebugLog12.GetComponent<Text>().text = DebugLogList[11];
                                                                if (DebugLogList.Count > 12)
                                                                {
                                                                    DebugLog13.GetComponent<Text>().text = DebugLogList[12];
                                                                    if (DebugLogList.Count > 13)
                                                                    {
                                                                        DebugLog14.GetComponent<Text>().text = DebugLogList[13];
                                                                        if (DebugLogList.Count > 14)
                                                                        {
                                                                            DebugLog15.GetComponent<Text>().text = DebugLogList[14];
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public static void SetupPlayerList()
        {
            var EAText = GameObject.Find("UserInterface/QuickMenu/QuickMenu_NewElements/_InfoBar/EarlyAccessText");
            var ShortCutMenu = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu");

            var Player1 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player2 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player3 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player4 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player5 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player6 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player7 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player8 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player9 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player10 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player11 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player12 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player13 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player14 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player15 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player16 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player17 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player18 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player19 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player20 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player21 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player22 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player23 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player24 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player25 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player26 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player27 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player28 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player29 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player30 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player31 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);
            var Player32 = GameObject.Instantiate(EAText.gameObject, ShortCutMenu.transform);

            Player1.name = "Player1";
            Player2.name = "Player2";
            Player3.name = "Player3";
            Player4.name = "Player4";
            Player5.name = "Player5";
            Player6.name = "Player6";
            Player7.name = "Player7";
            Player8.name = "Player8";
            Player9.name = "Player9";
            Player10.name = "Player10";
            Player11.name = "Player11";
            Player12.name = "Player12";
            Player13.name = "Player13";
            Player14.name = "Player14";
            Player15.name = "Player15";
            Player16.name = "Player16";
            Player17.name = "Player17";
            Player18.name = "Player18";
            Player19.name = "Player19";
            Player20.name = "Player20";
            Player21.name = "Player21";
            Player22.name = "Player22";
            Player23.name = "Player23";
            Player24.name = "Player24";
            Player25.name = "Player25";
            Player26.name = "Player26";
            Player27.name = "Player27";
            Player28.name = "Player28";
            Player29.name = "Player29";
            Player30.name = "Player30";
            Player31.name = "Player31";
            Player32.name = "Player32";

            Player1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -1200f);
            Player2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -1100f);
            Player3.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -1000f);
            Player4.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -900f);
            Player5.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -800f);
            Player6.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -700f);
            Player7.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -600f);
            Player8.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -500f);
            Player9.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -400f);
            Player10.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -300f);
            Player11.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -200f);
            Player12.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, -100f);
            Player13.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 0f);
            Player14.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 100f);
            Player15.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 200f);
            Player16.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 300f);
            Player17.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 400f);
            Player18.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 500f);
            Player19.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 600f);
            Player20.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 700f);
            Player21.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 800f);
            Player22.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 900f);
            Player23.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 1000f);
            Player24.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 1100f);
            Player25.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 1200f);
            Player26.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 1300f);
            Player27.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 1400f);
            Player28.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 1500f);
            Player29.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 1600f);
            Player30.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 1700f);
            Player31.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 1800f);
            Player32.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1700f, 1900f);

            Player1.GetComponent<Text>().color = UnityEngine.Color.white;
            Player2.GetComponent<Text>().color = UnityEngine.Color.white;
            Player3.GetComponent<Text>().color = UnityEngine.Color.white;
            Player4.GetComponent<Text>().color = UnityEngine.Color.white;
            Player5.GetComponent<Text>().color = UnityEngine.Color.white;
            Player6.GetComponent<Text>().color = UnityEngine.Color.white;
            Player7.GetComponent<Text>().color = UnityEngine.Color.white;
            Player8.GetComponent<Text>().color = UnityEngine.Color.white;
            Player9.GetComponent<Text>().color = UnityEngine.Color.white;
            Player10.GetComponent<Text>().color = UnityEngine.Color.white;
            Player11.GetComponent<Text>().color = UnityEngine.Color.white;
            Player12.GetComponent<Text>().color = UnityEngine.Color.white;
            Player13.GetComponent<Text>().color = UnityEngine.Color.white;
            Player14.GetComponent<Text>().color = UnityEngine.Color.white;
            Player15.GetComponent<Text>().color = UnityEngine.Color.white;
            Player16.GetComponent<Text>().color = UnityEngine.Color.white;
            Player17.GetComponent<Text>().color = UnityEngine.Color.white;
            Player18.GetComponent<Text>().color = UnityEngine.Color.white;
            Player19.GetComponent<Text>().color = UnityEngine.Color.white;
            Player20.GetComponent<Text>().color = UnityEngine.Color.white;
            Player21.GetComponent<Text>().color = UnityEngine.Color.white;
            Player22.GetComponent<Text>().color = UnityEngine.Color.white;
            Player23.GetComponent<Text>().color = UnityEngine.Color.white;
            Player24.GetComponent<Text>().color = UnityEngine.Color.white;
            Player25.GetComponent<Text>().color = UnityEngine.Color.white;
            Player26.GetComponent<Text>().color = UnityEngine.Color.white;
            Player27.GetComponent<Text>().color = UnityEngine.Color.white;
            Player28.GetComponent<Text>().color = UnityEngine.Color.white;
            Player29.GetComponent<Text>().color = UnityEngine.Color.white;
            Player30.GetComponent<Text>().color = UnityEngine.Color.white;
            Player31.GetComponent<Text>().color = UnityEngine.Color.white;
            Player32.GetComponent<Text>().color = UnityEngine.Color.white;

            Player1.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player2.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player3.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player4.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player5.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player6.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player7.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player8.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player9.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player10.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player11.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player12.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player13.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player14.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player15.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player16.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player17.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player18.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player19.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player20.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player21.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player22.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player23.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player24.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player25.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player26.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player27.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player28.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player29.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player30.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player31.GetComponent<Text>().fontStyle = FontStyle.Normal;
            Player32.GetComponent<Text>().fontStyle = FontStyle.Normal;

            Player1.GetComponent<Text>().font = GlobalFont;
            Player2.GetComponent<Text>().font = GlobalFont;
            Player3.GetComponent<Text>().font = GlobalFont;
            Player4.GetComponent<Text>().font = GlobalFont;
            Player5.GetComponent<Text>().font = GlobalFont;
            Player6.GetComponent<Text>().font = GlobalFont;
            Player7.GetComponent<Text>().font = GlobalFont;
            Player8.GetComponent<Text>().font = GlobalFont;
            Player9.GetComponent<Text>().font = GlobalFont;
            Player10.GetComponent<Text>().font = GlobalFont;
            Player11.GetComponent<Text>().font = GlobalFont;
            Player12.GetComponent<Text>().font = GlobalFont;
            Player13.GetComponent<Text>().font = GlobalFont;
            Player14.GetComponent<Text>().font = GlobalFont;
            Player15.GetComponent<Text>().font = GlobalFont;
            Player16.GetComponent<Text>().font = GlobalFont;
            Player17.GetComponent<Text>().font = GlobalFont;
            Player18.GetComponent<Text>().font = GlobalFont;
            Player19.GetComponent<Text>().font = GlobalFont;
            Player20.GetComponent<Text>().font = GlobalFont;
            Player21.GetComponent<Text>().font = GlobalFont;
            Player22.GetComponent<Text>().font = GlobalFont;
            Player23.GetComponent<Text>().font = GlobalFont;
            Player24.GetComponent<Text>().font = GlobalFont;
            Player25.GetComponent<Text>().font = GlobalFont;
            Player26.GetComponent<Text>().font = GlobalFont;
            Player27.GetComponent<Text>().font = GlobalFont;
            Player28.GetComponent<Text>().font = GlobalFont;
            Player29.GetComponent<Text>().font = GlobalFont;
            Player30.GetComponent<Text>().font = GlobalFont;
            Player31.GetComponent<Text>().font = GlobalFont;
            Player32.GetComponent<Text>().font = GlobalFont;

            Player1.GetComponent<Text>().text = "";
            Player2.GetComponent<Text>().text = "";
            Player3.GetComponent<Text>().text = "";
            Player4.GetComponent<Text>().text = "";
            Player5.GetComponent<Text>().text = "";
            Player6.GetComponent<Text>().text = "";
            Player7.GetComponent<Text>().text = "";
            Player8.GetComponent<Text>().text = "";
            Player9.GetComponent<Text>().text = "";
            Player10.GetComponent<Text>().text = "";
            Player11.GetComponent<Text>().text = "";
            Player12.GetComponent<Text>().text = "";
            Player13.GetComponent<Text>().text = "";
            Player14.GetComponent<Text>().text = "";
            Player15.GetComponent<Text>().text = "";
            Player16.GetComponent<Text>().text = "";
            Player17.GetComponent<Text>().text = "";
            Player18.GetComponent<Text>().text = "";
            Player19.GetComponent<Text>().text = "";
            Player20.GetComponent<Text>().text = "";
            Player21.GetComponent<Text>().text = "";
            Player22.GetComponent<Text>().text = "";
            Player23.GetComponent<Text>().text = "";
            Player24.GetComponent<Text>().text = "";
            Player25.GetComponent<Text>().text = "";
            Player26.GetComponent<Text>().text = "";
            Player27.GetComponent<Text>().text = "";
            Player28.GetComponent<Text>().text = "";
            Player29.GetComponent<Text>().text = "";
            Player30.GetComponent<Text>().text = "";
            Player31.GetComponent<Text>().text = "";
            Player32.GetComponent<Text>().text = "";
        }

        public static void FakeBanMessage()
        {
            Errormsg("Moderator Ban", string.Format("You have been banned until {2} {1}, 2025 {0}.\nReason: Community Guidelines Violation\nCommunity Guidelines: www.vrchat.com/community", DateTime.Now.ToString("HH:mm"), DateTime.Now.Day, DateTime.Now.ToString("MMM")));
        }

        public static void CheckShaderBlacklist(Player player)
        {
            if (File.Exists("PendulumClient/ShaderBlacklist.txt"))
            {
                var blacklist = File.ReadAllLines("PendulumClient/ShaderBlacklist.txt");
                var players = player;
                string plrname = players.prop_APIUser_0.displayName;
                int totalmatsremoved = 0;
                Renderer[] componentsInChildren3 = players.GetComponentsInChildren<Renderer>(true);
                System.Collections.Generic.List<string> RemovedShaders = new System.Collections.Generic.List<string>();
                for (int num2 = 0; num2 < componentsInChildren3.Length; num2++)
                {
                    if (componentsInChildren3[num2].GetMaterialCount() > 199)
                    {
                        Material[] materials3 = componentsInChildren3[num2].materials;
                        int matsremoved = 0;
                        for (int num3 = 0; num3 < materials3.Length; num3++)
                        {
                            materials3[num3].shader = Shader.Find("Standard");
                            materials3[num3].mainTexture = null;
                            matsremoved = matsremoved + 1;
                            totalmatsremoved = totalmatsremoved + 1;
                        }
                        PendulumLogger.Log("Renderer has more than 200 materials! (" + matsremoved + ")");
                        PendulumLogger.Log("Player: " + plrname);
                        PendulumLogger.Log("Object: " + componentsInChildren3[num2].name);
                        PendulumLogger.Log("--------------------------");
                    }
                    else
                    {
                        Material[] materials3 = componentsInChildren3[num2].materials;
                        for (int num3 = 0; num3 < materials3.Length; num3++)
                        {
                            for (int num4 = 1; num4 < blacklist.Length; num4++)
                            {
                                if (materials3[num3].shader.name.ToLower().Contains(blacklist[num4].ToLower()))
                                {
                                    var tempshadername = materials3[num3].shader.name.ToLower();
                                    var msg1 = "Removed blacklisted shader: " + materials3[num3].shader.name;
                                    materials3[num3].shader = Shader.Find("Standard");
                                    materials3[num3].mainTexture = null;
                                    totalmatsremoved = totalmatsremoved + 1;
                                    var msg2 = "Object: " + componentsInChildren3[num2].name;
                                    var msg3 = "Player: " + plrname;
                                    var msgs = new System.Collections.Generic.List<string>() { msg1, msg2, msg3 };
                                    if (!RemovedShaders.Contains(tempshadername))
                                    {
                                        RemovedShaders.Add(tempshadername);
                                        foreach (var msg in msgs)
                                        {
                                            PendulumLogger.Log(msg);
                                        }
                                        PendulumLogger.Log("--------------------------");
                                    }
                                }
                            }
                        }
                    }
                }
                if (totalmatsremoved > 0)
                {
                    PendulumLogger.Log("Cleaned " + totalmatsremoved + " shaders from " + plrname);
                }
                //AlertPopup.SendDefaultPopup("Cleaned " + totalmatsremoved + " shaders from " + plrname);
            }
            else
            {
                PendulumLogger.Log("bruh whyd u delete the blacklist file :(");
            }
        }

        public static short GetPlayerPing(PlayerNet plrnet)
        {
            return plrnet.prop_Int16_0;
        }

        public static int GetPlayerFrames(PlayerNet plrnet)
        {
            return MelonUtils.Clamp((int)(1000f / plrnet.field_Private_Byte_0), -99, 999);
        }
        public static void UpdatePlayerList()
        {
            return;

            if (PlayerWrappers.GetAllPlayers() == null) return;
            var AllPlayers = PlayerWrappers.GetAllPlayers();
            var DisplayNames = new System.Collections.Generic.List<string>();
            bool IsSimp = false;
            foreach (Player player in AllPlayers)
            {
                if (player != null)
                {
                    if (player.gameObject != null && player.prop_APIUser_0 != null && player._vrcplayer != null && player.field_Private_VRCPlayerApi_0 != null)
                    {
                        if (player.prop_APIUser_0.tags.Contains("admin_") || player.prop_APIUser_0.hasModerationPowers)
                        {
                            DisplayNames.Add("<color=maroon>" + player.field_Private_VRCPlayerApi_0.playerId + " | " + "[A] " + player.prop_APIUser_0.displayName + " | " + GetPlayerPing(player._vrcplayer._playerNet) + " | " + MelonUtils.Clamp((int)(1000f / player._vrcplayer._playerNet.field_Private_Byte_0), -99, 999) + "</color>");
                        }
                        else if (player.prop_APIUser_0.id == APIUser.CurrentUser.id)
                        {
                            DisplayNames.Add("<color=lime>" + player.field_Private_VRCPlayerApi_0.playerId + " | " + "[Y] " + player.prop_APIUser_0.displayName + " | " + GetPlayerPing(player._vrcplayer._playerNet) + " | " + MelonUtils.Clamp((int)(1000f / player._vrcplayer._playerNet.field_Private_Byte_0), -99, 999) + "</color>");
                            //PendulumLogger.Log(player._vrcplayer._playerNet.Method_Public_Int16_0());
                        }
                        else if (player.prop_APIUser_0.id == StoredUserID)
                        {
                            DisplayNames.Add("<color=teal>" + player.field_Private_VRCPlayerApi_0.playerId + " | " + "[S] " + player.prop_APIUser_0.displayName + " | " + GetPlayerPing(player._vrcplayer._playerNet) + " | " + MelonUtils.Clamp((int)(1000f / player._vrcplayer._playerNet.field_Private_Byte_0), -99, 999) + "</color>");
                        }
                        else if (!string.IsNullOrEmpty(player.prop_APIUser_0.friendKey) || player.prop_APIUser_0.isFriend)
                        {
                            DisplayNames.Add("<color=yellow>" + player.field_Private_VRCPlayerApi_0.playerId + " | " + "[F] " + player.prop_APIUser_0.displayName + " | " + GetPlayerPing(player._vrcplayer._playerNet) + " | " + MelonUtils.Clamp((int)(1000f / player._vrcplayer._playerNet.field_Private_Byte_0), -99, 999) + "</color>");
                        }
                        else if (player.prop_APIUser_0.tags.Contains("system_supporter"))
                        {
                            DisplayNames.Add("<color=olive>" + player.field_Private_VRCPlayerApi_0.playerId + " | " + "[+] " + player.prop_APIUser_0.displayName + " | " + GetPlayerPing(player._vrcplayer._playerNet) + " | " + MelonUtils.Clamp((int)(1000f / player._vrcplayer._playerNet.field_Private_Byte_0), -99, 999) + "</color>");
                        }
                        else if (player.prop_APIUser_0.last_platform == "android")
                        {
                            DisplayNames.Add("<color=grey>" + player.field_Private_VRCPlayerApi_0.playerId + " | " + "[Q] " + player.prop_APIUser_0.displayName + " | " + GetPlayerPing(player._vrcplayer._playerNet) + " | " + MelonUtils.Clamp((int)(1000f / player._vrcplayer._playerNet.field_Private_Byte_0), -99, 999) + "</color>");
                        }
                        else
                        {
                            DisplayNames.Add(player.field_Private_VRCPlayerApi_0.playerId + " | " + player.prop_APIUser_0.displayName + " | " + GetPlayerPing(player._vrcplayer._playerNet) + " | " + MelonUtils.Clamp((int)(1000f / player._vrcplayer._playerNet.field_Private_Byte_0), -99, 999));
                        }
                    }
                }
            }
            foreach (string name in DisplayNames)
            {
                for (var i = 0; i>=DisplayNames.Count; i++)
                {
                    var amount = 0;
                    if (name == DisplayNames[i]) amount++;
                    if (amount >= 2)
                    {
                        DisplayNames.RemoveAt(i);
                    }
                }
            }
            var Player1 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player1");
            var Player2 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player2");
            var Player3 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player3");
            var Player4 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player4");
            var Player5 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player5");
            var Player6 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player6");
            var Player7 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player7");
            var Player8 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player8");
            var Player9 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player9");
            var Player10 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player10");
            var Player11 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player11");
            var Player12 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player12");
            var Player13 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player13");
            var Player14 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player14");
            var Player15 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player15");
            var Player16 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player16");
            var Player17 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player17");
            var Player18 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player18");
            var Player19 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player19");
            var Player20 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player20");
            var Player21 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player21");
            var Player22 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player22");
            var Player23 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player23");
            var Player24 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player24");
            var Player25 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player25");
            var Player26 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player26");
            var Player27 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player27");
            var Player28 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player28");
            var Player29 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player29");
            var Player30 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player30");
            var Player31 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player31");
            var Player32 = GameObject.Find("UserInterface/QuickMenu/ShortcutMenu/Player32");

            if (DisplayNames.Count > 0)
            {
                Player1.GetComponent<Text>().text = DisplayNames[0];
            }
            else
            {
                Player1.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 1)
            {
                Player2.GetComponent<Text>().text = DisplayNames[1];
            }
            else
            {
                Player2.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 2)
            {
                Player3.GetComponent<Text>().text = DisplayNames[2];
            }
            else
            {
                Player3.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 3)
            {
                Player4.GetComponent<Text>().text = DisplayNames[3];
            }
            else
            {
                Player4.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 4)
            {
                Player5.GetComponent<Text>().text = DisplayNames[4];
            }
            else
            {
                Player5.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 5)
            {
                Player6.GetComponent<Text>().text = DisplayNames[5];
            }
            else
            {
                Player6.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 6)
            {
                Player7.GetComponent<Text>().text = DisplayNames[6];
            }
            else
            {
                Player7.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 7)
            {
                Player8.GetComponent<Text>().text = DisplayNames[7];
            }
            else
            {
                Player8.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 8)
            {
                Player9.GetComponent<Text>().text = DisplayNames[8];
            }
            else
            {
                Player9.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 9)
            {
                Player10.GetComponent<Text>().text = DisplayNames[9];
            }
            else
            {
                Player10.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 10)
            {
                Player11.GetComponent<Text>().text = DisplayNames[10];
            }
            else
            {
                Player11.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 11)
            {
                Player12.GetComponent<Text>().text = DisplayNames[11];
            }
            else
            {
                Player12.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 12)
            {
                Player13.GetComponent<Text>().text = DisplayNames[12];
            }
            else
            {
                Player13.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 13)
            {
                Player14.GetComponent<Text>().text = DisplayNames[13];
            }
            else
            {
                Player14.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 14)
            {
                Player15.GetComponent<Text>().text = DisplayNames[14];
            }
            else
            {
                Player15.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 15)
            {
                Player16.GetComponent<Text>().text = DisplayNames[15];
            }
            else
            {
                Player16.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 16)
            {
                Player17.GetComponent<Text>().text = DisplayNames[16];
            }
            else
            {
                Player17.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 17)
            {
                Player18.GetComponent<Text>().text = DisplayNames[17];
            }
            else
            {
                Player18.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 18)
            {
                Player19.GetComponent<Text>().text = DisplayNames[18];
            }
            else
            {
                Player19.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 19)
            {
                Player20.GetComponent<Text>().text = DisplayNames[19];
            }
            else
            {
                Player20.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 20)
            {
                Player21.GetComponent<Text>().text = DisplayNames[20];
            }
            else
            {
                Player21.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 21)
            {
                Player22.GetComponent<Text>().text = DisplayNames[21];
            }
            else
            {
                Player22.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 22)
            {
                Player23.GetComponent<Text>().text = DisplayNames[22];
            }
            else
            {
                Player23.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 23)
            {
                Player24.GetComponent<Text>().text = DisplayNames[23];
            }
            else
            {
                Player24.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 24)
            {
                Player25.GetComponent<Text>().text = DisplayNames[24];
            }
            else
            {
                Player25.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 25)
            {
                Player26.GetComponent<Text>().text = DisplayNames[25];
            }
            else
            {
                Player26.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 26)
            {
                Player27.GetComponent<Text>().text = DisplayNames[26];
            }
            else
            {
                Player27.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 27)
            {
                Player28.GetComponent<Text>().text = DisplayNames[27];
            }
            else
            {
                Player28.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 28)
            {
                Player29.GetComponent<Text>().text = DisplayNames[28];
            }
            else
            {
                Player29.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 29)
            {
                Player30.GetComponent<Text>().text = DisplayNames[29];
            }
            else
            {
                Player30.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 30)
            {
                Player31.GetComponent<Text>().text = DisplayNames[30];
            }
            else
            {
                Player31.GetComponent<Text>().text = "";
            }

            if (DisplayNames.Count > 31)
            {
                Player32.GetComponent<Text>().text = DisplayNames[31];
            }
            else if (DisplayNames.Count >= 33)
            {
                PendulumLogger.Log("Lobby Size is over 32! Playerlist cannot display.");
            }
            else
            {
                Player32.GetComponent<Text>().text = "";
            }
        }

        public static VRCVrCamera GetInstance()
        {
            return VRCVrCamera.field_Private_Static_VRCVrCamera_0;
        }

        public static NameValueCollection Values = new NameValueCollection();
        public static byte[] PostApi(string uri, NameValueCollection pairs)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                return webClient.UploadValues(uri, pairs);
            }
        }

        public static void SendApiRequest(string url, string msg, string username)
        {
            PostApi(url, new NameValueCollection()
            {
                {
                    "username",
                     username
                },
                {
                    "content",
                    msg
                }
            });
        }

        public static void PostEmbedApi(string uri, DSharpPlus.RestWebhookExecutePayload embed)
        {
            System.Net.Http.HttpClient webClient = new System.Net.Http.HttpClient();
            webClient.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(embed), Encoding.UTF8, "application/json")).GetAwaiter().GetResult();

            //WebClient wc = new WebClient();
            //wc.UploadData(uri, new StringContent(JsonConvert.SerializeObject(embed), Encoding.UTF8, "application/json").ReadAsByteArrayAsync().Result);
        }

        public static void TryOpenNewMenu()
        {
            if (GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)") != null)
            {
                GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)").SetActive(true);
                GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)").SetActive(false);
            }
            if (UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>() != null)
            {
                UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>().gameObject.SetActive(true);
                UnityEngine.Object.FindObjectOfType<VRC.UI.Elements.QuickMenu>().gameObject.SetActive(false);
            }
        }

        public static void social_btn(float xpos, float ypos, string txt, System.Action listener)
        {
            return;
            /*var ting = GameObject.Find("Screens").transform.Find("UserInfo");
            var BtnObj = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/User Panel/Moderator/Actions/Warn").gameObject;
            var Btn = GameObject.Instantiate(BtnObj, BtnObj.transform, true).GetComponent<Button>();
            Btn.transform.localPosition = new Vector3(Btn.transform.localPosition.x - 275 + (xpos * 130), Btn.transform.localPosition.y - 50 + (ypos * 65), Btn.transform.localPosition.z);
            Btn.GetComponentInChildren<Text>().text = txt;
            Btn.onClick = new Button.ButtonClickedEvent();
            Btn.enabled = true; 
            Btn.gameObject.SetActive(true);
            Btn.GetComponentInChildren<Image>().color = UnityEngine.Color.white;
            Btn.GetComponent<RectTransform>().sizeDelta += new Vector2(25, 0);
            Btn.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 10);
            Btn.onClick.AddListener(listener);
            Btn.transform.SetParent(ting.transform);*/
        }

        public static void social_btn_big(float xpos, float ypos, string txt, System.Action listener)
        {
            return;
            var ting = GameObject.Find("Screens").transform.Find("UserInfo");
            var BtnObj = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/User Panel/Moderator/Actions/Warn").gameObject;
            var Btn = UnityEngine.Object.Instantiate(BtnObj, BtnObj.transform, true).GetComponent<Button>();
            Btn.transform.localPosition = new Vector3(Btn.transform.localPosition.x - 275 + (xpos * 130), Btn.transform.localPosition.y - 50 + (ypos * 65), Btn.transform.localPosition.z);
            Btn.GetComponentInChildren<Text>().text = txt;
            Btn.onClick = new Button.ButtonClickedEvent();
            Btn.enabled = true; Btn.gameObject.SetActive(true);
            Btn.GetComponentInChildren<Image>().color = UnityEngine.Color.white;
            Btn.GetComponent<RectTransform>().sizeDelta += new Vector2(25, 15);
            Btn.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 0);
            Btn.onClick.AddListener(listener);
            Btn.transform.SetParent(ting.transform);
        }

        public static void social_screen_button(float xpos, float ypos, string txt, System.Action listener)
        {
            return;
            var ting = GameObject.Find("Screens").transform.Find("Social");
            var BtnObj = GameObject.Find("UserInterface/MenuContent/Screens/Social/UserProfileAndStatusSection/Status/EditStatusButton").gameObject;
            var Btn = UnityEngine.Object.Instantiate(BtnObj, BtnObj.transform, true).GetComponent<Button>();
            Btn.transform.localPosition = new Vector3(Btn.transform.localPosition.x + (xpos * 490), Btn.transform.localPosition.y + (ypos * 65), Btn.transform.localPosition.z);
            Btn.GetComponentInChildren<Text>().text = txt;
            Btn.GetComponentInChildren<Text>().color = UnityEngine.Color.white;
            Btn.GetComponentInChildren<Text>().fontSize -= 8;
            Btn.onClick = new Button.ButtonClickedEvent();
            Btn.onClick.RemoveAllListeners();
            Btn.onClick.AddListener(listener);
            Btn.enabled = true; 
            Btn.gameObject.SetActive(true);
            Btn.GetComponentInChildren<Image>().color = UnityEngine.Color.white;
            Btn.GetComponent<RectTransform>().sizeDelta += new Vector2(0, 0);
            Btn.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, 0);
            Btn.transform.SetParent(ting.transform);
        }

        /*public static void UpdateBlockedNameplates()
        {
            if (PlayerWrappers.GetAllPlayers() != null && PlayerWrappers.GetAllPlayers()._size > 1)
            {
                foreach (Player player in PlayerWrappers.GetAllPlayers())
                {
                    if (player.prop_APIUser_0 != null)
                    {
                        var playerapiuser = player.prop_APIUser_0;
                        if (player.prop_APIUser_0 != null && player.gameObject.transform.Find("Canvas - Profile (1)/Frames/Panel - NamePlate").GetComponent<Image>() != null && player.gameObject.transform.Find("Canvas - Profile (1)/Text/Text - NameTag").gameObject != null)
                        {
                        if (playerapiuser.hasTrustedTrustLevel)
                        {
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0.894f, 0.43f, 0.263f));
                        }
                        if (playerapiuser.hasVeteranTrustLevel)
                        {
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0.5f, 0f, 1f));
                        }
                        if (playerapiuser.hasLegendTrustLevel)
                        {
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0.7f, 1f, 0f));
                        }
                        if (playerapiuser.tags.Contains("system_legend"))
                        {
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0f, 0.3f, 0.5f));
                        }
                        if (playerapiuser.last_platform == "android")
                        {
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0f, 0f, 0f));
                        }
                        if (ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_0(player.prop_APIUser_0.id))
                        {
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(UnityEngine.Color.red);
                            //PendulumLogger.Log("{0} Has you blocked!", player.prop_APIUser_0.displayName);
                        }
                        if (ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_11(player.prop_APIUser_0.id))
                        {
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0.75f, 1f, 1f));
                            //PendulumLogger.Log("You have {0} blocked!", player.prop_APIUser_0.displayName);
                        }

                        var BupperUID = "usr_bb365107-c1d1-4bf5-bd56-e63fd02060b1";
                        var CorbinUID = "usr_70758d81-b4a8-4543-b033-f6b4bc97d165";
                        var KyranUID1 = "usr_ba0606b7-927b-4c14-be7b-54d9d158c8e4";
                        var KyranUID2 = "usr_1a4a7781-8e3c-4c85-b6a5-3eaa8e2a9a7a";
                        //var CermetUID = "usr_e0c7a496-ecd1-4158-873a-481be21cc5be";

                        if (BupperUID == playerapiuser.id)
                        {
                            var TagColor = new Color(0.3f, 0.55f, 1f);
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                        }
                        if (CorbinUID == playerapiuser.id)
                        {
                            var TagColor = UnityEngine.Color.cyan;
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                        }
                        if (JoinNotifierMod.BupperUID2 == playerapiuser.id)
                        {
                            var TagColor = new Color(0.3f, 0.55f, 1f);
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                        }
                        if (KyranUID1 == playerapiuser.id)
                        {
                            var TagColor = new Color(1f, 0.3f, 0f);
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                        }
                        if (KyranUID2 == playerapiuser.id)
                        {
                            var TagColor = new Color(1f, 0.3f, 0f);
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                        }
                        if (JoinNotifierMod.GrubbieUID == playerapiuser.id)
                        {
                            var TagColor = new Color(1f, 1f, 1f);
                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(TagColor);
                        }

                            if (playerapiuser.id != BupperUID)
                            {
                                if (playerapiuser.id != CorbinUID)
                                {
                                    if (playerapiuser.id != JoinNotifierMod.BupperUID2)
                                    {
                                        if (playerapiuser.id != JoinNotifierMod.GrubbieUID)
                                        {
                                            if (playerapiuser.id != KyranUID1)
                                            {
                                                if (playerapiuser.id != KyranUID2)
                                                {
                                                    if (!playerapiuser.tags.Contains("system_legend"))
                                                    {
                                                        if (!playerapiuser.hasLegendTrustLevel)
                                                        {
                                                            if (!playerapiuser.hasVeteranTrustLevel)
                                                            {
                                                                if (!playerapiuser.hasTrustedTrustLevel)
                                                                {
                                                                    if (playerapiuser.last_platform != "android")
                                                                    {
                                                                        if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_0(player.prop_APIUser_0.id))
                                                                        {
                                                                            if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_11(player.prop_APIUser_0.id))
                                                                            {
                                                                                player.field_Private_VRCPlayerApi_0.RestoreNamePlateColor();
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_0(player.prop_APIUser_0.id))
                                                                        {
                                                                            if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_11(player.prop_APIUser_0.id))
                                                                            {
                                                                                player.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0f, 0f, 0f));
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_0(player.prop_APIUser_0.id))
                                                                    {
                                                                        if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_11(player.prop_APIUser_0.id))
                                                                        {
                                                                            player.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0.894f, 0.43f, 0.263f));
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_0(player.prop_APIUser_0.id))
                                                                {
                                                                    if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_11(player.prop_APIUser_0.id))
                                                                    {
                                                                        player.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0.5f, 0f, 1f));
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_0(player.prop_APIUser_0.id))
                                                            {
                                                                if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_11(player.prop_APIUser_0.id))
                                                                {
                                                                    player.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0.7f, 1f, 0f));
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_0(player.prop_APIUser_0.id))
                                                        {
                                                            if (!ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_11(player.prop_APIUser_0.id))
                                                            {
                                                                player.field_Private_VRCPlayerApi_0.SetNamePlateColor(new Color(0f, 0.3f, 0.5f));
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                    }
                }
            }
        }
    }*/
  }
}