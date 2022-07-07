using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.Core;
using VRC.Networking;
using VRCFlatBuffers;
using VRC.SDKBase;
using VRC.Management;
using Transmtn;
using Transmtn.DTO;
using VRC.UI;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using System.Reflection;
using VRC;
using MelonLoader;
using UnhollowerBaseLib;
using System.Runtime.InteropServices;
using VRC.UserCamera;

namespace PendulumClient.Anti
{
    internal class HarmomyPatchManager
    {
        internal static void PatchHarmony(HarmonyLib.Harmony instance)
        {
            PendulumLogger.LogHarmony("Collecting methods...");
            /*var ModMethod1 = "_IsKickedFromWorld_b__0";
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
            var nameof(Prefixes.patch__3) = "patch__3";
            var nameof(Prefixes.patch__1) = "patch__1";
            var nameof(Prefixes.patch__4) = "patch__4";
            var nameof(Prefixes.patch__5) = "patch__5";
            var nameof(Prefixes.patch__10) = "patch__10";
            var nameof(Prefixes.patch__ping) = "patch__ping";
            //var AvatarChangePatch = "patch__avatar__logging";

            var nameof(Prefixes.patch__7) = "patch__7";
            var nameof(Prefixes.patch__8) = "patch__8";
            var nameof(Prefixes.patch__9) = "patch__9";
            var nameof(Prefixes.patch__enum__1) = "patch__enum__1";
            var nameof(Prefixes.patch__enum__2) = "patch__enum__2";
            var nameof(Prefixes.result__false) = "result__false";

            var UiListPatch = "UIList__Hook";
            var AvatarChangePatch = "AvatarChange__Hook";
            var UserInfoPatch = "UserInfo__Hook";
            var HWIDMethod = "GetDeviceUniqueIdentifier";
            var HWIDPatch = "HWID__Hook";*/

            //TODO xref scan these niggers
            var original2 = typeof(PortalInternal).GetMethod(nameof(PortalInternal.Method_Private_Void_1)); //DestroyPortal
            var original3 = typeof(PortalInternal).GetMethod(nameof(PortalInternal.Method_Private_Void_0)); //DestroyPortalNearSpawn

            var original28 = typeof(VRC_EventDispatcherRFC).GetMethod(nameof(VRC_EventDispatcherRFC.Method_Public_Void_Player_VrcEvent_VrcBroadcastType_Int32_Single_0));//GetMethods().Where(m => m.Name.Contains(EventMethod1));
            var original4 = typeof(RoomManager).GetMethod(nameof(RoomManager.Method_Public_Static_Boolean_ApiWorld_ApiWorldInstance_String_Int32_0));
            var original5 = typeof(VRC_EventHandler).GetMethod(nameof(VRC_EventHandler.InternalTriggerEvent));
            var original6 = typeof(Networking).GetMethod(nameof(Networking.GoToRoom));
            var original7 = typeof(PostOffice).GetMethod(nameof(PostOffice.Send));
            //var original8 = typeof(PhotonNetwork).GetMethod(PingMethod);
            var original9 = typeof(VRCPlayer).GetMethod(nameof(VRCPlayer.Method_Internal_Void_ApiAvatar_0));

            var Hook1 = typeof(UiUserList).GetMethod(nameof(UiUserList.Method_Protected_Virtual_Void_VRCUiContentButton_Object_0));
            var Hook1Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.UIList__Hook));
            var Hook2 = typeof(AssetBundleDownloadManager).GetMethods().Where(mi => mi.GetParameters().Length == 1 && mi.GetParameters().First().ParameterType == typeof(ApiAvatar) && mi.ReturnType == typeof(void));
            var Hook2Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.AvatarChange__Hook));
            var Hook3 = typeof(VRCPlayer).GetMethod(nameof(VRCPlayer.Awake));//typeof(VRCAvatarManager.MulticastDelegateNPublicSealedVoGaVRBoUnique).GetMethod(nameof(VRCAvatarManager.MulticastDelegateNPublicSealedVoGaVRBoUnique.Invoke));
            var Hook3Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.AvatarLoad__Hook));
            var Hook4 = typeof(PageUserInfo).GetMethod(nameof(PageUserInfo.Method_Public_Void_APIUser_InfoType_ListType_0));
            var Hook4Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.UserInfo__Hook));
            var Hook5 = typeof(SystemInfo).GetMethod(nameof(SystemInfo.GetDeviceUniqueIdentifier));
            var Hook5Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.HWID__Hook));

            var Hook6 = typeof(AssetBundleDownloadManager).GetMethod(nameof(AssetBundleDownloadManager.Method_Internal_Void_ApiWorld_PDM_0));
            var Hook6Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.WorldDownload__Hook));

            var NamePlateHook1 = typeof(PlayerNameplate).GetMethod(nameof(PlayerNameplate.Method_Private_Void_Single_Boolean_0));
            //var NamePlateHook2 = typeof(PlayerNameplate).GetMethod("Method_Private_Void_Single_Boolean_PDM_0");
            //var NamePlateHook3 = typeof(PlayerNameplate).GetMethod("Method_Private_Void_Single_Boolean_PDM_1");
            var NamePlatePatch1 = typeof(Prefixes).GetMethod(nameof(Prefixes.FloatPatch));

            var Hook7 = typeof(FlatBufferNetworkSerializer).GetMethod(nameof(FlatBufferNetworkSerializer.Method_Public_Void_EventData_0));
            var Hook7V2 = typeof(Photon.Realtime.LoadBalancingClient).GetMethod(nameof(Photon.Realtime.LoadBalancingClient.OnEvent));
            var Hook7V3 = typeof(VRCNetworkingClient).GetMethod(nameof(VRCNetworkingClient.OnEvent));
            //var Hook7V4 = typeof(VRC_EventLog.MonoBehaviour1NPublicObPrPrPrUnique).GetMethod("OnEvent");
            //var Hook7V3List = typeof(VRC_EventLog.MonoBehaviour1NPublicObPrPrPrUnique).GetMethods().Where(mi => mi.GetParameters().Length == 1 && mi.GetParameters().First().ParameterType == typeof(EventData) && mi.Name.StartsWith("Method_Public_Virtual_Final_New_Void_EventData_"));
            var Hook7Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.PhotonEvents));
            var Hook7PatchV2 = typeof(Prefixes).GetMethod(nameof(Prefixes.LBPhotonEvents));

            var Hook8 = typeof(VRC.UI.Core.Styles.StyleEngine).GetMethod(nameof(VRC.UI.Core.Styles.StyleEngine.Method_Public_Void_ElementStyle_String_0));
            var Hook8Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.ElementStyle__Hook));

            var Hook9 = typeof(VRC.UI.Elements.QuickMenu).GetMethod(nameof(VRC.UI.Elements.QuickMenu.OnDisable));
            var Hook9PrePatch = typeof(Prefixes).GetMethod(nameof(Prefixes.prepatch__QMOnClose));
            var Hook9PostPatch = typeof(Prefixes).GetMethod(nameof(Prefixes.postpatch__QMOnClose));

            var Hook10 = typeof(VRC.UI.Elements.QuickMenu).GetMethod(nameof(VRC.UI.Elements.QuickMenu.OnEnable));
            var Hook10PrePatch = typeof(Prefixes).GetMethod(nameof(Prefixes.prepatch__QMOnOpen));
            var Hook10PostPatch = typeof(Prefixes).GetMethod(nameof(Prefixes.postpatch__QMOnOpen));

            var Hook11 = typeof(PortalInternal).GetMethod(nameof(PortalInternal.ConfigurePortal));
            var Hook11PrePatch = typeof(Prefixes).GetMethod(nameof(Prefixes.prepatch__PortalAwake));
            var Hook11PostPatch = typeof(Prefixes).GetMethod(nameof(Prefixes.postpatch__PortalAwake));

            var Hook12 = typeof(VRCUiPageLoading).GetMethod(nameof(VRCUiPageLoading.OnEnable));
            var Hook12PostPatch = typeof(Prefixes).GetMethod(nameof(Prefixes.postpatch__OnLoading));

            /*var Hook13 = typeof(VRCAvatarManager).GetMethod(nameof(VRCAvatarManager.Method_Private_Void_5));
            var Hook13v2 = typeof(VRCAvatarManager).GetMethod(nameof(VRCAvatarManager.Method_Private_Void_2));
            var Hook14 = typeof(VRCAvatarManager).GetMethod(nameof(VRCAvatarManager.Method_Private_Boolean_GameObject_String_Single_String_0));
            var Hook15 = typeof(VRCAvatarManager).GetMethod(nameof(VRCAvatarManager.Method_Private_Void_Boolean_0));
            var Hook16 = typeof(VRCAvatarManager).GetMethod(nameof(VRCAvatarManager.Method_Private_Void_AvatarKind_0));
            var Hook17 = typeof(VRC_AnimationController).GetMethod(nameof(VRC_AnimationController.Reset));
            //var Hook16 = typeof(VRCAvatarManager).GetMethod(nameof(VRCAvatarManager.Method_Public_Void_Animator_byref_Vector3_byref_Vector3_byref_Vector3_byref_Vector3_0));
            var Hook13Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__false));
            var Hook14Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.patch_avatarVisibility));
            var Hook15Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.patch_avatarVisibilityBool));
            var Hook16Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.patch_AvatarKind));
            var Hook17Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.patch_AvatarReset));
           */
            //var Hook19 = typeof(RootMotion.FinalIK.IKSolverVR).GetMethods().Where(mi => mi.Name == nameof(RootMotion.FinalIK.IKSolverVR.IsValid) && mi.GetParameters().Length == 1);//.GetMethod(nameof(RootMotion.FinalIK.IKSolverVR.IsValid));
            //var Hook19Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.patch_IK));

            //what the fuck how the hell would this be anti block tf am i doin

            var Hook18 = typeof(VRCAvatarManager).GetMethod(nameof(VRCAvatarManager.Method_Private_Void_3));
            var Hook18Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.patch_Head));

            var Hook20 = GetPropertyMethod(typeof(Tools), nameof(Tools.Platform));//typeof(Tools).GetProperty(nameof(Tools.Platform));
            var Hook20Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.QuestPatch));
            //VRCAvatarManager.Method_Private_Void_LocalAvatarVisibility_0

            var Hook21 = typeof(VRC.UI.Elements.QuickMenu).GetMethod(nameof(VRC.UI.Elements.QuickMenu.Start));
            var Hook21Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.OnQMAwake));

            var Hook22 = typeof(AssetBundleDownloadManager).GetMethod(nameof(AssetBundleDownloadManager.Method_Internal_UniTask_1_InterfacePublicAbstractIDisposableGaObGaUnique_ApiAvatar_MulticastDelegateNInternalSealedVoUnUnique_Boolean_0));
            //var Hook22v2 = typeof(AssetBundleDownloadManager).GetMethod(nameof(AssetBundleDownloadManager.Method_Internal_Void_ApiAvatar_1));
            //var Hook22Patch = typeof(Prefixes).GetMethod(nameof(Prefixes.DownloadAvatarPatch));

            var Hook23 = typeof(API).GetMethod(nameof(API.SendGetRequest));
            var Hook23Patch = typeof(Anti.Patches.APIPatches).GetMethod(nameof(Anti.Patches.APIPatches.SendGetRequestPatch));

            //var AudioOnEndHook = typeof(AudioSource).GetMethod("get_" + nameof(AudioSource.isPlaying));
            var AudioOnEndHook = typeof(AudioSource).GetMethods().Where(mi => mi.GetParameters().Length == 0 && mi.Name == "Stop").First();
            var AudioOnEndPatch = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__AudioSourceOnEnd));

            //var testoriginal = typeof(VRCFlowManager.ObjectNPrivateSealedStObAcStOb1Ac1StStUnique).GetMethod(TestMethod);
            //var testoriginal2 = typeof(VRCFlowManager).GetMethod(TestMethod2);
            //moderation manager patches
            //var ModOrg1 = typeof(ModerationManager.ObjectNPrivateSealedStStUnique).GetMethod(ModMethod1);
            //var ModOrg2 = typeof(ModerationManager.ObjectNPrivateSealedMoVoBomAp2).GetMethod(ModMethod2);
            //var ModOrg3 = typeof(ModerationManager.ObjectNPrivateSealedMoVoBomAp1).GetMethod(ModMethod3);
            //var ModOrg4 = typeof(ModerationManager.ObjectNPrivateSealedMoVoBomAp0).GetMethod(ModMethod4);
            //var ModOrg5 = typeof(ModerationManager.__c).GetMethod(ModMethod5);
            //var ModOrg6 = typeof(ModerationManager.__c).GetMethod(ModMethod6);
            //var ModOrg7 = typeof(ModerationManager.__c).GetMethod(ModMethod7);
            //var ModOrg8 = typeof(ModerationManager.ObjectNPrivateSealedStAc1ApUnique).GetMethod(ModMethod8);
            //var ModOrg9 = typeof(ModerationManager.ObjectNPrivateSealedStAc1ApUnique).GetMethod(ModMethod9);
            //var ModOrg10 = typeof(ModerationManager.ObjectNPrivateSealedApVoBomAp1).GetMethod(ModMethod10);
            //var ModOrg11 = typeof(ModerationManager.ObjectNPrivateSealedIE1ApLiVo1ApBotmm1).GetMethod(ModMethod11);
            //var ModOrg12 = typeof(ModerationManager.ObjectNPrivateSealedIE1ApLiVo1ApBotmm1).GetMethod(ModMethod12);
            //var ModOrg13 = typeof(ModerationManager.ObjectNPrivateSealedApVoBomAp0).GetMethod(ModMethod13);
            //var ModOrg14 = typeof(ModerationManager.ObjectNPrivateSealedIE1ApLiVo1ApBotmm0).GetMethod(ModMethod14);
            //var ModOrg15 = typeof(ModerationManager.ObjectNPrivateSealedIE1ApLiVo1ApBotmm0).GetMethod(ModMethod15);
            //var ModOrg16 = typeof(ModerationManager.ObjectNPrivateSealedObAcUnique).GetMethod(ModMethod16);
            //var ModOrg17 = typeof(ModerationManager.ObjectNPrivateSealedObAcUnique).GetMethod(ModMethod17);
            //var ModOrg18 = typeof(ModerationManager.ObjectNPrivateSealedObAcUnique).GetMethod(ModMethod18);
            //var ModOrg19 = typeof(ModerationManager.ObjectNPrivateSealedObAcUnique).GetMethod(ModMethod19);

            var falseprefix = typeof(Prefixes).GetMethods().Where(m => m.GetParameters().Count() == 0).First();
            var CustomRPCPrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__3));
            var RoomPrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__1));
            var triggerprefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__4));
            var GoToRoomPrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__5));
            var AntiKickPrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__6));
            var NotiPrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__10));
            var PingPrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__ping));
            var AvatarChangePrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.AvatarChange__Hook));

            var ApiModBoolPrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__7));
            var ApiPlrModBoolPrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__8));
            var ApiModContPrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__9));
            var ApiModEnumPrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__enum__1));
            var ApiPlrModEnumPrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.patch__enum__2));
            var ResultFalsePrefix = typeof(Prefixes).GetMethod(nameof(Prefixes.result__false));


            PendulumLogger.LogHarmony("Patching methods...");

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
            //instance.Patch(ModOrg1, new HarmonyMethod(AntiKickPrefix), null, null);
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
            //instance.Patch(Hook2, new HarmonyMethod(Hook2Patch));
            instance.Patch(Hook6, new HarmonyMethod(Hook6Patch));
            instance.Patch(Hook7, new HarmonyMethod(Hook7Patch));
            //instance.Patch(Hook7V2, new HarmonyMethod(Hook7PatchV2));
            //instance.Patch(Hook7V3, new HarmonyMethod(Hook7Patch));
            /*foreach (var method in Hook7V3List)
            {
                instance.Patch(method, new HarmonyMethod(Hook7Patch));
            }*/
            instance.Patch(Hook8, new HarmonyMethod(Hook8Patch));
            instance.Patch(Hook9, new HarmonyMethod(Hook9PrePatch), new HarmonyMethod(Hook9PostPatch));
            //instance.Patch(Hook10, new HarmonyMethod(Hook10PrePatch), new HarmonyMethod(Hook10PostPatch));
            instance.Patch(Hook11, new HarmonyMethod(Hook11PrePatch), new HarmonyMethod(Hook11PostPatch));
            instance.Patch(Hook12, null, new HarmonyMethod(Hook12PostPatch));
            instance.Patch(AudioOnEndHook, new HarmonyMethod(AudioOnEndPatch));
            /*instance.Patch(Hook13, new HarmonyMethod(Hook13Patch));
            instance.Patch(Hook13v2, new HarmonyMethod(Hook13Patch));
            instance.Patch(Hook14, new HarmonyMethod(Hook14Patch));
            instance.Patch(Hook15, new HarmonyMethod(Hook15Patch));
            instance.Patch(Hook16, new HarmonyMethod(Hook16Patch));
            instance.Patch(Hook17, new HarmonyMethod(Hook17Patch));*/
            instance.Patch(Hook18, new HarmonyMethod(Hook18Patch));
            /*foreach(var hook in Hook19)
            {
                instance.Patch(hook, new HarmonyMethod(Hook19Patch));
            }*/
            instance.Patch(Hook20, new HarmonyMethod(Hook20Patch));
            instance.Patch(Hook21, null, new HarmonyMethod(Hook21Patch));

            //instance.Patch(Hook22, new HarmonyMethod(Hook22Patch));
            instance.Patch(Hook23, new HarmonyMethod(Hook23Patch));
            //instance.Patch(Hook22v2, new HarmonyMethod(Hook22Patch));
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


            if (Prefixes.debugmode)
            {
                var methods = instance.GetPatchedMethods();
                foreach (var method in methods)
                {
                    if (!string.IsNullOrEmpty(method.Name) && !method.Name.ToLower().Contains("get_") && !method.Name.ToLower().Contains("set_")) PendulumLogger.Log(ConsoleColor.DarkGray, "[Harmony] Patched Method: {0}", method.Name);
                }
            }
            try
            {
                unsafe
                {
                    var originalMethodPointer = *(IntPtr*)(IntPtr)UnhollowerUtils
                        .GetIl2CppMethodInfoPointerFieldForGeneratedMethod(typeof(AssetBundleDownloadManager).GetMethod(
                            nameof(AssetBundleDownloadManager.Method_Internal_UniTask_1_InterfacePublicAbstractIDisposableGaObGaUnique_ApiAvatar_MulticastDelegateNInternalSealedVoUnUnique_Boolean_0)))
                        .GetValue(null);

                    MelonUtils.NativeHookAttach((IntPtr)(&originalMethodPointer), typeof(Prefixes).GetMethod(nameof(Prefixes.DownloadAvatarPatch), BindingFlags.Static | BindingFlags.Public).MethodHandle.GetFunctionPointer());

                    dgAttemptAvatarDownload = Marshal.GetDelegateForFunctionPointer<AttemptAvatarDownloadDelegate>(originalMethodPointer);
                    //PendulumLogger.Log(ConsoleColor.Green, $"Successfully able to hook AssetBundleDownLoadManager");
                    if (Prefixes.debugmode) PendulumLogger.Log(ConsoleColor.DarkGray, "[Harmony] Patched Method: {0}", "AssetBundleDownloadManager.Method_Internal_UniTask_1_InterfacePublicAbstractIDisposableGaObGaUnique_ApiAvatar_MulticastDelegateNInternalSealedVoUnUnique_Boolean_0");
                }
            }
            catch (Exception e)
            {
                PendulumLogger.LogErrorSevere("Unable to hook AssetBundleDownloadManager: " + e.ToString());
            }

            //PendulumLogger.Log(ConsoleColor.Green, "[Harmony] Patched Harmony Instance!");
            var methodcount = instance.GetPatchedMethods().Count();
            methodcount += 37;
            PendulumLogger.LogHarmony($"Successfully patched {methodcount} methods!");

            if (Main.PendulumClientMain.FirstTimeinit == true && Main.PendulumClientMain.LogoDataDownloaded == true)
            {
                PendulumLogger.Log("First Time Initalization Detected.");
                PendulumLogger.Log("Restarting game to apply Assets.");
                Main.PendulumClientMain.ForceRestart();
            }
            //Setupnameof(Prefixes.patch__3)es();
        }

        private static MethodInfo GetPropertyMethod(Type inClass, string method)
        {
            return AccessTools.Property(inClass, method).GetMethod;
        }

        public unsafe delegate IntPtr AttemptAvatarDownloadDelegate(IntPtr hiddenValueTypeReturn, IntPtr thisPtr, IntPtr apiAvatarPtr, IntPtr multicastDelegatePtr, bool idfk, IntPtr nativeMethodInfo);
        public static AttemptAvatarDownloadDelegate dgAttemptAvatarDownload;
    }
}
