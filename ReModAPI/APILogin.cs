using System;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PendulumClient.ReModAPI
{
    class APILogin
    {
        private static HttpClient _httpClient;
        private static HttpClientHandler _httpClientHandler;
        public static void FirstTimeLogin()
        {
            _httpClientHandler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };
            _httpClient = new HttpClient(_httpClientHandler);

            var _userAgent = "ReModCE/Desktop.1.0.0.4 (Windows NT 10.0; Win64; x64)";
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://remod-ce.requi.dev/api/login.php")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user_id", "usr_d2e946d0-1fd7-4805-8429-4dfdcf9d07f3"),
                    new KeyValuePair<string, string>("pin", "373")
                })
            };

            _httpClient.SendAsync(request).ContinueWith(responseTask =>
            {
                if (!responseTask.Result.IsSuccessStatusCode)
                {
                    responseTask.Result.Content.ReadAsStringAsync().ContinueWith(tsk =>
                    {
                        var errorMessage = JsonConvert.DeserializeObject<ApiError>(tsk.Result).Error;
                        PendulumLogger.LogError("Failed to connect to ReMod API: " + errorMessage);
                    });
                }
                else
                {
                    PendulumLogger.Log(ConsoleColor.Green, "Sucessfully connected to ReMod API.");
                }
            }).GetAwaiter().GetResult();
        }

        public static HttpClient getHttp() //use this to make reqs to remod api
        {
            return _httpClient;
        }

        public static bool DontKeepTrying = false;
        public static void ReLogin(Action onLogin = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://remod-ce.requi.dev/api/login.php")
            {
                Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user_id", "usr_d2e946d0-1fd7-4805-8429-4dfdcf9d07f3"),
                    new KeyValuePair<string, string>("pin", "373")
                })
            };

            _httpClient.SendAsync(request).ContinueWith(responseTask =>
            {
                if (!responseTask.Result.IsSuccessStatusCode)
                {
                    DontKeepTrying = true;
                    responseTask.Result.Content.ReadAsStringAsync().ContinueWith(tsk =>
                    {
                        var errorMessage = JsonConvert.DeserializeObject<ApiError>(tsk.Result).Error;
                        PendulumLogger.LogError("Failed to connect to ReMod API: " + errorMessage);
                    });
                }
                else
                {
                    PendulumLogger.Log(ConsoleColor.Green, "Sucessfully re-connected to ReMod API.");
                }
            }).GetAwaiter().GetResult();

            if (onLogin != null && !DontKeepTrying) onLogin();
        }
    }
}
