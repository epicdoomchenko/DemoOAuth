using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpLogging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpLogging(f => { f.LoggingFields = HttpLoggingFields.All; });

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("cookie");

var app = builder.Build();

app.UseHttpLogging();

app.MapGet("/", async context =>
{
    if (context.Request.Cookies.TryGetValue("access_token", out var token))
    {
        var claims = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(token.Split('.')[1]));
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(claims);
    }
    else
    {
        var redirectUrl = "https://localhost:5000/connect/authorize"
                          + "?client_id=api.client"
                          + "&redirect_uri=https://localhost:5001/callback"
                          + "&response_type=id_token token"
                          + "&scope=openid implicit"
                          + $"&nonce={Guid.NewGuid()}"
                          + $"&state={Guid.NewGuid()}"
                          + "&response_mode=form_post";
        context.Response.Redirect(redirectUrl);
    }
});

app.MapPost("/callback", async context =>
{
    context.Request.EnableBuffering();
    context.Request.Body.Position = 0;

    var rawRequestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
    var parsed = HttpUtility.ParseQueryString(rawRequestBody);
    context.Response.Cookies.Append("access_token", parsed.Get("access_token")!);
    await context.AuthenticateAsync("cookie");
    context.Response.Redirect("/");
});

app.Run();