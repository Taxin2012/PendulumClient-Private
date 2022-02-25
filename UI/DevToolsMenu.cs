using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics;
using System.IO;
using MelonLoader;
using PendulumClient.Wrapper;
using PendulumClient.Main;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using PendulumClient.ButtonAPIInc;
using PendulumClient.Anti;
using Viveport;
using VRC.Core;
using VRC.SDKBase;
using VRC;
using VRC.UI;
using Valve.VR;
using VRTK;
using UnityEngine.Rendering;
using VRC.Management;

using ModerationManager = VRC.Management.ModerationManager;
using Player = VRC.Player;

namespace PendulumClient.UI
{
    class DevToolsMenu
    {
        public static Text PortalDistance;
        public static Text FlySpeed;

        public static GameObject VRCAButton = null;
        public static void SetupDevToolsMenu()
        {
            var ModerationMenu = PendulumClientMain.PendulumClientMenu.transform;
            var ProtMenu = PendulumClientMain.ProtectionsMenu.transform;
            var FuncMenu = PendulumClientMain.FunctionsMenu.transform;
            var SelectedMenu = PendulumClientMain.SelectedUserMenu.transform;
            if (ModerationMenu != null)
            {
                GameObject RPCCrash = ButtonAPI.CreateButton(ButtonType.Toggle, "Quest Lagger", "Spams Emoji RPC and Emote RPC.", Color.white, Color.white, -2f, -1f, ModerationMenu.transform, delegate
                {
                    PendulumClientMain.RPC_Crash = true;
                    PendulumLogger.Log("Quest Crash Enabled!");
                    AlertPopup.SendAlertPopup("Quest Crash Enabled!");
                }, delegate
                {
                    PendulumClientMain.RPC_Crash = false;
                    PendulumLogger.Log("Quest Crash Disabled!");
                    AlertPopup.SendAlertPopup("Quest Crash Disabled!");
                });

                GameObject worldtriggers = ButtonAPI.CreateButton(ButtonType.Toggle, "World Triggers", "Make all trigger interaction world.", Color.white, Color.white, -1f, -1f, ModerationMenu.transform, delegate
                {
                    Prefixes.WorldTriggers = true;
                    PendulumLogger.Log("World Triggers Enabled!");
                    AlertPopup.SendAlertPopup("World Triggers Enabled!");
                }, delegate
                {
                    Prefixes.WorldTriggers = false;
                    PendulumLogger.Log("World Triggers Disabled!");
                    AlertPopup.SendAlertPopup("World Triggers Disabled!");
                });

                GameObject DisconnectLoop = ButtonAPI.CreateButton(ButtonType.Toggle, "AddHealthRPC Spam", "Will disconnect the whole lobby after about 5mins of running.", Color.white, Color.white, -3f, 0f, ModerationMenu.transform, delegate
                {
                    PendulumClientMain.DisconnectOn = true;
                    //PendulumLogger.Log("PortalLoop Enabled!");
                    //AlertPopup.SendAlertPopup("PortalLoop Enabled!");
                }, delegate
                {
                    PendulumClientMain.DisconnectOn = false;
                    //PendulumLogger.Log("PortalLoop Disabled!");
                    //AlertPopup.SendAlertPopup("PortalLoop Disabled!");
                });

                GameObject Serialize = ButtonAPI.CreateButton(ButtonType.Toggle, "Serialize", "Freezes you in place for others.", Color.white, Color.white, -2f, 0f, ModerationMenu.transform, delegate
                {
                    PendulumClientMain.SerializePlayer(true);
                    PendulumLogger.Log("Serialization Enabled!");
                    AlertPopup.SendAlertPopup("Serialization Enabled!");
                }, delegate
                {
                    PendulumClientMain.SerializePlayer(false);
                    PendulumClientMain.CloseMenu();
                    PendulumLogger.Log("Serialization Disabled!");
                    AlertPopup.SendAlertPopup("Serialization Disabled!");
                });

                GameObject gameObject2 = ButtonAPI.CreateButton(ButtonType.Default, "Delete\nPortals", "Delete All Portals In Current Instance.", Color.white, Color.white, 1f, 2f, ModerationMenu, delegate
                {
                    PendulumClientMain.delete_portals();
                });
            }

            if (FuncMenu != null)
            {
                var menubutton = ButtonAPI.CreateButton(ButtonType.Default, "Functions", "Main Functions.", Color.white, Color.white, -3f, 3f, ModerationMenu.transform,
                new Action(() =>
                {
                    FuncMenu.gameObject.SetActive(true);
                    ModerationMenu.gameObject.SetActive(false);
                    PendulumClientMain.DevToolsMenuOpen = true;
                }),

                new Action(() =>
                {

                }));

                GameObject jumpbutton = ButtonAPI.CreateButton(ButtonType.Default, "Enable Jump", "this man got hops", Color.white, Color.white, -4f, 2f, FuncMenu, delegate
                {
                    var currentplayer = PlayerWrappers.GetCurrentPlayer();
                    if (currentplayer.gameObject.GetComponent<PlayerModComponentJump>() != null)
                    {
                        AlertPopup.SendAlertPopup("Jumping is already enabled.");
                    }
                    else
                    {
                        var comp = currentplayer.gameObject.AddComponent<PlayerModComponentJump>();
                        comp.field_Private_Single_0 = 3f;
                        AlertPopup.SendAlertPopup("Enabled jumping in current world.");
                    }
                });

                GameObject bruhmome = ButtonAPI.CreateButton(ButtonType.Default, "Align\nTracking", "foehead", Color.white, Color.white, -4f, 3f, FuncMenu, delegate
                {
                    VRCTrackingManager.Method_Private_Static_Void_0();
                });

                if (!Login.IsInVR)
                {

                    GameObject CameraToggle = ButtonAPI.CreateButton(ButtonType.Toggle, "Desktop Camera", "Enables your camera in desktop.", Color.white, Color.white, -3f, 1f, FuncMenu, delegate
                    {
                        PendulumClientMain.EnableDesktopCamera();
                        //PendulumLogger.Log("PortalLoop Enabled!");
                        //AlertPopup.SendAlertPopup("PortalLoop Enabled!");
                    }, delegate
                    {
                        PendulumClientMain.EnableDesktopCamera(false);
                        //PendulumLogger.Log("PortalLoop Disabled!");
                        //AlertPopup.SendAlertPopup("PortalLoop Disabled!");
                    });
                }

                var backbutton = ButtonAPI.CreateButton(ButtonType.Default, "Back", "We go Back.", Color.yellow, Color.white, 1f, 1f, FuncMenu,
                new Action(() =>
                {
                    FuncMenu.gameObject.SetActive(false);
                    ModerationMenu.gameObject.SetActive(true);

                }),
                new Action(() =>
                {

                }));
            }

            if (ProtMenu != null)
            {
                var menubutton = ButtonAPI.CreateButton(ButtonType.Default, "Protections", "Things to help protec you.", Color.white, Color.white, -2f, 3f, ModerationMenu.transform,
                new Action(() =>
                {
                    ProtMenu.gameObject.SetActive(true);
                    ModerationMenu.gameObject.SetActive(false);
                    PendulumClientMain.DevToolsMenuOpen = true;
                }),

                new Action(() =>
                {

                }));

                var backbutton = ButtonAPI.CreateButton(ButtonType.Default, "Back", "We go Back.", Color.yellow, Color.white, 1f, 1f, ProtMenu,
                new Action(() =>
                {
                    ProtMenu.gameObject.SetActive(false);
                    ModerationMenu.gameObject.SetActive(true);

                }),
                new Action(() =>
                {

                }));
            }

            if (SelectedMenu != null)
            {
                var PlayerText = PendulumClientMain.SetupSelectedPlayerText();
                var menubutton = ButtonAPI.CreateButton(ButtonType.Default, "Selected User\nOptions", "Options for the user you have selected.", Color.white, Color.white, -1f, 3f, ModerationMenu.transform,
                new Action(() =>
                {
                    SelectedMenu.gameObject.SetActive(true);
                    ModerationMenu.gameObject.SetActive(false);
                    PendulumClientMain.DevToolsMenuOpen = true;
                    PlayerText.GetComponent<Text>().text = "Selected Player: " + PendulumClientMain.CheckSelectedPlayer();
                }),

                new Action(() =>
                {

                }));

                var backbutton = ButtonAPI.CreateButton(ButtonType.Default, "Back", "We go Back.", Color.yellow, Color.white, 1f, 1f, SelectedMenu,
                new Action(() =>
                {
                    SelectedMenu.gameObject.SetActive(false);
                    ModerationMenu.gameObject.SetActive(true);

                }),
                new Action(() =>
                {

                }));

                bool hasbeentoggled = false;
                GameObject FollowButton = ButtonAPI.CreateButton(ButtonType.Toggle, "Follow", "Follows selected player.", Color.white, Color.white, 0f, 1f, SelectedMenu, delegate
                {
                    if (PendulumClientMain.StoredUserInInstance)
                    {
                        PendulumClientMain.FollowSelected = true;
                        hasbeentoggled = true;
                        PendulumLogger.Log("Follow Enabled!");
                        AlertPopup.SendAlertPopup("Follow Enabled!");
                    }
                    else
                    {
                        PendulumLogger.Log("Stored user is not in instance!");
                        AlertPopup.SendAlertPopup("Stored user is not in instance!");
                    }
                }, delegate
                {
                    if (hasbeentoggled)
                    {
                        PendulumClientMain.FollowSelected = false;
                        hasbeentoggled = false;
                        PendulumLogger.Log("Follow Disabled!");
                        AlertPopup.SendAlertPopup("Follow Disabled!");
                    }
                });

                GameObject ObjectLoop = ButtonAPI.CreateButton(ButtonType.Toggle, "Object Loop", "Spams pickup objects on selected player", Color.white, Color.white, 0f, 0f, SelectedMenu, delegate
                {
                    PendulumClientMain.ObjectLoopOn = true;
                    PendulumLogger.Log("Object Spam Enabled!");
                    //AlertPopup.SendAlertPopup("PortalLoop Enabled!");
                }, delegate
                {
                    PendulumClientMain.ObjectLoopOn = false;
                    PendulumLogger.Log("Object Spam Disabled!");
                    //AlertPopup.SendAlertPopup("PortalLoop Disabled!");
                });

                GameObject DropPotalloop = ButtonAPI.CreateButton(ButtonType.Toggle, "PortalLoop", "Drops portals on Selected User.", Color.white, Color.white, 0f, -1f, SelectedMenu, delegate
                {
                    PendulumClientMain.PortalDropLoop = true;
                    PendulumLogger.Log("PortalLoop Enabled!");
                    AlertPopup.SendAlertPopup("PortalLoop Enabled!");
                }, delegate
                {
                    PendulumClientMain.PortalDropLoop = false;
                    PendulumLogger.Log("PortalLoop Disabled!");
                    AlertPopup.SendAlertPopup("PortalLoop Disabled!");
                });

                PortalDistance = ButtonAPI.make_slider(SelectedMenu, delegate (float v)
                {
                    PendulumClientMain.PortalDropDistance = v;
                    PortalDistance.text = " PortalDrop Distance (" + String.Format("{0:0.##}", PendulumClientMain.PortalDropDistance) + ")";
                }, -2.9f, 2, " PortalDrop Distance (" + String.Format("{0:0.##}", PendulumClientMain.PortalDropDistance) + ")", PendulumClientMain.PortalDropDistance, 10, 0, 420);

                FlySpeed = ButtonAPI.make_slider_int(FuncMenu, delegate (float v)
                {
                    PendulumClientMain.FlightSpeed = Convert.ToInt32(v);
                    FlySpeed.text = " Fly Speed (" + PendulumClientMain.FlightSpeed + ")";
                }, -2.9f, 2, " Fly Speed (" + PendulumClientMain.FlightSpeed + ")", PendulumClientMain.FlightSpeed, 32, 1, 420);
            }
        }

        public static void SetupOptionsMenu()
        {
            var user_menu = PendulumClientMain.user_menu;
            PendulumClientMain.portal_menu = PendulumClientMain.blankmenupage("portal_menu");
            var portal_menu = PendulumClientMain.portal_menu;
            PendulumClientMain.pcu_moderation_menu = PendulumClientMain.blankmenupage("pcu_menu");
            var pcu_menu = PendulumClientMain.pcu_moderation_menu;

            var menubutton = ButtonAPI.CreateButton(ButtonType.Default, "Portal\nDrops", "Portal Drop Options.", Color.white, Color.white, 1f, 2f, user_menu.transform,
            new Action(() =>
            {
                portal_menu.SetActive(true);
                user_menu.gameObject.SetActive(false);
                PendulumClientMain.portal_menu_open = true;
            }),
            new Action(() =>
            {

            }));

            var backbutton = ButtonAPI.CreateButton(ButtonType.Default, "Back", "Go Back.", Color.yellow, Color.white, 1f, 1f, portal_menu.transform,
                new Action(() =>
                {
                    portal_menu.SetActive(false);
                    user_menu.gameObject.SetActive(true);

                }),
                new Action(() =>
                {

                }));

            var menubutton2 = ButtonAPI.CreateButton(ButtonType.Default, "PCUser\nModerations", ".", Color.white, Color.white, 1f, 3f, user_menu.transform,
            new Action(() =>
            {
                pcu_menu.SetActive(true);
                user_menu.gameObject.SetActive(false);
                PendulumClientMain.pcu_menu_open = true;
            }),
            new Action(() =>
            {

            }));

            var backbutton2 = ButtonAPI.CreateButton(ButtonType.Default, "Back", "Go Back.", Color.yellow, Color.white, 1f, 1f, pcu_menu.transform,
                new Action(() =>
                {
                    pcu_menu.SetActive(false);
                    user_menu.gameObject.SetActive(true);

                }),
                new Action(() =>
                {

                }));
            GameObject SelectUserButton = ButtonAPI.CreateButton(ButtonType.Default, "Select\nUser", "Select this user for the client to target.", Color.white, Color.white, -4f, 3f, user_menu.transform, delegate
            {
                var currentplayer = Wrappers.GetQuickMenu().GetSelectedPlayer();
                PendulumClientMain.SaveUserID(currentplayer);
            });
            GameObject PortalOntopButton = ButtonAPI.CreateButton(ButtonType.Default, "Drop Portal\n(Ontop)", "Drop Portal on Selected Player.", Color.white, Color.white, -3f, 3f, portal_menu.transform, delegate
            {
                var currentplayer = Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0;
                var found_player = PlayerWrappers.GetPlayer(currentplayer.id);
                if (found_player != null)
                {
                    PortalDrops.DropPortalOnPlayer(found_player);
                    AlertPopup.SendAlertPopup("Dropped Portal on " + found_player.prop_APIUser_0.displayName);
                }
            });

            GameObject PortalInfrontButton = ButtonAPI.CreateButton(ButtonType.Default, "Drop Portal\n(Infront)", "Drop Portal Infront of Selected Player.", Color.white, Color.white, -2f, 3f, portal_menu.transform, delegate
            {
                var currentplayer = Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0;
                var found_player = PlayerWrappers.GetPlayer(currentplayer.id);
                if (found_player != null)
                {
                    PortalDrops.DropPortalInfrontOfPlayer(found_player);
                    AlertPopup.SendAlertPopup("Dropped Portal infront of\n" + found_player.prop_APIUser_0.displayName);
                }
            });
            GameObject ModManagerValues = ButtonAPI.CreateButton(ButtonType.Default, "TestValues\n(does nothing)", "bruh", Color.white, Color.white, -4f, 2f, user_menu.transform, delegate
            {
                var currentplayer = Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0;
                bool Var1 = ModerationManager.prop_ModerationManager_0.Method_Public_Boolean_APIUser_0(currentplayer);
                bool Var2 = ModerationManager.prop_ModerationManager_0.Method_Public_Boolean_APIUser_1(currentplayer);
                bool Var3 = ModerationManager.prop_ModerationManager_0.Method_Public_Boolean_APIUser_2(currentplayer);
                bool Var4 = ModerationManager.prop_ModerationManager_0.Method_Public_Boolean_APIUser_3(currentplayer);
                bool Var5 = ModerationManager.prop_ModerationManager_0.Method_Public_Boolean_APIUser_4(currentplayer);
                //bool Var6 = ModerationManager.prop_ObjectPublicObLi1ApSiLi1ApBoSiUnique_0.Method_Public_Boolean_APIUser_5(currentplayer);
                bool Var7 = ModerationManager.prop_ModerationManager_0.Method_Public_Boolean_String_0(currentplayer.id);
                /*bool Var8 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_7(currentplayer);
                bool Var9 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_8(currentplayer);
                bool Var10 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_9(currentplayer);
                bool Var11 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_10(currentplayer);
                bool Var12 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_11(currentplayer);
                bool Var13 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_0(currentplayer);
                bool Var14 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_1(currentplayer);
                bool Var15 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_2(currentplayer);
                bool Var16 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_3(currentplayer);*/
                //bool Var17 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_4(currentplayer);
                //bool Var18 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_5(currentplayer);
                //bool Var16 = ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_15(currentplayer);
                PendulumLogger.Log("Variable1: " + Var1);
                PendulumLogger.Log("Variable2: " + Var2);
                PendulumLogger.Log("Variable3: " + Var3);
                PendulumLogger.Log("Variable4: " + Var4);
                PendulumLogger.Log("Variable5: " + Var5);
                // PendulumLogger.Log("Variable6: " + Var6);
                PendulumLogger.Log("Variable7BLS: " + Var7);
                /*PendulumLogger.Log("Variable8: " + Var8);
                PendulumLogger.Log("Variable9: " + Var9);
                PendulumLogger.Log("Variable10: " + Var10);
                PendulumLogger.Log("Variable11: " + Var11);
                PendulumLogger.Log("Variable12: " + Var12);
                PendulumLogger.Log("Variable13: " + Var13);
                PendulumLogger.Log("Variable14: " + Var14);
                PendulumLogger.Log("Variable15: " + Var15);
                PendulumLogger.Log("Variable15: " + Var16);*/
                //PendulumLogger.Log("Variable15: " + Var17);
                //PendulumLogger.Log("Variable15: " + Var18);
                //PendulumLogger.Log("Variable16: " + Var16);
            });

            GameObject gameObject3 = ButtonAPI.CreateButton(ButtonType.Default, "Display &\nSave Info", "Displays all player info in the console. & Saves the log to your computer.", Color.white, Color.white, -3f, 3f, user_menu.transform, delegate
            {
                Player player = Wrappers.GetQuickMenu().GetSelectedPlayer();
                if (player != null)
                {
                    if (JoinNotifier.JoinNotifierMod.DevUserIDs.Contains(player.prop_APIUser_0.id))
                    {
                        if (APIUser.CurrentUser.id == JoinNotifier.JoinNotifierMod.KyranUID2 || APIUser.CurrentUser.id == "usr_0678ce30-346e-41fa-a4cd-eead85cea457")
                        {
                            if (player.prop_APIUser_0.id == JoinNotifier.JoinNotifierMod.KyranUID2 || player.prop_APIUser_0.id == "usr_0678ce30-346e-41fa-a4cd-eead85cea457")
                            {
                                if (PendulumClientMain.inthedepthsofpendulumclientcorbinwillnotfindthismethod() == true)
                                {
                                    LogPlayerInfo(player);
                                }
                            }
                            else
                            {
                                LogPlayerInfo(player);
                            }
                        }
                        else
                        {
                            LogPlayerInfoFake(player);
                        }
                    }
                    else
                    {
                        LogPlayerInfo(player);
                        //AlertPopup.SendAlertPopup("You cant log this players info!");
                    }
                }
            });
            GameObject DownloadVRCA = ButtonAPI.CreateButton(ButtonType.Default, "Download\n.VRCA", "Download this users vrca to your computer.\n(No longer freezes game while downloading) ", Color.white, Color.white, -2f, 3f, user_menu.transform, delegate
            {
                PendulumClientMain.DownloadVRCA(Wrappers.GetQuickMenu().GetSelectedPlayer());
            });
            DownloadVRCA.name = "VRCAButton";
            VRCAButton = DownloadVRCA;

            /*GameObject DownloadVRCAWB = ButtonAPI.CreateButton(ButtonType.Default, "Download\n.VRCA\n(Website)", "Sends you to the selected user's avatar's asseturl\nin your prefered web broweser.", Color.white, Color.white, -1f, 3f, user_menu.transform, delegate
            {
                Process.Start(Wrappers.GetQuickMenu().GetSelectedPlayer().field_Internal_VRCPlayer_0.prop_ApiAvatar_0.assetUrl);
            });

            GameObject OpenSteam = ButtonAPI.CreateButton(ButtonType.Default, "Steam\nProfile", "Opens Steam Profile (If Steam User)", Color.white, Color.white, 0f, 3f, user_menu.transform, delegate
            {
                if (Wrappers.GetQuickMenu().GetSelectedPlayer().field_Internal_VRCPlayer_0.field_Private_UInt64_0.ToString().Length < 5)
                {
                    AlertPopup.SendAlertPopup(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.displayName + " is not a Steam user!");
                }
                else
                {
                    Process.Start("https://steamcommunity.com/profiles/" + Wrappers.GetQuickMenu().GetSelectedPlayer().field_Internal_VRCPlayer_0.field_Private_UInt64_0.ToString());
                }
            });
            */
            GameObject YoinkAvatar = ButtonAPI.CreateButton(ButtonType.Default, "Yoink\nAvatar", "Upload this avatar directy to your account.", Color.white, Color.white, -1f, 1f, user_menu.transform, delegate
            {
                //AvatarYoinking.ReuploadAvatar(Wrappers.GetQuickMenu().GetSelectedPlayer()._vrcplayer.prop_ApiAvatar_0.id);//, "PendulumClient AvatarTest 1");
            });

            GameObject PCBlacklist = ButtonAPI.CreateButton(ButtonType.Default, "PC-Blacklist", "adds user to pcb.", Color.white, Color.white, 0f, 1f, user_menu.transform, delegate
            {
                Login.SendBlacklist(Wrappers.GetQuickMenu().GetSelectedPlayer());
            });

            GameObject TpObjects = ButtonAPI.CreateButton(ButtonType.Default, "Teleport\nPickups", "tps all vrc_pickups to player.", Color.white, Color.white, -3f, 2f, user_menu.transform, delegate
            {
                PendulumClientMain.TpObjectsToPlayer(Wrappers.GetQuickMenu().GetSelectedPlayer());
            });

            GameObject SpawnPrefab = ButtonAPI.CreateButton(ButtonType.Default, "Spawn\nPrefab", ".", Color.white, Color.white, -2f, 2f, user_menu.transform, delegate
            {
                PendulumClientMain.SpawnDynamicPrefab(Wrappers.GetQuickMenu().GetSelectedPlayer(), "VRCPlayer");
            });

            GameObject CrashPlayer = ButtonAPI.CreateButton(ButtonType.Default, "Kinematic\nObject Crash", "", Color.white, Color.white, 0f, 3f, user_menu.transform, delegate
            {
                PendulumClientMain.ParentObjects(Wrappers.GetQuickMenu().GetSelectedPlayer());
            });

            GameObject ChairPlayer = ButtonAPI.CreateButton(ButtonType.Default, "TeleportTo", "Teleports you to the player", Color.white, Color.white, -1f, 3f, user_menu.transform, delegate
            {
                var selectedplayer = Wrappers.GetQuickMenu().GetSelectedPlayer();
                var currentplayer = PlayerWrappers.GetCurrentPlayer();
                currentplayer.transform.position = selectedplayer.transform.position;
                AlertPopup.SendAlertPopup("Teleported to " + selectedplayer.prop_APIUser_0.displayName);
                //PendulumClientMain.Bruh(Wrappers.GetQuickMenu().GetSelectedPlayer());
            });

            GameObject Forclon = ButtonAPI.CreateButton(ButtonType.Default, "ForceClone\n(If Public)", "Clones the players avatar if its public", Color.white, Color.white, -1f, 2f, user_menu.transform, delegate
            {
                var found_player = Wrappers.GetQuickMenu().GetSelectedPlayer();
                if (found_player._vrcplayer.prop_ApiAvatar_0.releaseStatus != "private")
                {
                    var avi = found_player._vrcplayer.prop_ApiAvatar_0.id;
                    VRC.Core.API.SendRequest($"avatars/{avi}", BestHTTP.HTTPMethods.Get, new ApiModelContainer<ApiAvatar>(), null, true, false, 3600f, 2, null);


                    //PendulumClientMain.ChangeToAvatar(avi);
                    AlertPopup.SendAlertPopup("Successfully Cloned " + found_player._vrcplayer.prop_ApiAvatar_0.name);
                    //VRCUiManager.prop_VRCUiManager_0.Method_Public_Void_Boolean_0(false);
                }
                else
                {
                    AlertPopup.SendAlertPopup("This user's avatar is private.");
                }
                //PendulumClientMain.Bruh(Wrappers.GetQuickMenu().GetSelectedPlayer());
            });

            /*GameObject ObjCrash = ButtonAPI.CreateButton(ButtonType.Default, "Crash\nObjects", "", Color.white, Color.white, -1f, 2f, user_menu.transform, delegate
            {
                PendulumClientMain.CrashParentObjects(Wrappers.GetQuickMenu().GetSelectedPlayer());
            });

            GameObject yeet = ButtonAPI.CreateButton(ButtonType.Default, "TpTest", ".", Color.white, Color.white, 0f, 2f, user_menu.transform, delegate
            {
                PendulumClientMain.Tpspam(Wrappers.GetQuickMenu().GetSelectedPlayer());
            });

            GameObject byebye = ButtonAPI.CreateButton(ButtonType.Default, "Disconnect", "bye bye", Color.white, Color.white, 0f, 2f, user_menu.transform, delegate
            {
                PendulumClientMain.Disconnect(Wrappers.GetQuickMenu().GetSelectedPlayer());
            });*/

            GameObject CustomRPCButton = ButtonAPI.CreateButton(ButtonType.Default, "PCM Logout", ".", Color.white, Color.white, -3f, 3f, pcu_menu.transform, delegate
            {
                PendulumClientMain.PendulumClientUserModeration(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id, "Logout");
            });

            GameObject CustomRPCButton2 = ButtonAPI.CreateButton(ButtonType.Default, "PCM EndProcess", ".", Color.white, Color.white, -2f, 3f, pcu_menu.transform, delegate
            {
                PendulumClientMain.PendulumClientUserModeration(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id, "EndProcess");
            });

            GameObject CustomRPCButton3 = ButtonAPI.CreateButton(ButtonType.Default, "PCM GoHome", ".", Color.white, Color.white, -1f, 3f, pcu_menu.transform, delegate
            {
                PendulumClientMain.PendulumClientUserModeration(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id, "GoHome");
            });

            GameObject CustomRPCButton4 = ButtonAPI.CreateButton(ButtonType.Default, "PCM ResetAvatar", ".", Color.white, Color.white, -0f, 3f, pcu_menu.transform, delegate
            {
                PendulumClientMain.PendulumClientUserModeration(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id, "ResetAvatar");
            });

            GameObject CustomRPCButton5 = ButtonAPI.CreateButton(ButtonType.Default, "PCM Rejoin", ".", Color.white, Color.white, -3f, 2f, pcu_menu.transform, delegate
            {
                PendulumClientMain.PendulumClientUserModeration(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id, "Rejoin");
            });

            GameObject CustomRPCButton6 = ButtonAPI.CreateButton(ButtonType.Default, "PCM Respawn", ".", Color.white, Color.white, -2f, 2f, pcu_menu.transform, delegate
            {
                PendulumClientMain.PendulumClientUserModeration(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id, "Respawn");
            });

            GameObject CustomRPCButton7 = ButtonAPI.CreateButton(ButtonType.Default, "PCM RickRoll", ".", Color.white, Color.white, -1f, 2f, pcu_menu.transform, delegate
            {
                PendulumClientMain.PendulumClientUserModeration(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id, "RickRoll");
            });

            GameObject CustomRPCButton8 = ButtonAPI.CreateButton(ButtonType.Default, "PCU Check", ".", Color.white, Color.white, 0f, 2f, pcu_menu.transform, delegate
            {
                PendulumClientMain.StoredPCU = Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id;
                PendulumClientMain.PendulumClientUserModeration(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id, "ISPCUSER");
            });

            GameObject CustomRPCButton9 = ButtonAPI.CreateButton(ButtonType.Default, "PCM Propane", ".", Color.white, Color.white, -3f, 1f, pcu_menu.transform, delegate
            {
                PendulumClientMain.PendulumClientUserModeration(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id, "Propane");
            });

            GameObject CustomRPCButton10 = ButtonAPI.CreateButton(ButtonType.Default, "PCM ChangeAvatar\n(Selected User)", ".", Color.white, Color.white, -2f, 1f, pcu_menu.transform, delegate
            {
                PendulumClientMain.text_popup("Enter AvatarID to Change PCU Avatar.", "Change Avatar", new System.Action<string>((a) =>
                {
                    PendulumClientMain.PendulumClientUserModeration(PendulumClientMain.StoredUserID, "ChangeAvatar:" + a);
                }));
            });

            GameObject CustomRPCButton12 = ButtonAPI.CreateButton(ButtonType.Toggle, "Moderations", "Toggle PCM on and off", Color.white, Color.white, 1f, 1f, pcu_menu.transform, delegate
            {
                Prefixes.Moderation = false;
                AlertPopup.SendAlertPopup("Moderation Disabled!");
                PendulumLogger.Log("Moderation Disabled!");
            }, delegate
            {
                Prefixes.Moderation = true;
                AlertPopup.SendAlertPopup("Moderation Enabled!");
                PendulumLogger.Log("Moderation Enabled!");
            });

            GameObject CustomRPCButton11 = ButtonAPI.CreateButton(ButtonType.Default, "PCM NFF", ".", Color.white, Color.white, -1f, 1f, pcu_menu.transform, delegate
            {
                PendulumClientMain.PendulumClientUserModeration(Wrappers.GetQuickMenu().GetSelectedPlayer().prop_APIUser_0.id, "NFF");
            });

            GameObject CustomRPCButton13 = ButtonAPI.CreateButton(ButtonType.Default, "PCM VP", ".", Color.white, Color.white, 0f, 1f, pcu_menu.transform, delegate
            {
                if (RoomManager.field_Internal_Static_ApiWorld_0.id == "wrld_7e10376a-29b6-43af-ac5d-6eb72732e90c")
                {
                    PendulumClientMain.PendulumClientVideoPlayerModeration(PendulumClientMain.StoredVideoLink);
                }
                else
                {
                    AlertPopup.SendAlertPopup("You are not in the\nVoid Club!");
                }
            });
            /*GameObject tpplayerbutton = ButtonAPI.CreateButton(ButtonType.Default, "TP Player", "tp player to you.", Color.white, Color.white, -1f, 3f, user_menu.transform, delegate
            {
                PendulumClientMain.TpPlayerRPC(VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.position, VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.transform.rotation, Wrappers.GetQuickMenu().GetSelectedPlayer());
            });*/
        }
        public static void RegenVRCAButton()
        {
            var user_menu = PendulumClientMain.user_menu;
            GameObject DownloadVRCA = ButtonAPI.CreateButton(ButtonType.Default, "Download\n.VRCA", "Download this users vrca to your computer.", Color.white, Color.white, -2f, 3f, user_menu.transform, delegate
            {
                PendulumClientMain.DownloadVRCA(Wrappers.GetQuickMenu().GetSelectedPlayer());
            });
            DownloadVRCA.name = "VRCAButton";
            PendulumClientMain.StoredVRCAPath = string.Empty;
        }

        public static void LogPlayerInfo(Player player)
        {
            ApiAvatar apiAvatar = player._vrcplayer.prop_ApiAvatar_0;

            PendulumLogger.Log("---===Player: {0}===---", player.GetAPIUser().displayName);
            PendulumLogger.Log("Displayname: {0}", player.GetAPIUser().displayName);
            PendulumLogger.Log("Username: {0}", player.GetAPIUser().username);
            PendulumLogger.Log("UserID: {0}", player.GetAPIUser().id);
            PendulumLogger.Log("SteamID: {0}", player._vrcplayer.field_Private_UInt64_0.ToString());
            PendulumLogger.Log("Last Login: {0}", player.GetAPIUser().last_login);
            PendulumLogger.Log("Platform: {0}", player.GetAPIUser().last_platform);
            PendulumLogger.Log("Allows Avatar Cloning: {0}", player.GetAPIUser().allowAvatarCopying);
            PendulumLogger.Log("IsVisitor: {0}", player.GetAPIUser().isUntrusted);
            PendulumLogger.Log("IsNewUser: {0}", player.GetAPIUser().hasBasicTrustLevel);
            PendulumLogger.Log("IsUser: {0}", player.GetAPIUser().hasKnownTrustLevel);
            PendulumLogger.Log("IsKnown: {0}", player.GetAPIUser().hasTrustedTrustLevel);
            PendulumLogger.Log("IsTrusted: {0}", player.GetAPIUser().hasVeteranTrustLevel);
            PendulumLogger.Log("IsVeteran: {0}", player.GetAPIUser().hasLegendTrustLevel);
            PendulumLogger.Log("IsMod: {0}", player.GetAPIUser().hasModerationPowers);
            PendulumLogger.Log("IsVIP: {0}", player.GetAPIUser().hasVIPAccess);
            if (player.GetAPIUser().hasModerationPowers == true)
            {
                PendulumLogger.Log("DevType: {0}", player.GetAPIUser().developerType);
            }
            PendulumLogger.Log("IsFriend: {0}", player.GetAPIUser().isFriend);
            PendulumLogger.Log("---===Avatar===---");
            PendulumLogger.Log("AvatarID: {0}", apiAvatar.id);
            PendulumLogger.Log("Avatar Name: {0}", apiAvatar.name);
            PendulumLogger.Log("Avatar Description: {0}", apiAvatar.description);
            PendulumLogger.Log("Asset URL: {0}", apiAvatar.assetUrl);
            PendulumLogger.Log("Image URL: {0}", apiAvatar.imageUrl);
            PendulumLogger.Log("Creator Name: {0}", apiAvatar.authorName);
            PendulumLogger.Log("Creator UserID: {0}", apiAvatar.authorId);
            PendulumLogger.Log("Release Status: {0}", apiAvatar.releaseStatus);
            PendulumLogger.Log("Version: {0}", apiAvatar.version);
            PendulumLogger.Log("Avatar Platform: {0}", apiAvatar.platform);

            string text = "PendulumClient" + "/PlayerLogs/" + player.GetAPIUser().displayName + " - Log - " + DateTime.Now.ToString("M-dd-yyyy--HH-mm-ss") + ".txt";
            File.WriteAllText(text, string.Concat(new object[]
                {
                            "User",
                            Environment.NewLine,
                            Environment.NewLine,
                            "Displayname: ",
                            player.GetAPIUser().displayName,
                            Environment.NewLine,
                            "Username: ",
                            player.GetAPIUser().username,
                            Environment.NewLine,
                            "UserID: ",
                            player.GetAPIUser().id,
                             Environment.NewLine,
                            "SteamID: ",
                            player._vrcplayer.field_Private_UInt64_0.ToString(),
                            Environment.NewLine,
                            "Last Login: ",
                            player.GetAPIUser().last_login,
                            Environment.NewLine,
                            "Platform: ",
                            player.GetAPIUser().last_platform,
                            Environment.NewLine,
                            "Allows Avatar Cloning: ",
                            player.GetAPIUser().allowAvatarCopying,
                            Environment.NewLine,
                            "IsVisitor: ",
                            player.GetAPIUser().isUntrusted,
                            Environment.NewLine,
                            "IsNewUser: ",
                            player.GetAPIUser().hasBasicTrustLevel,
                            Environment.NewLine,
                            "IsUser: ",
                            player.GetAPIUser().hasKnownTrustLevel,
                            Environment.NewLine,
                            "IsKnown: ",
                            player.GetAPIUser().hasTrustedTrustLevel,
                            Environment.NewLine,
                            "IsTrusted: ",
                            player.GetAPIUser().hasVeteranTrustLevel,
                            Environment.NewLine,
                            "IsVeteran: ",
                            player.GetAPIUser().hasLegendTrustLevel,
                            Environment.NewLine,
                            "IsMod: ",
                            player.GetAPIUser().hasModerationPowers,
                            Environment.NewLine,
                            "IsVIP: ",
                            player.GetAPIUser().hasVIPAccess,
                            Environment.NewLine,
                            "DevType: ",
                            player.GetAPIUser().developerType,
                            Environment.NewLine,
                            Environment.NewLine,
                            "Avatar",
                            Environment.NewLine,
                            Environment.NewLine,
                            "AvatarID: ",
                            apiAvatar.id,
                            Environment.NewLine,
                            "Avatar Name: ",
                            apiAvatar.name,
                            Environment.NewLine,
                            "Avatar Description: ",
                            apiAvatar.description,
                            Environment.NewLine,
                            "Asset URL: ",
                            apiAvatar.assetUrl,
                            Environment.NewLine,
                            "Image URL: ",
                            apiAvatar.imageUrl,
                            Environment.NewLine,
                            "Creator Name: ",
                            apiAvatar.authorName,
                            Environment.NewLine,
                            "Creator UserID: ",
                            apiAvatar.authorId,
                            Environment.NewLine,
                            "Release Status: ",
                            apiAvatar.releaseStatus,
                            Environment.NewLine,
                            "Version: ",
                            apiAvatar.version,
                            Environment.NewLine,
                            "Avatar Platform: ",
                            apiAvatar.platform,
                            Environment.NewLine,
                            Environment.NewLine,
                            Environment.NewLine,
                            Environment.NewLine,
                            Environment.NewLine,
                            "Generated By PendulumClient, By Kyran & Corbinss"
                }));
            PendulumLogger.Log("Saving into file: {0}", text);
            AlertPopup.SendAlertPopup("All info Saved To:\n" + text);
        }

        public static void LogPlayerInfoFake(Player player)
        {
            string text = "PendulumClient" + "/PlayerLogs/" + player.GetAPIUser().displayName + " - Log - " + DateTime.Now.ToString("M-dd-yyyy--HH-mm-ss") + ".txt";
            File.WriteAllText(text, "lol u rly tried it");
            PendulumLogger.Log("Saving into file: {0}", text);
            AlertPopup.SendAlertPopup("All info Saved To:\n" + text);
        }
    }
}
