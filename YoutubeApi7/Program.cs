// See https://aka.ms/new-console-template for more information

using System.Threading.Channels;
using Google.Apis.Auth.OAuth2;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTubeAnalytics.v2;
using YoutubeApi7;

Console.WriteLine("Hello, World!");

const string SECRET = "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";
const string CLIENT_ID = "915496023150-p3r9stpk2ehmi474lp4orcurg67q3tct.apps.googleusercontent.com";
const string API_KEY = "AIzaSyA_HckYImRn8zMOGks4tLVH2zsM1FTVWMA";
const string GOOGLE_CHANNEL_ID = "UC_x5XG1OV2P6uZZ5FSM9Ttw"; // 5700 видео
const string CHANNEL_NAME = "tankionline"; // 701 видео
const string TEST_VIDEO_ID = "kfd-oLypqFI";

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

ClientSecrets secrets = new ClientSecrets()
{
    ClientId = CLIENT_ID,
    ClientSecret = SECRET
};

YouTubeAnalyticsService youTubeAnalyticsService = new();
YouTubeService yt = new();

var userCredential = await UserAuthService.AuthorizeUser(secrets, scopes, "sdsвsuyhaываsdsss");



Console.WriteLine("Access token: " + userCredential.Token.AccessToken);

AnalyticsService analyticsService = new AnalyticsService(youTubeAnalyticsService, userCredential);
DataApiService dataService = new(yt, API_KEY);

MetricsCounterService mc = new(analyticsService,dataService);

Console.WriteLine(await mc.GetMeanViewsAsync());

