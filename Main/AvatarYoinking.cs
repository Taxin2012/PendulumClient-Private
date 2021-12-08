using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using VRC.Core;
using PendulumClient.AssetUploading;
using MelonLoader;
using PendulumClient.UI;
using System.Net.Http;
using UnityEngine;

using CompressionType = PendulumClient.AssetUploading.CompressionType;
using AssetBundle = PendulumClient.AssetUploading.AssetBundle;

namespace PendulumClient.Main
{
    public static class AvatarYoinking
    {
        private static string NewAvatarID = string.Empty;
        private static ApiAvatar SelectedAvatar;
        private static ApiAvatar apiAvatar_1;
        private static ApiFile AvatarAssetBundle;

        private static Assembly assembly_0;

        private static Type GetUnpackerType;

        private static MethodInfo GetUnpackerMethod;

        private static readonly string AssetBundlePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "AssetBundles");
        private static readonly string VrcaStorePath = Path.Combine(AssetBundlePath, "VrcaStore");

        public static void OnStart()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PendulumClient.Main.UBPU.exe"))
            {
                using (MemoryStream memoryStream = new MemoryStream((int)stream.Length))
                {
                    stream.CopyTo(memoryStream);
                    assembly_0 = Assembly.Load(memoryStream.ToArray());
                }
            }
            GetUnpackerType = assembly_0.GetTypes().First((Type type_0) => type_0.Name.Equals("Program"));
            GetUnpackerMethod = GetUnpackerType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic).First((MethodInfo methodInfo_0) => methodInfo_0.Name.Equals("Main"));
        }

        private static string GenerateAvatarID()
        {
            return "avtr_" + Guid.NewGuid().ToString();
        }

        private static string DownloadAvatar(ApiAvatar apiAvatar_2)
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
            byte[] bytes = http.GetByteArrayAsync(apiAvatar_2.assetUrl).GetAwaiter().GetResult();
            string text = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".vrca");
            File.WriteAllBytes(text, bytes);
            PendulumLogger.Log("Downloading Avatar");
            return text;

        }

        private static string GetDumpedVRCA(string path)
        {
            string result = string.Empty;
            System.Collections.Generic.IEnumerable<string> enumerable = from string_0 in Directory.EnumerateFiles(path + "_dump")
                                                                        select new FileInfo(string_0) into fileInfo_0
                                                                        orderby fileInfo_0.CreationTime
                                                                        select fileInfo_0.FullName;
            if (enumerable.Count() != 1)
            {
                foreach (string item in enumerable)
                {
                    if (string.IsNullOrEmpty(Path.GetExtension(item)))
                    {
                        result = item;
                    }
                }
                return result;
            }
            return enumerable.ElementAt(0);
        }

        public static void RunUBPU(string[] string_21)
        {
            try
            {
                GetUnpackerMethod?.Invoke(null, new object[1]
                {
                    string_21
                });
            }
            catch (Exception ex)
            {
                PendulumLogger.Error(ex.ToString());
            }
        }

        private static string UncompressBundle(string downloadedpath)
        {

            string AssetBundlePath = string.Empty;

            if (File.Exists(downloadedpath))
            {
                if (!Directory.Exists(AvatarYoinking.AssetBundlePath))
                {
                    Directory.CreateDirectory(AvatarYoinking.AssetBundlePath);
                }
                string path = AvatarYoinking.AssetBundlePath + "/" + Path.GetFileName(downloadedpath);
                File.Move(downloadedpath, path);
                if (File.Exists(path))
                {
                    File.Delete(downloadedpath);
                    PendulumLogger.Log("Decompressing assetbundle..");
                    RunUBPU(new string[1]
                    {
                    path
                    });

                    //await Task.Delay(1000);
                    System.Threading.Thread.Sleep(1000);
                    PendulumLogger.Log("Finished Decompressing");
                    AssetBundlePath = GetDumpedVRCA(path);
                    File.Delete(path);
                    return AssetBundlePath;
                }
                else
                {
                    return AssetBundlePath;
                }
            }
            else
            {
                return AssetBundlePath;
            }
        }

        private static int Dunnowtfisdis(byte[] byte_0, byte[] byte_1)
        {
            int num = 0;
            while (true)
            {
                if (num < byte_0.Length - byte_1.Length)
                {
                    bool flag = true;
                    for (int i = 0; i < byte_1.Length; i++)
                    {
                        if (byte_0[num + i] != byte_1[i])
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                    num++;
                    continue;
                }
                return -1;
            }
            return num;
        }

        private static bool ReplaceID(string filepath, string NewID, string OldID)
        {
            //await Task.Delay(100);
            System.Threading.Thread.Sleep(100);
            try
            {
                byte[] array = File.ReadAllBytes(filepath);
                byte[] bytes = Encoding.ASCII.GetBytes(OldID);
                Encoding.ASCII.GetBytes(OldID.ToLower());
                byte[] bytes2 = Encoding.ASCII.GetBytes(NewID);
                if (!OldID.Contains("avtr_") && !OldID.Contains("wrld_"))
                {
                    PendulumLogger.Log("Custom ID found");
                    return false;
                }
                byte[] array2 = new byte[array.Length + bytes2.Length - bytes.Length];
                byte[] array3 = array;
                int num;
                while ((num = Dunnowtfisdis(array3, bytes)) >= 0)
                {
                    Buffer.BlockCopy(array3, 0, array2, 0, num);
                    Buffer.BlockCopy(bytes2, 0, array2, num, bytes2.Length);
                    Buffer.BlockCopy(array3, num + bytes.Length, array2, num + bytes2.Length, array3.Length - num - bytes.Length);
                    array3 = array2;
                }
                File.WriteAllBytes(filepath, array2);
                PendulumLogger.Log("AssetBundle overwritten");
                return true;
            }
            catch (Exception ex)
            {
                PendulumLogger.Error(ex.ToString());
                return false;
            }
        }

        private static string GetXMLFile()
        {
            string result = string.Empty;
            System.Collections.Generic.IEnumerable<string> enumerable = from Path in Directory.EnumerateFiles(AssetBundlePath)
                                                                        select new FileInfo(Path) into File
                                                                        orderby File.CreationTime
                                                                        select File.FullName;
            if (enumerable.Any())
            {
                foreach (string item in enumerable)
                {
                    if (item.EndsWith(".xml"))
                    {
                        result = item;
                    }
                }
                return result;
            }
            return result;
        }

        private static string GetLZ4HCFile()
        {
            string result = string.Empty;
            System.Collections.Generic.IEnumerable<string> enumerable = from Path in Directory.EnumerateFiles(AssetBundlePath)
                                                                        select new FileInfo(Path) into File
                                                                        orderby File.CreationTime
                                                                        select File.FullName;
            if (enumerable.Any())
            {
                foreach (string item in enumerable)
                {
                    if (item.EndsWith(".LZ4HC"))
                    {
                        result = item;
                    }
                }
                return result;
            }
            return result;
        }

        private static string CompressAssetBundle()
        {
            try
            {
                string text = GetXMLFile();
                if (string.IsNullOrEmpty(text))
                {
                    PendulumLogger.Log("XML File Empty!");
                    return string.Empty;
                }
                PendulumLogger.Log("Compressing Assetbundle..");
                string directoryName = Path.GetDirectoryName(Application.dataPath);
                Directory.SetCurrentDirectory(AssetBundlePath);
                RunUBPU(new string[2]
                {
                    text,
                    "lz4hc"
                });
                //await Task.Delay(1000);
                System.Threading.Thread.Sleep(1000);
                Directory.SetCurrentDirectory(directoryName);
                PendulumLogger.Log("Finished Compressing");
                var Compressed = GetLZ4HCFile();
                if (!string.IsNullOrEmpty(Compressed))
                {
                    int startIndex = Compressed.IndexOf(".LZ4HC");
                    string fileName = Path.GetFileName(Compressed.Remove(startIndex, 6));
                    if (!Directory.Exists(VrcaStorePath))
                    {
                        Directory.CreateDirectory(VrcaStorePath);
                    }
                    string destFileName = Path.Combine(VrcaStorePath, fileName);
                    File.Move(Compressed, destFileName);
                    return destFileName;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                PendulumLogger.Error(ex.Message);
                return string.Empty;
            }
        }

        private static string DownloadImage(string string_21)
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
            HttpResponseMessage httpResponseMessage = http.GetAsync(string_21).GetAwaiter().GetResult();
            byte[] array = httpResponseMessage.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            if (array == null || array.Length == 0)
            {
                PendulumLogger.Log("image was null or 0");
            }
            string text = httpResponseMessage.Content.Headers.GetValues("Content-Type").First().Split('/')[1];
            string text2 = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + "." + text);
            File.WriteAllBytes(text2, array);
            PendulumLogger.Log("DownloadImage");
            return text2;
        }

        private static void OnUploadVrcaAsyncFailure(ApiFile ImageUrl, string status)
        {
            PendulumLogger.Log("VRCA Upload Failed");
        }

        private static void OnUploadVrcaAsyncSuccess(ApiFile avatar, string string_21)
        {
            PendulumLogger.Log("VRCA Uploaded");
            AvatarAssetBundle = avatar;
            PendulumLogger.Log("AvatarAssetBundle : " + avatar.GetFileURL());
            var image = "";//DownloadImage(SelectedAvatar.imageUrl);
            if (!string.IsNullOrEmpty(image))
            {
                PendulumLogger.Log("Avatar Image downloaded");
                PendulumLogger.Log("Uploading Image...");
                ApiFileHelper.upload(image, null, avatar.GetFileURL(), OnUploadVrcaAsynSuccess, OnUploadImageAsyncFailure, delegate (ApiFile apiFile_0, string string_0, string string_1, float Progress)
                {
                    PendulumLogger.Log($"Avatar Image Uploading: {Math.Round(Progress * 100, 2, MidpointRounding.AwayFromZero)}%");
                }, (ApiFile Assets) => false);
            }
            else
            {
                PendulumLogger.Log("Avatar Image missing - replacing with WengaPort Image");
                image = DownloadImage("https://media.discordapp.net/attachments/609571882009755651/886364010092576818/corbinss.png");
                PendulumLogger.Log("Uploading Image...");
                ApiFileHelper.upload(image, null, avatar.GetFileURL(), OnUploadVrcaAsynSuccess, OnUploadImageAsyncFailure, delegate (ApiFile apiFile_0, string string_0, string string_1, float Progress)
                {
                    PendulumLogger.Log($"Avatar Image Uploading: {Math.Round(Progress * 100, 2, MidpointRounding.AwayFromZero)}%");
                }, (ApiFile Assets) => false);
            }
        }

        private static void OnUploadImageAsyncFailure(ApiFile apiFile_2, string string_21)
        {
            PendulumLogger.Log("Avatar Image upload failed");
        }

        private static void ClearOldSession()
        {
            try
            {
                DirectoryInfo VrcaStore = new DirectoryInfo(VrcaStorePath);
                DirectoryInfo AssetBundles = new DirectoryInfo(AssetBundlePath);
                foreach (DirectoryInfo dir in AssetBundles.GetDirectories())
                {
                    if (!dir.Name.Contains("VrcaStore"))
                    {
                        dir.Delete(true);
                    }
                }
                foreach (FileInfo file in AssetBundles.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in VrcaStore.GetDirectories())
                {
                    dir.Delete(true);
                }
                foreach (FileInfo file in VrcaStore.GetFiles())
                {
                    file.Delete();
                }
                if (Directory.Exists(AvatarYoinking.VrcaStorePath))
                {
                    Directory.Delete(AvatarYoinking.VrcaStorePath);
                }
                if (Directory.Exists(AvatarYoinking.AssetBundlePath))
                {
                    Directory.Delete(AvatarYoinking.AssetBundlePath);
                }
            }
            catch { }

        }
        private static void OnUploadVrcaAsynSuccess(ApiFile ImageUrl, string status)
        {

            PendulumLogger.Log("VRCA Upload Success");
            PendulumLogger.Log("ImageURL: " + ImageUrl.GetFileURL());
            apiAvatar_1 = new ApiAvatar
            {
                id = NewAvatarID,
                authorName = APIUser.CurrentUser.username,
                authorId = APIUser.CurrentUser.id,
                name = SelectedAvatar.name,
                imageUrl = ImageUrl.GetFileURL(),
                assetUrl = AvatarAssetBundle.GetFileURL(),
                description = "joe biden",
                releaseStatus = "private"
            };
            apiAvatar_1.Post((Action<ApiContainer>)OnApiAvatarPostSuccess, (Action<ApiContainer>)OnApiAvatarPostFailure);
        }

        private static void OnApiAvatarPostFailure(ApiContainer apiContainer_0)
        {
            PendulumLogger.Log("Failed to Reupload Avatar: " + apiContainer_0.Error);
            ClearOldSession();
        }

        private static void OnApiAvatarPostSuccess(ApiContainer apiContainer_0)
        {
            PendulumLogger.Log("Avatar Reuploaded");
            //VRConsole.Log(VRConsole.LogsType.Avatar, $"Avatar Reuploaded");
            ClearOldSession();
        }


        public static void ReuploadAvatar(string avatarID)
        {
            Debug.developerConsoleVisible = true;
            if (string.IsNullOrEmpty(avatarID))
            {
                PendulumLogger.Log("No AvatarID found");
                return;
            }
            API.Fetch<ApiAvatar>(avatarID, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
            {
                SelectedAvatar = apiContainer_0.Model.Cast<ApiAvatar>();
                if (SelectedAvatar == null)
                {
                    PendulumLogger.Log("Failed to get Avatar");
                }
                else
                {
                    NewAvatarID = GenerateAvatarID();
                    API.Fetch<ApiAvatar>(NewAvatarID, (Action<ApiContainer>)delegate
                    {
                        PendulumLogger.Log("AvatarId " + NewAvatarID + " already in use");
                        ReuploadAvatar(avatarID);
                    }, (Action<ApiContainer>)delegate
                    {
                        PendulumLogger.Log("AvatarId: " + SelectedAvatar.id + " | AssetUrl: " + SelectedAvatar.assetUrl + " | Author: " + SelectedAvatar.authorName);
                        try
                        {
                            string DownloadPath = DownloadAvatar(SelectedAvatar);
                            if (!string.IsNullOrEmpty(DownloadPath))
                            {
                                PendulumLogger.Log("Avatar Downloaded");
                                string UncompressedVRCA = UncompressBundle(DownloadPath);
                                PendulumLogger.Log("AssetBundle created");
                                if (!string.IsNullOrEmpty(UncompressedVRCA))
                                {
                                    string unityVersion = SelectedAvatar.unityVersion.ToLower();
                                    string platform = SelectedAvatar.platform.ToLower();
                                    string ApiVersion = ApiWorld.VERSION.ApiVersion.ToString().ToLower();
                                    if (string.IsNullOrEmpty(ApiVersion))
                                    {
                                        ApiVersion = "4";
                                    }
                                    var avatarimage = "Avatar - " + SelectedAvatar.name + " - Image - " + unityVersion + "_" + ApiVersion + "_" + platform + "_Release";
                                    var AvatarAssetBundle = "Avatar - " + SelectedAvatar.name + " - Asset bundle - " + unityVersion + "_" + ApiVersion + "_" + platform + "_Release";
                                    if (!ReplaceID(UncompressedVRCA, NewAvatarID, SelectedAvatar.id))
                                    {
                                        PendulumLogger.Log("Failed to set AvatarID");
                                    }
                                    string PackedBundle = CompressAssetBundle();
                                    if (!string.IsNullOrEmpty(PackedBundle))
                                    {
                                            PendulumLogger.Log("Uploading VRCA...");
                                            ApiFileHelper.upload(PackedBundle, null, AvatarAssetBundle, OnUploadVrcaAsyncSuccess, OnUploadVrcaAsyncFailure, delegate (ApiFile imageBundle, string string_0, string string_1, float UploadingStatus)
                                            {
                                                PendulumLogger.Log($"VRCA Uploading Progress: {Math.Round(UploadingStatus * 100, 2, MidpointRounding.AwayFromZero)}%");
                                            }, (ApiFile File) => false);
                                    }
                                    else
                                    {
                                        PendulumLogger.Log("Failed to recompress AssetBundle");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            PendulumLogger.Error(ex.ToString());
                        }
                    });
                }
            }, (Action<ApiContainer>)delegate
            {
                PendulumLogger.Log("Couldn't fetch avatar (API)");
            });
        }
        /*public static void Init(this ApiAvatar avatar, string id, APIUser user, string name, string imageUrl, string assetUrl, string description, string releaseStatus, Il2CppSystem.Collections.Generic.List<string> tags, string packageUrl = null)
        {
            avatar.id = id;
            avatar.authorName = user.displayName;
            avatar.authorId = user.id;
            avatar.name = name;
            avatar.assetUrl = assetUrl;
            avatar.imageUrl = imageUrl;
            avatar.description = description;
            avatar.releaseStatus = releaseStatus;
            avatar.tags = tags;
            avatar.unityPackageUrl = packageUrl;
        }


        public static void DeleteAvatar(ApiAvatar avatar)
        {
            System.Action<ApiContainer> Success = (ApiContainer c) =>
            {
                PendulumLogger.Log("Avatar Deleted!");
            };

            System.Action<ApiContainer> Failure = (ApiContainer c) =>
            {
                PendulumLogger.Log("Avatar Delete Fail: " + c.Error);
            };

            avatar.Delete(Success, Failure);
        }

        private static void DownloadVRCImage(string address, string outFileName, out string finalFilePath)
        {
            using (WebClient wc = new WebClient())
            {
                byte[] imageBytes = wc.DownloadData(address);
                finalFilePath = string.Concat(outFileName, '.', wc.ResponseHeaders[HttpResponseHeader.ContentType].Split('/')[1]);
                File.WriteAllBytes(finalFilePath, imageBytes);
            }
        }

        public static string CalculateHash<T>(string input) where T : HashAlgorithm
        {
            byte[] hashBytes = Encoding.UTF8.GetBytes(input);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
                sb.Append(hashBytes[i].ToString("x2"));

            return sb.ToString();
        }

        public static ApiAvatar SaveAvatar(ApiAvatar avatar, string name)
        {
            Console.WriteLine("Saving avatar...");
            AlertPopup.SendAlertPopup("Saving Avatar...");
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

            using (WebClient wc = new WebClient())
            {
                string tempFile = string.Concat(Path.GetTempPath(), Path.GetRandomFileName(), ".vrca");
                try
                {
                    wc.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
                    string friendlyName = string.Concat(CalculateHash<MD5>(Guid.NewGuid().ToString()), Path.GetRandomFileName());
                    wc.DownloadFile(avatar.assetUrl, tempFile);

                    Console.WriteLine("Decompressing...");
                    AlertPopup.SendAlertPopup("Decompressing...");

                    AssetBundle assetBundle = new AssetBundle(tempFile);

                    Console.WriteLine("Decompressed...");
                    //AlertPopup.SendAlertPopup("Decompressed...");

                    void OnError(string error)
                    {
                        Console.WriteLine("Error saving avatar: {0}", error);
                        File.Delete(tempFile);
                    }

                    System.Action<ApiContainer> SuccessCallback = (ApiContainer c) =>
                    {
                        string fileUrl = "https://api.vrchat.cloud/api/1/file/" + c.Model.id + "/1/file";

                        ApiAvatar newAvatar = new ApiAvatar();
                        //.Init(null, APIUser.CurrentUser, name, avatar.imageUrl, fileUrl, "not pogger", "private", null, null);
                        newAvatar.id = null;
                        newAvatar.authorName = APIUser.CurrentUser.displayName;
                        newAvatar.authorId = APIUser.CurrentUser.id;
                        newAvatar.name = name;
                        newAvatar.imageUrl = avatar.imageUrl;
                        newAvatar.assetUrl = fileUrl;
                        newAvatar.description = "not pogger";
                        newAvatar.releaseStatus = "private";
                        newAvatar.tags = null;
                        newAvatar.unityPackageUrl = null;

                        System.Action<ApiContainer> SaveSuccess = (ApiContainer f) =>
                        {
                            PendulumLogger.Log("ApiAvatar Saved!");
                            Console.WriteLine("Changing blueprint id...");
                            assetBundle.SetAvatarId(f.Model.id);
                            Console.WriteLine("Recompressing...");
                            assetBundle.SaveTo(tempFile);
                            Console.WriteLine("Uploading file...");

                            ApiFileHelper.UploadFileAsync(tempFile, null, friendlyName, (assetFile, bt) =>
                            {
                                DownloadVRCImage(avatar.imageUrl, friendlyName, out string imagePath);
                                ApiFileHelper.UploadFileAsync(imagePath, null, friendlyName, (imageFile, msg) =>
                                {
                                    Console.WriteLine("Saving ApiAvatar...");
                                    newAvatar.imageUrl = imageFile.GetFileURL();
                                    newAvatar.assetUrl = assetFile.GetFileURL();
                                    System.Action<ApiContainer> AVSave = (ApiContainer z) =>
                                    {
                                        PendulumLogger.Log("Avatar saved!");
                                        AlertPopup.SendAlertPopup("Avatar saved!");
                                    };
                                    System.Action<ApiContainer> AVSaveFail = (ApiContainer z) =>
                                    {
                                        PendulumLogger.Log("Avatar Save Fail: " + z.Error);
                                        AlertPopup.SendAlertPopup("Avatar Save Fail");
                                    };
                                    newAvatar.Save(AVSave, AVSaveFail);
                                }, (_, error) => OnError(error), (_, b, d, e) => { }, _ => false);

                            }
                            , (_, error) => OnError(error), (az, b, d, e) => { }, _ => false);
                        };

                        System.Action<ApiContainer> SaveFailure = (ApiContainer d) =>
                        {
                            PendulumLogger.Log("ApiAvatar Save Fail: " + d.Error);
                            AlertPopup.SendAlertPopup("ApiAvatar Save Fail");
                        };

                        newAvatar.Save(SaveSuccess, SaveFailure);

                        /*PendulumLogger.Log("Changing blueprint id...");
                        AlertPopup.SendAlertPopup("Changing blueprint id...");
                        assetBundle.SetAvatarId(c.Model.id);
                        PendulumLogger.Log("Recompressing...");
                        //AlertPopup.SendAlertPopup("Recompressing...");
                        assetBundle.SaveTo(tempFile);
                        PendulumLogger.Log("Uploading file...");
                        AlertPopup.SendAlertPopup("Uploading file...");
                        ApiFileHelper.UploadFileAsync(tempFile, null, friendlyName, (apifile, message) =>
                        {
                            PendulumLogger.Log("Saving ApiAvatar...");
                            AlertPopup.SendAlertPopup("Saving ApiAvatar...");
                            AvatarToUpload.assetUrl = apifile.GetFileURL();

                            /*DownloadVRCImage(avatar.imageUrl, friendlyName, out string imagePath);
                            ApiFileHelper.UploadFileAsync(imagePath, null, friendlyName, (imageFile, msg) =>
                            {
                                PendulumLogger.Log("Saving ApiAvatar...");
                                AlertPopup.SendAlertPopup("Saving ApiAvatar...");
                                //AvatarToUpload.imageUrl = imageFile.GetFileURL();

                                AvatarToUpload.Save(SaveSuccess, SaveFailure);
                            }, null, null, null);

                            
                        }
                        , null, null, null); */

        /*if (newAvatar.authorId == APIUser.CurrentUser.id)
        {
            newAvatar.Save(SaveSuccess2, SaveFailure2);

            PendulumLogger.Log("ApiFile Created!");
            AlertPopup.SendAlertPopup("ApiFile Created!");
        }*/
        /*};

        System.Action<ApiContainer> FailureCallback = (ApiContainer c) =>
        {
            PendulumLogger.Log("ApiFile Creation Fail: " + c.Error);
            AlertPopup.SendAlertPopup("ApiFile Creation Fail");
        };

        ApiFile.Create(friendlyName, "application/x-avatar", ".vrca", SuccessCallback, FailureCallback);

    }
    catch (Exception e)
    {
        PendulumLogger.Log("Error saving avatar: {0}", e.Message);
        File.Delete(tempFile);
    }
}

return avatar;
}

private static void ApiFileConversion(ApiFile apiFile, string message)
{
string AssetURL = apiFile.GetFileURL();
//AvatarToUpload.assetUrl = apiFile.GetFileURL();
}

private static void ApiFileConversionError(ApiFile apiFile, string message)
{
PendulumLogger.Log("File Conversion Failed: " + message);
}*/
    }
}
