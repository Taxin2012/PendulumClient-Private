using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Il2CppSystem;
using System.Reflection;

namespace PendulumClient.Anti.Patches
{
    internal class ImageDownloadPatch : PendulumPatch
    {
		internal static bool PortalImageSafety = true;
        internal override void SetupPatches()
        {
            var PatchClass = typeof(ImageDownloader);
            var MethodToPatch = PatchClass.GetMethod(nameof(ImageDownloader.DownloadImageInternal));
            var PrefixClass = typeof(ImageDownloadPatch);
            var PrefixMethod = PrefixClass.GetMethod(nameof(ImageDownloadPatch.OnImageDownloadPatch), BindingFlags.Static | BindingFlags.NonPublic);
			PatchMethod(MethodToPatch, PrefixMethod);
        }

		private static bool OnImageDownloadPatch(string __0, int __1, Il2CppSystem.Action<Texture2D> __2, Il2CppSystem.Action __3, string __4, bool __5)
		{
			if (PortalImageSafety && !string.IsNullOrEmpty(__0))
			{
				for (int i = 0; i < BlockedStrings.allowedImageURLs.Count; i++)
				{
					if (__0.StartsWith(BlockedStrings.allowedImageURLs[i]))
					{
						return true;
					}
				}
				PendulumLogger.DebugLog($"Image download blocked: {__0}");
				return false;
			}
			return true;
		}
	}
}
