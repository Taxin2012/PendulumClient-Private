using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using PendulumClient.UI;
using PendulumClient.Main;
using MelonLoader;
using PendulumClient.Wrapper;
using Transmtn.DTO.Notifications;
using Transmtn;
using Transmtn.DTO;

namespace PendulumClient.ButtonAPIV2
{
    class MenuFunctions
    {
        public static Anti.ret_9 _ret { get; set; }
        public static Anti.ret_209 _ret2 { get; set; }
        public static Anti.ret_1 _ret3 { get; set; }


        public static object dsobj = null;
        public static object ds2obj = null;
        public static object usobj = null;

        public static bool OpenFileOnDownload = false;
        public static bool PlayerESPon = false;

        public static int copyVoice_photonId = 0;
        public static void PlayerESP(bool esp)
        {
            if (esp)
            {
                var allplayers = PlayerWrappers.GetAllPlayers();
                foreach (VRC.Player player in allplayers)
                {
                    if (player.gameObject.transform.Find("SelectRegion") != null)
                    {
                        Wrappers.EnableOutline(player.gameObject.transform.Find("SelectRegion").gameObject.GetComponent<MeshRenderer>(), ColorModule.ColorModule.CachedColor);
                    }
                }
                PlayerESPon = true;
                PendulumClientMain.VRC_UIManager.QueueHudMessage("PlayerESP Enabled");
            }
            else
            {
                var allplayers = PlayerWrappers.GetAllPlayers();
                foreach (VRC.Player player in allplayers)
                {
                    if (player.gameObject.transform.Find("SelectRegion") != null)
                    {
                        Wrappers.DisableOutline(player.gameObject.transform.Find("SelectRegion").gameObject.GetComponent<MeshRenderer>());
                    }
                }
                PlayerESPon = false;
                PendulumClientMain.VRC_UIManager.QueueHudMessage("PlayerESP Disabled");
            }
        }

        public static void SmallEvent9(int amount = 80)
        {
            if (amount < 2)
                return;

            if (amount > 80)
                amount = 80;

            byte[] LagData = new byte[] {
                   65, 13, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 128, 26, 1, 241, 0, 0, 0, 63, 98, 40, 62, 128, 225, 163, 187, 203, 81, 21, 65, 255, 69, 255, 31, 75, 0, 0, 0, 243, 0, 0, 0, 112, 180, 171, 190, 128, 225, 163, 187, 247, 139, 21, 65, 255, 69, 255, 31, 75, 0, 0, 0, 195, 0, 8, 32, 145, 0, 11, 30, 1, 6, 5, 4, 0, 28, 8, 0, 7, 11, 128, 3, 0, 231, 70, 16, 23, 51, 89, 216, 201, 51, 53, 98, 149, 111, 127, 153, 0, 51, 122, 76, 89, 0, 45, 95, 100, 0, 0, 71, 43, 180, 57, 33, 52, 250, 33, 55, 32, 255, 138, 6, 242, 56, 6, 37, 158, 39, 181, 177, 228, 41, 91, 175, 12, 41, 251, 38, 173, 172, 82, 160, 6, 38, 90, 187, 173, 50, 26, 46, 137, 56, 6, 178, 124, 180, 101, 59, 22, 47, 77, 170, 239, 48, 88, 146, 44, 52, 235, 55, 64, 56, 55, 55, 161, 172, 221, 49, 151, 27, 252, 151, 39, 176, 155, 49, 232, 180, 105, 52, 11, 181, 174, 180, 65, 35, 36, 175, 19, 59, 230, 185, 223, 180, 103, 184, 166, 52, 10, 50, 85, 53, 138, 48, 210, 45, 118, 185
                };
            int idfirst2 = int.Parse(VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.playerId + "00001");
            byte[] IDOut2 = BitConverter.GetBytes(idfirst2);
            Buffer.BlockCopy(IDOut2, 0, LagData, 0, 4);
            if (Anti.Prefixes.debugmode)
            {
                PendulumLogger.DebugLog("EVENT 9 STARTED");
            }
            for (int i = 0; i < amount; i++)
            {
                Anti.PhotonExtensions.OpRaiseEvent(9, LagData, new Photon.Realtime.RaiseEventOptions
                {
                    field_Public_ReceiverGroup_0 = Photon.Realtime.ReceiverGroup.Others,
                    field_Public_EventCaching_0 = Photon.Realtime.EventCaching.DoNotCache
                }, ExitGames.Client.Photon.SendOptions.SendReliable);
            }
        }
        public static void EnableEvent9(bool t)
        {
            if (t)
            {
                if (_ret == null)
                    _ret = new Anti.ret_9();

                _ret.toggled = true;
                if (!_ret.started)
                {
                    dsobj = MelonCoroutines.Start(_ret.Desync());
                }
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Event9 Enabled");
            }
            else
            {
                _ret.toggled = false;
                if (_ret.started && dsobj != null)
                {
                    MelonCoroutines.Stop(dsobj);
                }
                _ret = null;
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Event9 Disabled");
            }
        }
        public static void EnableEvent209(bool t)
        {
            if (t)
            {
                if (_ret2 == null)
                    _ret2 = new Anti.ret_209();

                _ret2.toggled = true;
                if (!_ret2.started)
                {
                    ds2obj = MelonCoroutines.Start(_ret2.Desync());
                }
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Event209 Enabled");
            }
            else
            {
                _ret2.toggled = false;
                if (_ret2.started && ds2obj != null)
                {
                    MelonCoroutines.Stop(ds2obj);
                }
                _ret2 = null;
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Event209 Disabled");
            }
        }

        public static void EnableEventBigUspeak(bool t)
        {
            if (t)
            {
                if (_ret3 == null)
                    _ret3 = new Anti.ret_1();

                _ret3.toggled = true;
                if (!_ret3.started)
                {
                    usobj = MelonCoroutines.Start(_ret3.Desync());
                }
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Big USpeak Enabled");
            }
            else
            {
                _ret3.toggled = false;
                if (_ret3.started && usobj != null)
                {
                    MelonCoroutines.Stop(usobj);
                }
                _ret3 = null;
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Big USpeak Disabled");
            }
        }

        public static void OpenFolder(string folderPath)
        {
            if (System.IO.Directory.Exists(folderPath))
            {
                string p = "";
                foreach (var chr in folderPath)
                {
                    char newchr = '0';
                    if (chr == '/')
                    {
                        newchr = '\\';
                    }
                    else
                    {
                        newchr = chr;
                    }
                    p += newchr;
                }

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    Arguments = p,
                    FileName = "explorer.exe"
                };

                System.Diagnostics.Process.Start(startInfo);
            }
        }

        public static System.Collections.IEnumerator BigUSpeak()
        {
            var data1 = new byte[] { 0, 1, 0, 0, 0, 255, 255, 255, 255, 1, 0, 0, 0, 0, 0, 0, 0, 15, 1, 0, 0, 0, 87, 0, 0, 0, 2, 2, 0, 0, 0, 111, 154, 208, 186, 252, 167, 75, 0, 120, 177, 31, 49, 190, 54, 13, 96, 212, 155, 205, 151, 91, 194, 253, 195, 89, 246, 218, 134, 30, 160, 127, 17, 160, 189, 184, 243, 104, 187, 128, 210, 92, 3, 117, 186, 133, 9, 1, 72, 184, 80, 240, 222, 27, 173, 56, 120, 23, 97, 9, 42, 240, 113, 51, 96, 83, 31, 139, 161, 47, 150, 36, 35, 115, 220, 94, 69, 108, 241, 134, 26, 51, 190, 94, 11 };
            PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data1, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
            yield return new WaitForSecondsRealtime(0.025f);
            var data2 = new byte[] { 0, 1, 0, 0, 0, 255, 255, 255, 255, 1, 0, 0, 0, 0, 0, 0, 0, 15, 1, 0, 0, 0, 163, 0, 0, 0, 2, 2, 0, 0, 0, 175, 154, 208, 186, 253, 167, 46, 0, 120, 147, 231, 168, 223, 79, 31, 84, 232, 191, 123, 215, 43, 227, 47, 57, 250, 38, 172, 77, 145, 122, 109, 230, 137, 83, 190, 167, 103, 109, 196, 8, 185, 249, 243, 133, 217, 131, 133, 219, 190, 156, 29, 253, 63, 73, 254, 167, 42, 0, 120, 145, 211, 213, 70, 100, 135, 12, 166, 74, 127, 145, 64, 161, 212, 136, 112, 210, 165, 83, 177, 76, 114, 129, 153, 214, 241, 70, 210, 61, 162, 205, 245, 151, 58, 4, 160, 229, 236, 1, 3, 9, 255, 167, 55, 0, 120, 132, 122, 110, 99, 189, 97, 217, 182, 72, 116, 22, 218, 11, 79, 204, 151, 230, 15, 39, 78, 43, 209, 151, 245, 231, 237, 214, 189, 221, 162, 178, 120, 121, 63, 181, 67, 24, 239, 242, 124, 43, 233, 112, 156, 109, 253, 164, 194, 212, 39, 206, 147, 231, 186, 11 };
            PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data2, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
            yield return new WaitForSecondsRealtime(0.025f);
            var data3 = new byte[] { 0, 1, 0, 0, 0, 255, 255, 255, 255, 1, 0, 0, 0, 0, 0, 0, 0, 15, 1, 0, 0, 0, 154, 0, 0, 0, 2, 2, 0, 0, 0, 248, 154, 208, 186, 0, 168, 45, 0, 120, 132, 9, 185, 171, 163, 236, 50, 237, 225, 189, 145, 67, 179, 65, 81, 224, 56, 253, 154, 114, 100, 224, 168, 118, 174, 226, 182, 82, 211, 218, 187, 192, 79, 173, 122, 157, 228, 151, 99, 190, 106, 138, 247, 3, 1, 168, 43, 0, 120, 61, 0, 52, 156, 230, 219, 29, 186, 245, 218, 102, 83, 8, 87, 174, 164, 93, 99, 24, 59, 27, 198, 17, 117, 42, 212, 60, 8, 106, 71, 245, 46, 165, 176, 171, 139, 39, 162, 250, 233, 213, 22, 2, 168, 46, 0, 120, 59, 85, 53, 173, 75, 140, 59, 247, 202, 222, 101, 200, 181, 228, 80, 20, 160, 132, 208, 19, 16, 219, 45, 68, 26, 49, 4, 125, 168, 245, 43, 160, 65, 130, 35, 233, 226, 208, 57, 129, 163, 125, 3, 150, 14, 11 };
            PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data3, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
            yield return new WaitForSecondsRealtime(0.025f);
            var data4 = new byte[] { 0, 1, 0, 0, 0, 255, 255, 255, 255, 1, 0, 0, 0, 0, 0, 0, 0, 15, 1, 0, 0, 0, 40, 1, 0, 0, 2, 2, 0, 0, 0, 65, 155, 208, 186, 3, 168, 43, 0, 120, 59, 84, 246, 254, 17, 188, 62, 90, 74, 239, 116, 164, 231, 227, 196, 163, 165, 119, 63, 226, 224, 53, 0, 7, 78, 77, 224, 128, 209, 124, 188, 8, 148, 224, 202, 63, 234, 165, 212, 63, 111, 235, 4, 168, 49, 0, 120, 142, 44, 201, 11, 236, 176, 75, 189, 248, 228, 186, 29, 189, 34, 189, 161, 10, 188, 145, 250, 36, 155, 254, 187, 145, 125, 198, 249, 67, 69, 216, 234, 40, 197, 88, 177, 83, 184, 115, 147, 201, 132, 186, 77, 189, 130, 16, 9, 5, 168, 67, 0, 120, 188, 13, 52, 56, 122, 91, 233, 51, 88, 95, 21, 237, 202, 127, 250, 233, 72, 89, 39, 245, 208, 149, 101, 80, 78, 19, 84, 191, 18, 96, 87, 222, 59, 28, 83, 188, 124, 99, 171, 235, 23, 50, 191, 79, 232, 231, 115, 212, 90, 33, 66, 172, 172, 172, 44, 170, 37, 130, 58, 124, 92, 98, 198, 63, 110, 240, 6, 168, 62, 0, 120, 189, 70, 95, 52, 202, 238, 82, 157, 14, 117, 101, 67, 157, 160, 213, 68, 178, 117, 216, 219, 163, 245, 51, 11, 53, 10, 68, 21, 29, 248, 61, 253, 225, 144, 23, 168, 133, 180, 66, 143, 159, 240, 200, 199, 69, 4, 154, 22, 31, 117, 154, 171, 179, 213, 192, 229, 253, 75, 241, 250, 40, 7, 168, 47, 0, 120, 189, 75, 239, 47, 127, 163, 170, 196, 148, 33, 152, 128, 200, 174, 135, 61, 42, 103, 77, 41, 149, 29, 79, 35, 68, 36, 211, 202, 101, 69, 150, 115, 183, 162, 206, 253, 174, 219, 199, 217, 165, 227, 204, 211, 8, 236, 11 };
            PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data4, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
            yield return new WaitForSecondsRealtime(0.025f);
            var data5 = new byte[] { 0, 1, 0, 0, 0, 255, 255, 255, 255, 1, 0, 0, 0, 0, 0, 0, 0, 15, 1, 0, 0, 0, 111, 0, 0, 0, 2, 2, 0, 0, 0, 130, 155, 208, 186, 8, 168, 51, 0, 120, 189, 67, 160, 172, 253, 93, 68, 39, 182, 243, 220, 39, 85, 231, 108, 247, 197, 247, 247, 61, 253, 197, 84, 235, 101, 249, 145, 232, 207, 142, 17, 167, 140, 53, 214, 94, 212, 60, 112, 185, 194, 38, 16, 42, 223, 89, 51, 221, 150, 225, 9, 168, 44, 0, 120, 189, 67, 148, 184, 249, 63, 132, 77, 48, 161, 185, 39, 2, 41, 144, 126, 53, 114, 157, 212, 121, 148, 191, 8, 99, 155, 45, 14, 80, 68, 40, 218, 211, 206, 145, 47, 92, 253, 195, 177, 239, 172, 106, 11 };
            PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data5, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
            yield return new WaitForSecondsRealtime(0.025f);
            var data6 = new byte[] { 0, 1, 0, 0, 0, 255, 255, 255, 255, 1, 0, 0, 0, 0, 0, 0, 0, 15, 1, 0, 0, 0, 3, 1, 0, 0, 2, 2, 0, 0, 0, 195, 155, 208, 186, 10, 168, 51, 0, 120, 189, 66, 219, 82, 245, 206, 79, 206, 185, 146, 162, 79, 223, 46, 89, 51, 20, 182, 240, 117, 10, 78, 231, 155, 179, 76, 232, 100, 27, 104, 161, 145, 59, 23, 127, 147, 243, 246, 147, 229, 4, 140, 146, 54, 148, 115, 208, 131, 4, 223, 11, 168, 53, 0, 120, 189, 65, 194, 32, 241, 198, 154, 22, 122, 250, 223, 141, 128, 140, 133, 152, 130, 92, 35, 136, 165, 183, 168, 21, 85, 43, 240, 84, 87, 50, 99, 168, 57, 58, 244, 203, 156, 67, 176, 86, 84, 253, 56, 99, 128, 64, 92, 56, 115, 97, 104, 233, 12, 168, 59, 0, 120, 189, 59, 152, 200, 39, 224, 112, 2, 114, 81, 248, 101, 148, 132, 111, 46, 244, 55, 31, 64, 11, 49, 22, 129, 247, 157, 122, 67, 56, 170, 55, 40, 245, 152, 12, 235, 158, 46, 164, 40, 165, 180, 98, 249, 164, 149, 194, 5, 30, 88, 159, 79, 249, 158, 252, 130, 249, 181, 13, 168, 72, 0, 120, 189, 70, 95, 25, 130, 207, 31, 246, 226, 86, 70, 221, 24, 13, 163, 146, 226, 6, 30, 85, 191, 128, 109, 114, 197, 224, 105, 90, 230, 38, 253, 105, 133, 6, 125, 62, 81, 64, 100, 194, 244, 140, 151, 167, 215, 242, 165, 249, 164, 221, 154, 207, 51, 234, 32, 227, 198, 141, 85, 195, 188, 45, 211, 99, 179, 150, 65, 21, 246, 29, 33, 11 };
            PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data6, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
            yield return new WaitForSecondsRealtime(0.025f);
            var data7 = new byte[] { 0, 1, 0, 0, 0, 255, 255, 255, 255, 1, 0, 0, 0, 0, 0, 0, 0, 15, 1, 0, 0, 0, 40, 1, 0, 0, 2, 2, 0, 0, 0, 65, 155, 208, 186, 3, 168, 43, 0, 120, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 245, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
            PendulumLogger.Log("USpeak Array Size: " + data7.Length);
            PendulumLogger.Log("USpeak Packet Size: " + (data7.Length - 40).ToString());
            PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data7, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
            yield return new WaitForSecondsRealtime(0.025f);
        }

        public static bool IsCopyingVoice = false;
        public static void CopyVoice(bool enable, VRC.Player player)
        {
            if (enable)
            {
                IsCopyingVoice = true;
                copyVoice_photonId = player.field_Private_VRCPlayerApi_0.playerId;
            }
            else
            {
                IsCopyingVoice = false;
                copyVoice_photonId = 0;
            }
        }

        public static void DropItems()
        {
            foreach (VRCSDK2.VRC_Pickup vrc_Pickup in Resources.FindObjectsOfTypeAll<VRCSDK2.VRC_Pickup>())
            {
                if (vrc_Pickup.IsHeld)
                {
                    PendulumClientMain.TakeOwnershipIfNecessary(vrc_Pickup.gameObject);
                    vrc_Pickup.Drop();
                }
            }

            foreach (VRC.SDK3.Components.VRCPickup vrc_Pickup in Resources.FindObjectsOfTypeAll<VRC.SDK3.Components.VRCPickup>())
            {
                if (vrc_Pickup.IsHeld)
                {
                    PendulumClientMain.TakeOwnershipIfNecessary(vrc_Pickup.gameObject);
                    vrc_Pickup.Drop();
                }
            }
        }

        public static VRC.Player GetSelectedPlayer()
        {
            var selectedUser = NewQuickMenu.Instance.field_Private_UIPage_1.Cast<MonoBehaviour1PublicBoRaGaObGaBuObToGaBuUnique>().field_Private_IUser_0; //VRC.UI.Elements.Menus.SelectedUserMenuQM
            if (selectedUser == null)
                return null;

            var player = VRC.PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(selectedUser.prop_String_0);
            return player;
        }

        public static VRC.Player GetSelectedUser()
        {
            if (!string.IsNullOrEmpty(PendulumClientMain.StoredUserID))
            {
                var player = VRC.PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(PendulumClientMain.StoredUserID);
                if (player != null)
                    return player;

                PendulumClientMain.StoredUserID = "";
            }
            return null;
        }
    }
}
