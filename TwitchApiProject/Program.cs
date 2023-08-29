// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Diagnostics;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Core.HttpCallHandlers;
using TwitchLib.Api.Core.Interfaces;
using TwitchLib.Api.Core.RateLimiter;
using TwitchLib.Api.Services;

Console.WriteLine("Hello, World!");

const string CLIENT_ID = "hirrtfyhk37fbgla954bj646i505h0";
const string CLIENT_SECRET = "19le5p7x5uip2zhhev805fm674eyqk";
const string REDIRECT_URI = "http://localhost:3000";
const string TEST_STATE = "c3ab8aa609ea11e793ae92361f002671";

ApiSettings settings = new()
{
    Secret = CLIENT_SECRET,
    ClientId = CLIENT_ID,
};

IRateLimiter limiter = TimeLimiter.GetFromMaxCountByInterval(1000, TimeSpan.MaxValue);
IHttpCallHandler callHandler = new TwitchHttpClient();

List<AuthScopes> scopes = new()
{
    AuthScopes.Channel_Editor
};

TwitchAPI api = new TwitchAPI(rateLimiter: limiter, http: callHandler)
{
    Settings =
    {
        Secret = CLIENT_SECRET,
        ClientId = CLIENT_ID,
        Scopes = scopes
    }
};

string authUrl = api.Auth.GetAuthorizationCodeUrl(REDIRECT_URI, scopes, true, state: TEST_STATE, CLIENT_ID);


using Process? n =  Process.Start(new ProcessStartInfo(authUrl) {UseShellExecute = true});
int g = 12;










