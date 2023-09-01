using System.Diagnostics;
using System.Formats.Tar;
using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
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
using TwitchWebApp.Extensions;
using TwitchWebApp.Models;

namespace TwitchWebApp.Services;

public class TwitchService
{
    private const string CLIENT_ID = "hirrtfyhk37fbgla954bj646i505h0";
    private const string CLIENT_SECRET = "19le5p7x5uip2zhhev805fm674eyqk";
    private const string REDIRECT_URI = "http://localhost:3000";
    private const string TEST_STATE = "c3ab8aa609ea11e793ae92361f002671";

    private IRateLimiter _limiter;
    private IHttpCallHandler _callHandler;
    private List<AuthScopes> _scopes;
    private TwitchAPI _api;

    private static string? AccessToken { get; set; }
    private static string? RefreshToken { get; set; }


    public TwitchService()
    {
        _limiter = TimeLimiter.GetFromMaxCountByInterval(1000, TimeSpan.FromDays(10));
        _callHandler = new TwitchHttpClient();

        _scopes = new()
        {
            AuthScopes.Any
        };

        _api = new TwitchAPI(rateLimiter: _limiter, http: _callHandler)
        {
            Settings =
            {
                Secret = CLIENT_SECRET,
                ClientId = CLIENT_ID,
                Scopes = _scopes
            }
        };
    }

    public void Authorize()
    {
        string authUrl = _api.Auth.GetAuthorizationCodeUrl(REDIRECT_URI, _scopes, true, state: TEST_STATE, CLIENT_ID);
        using Process? authProcess = Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });
    }

    public async Task SetAccessToken(string code)
    {
        var response = await _api.Auth.GetAccessTokenFromCodeAsync(code, CLIENT_SECRET, REDIRECT_URI, CLIENT_ID);
        AccessToken = response.AccessToken;
        RefreshToken = response.RefreshToken;
        
    }

    public async Task<TwitchReportDto> GetVideos()
    {
        var monthAgo = DateTime.Today.AddMonths(-1);
        var userId = await GetBroadcasterId("emongg");
        var videos = (await _api.Helix.Videos.GetVideosAsync(
                userId: userId,
                period: Period.Month,
                accessToken: AccessToken,
                type: VideoType.Archive,
                first: 100)).Videos
            .Where(x => DateTime.Parse(x.CreatedAt, CultureInfo.InvariantCulture) >= monthAgo)
            .ToArray();
        
        return new TwitchReportDto()
        {
        };
    }

    public async Task GetFollowers()
    {
        using HttpClient client = new HttpClient();
        var response = await client.GetAsync(new Uri($"https://api.twitch.tv/helix/channels/followers?broadcaster_id=emongg&access_token={AccessToken}"));
        string responseString = await response.Content.ReadAsStringAsync();
        int a = 12;
    }

    private TimeSpan GetMeanStreamingTime(Video[] videos)
    {
        string pattern = "H'h'm'm's's'";

        return (videos
                    .Select(x => DateTime.ParseExact(x.Duration, pattern, CultureInfo.InvariantCulture).TimeOfDay)
                    .Aggregate((item, accum) => accum += item)
                / videos.Length)
            .StripMillisecons();
    }


    private double GetStreamsAddingFrequency(Video[] videos)
    {
        return (double)videos.Length / DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month);
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

        return clips.Average(x => x.ViewCount);
    }

    private double GetMeanViewsCount(Video[] videos)
    {
        return videos.Average(x => x.ViewCount);
    }

    private async Task<string> GetBroadcasterId()
    {
        return (await _api.Helix.Users.GetUsersAsync(accessToken: AccessToken)).Users[0].Id;
    }

    private async Task<string> GetBroadcasterId(string login)
    {
        return (await _api.Helix.Users.GetUsersAsync(accessToken: AccessToken, logins: new() { login })).Users[0].Id;
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