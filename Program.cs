using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MyCoreApp;
using MyCoreApp.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Razor Pages
builder.Services.AddRazorPages();

builder.Services.AddScoped<DB_context>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

//  Логин
app.MapPost("/api/login", async (IUserService userService, HttpContext context,
    [FromForm] string username,
    [FromForm] string password,
    [FromForm] bool rememberMe) =>
{
    if (await userService.ValidateUserAsync(username, password))
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = rememberMe
        };

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        return Results.Ok(new { success = true });
    }

    return Results.Unauthorized();
});

// Регистрация
app.MapPost("/api/register", async (IUserService userService,
    [FromForm] string username,
    [FromForm] string password,
    [FromForm] string email,
    [FromForm] string userType) =>
{
    if (await userService.RegisterUserAsync(username, password, email, userType))
    {
        return Results.Ok(new { success = true });
    }

    return Results.BadRequest(new { error = "Пользователь с таким логином или email уже существует" });
});

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.Run();
