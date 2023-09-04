using Microsoft.AspNetCore.Mvc;
using TwitchWebApp.Services;

namespace TwitchWebApp.Controllers;

public class HomeController : Controller
{
    private TwitchService _service = new();
    
    public string Auth()
    {
      //  _service.Authorize(); 
        _service.AuthForFollowers(true).ConfigureAwait(false); 
        return  "Auth, i love you";
    }
    
    public async Task<string> Index([FromQuery] string? code)
    {
        if (code != null) 
           await _service.SetAccessToken(code);
        
        return code ?? "There is no code in index";
    }

    public async void GetB()
    {
         await _service.GetFollowers();
        var b = 12;
    }
    
    
}