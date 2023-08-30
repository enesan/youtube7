using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Core.HttpCallHandlers;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Core.RateLimiter;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

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

    public static string? AccessToken { private get; set; }
    public static string? RefreshToken { private get; set; }

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
        using Process? authProcess =  Process.Start(new ProcessStartInfo(authUrl) {UseShellExecute = true});
    }

    public async Task SetAccessToken(string code)
    {
        var response = await _api.Auth.GetAccessTokenFromCodeAsync(code, CLIENT_SECRET, REDIRECT_URI, CLIENT_ID);
        AccessToken = response.AccessToken;
        RefreshToken = response.RefreshToken;


        // var values = new Dictionary<string, string>()
        // {
        //     { "client_id", CLIENT_ID },
        //     { "client_secret", CLIENT_SECRET },
        //     { "code", code },
        //     { "grant_type", "authorization_code" },
        //     { "redirect_uri", REDIRECT_URI },
        // };
        //
        // var jsonDictionary = JsonConvert.SerializeObject(values);
        //
        // using HttpClient client = new HttpClient();
        // var response = await client.PostAsync("https://id.twitch.tv/oauth2/token",
        //         new StringContent(jsonDictionary, Encoding.UTF8, "application/json"));
    }

    public async Task GetVideo()
    {
        var videos = await _api.Helix.Videos.GetVideosAsync(
            userId: await GetBroadcasterId(),
            period: Period.Month,
            accessToken: AccessToken);
    }

    private async Task<string> GetBroadcasterId()
    {
        return (await _api.Helix.Users.GetUsersAsync(accessToken: AccessToken)).Users[0].Id; 
    } 
    

}