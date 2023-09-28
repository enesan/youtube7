using System.Diagnostics;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.YouTubeAnalytics.v2;
using ML.Interfaces;

namespace ML.Services;

public class UserAuthService : IUserAuthService
{
    public UserAuthService()
    {
    }

    public async Task<UserCredential> AuthorizeYoutube(ClientSecrets secrets, string[] scopes, string userName)
    {
        return await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scopes, userName, CancellationToken.None);
    }


    public string AuthorizeTwitch(IList<string> scopes, string clientId, string redirectUri, string testState,
        bool forceVerify = true)
    {
        StringBuilder sb = new();
        foreach (var scope in scopes)
        {
            sb.Append(scope);
            sb.Append("+");
        }

        sb.Remove(sb.Length - 1, 1);

        return
            $"https://id.twitch.tv/oauth2/authorize?client_id={clientId}&force_verify={forceVerify}&redirect_uri={redirectUri}" +
            $"&response_type=code&scope={sb}&state={testState}";
    }

    public string GetYoutubeAuthLink(string clientId, string redirectUri, string state)
    {
        string[] scopes =
        {
            YouTubeAnalyticsService.ScopeConstants.YoutubeReadonly,
            Oauth2Service.ScopeConstants.UserinfoProfile,
            Oauth2Service.ScopeConstants.Openid
        };
        return $"https://accounts.google.com/o/oauth2/v2/auth?scope={string.Join("%20", scopes)}" +
               $"&access_type=offline" +
               $"&include_granted_scopes=true" +
               $"&response_type=code" +
               $"&state={state}" +
               $"&redirect_uri={redirectUri}" +
               $"&client_id={clientId}";
    }
}