using Google.Apis.Oauth2.v2;
using Google.Apis.YouTubeAnalytics.v2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ML.Interfaces;
using ML.Services;
using TwitchWebApp;
using TwitchWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();



builder.Services.AddHttpClient<ITwitchService, ITwitchService>();
builder.Services.AddScoped<ITwitchService, TwitchService>();
builder.Services.AddScoped<IUserAuthService, UserAuthService>();

var app = builder.Build();


app.MapControllers();
app.UseRouting();



app.MapControllerRoute(
    name: "auth",
    pattern: "auth",
    defaults: new {Controller = "Home", Action = "Auth"}
);

app.MapControllerRoute(
    name: "getb",
    pattern: "getb",
    defaults: new { Controller = "Home", Action = "GetB" }
);

app.MapControllerRoute(
    name:"default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.Run();