using System;
using ReMod.Core.UI;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRC.SDKBase.Validation.Performance.Stats;
using VRC.Core;
using System.Collections.Generic;
using Newtonsoft.Json;
using AvatarList = Il2CppSystem.Collections.Generic.List<VRC.Core.ApiAvatar>;
using System.Linq;
using System.Net.Http;
using MelonLoader;
using System.Collections;
using System.IO;

namespace PendulumClient.ReModAPI
{
    public class AvatarSearch : IAvatarListOwner
    {
        private static ReAvatarList _searchedAvatarList;
        private static ReAvatarList _favoriteAvatarList;
        private static ReUiButton _favoriteButton;
        private static Button.ButtonClickedEvent _changeButtonEvent;
        private const bool AvatarFavoritesEnabled = true;
        private const int MaxAvatarsPerPage = 25;
        private static UnityAction<string> _searchAvatarsAction;
        private static UnityAction<string> _overrideSearchAvatarsAction;
        private static GameObject _avatarScreen;
        private static UiInputField _searchBox;
        private List<ReAvatar> _savedAvatars;
        private readonly AvatarList _searchedAvatars;
        private static AvatarFavoriteList _favList;

        private static int constantrefresh = 0;
        private static int constantrefreshtimes = 0;

        public AvatarSearch()
        {
            //_enabledToggle.Toggle(AvatarFavoritesEnabled);
            /*_favoriteAvatarList.GameObject.SetActive(AvatarFavoritesEnabled);
            _favoriteButton.GameObject.SetActive(AvatarFavoritesEnabled);

            _favoriteAvatarList.SetMaxAvatarsPerPage(MaxAvatarsPerPage);

            _savedAvatars = new List<ReAvatar>();*/
            _searchedAvatars = new AvatarList();
            if (!File.Exists("PendulumClient/AvatarFavorites.txt"))
            {
                _favList = new AvatarFavoriteList();
                _favList.avatars = new List<AvatarFavoriteListComponent>();
                File.WriteAllText("PendulumClient/AvatarFavorites.txt", _favList.JsonString());
            }
        }

        public void OnUiEarly()
        {
            _searchedAvatarList = new ReAvatarList("Avatar Search", this);

            _favoriteAvatarList = new ReAvatarList("PendulumClient Favorites", this, false);
            _favoriteAvatarList.AvatarPedestal.field_Internal_Action_3_String_GameObject_AvatarPerformanceStats_0 = new Action<string, GameObject, AvatarPerformanceStats>(OnAvatarInstantiated);
            _favoriteAvatarList.OnEnable += () =>
            {
                // make sure it stays off if it should be off.
                _favoriteAvatarList.GameObject.SetActive(AvatarFavoritesEnabled);
            };

            var parent = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Favorite Button").transform.parent;
            _favoriteButton = new ReUiButton("Favorite", new Vector2(-875f, 375f), new Vector2(250f, 80f),
                () => FavoriteAvatar(_favoriteAvatarList.AvatarPedestal.field_Internal_ApiAvatar_0),
                parent);
            _favoriteButton.GameObject.SetActive(AvatarFavoritesEnabled);

            var changeButton = GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Change Button");
            if (changeButton != null)
            {
                var button = changeButton.GetComponent<Button>();
                _changeButtonEvent = button.onClick;

                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(new Action(ChangeAvatarChecked));
            }

            _searchAvatarsAction = DelegateSupport.ConvertDelegate<UnityAction<string>>(
                (Action<string>)SearchAvatars);
            /*_overrideSearchAvatarsAction = DelegateSupport.ConvertDelegate<UnityAction<string>>(
                (Action<string>)PromptChooseSearch); prob dont need this*/

            _avatarScreen = GameObject.Find("UserInterface/MenuContent/Screens/Avatar");
            _searchBox = GameObject.Find("UserInterface/MenuContent/Backdrop/Header/Tabs/ViewPort/Content/Search/InputField").GetComponent<UiInputField>();

            _favoriteAvatarList.GameObject.SetActive(AvatarFavoritesEnabled);
            _favoriteButton.GameObject.SetActive(AvatarFavoritesEnabled);

            _favoriteAvatarList.SetMaxAvatarsPerPage(MaxAvatarsPerPage);

            _savedAvatars = new List<ReAvatar>();
            LoadSavedFavorites();
            /*new ApiAvatar { id = "avtr_01666e05-5d00-4d3e-86fd-43866265e525" }.Fetch(new Action<ApiContainer>(ac => 
            {
                var updatedAvatar = ac.Model.Cast<ApiAvatar>();
                _savedAvatars.Insert(0, new ReAvatar(updatedAvatar));
            }), new Action<ApiContainer>(ac =>
            {
                //PendulumClient.UI.AlertPopup.SendAlertPopup("Avatar Deleted!", "This avatar has been deleted. You can't switch into it.");
                PendulumLogger.LogError("Failed to find Avatar");
            }));*/
            APILogin.FirstTimeLogin();
        }

        public void OnUpdate()
        {
            if (_searchBox == null)
                return;

            if (!_avatarScreen.active)
            {
                return;
            }

            if (!_searchBox.field_Public_Button_0.interactable)
            {
                _searchBox.field_Public_Button_0.interactable = true;
                _searchBox.field_Public_UnityAction_1_String_0 = _searchAvatarsAction;
            }

            if (constantrefresh >= 100)
            {
                constantrefresh = 0;
                if (_savedAvatars.Count > 0 && GameObject.Find("UserInterface/MenuContent/Screens/Avatar").activeSelf && constantrefreshtimes < 1)
                {
                    _favoriteAvatarList.RefreshAvatars();
                    constantrefreshtimes++;
                }
                if (GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/PendulumClientFavoritesAvatarList") != null)
                {
                    if (!GameObject.Find("UserInterface/MenuContent/Screens/Avatar/Vertical Scroll View/Viewport/Content/PendulumClientFavoritesAvatarList").GetComponent<Canvas>().enabled)
                    {
                        constantrefreshtimes = 0;
                    }
                }
            }
            else
            {
                constantrefresh++;
            }
        }

        private bool IsAvatarInList(string id)
        {
            foreach (ApiAvatar avatar in _searchedAvatars)
            {
                if (avatar.id == id) return true;
            }
            return false;
        }
        private void ChangeAvatarChecked()
        {
            var currentAvatar = _favoriteAvatarList.AvatarPedestal.field_Internal_ApiAvatar_0;
            if (!HasAvatarFavorited(currentAvatar.id) || !IsAvatarInList(currentAvatar.id)) // this isn't in our list. we don't care about it
            {
                _changeButtonEvent.Invoke();
                return;
            }

            new ApiAvatar { id = currentAvatar.id }.Fetch(new Action<ApiContainer>(ac =>
            {
                var updatedAvatar = ac.Model.Cast<ApiAvatar>();
                switch (updatedAvatar.releaseStatus)
                {
                    case "private" when updatedAvatar.authorId != APIUser.CurrentUser.id:
                        Main.PendulumClientMain.Errormsg("Avatar Private", "This avatar is private and you dont own it.");
                        break;
                    case "unavailable":
                        Main.PendulumClientMain.Errormsg("Avatar Deleted", "This avatar has been deleted.");
                        break;
                    default:
                        _changeButtonEvent.Invoke();
                        break;
                }
            }), new Action<ApiContainer>(ac =>
            {
                Main.PendulumClientMain.Errormsg("Avatar Deleted", "This avatar has been deleted.");
            }));
        }
        private void FavoriteAvatar(ApiAvatar apiAvatar)
        {
            if (apiAvatar.authorId == APIUser.CurrentUser.id)
            {
                Main.PendulumClientMain.Errormsg("bruh", "i also enjoy unityexplorer");
            }
            /*SendAvatarRequest(hasFavorited ? HttpMethod.Delete : HttpMethod.Put, favResponse =>
            {
                if (!favResponse.IsSuccessStatusCode)
                {
                    if (favResponse.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        ReLogger.Msg($"Not logged into ReMod CE API anymore. Trying to login again and resuming request.");
                        LoginToAPI(APIUser.CurrentUser, () => FavoriteAvatar(apiAvatar));
                        return;
                    }

                    favResponse.Content.ReadAsStringAsync().ContinueWith(errorData =>
                    {
                        var errorMessage = JsonConvert.DeserializeObject<ApiError>(errorData.Result).Error;
                        ReLogger.Error($"Could not (un)favorite avatar: \"{errorMessage}\"");
                        if (favResponse.StatusCode == HttpStatusCode.Forbidden)
                        {
                            MelonCoroutines.Start(ShowAlertDelayed($"Could not (un)favorite avatar\nReason: \"{errorMessage}\""));
                        }
                    });
                }
            }, new ReAvatar(apiAvatar));*/

            if (HasAvatarFavorited(apiAvatar.id))
            {
                RemoveFavorite(apiAvatar);
                _favoriteButton.Text = "Favorite Avatar";
            }
            else
            {
                AddFavorite(apiAvatar);
                _favoriteButton.Text = "Unfavorite Avatar";
            }

            /*if (_favoriteAvatarList.AvatarPedestal.field_Internal_ApiAvatar_0.id == apiAvatar.id)
            {
                if (!HasAvatarFavorited(apiAvatar.id))
                {
                    _favoriteButton.Text = "Unfavorite";
                }
                else
                {
                    _favoriteButton.Text = "Favorite";
                }
            }*/

            _favoriteAvatarList.RefreshAvatars();
        }
        private bool HasAvatarFavorited(string id)
        {   
            return _savedAvatars.FirstOrDefault(a => a.Id == id) != null;
        }
        private void AddFavorite(ApiAvatar avatar)
        {
            _savedAvatars.Insert(0, new ReAvatar(avatar));
            SaveAvatarFile();
        }

        private void RemoveFavorite(ApiAvatar avatar)
        {
            _savedAvatars.RemoveAll(a => a.Id == avatar.id);
            SaveAvatarFile();
        }
        private void SaveAvatarFile()
        {
            try
            {
                List<AvatarFavoriteListComponent> templist = new List<AvatarFavoriteListComponent>();
                foreach (ReAvatar avatar in _savedAvatars)
                {
                    templist.Add(new AvatarFavoriteListComponent(avatar.AsApiAvatar()));
                }
                if (templist.Count > 0)
                {
                    _favList.avatars = templist;
                }
                File.WriteAllText("PendulumClient/AvatarFavorites.txt", _favList.JsonString());
            }
            catch (Exception e) 
            {
                PendulumLogger.LogError("Error saving favorite avatars: " + e.ToString());
            }
        }
        private void LoadSavedFavorites()
        {
            if (_favList == null)
            {
                try
                {
                    _favList = JsonConvert.DeserializeObject<AvatarFavoriteList>(File.ReadAllText("PendulumClient/AvatarFavorites.txt"));
                    List<ReAvatar> templist = new List<ReAvatar>();
                    foreach (AvatarFavoriteListComponent avatar in _favList.avatars)
                    {
                        templist.Add(new ReAvatar(avatar.Avatar()));
                    }
                    if (templist.Count > 0)
                    {
                        _savedAvatars = templist;
                    }
                    PendulumLogger.Log("Loaded " + templist.Count + " avatar favorites");
                }
                catch (Exception e)
                {
                    PendulumLogger.LogError("Failed to load avatar favorites: " + e.ToString());
                }
            }
        }
        private void SearchAvatars(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || searchTerm.Length < 3)
            {
                Main.PendulumClientMain.Errormsg("ReMod Search", "That search term is too short. The search term has to be at least 3 characters.");
                return;
            }


            var request2 = new HttpRequestMessage(HttpMethod.Get, $"https://remod-ce.requi.dev/api/search.php?searchTerm={searchTerm}");
            var output2 = APILogin.getHttp().SendAsync(request2).ContinueWith(rsp =>
            {
                var searchResponse = rsp.Result;
                if (!searchResponse.IsSuccessStatusCode)
                {
                    if (searchResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        PendulumLogger.Log($"Not logged into ReMod API anymore. Trying to login again and resuming request.");
                        APILogin.ReLogin(() => SearchAvatars(searchTerm));
                        return;
                    }

                    searchResponse.Content.ReadAsStringAsync().ContinueWith(errorData =>
                    {
                        var errorMessage = JsonConvert.DeserializeObject<ApiError>(errorData.Result).Error;

                        PendulumLogger.LogError($"Could not search for avatars: \"{errorMessage}\"");
                        if (searchResponse.StatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            //MelonCoroutines.Start(ShowAlertDelayed($"Could not search for avatars\nReason: \"{errorMessage}\""));
                        }
                    });
                }
                else
                {
                    searchResponse.Content.ReadAsStringAsync().ContinueWith(t =>
                    {
                        var avatars = JsonConvert.DeserializeObject<List<ReAvatar>>(t.Result) ?? new List<ReAvatar>();
                        MelonCoroutines.Start(RefreshSearchedAvatars(avatars));
                    });
                }
            });
        }

        private IEnumerator RefreshSearchedAvatars(List<ReAvatar> results)
        {
            yield return new WaitForEndOfFrame();

            _searchedAvatars.Clear();
            foreach (var avi in results.Select(x => x.AsApiAvatar()))
            {
                _searchedAvatars.Add(avi);
            }

            PendulumLogger.Log($"Found {_searchedAvatars.Count} avatars");
            _searchedAvatarList.RefreshAvatars();
        }
        private void OnAvatarInstantiated(string url, GameObject avatar, AvatarPerformanceStats avatarPerformanceStats)
        {
            if (_favoriteAvatarList.AvatarPedestal.field_Internal_ApiAvatar_0.authorId == APIUser.CurrentUser.id)
            {
                _favoriteButton.Interactable = false;
                _favoriteButton.Text = "Can't Favorite";
            }
            else
            {
                _favoriteButton.Interactable = true;
                _favoriteButton.Text = HasAvatarFavorited(_favoriteAvatarList.AvatarPedestal.field_Internal_ApiAvatar_0.id) ? "Unfavorite Avatar" : "Favorite Avatar";
            }
        }

        public AvatarList GetAvatars(ReAvatarList avatarList)
        {
            if (avatarList == _favoriteAvatarList)
            {
                var list = new AvatarList();
                try
                {
                    foreach (var avi in _savedAvatars.Select(x => x.AsApiAvatar()).ToList())
                    {
                        list.Add(avi);
                    }
                }
                catch { }

                return list;
            }
            else if (avatarList == _searchedAvatarList)
            {
                return _searchedAvatars;
            }

            return null;
        }

        public void Clear(ReAvatarList avatarList)
        {
            if (avatarList == _searchedAvatarList)
            {
                _searchedAvatars.Clear();
                avatarList.RefreshAvatars();
            }
        }
    }

    [Serializable]
    internal class ReAvatar
    {
        public string Id { get; set; }
        public string AvatarName { get; set; }
        public string AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Description { get; set; }
        public string AssetUrl { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public ApiModel.SupportedPlatforms SupportedPlatforms = ApiModel.SupportedPlatforms.StandaloneWindows;

        public ReAvatar()
        {
        }

        public ReAvatar(ApiAvatar apiAvatar)
        {
            Id = apiAvatar.id;
            AvatarName = apiAvatar.name;
            AuthorId = apiAvatar.authorId;
            AuthorName = apiAvatar.authorName;
            Description = apiAvatar.description;
            AssetUrl = apiAvatar.assetUrl;
            ThumbnailUrl = apiAvatar.thumbnailImageUrl;
            SupportedPlatforms = apiAvatar.supportedPlatforms;
        }

        public ApiAvatar AsApiAvatar()
        {
            return new ApiAvatar
            {
                id = Id,
                name = AvatarName,
                authorId = AuthorId,
                authorName = AuthorName,
                description = Description,
                assetUrl = AssetUrl,
                thumbnailImageUrl = string.IsNullOrEmpty(ThumbnailUrl) ? (string.IsNullOrEmpty(ImageUrl) ? "https://assets.vrchat.com/system/defaultAvatar.png" : ImageUrl) : ThumbnailUrl,
                releaseStatus = "public",
                unityVersion = "2019.4.29f1",
                version = 1,
                apiVersion = 1,
                Endpoint = "avatars",
                Populated = false,
                assetVersion = new AssetVersion("2019.4.29f1", 0),
                tags = new Il2CppSystem.Collections.Generic.List<string>(0),
                supportedPlatforms = SupportedPlatforms,
            };
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
