using System;
using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PendulumClient.Anti
{
    public class ret_9
    {
        public bool toggled { get; set; }
        public bool started { get; set; }
        public IEnumerator Desync()
        {
            /*byte[] LagData = new byte[] {
                   65, 13, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 128, 26, 1, 241, 0, 0, 0, 63, 98, 40, 62, 128, 225, 163, 187, 203, 81, 21, 65, 255, 69, 255, 31, 75, 0, 0, 0, 243, 0, 0, 0, 112, 180, 171, 190, 128, 225, 163, 187, 247, 139, 21, 65, 255, 69, 255, 31, 75, 0, 0, 0, 195, 0, 8, 32, 145, 0, 11, 30, 1, 6, 5, 4, 0, 28, 8, 0, 7, 11, 128, 3, 0, 231, 70, 16, 23, 51, 89, 216, 201, 51, 53, 98, 149, 111, 127, 153, 0, 51, 122, 76, 89, 0, 45, 95, 100, 0, 0, 71, 43, 180, 57, 33, 52, 250, 33, 55, 32, 255, 138, 6, 242, 56, 6, 37, 158, 39, 181, 177, 228, 41, 91, 175, 12, 41, 251, 38, 173, 172, 82, 160, 6, 38, 90, 187, 173, 50, 26, 46, 137, 56, 6, 178, 124, 180, 101, 59, 22, 47, 77, 170, 239, 48, 88, 146, 44, 52, 235, 55, 64, 56, 55, 55, 161, 172, 221, 49, 151, 27, 252, 151, 39, 176, 155, 49, 232, 180, 105, 52, 11, 181, 174, 180, 65, 35, 36, 175, 19, 59, 230, 185, 223, 180, 103, 184, 166, 52, 10, 50, 85, 53, 138, 48, 210, 45, 118, 185
                };*/
            byte[] LagData = new byte[8];
            int idfirst2 = int.Parse(VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_VRCPlayerApi_0.playerId + "00001");
            byte[] IDOut2 = BitConverter.GetBytes(idfirst2);
            Buffer.BlockCopy(IDOut2, 0, LagData, 0, 4);
            if (Prefixes.debugmode)
            {
                PendulumLogger.DebugLog("EVENT 9 STARTED");
            }
            started = true;
            while (toggled)
            {
                for (int i = 0; i < 80; i++)
                {
                    PhotonExtensions.OpRaiseEvent(9, LagData, new Photon.Realtime.RaiseEventOptions
                    {
                        field_Public_ReceiverGroup_0 = Photon.Realtime.ReceiverGroup.Others,
                        field_Public_EventCaching_0 = Photon.Realtime.EventCaching.DoNotCache
                    }, SendOptions.SendReliable);
                }
                yield return new WaitForSecondsRealtime(0.1f);
            }
            yield break;
        }
    }
    public class ret_209
    {
        public bool toggled { get; set; }
        public bool started { get; set; }
        public IEnumerator Desync()
		{
			PhotonView[] photonViews = Resources.FindObjectsOfTypeAll<PhotonView>();
            if (Prefixes.debugmode)
            {
                PendulumLogger.DebugLog("EVENT 209 STARTED");
            }
            started = true;
            while (toggled)
			{
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < photonViews.Length; j++)
					{
						PhotonExtensions.OpRaiseEvent(209, new int[]
						{
							photonViews[j].viewIdField,
							VRC.Player.Method_Internal_Static_Player_0().Method_Public_VRCPlayerApi_0().playerId
						}, new RaiseEventOptions
						{
							field_Public_ReceiverGroup_0 = (ReceiverGroup)1,
							field_Public_EventCaching_0 = 0
						}, default(SendOptions));
					}
				}
				yield return new WaitForSecondsRealtime(0.025f);
			}
			yield break;
		}
    }
}
