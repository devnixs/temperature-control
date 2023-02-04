using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace TemperatureControl.Services;

public class StartupService
{
    private readonly IServiceProvider _provider;
    private readonly DiscordSocketClient _discord;
    private readonly CommandService _commands;

    public StartupService(
        IServiceProvider provider,
        DiscordSocketClient discord,
        CommandService commands)
    {
        _provider = provider;
        _discord = discord;
        _commands = commands;
    }

    public async Task StartAsync()
    {
        string discordToken = Environment.GetEnvironmentVariable("BOT_TOKEN");     // Get the discord token from the config file
        if (string.IsNullOrWhiteSpace(discordToken))
            throw new Exception("Please enter your bot's token into the `_configuration.json` file found in the applications root directory.");

        await _discord.LoginAsync(TokenType.Bot, discordToken);     // Login to discord
        await _discord.StartAsync();                                // Connect to the websocket

        _discord.Log += Log;
        
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);     // Load commands and modules into the command service
    }
    
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}