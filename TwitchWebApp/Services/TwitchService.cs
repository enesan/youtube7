using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Core.HttpCallHandlers;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Core.RateLimiter;
using TwitchLib.Api.Helix;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Api.Helix.Models.Videos.GetVideos;
using TwitchLib.Api.Services.Events;
using TwitchLib.EventSub.Core.SubscriptionTypes.Stream;
using TwitchWebApp.Extensions;
using TwitchWebApp.Models;
using TwitchLib.EventSub;
using TwitchLib.EventSub.Webhooks;
using TwitchLib.EventSub.Webhooks.Core.EventArgs.Stream;

namespace TwitchWebApp.Services;

public class TwitchService
{
    private const string CLIENT_ID = "hirrtfyhk37fbgla954bj646i505h0";
    private const string CLIENT_SECRET = "19le5p7x5uip2zhhev805fm674eyqk";
    private const string REDIRECT_URI = "http://localhost:3000";
    private const string TEST_STATE = "c3ab8aa609ea11e793ae92361f002671";

    private IRateLimiter _limiter;
    private IHttpCallHandler _callHandler;
    private List<string> _scopesString;
    private TwitchAPI _api;
    

    private static string? AccessToken { get; set; }
    private static string? RefreshToken { get; set; }
    
    // добавить фабрику httpclient

    public TwitchService()
    {
        _limiter = TimeLimiter.GetFromMaxCountByInterval(1000, TimeSpan.FromDays(10));
        _callHandler = new TwitchHttpClient();

        _scopesString = new()
        {
            "moderator:read:followers"
        };

        _api = new TwitchAPI(rateLimiter: _limiter, http: _callHandler)
        {
            Settings =
            {
                Secret = CLIENT_SECRET,
                ClientId = CLIENT_ID
            }
        };
    }

    public async Task AuthForFollowers(bool forceVerify)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var scope in _scopesString)
        {
            sb.Append(scope);
            sb.Append("+");
        }
        sb.Remove(sb.Length-1, 1);
        
        string url = $"https://id.twitch.tv/oauth2/authorize?client_id={CLIENT_ID}&force_verify={forceVerify}&redirect_uri={REDIRECT_URI}" +
                     $"&response_type=code&scope={sb}&state={TEST_STATE}";
        
        using Process? authProcess = Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }

    public async Task SetAccessToken(string code)
    {
        var response = await _api.Auth.GetAccessTokenFromCodeAsync(code, CLIENT_SECRET, REDIRECT_URI, CLIENT_ID);
        AccessToken = response.AccessToken;
        RefreshToken = response.RefreshToken;
        Console.WriteLine(AccessToken);
    }

    public async Task<TwitchReportDto> GetReport()
    {
        var videos = await GetVideos();
        var meanStreamingTime = GetMeanStreamingTime(videos);
        var streamAddingFrequency = GetStreamsAddingFrequency(videos);
        var clipsMeanViews = await GetClipsMeanViews();
        var meanViewsCount = GetMeanViewsCount(videos);
        var totalFollowers = await GetFollowers();
        var subsDynamic = GetSubsDynamic(totalFollowers);

        return new TwitchReportDto()
        {
            MeanViews = meanViewsCount,
            MeanClipViews = clipsMeanViews,
            StreamsAddingFrequency = streamAddingFrequency,
            MeanStreamingTime = meanStreamingTime,
            TotalSubs = totalFollowers,
            SubsDynamic = subsDynamic
        };

    }

    private async Task<Video[]> GetVideos()
    {
        var monthAgo = DateTime.Today.AddMonths(-1);
        var userId = await GetBroadcasterId("emongg");
        return (await _api.Helix.Videos.GetVideosAsync(
                userId: userId,
                period: Period.Month,
                accessToken: AccessToken,
                type: VideoType.Archive,
                first: 100)).Videos
            .Where(x => DateTime.Parse(x.CreatedAt, CultureInfo.InvariantCulture) >= monthAgo)
            .ToArray();
    }

    private async Task<long> GetFollowers()
    {
        var userId = await GetBroadcasterId("emongg");
        using HttpClient client = new HttpClient();
        await RefreshAccessToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        client.DefaultRequestHeaders.Add("Client-Id", CLIENT_ID);
        var response = await client.GetAsync(new Uri($"https://api.twitch.tv/helix/channels/followers?broadcaster_id={userId}&access_token={AccessToken}"));
        string responseString = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(responseString);
        return Int64.Parse(json["total"]!.ToString());  
    }

    // взять метрики из бд
    private long GetSubsDynamic(long subsCount)
    {
        return subsCount - subsCount;
    }

    private async Task RefreshAccessToken()
    {
       var response = await _api.Auth.RefreshAuthTokenAsync(RefreshToken, CLIENT_SECRET, CLIENT_ID);

       RefreshToken = response.RefreshToken;
       AccessToken = response.AccessToken;
    }

    private TimeSpan GetMeanStreamingTime(Video[] videos)
    {
        string fullPattern = "H'h'm'm's's'";
        string minutesPattern = "m'm's's'";
        string secondsPattern = "s's'";
        
        return (videos
                    .Select(x =>
                    {
                        DateTime parseResult;

                        return (DateTime.TryParseExact(x.Duration, fullPattern, CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out parseResult) ? parseResult
                            : DateTime.TryParseExact(x.Duration, minutesPattern, CultureInfo.InvariantCulture,
                                DateTimeStyles.None, out parseResult) ? parseResult
                            : DateTime.ParseExact(x.Duration, secondsPattern, CultureInfo.InvariantCulture)).TimeOfDay;
                    })
                    .Aggregate((item, accum) => accum += item) 
                / videos.Length)
            .StripMillisecons();
    }


    private double GetStreamsAddingFrequency(Video[] videos)
    {
        return Math.Round((double)videos.Length / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month), 2);
    }

    private async Task<double> GetClipsMeanViews()
    {
        var startDate = DateTime.Now.AddMonths(-1);
        var endDate = DateTime.Now;

        var clips = (await _api.Helix.Clips.GetClipsAsync(
            broadcasterId: await GetBroadcasterId("emongg"),
            first: 100,
            startedAt: startDate,
            endedAt: endDate,
            accessToken: AccessToken
        )).Clips;

        return Math.Round(clips.Average(x => x.ViewCount),2);
    }

    private double GetMeanViewsCount(Video[] videos)
    {
        return Math.Round(videos.Average(x => x.ViewCount));
    }

    private async Task<string> GetBroadcasterId()
    {
        return (await _api.Helix.Users.GetUsersAsync(accessToken: AccessToken)).Users[0].Id;
    }

    private async Task<string> GetBroadcasterId(string login)
    {
        return (await _api.Helix.Users.GetUsersAsync(accessToken: AccessToken, logins: new() { login })).Users[0].Id;
    }


    public async Task webTest()
    {
        EventSubWebhooks e = new EventSubWebhooks();
        e.OnStreamOnline += Console.WriteLine("hell");
    }

    private void Test(object sender, StreamOnlineArgs args)
    {
        args. 
    }




    // public async Task SetAccessToken(string code)
    // {
    //     var values = new Dictionary<string, string>()
    //     {
    //         { "client_id", CLIENT_ID },
    //         { "client_secret", CLIENT_SECRET },
    //         { "code", code },
    //         { "grant_type", "authorization_code" },
    //         { "redirect_uri", REDIRECT_URI },
    //     };
    //     
    //     var content = new FormUrlEncodedContent(values);
    //     
    //     using HttpClient client = new HttpClient();
    //     var response = await client.PostAsync("https://id.twitch.tv/oauth2/token", content);
    //     var responseString = await response.Content.ReadAsStringAsync();
    //     int a = 12;
    //     
    // }
}