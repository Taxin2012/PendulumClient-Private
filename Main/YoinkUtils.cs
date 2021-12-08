using DotZLib;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Tar;
using librsync.net;
using MelonLoader;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using VRC;
using VRC.Core;
using PendulumClient;

namespace Evolve.Yoink
{
    internal class YoinkUtils : MonoBehaviour
    {
        public delegate void OnSuccess(ApiFile apiFile, string message);
        public delegate void OnFailure(ApiFile apiFile, string error);
        public delegate void OnFileCheck(ApiFile apiFile, string status, string subStatus, float pct);
        public delegate bool OnCancelled(ApiFile apiFile);

        public enum GEnum0
        {
            Success,
            Unchanged
        }

        public Delegate ReferencedDelegate;
        public IntPtr MethodInfo;
        public Il2CppSystem.Collections.Generic.List<MonoBehaviour> AntiGcList;
        private readonly int int_0 = 10485760;
        private readonly int int_1 = 52428800;
        private readonly float float_0 = 120f;
        private readonly float float_1 = 600f;
        private readonly float float_2 = 2f;
        private readonly float float_3 = 10f;
        private static bool UseCompression;

        private readonly Regex[] regex_0 = new Regex[4]
        {
            new Regex("/LightingData\\.asset$"),
            new Regex("/Lightmap-.*(\\.png|\\.exr)$"),
            new Regex("/ReflectionProbe-.*(\\.exr|\\.png)$"),
            new Regex("/Editor/Data/UnityExtensions/")
        };

        private static YoinkUtils helper;
        public static RemoteConfig RemoteConfig;
        public static YoinkUtils apifilehelper
        {
            get
            {
                smethod_9();
                return helper;
            }
        }

        public YoinkUtils(IntPtr intptr_1) : base(intptr_1)
        {
            AntiGcList = new Il2CppSystem.Collections.Generic.List<MonoBehaviour>(1);
            AntiGcList.Add(this);
        }

        public YoinkUtils(Delegate RefDelegate, IntPtr Info) : base(ClassInjector.DerivedConstructorPointer<YoinkUtils>())
        {
            ClassInjector.DerivedConstructorBody(this);
            ReferencedDelegate = RefDelegate;
            MethodInfo = Info;
        }

        ~YoinkUtils()
        {
            Marshal.FreeHGlobal(MethodInfo);
            MethodInfo = Il2CppSystem.IntPtr.Zero;
            ReferencedDelegate = null;
            AntiGcList.Remove(this);
            AntiGcList = null;
        }

        public static void upload(string FilePath, string record, string AssetBundle, OnSuccess success, OnFailure failure, OnFileCheck filecheck, OnCancelled cancelled)
        {
            MelonCoroutines.Start(apifilehelper.Upload(FilePath, record, AssetBundle, success, failure, filecheck, cancelled));
        }

        public static string FileType(string DotType)
        {
            switch (DotType)
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

        public static bool IsGzip(string FileName)
        {
            return FileType(Path.GetExtension(FileName)) == "application/gzip";
        }

        public IEnumerator Upload(string Path, string record, string assetbundle, OnSuccess success, OnFailure Failure, OnFileCheck filecheck, OnCancelled Cancelled)
        {
            UseCompression = RemoteConfig.GetBool("sdkEnableDeltaCompression");
            CheckFile(filecheck, null, "Checking file...");

            if (string.IsNullOrEmpty(Path)) Failed(Failure, null, "Upload filename is empty!");
            else if (!System.IO.Path.HasExtension(Path)) Failed(Failure, null, $"Upload filename must have an extension: {Path}");
            else if (Tools.FileCanRead(Path, out string whyNot))
            {
                CheckFile(filecheck, null, string.IsNullOrEmpty(record) ? "Creating file record..." : "Getting file record...");
                bool bool_0 = true;
                bool bool_3 = false;
                bool bool_2 = false;
                string Message = "";

                if (string.IsNullOrEmpty(assetbundle)) assetbundle = Path;
                string extension = System.IO.Path.GetExtension(Path);
                string mimeType = FileType(extension);
                ApiFile ApiFile = null;
                Action<ApiContainer> action = delegate (ApiContainer ApiContainer)
                {
                    ApiFile = ApiContainer.Model.Cast<ApiFile>();
                    bool_0 = false;
                };

                Action<ApiContainer> action2 = delegate (ApiContainer apiContainer_0)
                {
                    Message = apiContainer_0.Error;
                    bool_0 = false;
                    if (apiContainer_0.Code == 400) bool_2 = true;
                };

                while (true)
                {
                    ApiFile = null;
                    bool_0 = true;
                    bool_2 = false;
                    Message = string.Empty;

                    if (string.IsNullOrEmpty(record)) ApiFile.Create(assetbundle, mimeType, extension, action, action2);
                    else API.Fetch<ApiFile>(record, action, action2);

                    while (bool_0)
                    {
                        if (ApiFile != null && YoinkUtils.Cancelled(Cancelled, Failure, ApiFile)) yield break;
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(Message))
                    {
                        if (Message.Contains("File not found"))
                        {
                            PendulumLogger.Log($"Couldn't find file record: {record}, creating new file record");
                            record = string.Empty;
                            continue;
                        }

                        string Error = (string.IsNullOrEmpty(record) ? "Failed to create file record." : "Failed to get file record.");
                        Failed(Failure, null, Error, Message);
                        if (!bool_2) yield break;
                    }
                    if (bool_2)
                    {
                        yield return new WaitForSecondsRealtime(0.75f);
                        continue;
                    }
                    if (ApiFile == null) yield break;
                    smethod_3(ApiFile, false, true);
                    break;
                }
                string string_6;
                string string_7;

                while (true)
                {
                    if (ApiFile.HasQueuedOperation(YoinkUtils.UseCompression))
                    {
                        bool_0 = true;

                        ApiFile.DeleteLatestVersion((Action<ApiContainer>)delegate
                        {
                            bool_0 = false;
                        }, (Action<ApiContainer>)delegate
                        {
                            bool_0 = false;
                        });

                        while (bool_0)
                        {
                            if (ApiFile != null && YoinkUtils.Cancelled(Cancelled, Failure, ApiFile)) yield break;
                            yield return null;
                        }
                        continue;
                    }

                    yield return new WaitForSecondsRealtime(0.75f);
                    smethod_3(ApiFile, false);

                    if (ApiFile.IsInErrorState())
                    {
                        PendulumLogger.Log("ApiFile: " + ApiFile.id + ": server failed to process last uploaded, deleting failed version");

                        while (true)
                        {
                            CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Cleaning up previous version");
                            bool_0 = true;
                            Message = string.Empty;
                            bool_2 = false;
                            ApiFile.DeleteLatestVersion(action, action2);
                            while (bool_0)
                            {
                                if (!YoinkUtils.Cancelled(Cancelled, Failure, null))
                                {
                                    yield return null;
                                    continue;
                                }
                                yield break;
                            }

                            if (!string.IsNullOrEmpty(Message))
                            {
                                Failed(Failure, ApiFile, "Failed to delete previous failed version!", Message);
                                if (!bool_2)
                                {
                                    smethod_8(ApiFile.id);
                                    yield break;
                                }
                            }

                            if (!bool_2) break;
                            yield return new WaitForSecondsRealtime(0.75f);
                        }
                    }
                    yield return new WaitForSecondsRealtime(0.75f);
                    smethod_3(ApiFile, false);
                    if (!ApiFile.HasQueuedOperation(UseCompression))
                    {
                        CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Optimizing file");
                        string_6 = Tools.GetTempFileName(System.IO.Path.GetExtension(Path), out Message, ApiFile.id);
                        if (!string.IsNullOrEmpty(string_6))
                        {
                            bool_3 = false;
                            yield return MelonCoroutines.Start(method_1(Path, string_6, delegate (GEnum0 genum0_0)
                            {
                                if (genum0_0 == GEnum0.Unchanged) string_6 = Path;
                            }, delegate (string string_4)
                            {
                                Failed(Failure, ApiFile, "Failed to optimize file for upload.", string_4);
                                smethod_8(ApiFile.id);
                                bool_3 = true;
                            }));
                            if (bool_3) break;
                            smethod_3(ApiFile, false);
                            CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Generating file hash");
                            bool_0 = true;
                            Message = string.Empty;
                            string text = Convert.ToBase64String(MD5.Create().ComputeHash(File.ReadAllBytes(string_6)));
                            bool_0 = false;
                            while (bool_0)
                            {
                                if (!YoinkUtils.Cancelled(Cancelled, Failure, ApiFile))
                                {
                                    yield return null;
                                    continue;
                                }
                                smethod_8(ApiFile.id);
                                yield break;
                            }
                            if (!string.IsNullOrEmpty(Message))
                            {
                                Failed(Failure, ApiFile, "Failed to generate MD5 hash for upload file.", Message);
                                smethod_8(ApiFile.id);
                                break;
                            }
                            smethod_3(ApiFile, false);
                            CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Checking for changes");
                            bool flag = false;
                            if (ApiFile.HasExistingOrPendingVersion())
                            {
                                if (string.Compare(text, ApiFile.GetFileMD5(ApiFile.GetLatestVersionNumber())) == 0)
                                {
                                    if (!ApiFile.IsWaitingForUpload())
                                    {
                                        Success(success, ApiFile, "The file to upload is unchanged.");
                                        smethod_8(ApiFile.id);
                                        break;
                                    }
                                    flag = true;
                                    PendulumLogger.Log("Retrying previous upload");
                                }
                                else if (ApiFile.IsWaitingForUpload())
                                {
                                    do
                                    {
                                        CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Cleaning up previous version");
                                        bool_0 = true;
                                        bool_2 = false;
                                        Message = string.Empty;
                                        ApiFile.DeleteLatestVersion(action, action2);
                                        while (bool_0)
                                        {
                                            if (!YoinkUtils.Cancelled(Cancelled, Failure, ApiFile))
                                            {
                                                yield return null;
                                                continue;
                                            }
                                            yield break;
                                        }
                                        if (!string.IsNullOrEmpty(Message))
                                        {
                                            Failed(Failure, ApiFile, "Failed to delete previous incomplete version!", Message);
                                            if (!bool_2)
                                            {
                                                smethod_8(ApiFile.id);
                                                yield break;
                                            }
                                        }
                                        yield return new WaitForSecondsRealtime(0.75f);
                                    }
                                    while (bool_2);
                                }
                            }
                            smethod_3(ApiFile, false);
                            CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Generating signature");
                            string tempFileName = Tools.GetTempFileName(".sig", out Message, ApiFile.id);
                            if (!string.IsNullOrEmpty(tempFileName))
                            {
                                bool_3 = false;
                                yield return MelonCoroutines.Start(method_2(string_6, tempFileName, delegate
                                {
                                }, delegate (string string_4)
                                {
                                    Failed(Failure, ApiFile, "Failed to generate file signature!", string_4);
                                    smethod_8(ApiFile.id);
                                    bool_3 = true;
                                }));
                                if (bool_3)
                                {
                                    break;
                                }
                                smethod_3(ApiFile, false);
                                CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Generating signature hash");
                                bool_0 = true;
                                Message = string.Empty;
                                string text2 = Convert.ToBase64String(MD5.Create().ComputeHash(File.ReadAllBytes(tempFileName)));
                                bool_0 = false;
                                while (bool_0)
                                {
                                    if (YoinkUtils.Cancelled(Cancelled, Failure, ApiFile))
                                    {
                                        smethod_8(ApiFile.id);
                                        yield break;
                                    }
                                    yield return null;
                                }
                                if (string.IsNullOrEmpty(Message))
                                {
                                    long size = 0L;
                                    if (Tools.GetFileSize(tempFileName, out size, out Message))
                                    {
                                        smethod_3(ApiFile, false);
                                        string_7 = null;
                                        if (YoinkUtils.UseCompression && ApiFile.HasExistingVersion())
                                        {
                                            CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Downloading previous version signature");
                                            bool_0 = true;
                                            Message = string.Empty;
                                            ApiFile.DownloadSignature((Action<Il2CppStructArray<byte>>)delegate (Il2CppStructArray<byte> il2CppStructArray_0)
                                            {
                                                string_7 = Tools.GetTempFileName(".sig", out Message, ApiFile.id);
                                                if (!string.IsNullOrEmpty(string_7))
                                                {
                                                    try
                                                    {
                                                        File.WriteAllBytes(string_7, il2CppStructArray_0);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        string_7 = null;
                                                        Message = "Failed to write signature temp file:\n" + ex.Message;
                                                    }
                                                    bool_0 = false;
                                                }
                                                else
                                                {
                                                    Message = "Failed to create temp file: \n" + Message;
                                                    bool_0 = false;
                                                }
                                            }, (Action<string>)delegate (string string_4)
                                            {
                                                Message = string_4;
                                                bool_0 = false;
                                            }, (Action<long, long>)delegate (long long_0, long long_1)
                                            {
                                                CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Downloading previous version signature", Tools.DivideSafe(long_0, long_1));
                                            });
                                            while (bool_0)
                                            {
                                                if (!YoinkUtils.Cancelled(Cancelled, Failure, ApiFile))
                                                {
                                                    yield return null;
                                                    continue;
                                                }
                                                smethod_8(ApiFile.id);
                                                yield break;
                                            }
                                            if (!string.IsNullOrEmpty(Message))
                                            {
                                                Failed(Failure, ApiFile, "Failed to download previous file version signature.", Message);
                                                smethod_8(ApiFile.id);
                                                break;
                                            }
                                        }
                                        smethod_3(ApiFile, false);
                                        string text3 = null;
                                        if (YoinkUtils.UseCompression && !string.IsNullOrEmpty(string_7))
                                        {
                                            CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Creating file delta");
                                            text3 = Tools.GetTempFileName(".delta", out Message, ApiFile.id);
                                            if (string.IsNullOrEmpty(text3))
                                            {
                                                Failed(Failure, ApiFile, "Failed to create file delta for upload.", "Failed to create temp file: \n" + Message);
                                                smethod_8(ApiFile.id);
                                                break;
                                            }
                                            bool_3 = false;
                                            yield return MelonCoroutines.Start(method_3(string_6, string_7, text3, delegate
                                            {
                                            }, delegate (string string_4)
                                            {
                                                Failed(Failure, ApiFile, "Failed to create file delta for upload.", string_4);
                                                smethod_8(ApiFile.id);
                                                bool_3 = true;
                                            }));
                                            if (bool_3)
                                            {
                                                break;
                                            }
                                        }
                                        long size2 = 0L;
                                        long size3 = 0L;
                                        if (Tools.GetFileSize(string_6, out size2, out Message) && (string.IsNullOrEmpty(text3) || Tools.GetFileSize(text3, out size3, out Message)))
                                        {
                                            bool flag2 = YoinkUtils.UseCompression && size3 > 0L && size3 < size2;
                                            if (YoinkUtils.UseCompression)
                                            {
                                                VRC.Core.Logger.Log("Delta size " + size3 + " (" + (float)size3 / (float)size2 + " %), full file size " + size2 + ", uploading " + (flag2 ? " DELTA" : " FULL FILE"), DebugLevel.All);
                                            }
                                            else
                                            {
                                                VRC.Core.Logger.Log("Delta compression disabled, uploading FULL FILE, size " + size2, DebugLevel.All);
                                            }
                                            smethod_3(ApiFile, flag2);
                                            string text4 = "";
                                            if (flag2)
                                            {
                                                CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Generating file delta hash");
                                                bool_0 = true;
                                                Message = string.Empty;
                                                text4 = Convert.ToBase64String(MD5.Create().ComputeHash(File.ReadAllBytes(text3)));
                                                bool_0 = false;
                                                while (bool_0)
                                                {
                                                    if (!YoinkUtils.Cancelled(Cancelled, Failure, ApiFile))
                                                    {
                                                        yield return null;
                                                        continue;
                                                    }
                                                    smethod_8(ApiFile.id);
                                                    yield break;
                                                }
                                                if (!string.IsNullOrEmpty(Message))
                                                {
                                                    Failed(Failure, ApiFile, "Failed to generate file delta hash.", Message);
                                                    smethod_8(ApiFile.id);
                                                    break;
                                                }
                                            }
                                            bool flag3 = false;
                                            smethod_3(ApiFile, flag2);
                                            if (flag)
                                            {
                                                ApiFile.Version version = ApiFile.GetVersion(ApiFile.GetLatestVersionNumber());
                                                if (version == null || !(flag2 ? (size3 == version.delta.sizeInBytes && text4.CompareTo(version.delta.md5) == 0 && size == version.signature.sizeInBytes && text2.CompareTo(version.signature.md5) == 0) : (size2 == version.file.sizeInBytes && text.CompareTo(version.file.md5) == 0 && size == version.signature.sizeInBytes && text2.CompareTo(version.signature.md5) == 0)))
                                                {
                                                    CheckFile(filecheck, ApiFile, "Preparing file for upload...", "Cleaning up previous version");
                                                    do
                                                    {
                                                        bool_0 = true;
                                                        Message = string.Empty;
                                                        bool_2 = false;
                                                        ApiFile.DeleteLatestVersion(action, action2);
                                                        while (bool_0)
                                                        {
                                                            if (!YoinkUtils.Cancelled(Cancelled, Failure, null))
                                                            {
                                                                yield return null;
                                                                continue;
                                                            }
                                                            yield break;
                                                        }
                                                        if (!string.IsNullOrEmpty(Message))
                                                        {
                                                            Failed(Failure, ApiFile, "Failed to delete previous incomplete version!", Message);
                                                            if (!bool_2)
                                                            {
                                                                smethod_8(ApiFile.id);
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
                                            smethod_3(ApiFile, flag2);

                                            if (!flag3)
                                            {
                                                do
                                                {
                                                    CheckFile(filecheck, ApiFile, "Creating file version record...");
                                                    bool_0 = true;
                                                    Message = string.Empty;
                                                    bool_2 = false;

                                                    if (!flag2) ApiFile.CreateNewVersion(ApiFile.Version.FileType.Full, text, size2, text2, size, action, action2);
                                                    else ApiFile.CreateNewVersion(ApiFile.Version.FileType.Delta, text4, size3, text2, size, action, action2);

                                                    while (bool_0)
                                                    {
                                                        if (YoinkUtils.Cancelled(Cancelled, Failure, ApiFile))
                                                        {
                                                            smethod_8(ApiFile.id);
                                                            yield break;
                                                        }
                                                        yield return null;
                                                    }
                                                    if (!string.IsNullOrEmpty(Message))
                                                    {
                                                        Failed(Failure, ApiFile, "Failed to create file version record.", Message);
                                                        if (!bool_2)
                                                        {
                                                            smethod_8(ApiFile.id);
                                                            yield break;
                                                        }
                                                    }
                                                    yield return new WaitForSecondsRealtime(0.75f);
                                                }
                                                while (bool_2);
                                            }
                                            smethod_3(ApiFile, flag2);
                                            if (!flag2)
                                            {
                                                if (ApiFile.GetLatestVersion().file.status == ApiFile.Status.Waiting)
                                                {
                                                    CheckFile(filecheck, ApiFile, "Uploading file...");
                                                    bool_3 = false;
                                                    yield return MelonCoroutines.Start(method_10(ApiFile, ApiFile.Version.FileDescriptor.Type.file, string_6, text, size2, delegate (ApiFile apiFile_1)
                                                    {
                                                        VRC.Core.Logger.Log("Successfully uploaded file.", DebugLevel.All);
                                                        ApiFile = apiFile_1;
                                                    }, delegate (string string_4)
                                                    {
                                                        Failed(Failure, ApiFile, "Failed to upload file.", string_4);
                                                        smethod_8(ApiFile.id);
                                                        bool_3 = true;
                                                    }, delegate (long long_0, long long_1)
                                                    {
                                                        CheckFile(filecheck, ApiFile, "Uploading file...", "", Tools.DivideSafe(long_0, long_1));
                                                    }, Cancelled));
                                                    if (bool_3) break;
                                                }
                                            }
                                            else if (ApiFile.GetLatestVersion().delta.status == ApiFile.Status.Waiting)
                                            {
                                                CheckFile(filecheck, ApiFile, "Uploading file delta...");
                                                bool_3 = false;
                                                yield return MelonCoroutines.Start(method_10(ApiFile, ApiFile.Version.FileDescriptor.Type.delta, text3, text4, size3, delegate (ApiFile apiFile_1)
                                                {
                                                    PendulumLogger.Log("Successfully uploaded file delta.");
                                                    ApiFile = apiFile_1;
                                                }, delegate (string string_4)
                                                {
                                                    Failed(Failure, ApiFile, "Failed to upload file delta.", string_4);
                                                    smethod_8(ApiFile.id);
                                                    bool_3 = true;
                                                }, delegate (long long_0, long long_1)
                                                {
                                                    CheckFile(filecheck, ApiFile, "Uploading file delta...", "", Tools.DivideSafe(long_0, long_1));
                                                }, Cancelled));
                                                if (bool_3) break;
                                            }
                                            smethod_3(ApiFile, flag2);
                                            if (ApiFile.GetLatestVersion().signature.status == ApiFile.Status.Waiting)
                                            {
                                                CheckFile(filecheck, ApiFile, "Uploading file signature...");
                                                bool_3 = false;
                                                yield return MelonCoroutines.Start(method_10(ApiFile, ApiFile.Version.FileDescriptor.Type.signature, tempFileName, text2, size, delegate (ApiFile apiFile_1)
                                                {
                                                    VRC.Core.Logger.Log("Successfully uploaded file signature.", DebugLevel.All);
                                                    ApiFile = apiFile_1;
                                                }, delegate (string string_4)
                                                {
                                                    Failed(Failure, ApiFile, "Failed to upload file signature.", string_4);
                                                    smethod_8(ApiFile.id);
                                                    bool_3 = true;
                                                }, delegate (long long_0, long long_1)
                                                {
                                                    CheckFile(filecheck, ApiFile, "Uploading file signature...", "", Tools.DivideSafe(long_0, long_1));
                                                }, Cancelled));
                                                if (bool_3) break;
                                            }
                                            smethod_3(ApiFile, flag2);
                                            CheckFile(filecheck, ApiFile, "Validating upload...");
                                            if (!(flag2 ? (ApiFile.GetFileDescriptor(ApiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.delta).status == ApiFile.Status.Complete) : (ApiFile.GetFileDescriptor(ApiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.file).status == ApiFile.Status.Complete)) || ApiFile.GetFileDescriptor(ApiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.signature).status != ApiFile.Status.Complete)
                                            {
                                                Failed(Failure, ApiFile, "Failed to upload file.", "Record status is not 'complete'");
                                                smethod_8(ApiFile.id);
                                                break;
                                            }
                                            if (!(flag2 ? (ApiFile.GetFileDescriptor(ApiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.file).status != ApiFile.Status.Waiting) : (ApiFile.GetFileDescriptor(ApiFile.GetLatestVersionNumber(), ApiFile.Version.FileDescriptor.Type.delta).status != ApiFile.Status.Waiting)))
                                            {
                                                Failed(Failure, ApiFile, "Failed to upload file.", "Record is still in 'waiting' status");
                                                smethod_8(ApiFile.id);
                                                break;
                                            }
                                            smethod_3(ApiFile, flag2);
                                            CheckFile(filecheck, ApiFile, "Processing upload...");
                                            float num = float_2;
                                            float b = float_3;
                                            float num2 = method_5(ApiFile.GetLatestVersion().file.sizeInBytes);
                                            double num3 = Time.realtimeSinceStartup;
                                            double num4 = num3;
                                            while (ApiFile.HasQueuedOperation(flag2))
                                            {
                                                CheckFile(filecheck, ApiFile, "Processing upload...", "Checking status in " + Mathf.CeilToInt(num) + " seconds");
                                                while ((double)Time.realtimeSinceStartup - num4 < (double)num)
                                                {
                                                    if (YoinkUtils.Cancelled(Cancelled, Failure, ApiFile))
                                                    {
                                                        smethod_8(ApiFile.id);
                                                        yield break;
                                                    }
                                                    if ((double)Time.realtimeSinceStartup - num3 > (double)num2)
                                                    {
                                                        smethod_3(ApiFile, flag2);
                                                        Failed(Failure, ApiFile, "Timed out waiting for upload processing to complete.");
                                                        smethod_8(ApiFile.id);
                                                        yield break;
                                                    }
                                                    yield return null;
                                                }
                                                do
                                                {
                                                    CheckFile(filecheck, ApiFile, "Processing upload...", "Checking status...");
                                                    bool_0 = true;
                                                    bool_2 = false;
                                                    Message = string.Empty;
                                                    API.Fetch<ApiFile>(ApiFile.id, action, action2);
                                                    while (bool_0)
                                                    {
                                                        if (YoinkUtils.Cancelled(Cancelled, Failure, ApiFile))
                                                        {
                                                            smethod_8(ApiFile.id);
                                                            yield break;
                                                        }
                                                        yield return null;
                                                    }
                                                    if (!string.IsNullOrEmpty(Message))
                                                    {
                                                        Failed(Failure, ApiFile, "Checking upload status failed.", Message);
                                                        if (!bool_2)
                                                        {
                                                            smethod_8(ApiFile.id);
                                                            yield break;
                                                        }
                                                    }
                                                }
                                                while (bool_2);
                                                num = Mathf.Min(num * 2f, b);
                                                num4 = Time.realtimeSinceStartup;
                                            }
                                            yield return MelonCoroutines.Start(method_4(ApiFile.id));
                                            Success(success, ApiFile, "Upload complete!");
                                        }
                                        else
                                        {
                                            Failed(Failure, ApiFile, "Failed to create file delta for upload.", "Couldn't get file size: " + Message);
                                            smethod_8(ApiFile.id);
                                        }
                                    }
                                    else
                                    {
                                        Failed(Failure, ApiFile, "Failed to generate file signature!", "Couldn't get file size:\n" + Message);
                                        smethod_8(ApiFile.id);
                                    }
                                }
                                else
                                {
                                    Failed(Failure, ApiFile, "Failed to generate MD5 hash for signature file.", Message);
                                    smethod_8(ApiFile.id);
                                }
                            }
                            else
                            {
                                Failed(Failure, ApiFile, "Failed to generate file signature!", "Failed to create temp file: \n" + Message);
                                smethod_8(ApiFile.id);
                            }
                        }
                        else Failed(Failure, ApiFile, "Failed to optimize file for upload.", "Failed to create temp file: \n" + Message);
                    }
                    else Failed(Failure, ApiFile, "A previous upload is still being processed. Please try again later.");
                    break;
                }
            }
            else Failed(Failure, null, "Could not read file to upload!", Path + "\n" + whyNot);
        }

        private static void smethod_3(ApiFile ApiFile, bool LastVersion, bool Log = false)
        {
            if (ApiFile != null && ApiFile.IsInitialized)
            {
                //if (!ApiFile.IsInErrorState() && Log) PendulumLogger.Log($"Processing: {ApiFile.name}, {(ApiFile.IsWaitingForUpload() ? "waiting for upload" : "upload complete")}, {(ApiFile.HasExistingOrPendingVersion() ? "has existing or pending version" : "no previous version")}, {(ApiFile.IsLatestVersionQueued(LastVersion) ? "latest version queued" : "latest version not queued")}");
            }
            //else PendulumLogger.Log("ApiFile not initialized");
            if ((ApiFile?.IsInitialized ?? false) && Log)
            {
                Il2CppSystem.Collections.Generic.Dictionary<string, Il2CppSystem.Object> dictionary = ApiFile.ExtractApiFields();
                //if (dictionary != null) PendulumLogger.Log(Tools.JsonEncode(dictionary));
            }
        }

        public IEnumerator method_1(string string_0, string string_1, System.Action<GEnum0> action_0, System.Action<string> action_1)
        {
            VRC.Core.Logger.Log("CreateOptimizedFile: " + string_0 + " => " + string_1, DebugLevel.All);
            if (!IsGzip(string_0))
            {
                VRC.Core.Logger.Log("CreateOptimizedFile: (not gzip compressed, done)", DebugLevel.All);
                action_0?.Invoke(GEnum0.Unchanged);
                yield break;
            }
            bool flag = string.Compare(Path.GetExtension(string_0), ".unitypackage", ignoreCase: true) == 0;
            yield return null;
            Stream stream;
            try
            {
                stream = new GZipStream(string_0);
            }
            catch (Exception ex)
            {
                action_1?.Invoke("Couldn't read file: " + string_0 + "\n" + ex.Message);
                yield break;
            }
            yield return null;
            GZipStream gZipStream;
            try
            {
                gZipStream = new GZipStream(string_1, CompressLevel.Best);//, rsyncable: true, 262144);
            }
            catch (System.Exception ex2)
            {
                stream?.Close();
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
                    stream = new GZipStream(string_0);
                    TarOutputStream tarOutputStream = new TarOutputStream(gZipStream);
                    TarInputStream tarInputStream2 = new TarInputStream(stream);
                    for (TarEntry nextEntry2 = tarInputStream2.GetNextEntry(); nextEntry2 != null; nextEntry2 = tarInputStream2.GetNextEntry())
                    {
                        string string_2 = nextEntry2.Name.Substring(0, nextEntry2.Name.IndexOf('/'));
                        if (!list.Any((string string_1_1) => string.Compare(string_1_1, string_2) == 0))
                        {
                            tarOutputStream.PutNextEntry(nextEntry2);
                            tarInputStream2.CopyEntryContents(tarOutputStream);
                            tarOutputStream.CloseEntry();
                        }
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

        protected static void Success(OnSuccess OnSuccess, ApiFile ApiFile, string Message)
        {
            if (ApiFile == null) ApiFile = new ApiFile();
            //PendulumLogger.Log($"ApiFile {ApiFile.ToStringBrief()}: Operation Succeeded!");
            OnSuccess?.Invoke(ApiFile, Message);
        }

        protected static void Failed(OnFailure OnFailure, ApiFile ApiFile, string Error, string Message = "")
        {
            if (ApiFile == null) ApiFile = new ApiFile();
            //EvoVrConsole.Log(EvoVrConsole.LogsType.Avatar, Message);
           // PendulumLogger.Log($"ApiFile {ApiFile.ToStringBrief()}: Error: {Error}\n{Message}");
            OnFailure?.Invoke(ApiFile, Error);
        }

        protected static void CheckFile(OnFileCheck OnFileCheck, ApiFile ApiFile, string Status, string SubStatus = "", float Pct = 0f)
        {
            if (ApiFile == null) ApiFile = new ApiFile();
            OnFileCheck?.Invoke(ApiFile, Status, SubStatus, Pct);
        }

        protected static bool Cancelled(OnCancelled OnCancelled, OnFailure OnFailure, ApiFile ApiFile)
        {
            if (ApiFile == null)
            {
               // PendulumLogger.Log("ApiFile was null");
                return true;
            }

            if (OnCancelled != null && OnCancelled(ApiFile))
            {
                PendulumLogger.Log("ApiFile " + ApiFile.ToStringBrief() + ": Operation cancelled");
                OnFailure?.Invoke(ApiFile, "Cancelled by user.");
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
                helper = obj.AddComponent<YoinkUtils>();
                RemoteConfig = new RemoteConfig();
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

        private IEnumerator method_7(ApiFile apiFile_0, ApiFile.Version.FileDescriptor.Type type_0, string string_0, string string_1, long long_0, System.Action<ApiFile> action_0, System.Action<string> action_1, System.Action<long, long> action_2, OnCancelled gdelegate3_0)
        {
            OnFailure gdelegate1_ = delegate (ApiFile apiFile_1, string string_1_1)
            {
                action_1?.Invoke(string_1_1);
            };
            string string_2 = "";
            while (true)
            {
                bool bool_0 = true;
                string string_3 = "";
                bool bool_3 = false;
                apiFile_0.StartSimpleUpload(type_0, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    string_2 = IL2CPP.Il2CppStringToManaged(apiContainer_0.Cast<ApiDictContainer>().ResponseDictionary["url"].Pointer);
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
                    if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                    {
                        yield return null;
                        continue;
                    }
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
            HttpRequest httpRequest = ApiFile.PutSimpleFileToURL(string_2, string_0, FileType(Path.GetExtension(string_0)), string_1, false, (Action)delegate
            {
                bool_ = false;
            }, (Action<string>)delegate (string string_1_2)
            {
                string_4 = "Failed to upload file: " + string_1_2;
                bool_ = false;
            }, (Action<long, long>)delegate (long long_0_1, long long_1)
            {
                action_2?.Invoke(long_0_1, long_1);
            });
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
                if (Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                {
                    httpRequest?.Abort();
                    yield break;
                }
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
                    try
                    {
                        apiFile_0 = apiContainer_0.Model.Cast<ApiFile>();
                        bool_2 = false;
                    }
                    catch { }
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
                    if (Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                    {
                        yield break;
                    }
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

        private IEnumerator method_8(ApiFile apiFile_0, ApiFile.Version.FileDescriptor.Type type_0, string string_0, string string_1, long long_0, System.Action<ApiFile> action_0, System.Action<string> action_1, System.Action<long, long> action_2, OnCancelled gdelegate3_0)
        {
            FileStream fileStream_0 = null;
            OnFailure gdelegate1_ = delegate (ApiFile apiFile_1, string string_0_1)
            {
                if (fileStream_0 != null) fileStream_0.Close();
                action_1?.Invoke(string_0_1);
            };
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
                    //VRC.Core.Logger.Log("Found existing multipart upload status (next part = " + uploadStatus_0.nextPartNumber + ")", DebugLevel.All);
                }, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    string_6 = "Failed to query multipart upload status: " + apiContainer_0.Error;
                    bool_3 = false;
                    if (apiContainer_0.Code == 400) bool_5 = true;
                });
                while (bool_3)
                {
                    if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                    {
                        yield return null;
                        continue;
                    }
                    yield break;
                }
                if (!string.IsNullOrEmpty(string_6))
                {
                    action_1?.Invoke(string_6);
                    if (!bool_5) yield break;
                }
                if (bool_5) continue;
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
                if (uploadStatus_0 != null) list_0 = uploadStatus_0.etags.ToArray().ToList();
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
                            string_5 = IL2CPP.Il2CppStringToManaged(apiContainer_0.Cast<ApiDictContainer>().ResponseDictionary["url"].Pointer);
                            bool_2 = false;
                        }, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                        {
                            string_4 = "Failed to start part upload: " + apiContainer_0.Error;
                            bool_2 = false;
                        });
                        while (bool_2)
                        {
                            if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                            {
                                yield return null;
                                continue;
                            }
                            yield break;
                        }
                        if (!string.IsNullOrEmpty(string_4))
                        {
                            fileStream_0.Close();
                            action_1?.Invoke(string_4);
                            if (!flag) yield break;
                        }
                        yield return new WaitForSecondsRealtime(0.75f);
                    }
                    while (flag);
                    bool bool_ = true;
                    var string_3 = "";
                    HttpRequest httpRequest = ApiFile.PutMultipartDataToURL(string_5, array, int_0, FileType(Path.GetExtension(string_0)), false, (System.Action<string>)delegate (string string_1_2)
                    {
                        if (!string.IsNullOrEmpty(string_1)) list_0.Add(string_1_2);
                        long_4 += int_0;
                        bool_ = false;
                    }, (System.Action<string>)delegate (string string_1_1)
                    {
                        string_3 = "Failed to upload data: " + string_1_1;
                        bool_ = false;
                    }, (System.Action<long, long>)delegate (long long_2, long long_3)
                    {
                        action_2?.Invoke(long_4 + long_2, long_0);
                    });
                    while (bool_)
                    {
                        if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                        {
                            yield return null;
                            continue;
                        }
                        httpRequest?.Abort();
                        yield break;
                    }
                    if (!string.IsNullOrEmpty(string_3))
                    {
                        fileStream_0.Close();
                        action_1?.Invoke(string_3);
                        yield break;
                    }
                }
                else long_4 += int_0;
            }
            while (true)
            {
                yield return new WaitForSecondsRealtime(0.75f);
                bool bool_0 = true;
                var string_2 = "";
                bool bool_4 = false;
                Il2CppSystem.Collections.Generic.List<string> list = new Il2CppSystem.Collections.Generic.List<string>();
                foreach (string item in list_0) list.Add(item);
                apiFile_0.FinishUpload(type_0, list, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    try
                    {
                        apiFile_0 = apiContainer_0.Model.Cast<ApiFile>();
                        bool_0 = false;
                    }
                    catch { }
                }, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                {
                    string_2 = "Failed to finish upload: " + apiContainer_0.Error;
                    bool_0 = false;
                    if (apiContainer_0.Code == 400) bool_4 = true;
                });
                while (bool_0)
                {
                    if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                    {
                        yield return null;
                        continue;
                    }
                    yield break;
                }
                if (!string.IsNullOrEmpty(string_2))
                {
                    fileStream_0.Close();
                    action_1?.Invoke(string_2);
                    if (!bool_4) yield break;
                }
                yield return new WaitForSecondsRealtime(0.75f);
                if (!bool_4) break;
            }
            fileStream_0.Close();
        }

        private IEnumerator method_9(ApiFile apiFile_0, ApiFile.Version.FileDescriptor.Type type_0, string string_0, string string_1, long long_0, ApiFile.Version.FileDescriptor fileDescriptor_0, System.Action<ApiFile> action_0, System.Action<string> action_1, System.Action<long, long> action_2, OnCancelled gdelegate3_0)
        {
            OnFailure gdelegate1_ = delegate (ApiFile apiFile_0_1, string string_0_1)
            {
                action_1?.Invoke(string_0_1);
            };
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
                            if (Cancelled(gdelegate3_0, gdelegate1_, apiFile_0)) yield break;
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
                            var string_2 = "";
                            bool bool_1 = false;
                            apiFile_0.Refresh((Action<ApiContainer>)delegate
                            {
                                bool_0 = false;
                            }, (Action<ApiContainer>)delegate (ApiContainer apiContainer_0)
                            {
                                string_2 = "Couldn't verify upload status: " + apiContainer_0.Error;
                                bool_0 = false;
                                if (apiContainer_0.Code == 400) bool_1 = true;
                            });
                            while (bool_0)
                            {
                                if (!Cancelled(gdelegate3_0, gdelegate1_, apiFile_0))
                                {
                                    yield return null;
                                    continue;
                                }
                                yield break;
                            }
                            if (!string.IsNullOrEmpty(string_2))
                            {
                                action_1?.Invoke(string_2);
                                if (!bool_1) yield break;
                            }
                            if (!bool_1) break;
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

        private IEnumerator method_10(ApiFile apiFile_0, ApiFile.Version.FileDescriptor.Type type_0, string string_0, string string_1, long long_0, System.Action<ApiFile> action_0, System.Action<string> action_1, System.Action<long, long> action_2, OnCancelled gdelegate3_0)
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
}