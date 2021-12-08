using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using PendulumClient.Main;
using UnityEngine;
using VRC.Core;

namespace PendulumClient.ColorModule
{
    class ColorSettings
    {
        public const string SettingsCategory = "PendulumClient";
        public const string ColorR = "MenuColor-R";
        public const string ColorG = "MenuColor-G";
        public const string ColorB = "MenuColor-B";
        public const string ColorA = "MenuColor-A";
        public static void RegisterSettings()
        {
            ModPrefs.RegisterCategory(SettingsCategory, "PendulumClientSettings");
            ModPrefs.RegisterPrefFloat(SettingsCategory, ColorR, 1f, "R (0-1)");
            ModPrefs.RegisterPrefFloat(SettingsCategory, ColorG, 1f, "G (0-1)");
            ModPrefs.RegisterPrefFloat(SettingsCategory, ColorB, 1f, "B (0-1)");
            ModPrefs.RegisterPrefFloat(SettingsCategory, ColorA, 2f, "A (0-10)");
            ModPrefs.RegisterPrefBool(SettingsCategory, "AntiPortal", false, "Enable AntiPortal Before Game Start");
            ModPrefs.RegisterPrefInt(SettingsCategory, "FPSCap", 90);
            ModPrefs.RegisterPrefInt(SettingsCategory, "MenuStyle", 0);
            ModPrefs.RegisterPrefFloat(SettingsCategory, "NotifacationSoundVolume", 0.5f, "JoinNotifcation Sounds Volume");
            PendulumClientMain.ColorSettingsSetup = true;
        }
    }
}
