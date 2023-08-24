using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;

namespace YoutubeApi7;

public class UserAuthService
{
    public UserAuthService()
    {
       
    }

    public static async Task<UserCredential> AuthorizeUser(ClientSecrets secrets, string[] scopes, string userName)
    {
        return await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scopes, userName, CancellationToken.None);
    }

    public static async Task Authorize2()
    {
    }
}