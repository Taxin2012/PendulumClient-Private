using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using MelonLoader;
using Newtonsoft.Json;
using Transmtn;
using UnhollowerBaseLib;
using VRC.Core;
using UnityEngine;

namespace PendulumClient.Anti.Patches
{
    internal class TransmtnWebsocketPatch : PendulumPatch
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void ProcessPipelineDelegate(System.IntPtr instancePtr, System.IntPtr senderPtr, System.IntPtr argsPtr);

        private static readonly System.Collections.Generic.Dictionary<string, WSPlayerState> cachedPlayerStates = new System.Collections.Generic.Dictionary<string, WSPlayerState>();

        private static ProcessPipelineDelegate processPipelineDelegate = null;

		private static Color FrendNotifColor;
        internal unsafe override void SetupPatches()
        {
			FrendNotifColor = new Color();
			MethodInfo method = typeof(WebsocketPipeline).GetMethod(nameof(WebsocketPipeline._ProcessPipe_b__21_0));
            System.IntPtr ptr = *(System.IntPtr*)(void*)(System.IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(method).GetValue(null);
            MelonUtils.NativeHookAttach((System.IntPtr)(&ptr), typeof(TransmtnWebsocketPatch).GetMethod(nameof(TransmtnWebsocketPatch.OnDataReceivedPatch), BindingFlags.Static | BindingFlags.NonPublic).MethodHandle.GetFunctionPointer());
            processPipelineDelegate = Marshal.GetDelegateForFunctionPointer<ProcessPipelineDelegate>(ptr);
		}

		private static void CachePlayer(string userId, VRCApiUser user, string userLocation)
		{
			if (!cachedPlayerStates.ContainsKey(userId))
			{
				cachedPlayerStates.Add(userId, new WSPlayerState
				{
					username = user.displayName,
					joinState = user.state,
					location = userLocation
				});
			}
		}

		private unsafe static void OnDataReceivedPatch(System.IntPtr instancePtr, System.IntPtr senderPtr, System.IntPtr argsPtr)
		{
			processPipelineDelegate(instancePtr, senderPtr, argsPtr);
			if (argsPtr == System.IntPtr.Zero)
			{
				return;
			}
			string value;
			try
			{
				nint num = (nint)argsPtr + 16;
				value = IL2CPP.Il2CppStringToManaged(*(System.IntPtr*)num);
			}
			catch (System.Exception)
			{
				return;
			}
			VRCWebSocketObjectJSON vRCWebSocketObject;
			try
			{
				vRCWebSocketObject = JsonConvert.DeserializeObject<VRCWebSocketObjectJSON>(value);
			}
			catch (System.Exception)
			{
				return;
			}
			if (vRCWebSocketObject == null || vRCWebSocketObject.type == "notification")
			{
				return;
			}
			VRCWebSocketContentJSON VRCWebSocketContent;
			try
			{
				VRCWebSocketContent = JsonConvert.DeserializeObject<VRCWebSocketContentJSON>(vRCWebSocketObject.content);
			}
			catch (System.Exception)
			{
				return;
			}
			if (APIUser.CurrentUser == null || VRCWebSocketContent == null || VRCWebSocketContent.userId == APIUser.CurrentUser.id)
			{
				return;
			}
			if (VRCWebSocketContent.user != null)
			{
				string userLocation = ((!string.IsNullOrEmpty(VRCWebSocketContent.travelingToLocation)) ? VRCWebSocketContent.travelingToLocation : ((VRCWebSocketContent.world != null) ? VRCWebSocketContent.world.name : string.Empty));
				CachePlayer(VRCWebSocketContent.user.id, VRCWebSocketContent.user, userLocation);
			}

			if (string.IsNullOrEmpty(VRCWebSocketContent.user.displayName))
			{
				return;
			}

			if (vRCWebSocketObject.type == "friend-location")
			{
				try
				{
					string text3 = (string.IsNullOrEmpty(VRCWebSocketContent.travelingToLocation) ? VRCWebSocketContent.world.name : VRCWebSocketContent.travelingToLocation);
					if (text3 != cachedPlayerStates[VRCWebSocketContent.user.id].location)
					{
						cachedPlayerStates[VRCWebSocketContent.user.id].location = text3;
						string text = "<color=" + QMLogAndPlayerlist.PlayerListFunctions.FriendColor + ">" + VRCWebSocketContent.user.displayName + "</color> went to <color=" + Main.PendulumClientMain.MenuColorHex + ">" + text3 + "</color>";
						string textnorm = VRCWebSocketContent.user.displayName + " went to " + text3;
						PendulumLogger.SocialLog(textnorm);
						UI.NotificationsV2.SendHudNotificationThreaded(Convert.ToString(text.Clone()));
					}
				}
				catch (Exception e)
				{
					PendulumLogger.LogError("Error with WS patch: " + e.ToString());
				}
			}
			else if (vRCWebSocketObject.type == "friend-online")
			{
				string text = "<color=" + QMLogAndPlayerlist.PlayerListFunctions.FriendColor + ">" + VRCWebSocketContent.user.displayName + "</color> is now <color=#00FF00>online</color>";
				string textnorm = VRCWebSocketContent.user.displayName + " is now online";
				PendulumLogger.SocialLog(textnorm);
				UI.NotificationsV2.SendHudNotificationThreaded(text);
			}
			else if (vRCWebSocketObject.type == "friend-offline")
			{
				if (cachedPlayerStates.ContainsKey(VRCWebSocketContent.userId))
				{
					string text = "<color=" + QMLogAndPlayerlist.PlayerListFunctions.FriendColor + ">" + cachedPlayerStates[VRCWebSocketContent.userId].username + "</color> is now <color=#FF0000>offline</color>";
					string textnorm = cachedPlayerStates[VRCWebSocketContent.userId].username + " is now offline";
					PendulumLogger.SocialLog(textnorm);
					UI.NotificationsV2.SendHudNotificationThreaded(text);
				}
				else
				{
					API.SendGetRequest("users/" + VRCWebSocketContent.userId, (ApiContainer)new ApiModelContainer<APIUser>
					{
						OnSuccess = (System.Action<ApiContainer>)OnPlayerFetched,
						OnError = (System.Action<ApiContainer>)OnPlayerFailedFetch
					}, (Il2CppSystem.Collections.Generic.IReadOnlyDictionary<string, BestHTTP.JSON.Json.Token>)null, false, 0f, (API.CredentialsBundle)null);
				}
			}
				//case "friend-update":
				//case "friend-active":
		}

		private static void OnPlayerFetched(ApiContainer container)
		{
			try
			{
				ApiModelContainer<APIUser> apiModelContainer = container.Cast<ApiModelContainer<APIUser>>();
				string name = new Il2CppSystem.String(((ApiDictContainer)apiModelContainer).ResponseDictionary["displayName"].Pointer);
				string text = $"<color={QMLogAndPlayerlist.PlayerListFunctions.FriendColor}>{name}</color> is now <color=#FF0000>offline</color>";
				string textnorm = name + " is now offline";
				PendulumLogger.SocialLog(textnorm);
				UI.NotificationsV2.SendHudNotificationThreaded(text);
			}
			catch { }
		}

		private static void OnPlayerFailedFetch(ApiContainer container)
		{
		}
	}
	public class WSPlayerState
    {
        internal string username;

        internal string joinState;

        internal string location;
    }
	public class VRCWebSocketObjectJSON
	{
		public string type;

		public string content;
	}
	public class VRCApiUser
	{
		public string id;

		public string username;

		public string displayName;

		public string userIcon;

		public string bio;

		public string[] bioLinks;

		public string profilePicOverride;

		public string statusDescription;

		public string currentAvatarImageUrl;

		public string currentAvatarThumbnailImageUrl;

		public string fallbackAvatar;

		public string state;

		public string[] tags;

		public string developerType;

		public System.DateTime last_login;

		public string last_platform;

		public bool allowAvatarCopying;

		public string status;

		public System.DateTime date_joined;

		public string friendKey;

		public string worldId;

		public string instanceId;

		public string location;
	}
	public class VRCApiWorld
	{
		public string id;

		public string name;

		public string authorId;

		public string authorName;

		public int capacity;

		public string imageUrl;

		public string thumbnailImageUrl;

		public string releaseStatus;

		public string organization;

		public string[] tags;

		public int favorites;

		public int popularity;

		public int heat;

		public int occupants;
	}
	public class VRCWebSocketContentJSON
	{
		public string userId;

		public VRCApiUser user;

		public string location;

		public string travelingToLocation;

		public string instance;

		public VRCApiWorld world;

		public bool canRequestInvite;
	}
}
	