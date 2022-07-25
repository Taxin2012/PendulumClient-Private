using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using VRC.Core;
using VRC;
using JoinNotifier;
using PendulumClient.UI;
using System.Net.Http;
using UnityEngine;
using System.IO;

namespace PendulumClient.Main.Features
{
    internal class VRCDownloads
    {
        internal static async void DownloadAssetBundleAsync(string asseturl, string name, string extension, Action<bool, string> onDownloadCompleted = null)
        {
            var httpclient = new HttpClient();
            httpclient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36");
            httpclient.DefaultRequestHeaders.Add("X-Client-Version", Application.version);

            try
            {
                var BundleID = "";
                if (asseturl.StartsWith("https://api.vrchat.cloud/api/1/file/"))
                    BundleID = asseturl.Replace("https://api.vrchat.cloud/api/1/file/", "").Split('/')[0];

                var FolderName = $"PendulumClient/{extension.ToUpper()}/";
                var CompleteName = name;

                if (!string.IsNullOrEmpty(BundleID))
                    CompleteName += " - " + BundleID;

                if (!Directory.Exists(FolderName)) Directory.CreateDirectory(FolderName);
                var response = await httpclient.GetAsync(asseturl);
                using FileStream fileStream = new FileStream($"{FolderName}{CompleteName}.{extension}", FileMode.Create);
                await response.Content.CopyToAsync(fileStream);
                onDownloadCompleted?.Invoke(true, CompleteName);
                if (extension == "vrca")
                    NotificationsV2.SendHudNotificationThreaded($"Downloaded Avatar: {name}");
                else
                    NotificationsV2.SendHudNotificationThreaded($"Downloaded World: {name}");
            }
            catch(Exception e)
            {
                PendulumLogger.LogError("Error Downloading AssetBundle: " + e.ToString());
                onDownloadCompleted?.Invoke(false, null);
            }
        }
        internal static async void DownloadImageAsync(string imageurl, string name, string folder, Action<bool, string> onDownloadCompleted = null)
        {
            var httpclient = new HttpClient();
            httpclient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.159 Safari/537.36");
            httpclient.DefaultRequestHeaders.Add("X-Client-Version", Application.version);

            try
            {
                var ImageID = "";
                if (imageurl.StartsWith("https://api.vrchat.cloud/api/1/file/"))
                    ImageID = imageurl.Replace("https://api.vrchat.cloud/api/1/file/", "").Split('/')[0];

                var FolderName = $"PendulumClient/{folder.ToUpper()}/";
                var CompleteName = $"{name} - IMAGE";

                if (!string.IsNullOrEmpty(ImageID))
                    CompleteName += " - " + ImageID;

                if (!Directory.Exists(FolderName)) Directory.CreateDirectory(FolderName);
                var response = await httpclient.GetAsync(imageurl);
                using FileStream fileStream = new FileStream($"{FolderName}{CompleteName}.png", FileMode.Create);
                await response.Content.CopyToAsync(fileStream);
                onDownloadCompleted?.Invoke(true, CompleteName);
            }
            catch (Exception e)
            {
                PendulumLogger.LogError("Error Downloading Image: " + e.ToString());
                onDownloadCompleted?.Invoke(false, null);
            }
        }
        internal static void DownloadVRCA(Player player)
        {
            //GameObject.Destroy(user_menu.transform.Find("VRCAButton").gameObject);
            /*WebClient downloadhandler = new WebClient();
            var DownloadedBundle = downloadhandler.DownloadData(player._vrcplayer.prop_ApiAvatar_0.assetUrl);
            downloadhandler.DownloadDataCompleted += new DownloadDataCompletedEventHandler(VRCADownloaded);
            File.WriteAllBytes("PendulumClient/VRCA/" + player._vrcplayer.prop_ApiAvatar_0.name + ".vrca", DownloadedBundle);
            StoredVRCAPath = player._vrcplayer.prop_ApiAvatar_0.name + ".vrca";
            VRCADataDownloaded = true;*/
            /*if (IsDownloadingFile == true)
            {
                AlertPopup.SendAlertPopup("You already have a pending download!");
                return;
            }*/
            if (JoinNotifierMod.DevUserIDs.Contains(player._vrcplayer.prop_ApiAvatar_0.authorId))
            {
                if (APIUser.CurrentUser.id != JoinNotifierMod.KyranUID2 || APIUser.CurrentUser.id != JoinNotifierMod.KyranUID1)
                {
                    NotificationsV2.AddNotification("You cant steal this avatar!");
                    return;
                }
            }

            ApiAvatar avatar = player._vrcplayer.prop_ApiAvatar_0;

            NotificationsV2.AddNotification($"Downloading Avatar: {avatar.name}");

            DownloadAssetBundleAsync(avatar.assetUrl, avatar.name, "vrca");
            DownloadImageAsync(avatar.imageUrl, avatar.name, "vrca");
        }

        internal static void DowloadVRCW(ApiWorld world)
        {
            //GameObject.Destroy(user_menu.transform.Find("VRCAButton").gameObject);
            /*WebClient downloadhandler = new WebClient();
            var DownloadedBundle = downloadhandler.DownloadData(player._vrcplayer.prop_ApiAvatar_0.assetUrl);
            downloadhandler.DownloadDataCompleted += new DownloadDataCompletedEventHandler(VRCADownloaded);
            File.WriteAllBytes("PendulumClient/VRCA/" + player._vrcplayer.prop_ApiAvatar_0.name + ".vrca", DownloadedBundle);
            StoredVRCAPath = player._vrcplayer.prop_ApiAvatar_0.name + ".vrca";
            VRCADataDownloaded = true;*/
            if (JoinNotifierMod.DevUserIDs.Contains(world.authorId))
            {
                if (APIUser.CurrentUser.id != JoinNotifierMod.KyranUID2 || APIUser.CurrentUser.id != JoinNotifierMod.KyranUID1)
                {
                    NotificationsV2.AddNotification("You cant steal this world!");
                    return;
                }
            }

            NotificationsV2.AddNotification($"Downloading World: {world.name}");

            DownloadAssetBundleAsync(world.assetUrl, world.name, "vrcw");
            DownloadImageAsync(world.imageUrl, world.name, "vrcw");
        }

        /*private static void downloadFileVRCA(string asseturl, string avatarname)
        {
            AlertPopup.SendAlertPopup("Downloading " + avatarname + ".vrca");
            var Path = "PendulumClient/VRCA/" + avatarname + ".vrca";
            StoredVRCAPath = avatarname + ".vrca";
            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                wc.DownloadFileCompleted += OnVRCAComplete;
                wc.DownloadFileAsync(new Uri(asseturl), Path);
                //wc.DownloadFile(asseturl, Path);
            }
            catch (Exception e)
            {
                PendulumLogger.Log("Download Error: " + e.ToString());
            }
        }

        private static void downloadFileVRCW(string asseturl, string worldname)
        {
            AlertPopup.SendAlertPopup("Downloading " + worldname + ".vrcw");
            var Path = "PendulumClient/VRCW/" + worldname + ".vrcw";
            StoredVRCAPath = worldname + ".vrcw";
            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                wc.DownloadFileCompleted += OnVRCWComplete;
                wc.DownloadFileAsync(new Uri(asseturl), Path);
                //wc.DownloadFile(asseturl, Path);
            }
            catch (Exception e)
            {
                PendulumLogger.Log("Download Error: " + e.ToString());
            }
        }

        private static void OnVRCWComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                AlertPopup.SendAlertPopup("The download has been cancelled");
                IsDownloadingFile = false;
                return;
            }

            if (e.Error != null)
            {
                AlertPopup.SendAlertPopup("An error ocurred while trying to download file");
                PendulumLogger.Log("Download Error: " + e.Error.ToString());
                IsDownloadingFile = false;
                return;
            }

            if (MenuFunctions.OpenFileOnDownload)
            {
                string filePath = Environment.CurrentDirectory + "/PendulumClient/VRCW/" + StoredVRCAPath;
                if (File.Exists(filePath))
                {
                    string p = "";
                    foreach (var chr in filePath)
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
                    string args = string.Format("/e, /select, \"{0}\"", p);

                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = "explorer";
                    info.Arguments = args;
                    Process.Start(info);
                }
            }
            AlertPopup.SendAlertPopup(StoredVRCAPath + "\nSuccessfully downloaded!");
            StoredVRCAPath = "";
            IsDownloadingFile = false;
        }

        private static void OnVRCAComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                AlertPopup.SendAlertPopup("The download has been cancelled");
                IsDownloadingFile = false;
                return;
            }

            if (e.Error != null)
            {
                AlertPopup.SendAlertPopup("An error ocurred while trying to download file");
                PendulumLogger.Log("Download Error: " + e.Error.ToString());
                IsDownloadingFile = false;
                return;
            }

            if (MenuFunctions.OpenFileOnDownload)
            {
                string filePath = Environment.CurrentDirectory + "/PendulumClient/VRCA/" + StoredVRCAPath;
                if (File.Exists(filePath))
                {
                    string p = "";
                    foreach (var chr in filePath)
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
                    string args = string.Format("/e, /select, \"{0}\"", p);

                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = "explorer";
                    info.Arguments = args;
                    Process.Start(info);
                }
            }
            //DownloadButton(false, DevToolsMenu.VRCAButton);
            AlertPopup.SendAlertPopup(StoredVRCAPath + "\nSuccessfully downloaded!");
            StoredVRCAPath = "";
            IsDownloadingFile = false;
        }*/
    }
}
