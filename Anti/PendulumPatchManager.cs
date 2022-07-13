using System;
using System.Collections.Generic;

namespace PendulumClient.Anti
{
    internal class PendulumPatchManager
    {
        internal static bool PatchesSetup = false;

        internal static HarmonyLib.Harmony harmonyinstance = null;

        private static readonly List<PendulumPatch> pendulumPatches = new List<PendulumPatch>();
        internal static void SetupPatchManager(HarmonyLib.Harmony instance)
        {
            if (PatchesSetup == false)
            {
                PatchesSetup = true;

                if (harmonyinstance == null)
                    harmonyinstance = instance;

                //add all patch classes here
                pendulumPatches.Add(new Patches.ImageDownloadPatch());
                pendulumPatches.Add(new Patches.VideoSyncPatch());
                pendulumPatches.Add(new Patches.InvisibleDetection());
                pendulumPatches.Add(new Patches.TransmtnWebsocketPatch());
                pendulumPatches.Add(new Patches.NotificationManagerPatch());
            }
        }

        internal static void PatchMethods()
        {
            for (int i = 0; i < pendulumPatches.Count; i++)
            {
                try
                {
                    pendulumPatches[i].SetupPatches();
                }
                catch (Exception e)
                {
                    PendulumLogger.LogError("Error setting up patches: " + e.ToString());
                }
            }
        }

        internal static void OnUI_Early()
        {
            for (int i = 0; i < pendulumPatches.Count; i++)
            {
                try
                {
                    pendulumPatches[i].OnUI_Early();
                }
                catch (Exception e)
                {
                    PendulumLogger.LogError("Error with OnUI patches: " + e.ToString());
                }
            }
        }

        internal static void OnUpdate()
        {
            for (int i = 0; i < pendulumPatches.Count; i++)
            {
                try
                {
                    pendulumPatches[i].OnUpdate();
                }
                catch (Exception e)
                {
                    PendulumLogger.LogError("Error updating patches: " + e.ToString());
                }
            }
        }
    }
}
