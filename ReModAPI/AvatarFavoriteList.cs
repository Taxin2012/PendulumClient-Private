using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRC.Core;
using Newtonsoft.Json;

namespace PendulumClient.ReModAPI
{
    [Serializable]
    public class AvatarFavoriteListComponent
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

        public AvatarFavoriteListComponent()
        {
        }

        public AvatarFavoriteListComponent(ApiAvatar apiAvatar)
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

        public ApiAvatar Avatar()
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
    }
    [Serializable]
    public class AvatarFavoriteList
    {
        public List<AvatarFavoriteListComponent> avatars { get; set; }

        public string JsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
