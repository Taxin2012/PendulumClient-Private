using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PendulumClient.Anti
{
    internal static class BlockedStrings
    {
		internal static readonly List<string> allowedImageURLs = new List<string> { "https://vrchat.com/", "https://assets.vrchat.com/", "https://api.vrchat.cloud/", "https://files.vrchat.cloud/", "https://s3-us-west-2.amazonaws.com/vrc-uploads/", "https://s3-us-west-2.amazonaws.com/vrc-managed-files/", "https://s3.amazonaws.com/files.vrchat.cloud/", "https://d348imysud55la.cloudfront.net/", "https://imgur.com/", "https://vt.miinc.ru/" };

		internal static readonly List<string> allowedVideoPlayerURLs = new List<string>
		{
			"youtube.com", "youtu.be", "twitch.tv", "dropbox.com", "cdn.discordapp.com", "soundcloud.com", "player.vimeo.com", "shintostudios.net", "rootworld.xyz", "vrcm.nl",
			"lolisociety.com", "sec1.gencily.xyz", "weba.scrittonic.xyz", "t-ne.x0.to", "stream.vrcdn.live", "nepu.io", "dai.ly", "dailymotion.com", "googlevideo.com"
		};
		internal static bool SubDomainCheck(string url)
		{
			int num = 0;
			for (int i = 0; i < url.Length; i++)
			{
				if (url[i] == '.')
				{
					num++;
					if (num == 2)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
