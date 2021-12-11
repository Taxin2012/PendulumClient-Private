using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRC.UI.Elements;
using PendulumClient.UI;
using PendulumClient.Main;
using MelonLoader;
using PendulumClient.Wrapper;
using Transmtn.DTO.Notifications;
using Transmtn;
using Transmtn.DTO;

namespace PendulumClient.ButtonAPIV2
{
    public class MenuSetup
    {
        public static APIV2_ButtonPageInterface MainMenu { get; set; }
        public static APIV2_ButtonPageInterface TargetMenu { get; set; }
        public static Anti.ret_9 _ret { get; set; }
        public static Anti.ret_209 _ret2 { get; set; }
        public static APIV2_MenuButton _FlightSpeedButton { get; private set; }

        public static object dsobj = null;
        public static object ds2obj = null;

        public static bool OpenFileOnDownload = false;

        public static void EnableEvent9(bool t)
        {
            if (t)
            {
                if (_ret == null)
                    _ret = new Anti.ret_9();

                _ret.toggled = true;
                if (!_ret.started)
                {
                    dsobj = MelonCoroutines.Start(_ret.Desync());
                }
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Event9 Enabled");
            }
            else
            {
                _ret.toggled = false;
                if (_ret.started && dsobj != null)
                {
                    MelonCoroutines.Stop(dsobj);
                }
                _ret = null;
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Event9 Disabled");
            }
        }
        public static void EnableEvent209(bool t)
        {
            if (t)
            {
                if (_ret2 == null)
                    _ret2 = new Anti.ret_209();

                _ret2.toggled = true;
                if (!_ret2.started)
                {
                    ds2obj = MelonCoroutines.Start(_ret2.Desync());
                }
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Event209 Enabled");
            }
            else
            {
                _ret2.toggled = false;
                if (_ret2.started && ds2obj != null)
                {
                    MelonCoroutines.Stop(ds2obj);
                }
                _ret2 = null;
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Event209 Disabled");
            }
        }

        private static void OpenFolder(string folderPath)
        {
            if (System.IO.Directory.Exists(folderPath))
            {
                string p = "";
                foreach (var chr in folderPath)
                {
                    char newchr = '0';
                    if (chr == '/')
                    {
                        newchr = '\\';
                    }
                    else
                    {
                        newchr = chr;
                    }
                    p += newchr;
                }

                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    Arguments = p,
                    FileName = "explorer.exe"
                };

                System.Diagnostics.Process.Start(startInfo);
            }
        }

        public static VRC.Player GetSelectedPlayer()
        {
            var selectedUser = NewQuickMenu.Instance.field_Private_UIPage_1.Cast<VRC.UI.Elements.Menus.SelectedUserMenuQM>().field_Private_IUser_0;
            if (selectedUser == null)
                return null;

            var player = VRC.PlayerManager.field_Private_Static_PlayerManager_0.GetPlayer(selectedUser.prop_String_0);
            return player;
        }
        public static void SetupMenu(VRCUiManager uimanager, Sprite icon, string menuname = "PendulumClient")
        {
            if (uimanager == null) return;
            if (icon == null) return;

            MainMenu = new APIV2_MenuPage(menuname, true);
            APIV2_TabButton.Create(menuname, $"Open the {menuname} menu.", menuname, icon);
            TargetMenu = new APIV2_MenuCategory($"{menuname}", NewQuickMenu.Instance.field_Private_UIPage_1.transform.Find("ScrollRect").GetComponent<ScrollRect>().content, JoinNotifier.JoinNotifierMod.PendulumSprite);
            var FunctionMenu = MainMenu.AddMenuPage("Functions", "stuff");
            var MovementMenu = MainMenu.AddMenuPage("Movement", "move");
            var ProtectionMenu = MainMenu.AddMenuPage("Protections", "protec yourself");
            var ExploitsMenu = MainMenu.AddMenuPage("Exploits", "boo corbin");
            /*MainMenu.AddButton("sheesh", "we do a little trolling", () =>
            {
                PendulumClientMain.VRC_UIManager.QueueHudMessage("sheesh");
            });*/ // test button
            var FlightCategoryPage = MovementMenu.AddCategoryPage("Flight Menu");
            var flightcategory = FlightCategoryPage.AddCategory("Flight Options", true);
            flightcategory.AddToggle("Flight", "Enable Flight", b => {
                PendulumClientMain.FlightEnabled = b;
                if (PendulumClientMain.FlightEnabled)
                {
                    if (PendulumClientMain.NoClip)
                    {
                        PendulumClientMain.ToggleColliders(false);
                    }
                    Physics.gravity = Vector3.zero;
                    PendulumClientMain.VRC_UIManager.QueueHudMessage("Flight Enabled");
                }
                else
                {
                    PendulumClientMain.ToggleColliders(true);
                    Physics.gravity = PendulumClientMain.Grav;
                    //PendulumClientMain.NoClip = false;
                    PendulumClientMain.VRC_UIManager.QueueHudMessage("Flight Disabled");
                }
            });
            flightcategory.AddToggle("NoClip", "Enable NoClip while flying", b => {
                PendulumClientMain.NoClip = b;
            });
            flightcategory.AddToggle("Flight Keybind", "Enable Flight by pressing X", b => {
                PendulumClientMain.FlightKeybind = b;
            }, !Login.IsInVR);
            var speedcategory = FlightCategoryPage.AddCategory("Speed Options", true);
            speedcategory.AddButton("<size=200%>+</size>", "Increase Flight Speed", () => 
            {
                PendulumClientMain.FlightSpeed = PendulumClientMain.FlightSpeed + 1;
                _FlightSpeedButton.Text = "(" + PendulumClientMain.FlightSpeed + ")\n<size=33%>Flight Speed</size>";
            });
            _FlightSpeedButton = speedcategory.AddButton("(" + PendulumClientMain.FlightSpeed + ")\n<size=33%>Flight Speed</size>", "Set Flight Speed", () => 
            {
                //var sped = InputPopup.SendInputPopupReturnInt("Enter Flight Speed", "8");
                InputPopup.ShowInputPopupWithCancel(VRCUiPopupManager.prop_VRCUiPopupManager_0, "Enter Flight Speed", "8", InputField.InputType.Standard, false, "Submit", (s, k, t) =>
                {
                    if (string.IsNullOrEmpty(s))
                        return;

                    if (!int.TryParse(s, out var outnum))
                        return;

                    if (outnum > 0)
                    {
                        PendulumClientMain.FlightSpeed = outnum;
                    }
                    _FlightSpeedButton.Text = "(" + PendulumClientMain.FlightSpeed + ")\n<size=33%>Flight Speed</size>";
                }, null);
            });
            //_FlightSpeedButton.Interactable = false;
            speedcategory.AddButton("<size=200%><voffset=0.20em>-</size></voffset>", "Decrease Flight Speed", () =>
            {
                if (PendulumClientMain.FlightSpeed > 1)
                {
                    PendulumClientMain.FlightSpeed = PendulumClientMain.FlightSpeed - 1;
                }
                _FlightSpeedButton.Text = "(" + PendulumClientMain.FlightSpeed + ")\n<size=33%>Flight Speed</size>";
            });
            FunctionMenu.AddToggle("Serialize", "Move around while others see you standing still", b => {
                PendulumClientMain.SerializePlayer(b);
            });
            ExploitsMenu.AddToggle("Event9", "we do a slight amount of trolling", b => {
                EnableEvent9(b);
            });
            ExploitsMenu.AddToggle("Event209", "we do a slight amount of trolling", b => {
                EnableEvent209(b);
            });
            ProtectionMenu.AddToggle("Anti Event9", "Block all Code9 Photon Events\n(you cant see avatar 3.0 changes)", b => {
                Anti.Prefixes.Anti9 = b;
            });
            ProtectionMenu.AddToggle("Anti Event209", "Block all Code209 Photon Events", b => {
                Anti.Prefixes.Anti209 = b;
            });
            ProtectionMenu.AddButton("Delete Portals", "Delete all Portals", () =>
            {
                var intg = PendulumClientMain.delete_portals();
                if (intg == 1)
                {
                    PendulumClientMain.VRC_UIManager.QueueHudMessage("Deleted " + intg + " Portal");
                }
                else
                {
                    PendulumClientMain.VRC_UIManager.QueueHudMessage("Deleted " + intg + " Portals");
                }
            });
            ProtectionMenu.AddToggle("Auto Delete Portals", "Always Delete Portals", b =>
            {
                if (b)
                {
                    PendulumClientMain.delete_portals();
                    PendulumClientMain.DeleteNewPortals = true;
                }
                else
                {
                    PendulumClientMain.DeleteNewPortals = false;
                }
            });
            ProtectionMenu.AddToggle("Anti Portal", "Blocks you from entering portals\nalso displays who dropped the portal", b =>
            {
                PendulumClientMain.AntiPortalToggle = b;
            });
            FunctionMenu.AddButton("Copy InstanceID", "Copy InstanceID to Clipboard", () =>
            {
                var WorldID = RoomManager.field_Internal_Static_ApiWorld_0.id + ":" + RoomManager.field_Internal_Static_ApiWorldInstance_0.instanceId;
                System.Windows.Forms.Clipboard.SetText(WorldID);
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Copied InstanceID!");
            });
            FunctionMenu.AddButton("Copy InstanceID (Region)", "Copy InstanceID to Clipboard", () =>
            {
                var WorldID = RoomManager.field_Internal_Static_ApiWorld_0.id + ":" + RoomManager.field_Internal_Static_ApiWorldInstance_0.instanceId;
                if (!WorldID.Contains("~region("))
                {
                    WorldID += "~region(" + PendulumClientMain.GetRegion(RoomManager.field_Internal_Static_ApiWorldInstance_0) + ")";
                }
                System.Windows.Forms.Clipboard.SetText(WorldID);
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Copied InstanceID!");
            });
            TargetMenu.AddButton("Force Clone", "Force Clone Avatar (If Public)", () => {
                var plr = GetSelectedPlayer();
                if (plr == null)
                    return;

                PendulumClientMain.ForceClone(plr);
            });
            TargetMenu.AddButton("Copy UserID", "Copy UserID to Clipboard", () => {
                var plr = GetSelectedPlayer();
                if (plr == null)
                    return;

                System.Windows.Forms.Clipboard.SetText(plr.field_Private_APIUser_0.id);
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Copied UserID!");
            });
            var DownloadsMenu = FunctionMenu.AddMenuPage("VRCA/VRCW Downloads", "Download World and Avatar .VRCAs");
            DownloadsMenu.AddToggle("Open Folder on Download", "Open File Explorer to your .vrca/.vrcw on download", b => 
            {
                OpenFileOnDownload = b;
            });
            DownloadsMenu.AddButton("Download Current .VRCA", "Download your current avatars .VRCA", () =>
            {
                PendulumClientMain.DowloadVRCA(VRCPlayer.field_Internal_Static_VRCPlayer_0._player);
            });
            DownloadsMenu.AddButton("Download Current .VRCW", "Download your current worlds .VRCW", () =>
            {
                PendulumClientMain.DowloadVRCW(RoomManager.field_Internal_Static_ApiWorld_0);
            });
            DownloadsMenu.AddButton("Open VRCA Folder", "Open the folder containing all of your downloaded VRCAs", () =>
            {
                OpenFolder(Environment.CurrentDirectory + "/PendulumClient/VRCA");
            });
            DownloadsMenu.AddButton("Open VRCW Folder", "Open the folder containing all of your downloaded VRCWs", () =>
            {
                OpenFolder(Environment.CurrentDirectory + "/PendulumClient/VRCW");
            });
            FunctionMenu.AddButton("Join InstanceID", "Joins an instance by ID", () =>
            {
                //VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Invite.Create(userInfo.field_Private_APIUser_0.id, "", new Location(a, new Transmtn.DTO.Instance(b, "", "", "", "", false)), WorldName));
                InputPopup.ShowInputPopupWithCancel(VRCUiPopupManager.prop_VRCUiPopupManager_0, "Enter InstanceID", "", InputField.InputType.Standard, false, "Submit", (s, k, t) =>
                {
                    if (string.IsNullOrEmpty(s))
                        return;

                    if (!s.Contains("wrld_") && !s.Contains(":"))
                        return;

                    if (s.Length > 6)
                    {
                        var userInfo = VRC.Core.APIUser.CurrentUser;
                        var worldidlist = s.Split(':');
                        var a = worldidlist[0];
                        var b = worldidlist[1];
                        VRCWebSocketsManager.field_Private_Static_VRCWebSocketsManager_0.prop_Api_0.PostOffice.Send(Invite.Create(userInfo.id, "", new Location(a, new Transmtn.DTO.Instance(b)), "" ));
                    }
                }, null);
            });
        }
    }
}