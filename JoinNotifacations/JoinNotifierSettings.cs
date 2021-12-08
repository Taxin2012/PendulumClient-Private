using MelonLoader;
using UnityEngine;
using VRC.Core;

namespace JoinNotifier
{
    public static class JoinNotifierSettings
    {
        private const string SettingsCategory = "PendulumClient";
        private const string SettingShouldPlaySound = "PlaySound";
        private const string SettingSoundVolume = "SoundVolume";
        private const string SettingTextSize = "TextSize";
        private const string SettingShouldBlink = "BlinkIcon";
        private const string SettingJoinShowName = "ShowJoinedName";
        private const string SettingNotifyPublic = "NotifyInPublic";
        private const string SettingNotifyFriends = "NotifyInFriends";
        private const string SettingNotifyPrivate = "NotifyInPrivate";
        private const string SettingLeaveSound= "LeaveSound";
        private const string SettingLeaveBlink = "LeaveBlink";
        private const string SettingLeaveShowName = "ShowLeaveName";
        private const string SettingJoinIconColor = "JoinIconColor";
        private const string SettingLeaveIconColor = "LeaveIconColor";
        
        private const string SettingUseUiMixer = "UseUiMixer";
        
        /*public static void RegisterSettings()
        {
            //ModPrefs.RegisterCategory(SettingsCategory, "Join Notifier");
            
            //ModPrefs.RegisterPrefBool(SettingsCategory, SettingShouldBlink, true, "Blink HUD icon on join");
            ModPrefs.RegisterPrefBool(SettingsCategory, SettingShouldPlaySound, true, "Play sound on join");
            ModPrefs.RegisterPrefBool(SettingsCategory, SettingJoinShowName, true, "Show joined names");
            // ModPrefs.RegisterPrefInt(SettingsCategory, SettingTextSize, 36, "Text size (pt)");
            // ModPrefs.RegisterPrefBool(SettingsCategory, SettingNotifyPublic, true, "Notify in public instances");
            // ModPrefs.RegisterPrefBool(SettingsCategory, SettingNotifyFriends, true, "Notify in friends[+] instances");
            // ModPrefs.RegisterPrefBool(SettingsCategory, SettingNotifyPrivate, true, "Notify in private instances");
            
            ModPrefs.RegisterPrefBool(SettingsCategory, SettingLeaveBlink, true, "Blink HUD icon on leave");
            ModPrefs.RegisterPrefBool(SettingsCategory, SettingLeaveSound, false, "Play sound on leave");
            ModPrefs.RegisterPrefBool(SettingsCategory, SettingLeaveShowName, true, "Show left names");
            
            //ModPrefs.RegisterPrefColor(SettingsCategory, SettingJoinIconColor, new Color(0.50F, 0.75F, 1F), hideFromList: true);
            //ModPrefs.RegisterPrefColor(SettingsCategory, SettingLeaveIconColor, new Color(0.6f, 0.32f, 0.2f), hideFromList: true);
            
            ModPrefs.RegisterPrefBool(SettingsCategory, SettingUseUiMixer, true, "Notifications are UI sounds", hideFromList: true);
            
        }*/

        public static bool ShouldNotifyInCurrentInstance()
        {
            return true;
        }

        public static float GetSoundVolume() => ModPrefs.GetFloat("PendulumClient", "NotifacationSoundVolume");

        public static Color GetJoinIconColor() => new Color(0.4F, 1F, 0.4F);// ModPrefs.GetColor(SettingsCategory, SettingJoinIconColor);
        public static Color GetLeaveIconColor() => new Color(1f, 0.4f, 0.4f); //ModPrefs.GetColor(SettingsCategory, SettingLeaveIconColor);

        public static bool GetUseUiMixer() => true;

        public static int GetTextSize = 36;
    }
}