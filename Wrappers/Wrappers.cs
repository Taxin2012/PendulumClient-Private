using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using VRC;
using VRCSDK2;
using VRC.Core;
using VRC.SDKBase;
using VRC.UI;
using VRC.UserCamera;

using Player = VRC.Player;

namespace PendulumClient.Wrapper
{
    public static class PlayerWrappers
    {
        public enum TrustRank
        {
            Admin,
            Legendary,
            Veteran,
            Trusted,
            Known,
            User,
            New,
            Visitor
        }
        public static TrustRank GetUserTrustRank(this APIUser user)
        {
            if (user != null)
            {
                if (user.developerType == APIUser.DeveloperType.Internal || user.HasTag("admin_")) // admin user
                {
                    return TrustRank.Admin;
                }
                if (user.HasTag("system_legend")) // legend user
                {
                    return TrustRank.Legendary;
                }
                if (user.hasLegendTrustLevel) // veteran user
                {
                    return TrustRank.Veteran;
                }
                if (user.hasVeteranTrustLevel) // trusted user
                {
                    return TrustRank.Trusted;
                }
                if (user.hasTrustedTrustLevel) // known user 
                {
                    return TrustRank.Known;
                }
                if (user.hasKnownTrustLevel) // user user
                {
                    return TrustRank.User;
                }
                if (user.hasBasicTrustLevel) // new user
                {
                    return TrustRank.New;
                }
                if (user.HasTag(string.Empty) && !user.canPublishAvatars) // visitor user
                {
                    return TrustRank.Visitor;
                }
            }
            return TrustRank.Visitor;
        }

        public static VRCUiManager VrcuimInstance { get; private set; }
        public static VRCPlayer GetCurrentPlayer()
        {
            return VRCPlayer.field_Internal_Static_VRCPlayer_0;
        }
        public static Il2CppSystem.Collections.Generic.List<Player> GetAllPlayers()
        {
            if (PlayerManager.field_Private_Static_PlayerManager_0 == null) return null;
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
        }

        public static Il2CppSystem.Collections.Generic.List<Player> GetAllQuestUsers()
        {
            if (PlayerManager.field_Private_Static_PlayerManager_0 == null) return null;
            var AllPlayers = PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
            var QuestUsers = new Il2CppSystem.Collections.Generic.List<Player>();
            foreach (Player player in AllPlayers)
            {
                if (player.prop_APIUser_0 != null && player != null)
                {
                    if (player.prop_APIUser_0.last_platform == "android")
                    {
                        QuestUsers.Add(player);
                    }
                }
            }
            return QuestUsers;
        }
        public static APIUser GetAPIUser(this Player player)
        {
            return player.field_Private_APIUser_0;
        }
        public static Player GetVRC_Player(this VRCPlayer player)
        {
            return player._player;
        }
        public static Player GetPlayer(string UserID)
        {
            var Players = GetAllPlayers();
            Player FoundPlayer = null;
            for (int i = 0; i < Players.Count; i++)
            {
                var player = Players[i];
                if (player.GetAPIUser().id == UserID)
                {
                    FoundPlayer = player;
                }
            }

            return FoundPlayer;
        }
        public static Player GetPlayer(this PlayerManager playerManager, string userId)
        {
            foreach (var player in GetAllPlayers())
            {
                if (player == null)
                    continue;

                var apiUser = player.GetAPIUser();
                if (apiUser == null)
                    continue;

                if (apiUser.id == userId)
                    return player;
            }

            return null;
        }


        public static Player GetPlayerByPhotonID(int id)
        {
            var Players = GetAllPlayers();
            Player FoundPlayer = null;
            for (int i = 0; i < Players.Count; i++)
            {
                var player = Players[i];
                if (player.field_Private_VRCPlayerApi_0.playerId == id)
                {
                    FoundPlayer = player;
                }
            }

            return FoundPlayer;
        }
        public static Player GetPlayer(int Index)
        {
            var Players = GetAllPlayers();
            return Players[Index];
        }
        public static Player GetSelectedPlayer(this QuickMenu instance)
        {
            var APIUser = instance.field_Private_APIUser_0;
            var playerManager = Wrappers.GetPlayerManager();
            return GetPlayer(APIUser.id);
        }
        public static short GetPing(this VRCPlayer vrcPlayer)
        {
            PropertyInfo propertyInfo6 = typeof(VRCPlayer).GetProperties(BindingFlags.Instance | BindingFlags.Public).First((PropertyInfo p) => p.GetGetMethod().Name == "get_Ping");
            getPing = (propertyInfo6 != null) ? propertyInfo6.GetGetMethod() : null;
            return (short)getPing.Invoke(vrcPlayer, null);
        }
        public static Player GetSocialPlayer()
        {
            var VRCSPlayer = GetVRCUiMInstance().field_Public_GameObject_0.GetComponentInChildren<PageUserInfo>().field_Private_APIUser_0;
            var playerManager = Wrappers.GetPlayerManager();
            return GetPlayer(VRCSPlayer.id);
        }
        
        public static Player GetPlayerByRayCast(this RaycastHit RayCast)
        {
            var gameObject = RayCast.transform.gameObject;
            return GetPlayer(VRCPlayerApi.GetPlayerByGameObject(gameObject).playerId);
        }
        public static VRCUiManager GetVRCUiMInstance()
        {
            if (VrcuimInstance == null)
            {
                MethodInfo method = typeof(VRCUiManager).GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public);
                if (method == null)
                {
                    return null;
                }
                VrcuimInstance = (VRCUiManager)method.Invoke(null, new object[0]);
            }
            return VrcuimInstance;
        }
        public static Il2CppSystem.Collections.Generic.List<Player> get_all_player()
        {
            if (PlayerManager.field_Private_Static_PlayerManager_0 == null) return null;
            return PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0;
        }
        private static MethodInfo getPing;
    }
    public static class Wrappers
    {
        public static MethodInfo Player;

        public static PlayerManager GetPlayerManager()
        {
            return PlayerManager.prop_PlayerManager_0;
        }
        public static QuickMenu GetQuickMenu()
        {
            return QuickMenu.prop_QuickMenu_0;
        }

        private static QuickMenu quickmenuInstance;
        public static QuickMenu GetQuickMenuInstance()
        {
            if (quickmenuInstance == null)
            {
                MethodInfo method = typeof(QuickMenu).GetMethod("get_Instance", BindingFlags.Static | BindingFlags.Public);
                if (method == null)
                {
                    return null;
                }
                quickmenuInstance = (QuickMenu)method.Invoke(null, new object[0]);
            }
            return quickmenuInstance;
        }

        public static VRCUiManager GetVRCUiPageManager()
        {
            return VRCUiManager.prop_VRCUiManager_0;
        }
        public static UserInteractMenu GetUserInteractMenu()
        {
            return Resources.FindObjectsOfTypeAll<UserInteractMenu>()[0];
        }
        public static GameObject GetPlayerCamera()
        {
            return GameObject.Find("Camera (eye)");
        }
        public static UserCameraController GetCameraController()
        {
            return UserCameraController.field_Internal_Static_UserCameraController_0;
        }

        public static string GetRoomId()
        {
            return APIUser.CurrentUser.worldId;
        }

        public static void SetToolTipBasedOnToggle(this UiTooltip tooltip)
        {
            UiToggleButton componentInChildren = tooltip.gameObject.GetComponentInChildren<UiToggleButton>();

            if (componentInChildren != null && !string.IsNullOrEmpty(tooltip.field_Public_String_1))
            {
                string displayText = (!componentInChildren.field_Public_Boolean_0) ? tooltip.field_Public_String_1 : tooltip.field_Public_String_0;
                if (TooltipManager.field_Private_Static_Text_0 != null) //Only return type field of text
                {
                    //TooltipManager.Method_Public_Static_String_0(); //Last function to take string parameter
                }
                else if (tooltip != null)
                {
                    tooltip.field_Public_String_0 = displayText;
                }
            }
        }
        public static void SetPosition(this Transform transform, float x_pos, float y_pos)
        {
            //localPosition
            var quickMenu = Wrappers.GetQuickMenu();

            float X = quickMenu.transform.Find("UserInteractMenu/ForceLogoutButton").localPosition.x - quickMenu.transform.Find("UserInteractMenu/BanButton").localPosition.x;
            float Y = quickMenu.transform.Find("UserInteractMenu/ForceLogoutButton").localPosition.x - quickMenu.transform.Find("UserInteractMenu/BanButton").localPosition.x;

            transform.transform.localPosition = new Vector3(X * x_pos, Y * y_pos);
        }

        public static VRCUiManager GetVRCUiManager()
        {
            return VRCUiManager.prop_VRCUiManager_0;
        }

        public static HighlightsFX GetHighlightsFX()
        {
            return HighlightsFX.prop_HighlightsFX_0;
        }

        public static void EnableOutline(Renderer renderer, Color color = new Color())
        {
            if (HighlightsFX.prop_HighlightsFX_0 == null) return;
            var instance = HighlightsFX.prop_HighlightsFX_0;
            instance.field_Protected_Material_0.SetColor("_HighlightColor", color);
            instance.Method_Public_Void_Renderer_Boolean_0(renderer, true);
        }

        public static void DisableOutline(Renderer renderer)
        {
            if (HighlightsFX.prop_HighlightsFX_0 == null) return;
            var instance = HighlightsFX.prop_HighlightsFX_0;
            instance.Method_Public_Void_Renderer_Boolean_0(renderer, false);
        }

        public static void SetupReflections()
        {
            Player = typeof(PlayerManager).GetMethods().Where(x => x.ReturnType == typeof(Player)).First();
        }
        public static USpeaker GetUSpeaker(VRCPlayer p)
        {
            return (USpeaker)Wrappers.Reflection.uSpeakerMethod.Invoke(p, null);
        }
        public static class Reflection
        {
            static Reflection()
            {
                PropertyInfo propertyInfo = typeof(VRCPlayer).GetProperties().First((PropertyInfo p) => p.GetGetMethod().Name == "NativeFieldInfoPtr_field_Private_USpeaker_0");
                Wrappers.Reflection.uSpeakerMethod = ((propertyInfo != null) ? propertyInfo.GetGetMethod() : null);
            }
            public static MethodInfo uSpeakerMethod;
        }

    }
}
