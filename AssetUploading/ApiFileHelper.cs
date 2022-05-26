using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using BestHTTP;
using BestHTTP.Authentication;
using BestHTTP.JSON;
using DotZLib;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Tar;
using MelonLoader;
using UnhollowerBaseLib;
using librsync.net;
using Il2CppSystem.Collections.Generic;
using PendulumClient.Main;
using Debug = UnityEngine.Debug;
using System.Text.RegularExpressions;
using VRC.Core;
using VRC;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnhollowerRuntimeLib;


namespace PendulumClient.AssetUploading.unused
{
    public class ApiFileHelper : MonoBehaviour
    {
        public delegate void GDelegate0(ApiFile apiFile, string message);

        public delegate void GDelegate1(ApiFile apiFile, string error);

        public delegate void GDelegate2(ApiFile apiFile, string status, string subStatus, float pct);

        public delegate bool GDelegate3(ApiFile apiFile);

        public enum GEnum0
        {
            Success,
            Unchanged
        }

        public System.Delegate ReferencedDelegate;

        public System.IntPtr MethodInfo;

        public Il2CppSystem.Collections.Generic.List<MonoBehaviour> AntiGcList;

        private readonly int int_0 = 10485760;

        private readonly int int_1 = 52428800;

        private readonly float float_0 = 120f;

        private readonly float float_1 = 600f;

        private readonly float float_2 = 2f;

        private readonly float float_3 = 10f;

        private static bool bool_0;

        private readonly Regex[] regex_0 = new Regex[4]
        {
            new Regex("/LightingData\\.asset$"),
            new Regex("/Lightmap-.*(\\.png|\\.exr)$"),
            new Regex("/ReflectionProbe-.*(\\.exr|\\.png)$"),
            new Regex("/Editor/Data/UnityExtensions/")
        };

        private static ApiFileHelper helper;
        public static RemoteConfig remoteConfig_0;

        public static ApiFileHelper apifilehelper
        {
            get
            {
                smethod_9();
                return helper;
            }
        }

        public ApiFileHelper(System.IntPtr intptr_1) : base(intptr_1)
        {
            AntiGcList = new Il2CppSystem.Collections.Generic.List<MonoBehaviour>(1);
            AntiGcList.Add(this);
        }

        public ApiFileHelper(System.Delegate delegate_1, System.IntPtr intptr_1) : base(ClassInjector.DerivedConstructorPointer<ApiFileHelper>())
        {
            ClassInjector.DerivedConstructorBody(this);
            ReferencedDelegate = delegate_1;
            MethodInfo = intptr_1;
        }

        ~ApiFileHelper()
        {
            Marshal.FreeHGlobal(MethodInfo);
            MethodInfo = Il2CppSystem.IntPtr.Zero;
            ReferencedDelegate = null;
            AntiGcList.Remove(this);
            AntiGcList = null;
        }

        public static IEnumerator upload(string FilePath, string record, string AssetBundle, GDelegate0 success, GDelegate1 failure, GDelegate2 filecheck, GDelegate3 cancelled)
        {
            try
            {
                string extension = Path.GetExtension(FilePath);
                PendulumLogger.Log("Extension: " + extension);
            }
            catch
            { }
            return apifilehelper.Upload(FilePath, record, AssetBundle, success, failure, filecheck, cancelled);
        }

        public static string smethod_1(string string_0)
        {
            switch (string_0)
            {
                case ".vrcw":
                    return "application/x-world";

                case ".dll":
                    return "application/x-msdownload";

                case ".unitypackage":
                    return "application/gzip";

                case ".jpg":
                    return "image/jpg";

                default:
                    return "application/octet-stream";

                case ".delta":
                    return "application/x-rsync-delta";

                case ".sig":
                    return "application/x-rsync-signature";

                case ".png":
                    return "image/png";

                case ".gz":
                    return "application/gzip";

                case ".vrca":
                    return "application/x-avatar";
            }
        }

        public static bool smethod_2(string string_0)
        {
            return smethod_1(Path.GetExtension(string_0)) == "application/gzip";
        }

        public IEnumerator Upload(string Path, string record, string assetbundle, GDelegate0 success, GDelegate1 Failure, GDelegate2 filecheck, GDelegate3 Cancelled)
        {
            VRC.Core.Logger.Log("UploadFile: filename: " + Path + ", file id: " + ((!string.IsNullOrEmpty(record)) ? record : "<new>") + ", name: " + assetbundle, DebugLevel.All);
            if (!remoteConfig_0.IsInitialized())
            {
                bool bool_ = false;
                remoteConfig_0.Init((System.Action)delegate
                {
                    bool_ = true;
                }, (System.Action)delegate
                {
                    bool_ = true;
                });
                while (!bool_)
                {
                    yield return null;
                }
                if (!remoteConfig_0.IsInitialized())
                {
                    smethod_5(Failure, null, "Failed to fetch configuration.");
                    yield break;
                }
            }
            bool_0 = remoteConfig_0.GetBool("sdkEnableDeltaCompression");
            CheckFile(filecheck, null, "Checking file...");
            string whyNot;
            if (string.IsNullOrEmpty(Path))
            {
                smethod_5(Failure, null, "Upload filename is empty!");
            }
            else if (!System.IO.Path.HasExtension(Path))
            {
                smethod_5(Failure, null, "Upload filename must have an extension: " + Path);
            }
            else if (Tools.FileCanRead(Path, out whyNot))
            {
                CheckFile(filecheck, null, string.IsNullOrEmpty(record) ? "Creating file record..." : "Getting file record...");
                bool bool_0 = true;
                bool bool_3 = false;
                bool bool_2 = false;
                string string_5 = "";
                if (string.IsNullOrEmpty(assetbundle))
                {
                    assetbundle = Path;
                }
                string extension = System.IO.Path.GetExtension(Path);
                string mimeType = smethod_1(extension);
                ApiFile apiFile_2 = null;
                Action<ApiContainer> action = delegate (ApiContainer apiContainer_0)
                {
                    apiFile_2 = apiContainer_0.Model.Cast<ApiFile>();
                    bool_0 = false;
                };
                Action<ApiContainer> action2 = delegate (ApiContainer apiContainer_0)
                {
                    string_5 = apiContainer_0.Error;
                    bool_0 = false;
                    if (apiContainer_0.Code == 400)
                    {
                        bool_2 = true;
                    }
                };
                while (true)
                {
                    apiFile_2 = null;
                    bool_0 = true;
                    bool_2 = false;
                    string_5 = "";
                    if (string.IsNullOrEmpty(record))
                    {
                        ApiFile.Create(assetbundle, mimeType, extension, action, action2);
                    }
                    else
                    {
                        API.Fetch<ApiFile>(record, action, action2);
                    }
                    while (bool_0)
                    {
                        if (apiFile_2 != null && ApiFileHelper.Cancelled(Cancelled, Failure, apiFile_2))
                        {
                            yield break;
                        }
                        yield return null;
                    }
                    if (!string.IsNullOrEmpty(string_5))
                    {
                        if (string_5.Contains("File not found"))
                        {
                            PendulumLogger.Log("Couldn't find file record: " + record + ", creating new file record");
                            record = "";
                            continue;
                        }
                        string string_8 = (string.IsNullOrEmpty(record) ? "Failed to create file record." : "Failed to get file record.");
                        smethod_5(Failure, null, string_8, string_5);
                        if (!bool_2)
                        {
                            yield break;
                        }
                    }
                    if (bool_2)
                    {
                        yield return new WaitForSecondsRealtime(0.75f);
                        continue;
                    }
                    if (apiFile_2 == null)
                    {
                        yield break;
                    }
                    smethod_3(apiFile_2, bool_1: false, bool_2: true);
                    break;
                }
                string string_6;
                string string_7;
                while (true)
                {
                    if (apiFile_2.HasQueuedOperation(ApiFileHelper.bool_0))
                    {
                        bool_0 = true;
                        apiFile_2.DeleteLatestVersion((Action<ApiContainer>)delegate
                        {
                            bool_0 = false;
                        }, (Action<ApiContainer>)delegate
                        {
                            bool_0 = false;
                        });
                        while (bool_0)
                        {
                            if (apiFile_2 != null && ApiFileHelper.Cancelled(Cancelled, Failure, apiFile_2))
                            {
                                yield break;
                            }
                            yield return null;
                        }
                        continue;
                    }
                    yield return new WaitForSecondsRealtime(0.75f);
                    smethod_3(apiFile_2, bool_1: false);
                    if (apiFile_2.IsInErrorState())
                    {
                        PendulumLogger.Log("ApiFile: " + apiFile_2.id + ": server failed to process last uploaded, deleting failed version");
                        while (true)
                        {
                            CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Cleaning up previous version");
                            bool_0 = true;
                            string_5 = "";
                            bool_2 = false;
                            apiFile_2.DeleteLatestVersion(action, action2);
                            while (bool_0)
                            {
                                if (!ApiFileHelper.Cancelled(Cancelled, Failure, null))
                                {
                                    yield return null;
                                    continue;
                                }
                                yield break;
                            }
                            if (!string.IsNullOrEmpty(string_5))
                            {
                                smethod_5(Failure, apiFile_2, "Failed to delete previous failed version!", string_5);
                                if (!bool_2)
                                {
                                    smethod_8(apiFile_2.id);
                                    yield break;
                                }
                            }
                            if (!bool_2)
                            {
                                break;
                            }
                            yield return new WaitForSecondsRealtime(0.75f);
                        }
                    }
                    yield return new WaitForSecondsRealtime(0.75f);
                    smethod_3(apiFile_2, bool_1: false);
                    if (!apiFile_2.HasQueuedOperation(ApiFileHelper.bool_0))
                    {
                        CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Optimizing file");
                        string_6 = Tools.GetTempFileName(System.IO.Path.GetExtension(Path), out string_5, apiFile_2.id);
                        if (!string.IsNullOrEmpty(string_6))
                        {
                            bool_3 = false;
                            yield return MelonCoroutines.Start(method_1(Path, string_6, delegate (GEnum0 genum0_0)
                            {
                                if (genum0_0 == GEnum0.Unchanged)
                                {
                                    string_6 = Path;
                                }
                            }, delegate (string string_4)
                            {
                                smethod_5(Failure, apiFile_2, "Failed to optimize file for upload.", string_4);
                                smethod_8(apiFile_2.id);
                                bool_3 = true;
                            }));
                            if (bool_3)
                            {
                                break;
                            }
                            smethod_3(apiFile_2, bool_1: false);
                            CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Generating file hash");
                            bool_0 = true;
                            string_5 = "";
                            string text = Convert.ToBase64String(MD5.Create().ComputeHash(File.ReadAllBytes(string_6)));
                            bool_0 = false;
                            while (bool_0)
                            {
                                if (!ApiFileHelper.Cancelled(Cancelled, Failure, apiFile_2))
                                {
                                    yield return null;
                                    continue;
                                }
                                smethod_8(apiFile_2.id);
                                yield break;
                            }
                            if (!string.IsNullOrEmpty(string_5))
                            {
                                smethod_5(Failure, apiFile_2, "Failed to generate MD5 hash for upload file.", string_5);
                                smethod_8(apiFile_2.id);
                                break;
                            }
                            smethod_3(apiFile_2, bool_1: false);
                            CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Checking for changes");
                            bool flag = false;
                            if (apiFile_2.HasExistingOrPendingVersion())
                            {
                                if (string.Compare(text, apiFile_2.GetFileMD5(apiFile_2.GetLatestVersionNumber())) == 0)
                                {
                                    if (!apiFile_2.IsWaitingForUpload())
                                    {
                                        smethod_4(success, apiFile_2, "The file to upload is unchanged.");
                                        smethod_8(apiFile_2.id);
                                        break;
                                    }
                                    flag = true;
                                    PendulumLogger.Log("Retrying previous upload");
                                }
                                else if (apiFile_2.IsWaitingForUpload())
                                {
                                    do
                                    {
                                        CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Cleaning up previous version");
                                        bool_0 = true;
                                        bool_2 = false;
                                        string_5 = "";
                                        apiFile_2.DeleteLatestVersion(action, action2);
                                        while (bool_0)
                                        {
                                            if (!ApiFileHelper.Cancelled(Cancelled, Failure, apiFile_2))
                                            {
                                                yield return null;
                                                continue;
                                            }
                                            yield break;
                                        }
                                        if (!string.IsNullOrEmpty(string_5))
                                        {
                                            smethod_5(Failure, apiFile_2, "Failed to delete previous incomplete version!", string_5);
                                            if (!bool_2)
                                            {
                                                smethod_8(apiFile_2.id);
                                                yield break;
                                            }
                                        }
                                        yield return new WaitForSecondsRealtime(0.75f);
                                    }
                                    while (bool_2);
                                }
                            }
                            smethod_3(apiFile_2, bool_1: false);
                            CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Generating signature");
                            string tempFileName = Tools.GetTempFileName(".sig", out string_5, apiFile_2.id);
                            if (!string.IsNullOrEmpty(tempFileName))
                            {
                                bool_3 = false;
                                yield return MelonCoroutines.Start(method_2(string_6, tempFileName, delegate
                                {
                                }, delegate (string string_4)
                                {
                                    smethod_5(Failure, apiFile_2, "Failed to generate file signature!", string_4);
                                    smethod_8(apiFile_2.id);
                                    bool_3 = true;
                                }));
                                if (bool_3)
                                {
                                    break;
                                }
                                smethod_3(apiFile_2, bool_1: false);
                                CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Generating signature hash");
                                bool_0 = true;
                                string_5 = "";
                                string text2 = Convert.ToBase64String(MD5.Create().ComputeHash(File.ReadAllBytes(tempFileName)));
                                bool_0 = false;
                                while (bool_0)
                                {
                                    if (ApiFileHelper.Cancelled(Cancelled, Failure, apiFile_2))
                                    {
                                        smethod_8(apiFile_2.id);
                                        yield break;
                                    }
                                    yield return null;
                                }
                                if (string.IsNullOrEmpty(string_5))
                                {
                                    long size = 0L;
                                    if (Tools.GetFileSize(tempFileName, out size, out string_5))
                                    {
                                        smethod_3(apiFile_2, bool_1: false);
                                        string_7 = null;
                                        if (ApiFileHelper.bool_0 && apiFile_2.HasExistingVersion())
                                        {
                                            CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Downloading previous version signature");
                                            bool_0 = true;
                                            string_5 = "";
                                            apiFile_2.DownloadSignature((Action<Il2CppStructArray<byte>>)delegate (Il2CppStructArray<byte> il2CppStructArray_0)
                                            {
                                                string_7 = Tools.GetTempFileName(".sig", out string_5, apiFile_2.id);
                                                if (!string.IsNullOrEmpty(string_7))
                                                {
                                                    try
                                                    {
                                                        File.WriteAllBytes(string_7, il2CppStructArray_0);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        string_7 = null;
                                                        string_5 = "Failed to write signature temp file:\n" + ex.Message;
                                                    }
                                                    bool_0 = false;
                                                }
                                                else
                                                {
                                                    string_5 = "Failed to create temp file: \n" + string_5;
                                                    bool_0 = false;
                                                }
                                            }, (Action<string>)delegate (string string_4)
                                            {
                                                string_5 = string_4;
                                                bool_0 = false;
                                            }, (Action<long, long>)delegate (long long_0, long long_1)
                                            {
                                                CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Downloading previous version signature", Tools.DivideSafe(long_0, long_1));
                                            });
                                            while (bool_0)
                                            {
                                                if (!ApiFileHelper.Cancelled(Cancelled, Failure, apiFile_2))
                                                {
                                                    yield return null;
                                                    continue;
                                                }
                                                smethod_8(apiFile_2.id);
                                                yield break;
                                            }
                                            if (!string.IsNullOrEmpty(string_5))
                                            {
                                                smethod_5(Failure, apiFile_2, "Failed to download previous file version signature.", string_5);
                                                smethod_8(apiFile_2.id);
                                                break;
                                            }
                                        }
                                        smethod_3(apiFile_2, bool_1: false);
                                        string text3 = null;
                                        if (ApiFileHelper.bool_0 && !string.IsNullOrEmpty(string_7))
                                        {
                                            CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Creating file delta");
                                            text3 = Tools.GetTempFileName(".delta", out string_5, apiFile_2.id);
                                            if (string.IsNullOrEmpty(text3))
                                            {
                                                smethod_5(Failure, apiFile_2, "Failed to create file delta for upload.", "Failed to create temp file: \n" + string_5);
                                                smethod_8(apiFile_2.id);
                                                break;
                                            }
                                            bool_3 = false;
                                            yield return MelonCoroutines.Start(method_3(string_6, string_7, text3, delegate
                                            {
                                            }, delegate (string string_4)
                                            {
                                                smethod_5(Failure, apiFile_2, "Failed to create file delta for upload.", string_4);
                                                smethod_8(apiFile_2.id);
                                                bool_3 = true;
                                            }));
                                            if (bool_3)
                                            {
                                                break;
                                            }
                                        }
                                        long size2 = 0L;
                                        long size3 = 0L;
                                        if (Tools.GetFileSize(string_6, out size2, out string_5) && (string.IsNullOrEmpty(text3) || Tools.GetFileSize(text3, out size3, out string_5)))
                                        {
                                            bool flag2 = ApiFileHelper.bool_0 && size3 > 0L && size3 < size2;
                                            if (ApiFileHelper.bool_0)
                                            {
                                                VRC.Core.Logger.Log("Delta size " + size3 + " (" + (float)size3 / (float)size2 + " %), full file size " + size2 + ", uploading " + (flag2 ? " DELTA" : " FULL FILE"), DebugLevel.All);
                                            }
                                            else
                                            {
                                                VRC.Core.Logger.Log("Delta compression disabled, uploading FULL FILE, size " + size2, DebugLevel.All);
                                            }
                                            smethod_3(apiFile_2, flag2);
                                            string text4 = "";
                                            if (flag2)
                                            {
                                                CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Generating file delta hash");
                                                bool_0 = true;
                                                string_5 = "";
                                                text4 = Convert.ToBase64String(MD5.Create().ComputeHash(File.ReadAllBytes(text3)));
                                                bool_0 = false;
                                                while (bool_0)
                                                {
                                                    if (!ApiFileHelper.Cancelled(Cancelled, Failure, apiFile_2))
                                                    {
                                                        yield return null;
                                                        continue;
                                                    }
                                                    smethod_8(apiFile_2.id);
                                                    yield break;
                                                }
                                                if (!string.IsNullOrEmpty(string_5))
                                                {
                                                    smethod_5(Failure, apiFile_2, "Failed to generate file delta hash.", string_5);
                                                    smethod_8(apiFile_2.id);
                                                    break;
                                                }
                                            }
                                            bool flag3 = false;
                                            smethod_3(apiFile_2, flag2);
                                            if (flag)
                                            {
                                                ApiFile.Version version = apiFile_2.GetVersion(apiFile_2.GetLatestVersionNumber());
                                                if (version == null || !(flag2 ? (size3 == version.delta.sizeInBytes && text4.CompareTo(version.delta.md5) == 0 && size == version.signature.sizeInBytes && text2.CompareTo(version.signature.md5) == 0) : (size2 == version.file.sizeInBytes && text.CompareTo(version.file.md5) == 0 && size == version.signature.sizeInBytes && text2.CompareTo(version.signature.md5) == 0)))
                                                {
                                                    CheckFile(filecheck, apiFile_2, "Preparing file for upload...", "Cleaning up previous version");
                                                    do
                                                    {
                                                        bool_0 = true;
                                                        string_5 = "";
                                                        bool_2 = false;
                                                        apiFile_2.DeleteLatestVersion(action, action2);
                                                        while (bool_0)
                                                        {
                                                            if (!ApiFileHelper.Cancelled(Cancelled, Failure, null))
                                                            {
                                                                yield return null;
                                                                continue;
                                                            }
                                                            yield break;
                                                        }
                                                        if (!string.IsNullOrEmpty(string_5))
                                                        {
                                                            smethod_5(Failure, apiFile_2, "Failed to delete previous incomplete version!", string_5);
                                                            if (!bool_2)
                                                            {
                                                                smethod_8(apiFile_2.id);
                                                                yield break;
                                                            }
                                                        }
                                                        yield return new WaitForSecondsRealtime(0.75f);
                                                    }
                                                    while (bool_2);
                                                }
                                                else
                                                {
                                                    flag3 = true;
                                                    PendulumLogger.Log("Using existing version record");
                                                }
                                            }
                                            smethod_3(apiFile_2, flag2);
                                            if (!flag3)
                                            {
                                                do
                                                {
                                                    CheckFile(filecheck, apiFile_2, "Creating file version record...");
                                                    bool_0 = true;
                                                    string_5 = "";
                                                    bool_2 = false;
                                                    if (!flag2)
                                                    {
                                                        apiFile_2.CreateNewVersion(ApiFile.Version.FileType.Full, text, size2, text2, size, action, action2);
                                                    }
                                                    else
                                                    {
                                                        apiFile_2.CreateNewVersion(ApiFile.Version.FileType.Delta, text4, size3, text2, size, action, action2);
                                                    }
                                                    while (bool_0)
                                                    {
                                                        if (ApiFileHelper.Cancelled(Cancelled, Failure, apiFile_2))
                                                        {
                                                            smethod_8(apiFile_2.id);
                                                            yield break;
                                                        }
                                                        yield return null;
                                                    }
                                                    if (!string.IsNullOrEmpty(string_5))
                                                    {
                                                        smethod_5(Failure, apiFile_2, "Failed to create file version record.", string_5);
                                                        if (!bool_2)
                                                        {
                                                            smethod_8(apiFile_2.id);
                                                            yield break;
                                                        }
                                                    }
                                                    yield return new WaitForSecondsRealtime(0.75f);
                                                }
                                                while (bool_2);
                                            }
                                            smethod_3(apiFile_2, flag2);
                                            if (!flag2)
                                            {
                                                if (apiFile_2.GetLatestVersion().file.status == ApiFile.Status.Waiting)
                                                {
                                                    CheckFile(filecheck, apiFile_2, "Uploading file...");
                                                    bool_3 = false;
                                                    yield return MelonCoroutines.Start(method_10(apiFile_2, ApiFile.Version.FileDescriptor.Type.file, string_6, text, size2, delegate (ApiFile apiFile_1)
                                                    {
                                                        VRC.Core.Logger.Log("Successfully uploaded file.", DebugLevel.All);
                                                        apiFile_2 = apiFile_1;
                                                    }, delegate (string string_4)
                                                    {
                                                        smethod_5(Failure, apiFile_2, "Failed to upload file.", string_4);
                                                        smethod_8(apiFile_2.id);
                                                        bool_3 = true;
                                                    }, delegate (long long_0, long long_1)
                                                    {
                                                        CheckFile(filecheck, apiFile_2, "Uploading file...", "", Tools.DivideSafe(long_0, long_1));
                                                    }, Cancelled));
                                                    if (bool_3)
                                                    {
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (apiFile_2.GetLatestVersion().delta.status == ApiFile.Status.Waiting)
                                            {
                                                CheckFile(filecheck, apiFile_2, "Uploading file delta...");
                                                bool_3 = false;
                                                yield return MelonCoroutines.Start(method_10(apiFile_2, ApiFile.Version.FileDescriptor.Type.delta, text3, text4, size3, delegate (ApiFile apiFile_1)
                                                {
                                                    PendulumLogger.Log("Successfully uploaded file delta.");
                                                    apiFile_2 = apiFile_1;
                                                }, delegate (string string_4)
                                                {
                                                    smethod_5(Failure, apiFile_2, "Failed to upload file delta.", string_4);
                                                    smethod_8(apiFile_2.id);
                                                    bool_3 = true;
                                                }, delegate (long long_0, long long_1)
                                                {
                                                    CheckFile(filecheck, apiFile_2, "Uploading file delta...", "", Tools.DivideSafe(long_0, long_1));
                                                }, Cancelled));
                                                if (bool_3)
                                                {
                                                    break;
                                                }
                                            }
                                            smethod_3(apiFile_2, flag2);
                                            if (apiFile_2.GetLatestVersion().signature.status == ApiFile.Status.Waiting)
                                            {
                                                CheckFile(filecheck, apiFile_2, "Uploading file signature...");
                                                bool_3 = false;
                                                yield return MelonCoroutines.Start(method_10(apiFile_2, ApiFile.Version.FileDescriptor.Type.signature, tempFileName, text2, size, delegate (ApiFile apiFile_1)
                                                {
                                                    VRC.Core.Logger.Log("Successfully uploaded file signature.", DebugLevel.All);
                                                    apiFile_2 = apiFile_1;
                                                }, delegate (string string_4)
                                                {
                                                    smethod_5(Failure, apiFile_2, "Failed to upload file signature.", string_4);
                                                    smethod_8(apiFile_2.id);
                                                    bool_3 = true;
                                                }, delegate (long long_0, long long_1)
                                                {
                                                    CheckFile(filecheck, apiFile_2, "Uploading file signature...", "", Tools.DivideSafe(long_0, long_1));
                                                }, Cancelled));
                                                if (bool_3)
                                                {
                                                    break;
                                                }
                                            }
                                            smethod_3(apiFile_2, flag2);
                                            CheckFile(filecheck, apiFile_2, "Validating upload...");
                                            if (!(flag2 ? (apiFile_2.GetFileDescriptor(apiFile_2.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.delta).status == ApiFile.Status.Complete) : (apiFile_2.GetFileDescriptor(apiFile_2.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.file).status == ApiFile.Status.Complete)) || apiFile_2.GetFileDescriptor(apiFile_2.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.signature).status != ApiFile.Status.Complete)
                                            {
                                                smethod_5(Failure, apiFile_2, "Failed to upload file.", "Record status is not 'complete' " + apiFile_2.GetFileDescriptor(apiFile_2.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.signature).status.ToString());
                                                smethod_8(apiFile_2.id);
                                                break;
                                            }
                                            if (!(flag2 ? (apiFile_2.GetFileDescriptor(apiFile_2.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.file).status != ApiFile.Status.Waiting) : (apiFile_2.GetFileDescriptor(apiFile_2.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.delta).status != ApiFile.Status.Waiting)))
                                            {
                                                smethod_5(Failure, apiFile_2, "Failed to upload file.", "Record is still in 'waiting' status");
                                                smethod_8(apiFile_2.id);
                                                break;
                                            }
                                            smethod_3(apiFile_2, flag2);
                                            CheckFile(filecheck, apiFile_2, "Processing upload...");
                                            float num = float_2;
                                            float b = float_3;
                                            float num2 = method_5(apiFile_2.GetLatestVersion().file.sizeInBytes);
                                            double num3 = Time.realtimeSinceStartup;
                                            double num4 = num3;
                                            while (apiFile_2.HasQueuedOperation(flag2))
                                            {
                                                CheckFile(filecheck, apiFile_2, "Processing upload...", "Checking status in " + Mathf.CeilToInt(num) + " seconds");
                                                while ((double)Time.realtimeSinceStartup - num4 < (double)num)
                                                {
                                                    if (ApiFileHelper.Cancelled(Cancelled, Failure, apiFile_2))
                                                    {
                                                        smethod_8(apiFile_2.id);
                                                        yield break;
                                                    }
                                                    if ((double)Time.realtimeSinceStartup - num3 > (double)num2)
                                                    {
                                                        smethod_3(apiFile_2, flag2);
                                                        smethod_5(Failure, apiFile_2, "Timed out waiting for upload processing to complete.");
                                                        smethod_8(apiFile_2.id);
                                                        yield break;
                                                    }
                                                    yield return null;
                                                }
                                                do
                                                {
                                                    CheckFile(filecheck, apiFile_2, "Processing upload...", "Checking status...");
                                                    bool_0 = true;
                                                    bool_2 = false;
                                                    string_5 = "";
                                                    API.Fetch<ApiFile>(apiFile_2.id, action, action2);
                                                    while (bool_0)
                                                    {
                                                        if (ApiFileHelper.Cancelled(Cancelled, Failure, apiFile_2))
                                                        {
                                                            smethod_8(apiFile_2.id);
                                                            yield break;
                                                        }
                                                        yield return null;
                                                    }
                                                    if (!string.IsNullOrEmpty(string_5))
                                                    {
                                                        smethod_5(Failure, apiFile_2, "Checking upload status failed.", string_5);
                                                        if (!bool_2)
                                                        {
                                                            smethod_8(apiFile_2.id);
                                                            yield break;
                                                        }
                                                    }
                                                }
                                                while (bool_2);
                                                num = Mathf.Min(num * 2f, b);
                                                num4 = Time.realtimeSinceStartup;
                                            }
                                            yield return MelonCoroutines.Start(method_4(apiFile_2.id));
                                            smethod_4(success, apiFile_2, "Upload complete!");
                                        }
                                        else
                                        {
                                            smethod_5(Failure, apiFile_2, "Failed to create file delta for upload.", "Couldn't get file size: " + string_5);
                                            smethod_8(apiFile_2.id);
                                        }
                                    }
                                    else
                                    {
                                        smethod_5(Failure, apiFile_2, "Failed to generate file signature!", "Couldn't get file size:\n" + string_5);
                                        smethod_8(apiFile_2.id);
                                    }
                                }
                                else
                                {
                                    smethod_5(Failure, apiFile_2, "Failed to generate MD5 hash for signature file.", string_5);
                                    smethod_8(apiFile_2.id);
                                }
                            }
                            else
                            {
                                smethod_5(Failure, apiFile_2, "Failed to generate file signature!", "Failed to create temp file: \n" + string_5);
                                smethod_8(apiFile_2.id);
                            }
                        }
                        else
                        {
                            smethod_5(Failure, apiFile_2, "Failed to optimize file for upload.", "Failed to create temp file: \n" + string_5);
                        }
                    }
                    else
                    {
                        smethod_5(Failure, apiFile_2, "A previous upload is still being processed. Please try again later.");
                    }
                    break;
                }
            }
            else
            {
                smethod_5(Failure, null, "Could not read file to upload!", Path + "\n" + whyNot);
            }
        }

        private static void smethod_3(ApiFile apiFile_0, bool bool_1, bool bool_2 = false)
        {
            if (apiFile_0 != null && apiFile_0.IsInitialized)
            {
                if (!apiFile_0.IsInErrorState() && bool_2)
                {
                    VRC.Core.Logger.Log("< color = yellow > Processing { 3}: { 0}, { 1}, { 2}</ color > " + (apiFile_0.IsWaitingForUpload() ? "waiting for upload" : "upload complete") + (apiFile_0.HasExistingOrPendingVersion() ? "has existing or pending version" : "no previous version") + (apiFile_0.IsLatestVersionQueued(bool_1) ? "latest version queued" : "latest version not queued") + apiFile_0.name, DebugLevel.All);
                }
            }
            else
            {
                Debug.LogFormat("<color=yellow>apiFile not initialized</color>", null);
            }
            if ((apiFile_0?.IsInitialized ?? false) && bool_2)
            {
                Il2CppSystem.Collections.Generic.Dictionary<string, Json.Token> dictionary = apiFile_0.ExtractApiFields();
                if (dictionary != null)
                {
                    VRC.Core.Logger.Log("<color=yellow>{0}</color>" + Tools.JsonEncode(dictionary), DebugLevel.All);
                }
            }
        }

        public IEnumerator method_1(string string_0, string string_1, System.Action<GEnum0> action_0, System.Action<string> action_1)
        {
            VRC.Core.Logger.Log("CreateOptimizedFile: " + string_0 + " => " + string_1, DebugLevel.All);
            if (!smethod_2(string_0))
            {
                VRC.Core.Logger.Log("CreateOptimizedFile: (not gzip compressed, done)", DebugLevel.All);
                action_0?.Invoke(GEnum0.Unchanged);
                yield break;
            }
            bool flag = string.Compare(Path.GetExtension(string_0), ".unitypackage", ignoreCase: true) == 0;
            yield return null;
            Stream stream = null;
            try
            {
                //stream = new GZipStream(string_0, 262144);
            }
            catch (Exception ex)
            {
                action_1?.Invoke("Couldn't read file: " + string_0 + "\n" + ex.Message);
                yield break;
            }
            yield return null;
            GZipStream gZipStream = null;
            try
            {
                //gZipStream = new GZipStream(string_1, CompressLevel.Best, rsyncable: true, 262144);
            }
            catch (System.Exception ex2)
            {
                //stream?.Close();
                action_1?.Invoke("Couldn't create output file: " + string_1 + "\n" + ex2.Message);
                yield break;
            }
            yield return null;
            if (flag)
            {
                try
                {
                    System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
                    byte[] array = new byte[4096];
                    TarInputStream tarInputStream = new TarInputStream(stream);
                    for (TarEntry nextEntry = tarInputStream.GetNextEntry(); nextEntry != null; nextEntry = tarInputStream.GetNextEntry())
                    {
                        if (nextEntry.Size > 0L && nextEntry.Name.EndsWith("/pathname", System.StringComparison.OrdinalIgnoreCase))
                        {
                            int num = tarInputStream.Read(array, 0, (int)nextEntry.Size);
                            if (num > 0)
                            {
                                string string_3 = Encoding.ASCII.GetString(array, 0, num);
                                if (regex_0.Any((Regex regex_0) => regex_0.IsMatch(string_3)))
                                {
                                    string item = string_3.Substring(0, string_3.IndexOf('/'));
                                    list.Add(item);
                                }
                            }
                        }
                    }
                    tarInputStream.Close();
                    stream.Close();
                    //stream = new GZipStream(string_0, 262144);
                    TarOutputStream tarOutputStream = new TarOutputStream(gZipStream);
                    TarInputStream tarInputStream2 = new TarInputStream(stream);
                    for (TarEntry nextEntry2 = tarInputStream2.GetNextEntry(); nextEntry2 != null; nextEntry2 = tarInputStream2.GetNextEntry())
                    {
                        string string_2 = nextEntry2.Name.Substring(0, nextEntry2.Name.IndexOf('/'));
                        /*if (!list.Any((string string_1) => string.Compare(string_1, string_2) == 0))
                        {
                            tarOutputStream.PutNextEntry(nextEntry2);
                            tarInputStream2.CopyEntryContents(tarOutputStream);
                            tarOutputStream.CloseEntry();
                        }*/
                    }
                    tarInputStream2.Close();
                    tarOutputStream.Close();
                }
                catch (Exception ex3)
                {
                    stream?.Close();
                    gZipStream?.Close();
                    action_1?.Invoke("Failed to strip and recompress file.\n" + ex3.Message);
                    yield break;
                }
            }
            else
            {
                try
                {
                    byte[] buffer = new byte[262144];
                    StreamUtils.Copy(stream, gZipStream, buffer);
                }
                catch (Exception ex4)
                {
                    stream?.Close();
                    gZipStream?.Close();
                    action_1?.Invoke("Failed to recompress file.\n" + ex4.Message);
                    yield break;
                }
            }
            yield return null;
            stream?.Close();
            gZipStream?.Close();
            yield return null;
            action_0?.Invoke(GEnum0.Success);
        }

        public IEnumerator method_2(string string_0, string string_1, Action action_0, Action<string> action_1)
        {
            VRC.Core.Logger.Log("CreateFileSignature: " + string_0 + " => " + string_1, DebugLevel.All);
            yield return null;
            byte[] array = new byte[65536];
            Stream stream;
            try
            {
                stream = Librsync.ComputeSignature(File.OpenRead(string_0));
            }
            catch (Exception ex)
            {
                action_1?.Invoke("Couldn't open input file: " + ex.Message);
                yield break;
            }
            FileStream fileStream;
            try
            {
                fileStream = File.Open(string_1, FileMode.Create, FileAccess.Write);
            }
            catch (Exception ex2)
            {
                action_1?.Invoke("Couldn't create output file: " + ex2.Message);
                yield break;
            }
            while (true)
            {
                IAsyncResult asyncResult;
                try
                {
                    asyncResult = stream.BeginRead(array, 0, array.Length, null, null);
                }
                catch (Exception ex3)
                {
                    action_1?.Invoke("Couldn't read file: " + ex3.Message);
                    yield break;
                }
                while (!asyncResult.IsCompleted)
                {
                    yield return null;
                }
                int num;
                try
                {
                    num = stream.EndRead(asyncResult);
                }
                catch (Exception ex4)
                {
                    action_1?.Invoke("Couldn't read file: " + ex4.Message);
                    yield break;
                }
                if (num <= 0)
                {
                    break;
                }
                IAsyncResult asyncResult2;
                try
                {
                    asyncResult2 = fileStream.BeginWrite(array, 0, num, null, null);
                }
                catch (Exception ex5)
                {
                    action_1?.Invoke("Couldn't write file: " + ex5.Message);
                    yield break;
                }
                while (!asyncResult2.IsCompleted)
                {
                    yield return null;
                }
                try
                {
                    fileStream.EndWrite(asyncResult2);
                }
                catch (Exception ex6)
                {
                    action_1?.Invoke("Couldn't write file: " + ex6.Message);
                    yield break;
                }
            }
            stream.Close();
            fileStream.Close();
            yield return null;
            action_0?.Invoke();
        }

        public IEnumerator method_3(string string_0, string string_1, string string_2, System.Action action_0, System.Action<string> action_1)
        {
            PendulumLogger.Log("CreateFileDelta: " + string_0 + " (delta) " + string_1 + " => " + string_2);
            yield return null;
            byte[] array = new byte[65536];
            Stream stream;
            try
            {
                stream = Librsync.ComputeDelta(File.OpenRead(string_1), File.OpenRead(string_0));
            }
            catch (Exception ex)
            {
                action_1?.Invoke("Couldn't open input file: " + ex.Message);
                yield break;
            }
            FileStream fileStream;
            try
            {
                fileStream = File.Open(string_2, FileMode.Create, FileAccess.Write);
            }
            catch (Exception ex2)
            {
                action_1?.Invoke("Couldn't create output file: " + ex2.Message);
                yield break;
            }
            while (true)
            {
                IAsyncResult asyncResult;
                try
                {
                    asyncResult = stream.BeginRead(array, 0, array.Length, null, null);
                }
                catch (Exception ex3)
                {
                    action_1?.Invoke("Couldn't read file: " + ex3.Message);
                    yield break;
                }
                while (!asyncResult.IsCompleted)
                {
                    yield return null;
                }
                int num;
                try
                {
                    num = stream.EndRead(asyncResult);
                }
                catch (Exception ex4)
                {
                    action_1?.Invoke("Couldn't read file: " + ex4.Message);
                    yield break;
                }
                if (num <= 0)
                {
                    break;
                }
                IAsyncResult asyncResult2;
                try
                {
                    asyncResult2 = fileStream.BeginWrite(array, 0, num, null, null);
                }
                catch (Exception ex5)
                {
                    action_1?.Invoke("Couldn't write file: " + ex5.Message);
                    yield break;
                }
                while (!asyncResult2.IsCompleted)
                {
                    yield return null;
                }
                try
                {
                    fileStream.EndWrite(asyncResult2);
                }
                catch (Exception ex6)
                {
                    action_1?.Invoke("Couldn't write file: " + ex6.Message);
                    yield break;
                }
            }
            stream.Close();
            fileStream.Close();
            yield return null;
            action_0?.Invoke();
        }

        protected static void smethod_4(GDelegate0 gdelegate0_0, ApiFile apiFile_0, string string_0)
        {
            if (apiFile_0 == null)
            {
                apiFile_0 = new ApiFile();
            }
            VRC.Core.Logger.Log("ApiFile " + apiFile_0.ToStringBrief() + ": Operation Succeeded!", DebugLevel.All);
            gdelegate0_0?.Invoke(apiFile_0, string_0);
        }

        protected static void smethod_5(GDelegate1 gdelegate1_0, ApiFile apiFile_0, string string_0, string string_1 = "")
        {
            if (apiFile_0 == null)
            {
                apiFile_0 = new ApiFile();
            }
            PendulumLogger.Log("ApiFile " + apiFile_0.ToStringBrief() + ": Error: " + string_0 + "\n" + string_1);
            gdelegate1_0?.Invoke(apiFile_0, string_0);
        }

        protected static void CheckFile(GDelegate2 gdelegate2_0, ApiFile apiFile_0, string string_0, string string_1 = "", float float_5 = 0f)
        {
            if (apiFile_0 == null)
            {
                apiFile_0 = new ApiFile();
            }
            gdelegate2_0?.Invoke(apiFile_0, string_0, string_1, float_5);
        }

        protected static bool Cancelled(GDelegate3 gdelegate3_0, GDelegate1 gdelegate1_0, ApiFile apiFile_0)
        {
            if (apiFile_0 == null)
            {
                PendulumLogger.Log("apiFile was null");
                return true;
            }
            if (gdelegate3_0 != null && gdelegate3_0(apiFile_0))
            {
                PendulumLogger.Log("ApiFile " + apiFile_0.ToStringBrief() + ": Operation cancelled");
                gdelegate1_0?.Invoke(apiFile_0, "Cancelled by user.");
                return true;
            }
            return false;
        }

        protected static void smethod_8(string string_0)
        {
            MelonCoroutines.Start(apifilehelper.method_4(string_0));
        }

        protected IEnumerator method_4(string string_0)
        {
            if (string.IsNullOrEmpty(string_0))
            {
                yield break;
            }
            string tempFolderPath = Tools.GetTempFolderPath(string_0);
            while (Directory.Exists(tempFolderPath))
            {
                try
                {
                    if (Directory.Exists(tempFolderPath))
                    {
                        Directory.Delete(tempFolderPath, recursive: true);
                    }
                }
                catch { }
                yield return null;
            }
        }

        private static void smethod_9()
        {
            if (helper == null)
            {
                GameObject obj = new GameObject("ApiFileHelper")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                helper = obj.AddComponent<ApiFileHelper>();
                remoteConfig_0 = new RemoteConfig();
                DontDestroyOnLoad(obj);
            }
        }

        private float method_5(int int_2)
        {
            return Mathf.Clamp(Mathf.Ceil(int_2 / (float)int_1) * float_0, float_0, float_1);
        }

        private bool method_6(ApiFile apiFile_0, string string_0, string string_1, long long_0, ApiFile.Version.FileDescriptor fileDescriptor_0, System.Action<ApiFile> action_0, System.Action<string> action_1)
        {
            if (fileDescriptor_0.status != ApiFile.Status.Waiting)
            {
                PendulumLogger.Log("UploadFileComponent: (file record not in waiting status, done)");
                action_0?.Invoke(apiFile_0);
                return false;
            }
            if (long_0 == fileDescriptor_0.sizeInBytes)
            {
                if (string.Compare(string_1, fileDescriptor_0.md5) == 0)
                {
                    long size = 0L;
                    string errorStr = "";
                    if (!Tools.GetFileSize(string_0, out size, out errorStr))
                    {
                        action_1?.Invoke("Couldn't get file size");
                        return false;
                    }
                    if (size != long_0)
                    {
                        action_1?.Invoke("File size does not match input size");
                        return false;
                    }
                    return true;
                }
                action_1?.Invoke("File MD5 does not match version descriptor");
                return false;
            }
            action_1?.Invoke("File size does not match version descriptor");
            return false;
        }

        private IEnumerator method_7(ApiFile apiFile_0, ApiFile.Version.FileDescriptor.Type type_0, string string_0, string string_1, long long_0, System.Action<ApiFile> action_0, System.Action<string> action_1, System.Action<long, long> action_2, GDelegate3 gdelegate3_0)
        {
            /*GDelegate1 gdelegate1_ = delegate (ApiFile apiFile_1, string string_1)
            {
                action_1?.Invoke(string_1);
            };*/
            string string_2 = "";
            while (true)
            {
                bool bool_0 = true;
                string string_3 = "";
                bool bool_3 = false;
                apiFile_0.StartSimpleUpload(type_0, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    //string_2 = IL2CPP.Il2CppStringToManaged(apiContainer_0.Cast<ApiDictContainer>().ResponseDictionary["url"].Pointer);
                    bool_0 = false;
                }, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    string_3 = "Failed to start upload: " + apiContainer_0.Error;
                    bool_0 = false;
                    if (apiContainer_0.Code == 400)
                    {
                        bool_3 = true;
                    }
                });
                while (bool_0)
                {
                    /*if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                    {
                        yield return null;
                        continue;
                    }*/
                    yield break;
                }
                if (!string.IsNullOrEmpty(string_3))
                {
                    action_1?.Invoke(string_3);
                    if (!bool_3)
                    {
                        yield break;
                    }
                }
                yield return new WaitForSecondsRealtime(0.75f);
                if (!bool_3)
                {
                    break;
                }
            }
            bool bool_ = true;
            string string_4 = "";
            /*HttpRequest httpRequest = ApiFile.PutSimpleFileToURL(string_2, string_0, smethod_1(Path.GetExtension(string_0)), string_1, (Action)delegate
            {
                bool_ = false;
            }, (Action<string>)delegate (string string_1)
            {
                string_4 = "Failed to upload file: " + string_1;
                bool_ = false;
            }, (Action<long, long>)delegate (long long_0, long long_1)
            {
                action_2?.Invoke(long_0, long_1);
            });*/
            while (true)
            {
                if (!bool_)
                {
                    if (!string.IsNullOrEmpty(string_4))
                    {
                        action_1?.Invoke(string_4);
                        yield break;
                    }
                    break;
                }
                /*if (Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                {
                    httpRequest?.Abort();
                    yield break;
                }*/
                yield return null;
            }
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.75f);
                bool bool_2 = true;
                string string_5 = "";
                bool bool_4 = false;
                apiFile_0.FinishUpload(type_0, null, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    PendulumLogger.Log("!!!!YOU CAN IGNORE THIS CASTING ERRORS!!!!");
                    apiFile_0 = apiContainer_0.Model.Cast<ApiFile>();
                    bool_2 = false;
                }, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    string_5 = "Failed to finish upload: " + apiContainer_0.Error;
                    bool_2 = false;
                    if (apiContainer_0.Code == 400)
                    {
                        bool_4 = false;
                    }
                });
                while (bool_2)
                {
                    /*if (Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                    {
                        yield break;
                    }*/
                    yield return null;
                }
                if (!string.IsNullOrEmpty(string_5))
                {
                    action_1?.Invoke(string_5);
                    if (!bool_4)
                    {
                        break;
                    }
                }
                yield return new WaitForSecondsRealtime(0.75f);
                if (!bool_4)
                {
                    break;
                }
            }
        }

        private IEnumerator method_8(ApiFile apiFile_0, ApiFile.Version.FileDescriptor.Type type_0, string string_0, string string_1, long long_0, System.Action<ApiFile> action_0, System.Action<string> action_1, System.Action<long, long> action_2, GDelegate3 gdelegate3_0)
        {
            FileStream fileStream_0 = null;
            /*GDelegate1 gdelegate1_ = delegate (ApiFile apiFile_1, string string_0)
            {
                if (fileStream_0 != null)
                {
                    fileStream_0.Close();
                }
                action_1?.Invoke(string_0);
            };*/
            ApiFile.UploadStatus uploadStatus_0 = null;
            byte[] array;
            long long_4;
            System.Collections.Generic.List<string> list_0;
            int num;
            int i;
            while (true)
            {
                bool bool_3 = true;
                string string_6 = "";
                bool bool_5 = false;
                apiFile_0.GetUploadStatus(apiFile_0.GetLatestVersionNumber(), type_0, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    uploadStatus_0 = apiContainer_0.Model.Cast<ApiFile.UploadStatus>();
                    bool_3 = false;
                    VRC.Core.Logger.Log("Found existing multipart upload status (next part = " + uploadStatus_0.nextPartNumber + ")", DebugLevel.All);
                }, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    string_6 = "Failed to query multipart upload status: " + apiContainer_0.Error;
                    bool_3 = false;
                    if (apiContainer_0.Code == 400)
                    {
                        bool_5 = true;
                    }
                });
                while (bool_3)
                {
                    /*if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                    {
                        yield return null;
                        continue;
                    }*/
                    yield break;
                }
                if (!string.IsNullOrEmpty(string_6))
                {
                    action_1?.Invoke(string_6);
                    if (!bool_5)
                    {
                        yield break;
                    }
                }
                if (bool_5)
                {
                    continue;
                }
                try
                {
                    fileStream_0 = File.OpenRead(string_0);
                }
                catch (System.Exception ex)
                {
                    action_1?.Invoke("Couldn't open file: " + ex.Message);
                    yield break;
                }
                array = new byte[this.int_0 * 2];
                long_4 = 0L;
                list_0 = new System.Collections.Generic.List<string>();
                if (uploadStatus_0 != null)
                {
                    list_0 = uploadStatus_0.etags.ToArray().ToList();
                }
                num = Mathf.Max(1, Mathf.FloorToInt((float)fileStream_0.Length / (float)this.int_0));
                i = 1;
                break;
            }
            for (; i <= num; i++)
            {
                int num2 = (int)((i < num) ? this.int_0 : (fileStream_0.Length - fileStream_0.Position));
                int int_0 = 0;
                try
                {
                    int_0 = fileStream_0.Read(array, 0, num2);
                }
                catch (System.Exception ex2)
                {
                    fileStream_0.Close();
                    action_1?.Invoke("Couldn't read file: " + ex2.Message);
                    yield break;
                }
                if (int_0 != num2)
                {
                    fileStream_0.Close();
                    action_1?.Invoke("Couldn't read file: read incorrect number of bytes from stream");
                    yield break;
                }
                if (uploadStatus_0 == null || !(i <= uploadStatus_0.nextPartNumber))
                {
                    string string_5 = "";
                    bool flag;
                    do
                    {
                        bool bool_2 = true;
                        string string_4 = "";
                        flag = false;
                        apiFile_0.StartMultipartUpload(type_0, i, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                        {
                            //string_5 = IL2CPP.Il2CppStringToManaged(apiContainer_0.Cast<ApiDictContainer>().ResponseDictionary["url"].Pointer);
                            bool_2 = false;
                        }, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                        {
                            string_4 = "Failed to start part upload: " + apiContainer_0.Error;
                            bool_2 = false;
                        });
                        while (bool_2)
                        {
                            /*if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                            {
                                yield return null;
                                continue;
                            }*/
                            yield break;
                        }
                        if (!string.IsNullOrEmpty(string_4))
                        {
                            fileStream_0.Close();
                            action_1?.Invoke(string_4);
                            if (!flag)
                            {
                                yield break;
                            }
                        }
                        yield return new WaitForSecondsRealtime(0.75f);
                    }
                    while (flag);
                    bool bool_ = true;
                    string string_3 = "";
                    /*HttpRequest httpRequest = ApiFile.PutMultipartDataToURL(string_5, array, int_0, smethod_1(Path.GetExtension(string_0)), (System.Action<string>)delegate (string string_1)
                    {
                        if (!string.IsNullOrEmpty(string_1))
                        {
                            list_0.Add(string_1);
                        }
                        long_4 += int_0;
                        bool_ = false;
                    }, (System.Action<string>)delegate (string string_1)
                    {
                        string_3 = "Failed to upload data: " + string_1;
                        bool_ = false;
                    }, (System.Action<long, long>)delegate (long long_2, long long_3)
                    {
                        action_2?.Invoke(long_4 + long_2, long_0);
                    });*/
                    while (bool_)
                    {
                        /*if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                        {
                            yield return null;
                            continue;
                        }
                        httpRequest?.Abort();*/
                        yield break;
                    }
                    if (!string.IsNullOrEmpty(string_3))
                    {
                        fileStream_0.Close();
                        action_1?.Invoke(string_3);
                        yield break;
                    }
                }
                else
                {
                    long_4 += int_0;
                }
            }
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.75f);
                bool bool_0 = true;
                string string_2 = "";
                bool bool_4 = false;
                Il2CppSystem.Collections.Generic.List<string> list = new Il2CppSystem.Collections.Generic.List<string>();
                foreach (string item in list_0)
                {
                    list.Add(item);
                }
                apiFile_0.FinishUpload(type_0, list, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    PendulumLogger.Log("!!!!YOU CAN IGNORE THIS CASTING ERRORS!!!!");
                    apiFile_0 = apiContainer_0.Model.Cast<ApiFile>();
                    bool_0 = false;
                }, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    string_2 = "Failed to finish upload: " + apiContainer_0.Error;
                    bool_0 = false;
                    if (apiContainer_0.Code == 400)
                    {
                        bool_4 = true;
                    }
                });
                while (bool_0)
                {
                    /*if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                    {
                        yield return null;
                        continue;
                    }*/
                    yield break;
                }
                if (!string.IsNullOrEmpty(string_2))
                {
                    fileStream_0.Close();
                    action_1?.Invoke(string_2);
                    if (!bool_4)
                    {
                        yield break;
                    }
                }
                yield return new WaitForSecondsRealtime(0.75f);
                if (!bool_4)
                {
                    break;
                }
            }
            fileStream_0.Close();
        }

        private IEnumerator method_9(ApiFile apiFile_0, ApiFile.Version.FileDescriptor.Type type_0, string string_0, string string_1, long long_0, ApiFile.Version.FileDescriptor fileDescriptor_0, System.Action<ApiFile> action_0, System.Action<string> action_1, System.Action<long, long> action_2, GDelegate3 gdelegate3_0)
        {
            /*GDelegate1 gdelegate1_ = delegate (ApiFile apiFile_0, string string_0)
            {
                action_1?.Invoke(string_0);
            };*/
            float realtimeSinceStartup = Time.realtimeSinceStartup;
            float num = realtimeSinceStartup;
            float num2 = method_5(fileDescriptor_0.sizeInBytes);
            float num3 = float_2;
            float b = float_3;
            while (apiFile_0 != null)
            {
                ApiFile.Version.FileDescriptor fileDescriptor = apiFile_0.GetFileDescriptor(apiFile_0.GetLatestVersionNumber(), type_0);
                if (fileDescriptor != null)
                {
                    if (fileDescriptor.status == ApiFile.Status.Waiting)
                    {
                        while (Time.realtimeSinceStartup - num < num3)
                        {
                            /*if (Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                            {
                                yield break;
                            }*/
                            if (Time.realtimeSinceStartup - realtimeSinceStartup <= num2)
                            {
                                yield return null;
                                continue;
                            }
                            action_1?.Invoke("Couldn't verify upload status: Timed out wait for server processing");
                            yield break;
                        }
                        while (true)
                        {
                            bool bool_0 = true;
                            string string_2 = "";
                            bool bool_1 = false;
                            apiFile_0.Refresh((Action<ApiContainer>)delegate
                            {
                                bool_0 = false;
                            }, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                            {
                                string_2 = "Couldn't verify upload status: " + apiContainer_0.Error;
                                bool_0 = false;
                                if (apiContainer_0.Code == 400)
                                {
                                    bool_1 = true;
                                }
                            });
                            while (bool_0)
                            {
                                /*if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                                {
                                    yield return null;
                                    continue;
                                }*/
                                yield break;
                            }
                            if (!string.IsNullOrEmpty(string_2))
                            {
                                action_1?.Invoke(string_2);
                                if (!bool_1)
                                {
                                    yield break;
                                }
                            }
                            if (!bool_1)
                            {
                                break;
                            }
                        }
                        num3 = Mathf.Min(num3 * 2f, b);
                        num = Time.realtimeSinceStartup;
                        continue;
                    }
                    action_0?.Invoke(apiFile_0);
                    yield break;
                }
                action_1?.Invoke("File descriptor is null ('" + type_0.ToString() + "')");
                yield break;
            }
            action_1?.Invoke("ApiFile is null");
        }

        private IEnumerator method_10(ApiFile apiFile_0, ApiFile.Version.FileDescriptor.Type type_0, string string_0, string string_1, long long_0, System.Action<ApiFile> action_0, System.Action<string> action_1, System.Action<long, long> action_2, GDelegate3 gdelegate3_0)
        {
            VRC.Core.Logger.Log("UploadFileComponent: " + type_0.ToString() + " (" + apiFile_0.id + "): " + string_0, DebugLevel.All);
            ApiFile.Version.FileDescriptor fileDescriptor = apiFile_0.GetFileDescriptor(apiFile_0.GetLatestVersionNumber(), type_0);
            if (method_6(apiFile_0, string_0, string_1, long_0, fileDescriptor, action_0, action_1))
            {
                switch (fileDescriptor.category)
                {
                    case ApiFile.Category.Simple:
                        yield return method_7(apiFile_0, type_0, string_0, string_1, long_0, action_0, action_1, action_2, gdelegate3_0);
                        break;

                    case ApiFile.Category.Multipart:
                        yield return method_8(apiFile_0, type_0, string_0, string_1, long_0, action_0, action_1, action_2, gdelegate3_0);
                        break;

                    default:
                        action_1?.Invoke("Unknown file category type: " + fileDescriptor.category);
                        yield break;
                }
                yield return method_9(apiFile_0, type_0, string_0, string_1, long_0, fileDescriptor, action_0, action_1, action_2, gdelegate3_0);
            }
        }
    }
    class ApiFileHelperOld : MonoBehaviour
    {
        private readonly int kMultipartUploadChunkSize = 100 * 1024 * 1024; // 100 MB
        private readonly int SERVER_PROCESSING_WAIT_TIMEOUT_CHUNK_SIZE = 50 * 1024 * 1024;
        private readonly float SERVER_PROCESSING_WAIT_TIMEOUT_PER_CHUNK_SIZE = 120.0f;
        private readonly float SERVER_PROCESSING_MAX_WAIT_TIMEOUT = 600.0f;
        private readonly float SERVER_PROCESSING_INITIAL_RETRY_TIME = 2.0f;
        private readonly float SERVER_PROCESSING_MAX_RETRY_TIME = 10.0f;

        private static bool EnableDeltaCompression = false;

        private readonly Regex[] kUnityPackageAssetNameFilters = new Regex[]
        {
            new Regex(@"/LightingData\.asset$"),                    // lightmap base asset
            new Regex(@"/Lightmap-.*(\.png|\.exr)$"),               // lightmaps
            new Regex(@"/ReflectionProbe-.*(\.exr|\.png)$"),        // reflection probes
            new Regex(@"/Editor/Data/UnityExtensions/")             // anything that looks like part of the Unity installation
        };

        public delegate void OnFileOpSuccess(ApiFile apiFile, string message);
        public delegate void OnFileOpError(ApiFile apiFile, string error);
        public delegate void OnFileOpProgress(ApiFile apiFile, string status, string subStatus, float pct);
        public delegate bool FileOpCancelQuery(ApiFile apiFile);

        public static ApiFileHelperOld Instance
        {
            get
            {
                CheckInstance();
                return mInstance;
            }
        }

        private static ApiFileHelperOld mInstance = null;
        const float kPostWriteDelay = 0.75f;

        public bool DebugEnabled
        {
            get { return true; }
        }

        public enum FileOpResult
        {
            Success,
            Unchanged
        }

        public static void UploadFileAsync(string filename, string existingFileId, string friendlyName,
            OnFileOpSuccess onSuccess, OnFileOpError onError, OnFileOpProgress onProgress, FileOpCancelQuery cancelQuery)
        {
            MelonCoroutines.Start(Instance.UploadFile(filename, existingFileId, friendlyName, onSuccess, onError,
                onProgress, cancelQuery));
        }

        public static string GetMimeTypeFromExtension(string extension)
        {
            if (extension == ".vrcw")
                return "application/x-world";
            if (extension == ".vrca")
                return "application/x-avatar";
            if (extension == ".dll")
                return "application/x-msdownload";
            if (extension == ".unitypackage")
                return "application/gzip";
            if (extension == ".gz")
                return "application/gzip";
            if (extension == ".jpg")
                return "image/jpg";
            if (extension == ".png")
                return "image/png";
            if (extension == ".sig")
                return "application/x-rsync-signature";
            if (extension == ".delta")
                return "application/x-rsync-delta";

            Debug.LogWarning("Unknown file extension for mime-type: " + extension);
            return "application/octet-stream";
        }

        public static bool IsGZipCompressed(string filename)
        {
            return GetMimeTypeFromExtension(Path.GetExtension(filename)) == "application/gzip";
        }

        public IEnumerator UploadFile(string filename, string existingFileId, string friendlyName,
            OnFileOpSuccess onSuccess, OnFileOpError onError, OnFileOpProgress onProgress, FileOpCancelQuery cancelQuery)
        {
            Debug.Log("UploadFile: filename: " + filename + ", file id: " +
                      (!string.IsNullOrEmpty(existingFileId) ? existingFileId : "<new>") + ", name: " + friendlyName);

            // init remote config 
            if (false)
            {
                bool done = false;

                System.Action remoteconfigaction = new Action(() =>
                {
                    done = true;
                });

                /*RemoteConfig.Init(
                    remoteconfigaction,
                    remoteconfigaction
                );

                while (!done)
                    yield return null;

                if (!RemoteConfig.IsInitialized())
                {
                    Error(onError, null, "Failed to fetch configuration.");
                    yield break;
                }*/
            }

            // configure delta compression
            {
                //EnableDeltaCompression = RemoteConfig.GetBool("sdkEnableDeltaCompression", false);
            }

            // validate input file
            Progress(onProgress, null, "Checking file...");

            if (string.IsNullOrEmpty(filename))
            {
                Error(onError, null, "Upload filename is empty!");
                yield break;
            }

            if (!System.IO.Path.HasExtension(filename))
            {
                Error(onError, null, "Upload filename must have an extension: " + filename);
                yield break;
            }

            string whyNot;
            if (!VRC.Tools.FileCanRead(filename, out whyNot))
            {
                Error(onError, null, "Could not read file to upload!", filename + "\n" + whyNot);
                yield break;
            }

            // get or create ApiFile
            Progress(onProgress, null, string.IsNullOrEmpty(existingFileId) ? "Creating file record..." : "Getting file record...");

            bool wait = true;
            bool wasError = false;
            bool worthRetry = false;
            string errorStr = "";

            if (string.IsNullOrEmpty(friendlyName))
                friendlyName = filename;

            string extension = System.IO.Path.GetExtension(filename);
            string mimeType = GetMimeTypeFromExtension(extension);

            ApiFile apiFile = null;

            System.Action<ApiContainer> fileSuccess = (ApiContainer c) =>
            {
                apiFile = c.Model as ApiFile;
                wait = false;
            };

            System.Action<ApiContainer> fileFailure = (ApiContainer c) =>
            {
                errorStr = c.Error;
                wait = false;

                if (c.Code == 400)
                    worthRetry = true;
            };

            while (true)
            {
                apiFile = null;
                wait = true;
                worthRetry = false;
                errorStr = "";

                if (string.IsNullOrEmpty(existingFileId))
                    ApiFile.Create(friendlyName, mimeType, extension, fileSuccess, fileFailure);
                else
                    API.Fetch<ApiFile>(existingFileId, fileSuccess, fileFailure);

                while (wait)
                {
                    if (apiFile != null && CheckCancelled(cancelQuery, onError, apiFile))
                        yield break;

                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    if (errorStr.Contains("File not found"))
                    {
                        Debug.LogError("Couldn't find file record: " + existingFileId + ", creating new file record");

                        existingFileId = "";
                        continue;
                    }

                    string msg = string.IsNullOrEmpty(existingFileId) ? "Failed to create file record." : "Failed to get file record.";
                    Error(onError, null, msg, errorStr);

                    if (!worthRetry)
                        yield break;
                }

                if (!worthRetry)
                    break;
                else
                    yield return new WaitForSecondsRealtime(kPostWriteDelay);
            }

            if (apiFile == null)
                yield break;

            LogApiFileStatus(apiFile, false);

            while (apiFile.HasQueuedOperation())
            {
                wait = true;

                apiFile.DeleteLatestVersion(fileSuccess, fileFailure);

                while (wait)
                {
                    if (apiFile != null && CheckCancelled(cancelQuery, onError, apiFile))
                        yield break;

                    yield return null;
                }
            }

            // delay to let write get through servers
            yield return new WaitForSecondsRealtime(kPostWriteDelay);

            LogApiFileStatus(apiFile, false);

            // check for server side errors from last upload
            if (apiFile.IsInErrorState())
            {
                Debug.LogWarning("ApiFile: " + apiFile.id + ": server failed to process last uploaded, deleting failed version");

                while (true)
                {
                    // delete previous failed version
                    Progress(onProgress, apiFile, "Preparing file for upload...", "Cleaning up previous version");

                    wait = true;
                    errorStr = "";
                    worthRetry = false;

                    apiFile.DeleteLatestVersion(fileSuccess, fileFailure);

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onError, null))
                        {
                            yield break;
                        }

                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        Error(onError, apiFile, "Failed to delete previous failed version!", errorStr);
                        if (!worthRetry)
                        {
                            CleanupTempFiles(apiFile.id);
                            yield break;
                        }
                    }

                    if (worthRetry)
                        yield return new WaitForSecondsRealtime(kPostWriteDelay);
                    else
                        break;
                }
            }

            // delay to let write get through servers
            yield return new WaitForSecondsRealtime(kPostWriteDelay);

            LogApiFileStatus(apiFile, false);

            // verify previous file op is complete
            if (apiFile.HasQueuedOperation())
            {
                Error(onError, apiFile, "A previous upload is still being processed. Please try again later.");
                yield break;
            }

            // prepare file for upload
            Progress(onProgress, apiFile, "Preparing file for upload...", "Optimizing file");

            string uploadFilename = VRC.Tools.GetTempFileName(Path.GetExtension(filename), out errorStr, apiFile.id);
            if (string.IsNullOrEmpty(uploadFilename))
            {
                Error(onError, apiFile, "Failed to optimize file for upload.", "Failed to create temp file: \n" + errorStr);
                yield break;
            }

            wasError = false;
            yield return MelonCoroutines.Start(CreateOptimizedFileInternal(filename, uploadFilename,
                delegate (FileOpResult res)
                {
                    if (res == FileOpResult.Unchanged)
                        uploadFilename = filename;
                },
                delegate (string error)
                {
                    Error(onError, apiFile, "Failed to optimize file for upload.", error);
                    CleanupTempFiles(apiFile.id);
                    wasError = true;
                })
            );

            if (wasError)
                yield break;

            LogApiFileStatus(apiFile, false);

            // generate md5 and check if file has changed
            Progress(onProgress, apiFile, "Preparing file for upload...", "Generating file hash");

            string fileMD5Base64 = "";
            wait = true;
            errorStr = "";

            System.Action<Il2CppStructArray<byte>> Convertmb5bytes = (Il2CppStructArray<byte> c) =>
            {
                fileMD5Base64 = Convert.ToBase64String(c);
                wait = false;
            };

            System.Action<string> mdserror = (string error) =>
            {
                errorStr = uploadFilename + "\n" + error;
                wait = false;
            };

            VRC.Tools.FileMD5(uploadFilename, Convertmb5bytes, mdserror);

            while (wait)
            {
                if (CheckCancelled(cancelQuery, onError, apiFile))
                {
                    CleanupTempFiles(apiFile.id);
                    yield break;
                }
                yield return null;
            }

            if (!string.IsNullOrEmpty(errorStr))
            {
                Error(onError, apiFile, "Failed to generate MD5 hash for upload file.", errorStr);
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            LogApiFileStatus(apiFile, false);

            // check if file has been changed
            Progress(onProgress, apiFile, "Preparing file for upload...", "Checking for changes");

            bool isPreviousUploadRetry = false;
            if (apiFile.HasExistingOrPendingVersion())
            {
                // uploading the same file?
                if (string.Compare(fileMD5Base64, apiFile.GetFileMD5(apiFile.GetLatestVersionNumber())) == 0)
                {
                    // the previous operation completed successfully?
                    if (!apiFile.IsWaitingForUpload())
                    {
                        Success(onSuccess, apiFile, "The file to upload is unchanged.");
                        CleanupTempFiles(apiFile.id);
                        yield break;
                    }
                    else
                    {
                        isPreviousUploadRetry = true;

                        Debug.Log("Retrying previous upload");
                    }
                }
                else
                {
                    // the file has been modified
                    if (apiFile.IsWaitingForUpload())
                    {
                        // previous upload failed, and the file is changed
                        while (true)
                        {
                            // delete previous failed version
                            Progress(onProgress, apiFile, "Preparing file for upload...", "Cleaning up previous version");

                            wait = true;
                            worthRetry = false;
                            errorStr = "";

                            apiFile.DeleteLatestVersion(fileSuccess, fileFailure);

                            while (wait)
                            {
                                if (CheckCancelled(cancelQuery, onError, apiFile))
                                {
                                    yield break;
                                }
                                yield return null;
                            }

                            if (!string.IsNullOrEmpty(errorStr))
                            {
                                Error(onError, apiFile, "Failed to delete previous incomplete version!", errorStr);
                                if (!worthRetry)
                                {
                                    CleanupTempFiles(apiFile.id);
                                    yield break;
                                }
                            }

                            // delay to let write get through servers
                            yield return new WaitForSecondsRealtime(kPostWriteDelay);

                            if (!worthRetry)
                                break;
                        }
                    }
                }
            }

            LogApiFileStatus(apiFile, false);

            // generate signature for new file

            Progress(onProgress, apiFile, "Preparing file for upload...", "Generating signature");

            string signatureFilename = VRC.Tools.GetTempFileName(".sig", out errorStr, apiFile.id);
            if (string.IsNullOrEmpty(signatureFilename))
            {
                Error(onError, apiFile, "Failed to generate file signature!", "Failed to create temp file: \n" + errorStr);
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            wasError = false;
            yield return MelonCoroutines.Start(CreateFileSignatureInternal(uploadFilename, signatureFilename,
                delegate ()
                {
                    // success!
                },
                delegate (string error)
                {
                    Error(onError, apiFile, "Failed to generate file signature!", error);
                    CleanupTempFiles(apiFile.id);
                    wasError = true;
                })
            );

            if (wasError)
                yield break;

            LogApiFileStatus(apiFile, false);

            // generate signature md5 and file size
            Progress(onProgress, apiFile, "Preparing file for upload...", "Generating signature hash");

            string sigMD5Base64 = "";
            wait = true;
            errorStr = "";
            System.Action<Il2CppStructArray<byte>> Convertsig5bytes = (Il2CppStructArray<byte> c) =>
            {
                sigMD5Base64 = Convert.ToBase64String(c);
                wait = false;
            };

            System.Action<string> sigerror = (string error) =>
            {
                errorStr = signatureFilename + "\n" + error;
                wait = false;
            };
            VRC.Tools.FileMD5(signatureFilename, Convertsig5bytes, sigerror);

            while (wait)
            {
                if (CheckCancelled(cancelQuery, onError, apiFile))
                {
                    CleanupTempFiles(apiFile.id);
                    yield break;
                }
                yield return null;
            }

            if (!string.IsNullOrEmpty(errorStr))
            {
                Error(onError, apiFile, "Failed to generate MD5 hash for signature file.", errorStr);
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            long sigFileSize = 0;
            if (!VRC.Tools.GetFileSize(signatureFilename, out sigFileSize, out errorStr))
            {
                Error(onError, apiFile, "Failed to generate file signature!", "Couldn't get file size:\n" + errorStr);
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            LogApiFileStatus(apiFile, false);

            // download previous version signature (if exists)
            string existingFileSignaturePath = null;
            if (EnableDeltaCompression && apiFile.HasExistingVersion())
            {
                Progress(onProgress, apiFile, "Preparing file for upload...", "Downloading previous version signature");

                wait = true;
                errorStr = "";

                System.Action<Il2CppStructArray<byte>> signaturebytes = (Il2CppStructArray<byte> data) =>
                {
                    existingFileSignaturePath = VRC.Tools.GetTempFileName(".sig", out errorStr, apiFile.id);
                    if (string.IsNullOrEmpty(existingFileSignaturePath))
                    {
                        errorStr = "Failed to create temp file: \n" + errorStr;
                        wait = false;
                    }
                    else
                    {
                        try
                        {
                            File.WriteAllBytes(existingFileSignaturePath, data);
                        }
                        catch (Exception e)
                        {
                            existingFileSignaturePath = null;
                            errorStr = "Failed to write signature temp file:\n" + e.Message;
                        }
                        wait = false;
                    }
                };

                System.Action<string> signatureerror = (string error) =>
                {
                    errorStr = error;
                    wait = false;
                };

                System.Action<long, long> signatureprogress = (long downloaded, long length) =>
                {
                    Progress(onProgress, apiFile, "Preparing file for upload...", "Downloading previous version signature", Tools.DivideSafe(downloaded, length));
                };

                apiFile.DownloadSignature(signaturebytes, signatureerror, signatureprogress);

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onError, apiFile))
                    {
                        CleanupTempFiles(apiFile.id);
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    Error(onError, apiFile, "Failed to download previous file version signature.", errorStr);
                    CleanupTempFiles(apiFile.id);
                    yield break;
                }
            }

            LogApiFileStatus(apiFile, false);

            // create delta if needed
            string deltaFilename = null;

            if (EnableDeltaCompression && !string.IsNullOrEmpty(existingFileSignaturePath))
            {
                Progress(onProgress, apiFile, "Preparing file for upload...", "Creating file delta");

                deltaFilename = VRC.Tools.GetTempFileName(".delta", out errorStr, apiFile.id);
                if (string.IsNullOrEmpty(deltaFilename))
                {
                    Error(onError, apiFile, "Failed to create file delta for upload.", "Failed to create temp file: \n" + errorStr);
                    CleanupTempFiles(apiFile.id);
                    yield break;
                }

                wasError = false;
                yield return MelonCoroutines.Start(CreateFileDeltaInternal(uploadFilename, existingFileSignaturePath, deltaFilename,
                    delegate ()
                    {
                        // success!
                    },
                    delegate (string error)
                    {
                        Error(onError, apiFile, "Failed to create file delta for upload.", error);
                        CleanupTempFiles(apiFile.id);
                        wasError = true;
                    })
                );

                if (wasError)
                    yield break;
            }

            // upload smaller of delta and new file
            long fullFizeSize = 0;
            long deltaFileSize = 0;
            if (!VRC.Tools.GetFileSize(uploadFilename, out fullFizeSize, out errorStr) ||
                (!string.IsNullOrEmpty(deltaFilename) && !VRC.Tools.GetFileSize(deltaFilename, out deltaFileSize, out errorStr)))
            {
                Error(onError, apiFile, "Failed to create file delta for upload.", "Couldn't get file size: " + errorStr);
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            bool uploadDeltaFile = EnableDeltaCompression && deltaFileSize > 0 && deltaFileSize < fullFizeSize;
            if (EnableDeltaCompression)
                Debug.Log("Delta size " + deltaFileSize + " (" + ((float)deltaFileSize / (float)fullFizeSize) + " %), full file size " + fullFizeSize + ", uploading " + (uploadDeltaFile ? " DELTA" : " FULL FILE"));
            else
                Debug.Log("Delta compression disabled, uploading FULL FILE, size " + fullFizeSize);

            LogApiFileStatus(apiFile, uploadDeltaFile);

            string deltaMD5Base64 = "";
            if (uploadDeltaFile)
            {
                Progress(onProgress, apiFile, "Preparing file for upload...", "Generating file delta hash");

                wait = true;
                errorStr = "";
                System.Action<Il2CppStructArray<byte>> Convertdelta5bytes = (Il2CppStructArray<byte> c) =>
                {
                    sigMD5Base64 = Convert.ToBase64String(c);
                    wait = false;
                };

                System.Action<string> deltaerror = (string error) =>
                {
                    errorStr = error;
                    wait = false;
                };

                VRC.Tools.FileMD5(deltaFilename, Convertdelta5bytes, deltaerror);

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onError, apiFile))
                    {
                        CleanupTempFiles(apiFile.id);
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    Error(onError, apiFile, "Failed to generate file delta hash.", errorStr);
                    CleanupTempFiles(apiFile.id);
                    yield break;
                }
            }

            // validate existing pending version info, if this is a retry
            bool versionAlreadyExists = false;

            LogApiFileStatus(apiFile, uploadDeltaFile);

            if (isPreviousUploadRetry)
            {
                bool isValid = true;

                ApiFile.Version v = apiFile.GetVersion(apiFile.GetLatestVersionNumber());
                if (v != null)
                {
                    if (uploadDeltaFile)
                    {
                        isValid = deltaFileSize == v.delta.sizeInBytes &&
                            deltaMD5Base64.CompareTo(v.delta.md5) == 0 &&
                            sigFileSize == v.signature.sizeInBytes &&
                            sigMD5Base64.CompareTo(v.signature.md5) == 0;
                    }
                    else
                    {
                        isValid = fullFizeSize == v.file.sizeInBytes &&
                            fileMD5Base64.CompareTo(v.file.md5) == 0 &&
                            sigFileSize == v.signature.sizeInBytes &&
                            sigMD5Base64.CompareTo(v.signature.md5) == 0;
                    }
                }
                else
                {
                    isValid = false;
                }

                if (isValid)
                {
                    versionAlreadyExists = true;

                    Debug.Log("Using existing version record");
                }
                else
                {
                    // delete previous invalid version
                    Progress(onProgress, apiFile, "Preparing file for upload...", "Cleaning up previous version");

                    while (true)
                    {
                        wait = true;
                        errorStr = "";
                        worthRetry = false;

                        apiFile.DeleteLatestVersion(fileSuccess, fileFailure);

                        while (wait)
                        {
                            if (CheckCancelled(cancelQuery, onError, null))
                            {
                                yield break;
                            }
                            yield return null;
                        }

                        if (!string.IsNullOrEmpty(errorStr))
                        {
                            Error(onError, apiFile, "Failed to delete previous incomplete version!", errorStr);
                            if (!worthRetry)
                            {
                                CleanupTempFiles(apiFile.id);
                                yield break;
                            }
                        }

                        // delay to let write get through servers
                        yield return new WaitForSecondsRealtime(kPostWriteDelay);

                        if (!worthRetry)
                            break;
                    }
                }
            }

            LogApiFileStatus(apiFile, uploadDeltaFile);

            // create new version of file
            if (!versionAlreadyExists)
            {
                while (true)
                {
                    Progress(onProgress, apiFile, "Creating file version record...");

                    wait = true;
                    errorStr = "";
                    worthRetry = false;

                    if (uploadDeltaFile)
                        // delta file
                        apiFile.CreateNewVersion(ApiFile.Version.FileType.Delta, deltaMD5Base64, deltaFileSize, sigMD5Base64, sigFileSize, fileSuccess, fileFailure);
                    else
                        // full file
                        apiFile.CreateNewVersion(ApiFile.Version.FileType.Full, fileMD5Base64, fullFizeSize, sigMD5Base64, sigFileSize, fileSuccess, fileFailure);

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onError, apiFile))
                        {
                            CleanupTempFiles(apiFile.id);
                            yield break;
                        }

                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        Error(onError, apiFile, "Failed to create file version record.", errorStr);
                        if (!worthRetry)
                        {
                            CleanupTempFiles(apiFile.id);
                            yield break;
                        }
                    }

                    // delay to let write get through servers
                    yield return new WaitForSecondsRealtime(kPostWriteDelay);

                    if (!worthRetry)
                        break;
                }
            }

            // upload components

            LogApiFileStatus(apiFile, uploadDeltaFile);

            // upload delta
            if (uploadDeltaFile)
            {
                if (apiFile.GetLatestVersion().delta.status == ApiFile.Status.Waiting)
                {
                    Progress(onProgress, apiFile, "Uploading file delta...");

                    wasError = false;
                    yield return MelonCoroutines.Start(UploadFileComponentInternal(apiFile,
                        ApiFile.Version.FileDescriptor.Type.delta, deltaFilename, deltaMD5Base64, deltaFileSize,
                        delegate (ApiFile file)
                        {
                            Debug.Log("Successfully uploaded file delta.");
                            apiFile = file;
                        },
                        delegate (string error)
                        {
                            Error(onError, apiFile, "Failed to upload file delta.", error);
                            CleanupTempFiles(apiFile.id);
                            wasError = true;
                        },
                        delegate (long downloaded, long length)
                        {
                            Progress(onProgress, apiFile, "Uploading file delta...", "", Tools.DivideSafe(downloaded, length));
                        },
                        cancelQuery)
                    );

                    if (wasError)
                        yield break;
                }
            }
            // upload file
            else
            {
                if (apiFile.GetLatestVersion().file.status == ApiFile.Status.Waiting)
                {
                    Progress(onProgress, apiFile, "Uploading file...");

                    wasError = false;
                    yield return MelonCoroutines.Start(UploadFileComponentInternal(apiFile,
                        ApiFile.Version.FileDescriptor.Type.file, uploadFilename, fileMD5Base64, fullFizeSize,
                        delegate (ApiFile file)
                        {
                            Debug.Log("Successfully uploaded file.");
                            apiFile = file;
                        },
                        delegate (string error)
                        {
                            Error(onError, apiFile, "Failed to upload file.", error);
                            CleanupTempFiles(apiFile.id);
                            wasError = true;
                        },
                        delegate (long downloaded, long length)
                        {
                            Progress(onProgress, apiFile, "Uploading file...", "", Tools.DivideSafe(downloaded, length));
                        },
                        cancelQuery)
                    );

                    if (wasError)
                        yield break;
                }
            }

            LogApiFileStatus(apiFile, uploadDeltaFile);

            // upload signature
            if (apiFile.GetLatestVersion().signature.status == ApiFile.Status.Waiting)
            {
                Progress(onProgress, apiFile, "Uploading file signature...");

                wasError = false;
                yield return MelonCoroutines.Start(UploadFileComponentInternal(apiFile,
                    ApiFile.Version.FileDescriptor.Type.signature, signatureFilename, sigMD5Base64, sigFileSize,
                    delegate (ApiFile file)
                    {
                        Debug.Log("Successfully uploaded file signature.");
                        apiFile = file;
                    },
                    delegate (string error)
                    {
                        Error(onError, apiFile, "Failed to upload file signature.", error);
                        CleanupTempFiles(apiFile.id);
                        wasError = true;
                    },
                    delegate (long downloaded, long length)
                    {
                        Progress(onProgress, apiFile, "Uploading file signature...", "", Tools.DivideSafe(downloaded, length));
                    },
                    cancelQuery)
                );

                if (wasError)
                    yield break;
            }

            LogApiFileStatus(apiFile, uploadDeltaFile);

            // Validate file records queued or complete
            Progress(onProgress, apiFile, "Validating upload...");

            bool isUploadComplete = (uploadDeltaFile
                ? apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.delta).status == ApiFile.Status.Complete
                : apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.file).status == ApiFile.Status.Complete);
            isUploadComplete = isUploadComplete &&
                               apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.signature).status == ApiFile.Status.Complete;

            if (!isUploadComplete)
            {
                Error(onError, apiFile, "Failed to upload file.", "Record status is not 'complete'");
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            bool isServerOpQueuedOrComplete = (uploadDeltaFile
                ? apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.file).status != ApiFile.Status.Waiting
                : apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.delta).status != ApiFile.Status.Waiting);

            if (!isServerOpQueuedOrComplete)
            {
                Error(onError, apiFile, "Failed to upload file.", "Record is still in 'waiting' status");
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            LogApiFileStatus(apiFile, uploadDeltaFile);

            // wait for server processing to complete
            Progress(onProgress, apiFile, "Processing upload...");
            float checkDelay = SERVER_PROCESSING_INITIAL_RETRY_TIME;
            float maxDelay = SERVER_PROCESSING_MAX_RETRY_TIME;
            float timeout = GetServerProcessingWaitTimeoutForDataSize(apiFile.GetLatestVersion().file.sizeInBytes);
            double initialStartTime = Time.realtimeSinceStartup;
            double startTime = initialStartTime;
            while (apiFile.HasQueuedOperation(uploadDeltaFile))
            {
                // wait before polling again
                Progress(onProgress, apiFile, "Processing upload...", "Checking status in " + Mathf.CeilToInt(checkDelay) + " seconds");

                while (Time.realtimeSinceStartup - startTime < checkDelay)
                {
                    if (CheckCancelled(cancelQuery, onError, apiFile))
                    {
                        CleanupTempFiles(apiFile.id);
                        yield break;
                    }

                    if (Time.realtimeSinceStartup - initialStartTime > timeout)
                    {
                        LogApiFileStatus(apiFile, uploadDeltaFile);

                        Error(onError, apiFile, "Timed out waiting for upload processing to complete.");
                        CleanupTempFiles(apiFile.id);
                        yield break;
                    }

                    yield return null;
                }

                while (true)
                {
                    // check status
                    Progress(onProgress, apiFile, "Processing upload...", "Checking status...");

                    wait = true;
                    worthRetry = false;
                    errorStr = "";
                    API.Fetch<ApiFile>(apiFile.id, fileSuccess, fileFailure);

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onError, apiFile))
                        {
                            CleanupTempFiles(apiFile.id);
                            yield break;
                        }

                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        Error(onError, apiFile, "Checking upload status failed.", errorStr);
                        if (!worthRetry)
                        {
                            CleanupTempFiles(apiFile.id);
                            yield break;
                        }
                    }

                    if (!worthRetry)
                        break;
                }

                checkDelay = Mathf.Min(checkDelay * 2, maxDelay);
                startTime = Time.realtimeSinceStartup;
            }

            // cleanup and wait for it to finish
            yield return MelonCoroutines.Start(CleanupTempFilesInternal(apiFile.id));

            Success(onSuccess, apiFile, "Upload complete!");
        }

        private static void LogApiFileStatus(ApiFile apiFile, bool checkDelta)
        {
            if (apiFile == null || !apiFile.IsInitialized)
            {
                PendulumLogger.Log(ConsoleColor.Yellow, "apiFile not initialized");
            }
            else if (apiFile.IsInErrorState())
            {
                PendulumLogger.Log(ConsoleColor.Yellow, "ApiFile {0} is in an error state.", apiFile.name);
            }
            else
                PendulumLogger.Log(ConsoleColor.Yellow, "Processing {3}: {0}, {1}, {2}",
                    apiFile.IsWaitingForUpload() ? "waiting for upload" : "upload complete",
                    apiFile.HasExistingOrPendingVersion() ? "has existing or pending version" : "no previous version",
                    apiFile.IsLatestVersionQueued(checkDelta) ? "latest version queued" : "latest version not queued",
                    apiFile.name);

            if (apiFile != null && apiFile.IsInitialized)
            {
                var apiFields = apiFile.ExtractApiFields();
                if (apiFields != null)
                    PendulumLogger.Log("ApiFields: {0}", VRC.Tools.JsonEncode(apiFields));
            }
        }

        public IEnumerator CreateOptimizedFileInternal(string filename, string outputFilename, Action<FileOpResult> onSuccess, Action<string> onError)
        {
            Debug.Log("CreateOptimizedFile: " + filename + " => " + outputFilename);

            // assume it's a .gz, or a .unitypackage
            // else nothing to do

            if (!IsGZipCompressed(filename))
            {
                Debug.Log("CreateOptimizedFile: (not gzip compressed, done)");
                // nothing to do
                if (onSuccess != null)
                    onSuccess(FileOpResult.Unchanged);
                yield break;
            }

            bool isUnityPackage = string.Compare(Path.GetExtension(filename), ".unitypackage", true) == 0;

            yield return null;

            // open file
            const int kGzipBufferSize = 256 * 1024;
            Stream inStream = null;
            try
            {
                inStream = new DotZLib.GZipStream(filename);
            }
            catch (Exception e)
            {
                if (onError != null)
                    onError("Couldn't read file: " + filename + "\n" + e.Message);
                yield break;
            }

            yield return null;

            // create output
            DotZLib.GZipStream outStream = null;
            try
            {
                outStream = new DotZLib.GZipStream(outputFilename, DotZLib.CompressLevel.Best);    // this lib supports rsyncable output
            }
            catch (Exception e)
            {
                if (inStream != null)
                    inStream.Close();
                if (onError != null)
                    onError("Couldn't create output file: " + outputFilename + "\n" + e.Message);
                yield break;
            }

            yield return null;

            // copy / filter file
            if (isUnityPackage)
            {
                try
                {
                    // discard files in the package we don't need

                    // scan package and make list of asset guids we don't want
                    System.Collections.Generic.List<string> assetGuidsToStrip = new System.Collections.Generic.List<string>();
                    {
                        byte[] filenameBuf = new byte[4096];
                        ICSharpCode.SharpZipLib.Tar.TarInputStream tarInputStream = new ICSharpCode.SharpZipLib.Tar.TarInputStream(inStream);
                        ICSharpCode.SharpZipLib.Tar.TarEntry tarEntry = tarInputStream.GetNextEntry();
                        while (tarEntry != null)
                        {
                            if (tarEntry.Size > 0 && tarEntry.Name.EndsWith("/pathname", StringComparison.OrdinalIgnoreCase))
                            {
                                int bytesRead = tarInputStream.Read(filenameBuf, 0, (int)tarEntry.Size);
                                if (bytesRead > 0)
                                {
                                    string assetFilename = System.Text.ASCIIEncoding.ASCII.GetString(filenameBuf, 0, bytesRead);
                                    if (kUnityPackageAssetNameFilters.Any(r => r.IsMatch(assetFilename)))
                                    {
                                        string assetGuid = assetFilename.Substring(0, assetFilename.IndexOf('/'));
                                        // Debug.Log("-- stripped file from package: " + assetGuid + " - " + assetFilename);
                                        assetGuidsToStrip.Add(assetGuid);
                                    }
                                }
                            }

                            tarEntry = tarInputStream.GetNextEntry();
                        }

                        tarInputStream.Close();
                    }

                    // rescan input .tar and copy only entries we want to the output
                    {
                        inStream.Close();
                        inStream = new DotZLib.GZipStream(filename);

                        ICSharpCode.SharpZipLib.Tar.TarOutputStream tarOutputStream = new ICSharpCode.SharpZipLib.Tar.TarOutputStream(outStream);

                        ICSharpCode.SharpZipLib.Tar.TarInputStream tarInputStream = new ICSharpCode.SharpZipLib.Tar.TarInputStream(inStream);
                        ICSharpCode.SharpZipLib.Tar.TarEntry tarEntry = tarInputStream.GetNextEntry();
                        while (tarEntry != null)
                        {
                            string assetGuid = tarEntry.Name.Substring(0, tarEntry.Name.IndexOf('/'));
                            bool strip = assetGuidsToStrip.Any(s => string.Compare(s, assetGuid) == 0);
                            if (!strip)
                            {
                                tarOutputStream.PutNextEntry(tarEntry);
                                tarInputStream.CopyEntryContents(tarOutputStream);
                                tarOutputStream.CloseEntry();
                            }

                            tarEntry = tarInputStream.GetNextEntry();
                        }

                        tarInputStream.Close();
                        tarOutputStream.Close();
                    }
                }
                catch (Exception e)
                {
                    if (inStream != null)
                        inStream.Close();
                    if (outStream != null)
                        outStream.Close();
                    if (onError != null)
                        onError("Failed to strip and recompress file." + "\n" + e.Message);
                    yield break;
                }
            }
            else
            {
                // not a unitypackage 

                // straight stream copy
                try
                {
                    const int bufSize = 256 * 1024;
                    byte[] buf = new byte[bufSize];
                    ICSharpCode.SharpZipLib.Core.StreamUtils.Copy(inStream, outStream, buf);
                }
                catch (Exception e)
                {
                    if (inStream != null)
                        inStream.Close();
                    if (outStream != null)
                        outStream.Close();
                    if (onError != null)
                        onError("Failed to recompress file." + "\n" + e.Message);
                    yield break;
                }
            }

            yield return null;

            if (inStream != null)
                inStream.Close();
            inStream = null;
            if (outStream != null)
                outStream.Close();
            outStream = null;

            yield return null;

            if (onSuccess != null)
                onSuccess(FileOpResult.Success);
        }

        public IEnumerator CreateFileSignatureInternal(string filename, string outputSignatureFilename, Action onSuccess, Action<string> onError)
        {
            Debug.Log("CreateFileSignature: " + filename + " => " + outputSignatureFilename);

            yield return null;

            Stream inStream = null;
            FileStream outStream = null;
            byte[] buf = new byte[64 * 1024];
            IAsyncResult asyncRead = null;
            IAsyncResult asyncWrite = null;

            try
            {
                inStream = librsync.net.Librsync.ComputeSignature(File.OpenRead(filename));
            }
            catch (Exception e)
            {
                if (onError != null)
                    onError("Couldn't open input file: " + e.Message);
                yield break;
            }

            try
            {
                outStream = File.Open(outputSignatureFilename, FileMode.Create, FileAccess.Write);
            }
            catch (Exception e)
            {
                if (onError != null)
                    onError("Couldn't create output file: " + e.Message);
                yield break;
            }

            while (true)
            {
                try
                {
                    asyncRead = inStream.BeginRead(buf, 0, buf.Length, null, null);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't read file: " + e.Message);
                    yield break;
                }

                while (!asyncRead.IsCompleted)
                    yield return null;

                int read = 0;
                try
                {
                    read = inStream.EndRead(asyncRead);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't read file: " + e.Message);
                    yield break;
                }

                if (read <= 0)
                    break;

                try
                {
                    asyncWrite = outStream.BeginWrite(buf, 0, read, null, null);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't write file: " + e.Message);
                    yield break;
                }

                while (!asyncWrite.IsCompleted)
                    yield return null;

                try
                {
                    outStream.EndWrite(asyncWrite);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't write file: " + e.Message);
                    yield break;
                }
            }

            inStream.Close();
            outStream.Close();

            yield return null;

            if (onSuccess != null)
                onSuccess();
        }

        public IEnumerator CreateFileDeltaInternal(string newFilename, string existingFileSignaturePath, string outputDeltaFilename, Action onSuccess, Action<string> onError)
        {
            Debug.Log("CreateFileDelta: " + newFilename + " (delta) " + existingFileSignaturePath + " => " + outputDeltaFilename);

            yield return null;

            Stream inStream = null;
            FileStream outStream = null;
            byte[] buf = new byte[64 * 1024];
            IAsyncResult asyncRead = null;
            IAsyncResult asyncWrite = null;

            try
            {
                inStream = librsync.net.Librsync.ComputeDelta(File.OpenRead(existingFileSignaturePath), File.OpenRead(newFilename));
            }
            catch (Exception e)
            {
                if (onError != null)
                    onError("Couldn't open input file: " + e.Message);
                yield break;
            }

            try
            {
                outStream = File.Open(outputDeltaFilename, FileMode.Create, FileAccess.Write);
            }
            catch (Exception e)
            {
                if (onError != null)
                    onError("Couldn't create output file: " + e.Message);
                yield break;
            }

            while (true)
            {
                try
                {
                    asyncRead = inStream.BeginRead(buf, 0, buf.Length, null, null);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't read file: " + e.Message);
                    yield break;
                }

                while (!asyncRead.IsCompleted)
                    yield return null;

                int read = 0;
                try
                {
                    read = inStream.EndRead(asyncRead);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't read file: " + e.Message);
                    yield break;
                }

                if (read <= 0)
                    break;

                try
                {
                    asyncWrite = outStream.BeginWrite(buf, 0, read, null, null);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't write file: " + e.Message);
                    yield break;
                }

                while (!asyncWrite.IsCompleted)
                    yield return null;

                try
                {
                    outStream.EndWrite(asyncWrite);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't write file: " + e.Message);
                    yield break;
                }
            }

            inStream.Close();
            outStream.Close();

            yield return null;

            if (onSuccess != null)
                onSuccess();
        }

        protected static void Success(OnFileOpSuccess onSuccess, ApiFile apiFile, string message)
        {
            if (apiFile == null)
                apiFile = new ApiFile();

            Debug.Log("ApiFile " + apiFile.ToStringBrief() + ": Operation Succeeded!");
            if (onSuccess != null)
                onSuccess(apiFile, message);
        }

        protected static void Error(OnFileOpError onError, ApiFile apiFile, string error, string moreInfo = "")
        {
            if (apiFile == null)
                apiFile = new ApiFile();

            Debug.LogError("ApiFile " + apiFile.ToStringBrief() + ": Error: " + error + "\n" + moreInfo);
            if (onError != null)
                onError(apiFile, error);
        }

        protected static void Progress(OnFileOpProgress onProgress, ApiFile apiFile, string status, string subStatus = "", float pct = 0.0f)
        {
            if (apiFile == null)
                apiFile = new ApiFile();

            if (onProgress != null)
                onProgress(apiFile, status, subStatus, pct);
        }

        protected static bool CheckCancelled(FileOpCancelQuery cancelQuery, OnFileOpError onError, ApiFile apiFile)
        {
            if (apiFile == null)
            {
                Debug.LogError("apiFile was null");
                return true;
            }

            if (cancelQuery != null && cancelQuery(apiFile))
            {
                Debug.Log("ApiFile " + apiFile.ToStringBrief() + ": Operation cancelled");
                if (onError != null)
                    onError(apiFile, "Cancelled by user.");
                return true;
            }

            return false;
        }

        protected static void CleanupTempFiles(string subFolderName)
        {
            MelonCoroutines.Start(Instance.CleanupTempFilesInternal(subFolderName));
        }

        protected IEnumerator CleanupTempFilesInternal(string subFolderName)
        {
            if (!string.IsNullOrEmpty(subFolderName))
            {
                string folder = VRC.Tools.GetTempFolderPath(subFolderName);

                while (Directory.Exists(folder))
                {
                    try
                    {
                        if (Directory.Exists(folder))
                            Directory.Delete(folder, true);
                    }
                    catch (System.Exception)
                    {
                    }

                    yield return null;
                }
            }
        }

        private static void CheckInstance()
        {
            if (mInstance == null)
            {
                GameObject go = new GameObject("ApiFileHelperOld");
                mInstance = go.AddComponent<ApiFileHelperOld>();

                try
                {
                    GameObject.DontDestroyOnLoad(go);
                }
                catch
                {
                }
            }
        }

        private float GetServerProcessingWaitTimeoutForDataSize(int size)
        {
            float timeoutMultiplier = Mathf.Ceil((float)size / (float)SERVER_PROCESSING_WAIT_TIMEOUT_CHUNK_SIZE);
            return Mathf.Clamp(timeoutMultiplier * SERVER_PROCESSING_WAIT_TIMEOUT_PER_CHUNK_SIZE, SERVER_PROCESSING_WAIT_TIMEOUT_PER_CHUNK_SIZE, SERVER_PROCESSING_MAX_WAIT_TIMEOUT);
        }

        private bool uploadFileComponentValidateFileDesc(ApiFile apiFile, string filename, string md5Base64, long fileSize, ApiFile.Version.FileDescriptor fileDesc, Action<ApiFile> onSuccess, Action<string> onError)
        {
            if (fileDesc.status != ApiFile.Status.Waiting)
            {
                // nothing to do (might be a retry)
                Debug.Log("UploadFileComponent: (file record not in waiting status, done)");
                if (onSuccess != null)
                    onSuccess(apiFile);
                return false;
            }

            if (fileSize != fileDesc.sizeInBytes)
            {
                if (onError != null)
                    onError("File size does not match version descriptor");
                return false;
            }
            if (string.Compare(md5Base64, fileDesc.md5) != 0)
            {
                if (onError != null)
                    onError("File MD5 does not match version descriptor");
                return false;
            }

            // make sure file is right size
            long tempSize = 0;
            string errorStr = "";
            if (!VRC.Tools.GetFileSize(filename, out tempSize, out errorStr))
            {
                if (onError != null)
                    onError("Couldn't get file size");
                return false;
            }
            if (tempSize != fileSize)
            {
                if (onError != null)
                    onError("File size does not match input size");
                return false;
            }

            return true;
        }

        private IEnumerator uploadFileComponentDoSimpleUpload(ApiFile apiFile, ApiFile.Version.FileDescriptor.Type fileDescriptorType, string filename, string md5Base64, long fileSize, Action<ApiFile> onSuccess, Action<string> onError, Action<long, long> onProgess, FileOpCancelQuery cancelQuery)
        {
            OnFileOpError onCancelFunc = delegate (ApiFile file, string s)
            {
                if (onError != null)
                    onError(s);
            };

            string uploadUrl = "";
            while (true)
            {
                bool wait = true;
                string errorStr = "";
                bool worthRetry = false;

                System.Action<ApiContainer> SimpUploadSuccess = (ApiContainer c) =>
                {
                    //uploadUrl = (c as ApiDictContainer).ResponseDictionary.Keys["url"] as Il2CppSystem.String;
                    wait = false;
                };

                System.Action<ApiContainer> SimpUploadFailure = (ApiContainer c) =>
                {
                    errorStr = "Failed to start upload: " + c.Error;
                    wait = false;
                    if (c.Code == 400)
                        worthRetry = true;
                };

                apiFile.StartSimpleUpload(fileDescriptorType, SimpUploadSuccess, SimpUploadFailure);

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                    {
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    if (onError != null)
                        onError(errorStr);
                    if (!worthRetry)
                        yield break;
                }

                // delay to let write get through servers
                yield return new WaitForSecondsRealtime(kPostWriteDelay);

                if (!worthRetry)
                    break;
            }

            // PUT file
            {
                bool wait = true;
                string errorStr = "";

                System.Action HTTPReqSuccess = new System.Action(() =>
                {
                    wait = false;
                });

                System.Action<string> HTTPReqFailure = (string error) =>
                {
                    errorStr = "Failed to upload file: " + error;
                    wait = false;
                };

                System.Action<long, long> HTTPReqProgress = (long uploaded, long length) =>
                {
                    if (onProgess != null)
                        onProgess(uploaded, length);
                };

                VRC.HttpRequest req = ApiFile.PutSimpleFileToURL(uploadUrl, filename, GetMimeTypeFromExtension(Path.GetExtension(filename)), md5Base64, false, HTTPReqSuccess, HTTPReqFailure, HTTPReqProgress);

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                    {
                        if (req != null)
                            req.Abort();
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    if (onError != null)
                        onError(errorStr);
                    yield break;
                }
            }

            // finish upload
            while (true)
            {
                // delay to let write get through servers
                yield return new WaitForSecondsRealtime(kPostWriteDelay);

                bool wait = true;
                string errorStr = "";
                bool worthRetry = false;

                System.Action<ApiContainer> FinuploadFail = (ApiContainer c) =>
                {
                    errorStr = "Failed to finish upload: " + c.Error;
                    wait = false;
                    if (c.Code == 400)
                        worthRetry = false;
                };

                System.Action<ApiContainer> FinuploadSuccess = (ApiContainer c) =>
                {
                    apiFile = c.Model as ApiFile;
                    wait = false;
                };

                apiFile.FinishUpload(fileDescriptorType, null, FinuploadSuccess, FinuploadFail);

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                    {
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    if (onError != null)
                        onError(errorStr);
                    if (!worthRetry)
                        yield break;
                }

                // delay to let write get through servers
                yield return new WaitForSecondsRealtime(kPostWriteDelay);

                if (!worthRetry)
                    break;
            }

        }

        private IEnumerator uploadFileComponentDoMultipartUpload(ApiFile apiFile, ApiFile.Version.FileDescriptor.Type fileDescriptorType, string filename, string md5Base64, long fileSize, Action<ApiFile> onSuccess, Action<string> onError, Action<long, long> onProgess, FileOpCancelQuery cancelQuery)
        {
            FileStream fs = null;
            OnFileOpError onCancelFunc = delegate (ApiFile file, string s)
            {
                if (fs != null)
                    fs.Close();
                if (onError != null)
                    onError(s);
            };

            // query multipart upload status.
            // we might be resuming a previous upload
            ApiFile.UploadStatus uploadStatus = null;
            {
                while (true)
                {
                    bool wait = true;
                    string errorStr = "";
                    bool worthRetry = false;

                    System.Action<ApiContainer> UploadStatusFail = (ApiContainer c) =>
                    {
                        errorStr = "Failed to query multipart upload status: " + c.Error;
                        wait = false;
                        if (c.Code == 400)
                            worthRetry = true;
                    };

                    System.Action<ApiContainer> UploadStatusSuccess = (ApiContainer c) =>
                    {
                        uploadStatus = c.Model as ApiFile.UploadStatus;
                        wait = false;

                        Debug.Log("Found existing multipart upload status (next part = " + uploadStatus.nextPartNumber + ")");
                    };

                    apiFile.GetUploadStatus(apiFile.GetLatestVersionNumber(), fileDescriptorType, UploadStatusSuccess, UploadStatusFail);

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                        {
                            yield break;
                        }
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        if (onError != null)
                            onError(errorStr);
                        if (!worthRetry)
                            yield break;
                    }

                    if (!worthRetry)
                        break;
                }
            }

            // split file into chunks
            try
            {
                fs = File.OpenRead(filename);
            }
            catch (Exception e)
            {
                if (onError != null)
                    onError("Couldn't open file: " + e.Message);
                yield break;
            }

            byte[] buffer = new byte[kMultipartUploadChunkSize * 2];

            long totalBytesUploaded = 0;
            Il2CppSystem.Collections.Generic.List<string> etags = new Il2CppSystem.Collections.Generic.List<string>();
            if (uploadStatus != null)
                etags = uploadStatus.etags;

            int numParts = Mathf.Max(1, Mathf.FloorToInt((float)fs.Length / (float)kMultipartUploadChunkSize));
            for (int partNumber = 1; partNumber <= numParts; partNumber++)
            {
                // read chunk
                int bytesToRead = partNumber < numParts ? kMultipartUploadChunkSize : (int)(fs.Length - fs.Position);
                int bytesRead = 0;
                try
                {
                    bytesRead = fs.Read(buffer, 0, bytesToRead);
                }
                catch (Exception e)
                {
                    fs.Close();
                    if (onError != null)
                        onError("Couldn't read file: " + e.Message);
                    yield break;
                }

                if (bytesRead != bytesToRead)
                {
                    fs.Close();
                    if (onError != null)
                        onError("Couldn't read file: read incorrect number of bytes from stream");
                    yield break;
                }

                // check if this part has been upload already
                // NOTE: uploadStatus.nextPartNumber == number of parts already uploaded
                if (uploadStatus != null && partNumber <= uploadStatus.nextPartNumber)
                {
                    totalBytesUploaded += bytesRead;
                    continue;
                }

                // start upload
                string uploadUrl = "";

                while (true)
                {
                    bool wait = true;
                    string errorStr = "";
                    bool worthRetry = false;

                    System.Action<ApiContainer> MultipartUploadFail = (ApiContainer c) =>
                    {
                        errorStr = "Failed to start part upload: " + c.Error;
                        wait = false;
                    };

                    System.Action<ApiContainer> MultipartUploadSuccess = (ApiContainer c) =>
                    {
                        //uploadUrl = (c as ApiDictContainer).ResponseDictionary["url"].ToString();
                        wait = false;
                    };

                    apiFile.StartMultipartUpload(fileDescriptorType, partNumber, MultipartUploadSuccess, MultipartUploadFail);

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                        {
                            yield break;
                        }
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        fs.Close();
                        if (onError != null)
                            onError(errorStr);
                        if (!worthRetry)
                            yield break;
                    }

                    // delay to let write get through servers
                    yield return new WaitForSecondsRealtime(kPostWriteDelay);

                    if (!worthRetry)
                        break;
                }

                // PUT file part
                {
                    bool wait = true;
                    string errorStr = "";

                    System.Action<string> HTTPReqSuccess = (string etag) =>
                    {
                        wait = false;
                    };

                    System.Action<string> HTTPReqFailure = (string error) =>
                    {
                        errorStr = "Failed to upload file: " + error;
                        wait = false;
                    };

                    System.Action<long, long> HTTPReqProgress = (long uploaded, long length) =>
                    {
                        if (onProgess != null)
                            onProgess(uploaded, length);
                    };

                    VRC.HttpRequest req = ApiFile.PutMultipartDataToURL(uploadUrl, buffer, bytesRead, GetMimeTypeFromExtension(Path.GetExtension(filename)), false, HTTPReqSuccess, HTTPReqFailure, HTTPReqProgress);

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                        {
                            if (req != null)
                                req.Abort();
                            yield break;
                        }
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        fs.Close();
                        if (onError != null)
                            onError(errorStr);
                        yield break;
                    }
                }
            }

            // finish upload
            while (true)
            {
                // delay to let write get through servers
                yield return new WaitForSecondsRealtime(kPostWriteDelay);

                bool wait = true;
                string errorStr = "";
                bool worthRetry = false;

                System.Action<ApiContainer> FinishUploadFail = (ApiContainer c) =>
                {
                    errorStr = "Failed to finish upload: " + c.Error;
                    wait = false;
                    if (c.Code == 400)
                        worthRetry = true;
                };

                System.Action<ApiContainer> FinishUploadSuccess = (ApiContainer c) =>
                {
                    apiFile = c.Model as ApiFile;
                    wait = false;
                };

                apiFile.FinishUpload(fileDescriptorType, etags, FinishUploadSuccess, FinishUploadFail);

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                    {
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    fs.Close();
                    if (onError != null)
                        onError(errorStr);
                    if (!worthRetry)
                        yield break;
                }

                // delay to let write get through servers
                yield return new WaitForSecondsRealtime(kPostWriteDelay);

                if (!worthRetry)
                    break;
            }

            fs.Close();
        }

        private IEnumerator uploadFileComponentVerifyRecord(ApiFile apiFile, ApiFile.Version.FileDescriptor.Type fileDescriptorType, string filename, string md5Base64, long fileSize, ApiFile.Version.FileDescriptor fileDesc, Action<ApiFile> onSuccess, Action<string> onError, Action<long, long> onProgess, FileOpCancelQuery cancelQuery)
        {
            OnFileOpError onCancelFunc = delegate (ApiFile file, string s)
            {
                if (onError != null)
                    onError(s);
            };

            float initialStartTime = Time.realtimeSinceStartup;
            float startTime = initialStartTime;
            float timeout = GetServerProcessingWaitTimeoutForDataSize(fileDesc.sizeInBytes);
            float waitDelay = SERVER_PROCESSING_INITIAL_RETRY_TIME;
            float maxDelay = SERVER_PROCESSING_MAX_RETRY_TIME;

            while (true)
            {
                if (apiFile == null)
                {
                    if (onError != null)
                        onError("ApiFile is null");
                    yield break;
                }

                var desc = apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), fileDescriptorType);
                if (desc == null)
                {
                    if (onError != null)
                        onError("File descriptor is null ('" + fileDescriptorType + "')");
                    yield break;
                }

                if (desc.status != ApiFile.Status.Waiting)
                {
                    // upload completed or is processing
                    break;
                }

                // wait for next poll 
                while (Time.realtimeSinceStartup - startTime < waitDelay)
                {
                    if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                    {
                        yield break;
                    }

                    if (Time.realtimeSinceStartup - initialStartTime > timeout)
                    {
                        if (onError != null)
                            onError("Couldn't verify upload status: Timed out wait for server processing");
                        yield break;
                    }

                    yield return null;
                }


                while (true)
                {
                    bool wait = true;
                    string errorStr = "";
                    bool worthRetry = false;

                    System.Action<ApiContainer> RefreshFail = (ApiContainer c) =>
                    {
                        errorStr = "Failed to finish upload: " + c.Error;
                        wait = false;
                        if (c.Code == 400)
                            worthRetry = true;
                    };

                    System.Action<ApiContainer> RefreshSuccess = (ApiContainer c) =>
                    {
                        apiFile = c.Model as ApiFile;
                        wait = false;
                    };

                    apiFile.Refresh(RefreshSuccess, RefreshFail);

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                        {
                            yield break;
                        }

                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        if (onError != null)
                            onError(errorStr);
                        if (!worthRetry)
                            yield break;
                    }

                    if (!worthRetry)
                        break;
                }

                waitDelay = Mathf.Min(waitDelay * 2, maxDelay);
                startTime = Time.realtimeSinceStartup;
            }

            if (onSuccess != null)
                onSuccess(apiFile);
        }

        private IEnumerator UploadFileComponentInternal(ApiFile apiFile, ApiFile.Version.FileDescriptor.Type fileDescriptorType, string filename, string md5Base64, long fileSize, Action<ApiFile> onSuccess, Action<string> onError, Action<long, long> onProgess, FileOpCancelQuery cancelQuery)
        {
            Debug.Log("UploadFileComponent: " + fileDescriptorType + " (" + apiFile.id + "): " + filename);
            ApiFile.Version.FileDescriptor fileDesc = apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), fileDescriptorType);

            if (!uploadFileComponentValidateFileDesc(apiFile, filename, md5Base64, fileSize, fileDesc, onSuccess, onError))
                yield break;

            switch (fileDesc.category)
            {
                case ApiFile.Category.Simple:
                    yield return uploadFileComponentDoSimpleUpload(apiFile, fileDescriptorType, filename, md5Base64, fileSize, onSuccess, onError, onProgess, cancelQuery);
                    break;
                case ApiFile.Category.Multipart:
                    yield return uploadFileComponentDoMultipartUpload(apiFile, fileDescriptorType, filename, md5Base64, fileSize, onSuccess, onError, onProgess, cancelQuery);
                    break;
                default:
                    if (onError != null)
                        onError("Unknown file category type: " + fileDesc.category);
                    yield break;
            }

            yield return uploadFileComponentVerifyRecord(apiFile, fileDescriptorType, filename, md5Base64, fileSize, fileDesc, onSuccess, onError, onProgess, cancelQuery);
        }
    }
}

namespace PendulumClient.AssetUploading
{
    public class ApiFileHelper : MonoBehaviour
    {
        private readonly int kMultipartUploadChunkSize = 100 * 1024 * 1024; // 100 MB
        private readonly int SERVER_PROCESSING_WAIT_TIMEOUT_CHUNK_SIZE = 50 * 1024 * 1024;
        private readonly float SERVER_PROCESSING_WAIT_TIMEOUT_PER_CHUNK_SIZE = 120.0f;
        private readonly float SERVER_PROCESSING_MAX_WAIT_TIMEOUT = 600.0f;
        private readonly float SERVER_PROCESSING_INITIAL_RETRY_TIME = 2.0f;
        private readonly float SERVER_PROCESSING_MAX_RETRY_TIME = 10.0f;

        private static bool EnableDeltaCompression = false;

        private readonly Regex[] kUnityPackageAssetNameFilters = new Regex[]
        {
            new Regex(@"/LightingData\.asset$"),                    // lightmap base asset
            new Regex(@"/Lightmap-.*(\.png|\.exr)$"),               // lightmaps
            new Regex(@"/ReflectionProbe-.*(\.exr|\.png)$"),        // reflection probes
            new Regex(@"/Editor/Data/UnityExtensions/")             // anything that looks like part of the Unity installation
        };

        public delegate void OnFileOpSuccess(ApiFile apiFile, string message);
        public delegate void OnFileOpError(ApiFile apiFile, string error);
        public delegate void OnFileOpProgress(ApiFile apiFile, string status, string subStatus, float pct);
        public delegate bool FileOpCancelQuery(ApiFile apiFile);

        public static ApiFileHelper Instance
        {
            get
            {
                CheckInstance();
                return mInstance;
            }
        }

        private static ApiFileHelper mInstance = null;
        const float kPostWriteDelay = 0.75f;

        public System.Delegate ReferencedDelegate;

        public System.IntPtr MethodInfo;

        public Il2CppSystem.Collections.Generic.List<MonoBehaviour> AntiGcList;

        public ApiFileHelper(System.IntPtr intptr_1) : base(intptr_1)
        {
            AntiGcList = new Il2CppSystem.Collections.Generic.List<MonoBehaviour>(1);
            AntiGcList.Add(this);
        }

        public ApiFileHelper(System.Delegate delegate_1, System.IntPtr intptr_1) : base(ClassInjector.DerivedConstructorPointer<ApiFileHelper>())
        {
            ClassInjector.DerivedConstructorBody(this);
            ReferencedDelegate = delegate_1;
            MethodInfo = intptr_1;
        }

        ~ApiFileHelper()
        {
            Marshal.FreeHGlobal(MethodInfo);
            MethodInfo = Il2CppSystem.IntPtr.Zero;
            ReferencedDelegate = null;
            AntiGcList.Remove(this);
            AntiGcList = null;
        }

        public enum FileOpResult
        {
            Success,
            Unchanged
        }

        public static void upload(string FilePath, string record, string AssetBundle, OnFileOpSuccess success, OnFileOpError failure, OnFileOpProgress filecheck, FileOpCancelQuery cancelled)
        {
            try
            {
                string extension = Path.GetExtension(FilePath);
                PendulumLogger.Log("Extension: " + extension);
            }
            catch
            { }
            MelonCoroutines.Start(Instance.UploadFile(FilePath, record, AssetBundle, success, failure, filecheck, cancelled));
        }

        public static string GetMimeTypeFromExtension(string extension)
        {
            if (extension == ".vrcw")
                return "application/x-world";
            if (extension == ".vrca")
                return "application/x-avatar";
            if (extension == ".dll")
                return "application/x-msdownload";
            if (extension == ".unitypackage")
                return "application/gzip";
            if (extension == ".gz")
                return "application/gzip";
            if (extension == ".jpg")
                return "image/jpg";
            if (extension == ".png")
                return "image/png";
            if (extension == ".sig")
                return "application/x-rsync-signature";
            if (extension == ".delta")
                return "application/x-rsync-delta";

            Debug.LogWarning("Unknown file extension for mime-type: " + extension);
            return "application/octet-stream";
        }

        public static bool IsGZipCompressed(string filename)
        {
            return GetMimeTypeFromExtension(Path.GetExtension(filename)) == "application/gzip";
        }

        public IEnumerator UploadFile(string filename, string existingFileId, string friendlyName,
            OnFileOpSuccess onSuccess, OnFileOpError onError, OnFileOpProgress onProgress, FileOpCancelQuery cancelQuery)
        {
            VRC.Core.Logger.Log("UploadFile: filename: " + filename + ", file id: " +
                       (!string.IsNullOrEmpty(existingFileId) ? existingFileId : "<new>") + ", name: " + friendlyName, DebugLevel.All);

            // init remote config
            if (!ConfigManager.RemoteConfig.IsInitialized())
            {
                bool done = false;
                ConfigManager.RemoteConfig.Init(
                    (System.Action)delegate { done = true; },
                    (System.Action)delegate { done = true; }
                );

                while (!done)
                    yield return null;

                if (!ConfigManager.RemoteConfig.IsInitialized())
                {
                    Error(onError, null, "Failed to fetch configuration.");
                    yield break;
                }
            }

            // configure delta compression
            {
                EnableDeltaCompression = ConfigManager.RemoteConfig.GetBool("sdkEnableDeltaCompression", false);
            }

            // validate input file
            Progress(onProgress, null, "Checking file...");

            if (string.IsNullOrEmpty(filename))
            {
                Error(onError, null, "Upload filename is empty!");
                yield break;
            }

            if (!System.IO.Path.HasExtension(filename))
            {
                Error(onError, null, "Upload filename must have an extension: " + filename);
                yield break;
            }

            string whyNot;
            if (!VRC.Tools.FileCanRead(filename, out whyNot))
            {
                Error(onError, null, "Could not read file to upload!", filename + "\n" + whyNot);
                yield break;
            }

            // get or create ApiFile
            Progress(onProgress, null, string.IsNullOrEmpty(existingFileId) ? "Creating file record..." : "Getting file record...");

            bool wait = true;
            bool wasError = false;
            bool worthRetry = false;
            string errorStr = "";

            if (string.IsNullOrEmpty(friendlyName))
                friendlyName = filename;

            string extension = System.IO.Path.GetExtension(filename);
            string mimeType = GetMimeTypeFromExtension(extension);

            ApiFile apiFile = null;

            System.Action<ApiContainer> fileSuccess = (ApiContainer c) =>
            {
                apiFile = c.Model as ApiFile;
                wait = false;
            };

            System.Action<ApiContainer> fileFailure = (ApiContainer c) =>
            {
                errorStr = c.Error;
                wait = false;

                if (c.Code == 400)
                    worthRetry = true;
            };

            while (true)
            {
                apiFile = null;
                wait = true;
                worthRetry = false;
                errorStr = "";

                if (string.IsNullOrEmpty(existingFileId))
                    ApiFile.Create(friendlyName, mimeType, extension, fileSuccess, fileFailure);
                else
                    API.Fetch<ApiFile>(existingFileId, fileSuccess, fileFailure);

                while (wait)
                {
                    if (apiFile != null && CheckCancelled(cancelQuery, onError, apiFile))
                        yield break;

                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    if (errorStr.Contains("File not found"))
                    {
                        Debug.LogError("Couldn't find file record: " + existingFileId + ", creating new file record");

                        existingFileId = "";
                        continue;
                    }

                    string msg = string.IsNullOrEmpty(existingFileId) ? "Failed to create file record." : "Failed to get file record.";
                    Error(onError, null, msg, errorStr);

                    if (!worthRetry)
                        yield break;
                }

                if (!worthRetry)
                    break;
                else
                    yield return new WaitForSecondsRealtime(kPostWriteDelay);
            }

            if (apiFile == null)
                yield break;

            LogApiFileStatus(apiFile, false, true);

            while (apiFile.HasQueuedOperation(EnableDeltaCompression))
            {
                wait = true;

                apiFile.DeleteLatestVersion((System.Action<ApiContainer>)delegate { wait = false; }, (System.Action<ApiContainer>)delegate { wait = false; });

                while (wait)
                {
                    if (apiFile != null && CheckCancelled(cancelQuery, onError, apiFile))
                        yield break;

                    yield return null;
                }
            }

            // delay to let write get through servers
            yield return new WaitForSecondsRealtime(kPostWriteDelay);

            LogApiFileStatus(apiFile, false);

            // check for server side errors from last upload
            if (apiFile.IsInErrorState())
            {
                Debug.LogWarning("ApiFile: " + apiFile.id + ": server failed to process last uploaded, deleting failed version");

                while (true)
                {
                    // delete previous failed version
                    Progress(onProgress, apiFile, "Preparing file for upload...", "Cleaning up previous version");

                    wait = true;
                    errorStr = "";
                    worthRetry = false;

                    apiFile.DeleteLatestVersion(fileSuccess, fileFailure);

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onError, null))
                        {
                            yield break;
                        }

                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        Error(onError, apiFile, "Failed to delete previous failed version!", errorStr);
                        if (!worthRetry)
                        {
                            CleanupTempFiles(apiFile.id);
                            yield break;
                        }
                    }

                    if (worthRetry)
                        yield return new WaitForSecondsRealtime(kPostWriteDelay);
                    else
                        break;
                }
            }

            // delay to let write get through servers
            yield return new WaitForSecondsRealtime(kPostWriteDelay);

            LogApiFileStatus(apiFile, false);

            // verify previous file op is complete
            if (apiFile.HasQueuedOperation(EnableDeltaCompression))
            {
                Error(onError, apiFile, "A previous upload is still being processed. Please try again later.");
                yield break;
            }

            if (wasError)
                yield break;

            LogApiFileStatus(apiFile, false);

            // generate md5 and check if file has changed
            Progress(onProgress, apiFile, "Preparing file for upload...", "Generating file hash");

            string fileMD5Base64 = "";
            wait = true;
            errorStr = "";
            VRC.Tools.FileMD5(filename,
                (System.Action<Il2CppStructArray<byte>>)delegate (Il2CppStructArray<byte> md5Bytes)
                {
                    fileMD5Base64 = Convert.ToBase64String(md5Bytes);
                    wait = false;
                },
                (System.Action<string>)delegate (string error)
                {
                    errorStr = filename + "\n" + error;
                    wait = false;
                }
            );

            while (wait)
            {
                if (CheckCancelled(cancelQuery, onError, apiFile))
                {
                    CleanupTempFiles(apiFile.id);
                    yield break;
                }
                yield return null;
            }

            if (!string.IsNullOrEmpty(errorStr))
            {
                Error(onError, apiFile, "Failed to generate MD5 hash for upload file.", errorStr);
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            LogApiFileStatus(apiFile, false);

            // check if file has been changed
            Progress(onProgress, apiFile, "Preparing file for upload...", "Checking for changes");

            bool isPreviousUploadRetry = false;
            if (apiFile.HasExistingOrPendingVersion())
            {
                // uploading the same file?
                if (string.Compare(fileMD5Base64, apiFile.GetFileMD5(apiFile.GetLatestVersionNumber())) == 0)
                {
                    // the previous operation completed successfully?
                    if (!apiFile.IsWaitingForUpload())
                    {
                        Success(onSuccess, apiFile, "The file to upload is unchanged.");
                        CleanupTempFiles(apiFile.id);
                        yield break;
                    }
                    else
                    {
                        isPreviousUploadRetry = true;

                        Debug.Log("Retrying previous upload");
                    }
                }
                else
                {
                    // the file has been modified
                    if (apiFile.IsWaitingForUpload())
                    {
                        // previous upload failed, and the file is changed
                        while (true)
                        {
                            // delete previous failed version
                            Progress(onProgress, apiFile, "Preparing file for upload...", "Cleaning up previous version");

                            wait = true;
                            worthRetry = false;
                            errorStr = "";

                            apiFile.DeleteLatestVersion(fileSuccess, fileFailure);

                            while (wait)
                            {
                                if (CheckCancelled(cancelQuery, onError, apiFile))
                                {
                                    yield break;
                                }
                                yield return null;
                            }

                            if (!string.IsNullOrEmpty(errorStr))
                            {
                                Error(onError, apiFile, "Failed to delete previous incomplete version!", errorStr);
                                if (!worthRetry)
                                {
                                    CleanupTempFiles(apiFile.id);
                                    yield break;
                                }
                            }

                            // delay to let write get through servers
                            yield return new WaitForSecondsRealtime(kPostWriteDelay);

                            if (!worthRetry)
                                break;
                        }
                    }
                }
            }

            LogApiFileStatus(apiFile, false);

            // generate signature for new file

            Progress(onProgress, apiFile, "Preparing file for upload...", "Generating signature");

            string signatureFilename = VRC.Tools.GetTempFileName(".sig", out errorStr, apiFile.id);
            if (string.IsNullOrEmpty(signatureFilename))
            {
                Error(onError, apiFile, "Failed to generate file signature!", "Failed to create temp file: \n" + errorStr);
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            wasError = false;
            yield return MelonCoroutines.Start(CreateFileSignatureInternal(filename, signatureFilename,
                delegate ()
                {
                    // success!
                },
                delegate (string error)
                {
                    Error(onError, apiFile, "Failed to generate file signature!", error);
                    CleanupTempFiles(apiFile.id);
                    wasError = true;
                })
            );

            if (wasError)
                yield break;

            LogApiFileStatus(apiFile, false);

            // generate signature md5 and file size
            Progress(onProgress, apiFile, "Preparing file for upload...", "Generating signature hash");

            string sigMD5Base64 = "";
            wait = true;
            errorStr = "";
            VRC.Tools.FileMD5(signatureFilename,
                (System.Action<Il2CppStructArray<byte>>)delegate (Il2CppStructArray<byte> md5Bytes)
                {
                    sigMD5Base64 = Convert.ToBase64String(md5Bytes);
                    wait = false;
                },
                (System.Action<string>)delegate (string error)
                {
                    errorStr = signatureFilename + "\n" + error;
                    wait = false;
                }
            );

            while (wait)
            {
                if (CheckCancelled(cancelQuery, onError, apiFile))
                {
                    CleanupTempFiles(apiFile.id);
                    yield break;
                }
                yield return null;
            }

            if (!string.IsNullOrEmpty(errorStr))
            {
                Error(onError, apiFile, "Failed to generate MD5 hash for signature file.", errorStr);
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            long sigFileSize = 0;
            if (!VRC.Tools.GetFileSize(signatureFilename, out sigFileSize, out errorStr))
            {
                Error(onError, apiFile, "Failed to generate file signature!", "Couldn't get file size:\n" + errorStr);
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            LogApiFileStatus(apiFile, false);

            // download previous version signature (if exists)
            string existingFileSignaturePath = null;
            if (EnableDeltaCompression && apiFile.HasExistingVersion())
            {
                Progress(onProgress, apiFile, "Preparing file for upload...", "Downloading previous version signature");

                wait = true;
                errorStr = "";
                apiFile.DownloadSignature(
                    (System.Action<Il2CppStructArray<byte>>)delegate (Il2CppStructArray<byte> data)
                    {
                        // save to temp file
                        existingFileSignaturePath = VRC.Tools.GetTempFileName(".sig", out errorStr, apiFile.id);
                        if (string.IsNullOrEmpty(existingFileSignaturePath))
                        {
                            errorStr = "Failed to create temp file: \n" + errorStr;
                            wait = false;
                        }
                        else
                        {
                            try
                            {
                                File.WriteAllBytes(existingFileSignaturePath, data);
                            }
                            catch (Exception e)
                            {
                                existingFileSignaturePath = null;
                                errorStr = "Failed to write signature temp file:\n" + e.Message;
                            }
                            wait = false;
                        }
                    },
                    (System.Action<string>)delegate (string error)
                    {
                        errorStr = error;
                        wait = false;
                    },
                    (System.Action<long, long>)delegate (long downloaded, long length)
                    {
                        Progress(onProgress, apiFile, "Preparing file for upload...", "Downloading previous version signature", Tools.DivideSafe(downloaded, length));
                    }
                );

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onError, apiFile))
                    {
                        CleanupTempFiles(apiFile.id);
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    Error(onError, apiFile, "Failed to download previous file version signature.", errorStr);
                    CleanupTempFiles(apiFile.id);
                    yield break;
                }
            }

            LogApiFileStatus(apiFile, false);

            // create delta if needed
            string deltaFilename = null;

            if (EnableDeltaCompression && !string.IsNullOrEmpty(existingFileSignaturePath))
            {
                Progress(onProgress, apiFile, "Preparing file for upload...", "Creating file delta");

                deltaFilename = VRC.Tools.GetTempFileName(".delta", out errorStr, apiFile.id);
                if (string.IsNullOrEmpty(deltaFilename))
                {
                    Error(onError, apiFile, "Failed to create file delta for upload.", "Failed to create temp file: \n" + errorStr);
                    CleanupTempFiles(apiFile.id);
                    yield break;
                }

                wasError = false;
                yield return MelonCoroutines.Start(CreateFileDeltaInternal(filename, existingFileSignaturePath, deltaFilename,
                    delegate ()
                    {
                        // success!
                    },
                    delegate (string error)
                    {
                        Error(onError, apiFile, "Failed to create file delta for upload.", error);
                        CleanupTempFiles(apiFile.id);
                        wasError = true;
                    })
                );

                if (wasError)
                    yield break;
            }

            // upload smaller of delta and new file
            long fullFileSize = 0;
            long deltaFileSize = 0;
            if (!VRC.Tools.GetFileSize(filename, out fullFileSize, out errorStr) ||
                (!string.IsNullOrEmpty(deltaFilename) && !VRC.Tools.GetFileSize(deltaFilename, out deltaFileSize, out errorStr)))
            {
                Error(onError, apiFile, "Failed to create file delta for upload.", "Couldn't get file size: " + errorStr);
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            bool uploadDeltaFile = EnableDeltaCompression && deltaFileSize > 0 && deltaFileSize < fullFileSize;
            if (EnableDeltaCompression)
                VRC.Core.Logger.Log("Delta size " + deltaFileSize + " (" + ((float)deltaFileSize / (float)fullFileSize) + " %), full file size " + fullFileSize + ", uploading " + (uploadDeltaFile ? " DELTA" : " FULL FILE"), DebugLevel.All);
            else
                VRC.Core.Logger.Log("Delta compression disabled, uploading FULL FILE, size " + fullFileSize, DebugLevel.All);

            LogApiFileStatus(apiFile, uploadDeltaFile);

            string deltaMD5Base64 = "";
            if (uploadDeltaFile)
            {
                Progress(onProgress, apiFile, "Preparing file for upload...", "Generating file delta hash");

                wait = true;
                errorStr = "";
                VRC.Tools.FileMD5(deltaFilename,
                    (System.Action<Il2CppStructArray<byte>>)delegate (Il2CppStructArray<byte> md5Bytes)
                    {
                        deltaMD5Base64 = Convert.ToBase64String(md5Bytes);
                        wait = false;
                    },
                    (System.Action<string>)delegate (string error)
                    {
                        errorStr = error;
                        wait = false;
                    }
                );

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onError, apiFile))
                    {
                        CleanupTempFiles(apiFile.id);
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    Error(onError, apiFile, "Failed to generate file delta hash.", errorStr);
                    CleanupTempFiles(apiFile.id);
                    yield break;
                }
            }

            // validate existing pending version info, if this is a retry
            bool versionAlreadyExists = false;

            LogApiFileStatus(apiFile, uploadDeltaFile);

            if (isPreviousUploadRetry)
            {
                bool isValid = true;

                ApiFile.Version v = apiFile.GetVersion(apiFile.GetLatestVersionNumber());
                if (v != null)
                {
                    if (uploadDeltaFile)
                    {
                        isValid = deltaFileSize == v.delta.sizeInBytes &&
                            deltaMD5Base64.CompareTo(v.delta.md5) == 0 &&
                            sigFileSize == v.signature.sizeInBytes &&
                            sigMD5Base64.CompareTo(v.signature.md5) == 0;
                    }
                    else
                    {
                        isValid = fullFileSize == v.file.sizeInBytes &&
                            fileMD5Base64.CompareTo(v.file.md5) == 0 &&
                            sigFileSize == v.signature.sizeInBytes &&
                            sigMD5Base64.CompareTo(v.signature.md5) == 0;
                    }
                }
                else
                {
                    isValid = false;
                }

                if (isValid)
                {
                    versionAlreadyExists = true;

                    Debug.Log("Using existing version record");
                }
                else
                {
                    // delete previous invalid version
                    Progress(onProgress, apiFile, "Preparing file for upload...", "Cleaning up previous version");

                    while (true)
                    {
                        wait = true;
                        errorStr = "";
                        worthRetry = false;

                        apiFile.DeleteLatestVersion(fileSuccess, fileFailure);

                        while (wait)
                        {
                            if (CheckCancelled(cancelQuery, onError, null))
                            {
                                yield break;
                            }
                            yield return null;
                        }

                        if (!string.IsNullOrEmpty(errorStr))
                        {
                            Error(onError, apiFile, "Failed to delete previous incomplete version!", errorStr);
                            if (!worthRetry)
                            {
                                CleanupTempFiles(apiFile.id);
                                yield break;
                            }
                        }

                        // delay to let write get through servers
                        yield return new WaitForSecondsRealtime(kPostWriteDelay);

                        if (!worthRetry)
                            break;
                    }
                }
            }

            LogApiFileStatus(apiFile, uploadDeltaFile);

            // create new version of file
            if (!versionAlreadyExists)
            {
                while (true)
                {
                    Progress(onProgress, apiFile, "Creating file version record...");

                    wait = true;
                    errorStr = "";
                    worthRetry = false;

                    if (uploadDeltaFile)
                        // delta file
                        apiFile.CreateNewVersion(ApiFile.Version.FileType.Delta, deltaMD5Base64, deltaFileSize, sigMD5Base64, sigFileSize, fileSuccess, fileFailure);
                    else
                        // full file
                        apiFile.CreateNewVersion(ApiFile.Version.FileType.Full, fileMD5Base64, fullFileSize, sigMD5Base64, sigFileSize, fileSuccess, fileFailure);

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onError, apiFile))
                        {
                            CleanupTempFiles(apiFile.id);
                            yield break;
                        }

                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        Error(onError, apiFile, "Failed to create file version record.", errorStr);
                        if (!worthRetry)
                        {
                            CleanupTempFiles(apiFile.id);
                            yield break;
                        }
                    }

                    // delay to let write get through servers
                    yield return new WaitForSecondsRealtime(kPostWriteDelay);

                    if (!worthRetry)
                        break;
                }
            }

            // upload components

            LogApiFileStatus(apiFile, uploadDeltaFile);

            // upload delta
            if (uploadDeltaFile)
            {
                if (apiFile.GetLatestVersion().delta.status == ApiFile.Status.Waiting)
                {
                    Progress(onProgress, apiFile, "Uploading file delta...");

                    wasError = false;
                    yield return MelonCoroutines.Start(UploadFileComponentInternal(apiFile,
                        ApiFile.Version.FileDescriptor.Type.delta, deltaFilename, deltaMD5Base64, deltaFileSize,
                        delegate (ApiFile file)
                        {
                            Debug.Log("Successfully uploaded file delta.");
                            apiFile = file;
                        },
                        delegate (string error)
                        {
                            Error(onError, apiFile, "Failed to upload file delta.", error);
                            CleanupTempFiles(apiFile.id);
                            wasError = true;
                        },
                        delegate (long downloaded, long length)
                        {
                            Progress(onProgress, apiFile, "Uploading file delta...", "", Tools.DivideSafe(downloaded, length));
                        },
                        cancelQuery)
                    );

                    if (wasError)
                        yield break;
                }
            }
            // upload file
            else
            {
                if (apiFile.GetLatestVersion().file.status == ApiFile.Status.Waiting)
                {
                    Progress(onProgress, apiFile, "Uploading file...");

                    wasError = false;
                    yield return MelonCoroutines.Start(UploadFileComponentInternal(apiFile,
                        ApiFile.Version.FileDescriptor.Type.file, filename, fileMD5Base64, fullFileSize,
                        delegate (ApiFile file)
                        {
                            VRC.Core.Logger.Log("Successfully uploaded file.", DebugLevel.All);
                            apiFile = file;
                        },
                        delegate (string error)
                        {
                            Error(onError, apiFile, "Failed to upload file.", error);
                            CleanupTempFiles(apiFile.id);
                            wasError = true;
                        },
                        delegate (long downloaded, long length)
                        {
                            Progress(onProgress, apiFile, "Uploading file...", "", Tools.DivideSafe(downloaded, length));
                        },
                        cancelQuery)
                    );

                    if (wasError)
                        yield break;
                }
            }

            LogApiFileStatus(apiFile, uploadDeltaFile);

            // upload signature
            if (apiFile.GetLatestVersion().signature.status == ApiFile.Status.Waiting)
            {
                Progress(onProgress, apiFile, "Uploading file signature...");

                wasError = false;
                yield return MelonCoroutines.Start(UploadFileComponentInternal(apiFile,
                    ApiFile.Version.FileDescriptor.Type.signature, signatureFilename, sigMD5Base64, sigFileSize,
                    delegate (ApiFile file)
                    {
                        VRC.Core.Logger.Log("Successfully uploaded file signature.", DebugLevel.All);
                        apiFile = file;
                    },
                    delegate (string error)
                    {
                        Error(onError, apiFile, "Failed to upload file signature.", error);
                        CleanupTempFiles(apiFile.id);
                        wasError = true;
                    },
                    delegate (long downloaded, long length)
                    {
                        Progress(onProgress, apiFile, "Uploading file signature...", "", Tools.DivideSafe(downloaded, length));
                    },
                    cancelQuery)
                );

                if (wasError)
                    yield break;
            }

            LogApiFileStatus(apiFile, uploadDeltaFile);

            // Validate file records queued or complete
            Progress(onProgress, apiFile, "Validating upload...");

            bool isUploadComplete = (uploadDeltaFile
                ? apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.delta).status == ApiFile.Status.Complete
                : apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.file).status == ApiFile.Status.Complete);
            isUploadComplete = isUploadComplete &&
                               apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.signature).status == ApiFile.Status.Complete;

            if (!isUploadComplete)
            {
                Error(onError, apiFile, "Failed to upload file.", "Record status is not 'complete'");
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            bool isServerOpQueuedOrComplete = (uploadDeltaFile
                ? apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.file).status != ApiFile.Status.Waiting
                : apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.delta).status != ApiFile.Status.Waiting);

            if (!isServerOpQueuedOrComplete)
            {
                Error(onError, apiFile, "Failed to upload file.", "Record is still in 'waiting' status");
                CleanupTempFiles(apiFile.id);
                yield break;
            }

            LogApiFileStatus(apiFile, uploadDeltaFile);

            // wait for server processing to complete
            Progress(onProgress, apiFile, "Processing upload...");
            float checkDelay = SERVER_PROCESSING_INITIAL_RETRY_TIME;
            float maxDelay = SERVER_PROCESSING_MAX_RETRY_TIME;
            float timeout = GetServerProcessingWaitTimeoutForDataSize(apiFile.GetLatestVersion().file.sizeInBytes);
            double initialStartTime = Time.realtimeSinceStartup;
            double startTime = initialStartTime;
            while (apiFile.HasQueuedOperation(uploadDeltaFile))
            {
                // wait before polling again
                Progress(onProgress, apiFile, "Processing upload...", "Checking status in " + Mathf.CeilToInt(checkDelay) + " seconds");

                while (Time.realtimeSinceStartup - startTime < checkDelay)
                {
                    if (CheckCancelled(cancelQuery, onError, apiFile))
                    {
                        CleanupTempFiles(apiFile.id);
                        yield break;
                    }

                    if (Time.realtimeSinceStartup - initialStartTime > timeout)
                    {
                        LogApiFileStatus(apiFile, uploadDeltaFile);

                        Error(onError, apiFile, "Timed out waiting for upload processing to complete.");
                        CleanupTempFiles(apiFile.id);
                        yield break;
                    }

                    yield return null;
                }

                while (true)
                {
                    // check status
                    Progress(onProgress, apiFile, "Processing upload...", "Checking status...");

                    wait = true;
                    worthRetry = false;
                    errorStr = "";
                    API.Fetch<ApiFile>(apiFile.id, fileSuccess, fileFailure);

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onError, apiFile))
                        {
                            CleanupTempFiles(apiFile.id);
                            yield break;
                        }

                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        Error(onError, apiFile, "Checking upload status failed.", errorStr);
                        if (!worthRetry)
                        {
                            CleanupTempFiles(apiFile.id);
                            yield break;
                        }
                    }

                    if (!worthRetry)
                        break;
                }

                checkDelay = Mathf.Min(checkDelay * 2, maxDelay);
                startTime = Time.realtimeSinceStartup;
            }

            // cleanup and wait for it to finish
            yield return MelonCoroutines.Start(CleanupTempFilesInternal(apiFile.id));

            Success(onSuccess, apiFile, "Upload complete!");
        }

        private static void LogApiFileStatus(ApiFile apiFile, bool checkDelta, bool logSuccess = false)
        {
            if (apiFile == null || !apiFile.IsInitialized)
            {
                PendulumLogger.Log("<color=yellow>apiFile not initialized</color>");
            }
            else if (apiFile.IsInErrorState())
            {
                PendulumLogger.Log("<color=yellow>ApiFile {0} is in an error state.</color>", apiFile.name);
            }
            else if (logSuccess)
                VRC.Core.Logger.Log("< color = yellow > Processing { 3}: { 0}, { 1}, { 2}</ color > " +
                           (apiFile.IsWaitingForUpload() ? "waiting for upload" : "upload complete") +
                           (apiFile.HasExistingOrPendingVersion() ? "has existing or pending version" : "no previous version") +
                           (apiFile.IsLatestVersionQueued(checkDelta) ? "latest version queued" : "latest version not queued") +
                           apiFile.name, DebugLevel.All);

            if (apiFile != null && apiFile.IsInitialized && logSuccess)
            {
                var apiFields = apiFile.ExtractApiFields();
                if (apiFields != null)
                    VRC.Core.Logger.Log("<color=yellow>{0}</color>" + VRC.Tools.JsonEncode(apiFields), DebugLevel.All);
            }
        }

        public IEnumerator CreateFileSignatureInternal(string filename, string outputSignatureFilename, Action onSuccess, Action<string> onError)
        {
            VRC.Core.Logger.Log("CreateFileSignature: " + filename + " => " + outputSignatureFilename, DebugLevel.All);

            yield return null;

            Stream inStream = null;
            FileStream outStream = null;
            byte[] buf = new byte[64 * 1024];
            IAsyncResult asyncRead = null;
            IAsyncResult asyncWrite = null;

            try
            {
                inStream = librsync.net.Librsync.ComputeSignature(File.OpenRead(filename));
            }
            catch (Exception e)
            {
                if (onError != null)
                    onError("Couldn't open input file: " + e.Message);
                yield break;
            }

            try
            {
                outStream = File.Open(outputSignatureFilename, FileMode.Create, FileAccess.Write);
            }
            catch (Exception e)
            {
                if (onError != null)
                    onError("Couldn't create output file: " + e.Message);
                yield break;
            }

            while (true)
            {
                try
                {
                    asyncRead = inStream.BeginRead(buf, 0, buf.Length, null, null);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't read file: " + e.Message);
                    yield break;
                }

                while (!asyncRead.IsCompleted)
                    yield return null;

                int read = 0;
                try
                {
                    read = inStream.EndRead(asyncRead);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't read file: " + e.Message);
                    yield break;
                }

                if (read <= 0)
                    break;

                try
                {
                    asyncWrite = outStream.BeginWrite(buf, 0, read, null, null);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't write file: " + e.Message);
                    yield break;
                }

                while (!asyncWrite.IsCompleted)
                    yield return null;

                try
                {
                    outStream.EndWrite(asyncWrite);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't write file: " + e.Message);
                    yield break;
                }
            }

            inStream.Close();
            outStream.Close();

            yield return null;

            if (onSuccess != null)
                onSuccess();
        }

        public IEnumerator CreateFileDeltaInternal(string newFilename, string existingFileSignaturePath, string outputDeltaFilename, Action onSuccess, Action<string> onError)
        {
            Debug.Log("CreateFileDelta: " + newFilename + " (delta) " + existingFileSignaturePath + " => " + outputDeltaFilename);

            yield return null;

            Stream inStream = null;
            FileStream outStream = null;
            byte[] buf = new byte[64 * 1024];
            IAsyncResult asyncRead = null;
            IAsyncResult asyncWrite = null;

            try
            {
                inStream = librsync.net.Librsync.ComputeDelta(File.OpenRead(existingFileSignaturePath), File.OpenRead(newFilename));
            }
            catch (Exception e)
            {
                if (onError != null)
                    onError("Couldn't open input file: " + e.Message);
                yield break;
            }

            try
            {
                outStream = File.Open(outputDeltaFilename, FileMode.Create, FileAccess.Write);
            }
            catch (Exception e)
            {
                if (onError != null)
                    onError("Couldn't create output file: " + e.Message);
                yield break;
            }

            while (true)
            {
                try
                {
                    asyncRead = inStream.BeginRead(buf, 0, buf.Length, null, null);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't read file: " + e.Message);
                    yield break;
                }

                while (!asyncRead.IsCompleted)
                    yield return null;

                int read = 0;
                try
                {
                    read = inStream.EndRead(asyncRead);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't read file: " + e.Message);
                    yield break;
                }

                if (read <= 0)
                    break;

                try
                {
                    asyncWrite = outStream.BeginWrite(buf, 0, read, null, null);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't write file: " + e.Message);
                    yield break;
                }

                while (!asyncWrite.IsCompleted)
                    yield return null;

                try
                {
                    outStream.EndWrite(asyncWrite);
                }
                catch (Exception e)
                {
                    if (onError != null)
                        onError("Couldn't write file: " + e.Message);
                    yield break;
                }
            }

            inStream.Close();
            outStream.Close();

            yield return null;

            if (onSuccess != null)
                onSuccess();
        }

        protected static void Success(OnFileOpSuccess onSuccess, ApiFile apiFile, string message)
        {
            if (apiFile == null)
                apiFile = new ApiFile();

            VRC.Core.Logger.Log("ApiFile " + apiFile.ToStringBrief() + ": Operation Succeeded!", DebugLevel.All);
            if (onSuccess != null)
                onSuccess(apiFile, message);
        }

        protected static void Error(OnFileOpError onError, ApiFile apiFile, string error, string moreInfo = "")
        {
            if (apiFile == null)
                apiFile = new ApiFile();

            Debug.LogError("ApiFile " + apiFile.ToStringBrief() + ": Error: " + error + "\n" + moreInfo);
            if (onError != null)
                onError(apiFile, error);
        }

        protected static void Progress(OnFileOpProgress onProgress, ApiFile apiFile, string status, string subStatus = "", float pct = 0.0f)
        {
            if (apiFile == null)
                apiFile = new ApiFile();

            if (onProgress != null)
                onProgress(apiFile, status, subStatus, pct);
        }

        protected static bool CheckCancelled(FileOpCancelQuery cancelQuery, OnFileOpError onError, ApiFile apiFile)
        {
            if (apiFile == null)
            {
                Debug.LogError("apiFile was null");
                return true;
            }

            if (cancelQuery != null && cancelQuery(apiFile))
            {
                Debug.Log("ApiFile " + apiFile.ToStringBrief() + ": Operation cancelled");
                if (onError != null)
                    onError(apiFile, "Cancelled by user.");
                return true;
            }

            return false;
        }

        protected static void CleanupTempFiles(string subFolderName)
        {
            MelonCoroutines.Start(Instance.CleanupTempFilesInternal(subFolderName));
        }

        protected IEnumerator CleanupTempFilesInternal(string subFolderName)
        {
            if (!string.IsNullOrEmpty(subFolderName))
            {
                string folder = VRC.Tools.GetTempFolderPath(subFolderName);

                while (Directory.Exists(folder))
                {
                    try
                    {
                        if (Directory.Exists(folder))
                            Directory.Delete(folder, true);
                    }
                    catch (System.Exception)
                    {
                    }

                    yield return null;
                }
            }
        }

        private static void CheckInstance()
        {
            if (mInstance == null)
            {
                GameObject go = new GameObject("ApiFileHelper");
                mInstance = go.AddComponent<ApiFileHelper>();

                try
                {
                    GameObject.DontDestroyOnLoad(go);
                }
                catch
                {
                }
            }
        }

        private float GetServerProcessingWaitTimeoutForDataSize(int size)
        {
            float timeoutMultiplier = Mathf.Ceil((float)size / (float)SERVER_PROCESSING_WAIT_TIMEOUT_CHUNK_SIZE);
            return Mathf.Clamp(timeoutMultiplier * SERVER_PROCESSING_WAIT_TIMEOUT_PER_CHUNK_SIZE, SERVER_PROCESSING_WAIT_TIMEOUT_PER_CHUNK_SIZE, SERVER_PROCESSING_MAX_WAIT_TIMEOUT);
        }

        private bool UploadFileComponentValidateFileDesc(ApiFile apiFile, string filename, string md5Base64, long fileSize, ApiFile.Version.FileDescriptor fileDesc, Action<ApiFile> onSuccess, Action<string> onError)
        {
            if (fileDesc.status != ApiFile.Status.Waiting)
            {
                // nothing to do (might be a retry)
                Debug.Log("UploadFileComponent: (file record not in waiting status, done)");
                if (onSuccess != null)
                    onSuccess(apiFile);
                return false;
            }

            if (fileSize != fileDesc.sizeInBytes)
            {
                if (onError != null)
                    onError("File size does not match version descriptor");
                return false;
            }
            if (string.Compare(md5Base64, fileDesc.md5) != 0)
            {
                if (onError != null)
                    onError("File MD5 does not match version descriptor");
                return false;
            }

            // make sure file is right size
            long tempSize = 0;
            string errorStr = "";
            if (!VRC.Tools.GetFileSize(filename, out tempSize, out errorStr))
            {
                if (onError != null)
                    onError("Couldn't get file size");
                return false;
            }
            if (tempSize != fileSize)
            {
                if (onError != null)
                    onError("File size does not match input size");
                return false;
            }

            return true;
        }

        private IEnumerator UploadFileComponentDoSimpleUpload(ApiFile apiFile,
            ApiFile.Version.FileDescriptor.Type fileDescriptorType,
            string filename,
            string md5Base64,
            long fileSize,
            Action<ApiFile> onSuccess,
            Action<string> onError,
            Action<long, long> onProgress,
            FileOpCancelQuery cancelQuery)
        {
            OnFileOpError onCancelFunc = delegate (ApiFile file, string s)
            {
                if (onError != null)
                    onError(s);
            };

            string uploadUrl = "";
            while (true)
            {
                bool wait = true;
                string errorStr = "";
                bool worthRetry = false;

                apiFile.StartSimpleUpload(fileDescriptorType,
                    (System.Action<ApiContainer>)delegate (ApiContainer c)
                    {
                        //uploadUrl = (c as ApiDictContainer).ResponseDictionary["url"].ToString();
                        wait = false;
                    },
                    (System.Action<ApiContainer>)delegate (ApiContainer c)
                    {
                        errorStr = "Failed to start upload: " + c.Error;
                        wait = false;
                        if (c.Code == 400)
                            worthRetry = true;
                    });

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                    {
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    if (onError != null)
                        onError(errorStr);
                    if (!worthRetry)
                        yield break;
                }

                // delay to let write get through servers
                yield return new WaitForSecondsRealtime(kPostWriteDelay);

                if (!worthRetry)
                    break;
            }

            // PUT file
            {
                bool wait = true;
                string errorStr = "";

                VRC.HttpRequest req = ApiFile.PutSimpleFileToURL(uploadUrl, filename, GetMimeTypeFromExtension(Path.GetExtension(filename)), md5Base64, false,
                    (System.Action)delegate ()
                    {
                        wait = false;
                    },
                    (System.Action<string>)delegate (string error)
                    {
                        errorStr = "Failed to upload file: " + error;
                        wait = false;
                    },
                    (System.Action<long, long>)delegate (long uploaded, long length)
                    {
                        if (onProgress != null)
                            onProgress(uploaded, length);
                    }
                );

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                    {
                        if (req != null)
                            req.Abort();
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    if (onError != null)
                        onError(errorStr);
                    yield break;
                }
            }

            // finish upload
            while (true)
            {
                // delay to let write get through servers
                yield return new WaitForSecondsRealtime(kPostWriteDelay);

                bool wait = true;
                string errorStr = "";
                bool worthRetry = false;

                apiFile.FinishUpload(fileDescriptorType, null,
                    (System.Action<ApiContainer>)delegate (ApiContainer c)
                    {
                        apiFile = c.Model as ApiFile;
                        wait = false;
                    },
                    (System.Action<ApiContainer>)delegate (ApiContainer c)
                    {
                        errorStr = "Failed to finish upload: " + c.Error;
                        wait = false;
                        if (c.Code == 400)
                            worthRetry = false;
                    });

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                    {
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    if (onError != null)
                        onError(errorStr);
                    if (!worthRetry)
                        yield break;
                }

                // delay to let write get through servers
                yield return new WaitForSecondsRealtime(kPostWriteDelay);

                if (!worthRetry)
                    break;
            }

        }

        private IEnumerator UploadFileComponentDoMultipartUpload(ApiFile apiFile,
            ApiFile.Version.FileDescriptor.Type fileDescriptorType,
            string filename,
            string md5Base64,
            long fileSize,
            Action<ApiFile> onSuccess,
            Action<string> onError,
            Action<long, long> onProgress,
            FileOpCancelQuery cancelQuery)
        {
            FileStream fs = null;
            OnFileOpError onCancelFunc = delegate (ApiFile file, string s)
            {
                if (fs != null)
                    fs.Close();
                if (onError != null)
                    onError(s);
            };

            // query multipart upload status.
            // we might be resuming a previous upload
            ApiFile.UploadStatus uploadStatus = null;
            {
                while (true)
                {
                    bool wait = true;
                    string errorStr = "";
                    bool worthRetry = false;

                    apiFile.GetUploadStatus(apiFile.GetLatestVersionNumber(), fileDescriptorType,
                       (System.Action<ApiContainer>)delegate (ApiContainer c)
                       {
                           uploadStatus = c.Model as ApiFile.UploadStatus;
                           wait = false;

                           VRC.Core.Logger.Log("Found existing multipart upload status (next part = " + uploadStatus.nextPartNumber + ")", DebugLevel.All);
                       },
                       (System.Action<ApiContainer>)delegate (ApiContainer c)
                       {
                           errorStr = "Failed to query multipart upload status: " + c.Error;
                           wait = false;
                           if (c.Code == 400)
                               worthRetry = true;
                       });

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                        {
                            yield break;
                        }
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        if (onError != null)
                            onError(errorStr);
                        if (!worthRetry)
                            yield break;
                    }

                    if (!worthRetry)
                        break;
                }
            }

            // split file into chunks
            try
            {
                fs = File.OpenRead(filename);
            }
            catch (Exception e)
            {
                if (onError != null)
                    onError("Couldn't open file: " + e.Message);
                yield break;
            }

            byte[] buffer = new byte[kMultipartUploadChunkSize * 2];

            long totalBytesUploaded = 0;
            List<string> etags = new List<string>();
            if (uploadStatus != null)
                etags = uploadStatus.etags;

            int numParts = Mathf.Max(1, Mathf.FloorToInt((float)fs.Length / (float)kMultipartUploadChunkSize));
            for (int partNumber = 1; partNumber <= numParts; partNumber++)
            {
                // read chunk
                int bytesToRead = partNumber < numParts ? kMultipartUploadChunkSize : (int)(fs.Length - fs.Position);
                int bytesRead = 0;
                try
                {
                    bytesRead = fs.Read(buffer, 0, bytesToRead);
                }
                catch (Exception e)
                {
                    fs.Close();
                    if (onError != null)
                        onError("Couldn't read file: " + e.Message);
                    yield break;
                }

                if (bytesRead != bytesToRead)
                {
                    fs.Close();
                    if (onError != null)
                        onError("Couldn't read file: read incorrect number of bytes from stream");
                    yield break;
                }

                // check if this part has been upload already
                // NOTE: uploadStatus.nextPartNumber == number of parts already uploaded
                if (uploadStatus != null && partNumber <= uploadStatus.nextPartNumber)
                {
                    totalBytesUploaded += bytesRead;
                    continue;
                }

                // start upload
                string uploadUrl = "";

                while (true)
                {
                    bool wait = true;
                    string errorStr = "";
                    bool worthRetry = false;

                    apiFile.StartMultipartUpload(fileDescriptorType, partNumber,
                        (System.Action<ApiContainer>)delegate (ApiContainer c)
                        {
                            //uploadUrl = (c as ApiDictContainer).ResponseDictionary["url"] as Il2CppSystem.String;
                            wait = false;
                        },
                        (System.Action<ApiContainer>)delegate (ApiContainer c)
                        {
                            errorStr = "Failed to start part upload: " + c.Error;
                            wait = false;
                        });

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                        {
                            yield break;
                        }
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        fs.Close();
                        if (onError != null)
                            onError(errorStr);
                        if (!worthRetry)
                            yield break;
                    }

                    // delay to let write get through servers
                    yield return new WaitForSecondsRealtime(kPostWriteDelay);

                    if (!worthRetry)
                        break;
                }

                // PUT file part
                {
                    bool wait = true;
                    string errorStr = "";

                    VRC.HttpRequest req = ApiFile.PutMultipartDataToURL(uploadUrl, buffer, bytesRead, GetMimeTypeFromExtension(Path.GetExtension(filename)), false,
                        (System.Action<string>)delegate (string etag)
                        {
                            if (!string.IsNullOrEmpty(etag))
                                etags.Add(etag);
                            totalBytesUploaded += bytesRead;
                            wait = false;
                        },
                        (System.Action<string>)delegate (string error)
                        {
                            errorStr = "Failed to upload data: " + error;
                            wait = false;
                        },
                        (System.Action<long, long>)delegate (long uploaded, long length)
                        {
                            if (onProgress != null)
                                onProgress(totalBytesUploaded + uploaded, fileSize);
                        }
                    );

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                        {
                            if (req != null)
                                req.Abort();
                            yield break;
                        }
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        fs.Close();
                        if (onError != null)
                            onError(errorStr);
                        yield break;
                    }
                }
            }

            // finish upload
            while (true)
            {
                // delay to let write get through servers
                yield return new WaitForSecondsRealtime(kPostWriteDelay);

                bool wait = true;
                string errorStr = "";
                bool worthRetry = false;

                apiFile.FinishUpload(fileDescriptorType, etags,
                    (System.Action<ApiContainer>)delegate (ApiContainer c)
                    {
                        apiFile = c.Model as ApiFile;
                        wait = false;
                    },
                    (System.Action<ApiContainer>)delegate (ApiContainer c)
                    {
                        errorStr = "Failed to finish upload: " + c.Error;
                        wait = false;
                        if (c.Code == 400)
                            worthRetry = true;
                    });

                while (wait)
                {
                    if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                    {
                        yield break;
                    }
                    yield return null;
                }

                if (!string.IsNullOrEmpty(errorStr))
                {
                    fs.Close();
                    if (onError != null)
                        onError(errorStr);
                    if (!worthRetry)
                        yield break;
                }

                // delay to let write get through servers
                yield return new WaitForSecondsRealtime(kPostWriteDelay);

                if (!worthRetry)
                    break;
            }

            fs.Close();
        }

        private IEnumerator UploadFileComponentVerifyRecord(ApiFile apiFile,
            ApiFile.Version.FileDescriptor.Type fileDescriptorType,
            string filename,
            string md5Base64,
            long fileSize,
            ApiFile.Version.FileDescriptor fileDesc,
            Action<ApiFile> onSuccess,
            Action<string> onError,
            Action<long, long> onProgress,
            FileOpCancelQuery cancelQuery)
        {
            OnFileOpError onCancelFunc = delegate (ApiFile file, string s)
            {
                if (onError != null)
                    onError(s);
            };

            float initialStartTime = Time.realtimeSinceStartup;
            float startTime = initialStartTime;
            float timeout = GetServerProcessingWaitTimeoutForDataSize(fileDesc.sizeInBytes);
            float waitDelay = SERVER_PROCESSING_INITIAL_RETRY_TIME;
            float maxDelay = SERVER_PROCESSING_MAX_RETRY_TIME;

            while (true)
            {
                if (apiFile == null)
                {
                    if (onError != null)
                        onError("ApiFile is null");
                    yield break;
                }

                var desc = apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), fileDescriptorType);
                if (desc == null)
                {
                    if (onError != null)
                        onError("File descriptor is null ('" + fileDescriptorType + "')");
                    yield break;
                }

                if (desc.status != ApiFile.Status.Waiting)
                {
                    // upload completed or is processing
                    break;
                }

                // wait for next poll
                while (Time.realtimeSinceStartup - startTime < waitDelay)
                {
                    if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                    {
                        yield break;
                    }

                    if (Time.realtimeSinceStartup - initialStartTime > timeout)
                    {
                        if (onError != null)
                            onError("Couldn't verify upload status: Timed out wait for server processing");
                        yield break;
                    }

                    yield return null;
                }


                while (true)
                {
                    bool wait = true;
                    string errorStr = "";
                    bool worthRetry = false;

                    apiFile.Refresh(
                        (System.Action<ApiContainer>)delegate (ApiContainer c)
                        {
                            wait = false;
                        },
                        (System.Action<ApiContainer>)delegate (ApiContainer c)
                        {
                            errorStr = "Couldn't verify upload status: " + c.Error;
                            wait = false;
                            if (c.Code == 400)
                                worthRetry = true;
                        });

                    while (wait)
                    {
                        if (CheckCancelled(cancelQuery, onCancelFunc, apiFile))
                        {
                            yield break;
                        }

                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        if (onError != null)
                            onError(errorStr);
                        if (!worthRetry)
                            yield break;
                    }

                    if (!worthRetry)
                        break;
                }

                waitDelay = Mathf.Min(waitDelay * 2, maxDelay);
                startTime = Time.realtimeSinceStartup;
            }

            if (onSuccess != null)
                onSuccess(apiFile);
        }

        private IEnumerator UploadFileComponentInternal(ApiFile apiFile,
            ApiFile.Version.FileDescriptor.Type fileDescriptorType,
            string filename,
            string md5Base64,
            long fileSize,
            Action<ApiFile> onSuccess,
            Action<string> onError,
            Action<long, long> onProgress,
            FileOpCancelQuery cancelQuery)
        {
            VRC.Core.Logger.Log("UploadFileComponent: " + fileDescriptorType + " (" + apiFile.id + "): " + filename, DebugLevel.All);
            ApiFile.Version.FileDescriptor fileDesc = apiFile.GetFileDescriptor(apiFile.GetLatestVersionNumber(), fileDescriptorType);

            if (!UploadFileComponentValidateFileDesc(apiFile, filename, md5Base64, fileSize, fileDesc, onSuccess, onError))
                yield break;

            switch (fileDesc.category)
            {
                case ApiFile.Category.Simple:
                    yield return UploadFileComponentDoSimpleUpload(apiFile, fileDescriptorType, filename, md5Base64, fileSize, onSuccess, onError, onProgress, cancelQuery);
                    break;
                case ApiFile.Category.Multipart:
                    yield return UploadFileComponentDoMultipartUpload(apiFile, fileDescriptorType, filename, md5Base64, fileSize, onSuccess, onError, onProgress, cancelQuery);
                    break;
                default:
                    if (onError != null)
                        onError("Unknown file category type: " + fileDesc.category);
                    yield break;
            }

            yield return UploadFileComponentVerifyRecord(apiFile, fileDescriptorType, filename, md5Base64, fileSize, fileDesc, onSuccess, onError, onProgress, cancelQuery);
        }
    }
}