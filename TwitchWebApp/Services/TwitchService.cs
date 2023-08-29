using System.Diagnostics;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Core.HttpCallHandlers;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Core.RateLimiter;

namespace TwitchWebApp.Services;

public class TwitchService
{
    private const string CLIENT_ID = "hirrtfyhk37fbgla954bj646i505h0";
    private const string CLIENT_SECRET = "19le5p7x5uip2zhhev805fm674eyqk";
    private const string REDIRECT_URI = "http://localhost:3000";
    private const string TEST_STATE = "c3ab8aa609ea11e793ae92361f002671";
 
    private ApiSettings _settings;
    private IRateLimiter _limiter;
    private IHttpCallHandler _callHandler;
    private List<AuthScopes> _scopes;
    private TwitchAPI _api;

    public string RequestCode { get; set; }

    public TwitchService()
    {
        _settings = new()
        {
            Secret = CLIENT_SECRET,
            ClientId = CLIENT_ID,
        };
        
        _limiter = TimeLimiter.GetFromMaxCountByInterval(1000, TimeSpan.MaxValue);
        _callHandler = new TwitchHttpClient();
        
        _scopes = new()
        {
            AuthScopes.Channel_Editor
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
    

}