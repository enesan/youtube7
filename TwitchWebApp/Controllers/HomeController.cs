using Microsoft.AspNetCore.Mvc;
using TwitchWebApp.Services;

namespace TwitchWebApp.Controllers;

public class HomeController : Controller
{
    private TwitchService _service = new();
    
    public string Auth()
    {
        _service.Authorize(); 
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
       // var a =  await _service.GetVideos();
        await _service.GetFollowers();
        int aa = 12;
    }
    
    
}