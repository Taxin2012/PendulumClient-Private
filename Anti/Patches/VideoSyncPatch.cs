using System;
using System.Reflection;
using UnityEngine.Video;
using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDK3.Video.Components.Base;
using VRC.SDKBase;
using VRCSDK2;

namespace PendulumClient.Anti.Patches
{
    internal class VideoSyncPatch : PendulumPatch
    {
		internal static bool AntiVideoPlayer = true;
        internal override void SetupPatches()
        {
			PatchMethod(typeof(VRC_SyncVideoPlayer).GetMethod("AddURL"), typeof(VideoSyncPatch).GetMethod(nameof(VideoSyncPatch.OnVideoPlayerUrlQueuedPatch), BindingFlags.Static | BindingFlags.NonPublic));
			PatchMethod(typeof(VRC_SyncVideoPlayer).GetMethod("Play"), typeof(VideoSyncPatch).GetMethod(nameof(VideoSyncPatch.OnVideoPlayerUrlPlayedPatch), BindingFlags.Static | BindingFlags.NonPublic));
			PatchMethod(typeof(VRC_SyncVideoPlayer).GetMethod("PlayIndex"), typeof(VideoSyncPatch).GetMethod(nameof(VideoSyncPatch.OnVideoPlayerUrlPlayedIndexPatch), BindingFlags.Static | BindingFlags.NonPublic));
			PatchMethod(typeof(VRCUnityVideoPlayer).GetMethod("LoadURL"), typeof(VideoSyncPatch).GetMethod(nameof(VideoSyncPatch.LoadUrlPatch), BindingFlags.Static | BindingFlags.NonPublic));
			PatchMethod(typeof(VRCUnityVideoPlayer).GetMethod("PlayURL"), typeof(VideoSyncPatch).GetMethod(nameof(VideoSyncPatch.PlayUrlPatch), BindingFlags.Static | BindingFlags.NonPublic));
			PatchMethod(typeof(VRCAVProVideoPlayer).GetMethod("LoadURL"), typeof(VideoSyncPatch).GetMethod(nameof(VideoSyncPatch.LoadUrlPatch), BindingFlags.Static | BindingFlags.NonPublic));
			PatchMethod(typeof(VRCAVProVideoPlayer).GetMethod("PlayURL"), typeof(VideoSyncPatch).GetMethod(nameof(VideoSyncPatch.PlayUrlPatch), BindingFlags.Static | BindingFlags.NonPublic));
			PatchMethod(typeof(BaseVRCVideoPlayer).GetMethod("LoadURL"), typeof(VideoSyncPatch).GetMethod(nameof(VideoSyncPatch.LoadUrlPatch), BindingFlags.Static | BindingFlags.NonPublic));
			PatchMethod(typeof(BaseVRCVideoPlayer).GetMethod("PlayURL"), typeof(VideoSyncPatch).GetMethod(nameof(VideoSyncPatch.PlayUrlPatch), BindingFlags.Static | BindingFlags.NonPublic));
			PatchMethod(typeof(VideoPlayer).GetMethod("Play"), typeof(VideoSyncPatch).GetMethod(nameof(VideoSyncPatch.VideoPlayerPlayPatch), BindingFlags.Static | BindingFlags.NonPublic));
		}

		private static bool IsURLWhitelisted(string url)
		{
			if (!AntiVideoPlayer)
			{
				return true;
			}
			string text = url.Trim();
			if (string.IsNullOrEmpty(text) || text.Length < 7)
			{
				return true;
			}
			int num = text.IndexOf('/');
			if (num != -1 && text[num + 1] == '/')
			{
				string text2 = text.Substring(num + 2);
				int num2 = text2.IndexOf('/');
				if (num2 != -1)
				{
					string text3 = (text2.StartsWith("www") ? text2.Substring(4, num2 - 4) : text2.Substring(0, num2));
					bool flag = BlockedStrings.SubDomainCheck(text3);
					for (int i = 0; i < BlockedStrings.allowedVideoPlayerURLs.Count; i++)
					{
						bool flag2 = BlockedStrings.SubDomainCheck(BlockedStrings.allowedVideoPlayerURLs[i]);
						if ((flag && flag2) || (!flag && !flag2))
						{
							if (text3 == BlockedStrings.allowedVideoPlayerURLs[i])
							{
								return true;
							}
						}
						else if (flag && !flag2)
						{
							string text4 = text3.Substring(text3.IndexOf('.') + 1);
							if (text4 == BlockedStrings.allowedVideoPlayerURLs[i])
							{
								return true;
							}
						}
					}
				}
			}
			PendulumLogger.DebugLog($"Video download blocked: {url}");
			return false;
		}

		private static bool VideoPlayerPlayPatch(ref VideoPlayer __instance)
		{
			return IsURLWhitelisted(__instance.url);
		}

		private static bool LoadUrlPatch(VRCUrl __0)
		{
			return IsURLWhitelisted(__0.url);
		}

		private static bool PlayUrlPatch(VRCUrl __0)
		{
			return IsURLWhitelisted(__0.url);
		}

		private static bool OnVideoPlayerUrlQueuedPatch(string __0)
		{
			return IsURLWhitelisted(__0);
		}

		private static bool OnVideoPlayerUrlPlayedPatch(VRC_SyncVideoPlayer __instance)
		{
			return __instance.Videos.Length <= 0 || IsURLWhitelisted(__instance.Videos[0].URL);
		}

		private static bool OnVideoPlayerUrlPlayedIndexPatch(int __0, VRC_SyncVideoPlayer __instance)
		{
			return __instance.Videos.Length < __0 || IsURLWhitelisted(__instance.Videos[__0].URL);
		}
	}
}
