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
        public static APIV2_MenuButton _FlightSpeedButton { get; private set; }

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
            var SelectedUserMenu = MainMenu.AddMenuPage("Selected User Options", "Options for the user you have selected");
            var funstuff = MainMenu.AddMenuPage("FunStuff", "The Funnys");
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
            funstuff.AddToggle("Show Head", "Shows your head", b =>
            {
                Anti.Prefixes.showHead = b;
            });
            FunctionMenu.AddToggle("Serialize", "Move around while others see you standing still", b => {
                PendulumClientMain.SerializePlayer(b);
            });
            ExploitsMenu.AddToggle("Event9", "we do a slight amount of trolling", b => {
                MenuFunctions.EnableEvent9(b);
            });
            ExploitsMenu.AddToggle("Event209", "we do a slight amount of trolling", b => {
                MenuFunctions.EnableEvent209(b);
            });
            ExploitsMenu.AddToggle("Big Uspeak Spammer", "we do a massive amount of trolling", b => {
                MenuFunctions.EnableEventBigUspeak(b);
            });
            ExploitsMenu.AddButton("Small Event9", "we do a small amount of trolling", () => {
                MenuFunctions.SmallEvent9();
            });
            ExploitsMenu.AddButton("big uspeak", "deez", () => 
            {
                //MelonCoroutines.Start(MenuFunctions.BigUSpeak());
                byte[] VoiceData = Convert.FromBase64String("AgAAAKWkyYm7hjsA+H3owFygUv4w5B67lcSx14zff9FCPADiNbSwYWgE+O7DrSy5tkRecs21ljjofvebe6xsYlA4cVmgrd0=");
				byte[] nulldata = new byte[4];
				byte[] ServerTime = BitConverter.GetBytes(VRC.SDKBase.Networking.GetServerTimeInMilliseconds());
				Buffer.BlockCopy(nulldata, 0, VoiceData, 0, 4);
				Buffer.BlockCopy(ServerTime, 0, VoiceData, 4, 4);
				for (int i = 0; i < 10; i++)
				{
					Anti.PhotonExtensions.OpRaiseEvent(1, VoiceData, new Photon.Realtime.RaiseEventOptions
					{
						field_Public_ReceiverGroup_0 = 0,
						field_Public_EventCaching_0 = 0
					}, default(ExitGames.Client.Photon.SendOptions));
				}
                //var data = new byte[] { 0, 1, 0, 0, 0, 255, 255, 255, 255, 1, 0, 0, 0, 0, 0, 0, 0, 15, 1, 0, 0, 0, 203, 0, 0, 0, 2, 2, 0, 0, 0, /*end8*/255, 251, 145, 127, 204, 44, 46, 0 };
                //var data7 = new byte[] { };
                //PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data5, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                /*var data6 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data7 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data8 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data9 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data10 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data11 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data12 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data13 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data14 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data15 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data16 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data17 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data18 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);
                var data19 = new byte[] { };
                PendulumClient.Anti.PhotonExtensions.OpRaiseEvent(1, data, Anti.PhotonExtensions.UnreliableEventOptions, Anti.PhotonExtensions.UnreliableOptions);*/
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
                var plr = MenuFunctions.GetSelectedPlayer();
                if (plr == null)
                    return;

                PendulumClientMain.ForceClone(plr);
            });
            TargetMenu.AddButton("Copy UserID", "Copy UserID to Clipboard", () => {
                var plr = MenuFunctions.GetSelectedPlayer();
                if (plr == null)
                    return;

                System.Windows.Forms.Clipboard.SetText(plr.field_Private_APIUser_0.id);
                PendulumClientMain.VRC_UIManager.QueueHudMessage("Copied UserID!");
            });
            TargetMenu.AddButton("Select User", "Select this user for the client to target", () =>
            {
                var plr = MenuFunctions.GetSelectedPlayer();
                if (plr == null)
                    return;

                PendulumClientMain.SaveUserID(plr.field_Private_APIUser_0);
            });
            var DownloadsMenu = FunctionMenu.AddMenuPage("VRCA/VRCW Downloads", "Download World and Avatar .VRCAs");
            DownloadsMenu.AddToggle("Open Folder on Download", "Open File Explorer to your .vrca/.vrcw on download", b => 
            {
                MenuFunctions.OpenFileOnDownload = b;
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
                MenuFunctions.OpenFolder(Environment.CurrentDirectory + "/PendulumClient/VRCA");
            });
            DownloadsMenu.AddButton("Open VRCW Folder", "Open the folder containing all of your downloaded VRCWs", () =>
            {
                MenuFunctions.OpenFolder(Environment.CurrentDirectory + "/PendulumClient/VRCW");
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
            FunctionMenu.AddToggle("PlayerESP", "Shows people through walls", b =>
            {
                MenuFunctions.PlayerESP(b);
            });
            SelectedUserMenu.AddButton("Drop Sideways Portal", "Drop Sideways Portal on User", () =>
            {
                var plr = MenuFunctions.GetSelectedUser();
                if (plr == null)
                {
                    PendulumClientMain.VRC_UIManager.QueueHudMessage("No Selected User Found!");
                    return;
                }

                PortalDrops.DropPortalInvalidSidewaysOnPlayerHead(plr);
            });

            SelectedUserMenu.AddToggle("Copy Voice", "Drop Sideways Portal on User", b =>
            {
                var plr = MenuFunctions.GetSelectedUser();
                if (plr == null)
                {
                    PendulumClientMain.VRC_UIManager.QueueHudMessage("No Selected User Found!");
                    MenuFunctions.CopyVoice(false, null);
                    return;
                }

                MenuFunctions.CopyVoice(b, plr);
                if (b)
                    PendulumClientMain.VRC_UIManager.QueueHudMessage("Copying " + plr.field_Private_APIUser_0.displayName + "\'s voice");
            });
        }
    }
}