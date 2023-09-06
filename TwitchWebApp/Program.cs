using Microsoft.AspNetCore.Mvc;
using TwitchWebApp;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddHttpClient<ITwitchService, ITwitchService>();

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