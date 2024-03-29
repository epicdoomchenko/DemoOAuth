var body = new Dictionary<string, string>
{
    { "client_id", "m2m.client" },
    { "client_secret", "511536EF-F270-4058-80CA-1C89C192F69A" },
    { "grant_type", "client_credentials" },
    { "scope", "m2m.client" }
};

var content = new FormUrlEncodedContent(body);
using var httpClient = new HttpClient();
var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri("https://localhost:5000/connect/token"));
requestMessage.Content = content;
var response = await httpClient.SendAsync(requestMessage);

var responseContent = await response.Content.ReadAsStringAsync();

Console.WriteLine($"Response from authorization server: {responseContent}");