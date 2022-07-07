using System;
using System.Reflection;
using HarmonyLib;

namespace PendulumClient.Anti
{
    internal class PendulumPatch
    {
        internal virtual void SetupPatches()
        {
        }

        protected bool PatchMethod(MethodBase original, MethodInfo prefix = null, MethodInfo postfix = null)
        {
            if (original == null)
            {
                return false;
            }

            if (prefix == null && postfix == null)
            {
                return false;
            }

            try
            {
                if (postfix == null)
                {
                    PendulumPatchManager.harmonyinstance.Patch(original, new HarmonyMethod(prefix));
                    return true;
                }
                else if (prefix == null)
                {
                    PendulumPatchManager.harmonyinstance.Patch(original, null, new HarmonyMethod(postfix));
                    return true;
                }
                else
                {
                    PendulumPatchManager.harmonyinstance.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
                    return true;
                }
            }
            catch (Exception e)
            {
                PendulumLogger.LogError("Error Patching: " + e.ToString());
                return false;
            }
        }
    }
}
