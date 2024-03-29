var body = new Dictionary<string, string>
{
    { "client_id", "console.client" },
    { "client_secret", "511536EF-F270-4058-80CA-1C89C192F69A" },
    { "username", "alice" },
    { "password", "Pass123$" },
    { "grant_type", "password" },
    { "scope", "console.client" }
};

var content = new FormUrlEncodedContent(body);
using var httpClient = new HttpClient();
var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri("https://localhost:5000/connect/token"));
requestMessage.Content = content;
var response = await httpClient.SendAsync(requestMessage);

var responseContent = await response.Content.ReadAsStringAsync();

Console.WriteLine($"Response from authorization server: {responseContent}");