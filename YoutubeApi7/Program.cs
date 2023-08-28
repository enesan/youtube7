// See https://aka.ms/new-console-template for more information

using System.Threading.Channels;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTubeAnalytics.v2;
using YoutubeApi7;

Console.WriteLine("Hello, World!");

const string SECRET = "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";
const string CLIENT_ID = "915496023150-p3r9stpk2ehmi474lp4orcurg67q3tct.apps.googleusercontent.com";
const string API_KEY = "AIzaSyA_HckYImRn8zMOGks4tLVH2zsM1FTVWMA";
// const string GOOGLE_CHANNEL_ID = "UC_x5XG1OV2P6uZZ5FSM9Ttw"; // 5700 видео
// const string CHANNEL_NAME = "tankionline"; // 701 видео
// const string TEST_VIDEO_ID = "kfd-oLypqFI";

string[] scopes = 
{
    YouTubeService.ScopeConstants.Youtube,
    YouTubeService.ScopeConstants.YoutubeChannelMembershipsCreator,
    YouTubeService.ScopeConstants.YoutubeForceSsl,
    YouTubeService.ScopeConstants.Youtubepartner,
    YouTubeAnalyticsService.ScopeConstants.YtAnalyticsMonetaryReadonly,
    YouTubeAnalyticsService.ScopeConstants.YtAnalyticsReadonly,
    YouTubeAnalyticsService.ScopeConstants.YoutubeReadonly
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

YouTubeAnalyticsService youTubeAnalyticsService = new();
YouTubeService yt = new();

var userCredential = await UserAuthService.AuthorizeUser(secrets, scopes, "sdsвsuyhaываsdsss");

Oauth2Service auth = new();
var a = auth.Features;

List<int> aa = new List<int>();

foreach (var i in aa)
{
    
}

    
AnalyticsService analyticsService = new AnalyticsService(youTubeAnalyticsService, userCredential);
DataApiService dataService = new(yt, API_KEY, userCredential.Token.AccessToken);

MetricsCounterService mc = new(analyticsService,dataService);

Console.WriteLine("Mean views " + await mc.GetMeanViewsAsync());
Console.WriteLine("Comments/views " + await mc.GetCommentsViewsAsync());
Console.WriteLine("EngagementRate " + await mc.GetEngagementRateAsync());
Console.WriteLine("Likes/dislikes " + await mc.GetLikesDislikesAsync());
Console.WriteLine("mean dislikes " + await mc.GetMeanDislikesAsync());
Console.WriteLine(" mean shares" + await mc.GetMeanSharesAsync());
Console.WriteLine("mean video adding freq " + await mc.GetMeanVideoAddFrequencyAsync());

