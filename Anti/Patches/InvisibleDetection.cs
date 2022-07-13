using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Reflection;
using VRC.SDKBase;

namespace PendulumClient.Anti.Patches
{
	internal class InvisibleDetection : PendulumPatch
	{
		private static readonly System.Collections.Generic.Dictionary<int, float> playerInvisibleCheck = new System.Collections.Generic.Dictionary<int, float>();

		internal static bool isConnectedToInstance = false;
		internal static bool moderationEventsEnabled = false;


		internal override void SetupPatches()
		{
			PatchMethod(typeof(NetworkManager).GetMethod("OnJoinedRoom"), null, typeof(InvisibleDetection).GetMethod(nameof(InvisibleDetection.OnJoinedRoomPatch), BindingFlags.Static | BindingFlags.NonPublic));
			PatchMethod(typeof(NetworkManager).GetMethod("OnLeftRoom"), null, typeof(InvisibleDetection).GetMethod(nameof(InvisibleDetection.OnLeftRoomPatch), BindingFlags.Static | BindingFlags.NonPublic));

			PatchMethod((from mb in typeof(NetworkManager).GetMethods()
						 where mb.Name.StartsWith("Method_Public_Void_Player_") && CheckMethod(mb, "OnPlayerJoined")
						 select mb).First(), null, typeof(InvisibleDetection).GetMethod(nameof(InvisibleDetection.OnPlayerJoinPatch), BindingFlags.Static | BindingFlags.NonPublic));
			PatchMethod((from mb in typeof(NetworkManager).GetMethods()
						 where mb.Name.StartsWith("Method_Public_Void_Player_") && CheckMethod(mb, "OnPlayerLeft")
						 select mb).Last(), null, typeof(InvisibleDetection).GetMethod(nameof(InvisibleDetection.OnPlayerLeavePatch), BindingFlags.Static | BindingFlags.NonPublic));
		}
		internal override void OnUpdate()
		{
			if (isConnectedToInstance)
				CheckForInvisiblePlayers();
		}

		private static void OnJoinedRoomPatch()
		{
			isConnectedToInstance = true;
		}

		private static void OnLeftRoomPatch()
		{
			isConnectedToInstance = false;
		}

		private static void OnPlayerJoinPatch(ref VRC.Player __0)
		{
			JoinNotifier.JoinNotifierMod.OnPlayerJoined(__0);
			if (__0.field_Private_APIUser_0.id == VRC.Core.APIUser.CurrentUser.id)
				MelonCoroutines.Start(DelayModerationEventsEnumerator());
		}
		private static void OnPlayerLeavePatch(ref VRC.Player __0)
		{
			JoinNotifier.JoinNotifierMod.OnPlayerLeft(__0);
			if (__0.field_Private_APIUser_0.id == VRC.Core.APIUser.CurrentUser.id)
			{
				moderationEventsEnabled = false;
				NotificationManagerPatch.firstTimeFriendRequestsReceived = false;
			}
		}
		private static IEnumerator DelayModerationEventsEnumerator()
		{
			yield return new WaitForSeconds(1.5f);
			moderationEventsEnabled = true;
			NotificationManagerPatch.firstTimeFriendRequestsReceived = true;
		}

		private void CheckForInvisiblePlayers()
		{
			if (PhotonNetwork.prop_Room_0 == null)
			{
				return;
			}
			Il2CppSystem.Collections.Generic.Dictionary<int, Player>.Enumerator enumerator = PhotonNetwork.prop_Room_0.prop_Dictionary_2_Int32_Player_0.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Il2CppSystem.Collections.Generic.KeyValuePair<int, Player> current = enumerator.Current;
				if (current.Value.field_Public_Player_0 != null)
				{
					continue;
				}
				if (!playerInvisibleCheck.ContainsKey(current.Value.field_Private_Int32_0))
				{
					playerInvisibleCheck.Add(current.Value.field_Private_Int32_0, 10f);
				}
				else if (playerInvisibleCheck[current.Value.field_Private_Int32_0] > 0f)
				{
					playerInvisibleCheck[current.Value.field_Private_Int32_0] -= Time.deltaTime;
				}
				else if (current.Value.field_Private_Hashtable_0.ContainsKey("user"))
				{
					Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object> dictionary = current.Value.field_Private_Hashtable_0["user"].Cast<Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object>>();
					string displayName = new Il2CppSystem.String(dictionary["displayName"].Pointer);
					string UserID = new Il2CppSystem.String(dictionary["id"].Pointer);
					var arrayptr = new Il2CppSystem.Array(dictionary["tags"].Pointer);
					if (!JoinNotifier.JoinNotifierMod.CheckNotification(displayName, "joinedInvisible"))
					{
						JoinNotifier.JoinNotifierMod.ToggleNotification(displayName, "joinedInvisible", state: true);
						var tags = new System.Collections.Generic.List<string>();
						foreach (var value in arrayptr)
						{
							string tag = new Il2CppSystem.String(value.Pointer);
							//PendulumLogger.DebugLog($"Tag: {tag}");
							tags.Add(tag);
						}
						var trustrank = Wrapper.PlayerWrappers.GetUserTrustFromTags(tags);
						var namecolored = QMLogAndPlayerlist.PlayerListFunctions.GetNameColoredQuestFromTrust(trustrank, displayName, UserID);
						UI.NotificationsV2.AddNotification($"{namecolored} <color=#969696>joined while invisible.</color>", Color.yellow);
						QMLogAndPlayerlist.DebugLogFunctions.DebugLog($"{namecolored} <color=#969696>joined while invisible.</color>");
						PendulumLogger.Log($"{displayName} ({UserID}) joined while invisible.");
					}
				}
			}
		}
	}
}
