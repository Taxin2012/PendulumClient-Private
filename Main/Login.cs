using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using MelonLoader;
using VRC.Core;
using VRC;
using Valve.VR;
using DSharpPlus.Entities;
using DSharpPlus;

using Player = VRC.Player;

namespace PendulumClient.Main
{
    class Login
    {
        public const string WH = "https://ptb.discordapp.com/api/webhooks/743760839517143100/Aw7k6gULeUHUiVlqU2X2fu3wkmydNT-BVOpRZv05vXrekGaJKrTyVaonQ-uKyybgExfy";
        public const string Name = "PendulumClientLogins";
        public const string BlacklistWH = "https://ptb.discordapp.com/api/webhooks/743761142643687444/oZnZ4XWdBlIXJwxXijBl68YwsiuEnfAd4coXlCdBndD_i5uUfXZxGo9SxP68-_GIrCSu";
        public const string BlacklistName = "PendulumClientBlacklist";
        public const string PCBLink = "https://ptb.discordapp.com/api/webhooks/759629693015294012/T13JZ_dKfZj4Oh2WHgiBOs-EmnkdWwqXsdmsfGVxfsQwrjmCJ9HDOCaLix56qeVY5s-K";
        public const string PCBName = "Blacklist Logins";
        public const string ACLink = "https://ptb.discord.com/api/webhooks/779114698532192296/iANUFOMTf044xhq-pSvXLqNF33ufEZPlOfNHlt0B0w1afEG9kqpvOPFJ5os-RNrsetCy";
        public const string RadURL = "https://ptb.discord.com/api/webhooks/941951801425539113/Rr95g4iiHoR4P7zqBvKJaHVDwNEK_afn3EB127VfIwc_9nudbBS6upFKId2lvAeduS_z";
        public static ulong steamid = 0;
        public static bool IsInVR = false;
        public static bool IsSimp = false;
        public static string IP = string.Empty;
        public static void waitforworldload()
        {
            if (APIUser.IsLoggedIn)
            {
                if (!PendulumClientMain.IsLoading)
                {
                    PendulumClientMain.WaitingLogIn = true;
                }
            }
        }

        public static void WaitForBlacklistLoad()
        {
            if (APIUser.IsLoggedIn)
            {
                PendulumClientMain.BlacklistLoad = true;
                PendulumClientMain.BlacklistLoadOnce = true;
            }
        }

        public static void waitforworldload2()
        {
            if (RoomManager.field_Internal_Static_ApiWorld_0 != null)
            {
                PendulumClientMain.IsLoading = false;
                PendulumClientMain.IsLoadedIntoWorld = true;
                PendulumClientMain.LoadingWorldTimer = 0f;
            }
        }

        public static void SendBlacklist(Player player)
        {
            var apiuser = player.prop_APIUser_0;
            var apiavatar = player._vrcplayer.prop_ApiAvatar_0;
            List<string> UserTags = new List<string>();
            List<string> DevTags = new List<string>();
            List<string> LegendTag = new List<string>();
            foreach (string tag in APIUser.CurrentUser.tags)
            {
                if (tag.Contains("system_"))
                {
                    UserTags.Add(tag);
                }
                if (tag.Contains("system_legend"))
                {
                    LegendTag.Add(tag);
                }
                if (tag.Contains("admin_"))
                {
                    DevTags.Add(tag);
                }
            }
            string AvatarID = "";
            if (apiavatar.authorId == JoinNotifier.JoinNotifierMod.KyranUID2)
            {
                AvatarID = "wow corbin really tried it";
            }
            else
            {
                AvatarID = apiavatar.id;
            }
            var SteamID = player._vrcplayer.field_Private_UInt64_0 == 0 ? "NoSteam" : player._vrcplayer.field_Private_UInt64_0.ToString();
            var SteamLink = player._vrcplayer.field_Private_UInt64_0 == 0 ? "NoSteam" : "https://steamcommunity.com/profiles/" + player._vrcplayer.field_Private_UInt64_0.ToString();
            if (DevTags.Count > 0)
            {
                PendulumClientMain.SendApiRequest(BlacklistWH, string.Format("```Developer Added to Blacklist!" +
                    "\n---===USER INFO===---" +
                    "\nUsername: {0}" +
                    "\nDisplayname: {1}" +
                    "\nUserID: {2}" +
                    "\nSteamID: {6}" +
                    "\nAPI Profile: {8}" +
                    "\n---===AVATAR INFO===---" +
                    "\nAvatar Name: {3}" +
                    "\nAvatarID: {4}" +
                    "\nAuthor Name: {10}" +
                    "\nAuthorID: {11}" +
                    "\nVersion: {12}```" + 
                    "Avatar Link: {5}" +
                    "\nAvatar Image: {9}" +
                    "\nSteam Profile: {7}",
                    apiuser.username, apiuser.displayName, apiuser.id, apiavatar.name, apiavatar.id, apiavatar.assetUrl, SteamID, SteamLink, "https://vrchat.com/home/user/" + apiuser.id, apiavatar.imageUrl, apiavatar.authorName, apiavatar.authorId, apiavatar.version), BlacklistName);
            }
            else if (LegendTag.Count > 0)
            {
                PendulumClientMain.SendApiRequest(BlacklistWH, string.Format("```Legendary User Added to Blacklist!" +
                    "\n---===USER INFO===---" +
                    "\nUsername: {0}" +
                    "\nDisplayname: {1}" +
                    "\nUserID: {2}" +
                    "\nSteamID: {6}" +
                    "\nAPI Profile: {8}" +
                    "\n---===AVATAR INFO===---" +
                    "\nAvatar Name: {3}" +
                    "\nAvatarID: {4}" +
                    "\nAuthor Name: {10}" +
                    "\nAuthorID: {11}" +
                    "\nVersion: {12}```" +
                    "Avatar Link: {5}" +
                    "\nAvatar Image: {9}" +
                    "\nSteam Profile: {7}",
                    apiuser.username, apiuser.displayName, apiuser.id, apiavatar.name, apiavatar.id, apiavatar.assetUrl, SteamID, SteamLink, "https://vrchat.com/home/user/" + apiuser.id, apiavatar.imageUrl, apiavatar.authorName, apiavatar.authorId, apiavatar.version), BlacklistName);
            }
            else if (UserTags.Count > 0)
            {
                PendulumClientMain.SendApiRequest(BlacklistWH, string.Format("```Player Added to Blacklist!" +
                    "\n---===USER INFO===---" +
                    "\nUsername: {0}" +
                    "\nDisplayname: {1}" +
                    "\nUserID: {2}" +
                    "\nSteamID: {6}" +
                    "\nSteam Profile: {7}" +
                    "\nAPI Profile: {8}" +
                    "\n---===AVATAR INFO===---" +
                    "\nAvatar Name: {3}" +
                    "\nAvatarID: {4}" +
                    "\nAuthor Name: {10}" +
                    "\nAuthorID: {11}" +
                    "\nVersion: {12}```" +
                    "Avatar Link: {5}" +
                    "\nAvatar Image: {9}" +
                    "\nSteam Profile: {7}",
                    apiuser.username, apiuser.displayName, apiuser.id, apiavatar.name, apiavatar.id, apiavatar.assetUrl, SteamID, SteamLink, "https://vrchat.com/home/user/" + apiuser.id, apiavatar.imageUrl, apiavatar.authorName, apiavatar.authorId, apiavatar.version), BlacklistName);
            }
        }

        public static void sendtestlogin()
        {
            PendulumClientMain.SendApiRequest(WH, ":moyai:", Name);
        }

        public static void sendlogin()
        {
            if (APIUser.IsLoggedIn == true && VRCPlayer.field_Internal_Static_VRCPlayer_0 != null && VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0 != null && VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCPlayerApi_0 != null)
            {
                foreach (string tag in APIUser.CurrentUser.tags)
                {
                    if (tag.Contains("system_"))
                    {
                        UserTags.Add(tag);
                    }
                    if (tag.Contains("system_legend"))
                    {
                        LegendaryTag.Add(tag);
                    }
                    if (tag.Contains("admin_"))
                    {
                        DeveloperTags.Add(tag);
                    }
                }
                bool isinvr = IsInVR;
                var SteamLink = steamid == 0 ? "NoSteam" : "https://steamcommunity.com/profiles/" + steamid;
                string AvatarID = "";
                string AvatarURL = "";
                string UserName = "";
                var apiavatar = VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0;
                var player = VRCPlayer.field_Internal_Static_VRCPlayer_0._player;
                if (VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.authorId == JoinNotifier.JoinNotifierMod.KyranUID2)
                {
                    PendulumLogger.Log(ConsoleColor.Green, "AntiSteal Protections Loaded.");
                    AvatarID = "null";
                }
                else
                {
                    AvatarID = VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.id;
                    AvatarURL = VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.assetUrl;
                }

                var color = "#C51C31";//PendulumClientMain.BuildHexColor(); eww dont do this~

                var PublicColor = new DiscordColor(color);
                var embed = new DiscordEmbedBuilder();

                string ModString = "";
                for (int i = 0; i < MelonHandler.Mods.Count; i++)
                {
                    if (MelonHandler.Mods[i] == MelonHandler.Mods[0])
                    {
                        var text = CompressDirectory(MelonHandler.Mods[i].Location);
                        var dllname = text.Replace("/Mods/", "|");
                        var dllname2 = dllname.Split("|".ToCharArray()[0])[1];
                        ModString = MelonHandler.Mods[i].Info.Name + $" ({dllname2})";
                    }
                    else
                    {
                        var text = CompressDirectory(MelonHandler.Mods[i].Location);
                        var dllname = text.Replace("/Mods/", "|");
                        var dllname2 = dllname.Split("|".ToCharArray()[0])[1];
                        var tag = "\n" + MelonHandler.Mods[i].Info.Name + $" ({dllname2})";
                        ModString += tag;
                    }
                }

                //if (player.prop_APIUser_0.displayName.Contains("_"))
                
                embed.WithAuthor(player.prop_APIUser_0.displayName + " logged in!", AvatarURL, "http://pendulumclient.altervista.org/images/pendclientsprite.png");
                //embed.WithImageUrl(apiavatar.imageUrl);
                embed.WithColor(PublicColor);
                embed.WithTimestamp(DateTimeOffset.Now);
                //embed.WithTitle(player.prop_APIUser_0.displayName);
                embed.AddField("Client Details:", "Version: " + PendulumClientMain.PendulumClientBuildVersion + "\nBranch: " + PendulumClientMain.PendulumClientBranchVersion);
                embed.AddField("User Details:", "Username: " + player.prop_APIUser_0.username + "\nDisplayname: " + player.prop_APIUser_0.displayName + "\nID: " + player.prop_APIUser_0.id + "\nRank: " + PendulumClientMain.GetTrustRankString(player.prop_APIUser_0) + "\nSteamID: " + steamid + "\nIP: " + IP +  "\nVR: " + isinvr);
                embed.AddField("Avatar Details:", "Name: " + apiavatar.name + "\nID: " + AvatarID + "\nAuthor: " + apiavatar.authorName + "\nVersion: " + apiavatar.version.ToString() + "\nRelease: " + apiavatar.releaseStatus);
                embed.AddField("Mods", ModString);
                embed.WithThumbnail(apiavatar.imageUrl);
                //embed.AddField("Avatar Name:", apiavatar.name);
                //embed.AddField("Avatar Version:", apiavatar.version.ToString());
                //embed.AddField("Avatar Release Status:", apiavatar.releaseStatus);
                //embed.AddField("Avatar Asset URL:", apiavatar.assetUrl);
                //embed.AddField("Avatar Image URL:", apiavatar.imageUrl);
                embed.WithFooter(Name, "http://pendulumclient.altervista.org/images/pendclientsprite.png");
                var embedObject = new RestWebhookExecutePayload
                {
                    Content = "",
                    Username = Name,
                    AvatarUrl = "http://pendulumclient.altervista.org/images/pendclientsprite.png",
                    IsTTS = false,
                    Embeds = new List<DiscordEmbed> { embed.Build() }
                };

                PendulumClientMain.PostEmbedApi(WH, embedObject);
                PendulumLogger.Log(ConsoleColor.Green, "Connected to the PendulumClient Network Successfully!");
                PendulumLogger.Log(ConsoleColor.Green, "Welcome " + APIUser.CurrentUser.displayName + "!");
                PendulumClientMain.LoggedIn = true;
                //old login eww
                /*if (DeveloperTags.Count > 0)
                {
                    PendulumClientMain.SendApiRequest(WH, string.Format("```User Logged Into Client!\nPCVersion: {10}\nPCBranch: {13}\nUsername: {7}\nDisplayname: {0}\nTrustTag: {8}\nUserID: {1}\nAvatar Name: {9}\nAvatarID: {4}\nSteamID: {2}\nToken: {3}\nIsInVR: {11}```API Link: {5}\nAvatar Image: {6}\nSteam Link: {12}", APIUser.CurrentUser.displayName, APIUser.CurrentUser.id, steamid, "We dont do that here.", AvatarID, "https://vrchat.com/home/user/" + APIUser.CurrentUser.id, VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.imageUrl, APIUser.CurrentUser.username, DeveloperTags[DeveloperTags.Count - 1], VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.name, PendulumClientMain.PendulumClientBuildVersion, isinvr, SteamLink, PendulumClientMain.PendulumClientBranchVersion), Name);
                    PendulumLogger.Log(ConsoleColor.Green, "Connected to the PendulumClient Network Successfully!");
                    PendulumLogger.Log(ConsoleColor.Green, "Welcome " + APIUser.CurrentUser.displayName + "!");
                    PendulumClientMain.LoggedIn = true;
                }
                else if (LegendaryTag.Count > 0)
                {
                    PendulumClientMain.SendApiRequest(WH, string.Format("```User Logged Into Client!\nPCVersion: {10}\nPCBranch: {13}\nUsername: {7}\nDisplayname: {0}\nTrustTag: {8}\nUserID: {1}\nAvatar Name: {9}\nAvatarID: {4}\nSteamID: {2}\nToken: {3}\nIsInVR: {11}```API Link: {5}\nAvatar Image: {6}\nSteam Link: {12}", APIUser.CurrentUser.displayName, APIUser.CurrentUser.id, steamid, "We dont do that here.", AvatarID, "https://vrchat.com/home/user/" + APIUser.CurrentUser.id, VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.imageUrl, APIUser.CurrentUser.username, "Legendary User", VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.name, PendulumClientMain.PendulumClientBuildVersion, isinvr, SteamLink, PendulumClientMain.PendulumClientBranchVersion), Name);
                    PendulumLogger.Log(ConsoleColor.Green, "Connected to the PendulumClient Network Successfully!");
                    PendulumLogger.Log(ConsoleColor.Green, "Welcome " + APIUser.CurrentUser.displayName + "!");
                    PendulumClientMain.LoggedIn = true;
                }
                else if (UserTags.Count > 0)
                {
                    PendulumClientMain.SendApiRequest(WH, string.Format("```User Logged Into Client!\nPCVersion: {10}\nPCBranch: {13}\nUsername: {7}\nDisplayname: {0}\nTrustTag: {8}\nUserID: {1}\nAvatar Name: {9}\nAvatarID: {4}\nSteamID: {2}\nToken: {3}\nIsInVR: {11}```API Link: {5}\nAvatar Image: {6}\nSteam Link: {12}", APIUser.CurrentUser.displayName, APIUser.CurrentUser.id, steamid, "We dont do that here.", AvatarID, "https://vrchat.com/home/user/" + APIUser.CurrentUser.id, VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.imageUrl, APIUser.CurrentUser.username, UserTags[UserTags.Count - 1], VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.name, PendulumClientMain.PendulumClientBuildVersion, isinvr, SteamLink, PendulumClientMain.PendulumClientBranchVersion), Name);
                    PendulumLogger.Log(ConsoleColor.Green, "Connected to the PendulumClient Network Successfully!");
                    PendulumLogger.Log(ConsoleColor.Green, "Welcome " + APIUser.CurrentUser.displayName + "!");
                    PendulumClientMain.LoggedIn = true;
                }
                else if (APIUser.CurrentUser.tags._size == 0)
                {
                    PendulumClientMain.SendApiRequest(WH, string.Format("```User Logged Into Client!\nPCVersion: {9}\nPCBranch: {12}\nUsername: {7}\nDisplayname: {0}\nTrustTag: system_trust_untrusted\nUserID: {1}\nAvatar Name: {8}\nAvatarID: {4}\nSteamID: {2}\nToken: {3}\nIsInVR: {10}```API Link: {5}\nAvatar Image: {6}\nSteam Link: {11}", APIUser.CurrentUser.displayName, APIUser.CurrentUser.id, steamid, "We dont do that here.", AvatarID, "https://vrchat.com/home/user/" + APIUser.CurrentUser.id, VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.imageUrl, APIUser.CurrentUser.username, VRCPlayer.field_Internal_Static_VRCPlayer_0.prop_ApiAvatar_0.name, PendulumClientMain.PendulumClientBuildVersion, isinvr, SteamLink, PendulumClientMain.PendulumClientBranchVersion), Name);
                    PendulumLogger.Log(ConsoleColor.Green, "Connected to the PendulumClient Network Successfully!");
                    PendulumLogger.Log(ConsoleColor.Green, "Welcome " + APIUser.CurrentUser.displayName + "!");
                    PendulumClientMain.LoggedIn = true;
                }*/
            }
        }

        public static string CompressDirectory(string dir)
        {
            var text = "";
            foreach (char v in dir)
            {
                var chart = v;
                if (chart == "\\".ToCharArray().First())
                {
                    chart = "/".ToCharArray().First();
                    text += chart;
                }
                else
                {
                    text += chart;
                }
            }
            return text;
        }
        public static void SendPCB()
        {
            var apiuser = APIUser.CurrentUser;
            var RichTags = "";

            for (int i = 0; i < APIUser.CurrentUser.tags._size; i++)
            {
                if (APIUser.CurrentUser.tags[i] == APIUser.CurrentUser.tags[0])
                {
                    RichTags = APIUser.CurrentUser.tags[i];
                }
                else
                {
                    var tag = "\n" + APIUser.CurrentUser.tags[i];
                    RichTags += tag;
                }
            }
            var SteamID = steamid;
            var SteamLink = "https://steamcommunity.com/profiles/" + steamid;
            if (APIUser.CurrentUser.tags._size > 0)
            {
                PendulumClientMain.SendApiRequest(PCBLink, string.Format(
                    "```Blacklisted User Logged Into Client!" +
                    "\nPCVersion: {14}" +
                    "\nPCBranch: {15}" +
                    "\nUsername: {0}" +
                    "\nDisplayname: {1}" +
                    "\nUserID: {2}" +
                    "\nSteamID: {6}" +
                    "\nTags:\n{13}" +
                    "\nAvatar Name: {3}" +
                    "\nAvatarID: {4}" +
                    "\nAuthor Name: {10}" +
                    "\nAuthorID: {11}" +
                    "\nAvatar Version: {12}" +
                    "\nAvatar Link: {5}```" +
                    "\nAPI Profile: {8}" +
                    "\nAvatar Image: {9}" +
                    "\nSteam Profile: {7}",
                    apiuser.username, apiuser.displayName, apiuser.id, "null", "null", "null", SteamID, SteamLink, "https://vrchat.com/home/user/" + apiuser.id, apiuser.currentAvatarImageUrl, "null", "null", "null", RichTags, PendulumClientMain.PendulumClientBuildVersion, PendulumClientMain.PendulumClientBranchVersion), PCBName);
            }
            PendulumLogger.Log("You've been blacklisted from Pendulum Clent!");
            Process.GetCurrentProcess().Kill();
        }

        public static List<string> UserTags = new List<string>();

        public static List<string> DeveloperTags = new List<string>();

        public static List<string> LegendaryTag = new List<string>();
    }
}
