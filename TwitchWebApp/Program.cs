using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();



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