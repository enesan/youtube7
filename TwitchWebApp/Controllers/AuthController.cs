using System.ComponentModel.DataAnnotations;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Oauth2.v2;
using Google.Apis.YouTubeAnalytics.v2;
using Microsoft.AspNetCore.Mvc;
using ML.Interfaces;
using TwitchLib.Api;
using TwitchLib.Api.Core.Interfaces;

namespace ML.Controllers;

public class AuthController : Controller
{
    private IUserAuthService _service;

    private const string CLIENT_ID = "hirrtfyhk37fbgla954bj646i505h0";
    private const string CLIENT_SECRET = "19le5p7x5uip2zhhev805fm674eyqk";
    private const string REDIRECT_URI = "http://localhost:3000";
    private const string TEST_STATE = "c3ab8aa609ea11e793ae92361f002671";


    private IRateLimiter _limiter;
    private IHttpCallHandler _callHandler;

    private List<string> _scopesString = new()
    {
        "moderator:read:followers"
    };

    private TwitchAPI _api;

    #region youtube

    const string SECRET = "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"; // const

    const string YOUTUBE_CLIENT_ID =
        "915496023150-p3r9stpk2ehmi474lp4orcurg67q3tct.apps.googleusercontent.com"; // const

    const string API_KEY = "AIzaSyA_HckYImRn8zMOGks4tLVH2zsM1FTVWMA"; // const
    string username = "ashifаyfsh";


    string[] _youtubeScopes =
    {
        YouTubeAnalyticsService.ScopeConstants.YoutubeReadonly,
        Oauth2Service.ScopeConstants.UserinfoProfile,
        Oauth2Service.ScopeConstants.Openid,
    };

    ClientSecrets secrets = new ClientSecrets()
    {
        ClientId = YOUTUBE_CLIENT_ID,
        ClientSecret = SECRET
    };

    #endregion

    public AuthController(IUserAuthService service)
    {
        _service = service;
    }

    public IActionResult Twitch()
    {
        return Redirect(_service.AuthorizeTwitch(_scopesString, CLIENT_ID, REDIRECT_URI, TEST_STATE, false));
    }

    [HttpGet]
    public IActionResult YoutubeAuth()
    {
        // отсюда надо забирать токены
        return Redirect(_service.GetYoutubeAuthLink(YOUTUBE_CLIENT_ID, REDIRECT_URI + "/Auth/Youtube", "adsfsf"));
    }

    
    public async Task<IActionResult> Youtube([FromQuery] string? code)
    {
        string res = $"https://oauth2.googleapis.com/token?" +
                     $"code={code}" +
                     $"&client_id={YOUTUBE_CLIENT_ID}" +
                     $"&client_secret={SECRET}" +
                     $"&redirect_uri={REDIRECT_URI}" +
                     $"&grant_type=authorization_code";

        using HttpClient c = new HttpClient();
        HttpContent content = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
        {
            new("code", code),
            new("client_id", YOUTUBE_CLIENT_ID),
            new("client_secret", SECRET),
            new("redirect_uri", REDIRECT_URI),
            new("grant_type", "authorization_code"),
        });

        var response = await c.PostAsync("https://oauth2.googleapis.com/token", content);
        if (response.IsSuccessStatusCode)
        {
            int b = 12;
        }
            

            return Redirect(res);
    }
}