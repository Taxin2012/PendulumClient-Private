using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace PendulumClient.QMLogAndPlayerlist
{
    class QM_PlayerListAndDebugLog
    {
        internal static VRC_PLScrollRect PlayerList;
        internal static VRC_DebugScrollRect DebugLogRect;
        public static void OnUI()
        {
            PlayerList = new VRC_PLScrollRect("Right_PL_Closed", "PC_RightWingPlayerList", 2222, 0/*609.902f, 457.9203f*/, GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Right/Button").transform);
            DebugLogRect = new VRC_DebugScrollRect("Left_DL_Closed", "PC_LeftWingDebugLog", -2222, 0/*609.902f, 457.9203f*/, GameObject.Find("UserInterface/Canvas_QuickMenu(Clone)/Container/Window/Wing_Left/Button").transform);
            DebugLogFunctions.DebugLogStartup();
        }
    }

    public static class DebugLogFunctions
    {
        public static string TimeColor = ColorModule.ColorModule.ColorToHex(ColorModule.ColorModule.CachedColor);

        public const int MaxLogs = 100;
        public static List<string> Logs = new List<string>();
        public static List<GameObject> TextObjects = new List<GameObject>(); //logs are TextObjects[#] 0-99
        public static void DebugLogStartup()
        {
            if (QM_PlayerListAndDebugLog.DebugLogRect != null)
            {
                if (VRC_DebugScrollRect.TempText != null)
                {
                    UnityEngine.Object.DestroyImmediate(VRC_DebugScrollRect.TempText);
                    DebugLog("Welcome to Pendulum Client!");
                }
                else
                {
                    PendulumLogger.LogErrorSevere("Debug Log Setup Error (2)");
                }
            }
            else
            {
                PendulumLogger.LogErrorSevere("Debug Log Setup Error (1)");
            }
        }

        public static void DebugLogUpdate(string Input)
        {
            if (!QM_PlayerListAndDebugLog.DebugLogRect.enabled)
            {
                return;
            }
            else if (!QM_PlayerListAndDebugLog.DebugLogRect.Scroll.activeSelf)
            {
                QM_PlayerListAndDebugLog.DebugLogRect.Scroll.SetActive(true);
            }

            if (TextObjects.Count >= MaxLogs)
            {
                if (TextObjects.ElementAt(0) != null)
                {
                    var TextToDelete = TextObjects.ElementAt(0);
                    TextObjects.RemoveAt(0);
                    UnityEngine.Object.DestroyImmediate(TextToDelete);
                    var NewItem = GameObject.Instantiate(VRC_DebugScrollRect.CloneableText, VRC_DebugScrollRect.VerticalLayout.transform);
                    NewItem.name = "DebugLogItem";
                    var Text = NewItem.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
                    Text.text = Input;
                    NewItem.transform.SetAsFirstSibling();
                    TextObjects.Add(NewItem);
                    NewItem.SetActive(true);
                }
            }
            else
            {
                var NewItem = GameObject.Instantiate(VRC_DebugScrollRect.CloneableText, VRC_DebugScrollRect.VerticalLayout.transform);
                NewItem.name = "DebugLogItem";
                var Text = NewItem.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
                Text.text = Input;
                NewItem.transform.SetAsFirstSibling();
                TextObjects.Add(NewItem);
                NewItem.SetActive(true);
            }
        }

        public static void DebugLog(string log)
        {
            Logs.Add(log);
            string logwithprefix = $"<color=white>[</color><color={TimeColor}>{DateTime.Now.ToString("h:mm:ss tt")}</color><color=white>]</color> {log}";
            DebugLogUpdate(logwithprefix);
        }

        public static void DebugLogPlayerJoin(string name)
        {
            //66ff66ff
            string logwithprefix = $"<color=white>[</color><color={TimeColor}>{DateTime.Now.ToString("h:mm:ss tt")}</color><color=white>]</color> <color=#66ff66ff>{name} has joined.</color>";
            DebugLogUpdate(logwithprefix);
        }

        public static void DebugLogPlayerLeave(string name)
        {
            //ff6666ff
            string logwithprefix = $"<color=white>[</color><color={TimeColor}>{DateTime.Now.ToString("h:mm:ss tt")}</color><color=white>]</color> <color=#ff6666ff>{name} has left.</color>";
            DebugLogUpdate(logwithprefix);
        }
    }
    public static class PlayerListFunctions
    {
        public const string QuestColor = "grey";
        public const string VRColor = "#00c8ffff";
        public const string PCColor = "#2bc494ff";
        public const string ClientUserColor = "#c43a2bff";
        public const string DevColor = "#ff0000ff";
        public const string FriendColor = "#ffd000ff";
        public const string MasterColor = "#ffb300ff";
        public const string HostColor = "#ffb300ff";
        public const string VRCPlusColor = "#ba8604ff";
        public const string YouColor = "#00ff00ff";
        public const string LegendaryColor = "#004c7fff";
        public const string VeteranColor = "#b2ff00ff";
        public const string TrustedColor = "#8000ffff";
        public const string KnownColor = "#e46e43ff";
        public const string UserColor = "#2acf5bff";
        public const string NewUserColor = "#1677ffff";
        public const string VisitorColor = "#ccccccff";
        public const string NuisanceColor = "#401d0fff";
        public const string SelectedColor = "#004c7fff";
        public static string GetPlatform(this VRC.Player player)
        {
            //check if stuff is null before using it will prob fix errors
            if (player.field_Private_APIUser_0.last_platform == "android" || player.field_Private_APIUser_0.IsOnMobile)
            {
                return $"<color={QuestColor}>[Q]</color>";
            }
            else if (player.field_Private_VRCPlayerApi_0.IsUserInVR())
            {
                return $"<color={VRColor}>[VR]</color>";
            }
            else
            {
                return $"<color={PCColor}>[PC]</color>";
            }
        }
        public static string IsDev(this VRC.Player player)
        {
            foreach (var tag in player.field_Private_APIUser_0.tags)
            {
                if (tag.Contains("admin_") || player.field_Private_APIUser_0.id.Length == 10 || player.field_Private_APIUser_0.developerType != VRC.Core.APIUser.DeveloperType.None || player.field_Private_APIUser_0.hasModerationPowers || player.field_Private_VRCPlayerApi_0.isModerator)
                {
                    return $"<color={DevColor}>[DEV]</color>";
                }
            }
            return "";
        }
        public static string IsYou(this VRC.Player player)
        {
            if (player.field_Private_APIUser_0.id == VRC.Core.APIUser.CurrentUser.id)
            {
                return $"<color={YouColor}>[Y]</color>";
            }
            return "";
        }
        public static string IsFriend(this VRC.Player player)
        {
            if (VRC.Core.APIUser.IsFriendsWith(player.field_Private_APIUser_0.id))
            {
                return $"<color={FriendColor}>[F]</color>";
            }
            return "";
        }
        public static string IsInstanceMaster(this VRC.Player player)
        {
            if (player.field_Private_VRCPlayerApi_0.isMaster)
            {
                return $"<color={MasterColor}>[M]</color>";
            }
            return "";
        }
        public static string IsVRCPlusUser(this VRC.Player player)
        {
            foreach (var tag in player.field_Private_APIUser_0.tags)
            {
                if (tag == "system_supporter")
                {
                    return $"<color={VRCPlusColor}>[+]</color>";
                }
            }
            return "";
        }
        public static string IsInstanceHost(this VRC.Player player)
        {
            if (RoomManager.field_Internal_Static_ApiWorldInstance_0 == null) return "";
            if (string.IsNullOrEmpty(RoomManager.field_Internal_Static_ApiWorldInstance_0.ownerId)) return "";
            if (RoomManager.field_Internal_Static_ApiWorldInstance_0.ownerId == player.field_Private_APIUser_0.id)
            {
                return $"<color={HostColor}>[H]</color>";
            }
            return "";
        }
        public static string IsSelectedUser(this VRC.Player player)
        {
            if (Main.PendulumClientMain.StoredUserID == player.field_Private_APIUser_0.id)
            {
                return $"<color={SelectedColor}>[S]</color>";
            }
            return "";
        }
        public static List<string> ClientUsers = new List<string>();
        public static string IsClientUser(this VRC.Player player)
        {
            if (ClientUsers.Contains(player.field_Private_APIUser_0.id))
            {
                return $"<color={ClientUserColor}>[C]</color>";
            }
            float fps = (player._playerNet.prop_Byte_0 != 0) ? Mathf.Floor(1000f / (float)player._playerNet.prop_Byte_0) : -1f;
            if (!player.field_Private_VRCPlayerApi_0.IsUserInVR())
            {
                if ((fps > 90 || fps < 1) && fps != -1f)
                {
                    ClientUsers.Add(player.field_Private_APIUser_0.id);
                    return $"<color={ClientUserColor}>[C]</color>";
                }
            }
            else if ((fps > 144 || fps < 1 || player._playerNet.field_Private_Int16_0 < 0) && fps != -1f)
            {
                ClientUsers.Add(player.field_Private_APIUser_0.id);
                return $"<color={ClientUserColor}>[C]</color>";
            }
            return "";
        }
        public static string GetPingColored(this VRC.Player player)
        {
            short ping = player._playerNet.field_Private_Int16_0;
            if (ping > 150)
                return "<color=red>PING: " + ping + "</color>";
            else if (ping > 75)
                return "<color=yellow>PING: " + ping + "</color>";
            else if (ping == 0)
                return "<color=grey>PING: " + ping + "</color>";
            else
                return "<color=green>PING: " + ping + "</color>";
        }
        public static string GetFramesColored(this VRC.Player player)
        {
            float fps = (player._playerNet.prop_Byte_0 != 0) ? Mathf.Floor(1000f / (float)player._playerNet.prop_Byte_0) : -1f;
            if (fps > 75)
                return "<color=green>FPS: " + fps + "</color>";
            else if (fps > 30)
                return "<color=yellow>FPS: " + fps + "</color>";
            else if (fps == -1)
                return "<color=grey>FPS: " + fps + "</color>";
            else
                return "<color=red>FPS: " + fps + "</color>";
        }
        public static string GetNameColored(this VRC.Player player)
        {
            var apiuser = player.field_Private_APIUser_0;
            foreach (var tag in apiuser.tags)
            {
                if (tag == "system_legend")
                {
                    return $"<color={LegendaryColor}>{apiuser.displayName}</color>";
                }
            }
            foreach (var tag in apiuser.tags)
            {
                if (tag == "system_trust_legend")
                {
                    return $"<color={VeteranColor}>{apiuser.displayName}</color>";
                }
            }
            if (apiuser.hasVeteranTrustLevel)
            {
                return $"<color={TrustedColor}>{apiuser.displayName}</color>";
            }
            else if (apiuser.hasTrustedTrustLevel)
            {
                return $"<color={KnownColor}>{apiuser.displayName}</color>";
            }
            else if (apiuser.hasKnownTrustLevel)
            {
                return $"<color={UserColor}>{apiuser.displayName}</color>";
            }
            else if (apiuser.hasBasicTrustLevel)
            {
                return $"<color={NewUserColor}>{apiuser.displayName}</color>";
            }
            else if (apiuser.isUntrusted)
            {
                return $"<color={VisitorColor}>{apiuser.displayName}</color>";
            }
            else
            {
                return $"<color={NuisanceColor}>{apiuser.displayName}</color>";
            }
        }
        public static string GetNameColoredQuest(this VRC.Player player)
        {
            var apiuser = player.field_Private_APIUser_0;
            var name = player.field_Private_APIUser_0.displayName;
            if (apiuser.last_platform == "android")
            {
                name = "<color=#404040>[Q]</color> " + name;
            }
            if (VRC.Core.APIUser.IsFriendsWith(apiuser.id))
            {
                name = "<color=#FFCC00>[F]</color> " + name;
            }
            foreach (var tag in apiuser.tags)
            {
                if (tag.Contains("admin_"))
                {
                    return $"<color=#FF0000>[DEV]</color> <color={DevColor}>{name}</color>";
                }
                else if (tag == "system_legend")
                {
                    return $"<color={LegendaryColor}>{name}</color>";
                }
                else if (tag == "system_trust_legend")
                {
                    return $"<color={VeteranColor}>{name}</color>";
                }
            }
            if (apiuser.hasVeteranTrustLevel)
            {
                return $"<color={TrustedColor}>{name}</color>";
            }
            else if (apiuser.hasTrustedTrustLevel)
            {
                return $"<color={KnownColor}>{name}</color>";
            }
            else if (apiuser.hasKnownTrustLevel)
            {
                return $"<color={UserColor}>{name}</color>";
            }
            else if (apiuser.hasBasicTrustLevel)
            {
                return $"<color={NewUserColor}>{name}</color>";
            }
            else if (apiuser.isUntrusted)
            {
                return $"<color={VisitorColor}>{name}</color>";
            }
            else
            {
                return $"<color={NuisanceColor}>{name}</color>";
            }
        }

        public static string GetNameColoredQuestFromTrust(Wrapper.PlayerWrappers.TrustRank trustRank, string name, string userid)
        {
            if (VRC.Core.APIUser.IsFriendsWith(userid))
            {
                name = "<color=#FFCC00>[F]</color> " + name;
            }
            if (trustRank == Wrapper.PlayerWrappers.TrustRank.Admin)
            {
                return $"<color=#FF0000>[DEV]</color> <color={DevColor}>{name}</color>";
            }
            if (trustRank == Wrapper.PlayerWrappers.TrustRank.Trusted)
            {
                return $"<color={TrustedColor}>{name}</color>";
            }
            else if (trustRank == Wrapper.PlayerWrappers.TrustRank.Known)
            {
                return $"<color={KnownColor}>{name}</color>";
            }
            else if (trustRank == Wrapper.PlayerWrappers.TrustRank.User)
            {
                return $"<color={UserColor}>{name}</color>";
            }
            else if (trustRank == Wrapper.PlayerWrappers.TrustRank.New)
            {
                return $"<color={NewUserColor}>{name}</color>";
            }
            else if (trustRank == Wrapper.PlayerWrappers.TrustRank.Visitor)
            {
                return $"<color={VisitorColor}>{name}</color>";
            }
            else
            {
                return $"<color={NuisanceColor}>{name}</color>";
            }
        }
        public static string GetPlayerListName(VRC.Player player)
        {
            var output = "";
            var addspace = false;
            if (player.IsYou() != "") { output += player.IsYou(); addspace = true; }
            if (addspace)
            {
                if (player.GetPlatform() != "") output += " " + player.GetPlatform();
            }
            else
            {
                if (player.GetPlatform() != "") output += player.GetPlatform();
            }
            if (player.IsInstanceHost() != "") output += " " + player.IsInstanceHost();
            if (player.IsInstanceMaster() != "") output += " " + player.IsInstanceMaster();
            if (player.IsVRCPlusUser() != "") output += " " + player.IsVRCPlusUser();
            if (player.IsSelectedUser() != "") output += " " + player.IsSelectedUser();
            if (player.IsClientUser() != "") output += " " + player.IsClientUser();
            if (player.IsDev() != "") output += " " + player.IsDev();
            if (player.IsFriend() != "") output += " " + player.IsFriend();
            output += " " + player.GetNameColored() + " | ";
            output += player.GetPingColored() + " | ";
            output += player.GetFramesColored() + "";
            return output;
        }
        public static void ClearPlayerList(bool remake)
        {
            try
            {
                if (PlayerListTexts.Count == 0) return;

                foreach (GameObject text in PlayerListTexts)
                {
                    PlayerListTexts.Remove(text);
                    GameObject.DestroyImmediate(text);
                }
                if (remake) PlayerListUpdate1Time();
            }
            catch { }
        }
        public static void UpdatePlayerCount()
        {
            if (VRC_PLScrollRect.PlayerCount == null || VRC_PLScrollRect.QuestCount == null)
                return;

            VRC_PLScrollRect.PlayerCount_T.text = "Players: " + Wrapper.PlayerWrappers.GetAllPlayers()._size;
            VRC_PLScrollRect.QuestCount_T.text = "Quest Users: " + Wrapper.PlayerWrappers.GetAllQuestUsers()._size;
        }
        public static List<GameObject> PlayerListTexts = new List<GameObject>();
        public static System.Collections.IEnumerator PlayerListUpdate()
        {
            for (; ; )
            {
                if (!QM_PlayerListAndDebugLog.PlayerList.enabled)
                {
                    yield break;
                }
                else if (VRC_PLScrollRect.CloneableText != null && VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0 != null && VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count > 0)
                {
                    if (VRC_PLScrollRect.TempText != null)
                    {
                        GameObject.DestroyImmediate(VRC_PLScrollRect.TempText);
                    }
                    if (!QM_PlayerListAndDebugLog.PlayerList.Scroll.activeSelf)
                    {
                        QM_PlayerListAndDebugLog.PlayerList.Scroll.SetActive(true);
                    }
                    try
                    {
                        UpdatePlayerCount();

                        var num = 0;
                        foreach (VRC.Player player in VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
                        {
                            if (player != null && player.prop_APIUser_0 != null && player.field_Private_VRCPlayerApi_0 != null)
                            {
                                if (PlayerListTexts.Count > num)
                                {
                                    var ListItem = PlayerListTexts[num];
                                    var Text = ListItem.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
                                    Text.text = GetPlayerListName(player);
                                }
                                else
                                {
                                    var ListItem = GameObject.Instantiate(VRC_PLScrollRect.CloneableText, VRC_PLScrollRect.VerticalLayout.transform);
                                    ListItem.name = "PlayerListItem";
                                    var Text = ListItem.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
                                    Text.text = GetPlayerListName(player);
                                    Text.enableWordWrapping = false;
                                    ListItem.SetActive(true);

                                    PlayerListTexts.Add(ListItem);
                                }
                                num++;
                            }
                        }
                        if (num > VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count)
                        {
                            ClearPlayerList(true);
                        }
                    }
                    catch (Exception e)
                    {
                        PendulumLogger.LogErrorSevere("Error with playerlist: " + e);
                        //ClearPlayerList(false);
                    }
                }
                else
                {
                    ClearPlayerList(false);
                }
                yield return new WaitForSecondsRealtime(0.5f);
            }
            yield break;
        }

        public static void PlayerListUpdate1Time()
        {
            if (VRC_PLScrollRect.CloneableText != null && VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0 != null && VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count > 0)
            {
                if (VRC_PLScrollRect.TempText != null)
                {
                    GameObject.DestroyImmediate(VRC_PLScrollRect.TempText);
                }

                try
                {
                    UpdatePlayerCount();

                    var num = 0;
                    foreach (VRC.Player player in VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
                    {
                        if (player != null && player.prop_APIUser_0 != null && player.field_Private_VRCPlayerApi_0 != null)
                        {
                            if (PlayerListTexts.Count > num)
                            {
                                var ListItem = PlayerListTexts[num];
                                var Text = ListItem.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
                                Text.text = GetPlayerListName(player);
                            }
                            else
                            {
                                var ListItem = GameObject.Instantiate(VRC_PLScrollRect.CloneableText, VRC_PLScrollRect.VerticalLayout.transform);
                                ListItem.name = "PlayerListItem";
                                var text = ListItem.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>();
                                text.text = GetPlayerListName(player);
                                text.enableWordWrapping = false;
                                ListItem.SetActive(true);

                                PlayerListTexts.Add(ListItem);
                            }
                            num++;
                        }
                    }
                    if (PlayerListTexts.Count > VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count)
                    {
                        ClearPlayerList(true);
                    }
                }
                catch
                {
                    ClearPlayerList(false);
                }
            }
            else
            {
                ClearPlayerList(false);
            }
        }
    }
}
