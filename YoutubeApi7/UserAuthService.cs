using System.Diagnostics;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Google.Apis.YouTubeReporting.v1;

namespace YoutubeApi7;

public class UserAuthService
{
    
    private const string REDIRECT_URI = "http://localhost:3000";
    public UserAuthService()
    {
       
    }

    private LocalServerCodeReceiver e = new LocalServerCodeReceiver();

    public static async Task<UserCredential> AuthorizeUser(ClientSecrets secrets, IEnumerable<string> scopes, string userName)
    {
        return await GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, scopes, userName, CancellationToken.None);
    }
    

}