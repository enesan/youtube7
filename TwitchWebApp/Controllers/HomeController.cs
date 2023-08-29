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
    
    public string Index([FromQuery] string? code)
    {
        if (code != null)
            _service.RequestCode = code;

        return code ?? "There is no code in index";
    }
}