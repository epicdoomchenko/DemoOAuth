using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/getLink", () =>
{
    var link = "https://localhost:5000/connect/authorize"
               + "?client_id=interactive.client"
               + "&client_secret=511536EF-F270-4058-80CA-1C89C192F69A"
               + "&redirect_uri=https://localhost:5002/callback"
               + "&response_type=code"
               + "&scope=openid profile ui_server.client website worker_info"
               + $"&nonce={Guid.NewGuid()}"
               + $"&state={Guid.NewGuid()}";
    return Results.Ok(new { Link = link });
});

app.MapPost("/getToken", async ([FromBody] CodeData codeData, IHttpClientFactory httpClientFactory) =>
{
    var body = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("grant_type", "authorization_code")
        ,new KeyValuePair<string, string>("code", codeData.Code)
        ,new KeyValuePair<string, string>("redirect_uri", "https://localhost:5002/callback")
        ,new KeyValuePair<string, string>("client_id", "interactive.client")
        ,new KeyValuePair<string, string>("client_secret", "511536EF-F270-4058-80CA-1C89C192F69A")
    });

    using var client = httpClientFactory.CreateClient("BackClient");
    var response = await client.PostAsync("https://localhost:5000/connect/token", body);
    var responseContent = await response.Content.ReadAsStringAsync();
    var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent, new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    });
    
    
    return Results.Ok(tokenResponse);
});

app.Run();

public record CodeData(string Code);
public record TokenResponse(string AccessToken);