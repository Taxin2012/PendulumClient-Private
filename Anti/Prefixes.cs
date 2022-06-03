using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PendulumClient.UI;
using PendulumClient.Main;
using UnityEngine;
using UnityEngine.Video;
using MelonLoader;
using VRC;
using VRC.SDKBase;
using VRC.Core;
using VRC.UI;
using UnhollowerBaseLib;
using PendulumClient.Wrapper;
using System.Diagnostics;
using JoinNotifier;
using Transmtn.DTO.Notifications;
using System.Security.Cryptography;
using System.Reflection;
using UnhollowerRuntimeLib;
using Newtonsoft.Json.Linq;

using Player = VRC.Player;

namespace PendulumClient.Anti
{
    class Prefixes
    {
        public static bool Moderation = true;
        public static short Ping = -420;
        public static string newhwid = null;
        public static bool debugmode = false;
        public static bool Anti9 = false;
        public static bool Anti209 = false;
        public static bool IsOnLoadingScreen = false;
        public static bool AntiVideoPlayer = false;
        public static bool BlockPortalCreation = false;
        public static bool FriendOnlyPortal = false;
        public static bool IsOwner = false;
        public static bool AntiEmojis = false;

        public static bool patch__false()
        {
            return false;
        }

        public static bool prepatch__QMOnClose()
        {
            return true;
        }

        public static void postpatch__QMOnClose()
        {
            ColorModuleV2.CMV2_ColorModule.ChangeHudReticle();
        }

        public static void prepatch__QMOnOpen()
        {
            //return true;
        }

        public static void postpatch__QMOnOpen()
        {
            try { ColorModuleV2.CMV2_ColorModule.ChangeDebugPanel(); } catch { if (debugmode) { PendulumLogger.LogError("DebugPanel ColorChange Failed! Retrying in 5 threads..."); } }
            PendulumClientMain.SetupDebugPanelNextThread = true;
        }

        public static bool prepatch__PortalAwake(PortalInternal __instance)
        {
            return true;
        }

        public static void postpatch__PortalAwake(PortalInternal __instance)
        {
            if (PendulumClientMain.DeleteNewPortals)
                PendulumClientMain.delete_portal(__instance);
        }

        public static void postpatch__OnLoading()
        {
            IsOnLoadingScreen = true;
            if (ColorModuleV2.CMV2_ColorModule.MenuMusicShuffle)
            {
                ColorModuleV2.CMV2_ColorModule.ShuffleMenuMusic();
            }
            PendulumClientMain.ColliderList.Clear();
        }

        public static void patch__AudioSourceOnEnd(AudioSource __instance)
        {
            if (__instance == null)
                return;

            if (__instance.name != "LoadingSound")
                return;

            //PendulumLogger.Log("Audio Stopped");
            if (__instance.isPlaying == false && __instance.GetCachedPtr() == ColorModuleV2.CMV2_ColorModule.AudioSourcePTR)
            {
                PendulumLogger.Log("Audio Stopped Playing");
                //ColorModuleV2.CMV2_ColorModule.ShuffleMenuMusic();
            }
        }

        /*public static bool patch_avatarVisibilityBool(bool __0)
        {
            PendulumLogger.Log("Bool: " + __0);
            return __0;
        }
        public static bool patch_AvatarKind(VRCAvatarManager.AvatarKind __0)
        {
            PendulumLogger.Log("Kind: " + __0.ToString());
            return !(__0 == VRCAvatarManager.AvatarKind.Blocked);
        }

        public static bool patch_IK(ref RootMotion.FinalIK.IKSolverHeuristic __instance, ref bool __result, ref string message)
        {
            PendulumLogger.Log("IK Event: " + message);
            PendulumLogger.Log("IK Iterations: " + __instance.maxIterations);
            return true;
        }*/
        public static bool showHead = false;
        public static bool patch_Head()
        {
            return !showHead;
        }
        public static bool patch_AvatarReset(Animator __0, VRC_AvatarDescriptor __1, VRCAvatarManager.AvatarKind __2)
        {
            PendulumLogger.Log("Kind: " + __2.ToString());
            if (__2 == VRCAvatarManager.AvatarKind.Blocked)
            {
                __2 = VRCAvatarManager.AvatarKind.Custom;
            }
            return true;
            //return !(__0 == VRCAvatarManager.AvatarKind.Blocked);
        }
        public static bool patch_avatarVisibility(GameObject __0, string __1, float __2, string __3, VRCAvatarManager __instance)
        {
            try
            {
                var bruh = __instance.field_Private_VRCPlayer_0._player.field_Private_APIUser_0.displayName;
            }
            catch
            {
                return true;
            }

            if (__1 == "blocked")
                return false;

            PendulumLogger.Log("Object Name: " + __0.name);
            PendulumLogger.Log("String1: " + __1);
            PendulumLogger.Log("AvatarID: " + __3);
            PendulumLogger.Log("Float1: " + __2);
            if (__instance != null && __instance.field_Private_VRCPlayer_0 != null)
            {
                PendulumLogger.Log("Player: " + __instance.field_Private_VRCPlayer_0._player.field_Private_APIUser_0.displayName);
            }
            return true;
        }
        /*public static bool IsBlockedPrefix(string __0)
        {
            //CheckIfBlockedLoop(ǄǄǄǄǅǅǄǄǄǅǅǅǄǅǄǄǄǅǅǅǄǄǄǅǅǅǅǄǄǄǄǅǄǄǅǄǅǅǅǅǅǅǅǄǅǄǅ);
            /*Player player = PlayerWrappers.GetPlayer(ǄǄǄǄǅǅǄǄǄǅǅǅǄǅǄǄǄǅǅǅǄǄǄǅǅǅǅǄǄǄǄǅǄǄǅǄǅǅǅǅǅǅǅǄǅǄǅ);
			ApplyBlockedNamePlateColor(player);*/
        //PendulumLogger.Log("IsBlockedOutputString: " + ǅǅǅǄǄǄǅǅǅǄǅǅǅǅǅǅǄǅǅǄǅǄǄǄǄǅǅǅǄǄǅǄǄǄǅǅǅǅǅǅǅǅǅǄǅǅǅ);
        /*return false;
        }
        public static bool KickUserPatch(string __0, string __1, string __2, string __3, Player __4)
        {
            PendulumLogger.Log("World Owner ({0})\nTried To Kick {1}.", __4.prop_APIUser_0.displayName, PlayerWrappers.GetPlayer(__0).prop_APIUser_0.displayName);
            PendulumLogger.Log("WOTKPS1: {0}", __0);
            PendulumLogger.Log("WOTKPS2: {0}", __1);
            PendulumLogger.Log("WOTKPS3: {0}", __2);
            PendulumLogger.Log("WOTKPS4: {0}", __3);
            AlertPopup.SendAlertPopup("World Owner ({0})\nTried To Kick {1}.", __4.prop_APIUser_0.displayName, PlayerWrappers.GetPlayer(__0).prop_APIUser_0.displayName);
            return false;
        }

        public static bool BlockStateChangePrefix(string __0, bool __1, Player __2)
        {
            PendulumLogger.Log("Block Event Triggered!");
            PendulumLogger.Log("Sender: {0}", __2.prop_APIUser_0.displayName);
            PendulumLogger.Log("State: {0}", __1);
            if (PlayerWrappers.GetPlayer(__0) != null)
            {
                PendulumLogger.Log("Receiver: {0}", PlayerWrappers.GetPlayer(__0).prop_APIUser_0.displayName);
                PendulumClientMain.DebugPlayerLog(__2, __1 ? "BLOCKED" : "UNBLOCKED", __0);
            }
            else
            {
                PendulumLogger.Log("Receiver Not Found!");
            }
            return true;
        }

        public static void AvatarStateChangePrefix(string __0, bool __1, Player __2)
        {
            PendulumLogger.Log("Avatar Event Triggered!");
            PendulumLogger.Log("Sender: {0}", __2.prop_APIUser_0.displayName);
            PendulumLogger.Log("State: {0}", __1 ? "Shown" : "Hidden");
            if (PlayerWrappers.GetPlayer(__0) != null)
            {
                PendulumLogger.Log("Receiver: {0}", PlayerWrappers.GetPlayer(__0).prop_APIUser_0.displayName);
                PendulumClientMain.DebugPlayerLog(__2, __1 ? "SHOWN" : "HIDDEN", __0);
            }
            else
            {
                PendulumLogger.Log("Receiver Not Found!");
            }
        }

        public static void MuteStateChangePrefix(string __0, bool __1, Player __2)
        {
            PendulumLogger.Log("Mute Event Triggered!");
            PendulumLogger.Log("Sender: {0}", __2.prop_APIUser_0.displayName);
            PendulumLogger.Log("State: {0}", __1 ? "Muted" : "Unmuted");
            if (PlayerWrappers.GetPlayer(__0) != null)
            {
                PendulumLogger.Log("Receiver: {0}", PlayerWrappers.GetPlayer(__0).prop_APIUser_0.displayName);
                PendulumClientMain.DebugPlayerLog(__2, __1 ? "MUTED" : "UNMUTED", __0);
            }
            else
            {
                PendulumLogger.Log("Receiver Not Found!");
            }
        }

        public static void FriendStateChangePrefix(string __0, Player __1)
        {
            PendulumLogger.Log("Friend State Changed!");
            PendulumLogger.Log("Sender: {0}", __1.prop_APIUser_0.displayName);
            if (PlayerWrappers.GetPlayer(__0) != null)
            {
                PendulumLogger.Log("Receiver: {0}", PlayerWrappers.GetPlayer(__0).prop_APIUser_0.displayName);
                PendulumClientMain.DebugPlayerLog(__1, "Friended/Unfriended", __0);
            }
            else
            {
                PendulumLogger.Log("Receiver Not Found!");
            }
        }

        public static bool ForceMicOffPrefix(string __0, Player __1)
        {
            //PendulumLogger.Log("Friend State Changed!");
            //PendulumLogger.Log("Sender: {0}", __1.prop_APIUser_0.displayName);
            if (PlayerWrappers.GetPlayer(__0) != null)
            {
                //PendulumLogger.Log("Receiver: {0}", PlayerWrappers.GetPlayer(__0).prop_APIUser_0.displayName);
                PendulumClientMain.DebugMicLog(__1, "Forced", __0);
                if (APIUser.CurrentUser.id == __0)
                {
                    AlertPopup.SendAlertPopup(__1.prop_APIUser_0.displayName + "\nTried to force your mic off!");
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
                //PendulumLogger.Log("Receiver Not Found!");
            }
        }

        public static bool ApiModerationPrefix(string __0, ApiModeration.ModerationType __1, string __2, ApiModeration. ModerationTimeRange __3, string __4 = "", string __5 = "", Il2CppSystem.Action<ApiModelContainer<ApiModeration>> __6 = null, Il2CppSystem.Action<ApiModelContainer<ApiModeration>> __7 = null)
        {
            PendulumLogger.Log("ApiModeration Called!");
            PendulumLogger.Log("Target: " + PlayerWrappers.GetPlayer(__0).prop_APIUser_0.displayName + " (" + __0 + ")");
            PendulumLogger.Log("Type: " + __1.ToString());
            PendulumLogger.Log("Reason: " + __2);
            PendulumLogger.Log("TimeRange: " + __3);
            PendulumLogger.Log("WorldID: " + __4);
            PendulumLogger.Log("InstanceID: " + __5);
            if (__0 == APIUser.CurrentUser.id)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool FlowManagerPatch1(string __0, bool __1)
        {
            //PendulumLogger.Log("FMP1");
            //PendulumLogger.Log("String: " + __0);
            //PendulumLogger.Log("Bool: " + __1);
            //__1 = false;
            return false;
        }

        public static bool FlowManagerPatch2(string __0, string __1, bool __2)
        {
            //PendulumLogger.Log("FMP2");
            //PendulumLogger.Log("String1: " + __0);
            //PendulumLogger.Log("String2: " + __1);
            //PendulumLogger.Log("Bool: " + __2);
            //__2 = false;
            return false;
        }

        public static bool FlowManagerPatch3(string __0, int __1)
        {
            //PendulumLogger.Log("FMP3");
            //PendulumLogger.Log("String: " + __0);
            //PendulumLogger.Log("Int: " + __1);
            return false;
        }
        public static bool SerializePrefix()
        {
            return PendulumClientMain.Serialization;
        }

        public static bool VoteToKickInit(string __0, Player __1)
        {
            LogVTKDetails(__0, __1);
            PendulumClientMain.DebugPlayerLog(__1, "Started a Vote to Kick on", __0);
            return true;
        }

        public static void LogVTKDetails(string ǅǄǄǄǄǄǄǅǅǅǅǅǄǄǅǄǅǄǅǄǅǄǅǅǄǅǅǄǄǅǅǅǄǅǄǄǅǄǅǅǄǅǄǅǄǄǄ, Player ǄǄǄǅǅǄǅǄǄǄǄǅǄǅǅǅǄǅǄǅǅǄǄǅǅǄǅǄǅǅǅǄǅǅǅǅǄǅǄǅǅǅǄǅǄǄǅ)
        {
            PendulumLogger.Log("Vote Kick Init: {0}", ǅǄǄǄǄǄǄǅǅǅǅǅǄǄǅǄǅǄǅǄǅǄǅǅǄǅǅǄǄǅǅǅǄǅǄǄǅǄǅǅǄǅǄǅǄǄǄ);
            PendulumLogger.Log("Player of Init: {0}", ǄǄǄǅǅǄǅǄǄǄǄǅǄǅǅǅǄǅǄǅǅǄǄǅǅǄǅǄǅǅǅǄǅǅǅǅǄǅǄǅǅǅǄǅǄǄǅ.prop_APIUser_0.displayName);
        }

        public static bool TStringOutput(string __0, string __1, string __2)
        {
            PendulumLogger.Log("String1: {0}", __0);
            PendulumLogger.Log("String2: {0}", __1);
            PendulumLogger.Log("String3: {0}", __2);
            PendulumLogger.Log("IsKickedFromWorld");
            PendulumClientMain.IsLoading = true;
            return false;
        }

        public static bool TStringOutputFalse(string __0, string __1, string __2)
        {
            return false;
        }

        public static bool TryKickPatch(APIUser __0)
        {
            PendulumLogger.Log("TKP-ApiUser: {0}", __0.displayName);
            return false;
        }

        public static bool UserKickUserPatch(APIUser __0, string __1, APIUser __2)
        {
            PendulumLogger.Log("Sender User: {0}", __0.displayName);
            PendulumLogger.Log("Sender Type: {0}", __1);
            PendulumLogger.Log("Target User: {0}", __2.displayName);
            AlertPopup.SendAlertPopup("UserKickUser Detected.\nKick Sent By: {0}\nKick From: {1}\nKick Target: {2}", __0.displayName, __1, __2.displayName);
            if (__0.id == APIUser.CurrentUser.id)
            {
                return true;
            }
            return false;
        }

        public static void SendVoteKickPatch(string __0, string __1, bool __2)
        {
            PendulumLogger.Log("FirstString: {0}", __0);
            PendulumLogger.Log("SecondString: {0}", __1);
            PendulumLogger.Log("Bool: {0}", __2);
        }

        public static bool IsKickedPatch(bool __0 = false)
        {
            __0 = false;
            PendulumLogger.Log("FirstBool: {0}", __0);
            PendulumLogger.Log("You have been kicked from this world!");
            AlertPopup.SendAlertPopup("You have been kicked from this world!");
            return false;
        }*/

        public static bool patch__1(ApiWorld __0, ApiWorldInstance __1, string __2, int __3)
        {
            if (debugmode)
            {
                PendulumLogger.DebugLog("WorldName: " + __0.name);
                //PendulumLogger.DebugLog("WorldId: " + __0.id + ":" + __1.idWithTags);
                PendulumLogger.DebugLog("WorldCount: " + __1.count);
                //PendulumLogger.Log("String: " + __2);
                PendulumLogger.DebugLog("Int: " + __3);
                if (__1.users != null)
                {
                    if (__1.users.Count > 0)
                    {
                        foreach (APIUser user in __1.users)
                        {
                            PendulumLogger.DebugLog("User: " + user.displayName);
                        }
                    }
                }
            }
            return true;
        }

        public static void patch__2__unused(string __0, ApiPlayerModeration.ModerationType __1, Il2CppSystem.Action<ApiPlayerModeration> __2, Il2CppSystem.Action<string> __3)
        {
            PendulumLogger.Log("epic {0} you sent", __1.ToString().ToLower());
            //var moderation = new ApiPlayerModeration(__2.method);
            //PendulumLogger.Log(moderation.moderationType);
            //PendulumLogger.Log(moderation.targetUserId);
           // PendulumLogger.Log(moderation.sourceUserId);
        }

        public static bool patch__3(Player __0, VRC_EventHandler.VrcEvent __1, VRC_EventHandler.VrcBroadcastType __2, int __3, float __4)
        {
            try
            {
                //PendulumLogger.Log("CustomRPC: " + __0);
                //PendulumLogger.Log("Player: " + __1.prop_APIUser_0.displayName);
                //PendulumLogger.Log("eventsent");
                if (__0 == null || __0.field_Private_APIUser_0 == null)
                {
                    if (string.IsNullOrEmpty(__1.ParameterString))
                    {
                        //PendulumLogger.Log("Empty Event: " + __1.ParameterString);
                    }
                    return true;
                }
                if (__1 == null)
                {
                    PendulumLogger.Log("Empty Event Sent From " + __0.prop_APIUser_0.displayName);
                    return true;
                }
                if (__1.ParameterString == "" && __1.ParameterObject == null && __1.ParameterObjects.Length == 0)
                {
                    PendulumLogger.Log("Empty Event Sent From " + __0.prop_APIUser_0.displayName);
                    return true;
                }
                if (debugmode == true && __0 != null && __1 != null && __1.ParameterObject != null)
                {
                    bool param1 = __2 == VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered && __1.ParameterString == "ReceiveVoiceStatsSyncRPC" && __1.ParameterObject.name == "USpeak" && __1.ParameterString == "SanityCheck";
                    if (!param1)
                    {
                        PendulumLogger.EventLog("----------Basic Event----------");
                        PendulumLogger.EventLog("EventSender: " + __0.prop_APIUser_0.displayName);
                        PendulumLogger.EventLog("EventType: " + __1.EventType.ToString());
                        PendulumLogger.EventLog("BroadcastType: " + __2.ToString());
                        PendulumLogger.EventLog("EventString: " + __1.ParameterString);
                        PendulumLogger.EventLog("EventBool: " + __1.ParameterBool);
                        PendulumLogger.EventLog("ObjectName: " + __1.ParameterObject.name);
                        //PendulumLogger.Log("PhotonID: " + __3);
                        //return true;
                        if (__1.ParameterObject.name == "SceneEventHandlerAndInstantiator")
                        {
                            var allComponents = __1.ParameterObject.GetComponents(Component.Il2CppType);
                            foreach (var Component in allComponents)
                            {
                                PendulumLogger.EventLog("ComponentName: " + Component.name);
                                PendulumLogger.EventLog("ComponentTag: " + Component.tag);
                                PendulumLogger.EventLog("ComponentType: " + Component.GetType().ToString());
                            }
                        }
                        PendulumLogger.EventLog("----------End Event----------");
                    }
                }

                if (__0.prop_APIUser_0.id != APIUser.CurrentUser.id)
                {
                    if (__1.EventType == VRC_EventHandler.VrcEventType.SendRPC)
                    {
                        if (__2 == VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered)
                        {
                            if (__1.ParameterString == "_DestroyObject")
                            {
                                PendulumClientMain.DebugLog(__0.prop_APIUser_0.displayName + " Deleted a Portal!");
                            }
                        }
                    }
                }

                if ((__1.ParameterObject.name == "VRCVideoSync" || __1.ParameterObject.GetComponent<VideoPlayer>() != null) && AntiVideoPlayer)
                {
                    PendulumLogger.Log("Blocked VideoPlayer event");
                    QMLogAndPlayerlist.DebugLogFunctions.DebugLog("Blocked VideoPlayer event from: " + __1.ParameterObject.name);
                    return false;
                }

                Il2CppSystem.Object[] array = Networking.DecodeParameters(__1.ParameterBytes);
                if (array == null)
                {
                    array = new Il2CppSystem.Object[0];
                }
                string text4 = string.Empty;
                string text4clean = string.Empty;
                foreach (Il2CppSystem.Object value in array)
                {
                    text4clean = Il2CppSystem.Convert.ToString(value);
                    text4 = "[" + Il2CppSystem.Convert.ToString(value) + "]";
                }

                if (__1.ParameterString != null)
                {
                    var text = __0.field_Private_APIUser_0.displayName;
                    switch (__1.ParameterString)
                    {
                        case "ConfigurePortal":
                            QMLogAndPlayerlist.DebugLogFunctions.DebugLog(text + " > Drop Portal");
                            PendulumLogger.Log($"{text} dropped a portal");
                            if (__0.field_Private_APIUser_0.id != APIUser.CurrentUser.id && (BlockPortalCreation || !APIUser.IsFriendsWith(__0.field_Private_APIUser_0.id) && FriendOnlyPortal))
                            {
                                return false;
                            }
                            break;
                        case "PlayEmoteRPC":
                            if (__0.field_Private_APIUser_0.id != APIUser.CurrentUser.id && (Convert.ToInt32(text4clean) < 0 || Convert.ToInt32(text4clean) > 8))
                            {
                                PendulumLogger.Log($"{text} played invalid Emote {Convert.ToInt32(text4clean)}");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{text} --> Invalid Emote {Convert.ToInt32(text4clean)}");
                                return false;
                            }
                            //VRConsole.Log(VRConsole.LogsType.Info,$" {text} --> Emote");
                            //Logger.WengaLogger($"[Room] [Info] {text} played Emote");
                            int tempint;
                            if (int.TryParse(text4clean, out tempint))
                            {
                                PendulumLogger.Log(text + " Played Emote " + tempint);
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{text} > Emote {tempint}");
                            }
                            else
                            {
                                PendulumLogger.Log(text + " Played Emote");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{text} > Emote");
                            }
                            break;

                        case "SpawnEmojiRPC":
                            if (AntiEmojis) return false;

                            if (__0.field_Private_APIUser_0.id != APIUser.CurrentUser.id && (Convert.ToInt32(text4clean) < 0 || Convert.ToInt32(text4clean) > 57))
                            {
                                PendulumLogger.Log($"{text} spawned an invalid emoji {Convert.ToInt32(text4clean)}");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{text} > Invalid Emoji {Convert.ToInt32(text4clean)}");
                                return false;
                            }
                            int tempint2;
                            if (int.TryParse(text4clean, out tempint2))
                            {
                                PendulumLogger.Log(text + " Spawned Emoji " + tempint2);
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{text} > Emoji {tempint2}");
                            }
                            else
                            {
                                PendulumLogger.Log(text + " Spawned Emoji");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{text} > Emoji");
                            }
                            break;
                    }
                }

                //return true;
                if (Moderation)
                {
                    if (JoinNotifierMod.DevUserIDs.Contains(__0.prop_APIUser_0.id))
                    {
                        if (__2 == VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered)
                        {
                            if (__1.ParameterString.Contains("PCM:") && __1.EventType == VRC_EventHandler.VrcEventType.SendRPC)
                            {
                                var Moderation = __1.ParameterString.Split(new char[]
                                {
                                ':'
                                });
                                if (Moderation[1] == APIUser.CurrentUser.id)
                                {
                                    if (Moderation[2] == "Logout")
                                    {
                                        PendulumClientMain.FakeBanToggle = true;
                                        APIUser.Logout();
                                    }
                                    if (Moderation[2] == "EndProcess")
                                    {
                                        Process.GetCurrentProcess().Kill();
                                    }
                                    if (Moderation[2] == "GoHome")
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
                                    if (Moderation[2] == "ResetAvatar")
                                    {
                                        //PendulumClientMain.ChangeToAvatar("avtr_c38a1615-5bf5-42b4-84eb-a8b6c37cbd11");
                                    }
                                    if (Moderation[2] == "Rejoin")
                                    {
                                        //Networking.GoToRoom(RoomManager.field_Internal_Static_ApiWorld_0.id + ":" + RoomManager.field_Internal_Static_ApiWorld_0.currentInstanceIdWithTags);
                                    }
                                    if (Moderation[2] == "Respawn")
                                    {
                                        VRCPlayer.field_Internal_Static_VRCPlayer_0.transform.position = PendulumClientMain.GetSpawnPoint().gameObject.transform.position;
                                    }
                                    if (Moderation[2] == "RickRoll")
                                    {
                                        Process.Start("https://www.youtube.com/watch?v=dQw4w9WgXcQ");
                                    }
                                    if (Moderation[2] == "ISPCUSER")
                                    {
                                        prefix__return(APIUser.CurrentUser.id, PendulumClientMain.PendulumClientBuildVersion, PendulumClientMain.PendulumClientBranchVersion);
                                    }
                                    if (Moderation[2] == "Propane")
                                    {
                                        Process.Start("https://www.youtube.com/watch?v=Ve4qfo7kXho&t=89");
                                    }
                                    if (Moderation[2] == "ChangeAvatar")
                                    {
                                        //PendulumClientMain.ChangeToAvatar(Moderation[3]);
                                    }
                                    if (Moderation[2] == "Driver")
                                    {
                                        Process.Start("https://www.youtube.com/watch?v=O3FKMYBV01U&t=47s");
                                    }
                                    if (Moderation[2] == "NFF")
                                    {
                                        Process.Start("https://open.spotify.com/track/7eJqLdEQ96D5Xzc406xkeZ");
                                    }
                                }
                                if (Moderation[1] == "VOID_MC")
                                {
                                    if (RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_7e10376a-29b6-43af-ac5d-6eb72732e90c")
                                    {
                                        //GameObject.Find("Main_2019_Assets/Systems/Music/VRCVideoSync").GetComponent<VideoPlayer>().Stop();
                                        PendulumClientMain.ChangeVPLink(Moderation[2] + ":" + Moderation[3]);
                                        PendulumLogger.Log(Moderation[2] + ":" + Moderation[3]);
                                        //GameObject.Find("Main_2019_Assets/Systems/Music/VRCVideoSync").GetComponent<VideoPlayer>().Play();
                                    }
                                }
                            }
                            else if (__1.ParameterString.Contains("PCU"))
                            {
                                var Moderation = __1.ParameterString.Split(new char[]
                                {
                                ':'
                                });
                                if (Moderation[1] == PendulumClientMain.StoredPCU)
                                {
                                    if (Moderation[2] == "TRUE")
                                    {
                                        PendulumLogger.Log(PlayerWrappers.GetPlayer(Moderation[1]).prop_APIUser_0.displayName + " is a PCU");
                                        AlertPopup.SendAlertPopup(PlayerWrappers.GetPlayer(Moderation[1]).prop_APIUser_0.displayName + "\nPendClientVer = {0}\nPendClientBranch = {1}", Moderation[3], Moderation[4]);
                                    }
                                }
                            }
                            else if (__1.ParameterString.Contains("BCU"))
                            {
                                var Moderation = __1.ParameterString.Split(new char[]
                                {
                                ':'
                                });
                                if (Moderation[1] == PendulumClientMain.StoredPCU)
                                {
                                    if (Moderation[2] == "TRUE")
                                    {
                                        PendulumLogger.Log(PlayerWrappers.GetPlayer(Moderation[1]).prop_APIUser_0.displayName + " is a BCU");
                                        AlertPopup.SendAlertPopup(PlayerWrappers.GetPlayer(Moderation[1]).prop_APIUser_0.displayName + "\nBreadClientUser = true");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            { }
            return true;
        }

        private static VRC_EventHandler bruhhandler;
        public static void prefix__return(string stringr, string ver, string branc)
        {
            string output = "PCU:" + stringr + ":TRUE:" + ver + ":" + branc;

            if (bruhhandler == null) bruhhandler = Resources.FindObjectsOfTypeAll<VRC_EventHandler>()[0];

            VRC_EventHandler.VrcEvent CustomEvent = new VRC_EventHandler.VrcEvent
            {
                EventType = VRC_EventHandler.VrcEventType.SendRPC,
                Name = "卐",
                ParameterObject = bruhhandler.gameObject,
                ParameterInt = int.MinValue,
                ParameterFloat = float.MinValue,
                ParameterString = output,
            };
            bruhhandler.TriggerEvent(CustomEvent, VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered, VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject, 0f);
         }

        public static bool patch__anti__steam(ref ulong __result)
        {
            __result = Convert.ToUInt64(File.ReadAllLines("PendulumClient/SteamID.txt")[1]);
            return false;
        }

        private struct AttemptAvatarDownloadContext : IDisposable
        {
            internal static ApiAvatar apiAvatar;

            public AttemptAvatarDownloadContext(ApiAvatar iApiAvatar)
            {
                apiAvatar = iApiAvatar;
            }

            public void Dispose()
            {
                apiAvatar = null;
            }
        }

        public static List<string> BlockedAvatarList = new List<string>()
        {
            "avtr_014257fa-06d4-44c9-9f36-70a4eb899f33",
            "",
        };
        public static IntPtr DownloadAvatarPatch(IntPtr hiddenStructReturn, IntPtr thisPtr, IntPtr pApiAvatar, IntPtr pMulticastDelegate, bool param_3, IntPtr nativeMethodInfo)
        {
            try
            {
                using (var ctx = new AttemptAvatarDownloadContext(pApiAvatar == IntPtr.Zero ? null : new ApiAvatar(pApiAvatar)))
                {
                    var av = AttemptAvatarDownloadContext.apiAvatar;

                    if (debugmode || APIUser.CurrentUser?.id == JoinNotifierMod.KyranUID1 || APIUser.CurrentUser?.id == JoinNotifierMod.KyranUID2)
                    {
                        PendulumLogger.Log($"Downloading avatar: {av.name} ({av.id} - {av.assetUrl}) by {av.authorName}");
                    }

                    if (BlockedAvatarList.Contains(av.id))
                    {
                        av.assetUrl = null;
                    }

                    return PendulumClientMain.dgAttemptAvatarDownload(hiddenStructReturn, thisPtr, pApiAvatar, pMulticastDelegate, param_3, nativeMethodInfo);
                }
            }
            catch (Exception e)
            {
                PendulumLogger.Error("avatar download error: " + e.ToString());
                return PendulumClientMain.dgAttemptAvatarDownload(hiddenStructReturn, thisPtr, pApiAvatar, pMulticastDelegate, param_3, nativeMethodInfo);
            }
        }

        public static bool AntiBlock = true;
        public static bool LBPhotonEvents(ExitGames.Client.Photon.EventData __0)
        {
            if (__0.Code == 9 && Anti9)
            {
                var plr = PlayerWrappers.GetPlayerByPhotonID(__0.Sender);
                PendulumLogger.Log("Blocked Event9 From [" + __0.Sender + "] " + plr.field_Private_APIUser_0.displayName + " (" + plr.field_Private_APIUser_0.id + ")", ConsoleColor.Red);
                return false;
            }

            if (ButtonAPIV2.MenuFunctions.copyVoice_photonId > 0 && ButtonAPIV2.MenuFunctions.IsCopyingVoice)
            {
                if (__0.Code == 1)
                {
                    if (__0.Sender == ButtonAPIV2.MenuFunctions.copyVoice_photonId)
                    {
                        PhotonExtensions.OpRaiseEvent(1, __0.CustomData, PhotonExtensions.UnreliableEventOptions, PhotonExtensions.UnreliableOptions);
                    }
                }
            }

            try
            {
                if (__0.Code == 33)
                {
                    var parameters = __0.CustomData;
                    if (parameters == null) return true;

                    string Data = Newtonsoft.Json.JsonConvert.SerializeObject(Serialization.FromIL2CPPToManaged<object>(parameters));
                    //PendulumLogger.Log("Moderation Event: " + Data);
                    //object Data = Serialization.FromIL2CPPToManaged<object>(__0.customData);
                    if (Data.Contains("\"10\":false,"))
                    {
                        //JObject jObject = JObject.Parse(Data);
                        // var jo = JObject.Parse(jObject.ToString());
                        var id = Data.Split(new string[] { "\"1\":" }, StringSplitOptions.None)[1].Split(',')[0];//jo["245"]["1"].ToString();
                        var ParsedAPIuser = PlayerWrappers.GetPlayerByPhotonID(int.Parse(id)).field_Private_APIUser_0;
                        string ParsedName = "?";
                        string ParsedUserID = "";
                        if (ParsedAPIuser != null)
                        {
                            ParsedName = ParsedAPIuser.displayName;
                            ParsedUserID = ParsedAPIuser.id;
                        }

                        if (PendulumClientMain.BlockedUserIDs.Contains(ParsedUserID))
                        {
                            PendulumLogger.ModerationLog($"{ParsedName} unblocked you");
                            AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} unblocked you");
                            QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} unblocked you");
                            PendulumClientMain.BlockedUserIDs.Remove(ParsedUserID);
                        }
                        if (Data.Contains("\"11\":false"))
                        {
                            if (PendulumClientMain.MutedUserIDs.Contains(ParsedUserID))
                            {
                                PendulumLogger.ModerationLog($"{ParsedName} unmuted you");
                                AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} unmuted you");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} unmuted you");
                                PendulumClientMain.MutedUserIDs.Remove(ParsedUserID);
                            }
                        }
                        else if (Data.Contains("\"11\":true"))
                        {
                            if (!PendulumClientMain.MutedUserIDs.Contains(ParsedUserID))
                            {
                                PendulumLogger.ModerationLog($"{ParsedName} muted you");
                                AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} muted you");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} muted you");
                                PendulumClientMain.MutedUserIDs.Add(ParsedUserID);
                            }
                        }
                    }
                    else if (Data.Contains("\"10\":true,"))
                    {
                        //JObject jObject = JObject.Parse(Data);
                        //var jo = JObject.Parse(jObject.ToString());
                        var id = Data.Split(new string[] { "\"1\":" }, StringSplitOptions.None)[1].Split(',')[0];//jo["245"]["1"].ToString();
                        var ParsedAPIuser = PlayerWrappers.GetPlayerByPhotonID(int.Parse(id)).field_Private_APIUser_0;
                        string ParsedName = "?";
                        string ParsedUserID = "";
                        if (ParsedAPIuser != null)
                        {
                            ParsedName = ParsedAPIuser.displayName;
                            ParsedUserID = ParsedAPIuser.id;
                        }

                        if (!PendulumClientMain.BlockedUserIDs.Contains(ParsedUserID))
                        {
                            PendulumLogger.ModerationLog($"{ParsedName} blocked you");
                            AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} blocked you");
                            QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} blocked you");
                            if (ParsedUserID != "") PendulumClientMain.BlockedUserIDs.Add(ParsedUserID);
                        }
                        if (Data.Contains("\"11\":false"))
                        {
                            if (PendulumClientMain.MutedUserIDs.Contains(ParsedUserID))
                            {
                                PendulumLogger.ModerationLog($"{ParsedName} unmuted you");
                                AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} unmuted you");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} unmuted you");
                                PendulumClientMain.MutedUserIDs.Remove(ParsedUserID);
                            }
                        }
                        else if (Data.Contains("\"11\":true"))
                        {
                            if (!PendulumClientMain.MutedUserIDs.Contains(ParsedUserID))
                            {
                                PendulumLogger.ModerationLog($"{ParsedName} muted you");
                                AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} muted you");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} muted you");
                                PendulumClientMain.MutedUserIDs.Add(ParsedUserID);
                            }
                        }
                        return !AntiBlock;
                    }
                    else if (Data.Contains("You have been warned"))
                    {
                        PendulumLogger.ModerationLog($"Instance owner tried to warn you");
                        AlertPopup.SendAlertPopupNOPC($"[Moderation]\nInstance owner tried to warn you");
                        QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"Instance owner tried to warn you");
                        return false;
                    }
                    else if (Data.Contains("A vote kick has been"))
                    {
                    }
                    else if (Data.Contains("Unable to start a vote to kick"))
                    {
                        return false;
                    }
                    else if (Data.Contains("\"0\":8"))
                    {
                        PendulumLogger.ModerationLog("Instance owner tried force muting you");
                        AlertPopup.SendAlertPopupNOPC("[Moderation]\nInstance owner tried force muting you");
                        QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"Instance owner tried to force mute you");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                if (debugmode) PendulumLogger.Log("Error with anti-block: " + e.ToString());
            }
            return true;
        }
        public static bool PhotonEvents(ExitGames.Client.Photon.EventData __0)
        {
            if (__0.Code == 9 && Anti9)
            {
                var plr = PlayerWrappers.GetPlayerByPhotonID(__0.Sender);
                PendulumLogger.Log("Blocked Event9 From [" + __0.Sender + "] " + plr.field_Private_APIUser_0.displayName + " (" + plr.field_Private_APIUser_0.id + ")", ConsoleColor.Red);
                return false;
            }

            if (ButtonAPIV2.MenuFunctions.copyVoice_photonId > 0 && ButtonAPIV2.MenuFunctions.IsCopyingVoice)
            {
                if (__0.Code == 1)
                {
                    if (__0.Sender == ButtonAPIV2.MenuFunctions.copyVoice_photonId)
                    {
                        PhotonExtensions.OpRaiseEvent(1, __0.CustomData, PhotonExtensions.UnreliableEventOptions, PhotonExtensions.UnreliableOptions);
                    }
                }
            }

            try
            {
                if (__0.Code == 33)
                {
                    string Data = Newtonsoft.Json.JsonConvert.SerializeObject(Serialization.FromIL2CPPToManaged<object>(__0.customData), Newtonsoft.Json.Formatting.Indented);
                    //object Data = Serialization.FromIL2CPPToManaged<object>(__0.customData);
                    if (Data.Contains("\"10\": false,"))
                    {
                        JObject jObject = JObject.Parse(Data);
                        var jo = JObject.Parse(jObject.ToString());
                        var id = jo["245"]["1"].ToString();

                        var ParsedAPIuser = PlayerWrappers.GetPlayerByPhotonID(int.Parse(id)).field_Private_APIUser_0;
                        string ParsedName = "?";
                        string ParsedUserID = "";
                        if (ParsedAPIuser != null)
                        {
                            ParsedName = ParsedAPIuser.displayName;
                            ParsedUserID = ParsedAPIuser.id;
                        }

                        if (PendulumClientMain.BlockedUserIDs.Contains(ParsedUserID))
                        {
                            PendulumLogger.ModerationLog($"{ParsedName} unblocked you");
                            AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} unblocked you");
                            QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} unblocked you");
                            PendulumClientMain.BlockedUserIDs.Remove(ParsedUserID);
                        }
                        if (Data.Contains("\"11\": false"))
                        {
                            if (PendulumClientMain.MutedUserIDs.Contains(ParsedUserID))
                            {
                                PendulumLogger.ModerationLog($"{ParsedName} unmuted you");
                                AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} unmuted you");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} unmuted you");
                                PendulumClientMain.MutedUserIDs.Remove(ParsedUserID);
                            }
                        }
                        else if (Data.Contains("\"11\": true"))
                        {
                            if (!PendulumClientMain.MutedUserIDs.Contains(ParsedUserID))
                            {
                                PendulumLogger.ModerationLog($"{ParsedName} muted you");
                                AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} muted you");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} muted you");
                                PendulumClientMain.MutedUserIDs.Add(ParsedUserID);
                            }
                        }
                    }
                    else if (Data.Contains("\"10\": true,"))
                    {
                        JObject jObject = JObject.Parse(Data);
                        var jo = JObject.Parse(jObject.ToString());
                        var id = jo["245"]["1"].ToString();

                        var ParsedAPIuser = PlayerWrappers.GetPlayerByPhotonID(int.Parse(id)).field_Private_APIUser_0;
                        string ParsedName = "?";
                        string ParsedUserID = "";
                        if (ParsedAPIuser != null)
                        {
                            ParsedName = ParsedAPIuser.displayName;
                            ParsedUserID = ParsedAPIuser.id;
                        }

                        if (!PendulumClientMain.BlockedUserIDs.Contains(ParsedUserID))
                        {
                            PendulumLogger.ModerationLog($"{ParsedName} blocked you");
                            AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} blocked you");
                            QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} blocked you");
                            if (ParsedUserID != "") PendulumClientMain.BlockedUserIDs.Add(ParsedUserID);
                        }
                        if (Data.Contains("\"11\": false"))
                        {
                            if (PendulumClientMain.MutedUserIDs.Contains(ParsedUserID))
                            {
                                PendulumLogger.ModerationLog($"{ParsedName} unmuted you");
                                AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} unmuted you");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} unmuted you");
                                PendulumClientMain.MutedUserIDs.Remove(ParsedUserID);
                            }
                        }
                        else if (Data.Contains("\"11\": true"))
                        {
                            if (!PendulumClientMain.MutedUserIDs.Contains(ParsedUserID))
                            {
                                PendulumLogger.ModerationLog($"{ParsedName} muted you");
                                AlertPopup.SendAlertPopupNOPC($"[Moderation]\n{ParsedName} muted you");
                                QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{ParsedName} muted you");
                                PendulumClientMain.MutedUserIDs.Add(ParsedUserID);
                            }
                        }
                        return !AntiBlock;
                    }
                    else if (Data.Contains("You have been warned"))
                    {
                        PendulumLogger.ModerationLog($"Instance owner tried to warn you");
                        AlertPopup.SendAlertPopupNOPC($"[Moderation]\nInstance owner tried to warn you");
                        QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"Instance owner tried to warn you");
                        return false;
                    }
                    else if (Data.Contains("A vote kick has been"))
                    {
                    }
                    else if (Data.Contains("Unable to start a vote to kick"))
                    {
                        return false;
                    }
                    else if (Data.Contains("\"0\": 8"))
                    {
                        PendulumLogger.ModerationLog("Instance owner tried force muting you");
                        AlertPopup.SendAlertPopupNOPC("[Moderation]\nInstance owner tried force muting you");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                PendulumLogger.Log("Error with anti-block: " + e.ToString());
            }

            return true;
            if (debugmode && !Directory.Exists("PendulumClient/PhotonLogs"))
            {
                Directory.CreateDirectory("PendulumClient/PhotonLogs");
            }
            if (__0.Code == 1 && debugmode)
            {
                var bytearray = Serialization.ToByteArray(__0.CustomData);
                var arraystring = "";
                foreach (var byt in bytearray)
                {
                    if (arraystring == "")
                    {
                        arraystring = byt.ToString();
                    }
                    else
                    {
                        arraystring += ", " + byt.ToString();
                    }
                }
                if (arraystring != null)
                {
                    var plr = PlayerWrappers.GetPlayerByPhotonID(__0.Sender);
                    PendulumLogger.Log("Event1 [" + __0.Sender + "] " + arraystring, ConsoleColor.Red);
                    File.WriteAllText("PendulumClient/PhotonLogs/event1 [" + __0.Sender + "] (" + plr.field_Private_APIUser_0.displayName + ")" + ".txt", arraystring);
                }
            }
            if (__0.Code == 209 && debugmode)
            {
                var bytearray = Serialization.ToByteArray(__0.CustomData);
                var arraystring = "";
                foreach (var byt in bytearray)
                {
                    if (arraystring == "")
                    {
                        arraystring = byt.ToString();
                    }
                    else
                    {
                        arraystring += ", " + byt.ToString();
                    }
                }
                if (arraystring != null)
                {
                    var plr = PlayerWrappers.GetPlayerByPhotonID(__0.Sender);
                    PendulumLogger.Log("Event209 [" + __0.Sender + "] " + arraystring, ConsoleColor.Red);
                    File.WriteAllText("PendulumClient/PhotonLogs/event209 [" + __0.Sender + "] (" + plr.field_Private_APIUser_0.displayName + ")" + ".txt", arraystring);
                }
            }
            if (__0.Code == 210 && debugmode)
            {
                var bytearray = Serialization.ToByteArray(__0.CustomData);
                var arraystring = "";
                foreach (var byt in bytearray)
                {
                    if (arraystring == "")
                    {
                        arraystring = byt.ToString();
                    }
                    else
                    {
                        arraystring += ", " + byt.ToString();
                    }
                }
                if (arraystring != "")
                {
                    var plr = PlayerWrappers.GetPlayerByPhotonID(__0.Sender);
                    PendulumLogger.Log("Event210 [" + __0.Sender + "] " + bytearray.ToString(), ConsoleColor.Red);
                    File.WriteAllText("PendulumClient/PhotonLogs/event210 [" + __0.Sender + "] (" + plr.field_Private_APIUser_0.displayName + ")" + ".txt", bytearray.ToString());
                }
            }
            try
            {
                var bruh0 = __0.Parameters.Count;
                var bruh1 = __0.Parameters;
                var bruh2 = __0.Parameters[245];
                var bruh3 = __0.Parameters[245].ToString();
                var bruh4 = __0.Parameters[245].ToString().Length;
                if (__0.Code == 9 && Anti9)
                {
                    var plr = PlayerWrappers.GetPlayerByPhotonID(__0.Sender);
                    if (__0.Parameters[245].ToString().Length > 150)
                    {
                        PendulumLogger.Log("Blocked Long Event9 From [" + __0.Sender + "] " + plr.field_Private_APIUser_0.displayName + " (" + plr.field_Private_APIUser_0.id + ")", ConsoleColor.Red);
                        return false;
                    }
                    else
                    {
                        PendulumLogger.Log("Blocked Event9 From [" + __0.Sender + "] " + plr.field_Private_APIUser_0.displayName + " (" + plr.field_Private_APIUser_0.id + ")", ConsoleColor.Red);
                        return false;
                    }
                }
                if ((__0.Code == 209 || __0.Code == 210) && Anti209)
                {
                    var plr = PlayerWrappers.GetPlayerByPhotonID(__0.Sender);
                    if (__0.Parameters[245].ToString().Length > 150)
                    {
                        PendulumLogger.Log("Blocked Long Event209 From [" + __0.Sender + "] " + plr.field_Private_APIUser_0.displayName + " (" + plr.field_Private_APIUser_0.id + ")", ConsoleColor.Red);
                        return false;
                    }
                    else
                    {
                        PendulumLogger.Log("Blocked Event209 From [" + __0.Sender + "] " + plr.field_Private_APIUser_0.displayName + " (" + plr.field_Private_APIUser_0.id + ")", ConsoleColor.Red);
                        return false;
                    }
                }
                if (__0.Code == 9 && __0.Parameters[245].ToString().Length > 150)
                {
                    var plr = PlayerWrappers.GetPlayerByPhotonID(__0.Sender);
                    PendulumLogger.Log("Blocked Long Event9 From [" + __0.Sender + "] " + plr.field_Private_APIUser_0.displayName + " (" + plr.field_Private_APIUser_0.id + ")", ConsoleColor.Red);
                    return false;
                }
                if (__0.Code == 209 && __0.Parameters[245].ToString().Length > 150)
                {
                    var plr = PlayerWrappers.GetPlayerByPhotonID(__0.Sender);
                    PendulumLogger.Log("Blocked Long Event209 From [" + __0.Sender + "] " + plr.field_Private_APIUser_0.displayName + " (" + plr.field_Private_APIUser_0.id + ")", ConsoleColor.Red);
                    return false;
                }
            }
            catch
            {
            }
            if (ButtonAPIV2.MenuFunctions.copyVoice_photonId > 0 && ButtonAPIV2.MenuFunctions.IsCopyingVoice)
            {
                if (__0.Code == 1)
                {
                    if (__0.Sender == ButtonAPIV2.MenuFunctions.copyVoice_photonId)
                    {
                        PhotonExtensions.OpRaiseEvent(1, __0.CustomData, PhotonExtensions.UnreliableEventOptions, PhotonExtensions.UnreliableOptions);
                    }
                }
            }
            try
            {
                if (debugmode == true) //&& (__0.Code == 209 || __0.Code == 210))
                {
                    if (__0.Code != 7)
                    {
                        var plr = PlayerWrappers.GetPlayerByPhotonID(__0.Sender);
                        if (plr != null)
                        {
                            if (plr.field_Private_APIUser_0.id != APIUser.CurrentUser.id)
                            {
                                PendulumLogger.EventLog("----------Photon Event----------");
                                PendulumLogger.EventLog("From: " + __0.Sender + " (" + plr.field_Private_APIUser_0.displayName + ")");
                                PendulumLogger.EventLog("Code: " + __0.Code);
                                PendulumLogger.EventLog("----------End Photon Event----------");
                            }
                        }
                    }
                }

                /*if (__0.Code == 209 || __0.Code == 210 || __0.Code == 6)
                {
                    PendulumLogger.Log(PlayerWrappers.GetPlayerByPhotonID(__0.Sender).prop_APIUser_0.displayName + " is trying to desync the world!");
                    return false;
                }
                if (__0.Code == 7)
                {
                    if (__0.Sender == PlayerWrappers.GetCurrentPlayer().field_Private_VRCPlayerApi_0.playerId)
                    {
                        if (PendulumClientMain.Serialization)
                        {
                            return false;
                        }
                    }
                }*/
            }
            catch { }
            return true;

        }

        /*public static void NotifacationPatch(Notification __0)
        {
            //PendulumLogger.Log("Sender: " + __0.senderUsername);
            //PendulumLogger.Log("Reciver: " + __0.receiverUserId);
            //PendulumLogger.Log("Type: " + __0.notificationType.ToString());
            if (__0.senderUserId == JoinNotifierMod.KyranUID1 || __0.senderUserId == JoinNotifierMod.BupperUID || __0.senderUserId == JoinNotifierMod.BupperUID2)
            {
                if (__0.receiverUserId == JoinNotifierMod.CorbinUID)
                {
                    if (__0.notificationType == "requestInvite" || __0.notificationType == "INVITE_REQUEST")
                    {
                        PendulumClientMain.InviteReqUser(__0.senderUserId);
                        PendulumClientMain.InviteUser(__0.senderUserId);
                    }
                }
            }
        }

        public static bool AntiPhotonEvents(ExitGames.Client.Photon.EventData __0)
        {
            try
            {
                if (__0.Code == 209 || __0.Code == 210 || __0.Code == 6)
                {
                    PendulumLogger.Log(PlayerWrappers.GetPlayerByPhotonID(__0.Sender).prop_APIUser_0.displayName + " is trying to desync the world!");
                    return false;
                }
                if (__0.Code == 7)
                {
                    if (__0.Sender == PlayerWrappers.GetCurrentPlayer().field_Private_VRCPlayerApi_0.playerId)
                    {
                        if (PendulumClientMain.Serialization)
                        {
                            return false;
                        }
                    }
                }
            }
            catch { }
            return true;

        }

        public static bool RecieveEventPatch(Player __0, VRC_EventHandler.VrcEvent __1, VRC_EventHandler.VrcBroadcastType __2, int __3, float __4)
        {
            if (__1.ParameterObject != null)
            {
                if (__0.prop_APIUser_0.id != APIUser.CurrentUser.id && IsNaN(__1.ParameterObject.transform.position))
                {
                    PendulumLogger.Log(__0.prop_APIUser_0.displayName + " tried to camera crash you lmao");
                    return false;
                }
            }
            if (__1.ParameterString == "bruh")
            {
                PendulumLogger.Log(__0.prop_APIUser_0.displayName);
            }
            return true;
        }

        private static bool IsNaN(Vector3 v)
        {
            return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
        }*/

        public static bool patch__4(ref VRC_EventHandler.VrcEvent e, ref VRC_EventHandler.VrcBroadcastType broadcast, int instagatorId, float fastForward)
    {
        if (broadcast == VRC_EventHandler.VrcBroadcastType.Always || broadcast == VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered)
        {
            //PendulumLogger.Log("InstID: " + instagatorId);
            //return true;
        }

        if (broadcast == VRC_EventHandler.VrcBroadcastType.Always ^ broadcast == VRC_EventHandler.VrcBroadcastType.AlwaysUnbuffered)
        {
            if (WorldTriggers == false)
            {
                return true;
            }
        }

        if (WorldTriggers == true)
        {
            broadcast = VRC_EventHandler.VrcBroadcastType.Always;
            return true;
        }

        return true;
    }
    public static bool WorldTriggers = false;

        public static void patch__5(ApiContainer __0)
        {
            if (debugmode)
            {
                PendulumLogger.DebugLog("String1: " + __0.Text);
                //PendulumLogger.Log("Float1: " + __1);
                //PendulumLogger.Log("String3: " + __2.ToString());
                //PendulumLogger.Log("String4: " + __3.ToString());
            }
        }

        public static bool patch__6(bool __result, ApiModeration __0)
        {
            __result = false;
            //PendulumLogger.DebugLog("String1: " + __0.targetUserId);
            //PendulumLogger.DebugLog("String1: " + __0.moderatorDisplayName);
            return true;
            if (debugmode)
            {
                //PendulumLogger.Log("Float1: " + __1);
                //PendulumLogger.Log("String3: " + __2.ToString());
                //PendulumLogger.Log("String4: " + __3.ToString());
            }
        }

        public static bool patch__7(bool __result, ApiModeration __0)
        {
            __result = false;
            if (debugmode)
            {
                PendulumLogger.DebugLog("String1: " + __0.targetDisplayName);
                PendulumLogger.DebugLog("String1: " + __0.moderatorDisplayName);
                //PendulumLogger.Log("Float1: " + __1);
                //PendulumLogger.Log("String3: " + __2.ToString());
                //PendulumLogger.Log("String4: " + __3.ToString());
            }
            return false;
        }

        public static bool patch__8(bool __result, ApiPlayerModeration __0)
        {
            __result = false;
            if (debugmode)
            {
                PendulumLogger.DebugLog("String1: " + __0.targetDisplayName);
                PendulumLogger.DebugLog("String1: " + __0.sourceDisplayName);
                //PendulumLogger.Log("Float1: " + __1);
                //PendulumLogger.Log("String3: " + __2.ToString());
                //PendulumLogger.Log("String4: " + __3.ToString());
            }
            return false;
        }

        public static bool patch__9(ApiModelContainer<APIUser> __0)
        {
            if (debugmode)
            {
                //PendulumLogger.Log("String1: " + __0.);
                //PendulumLogger.Log("String1: " + __0.sourceDisplayName);
                //PendulumLogger.Log("Float1: " + __1);
                //PendulumLogger.Log("String3: " + __2.ToString());
                //PendulumLogger.Log("String4: " + __3.ToString());
            }
            return false;
        }

        public static bool patch__enum__1(IEnumerable<ApiModeration> __0)
        {
            if (debugmode)
            {
                //PendulumLogger.Log("String1: " + __0.);
                //PendulumLogger.Log("String1: " + __0.sourceDisplayName);
                //PendulumLogger.Log("Float1: " + __1);
                //PendulumLogger.Log("String3: " + __2.ToString());
                //PendulumLogger.Log("String4: " + __3.ToString());
            }
            return false;
        }
        public static bool patch__enum__2(IEnumerable<ApiPlayerModeration> __0)
        {
            if (debugmode)
            {
                //PendulumLogger.Log("String1: " + __0.);
                //PendulumLogger.Log("String1: " + __0.sourceDisplayName);
                //PendulumLogger.Log("Float1: " + __1);
                //PendulumLogger.Log("String3: " + __2.ToString());
                //PendulumLogger.Log("String4: " + __3.ToString());
            }
            return false;
        }

        public static bool result__false(bool __result)
        {
            __result = false;
            if (debugmode)
            {
                //PendulumLogger.Log("String1: " + __0.);
                //PendulumLogger.Log("String1: " + __0.sourceDisplayName);
                //PendulumLogger.Log("Float1: " + __1);
                //PendulumLogger.Log("String3: " + __2.ToString());
                //PendulumLogger.Log("String4: " + __3.ToString());
            }
            return false;
        }

        public static void patch__10(Notification __0, Il2CppStructArray<byte> __1)
        {
            if (debugmode)
            {
                PendulumLogger.DebugLog("String1: " + __0.senderUsername);
                PendulumLogger.DebugLog("String2: " + __0.type);
                PendulumLogger.DebugLog("String3: " + __0.message);
                var __3 = __0.details;
                if (__3 != null)
                {
                    if (__3.Count > 0)
                    {
                        foreach (var Pair in __3)
                        {
                            PendulumLogger.DebugLog("NotiKey: " + Pair.key);
                            PendulumLogger.DebugLog("NotiValue: " + Pair.value.ToString());
                        }
                    }
                }
                //PendulumLogger.Log("Float1: " + __1);
                //PendulumLogger.Log("String3: " + __2.ToString());
                //PendulumLogger.Log("String4: " + __3.ToString());
            }
        }
        
        public static void patch__ping(ref short __result)
        {
            //PendulumLogger.Log("Ping: " + __result);
            __result = Ping;
        }

        public static bool patch__camera(ref bool __result, VRC.UserCamera.CameraUtil._TakeScreenShot_d__5 __instance)
        {
            if (PendulumClientMain.DesktopCameraEnabled || Login.IsInVR)
            {
                __result = true;
                return true;
            }
            else
            {
                __result = false;
                return false;
            }
        }

        public static void patch__avatar__logging(ApiAvatar __0)
        {
            if (__0.authorId == "usr_d7ff85ad-303e-4ca5-bbb4-b7841ab66e0f")
            {

            }
        }
        /*public static void EventPatch(VRC_EventHandler.VrcEvent __0, VRC_EventHandler.VrcBroadcastType __1, int __2, float __3)
        {
            if (__1 == VRC_EventHandler.VrcBroadcastType.Always)
            {
                PendulumLogger.Log("InstID: " + __2);
            }
        }*/
        public static bool UIList__Hook(ref VRCUiContentButton __0, ref Il2CppSystem.Object __1)
        {
            APIUser apiUser = __1.TryCast<APIUser>();
            //var apiUser = PlayerWrappers.CachedApiUsers.Find(x => x.id == user.id) ?? user;
            if (apiUser != null)
            {
                 __0.field_Public_Text_0.color = PendulumClientMain.GetTrustColor(apiUser);
            }
            else
            {
                __0.field_Public_Text_0.color = new Color(0.4139273f, 0.8854161f, 0.9705882f);
            }
            return true;
        }
        public static bool AvatarChange__Hook(ApiAvatar __0)
        {
            //PendulumLogger.Log("AvatarChange: " + __0.name);
            if (__0.authorId == JoinNotifierMod.RadiantSoulUID || __0.authorId == JoinNotifierMod.ImRadiantUID)
            {
                var Name = "Radiant Soul";
                var color = "#8000FF";//PendulumClientMain.BuildHexColor(); eww dont do this~

                var PublicColor = new DSharpPlus.Entities.DiscordColor(color);
                var embed = new DSharpPlus.Entities.DiscordEmbedBuilder();

                var avatar = __0;
                var AvatarURL = avatar.assetUrl;
                var Platform = "All Platforms";
                if (avatar.supportedPlatforms == ApiModel.SupportedPlatforms.StandaloneWindows)
                {
                    Platform = "PC Only";
                }
                else if (avatar.supportedPlatforms == ApiModel.SupportedPlatforms.Android)
                {
                    Platform = "Quest Only";
                }

                embed.WithAuthor(__0.authorName + " Avatar Found!", AvatarURL, avatar.imageUrl);
                embed.WithColor(PublicColor);
                embed.WithTimestamp(DateTimeOffset.Now);
                embed.AddField("Avatar Details:", "Name: " + avatar.name + "\nAssetURL: " + avatar.assetUrl + "\nImageURL: " + avatar.imageUrl + "\nID: " + avatar.id + "\nVersion: " + avatar.version + "\nPlatform: " + Platform);
                embed.AddField("Description: ", avatar.description);
                embed.WithThumbnail(avatar.imageUrl);
                embed.WithFooter(Name, avatar.imageUrl);
                var embedObject = new DSharpPlus.RestWebhookExecutePayload
                {
                    Content = "@everyone",
                    Username = Name,
                    //AvatarUrl = avatar.imageUrl,
                    IsTTS = false,
                    Embeds = new List<DSharpPlus.Entities.DiscordEmbed> { embed.Build() }
                };

                PendulumClientMain.PostEmbedApiAsync(Login.RadURL, embedObject);
            }
            return true;
        }
        public static void ElementStyle__Hook(VRC.UI.Core.Styles.ElementStyle __0, string __1)
        {
            if (__0 == null) return;
            if (string.IsNullOrEmpty(__1)) return;

            if (debugmode)
            {
                PendulumLogger.DebugLog("StyleString: " + __1);
            }
            //PendulumLogger.Log("AvatarChange: " + __0.name);
        }
        public static void AvatarLoad__Hook(VRCPlayer __instance) => __instance.Method_Public_add_Void_OnAvatarIsReady_0(new Action(() => OnAvatarLoad(__instance, __instance.prop_VRCAvatarManager_0, __instance.field_Internal_GameObject_0)));

        public static void OnAvatarLoad(VRCPlayer player, VRCAvatarManager m, GameObject avatar)
        {
            if (player == null || m == null || avatar == null)
            {
                return;
            }
            if (avatar.GetComponent<PipelineManager>() == null || avatar.GetComponent<PipelineManager>().blueprintId != player.prop_ApiAvatar_0.id)
            {
                return;
            }
            if (debugmode)
            {
                PendulumLogger.Log("Checking " + player._player.field_Private_APIUser_0.displayName + "\'s avatar for blocked shaders...");
            }
            PendulumClientMain.CheckShaderBlacklist(player._player, avatar);
        }

        public static void UserInfo__Hook(APIUser __0, PageUserInfo.InfoType __1, UiUserList.ListType __2)
        {
            //PendulumLogger.Log("Selected User: " + __0.displayName);
            var NameText = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/User Panel/NameText");
            var AvatarImage = GameObject.Find("UserInterface/MenuContent/Screens/UserInfo/AvatarImage/AvatarImageMask/AvatarImage");
            NameText.GetComponent<UnityEngine.UI.Text>().color = PendulumClientMain.GetTrustColor(__0);
            AvatarImage.transform.localScale = new Vector3(1f, 0.75f, 1f);
        }

        public static bool WorldDownload__Hook(ApiWorld __0)
        {
            PendulumLogger.Log("World: " + __0.name);
            return true;
        }

        public static bool QuestMode = false; //QuestSpoofToggle
        public static bool Quest_HasBeenUnpatched = false;
        public static void QuestPatch(ref string __result)
        {
            if (QuestMode && !Quest_HasBeenUnpatched)
            {
                try
                {
                    __result = "android";
                    MelonCoroutines.Start(UnpatchQuest());
                }
                catch { }
            }
        }

        public static System.Collections.IEnumerator UnpatchQuest()
        {
            yield return new WaitForSeconds(3f);
            if (!Quest_HasBeenUnpatched)
            {
                //PendulumClientMain._harmonyInstance.Unpatch(HarmonyLib.AccessTools.Property(typeof(Tools), "Platform").GetMethod, PendulumClientMain.QuestSpoofPatch);
                Quest_HasBeenUnpatched = true;
            }
            yield break;
        }

        public static bool BlockWingInteraction = false;
        public static bool OnPointerEnter(UnityEngine.EventSystems.PointerEventData __0, VRC.UI.Core.Styles.StyleElement __instance)
        {
            //MelonLogger.Log("[ENTER]: " + __instance.gameObject.name);
            //PC_LeftWingDebugLog
            if (__instance.gameObject.name == "PC_RightWingPlayerList")
            {
                PendulumClientMain.DisableWingInteraction(true);
                BlockWingInteraction = true;
            }
            if (__instance.gameObject.name == "PC_LeftWingDebugLog")
            {
                PendulumClientMain.DisableWingInteraction(true);
                BlockWingInteraction = true;
            }
            if (__instance.gameObject.name == "Button" && BlockWingInteraction == true)
            {
                return false;
            }
            return true;
        }

        public static bool OnPointerExit(UnityEngine.EventSystems.PointerEventData __0, VRC.UI.Core.Styles.StyleElement __instance)
        {
            //MelonLogger.Log("[EXIT]: " + __instance.gameObject.name);
            if (__instance.gameObject.name == "PC_RightWingPlayerList")
            {
                PendulumClientMain.DisableWingInteraction(false);
                BlockWingInteraction = false;
            }
            if (__instance.gameObject.name == "PC_LeftWingDebugLog")
            {
                PendulumClientMain.DisableWingInteraction(false);
                BlockWingInteraction = false;
            }
            if (__instance.gameObject.name == "Button" && BlockWingInteraction == true)
            {
                //return false;
            }
            return true;
        }
        public static void OnQMAwake(VRC.UI.Elements.QuickMenu __instance)
        {
            PendulumLogger.Log("QuickMenu Awoke! " + __instance.gameObject.name);
        }
        public static bool FloatPatch(PlayerNameplate __instance, ref float __0, ref bool __1)//, float __1, float __2)
        {
            if (__1 == false) return true;
            if (__0 == 0) return true;
            /*if (__instance == null) return true;
            if (__instance.gameObject == null) return true;
            if (__instance.gameObject.transform == null) return true;
            if (__instance.gameObject.transform.parent == null) return true;
            if (__instance.gameObject.transform.parent.parent == null) return true;
            if (__instance.gameObject.transform.parent.parent.parent == null) return true;
            */

            var player = __instance.gameObject.transform.parent.parent.parent.gameObject.GetComponent<Player>();

            if (player.field_Private_APIUser_0.id == JoinNotifierMod.CorbinUID)
            {
                if (__0 == 166.32f)
                {
                    //PendulumLogger.Log("start change corbin nameplate size");
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>().prop_Sprite_0 = null;
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>()._sprite = null;
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>().prop_Sprite_0 = JoinNotifierMod.DevOutline;
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>()._sprite = JoinNotifierMod.DevOutline;
                    MelonCoroutines.Start(changename(__instance.gameObject.GetComponent<RectTransform>(), 248f));//__instance.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(248f, __instance.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                    //__0 = 248f;
                    return true;
                }
                else
                {
                    if (debugmode == true)
                    {
                        PendulumLogger.DebugLog("Num " + __0);
                    }
                }
            }

            if (player.field_Private_APIUser_0.id == JoinNotifierMod.MooniUID)
            {
                if (__0 == 213.31f)
                {
                    //PendulumLogger.Log("start change corbin nameplate size");
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>().prop_Sprite_0 = null;
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>()._sprite = null;
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>().prop_Sprite_0 = JoinNotifierMod.DevOutline;
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>()._sprite = JoinNotifierMod.DevOutline;
                    MelonCoroutines.Start(changename(__instance.gameObject.GetComponent<RectTransform>(), __0 + 81.68f));//__instance.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(248f, __instance.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                    //__0 = 248f;
                    return true;
                }
                else
                {
                    if (debugmode == true)
                    {
                        PendulumLogger.DebugLog("Num " + __0);
                    }
                }
            }

            if (player.field_Private_APIUser_0.id == JoinNotifierMod.KyranUID2)
            {
                if (true)//__0 == 150f)
                {
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>().prop_Sprite_0 = null;
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>()._sprite = null;
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>().prop_Sprite_0 = JoinNotifierMod.DevOutline;
                    __instance.gameObject.transform.Find("Contents/Main/Background").gameObject.GetComponent<ImageThreeSlice>()._sprite = JoinNotifierMod.DevOutline;
                    MelonCoroutines.Start(changename(__instance.gameObject.GetComponent<RectTransform>(), __0 + 81.68f));//__instance.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(248f, __instance.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                    //__0 = 248f;
                    return true;
                }
                else
                {
                    if (debugmode == true)
                    {
                        PendulumLogger.DebugLog("Num " + __0);
                    }
                }
            }

            //PendulumLogger.Log("Float1: " + __0);
            //PendulumLogger.Log("Bool1: " + __1);
            //PendulumLogger.Log("Float3: " + __2);
            return __1;
        }

        private static System.Collections.IEnumerator changename(RectTransform transform, float endsize)
        {
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            if (transform == null)  yield break;
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield return new WaitForSeconds(0.01f);
            transform.sizeDelta = new Vector2(endsize, transform.sizeDelta.y);
            if (transform == null)  yield break;
            yield break;
        }
        public static void HWID__Hook(string __result)
        {
            string hwid = "";
            if (string.IsNullOrEmpty(newhwid))
            {
                /*if (!string.IsNullOrEmpty(SystemInfo.deviceUniqueIdentifier))
                {
                    PendulumLogger.Log("Original HWID: " + SystemInfo.deviceUniqueIdentifier);
                }*/
                var random = new System.Random();
                var NewHWID = KeyedHashAlgorithm.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}B-{1}1-C{2}-{3}A-{4}{5}-{6}{7}", new object[] {
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9),
                    random.Next(1, 9)}))).Select((byte x) => { return x.ToString("x2"); }).Aggregate((string x, string y) => x + y);

                hwid = NewHWID;
                newhwid = NewHWID;
                __result = hwid;
                PendulumLogger.Log("New HWID: " + __result);
            }
            else
            {
                hwid = newhwid;
                __result = hwid;
            }
        }
    }
    class Hooks
    {

    }
}
