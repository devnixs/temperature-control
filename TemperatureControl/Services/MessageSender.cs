using System.Net.Http.Json;
using System.Text.Json;
using TemperatureControl.Models;

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

    public async Task SendStatus(decimal? temperature, DateTimeOffset? temperatureUpdateDate, int? humidity, DateTimeOffset? humidityUpdateDate, AcStatus status)
    {
        var temperatureElapsedTime = temperatureUpdateDate.HasValue ? DateTimeOffset.UtcNow - temperatureUpdateDate.Value : (TimeSpan?) null;
        var humidityElapsedTime = humidityUpdateDate.HasValue ? DateTimeOffset.UtcNow - humidityUpdateDate.Value : (TimeSpan?) null;
        
        var msg = new DiscordWebhookModel()
        {
            Embeds = new Embed[]
            {
                new Embed()
                {
                    Title = "Status",
                    Description = "",
                    Color = 15258703,
                    Fields = new Field[]
                    {
                        new Field()
                        {
                            Inline = true,
                            Name = "Température",
                            Value = temperature.HasValue ? $"{temperature.Value:00.0}°C {RenderElapsedTime(temperatureElapsedTime)}" : "-",
                        },
                        new Field()
                        {
                            Inline = true,
                            Name = "Humidité",
                            Value = humidity.HasValue ? $"{humidity.Value:00}% {RenderElapsedTime(humidityElapsedTime)}" : "-",
                        },
                        new Field()
                        {
                            Inline = false,
                            Name = "Consigne",
                            Value = status.ToString(),
                        }
                    }
                }
            }
        };

        var url = Environment.GetEnvironmentVariable("WEBHOOK_URL");
        var client = _clientFactory.CreateClient();
        await client.PostAsJsonAsync(url, msg);
    }

    private string RenderElapsedTime(TimeSpan? duration)
    {
        if (duration == null)
        {
            return "- (?)";
        }else if (duration.Value > TimeSpan.FromDays(1))
        {
            return $"- ({duration.Value.Days} jour{(duration.Value.Days > 1 ? "s" : "")})";
        }else if (duration.Value > TimeSpan.FromMinutes(1))
        {
            return $"- ({duration.Value.Minutes} jour{(duration.Value.Minutes > 1 ? "min" : "")})";
        }else
        {
            return $"";
        }
    }
}