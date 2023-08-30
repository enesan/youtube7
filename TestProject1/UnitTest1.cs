using System.Diagnostics;
using TwitchLib.Api;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Core.HttpCallHandlers;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Core.RateLimiter;

namespace TestProject1;

[TestClass]
public class UnitTest1
{
    const string CLIENT_ID = "hirrtfyhk37fbgla954bj646i505h0";
    const string CLIENT_SECRET = "19le5p7x5uip2zhhev805fm674eyqk";
    const string REDIRECT_URI = "http://localhost:3000";
    const string TEST_STATE = "c3ab8aa609ea11e793ae92361f002671";
    
    static IRateLimiter limiter = TimeLimiter.GetFromMaxCountByInterval(1000, TimeSpan.MaxValue);
    static IHttpCallHandler callHandler = new TwitchHttpClient();

    static List<AuthScopes> scopes = new()
    {
        AuthScopes.Channel_Editor
    };

    static TwitchAPI api = new TwitchAPI(rateLimiter: limiter, http: callHandler)
    {
        Settings =
        {
            Secret = CLIENT_SECRET,
            ClientId = CLIENT_ID,
            Scopes = scopes
        }
    };

    string authUrl = api.Auth.GetAuthorizationCodeUrl(REDIRECT_URI, scopes, true, state: TEST_STATE, CLIENT_ID);
    

}