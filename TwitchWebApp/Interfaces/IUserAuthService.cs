
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;

namespace ML.Interfaces;

public interface IUserAuthService
{
     string AuthorizeTwitch(IList<string> scopes, string clientId, string redirectUri, string testState,
        bool forceVerify = true)
    {
        throw new NotImplementedException("Method isn't implemented rather in authorize service");
    }

     Task<UserCredential> AuthorizeYoutube(ClientSecrets secrets, string[] scopes, string userName);
     string GetYoutubeAuthLink(string clientId, string redirectUri, string state);
}