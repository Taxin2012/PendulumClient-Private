using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using PendulumClient.Main;
using PendulumClient.ColorModule;
using VRC.UI.Core.Styles;
using UnityEngine.Networking;
using UnhollowerRuntimeLib;

namespace PendulumClient.UI
{
    internal class MenuMusicStuff
    {
        internal static bool enumstarted = false;
        internal static object shufflecoroutine;
        internal static bool MenuMusicShuffle = false;
        internal static List<AudioClip> Musics = new List<AudioClip>();

        internal static void SetupMenuMusic()
        {
            if (File.Exists("PendulumClient/MenuMusic/LoginMusic.wav"))
            {
                var uwr = UnityWebRequest.Get($"file://{Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/LoginMusic.wav")}");
                uwr.SendWebRequest();
                if (!uwr.isDone)
                {
                    while (true)
                    {
                        System.Threading.Thread.Sleep(1);
                        if (uwr.isDone)
                        {
                            break;
                        }
                    }
                }
                var audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(uwr.downloadHandler, uwr.url, false, false, AudioType.UNKNOWN);

                var audioSource = GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>();
                audioSource.Stop();
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else if (File.Exists("PendulumClient/MenuMusic/LoginMusic.ogg"))
            {
                var uwr = UnityWebRequest.Get($"file://{Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/LoginMusic.ogg")}");
                uwr.SendWebRequest();
                if (!uwr.isDone)
                {
                    while (true)
                    {
                        System.Threading.Thread.Sleep(1);
                        if (uwr.isDone)
                        {
                            break;
                        }
                    }
                }
                var audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(uwr.downloadHandler, uwr.url, false, false, AudioType.UNKNOWN);

                var audioSource = GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>();
                audioSource.Stop();
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else if (File.Exists("PendulumClient/MenuMusic/LoginMusic.mp3"))
            {
                var uwr = UnityWebRequest.Get($"file://{Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/LoginMusic.mp3")}");
                uwr.SendWebRequest();
                if (!uwr.isDone)
                {
                    while (true)
                    {
                        System.Threading.Thread.Sleep(1);
                        if (uwr.isDone)
                        {
                            break;
                        }
                    }
                }
                var audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(uwr.downloadHandler, uwr.url, false, false, AudioType.UNKNOWN);

                var audioSource = GameObject.Find("UserInterface/LoadingBackground_TealGradient_Music/LoadingSound").GetComponent<AudioSource>();
                audioSource.Stop();
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/ShuffleMusic")))
            {
                try
                {
                    foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/ShuffleMusic")))
                    {
                        var extension = Path.GetExtension(file);
                        if (extension == ".wav" || extension == ".ogg" || extension == ".mp3")
                        {
                            var uwr = UnityWebRequest.Get($"file://{file}");
                            uwr.SendWebRequest();
                            if (!uwr.isDone)
                            {
                                while (true)
                                {
                                    System.Threading.Thread.Sleep(1);
                                    if (uwr.isDone)
                                    {
                                        break;
                                    }
                                }
                            }
                            var audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(uwr.downloadHandler, uwr.url, false, false, AudioType.UNKNOWN);
                            audioClip.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                            Musics.Add(audioClip);
                            MenuMusicShuffle = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    PendulumLogger.LogErrorSevere("Failed to load ShuffleMenuMusic: " + e.ToString());
                }
            }
            else
            {
                if (File.Exists("PendulumClient/MenuMusic/LoadingMusic.wav"))
                {
                    var uwr = UnityWebRequest.Get($"file://{Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/LoadingMusic.wav")}");
                    uwr.SendWebRequest();
                    if (!uwr.isDone)
                    {
                        while (true)
                        {
                            System.Threading.Thread.Sleep(1);
                            if (uwr.isDone)
                            {
                                break;
                            }
                        }
                    }
                    var audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(uwr.downloadHandler, uwr.url, false, false, AudioType.UNKNOWN);

                    var audioSource = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>();
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                        audioSource.clip = audioClip;
                        audioSource.Play();
                    }
                    else
                    {
                        audioSource.clip = audioClip;
                    }
                }
                else if (File.Exists("PendulumClient/MenuMusic/LoadingMusic.ogg"))
                {
                    var uwr = UnityWebRequest.Get($"file://{Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/LoadingMusic.ogg")}");
                    uwr.SendWebRequest();
                    if (!uwr.isDone)
                    {
                        while (true)
                        {
                            System.Threading.Thread.Sleep(1);
                            if (uwr.isDone)
                            {
                                break;
                            }
                        }
                    }
                    var audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(uwr.downloadHandler, uwr.url, false, false, AudioType.UNKNOWN);

                    var audioSource = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>();
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                        audioSource.clip = audioClip;
                        audioSource.Play();
                    }
                    else
                    {
                        audioSource.clip = audioClip;
                    }
                }
                else if (File.Exists("PendulumClient/MenuMusic/LoadingMusic.mp3"))
                {
                    var uwr = UnityWebRequest.Get($"file://{Path.Combine(Environment.CurrentDirectory, "PendulumClient/MenuMusic/LoadingMusic.mp3")}");
                    uwr.SendWebRequest();
                    if (!uwr.isDone)
                    {
                        while (true)
                        {
                            System.Threading.Thread.Sleep(1);
                            if (uwr.isDone)
                            {
                                break;
                            }
                        }
                    }
                    var audioClip = WebRequestWWW.InternalCreateAudioClipUsingDH(uwr.downloadHandler, uwr.url, false, false, AudioType.UNKNOWN);

                    var audioSource = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>();
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                        audioSource.clip = audioClip;
                        audioSource.Play();
                    }
                    else
                    {
                        audioSource.clip = audioClip;
                    }
                }
            }
        }
        internal static void ShuffleMenuMusic()
        {
            if (shufflecoroutine != null)
            {
                MelonLoader.MelonCoroutines.Stop(shufflecoroutine);
                enumstarted = false;
                shufflecoroutine = null;
            }
            if (Musics.Count > 0 && shufflecoroutine == null && !Anti.Patches.InvisibleDetection.isConnectedToInstance)
            {
                var i = new System.Random().Next(0, Musics.Count - 1);
                var audioSource = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/LoadingSound").GetComponent<AudioSource>();
                //audioSource.loop = false;
                //AudioSourcePTR = audioSource.GetCachedPtr();
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.clip = Musics[i];
                }
                else
                {
                    audioSource.clip = Musics[i];
                }
                audioSource.Play();
                shufflecoroutine = MelonLoader.MelonCoroutines.Start(ShuffleMusicAfter(Musics[i].length));
            }
        }
        internal static IEnumerator ShuffleMusicAfter(float time)
        {
            if (enumstarted == false)
            {
                enumstarted = true;
                if (!Anti.Patches.InvisibleDetection.isConnectedToInstance)
                    yield return new WaitForSecondsRealtime(time);

                if (!Anti.Patches.InvisibleDetection.isConnectedToInstance)
                    ShuffleMenuMusic();

                enumstarted = false;
                shufflecoroutine = null;
            }
        }
    }
}
