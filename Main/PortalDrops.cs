using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.Core;
using VRC.SDKBase;
using UnityEngine;
using VRC;
using PendulumClient.Wrapper;
using MelonLoader;

using Player = VRC.Player;


namespace PendulumClient.Main
{
    class PortalDrops
    {
        public static void DropPortalOnPlayer(Player player)
        {
            GameObject gameObject = Networking.Instantiate(VRC_EventHandler.VrcBroadcastType.Always, "Portals/PortalInternalDynamic", player.transform.position + (player.transform.forward * PendulumClientMain.PortalDropDistance), player.transform.rotation);
            Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
            {
                (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                (Il2CppSystem.String)"\nNIGGER" +
                                     "\n",
                new Il2CppSystem.Int32
                {
                m_value = -2147483647
                }.BoxIl2CppObject()
            });
            gameObject.name = "DroppedUserPortal";
            gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
        }

        public static void DropPortalInfrontOfPlayer(Player player)
        {
            GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", player.transform.position + (player.transform.forward * PendulumClientMain.PortalDropDistance), player.transform.rotation);
            Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
            {
                (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                (Il2CppSystem.String)"\nNIGGER" +
                                     "\n",
                new Il2CppSystem.Int32
                {
                m_value = -2147483647
                }.BoxIl2CppObject()
            });
            gameObject.name = "DroppedUserPortal";
            gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
        }
        public static void DropPortalBehindPlayer(Player player)
        {
            GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", player.transform.position + (player.transform.forward * -PendulumClientMain.PortalDropDistance), player.transform.rotation);
            Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
            {
                (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                (Il2CppSystem.String)"\nNIGGER" +
                                     "\n",
                new Il2CppSystem.Int32
                {
                m_value = -2147483647
                }.BoxIl2CppObject()
            });
            gameObject.name = "DroppedUserPortal";
            gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
        }
        public static void DropPortalWithText(Player player, string text)
        {
            GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", player.transform.position + (player.transform.forward * PendulumClientMain.PortalDropDistance), player.transform.rotation);
            Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
            {
                (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                (Il2CppSystem.String)"\n" + text,// +
                                     //"\n" + player.prop_APIUser_0.displayName + "\npublic" + "\0",
                new Il2CppSystem.Int32
                {
                m_value = -2147483647
                }.BoxIl2CppObject()
            });
            gameObject.name = "DroppedUserPortal";
            gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
        }

        public static void DropPortalOnUserID(string userid)
        {
            if (PlayerWrappers.GetPlayer(userid) != null)
            {
                var player = PlayerWrappers.GetPlayer(userid);
                if (player.prop_APIUser_0.displayName.Contains("~"))
                {
                    var OutputDisplayName = player.prop_APIUser_0.displayName;
                    var NameOut = OutputDisplayName.Replace("~", string.Empty);
                    try
                    {
                        if (false)//ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_0(player.prop_APIUser_0.id))
                        {
                            GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", player.transform.position + (player.transform.forward * PendulumClientMain.PortalDropDistance), new Quaternion(player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z, player.transform.rotation.w));
                            Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
                            {
                    (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                    (Il2CppSystem.String)"\nUnblock me NIGGER" +
                                     "\n",
                    new Il2CppSystem.Int32
                    {
                    m_value = -2147483647
                    }.BoxIl2CppObject()
                            });
                            gameObject.name = "DroppedUserPortal";
                            gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
                        }
                        else if (false)//player.prop_APIUser_0.last_platform == "android")
                        {
                            GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", player.transform.position + (player.transform.forward * PendulumClientMain.PortalDropDistance), new Quaternion(player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z, player.transform.rotation.w));
                            Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
                            {
                    (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                    (Il2CppSystem.String)"\nQuest User NIGGER" +
                                     "\n",
                    new Il2CppSystem.Int32
                    {
                    m_value = -2147483647
                    }.BoxIl2CppObject()
                            });
                            gameObject.name = "DroppedUserPortal";
                            gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
                        }
                        else
                        {
                            GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", player.transform.position + (player.transform.forward * PendulumClientMain.PortalDropDistance), new Quaternion(player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z, player.transform.rotation.w));
                            Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
                            {
                    (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                    (Il2CppSystem.String)"\nNIGGER" +
                                     "\n",
                    new Il2CppSystem.Int32
                    {
                    m_value = -2147483647
                    }.BoxIl2CppObject()
                            });
                            gameObject.name = "DroppedUserPortal";
                            gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
                        }
                    }
                    catch
                    {
                    }
                }
                else
                {
                    if (false)//ModerationManager.field_Private_Static_ModerationManager_0.Method_Public_Boolean_String_PDM_0(player.prop_APIUser_0.id))
                    {
                        GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", player.transform.position + (player.transform.forward * PendulumClientMain.PortalDropDistance), new Quaternion(player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z, player.transform.rotation.w));
                        Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
                        {
                    (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                    (Il2CppSystem.String)"\nUnblock me NIGGER" +
                                     "\n",
                    new Il2CppSystem.Int32
                    {
                    m_value = -2147483647
                    }.BoxIl2CppObject()
                        });
                        gameObject.name = "DroppedUserPortal";
                        gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
                    }
                    else if (false)//player.prop_APIUser_0.last_platform == "android")
                    {
                        GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", player.transform.position + (player.transform.forward * PendulumClientMain.PortalDropDistance), new Quaternion(player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z, player.transform.rotation.w));
                        Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
                        {
                    (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                    (Il2CppSystem.String)"\nQuest User NIGGER" +
                                     "\n",
                    new Il2CppSystem.Int32
                    {
                    m_value = -2147483647
                    }.BoxIl2CppObject()
                        });
                        gameObject.name = "DroppedUserPortal";
                        gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
                    }
                    else
                    {
                        GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", player.transform.position + (player.transform.forward * PendulumClientMain.PortalDropDistance), new Quaternion(player.transform.rotation.x, player.transform.rotation.y, player.transform.rotation.z, player.transform.rotation.w));
                        Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
                        {
                    (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                    (Il2CppSystem.String)"\nNIGGER" +
                                     "\n",
                    new Il2CppSystem.Int32
                    {
                    m_value = -2147483647
                    }.BoxIl2CppObject()
                        });
                        gameObject.name = "DroppedUserPortal";
                        gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
                    }
                }
            }
            else
            {
                PendulumLogger.Log("Cant find Selected Player!");
            }
        }

        public static void DropPortalWithInstance(string[] instance)
        {
            VRCPlayer CurrentPlayer = PlayerWrappers.GetCurrentPlayer();
            GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", CurrentPlayer.transform.position + (CurrentPlayer.transform.forward * PendulumClientMain.PortalDropDistance), CurrentPlayer.transform.rotation);
            Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
            {
                (Il2CppSystem.String)instance[0],
                (Il2CppSystem.String)instance[1] + "\0",
                new Il2CppSystem.Int32
                {
                m_value = 0
                }.BoxIl2CppObject()
            });
            gameObject.name = "DroppedUserPortal";
            gameObject.GetComponent<PortalInternal>().SetTimerRPC(-391, CurrentPlayer._player);
            PendulumLogger.Log(ConsoleColor.Green, "Dropping Portal to Instance: " + instance[0] + ":" + instance[1]);
        }

        public static void DropPortalWithInstanceText(Player player, string text)
        {
            GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", player.transform.position + (player.transform.forward * PendulumClientMain.PortalDropDistance), player.transform.rotation);
            Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
            {
                (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                (Il2CppSystem.String)text,
                new Il2CppSystem.Int32
                {
                m_value = -25
                }.BoxIl2CppObject()
            });
            gameObject.name = "DroppedUserPortal";
        }

        public static void DropPortalInfrontRandomName()
        {
            if (PlayerWrappers.GetAllPlayers() != null)
            {
                var player = PlayerWrappers.GetAllPlayers()[new System.Random().Next(PlayerWrappers.GetAllPlayers()._size - 1)];
                if (true)//player.prop_APIUser_0.id != APIUser.CurrentUser.id) rip anti portal drop name :(
                {
                    if (player.prop_APIUser_0.displayName.Contains("~"))
                    {
                        var OutputDisplayName = player.prop_APIUser_0.displayName;
                        var NameOut = OutputDisplayName.Replace("~", string.Empty);
                        GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", PlayerWrappers.GetCurrentPlayer().transform.position + (PlayerWrappers.GetCurrentPlayer().transform.forward * PendulumClientMain.PortalDropDistance), new Quaternion(20f, PlayerWrappers.GetCurrentPlayer().transform.rotation.y, PlayerWrappers.GetCurrentPlayer().transform.rotation.z, PlayerWrappers.GetCurrentPlayer().transform.rotation.w));
                        Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
                            {
                                (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                                (Il2CppSystem.String)"😆\nPendulum Client Portal\n",
                                new Il2CppSystem.Int32
                                    {
                                        m_value = -2147483647
                                    }.BoxIl2CppObject()
                            });
                        gameObject.name = "DroppedUserPortal";
                        gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
                    }
                    else
                    {
                        var OutputDisplayName = player.prop_APIUser_0.displayName;
                        GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", PlayerWrappers.GetCurrentPlayer().transform.position + (PlayerWrappers.GetCurrentPlayer().transform.forward * PendulumClientMain.PortalDropDistance), new Quaternion(20f, PlayerWrappers.GetCurrentPlayer().transform.rotation.y, PlayerWrappers.GetCurrentPlayer().transform.rotation.z, PlayerWrappers.GetCurrentPlayer().transform.rotation.w));
                        Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
                            {
                                (Il2CppSystem.String)"wrld_496b11e8-25a0-4f35-976d-faae5e00d60e",
                                (Il2CppSystem.String)"😆\nPendulum Client Portal\n",
                                new Il2CppSystem.Int32
                                    {
                                        m_value = -2147483647
                                    }.BoxIl2CppObject()
                            });
                        gameObject.name = "DroppedUserPortal";
                        gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
                    }
                }
                else
                {
                    //PendulumLogger.Log("Random Name Selected Yourself, Cancelling Portal Drop.");
                }
            }
            else
            {
                PendulumLogger.Log("PlayerList is null or empty!");
            }
        }

        public static void DropPortalInvalidSideways()
        {
            if (PlayerWrappers.GetAllPlayers() != null)
            {
                //var player = PlayerWrappers.GetAllPlayers()[new System.Random().Next(PlayerWrappers.GetAllPlayers()._size-1)];
                //var OutputDisplayName = player.prop_APIUser_0.displayName;
                //var NameOut = OutputDisplayName.Replace("~", string.Empty);
                try
                {
                    var rot = new Quaternion(PlayerWrappers.GetCurrentPlayer().transform.rotation.x, PlayerWrappers.GetCurrentPlayer().transform.rotation.y, PlayerWrappers.GetCurrentPlayer().transform.rotation.z, PlayerWrappers.GetCurrentPlayer().transform.rotation.w);
                    //rot.SetEulerAngles(new Vector3(PlayerWrappers.GetCurrentPlayer().transform.rotation.eulerAngles.x, PlayerWrappers.GetCurrentPlayer().transform.rotation.eulerAngles.y, PlayerWrappers.GetCurrentPlayer().transform.rotation.eulerAngles.z));
                    //rot.SetEulerRotation(0f, 0f, 75f);
                    //rot.SetAxisAngle(Vector3.back, 75f);
                    rot *= Quaternion.Euler(0, 0, 90f);
                    GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", PlayerWrappers.GetCurrentPlayer().transform.position + (PlayerWrappers.GetCurrentPlayer().transform.right * 1.2f) + (PlayerWrappers.GetCurrentPlayer().transform.up * 0.75f) + (PlayerWrappers.GetCurrentPlayer().transform.forward * PendulumClientMain.PortalDropDistance), rot);//Quaternion(PlayerWrappers.GetCurrentPlayer().transform.rotation.x, 90f, PlayerWrappers.GetCurrentPlayer().transform.rotation.z, PlayerWrappers.GetCurrentPlayer().transform.rotation.w));
                    Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
                    {
                    (Il2CppSystem.String)"\0",
                    (Il2CppSystem.String)"",
                    new Il2CppSystem.Int32
                        {
                            m_value = -2147483647
                        }.BoxIl2CppObject()
                    });
                    gameObject.name = "DroppedUserPortal";
                    gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, PlayerWrappers.GetCurrentPlayer()._player);
                }
                catch ( Exception )
                {
                    PendulumLogger.Log("Wait 5s before dropping more portals!");
                }
            }
            else
            {
                PendulumLogger.Log("PlayerList is null or empty!");
            }
        }

        public static void DropPortalInvalidSidewaysOnPlayerHead(Player player)
        {
            if (PlayerWrappers.GetAllPlayers() != null)
            {
                var HeadObject = player.gameObject.transform.Find("AnimationController/HeadAndHandIK/HeadEffector").gameObject;
                //var player = PlayerWrappers.GetAllPlayers()[new System.Random().Next(PlayerWrappers.GetAllPlayers()._size-1)];
                //var OutputDisplayName = player.prop_APIUser_0.displayName;
                //var NameOut = OutputDisplayName.Replace("~", string.Empty);
                if (HeadObject != null)
                {
                    try
                    {
                        var rot = new Quaternion(HeadObject.transform.rotation.x, HeadObject.transform.rotation.y, HeadObject.transform.rotation.z, HeadObject.transform.rotation.w);
                        //rot.SetEulerAngles(new Vector3(PlayerWrappers.GetCurrentPlayer().transform.rotation.eulerAngles.x, PlayerWrappers.GetCurrentPlayer().transform.rotation.eulerAngles.y, PlayerWrappers.GetCurrentPlayer().transform.rotation.eulerAngles.z));
                        //rot.SetEulerRotation(0f, 0f, 75f);
                        //rot.SetAxisAngle(Vector3.back, 75f);
                        rot *= Quaternion.Euler(0, 0, 90f);
                        GameObject gameObject = Networking.Instantiate(0, "Portals/PortalInternalDynamic", HeadObject.transform.position + (HeadObject.transform.right * 1.2f) + (HeadObject.transform.forward * PendulumClientMain.PortalDropDistance), rot);//Quaternion(PlayerWrappers.GetCurrentPlayer().transform.rotation.x, 90f, PlayerWrappers.GetCurrentPlayer().transform.rotation.z, PlayerWrappers.GetCurrentPlayer().transform.rotation.w));
                        Networking.RPC(VRC.SDKBase.RPC.Destination.AllBufferOne, gameObject, "ConfigurePortal", new Il2CppSystem.Object[]
                        {
                    (Il2CppSystem.String)"\0",
                    (Il2CppSystem.String)"",
                    new Il2CppSystem.Int32
                        {
                            m_value = -2147483647
                        }.BoxIl2CppObject()
                        });
                        gameObject.name = "DroppedUserPortal";
                        gameObject.GetComponent<PortalInternal>().SetTimerRPC(float.MinValue, player);
                    }
                    catch (Exception)
                    {
                        PendulumLogger.Log("Wait 5s before dropping more portals!");
                    }
                }
            }
            else
            {
                PendulumLogger.Log("PlayerList is null or empty!");
            }
        }
    }
}
