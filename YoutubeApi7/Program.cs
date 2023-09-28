// See https://aka.ms/new-console-template for more information

using System.Threading.Channels;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Oauth2.v2;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTubeAnalytics.v2;
using Newtonsoft.Json.Linq;
using YoutubeApi7;

Console.WriteLine("Hello, World!");

const string SECRET = "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";
const string CLIENT_ID = "915496023150-cv7lfe22g2mm1m96oi2uggtjn32gufnt.apps.googleusercontent.com";
const string API_KEY = "AIzaSyA_HckYImRn8zMOGks4tLVH2zsM1FTVWMA";
string REDIRECT_URI = "http://localhost:3000";
// const string GOOGLE_CHANNEL_ID = "UC_x5XG1OV2P6uZZ5FSM9Ttw"; // 5700 видео
// const string CHANNEL_NAME = "tankionline"; // 701 видео
// const string TEST_VIDEO_ID = "kfd-oLypqFI";

string[] scopes = 
{

    YouTubeAnalyticsService.ScopeConstants.YoutubeReadonly,
    Oauth2Service.ScopeConstants.Openid
};

string[] oaScopes =
{
    Oauth2Service.ScopeConstants.Openid,
    Oauth2Service.ScopeConstants.UserinfoProfile
};

ClientSecrets secrets = new ClientSecrets()
{
    ClientId = CLIENT_ID,
    ClientSecret = SECRET
};


//var userCredential = await UserAuthService.AuthorizeUser(secrets, scopes, "sddsвsuyhaываsdsss");



var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
{
    ClientSecrets = new ClientSecrets
    {
        ClientId = CLIENT_ID,
        ClientSecret = SECRET
    },
    Scopes = scopes,
});

HttpClient httpClient = new HttpClient();
        HttpResponseMessage response = await httpClient.PostAsync($"https://accounts.google.com/o/oauth2/device/code?client_id={CLIENT_ID}&scope={scopes}",
            null);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(content);
            string deviceCode = jsonResponse["device_code"].ToString();
            string userCode = jsonResponse["user_code"].ToString();

            Console.WriteLine($"Please visit: https://www.google.com/device and enter the code: {userCode}");

            // Step 2: Poll for authorization
            int interval = int.Parse(jsonResponse["interval"].ToString());
            int sleepDuration = 5; // initial sleep duration in seconds

            while (true)
            {
                await Task.Delay(sleepDuration * 1000); // Sleep for the specified duration

                response = await httpClient.PostAsync($"https://oauth2.googleapis.com/token?client_id={CLIENT_ID}&client_secret={SECRET}&code={deviceCode}&grant_type=http://oauth.net/grant_type/device/1.0",
                    null);

                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JObject.Parse(content);

                    if (tokenResponse.ContainsKey("error"))
                    {
                        string error = tokenResponse["error"].ToString();
                        if (error == "authorization_pending")
                        {
                            // The user hasn't approved the authorization yet; continue polling.
                        }
                        else
                        {
                            Console.WriteLine($"Error: {error}");
                            break;
                        }
                    }
                    else
                    {
                        string idToken = tokenResponse["id_token"].ToString();
                        Console.WriteLine($"ID Token: {idToken}");
                        break;
                    }
                }
                else
                {
                    Console.WriteLine($"Error: {response.ReasonPhrase}");
                    break;
                }

                // Increase the sleep duration for the next poll
                sleepDuration = Math.Min(sleepDuration * 2, interval);
            }
        }
        else
        {
            Console.WriteLine($"Error: {response.ReasonPhrase}");
        }



// YouTubeAnalyticsService youTubeAnalyticsService = new();
// YouTubeService yt = new();
//
 var userCredential = await UserAuthService.AuthorizeUser(secrets, scopes, "l");

 Oauth2Service auth = new();
// var a = auth.Features;
//
// List<int> aa = new List<int>();
//
// foreach (var i in aa)
// {
//     
// }
//
//     
// AnalyticsService analyticsService = new AnalyticsService(youTubeAnalyticsService, userCredential);
// DataApiService dataService = new(yt, API_KEY, userCredential.Token.AccessToken);
//
// MetricsCounterService mc = new(analyticsService,dataService);
//
// Console.WriteLine("Mean views " + await mc.GetMeanViewsAsync());
// Console.WriteLine("Comments/views " + await mc.GetCommentsViewsAsync());
// Console.WriteLine("EngagementRate " + await mc.GetEngagementRateAsync());
// Console.WriteLine("Likes/dislikes " + await mc.GetLikesDislikesAsync());
// Console.WriteLine("mean dislikes " + await mc.GetMeanDislikesAsync());
// Console.WriteLine(" mean shares" + await mc.GetMeanSharesAsync());
// Console.WriteLine("mean video adding freq " + await mc.GetMeanVideoAddFrequencyAsync());

