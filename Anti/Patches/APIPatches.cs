using System;
using Il2CppSystem.Collections.Generic;
using VRC.Core;

namespace PendulumClient.Anti.Patches
{
    internal class APIPatches
    {
        public static bool SendGetRequestPatch(string __0, ApiContainer __1, Dictionary<string, Il2CppSystem.Object> __2, bool __3, float __4, API.CredentialsBundle __5)
        {
            if (__0.StartsWith("auth") && __0.Length == 9)
            {
                Prefixes.QuestSpoofPatch = false;
            }
            else if (__0.StartsWith("message") && __0.Contains("requestResponse"))
            {
                Prefixes.QuestSpoofPatch = true;
            }

            return true;
        }
    }
}
