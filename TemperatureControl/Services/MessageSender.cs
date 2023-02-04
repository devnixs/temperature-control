using System.Net.Http.Json;
using System.Text.Json;

namespace TemperatureControl.Services;

public class MessageSender
{
    private readonly IHttpClientFactory _clientFactory;

    public MessageSender(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task SendMessage(string message)
    {
        var url = Environment.GetEnvironmentVariable("WEBHOOK_URL");
        var client = _clientFactory.CreateClient();
        var response = await client.PostAsJsonAsync(url, new
        {
            content = message,
        });
        
    }
}