using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Il2CppSystem;
using MelonLoader;
using Transmtn.DTO.Notifications;
using UnhollowerBaseLib;
using VRC.Core;

namespace PendulumClient.Anti.Patches
{
    internal class NotificationManagerPatch : PendulumPatch
    {
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate System.IntPtr AddNotificationDelegate(System.IntPtr instancePtr, System.IntPtr notificationPtr, System.IntPtr returnedException);

		internal static bool firstTimeFriendRequestsReceived = false;

		private static readonly HashSet<string> handledNotifications = new HashSet<string>();

		private static AddNotificationDelegate addNotificationDelegate = null;

		internal static bool INV_HUD = true;
		internal static bool INV_LOG = true;
		internal static bool REQINV_HUD = true;
		internal static bool REQINV_LOG = true;
		internal static bool REQINVRESP_HUD = true;
		internal static bool REQINVRESP_LOG = true;
		internal static bool FREQ_HUD = true;
		internal static bool FREQ_LOG = true;
		internal static bool VTK_HUD = true;
		internal static bool VTK_LOG = true;

		internal unsafe override void SetupPatches()
		{
			MethodInfo method = typeof(NotificationManager.ObjectNPrivateSealedNoBoVoNoBoNoBoNoBoNo0).GetMethods(BindingFlags.Instance | BindingFlags.Public).First((MethodInfo m) => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(Notification));
			System.IntPtr ptr = *(System.IntPtr*)(void*)(System.IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method).GetValue(null);
			MelonUtils.NativeHookAttach((System.IntPtr)(&ptr), typeof(NotificationManagerPatch).GetMethod(nameof(NotificationManagerPatch.AddNotificationPatch), BindingFlags.Static | BindingFlags.NonPublic).MethodHandle.GetFunctionPointer());
			addNotificationDelegate = Marshal.GetDelegateForFunctionPointer<AddNotificationDelegate>(ptr);
		}

		private static System.IntPtr AddNotificationPatch(System.IntPtr instancePtr, System.IntPtr notificationPtr, System.IntPtr returnedException)
		{
			if (instancePtr == System.IntPtr.Zero || notificationPtr == System.IntPtr.Zero)
			{
				return System.IntPtr.Zero;
			}
			Notification notification = new Notification(notificationPtr);
			if (notification.id != null && !handledNotifications.Contains(notification.id))
			{
				handledNotifications.Add(notification.id);
				try
				{
					HandleNotification(ref notification);
				}
				catch (System.Exception e)
				{
					PendulumLogger.LogError("Error handling notification: " + e);
				}
			}
			return addNotificationDelegate(instancePtr, notificationPtr, returnedException);
		}
		private static void HandleNotification(ref Notification notification)
		{
			if (!InvisibleDetection.isConnectedToInstance || notification.notificationType == null)
			{
				return;
			}
			//PlayerInformation localPlayerInformation = PlayerWrappers.GetLocalPlayerInformation();
			var localPlayer = Wrapper.PlayerWrappers.GetCurrentPlayer();
			if ((localPlayer != null && Wrapper.PlayerWrappers.IsLocalPlayer(localPlayer._player) && localPlayer._player.field_Private_APIUser_0.statusValue == APIUser.UserStatus.DoNotDisturb) || VRCInputManager.Method_Public_Static_Boolean_InputSetting_0(VRCInputManager.InputSetting.StreamerModeEnabled))
			{
				return;
			}
			switch (notification.notificationType)
			{
			case "invite":
				{
					if (!INV_HUD && !INV_LOG)
						break;

					string worldname = new Il2CppSystem.String(notification.details["worldName"].Pointer);
					string displayName = notification.senderUsername;
					string text = $"<color={QMLogAndPlayerlist.PlayerListFunctions.FriendColor}>{displayName}</color> sent you an invite to <color=#00CAFF>{worldname}</color>";
					string textnorm = $"{displayName} sent you an invite to {worldname}";
					PendulumLogger.SocialLog(textnorm);

					if (INV_HUD)
						UI.NotificationsV2.AddNotification(text);

					break;
				}
			case "requestInvite":
				{
					if (!REQINV_HUD && !REQINV_LOG)
						break;

					string displayName = notification.senderUsername;
					string text = $"<color={QMLogAndPlayerlist.PlayerListFunctions.FriendColor}>{displayName}</color> requested to join your world";
					string textnorm = $"{displayName} requested to join your world";
					PendulumLogger.SocialLog(textnorm);

					if (REQINV_HUD)
						UI.NotificationsV2.AddNotification(text);

					break;
				}
			case "requestInviteResponse":
				{
					if (!REQINVRESP_HUD && !REQINVRESP_LOG)
						break;

					string displayName = notification.senderUsername;
					string text = $"<color={QMLogAndPlayerlist.PlayerListFunctions.FriendColor}>{displayName}</color> denied your request";
					string textnorm = $"{displayName} denied your request";
					PendulumLogger.SocialLog(textnorm);

					if (REQINVRESP_HUD)
						UI.NotificationsV2.AddNotification(text);

					break;
				}
			case "friendRequest":
				{
					if ((!FREQ_HUD && !FREQ_LOG) || !firstTimeFriendRequestsReceived)
						break;

					string displayName = notification.senderUsername;
					string text = $"<color={QMLogAndPlayerlist.PlayerListFunctions.UserColor}>{displayName}</color> sent you a friend request";
					string textnorm = $"{displayName} sent you a friend request";
					PendulumLogger.SocialLog(textnorm);

					if (FREQ_HUD)
						UI.NotificationsV2.AddNotification(text);

					break;
				}
			case "voteToKick":
				{
					if (!VTK_HUD && !VTK_LOG)
						break;

					string userId = new Il2CppSystem.String(notification.details["id"].Pointer);
					//bool flag = APIUser.IsFriendsWith(userId);
					/*if ((!flag || !Configuration.GetModerationsConfig().LogModerationsVotekickFriends) && (flag || !Configuration.GetModerationsConfig().LogModerationsVotekickOthers))
					{
						break;
					}*/
					if (Wrapper.PlayerWrappers.GetPlayer(userId) == null)
						break;

					var displayName = Wrapper.PlayerWrappers.GetPlayer(userId).field_Private_APIUser_0.displayName;
					var coloredname = QMLogAndPlayerlist.PlayerListFunctions.GetNameColoredQuest(Wrapper.PlayerWrappers.GetPlayer(userId));
					//string username = notification.message.Substring(39, notification.message.IndexOf(',') - 39);
					string text = $"Votekick started against {coloredname}";
					string textnorm = $"Votekick started against {displayName}";
					PendulumLogger.SocialLog(textnorm);

					if (VTK_HUD)
						UI.NotificationsV2.AddNotification(text);

					break;
				}
			}
		}
	}
}
