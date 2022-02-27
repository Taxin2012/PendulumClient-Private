using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PendulumClient.QMLogAndPlayerlist
{
    public static class PlayerlistV2
    {
        public const string QuestColor = "grey";
        public const string VRColor = "#00c8ffff";
        public const string PCColor = "#2bc494ff";
        public const string ClientUserColor = "#c43a2bff";
        public const string DevColor = "#ff0000ff";
        public const string FriendColor = "#ffd000ff";
        public const string MasterColor = "#ffb300ff";
        public const string HostColor = "#ffb300ff";
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
            if ((fps > 90 || fps < 1 || player._playerNet.field_Private_Int16_0 < 0) && fps != -1f)
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
            foreach (var tag in player.field_Private_APIUser_0.tags)
            {
                if (tag == "system_legend")
                {
                    return $"<color={LegendaryColor}>{player.field_Private_APIUser_0.displayName}</color>";
                }
            }
            foreach (var tag in player.field_Private_APIUser_0.tags)
            {
                if (tag == "system_trust_legend")
                {
                    return $"<color={VeteranColor}>{player.field_Private_APIUser_0.displayName}</color>";
                }
            }
            if (player.prop_APIUser_0.hasVeteranTrustLevel)
            {
                return $"<color={TrustedColor}>{player.field_Private_APIUser_0.displayName}</color>";
            }
            else if (player.prop_APIUser_0.hasTrustedTrustLevel)
            {
                return $"<color={KnownColor}>{player.field_Private_APIUser_0.displayName}</color>";
            }
            else if (player.prop_APIUser_0.hasKnownTrustLevel)
            {
                return $"<color={UserColor}>{player.field_Private_APIUser_0.displayName}</color>";
            }
            else if (player.prop_APIUser_0.hasBasicTrustLevel)
            {
                return $"<color={NewUserColor}>{player.field_Private_APIUser_0.displayName}</color>";
            }
            else if (player.prop_APIUser_0.isUntrusted)
            {
                return $"<color={VisitorColor}>{player.field_Private_APIUser_0.displayName}</color>";
            }
            else
            {
                return $"<color={NuisanceColor}>{player.field_Private_APIUser_0.displayName}</color>";
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
        public static List<GameObject> PlayerListTexts = new List<GameObject>();
        public static System.Collections.IEnumerator PlayerListUpdate()
        {
            for (;;)
            {
                if (VRC_PLScrollRect.CloneableText != null && VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0 != null && VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0.Count > 0)
                {
                    if (VRC_PLScrollRect.TempText != null)
                    {
                        GameObject.DestroyImmediate(VRC_PLScrollRect.TempText);
                    }

                    try
                    {
                        var num = 0;
                        foreach (VRC.Player player in VRC.PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
                        {
                            if (player != null)
                            {
                                if (player.prop_APIUser_0 != null)
                                {
                                    if (player.field_Private_VRCPlayerApi_0 != null)
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
                                    else
                                    {
                                        //PendulumLogger.Log("PlayerAPI is null");
                                    }
                                }
                                else
                                {
                                    //PendulumLogger.Log("APIUser is null");
                                }
                            }
                            else
                            {
                                //PendulumLogger.Log("Player is null");
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
    public class VRC_PLScrollRect
    {
        public GameObject Scroll;
        public GameObject BG;
        public GameObject Viewport;
        public static GameObject VerticalLayout;
        public static GameObject CloneableText;
        public static GameObject TempText;
        public static GameObject PlayerCount;
        public string name { get; private set; }
        public VRC_PLScrollRect(string filtername, string internalname, float x, float y, Transform parent)
        {
            Scroll = UnityEngine.Object.Instantiate<GameObject>(ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window/QMParent/Menu_Dashboard/ScrollRect").gameObject, parent);
            //BG = UnityEngine.Object.Instantiate<GameObject>(ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window/QMParent/BackgroundLayer01").gameObject, lable.transform.Find("Viewport/VerticalLayoutGroup"));
            //BG.name = "NewBGLayer";
            Scroll.name = internalname;
            name = filtername;
            Scroll.transform.localPosition = new Vector3(x, y, 0);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/VRC+_Banners").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Carousel_Banners").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Buttons_QuickLinks").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Buttons_QuickActions").gameObject);
            Viewport = Scroll.transform.Find("Viewport").gameObject;
            VerticalLayout = Viewport.transform.Find("VerticalLayoutGroup").gameObject;
            Viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(4096f, 0f);
            Viewport.GetComponent<RectMask2D>().enabled = true;
            Scroll.GetComponent<ScrollRect>().enabled = true;
            ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window").GetComponent<BoxCollider>().size = new Vector3(4096f, 1024f, 1f);
            Scroll.AddComponent<Button>();
            Scroll.AddComponent<VRC.UI.Core.Styles.StyleElement>();
            VerticalLayout.GetComponent<RectTransform>().localPosition += new Vector3(-40f, 0f);


            var tempqa = VerticalLayout.transform.Find("Header_QuickActions").gameObject;
            tempqa.name = "PC_CloneablePlayerListText";
            tempqa.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().text = "EMPTY";
            tempqa.SetActive(false);
            CloneableText = tempqa;
            var temptext = VerticalLayout.transform.Find("Header_QuickLinks").gameObject;
            temptext.name = "DontDestroy";
            temptext.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().text = "EMPTY";
            TempText = temptext;

            VerticalLayout.GetComponent<VerticalLayoutGroup>().childControlHeight = true;

            Scroll.gameObject.SetActive(true);

            SetupPlayerCount(CloneableText, Scroll.gameObject.transform);
        }

        private void SetupPlayerCount(GameObject textobj, Transform parent)
        {
            var PlayerCountText = GameObject.Instantiate(textobj, parent);
            PlayerCountText.name = "PC_PlayerCounter";
            //UnityEngine.Object.Destroy(PlayerCountText.GetComponent<LayoutElement>());
            PlayerCountText.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().text = "Players: NULL";
            PlayerCountText.SetActive(true);

            PlayerCount = PlayerCountText;
        }
    }

    public class VRC_DebugScrollRect
    {
        public GameObject Scroll;
        public GameObject BG;
        public GameObject Viewport;
        public static GameObject VerticalLayout;
        public static GameObject CloneableText;
        public static GameObject TempText;
        public string name { get; private set; }
        public VRC_DebugScrollRect(string filtername, string internalname, float x, float y, Transform parent)
        {
            Scroll = UnityEngine.Object.Instantiate<GameObject>(ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window/QMParent/Menu_Dashboard/ScrollRect").gameObject, parent);
            //BG = UnityEngine.Object.Instantiate<GameObject>(ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window/QMParent/BackgroundLayer01").gameObject, lable.transform.Find("Viewport/VerticalLayoutGroup"));
            //BG.name = "NewBGLayer";
            Scroll.name = internalname;
            name = filtername;
            Scroll.transform.localPosition = new Vector3(x, y, 0);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/VRC+_Banners").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Carousel_Banners").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Buttons_QuickLinks").gameObject);
            GameObject.Destroy(Scroll.transform.Find("Viewport/VerticalLayoutGroup/Buttons_QuickActions").gameObject);
            Viewport = Scroll.transform.Find("Viewport").gameObject;
            VerticalLayout = Viewport.transform.Find("VerticalLayoutGroup").gameObject;
            Viewport.GetComponent<RectTransform>().sizeDelta = new Vector2(4096f, 0f);
            Viewport.GetComponent<RectMask2D>().enabled = true;
            Scroll.GetComponent<ScrollRect>().enabled = true;
            ButtonAPIV2.NewQuickMenu.Instance.transform.Find("Container/Window").GetComponent<BoxCollider>().size = new Vector3(4096f, 1024f, 1f);
            Scroll.AddComponent<Button>();
            Scroll.AddComponent<VRC.UI.Core.Styles.StyleElement>();
            VerticalLayout.GetComponent<RectTransform>().localPosition += new Vector3(-40f, 0f);


            var tempqa = VerticalLayout.transform.Find("Header_QuickActions").gameObject;
            tempqa.name = "PC_CloneableDebugLogText";
            tempqa.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().text = "EMPTY";
            tempqa.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
            tempqa.SetActive(false);
            CloneableText = tempqa;
            var temptext = VerticalLayout.transform.Find("Header_QuickLinks").gameObject;
            temptext.name = "DontDestroy";
            temptext.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().text = "EMPTY";
            temptext.transform.Find("LeftItemContainer/Text_Title").gameObject.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
            TempText = temptext;

            VerticalLayout.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
            VerticalLayout.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.LowerCenter;

            Scroll.gameObject.SetActive(true);
        }
    }
}
