using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace TemperatureControl.Services;

public class LoggingService
{
    private readonly DiscordSocketClient _discord;
    private readonly CommandService _commands;

    public LoggingService(DiscordSocketClient discord, CommandService commands)
    {
        _discord = discord;
        _commands = commands;
    }

    public void Initialize()
    {
        _discord.Log += OnLogAsync;
        _commands.Log += OnLogAsync;
    }
        
    private Task OnLogAsync(LogMessage msg)
    {
        string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
        return Console.Out.WriteLineAsync(logText);       // Write the log text to the console
    }
}