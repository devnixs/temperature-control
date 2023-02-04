using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace TemperatureControl.Services;

public class InteractionServiceHandler
{
    private readonly InteractionService _ir;
    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordSocketClient _discordSocketClient;

    public InteractionServiceHandler(InteractionService ir, IServiceProvider serviceProvider, DiscordSocketClient discordSocketClient)
    {
        _ir = ir;
        _serviceProvider = serviceProvider;
        _discordSocketClient = discordSocketClient;
    }

    public async Task Initialize()
    {

        _ir.Log += IrOnLog;
        _discordSocketClient.SlashCommandExecuted += DiscordSocketClientOnSlashCommandExecuted;
        _discordSocketClient.Ready += DiscordSocketClientOnReady;
    }

    private async Task DiscordSocketClientOnReady()
    {
        var guildId = ulong.Parse(Environment.GetEnvironmentVariable("GUILD_ID"));

        await _ir.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        
        await _ir.RegisterCommandsGloballyAsync(true);
        await _ir.RegisterCommandsToGuildAsync(guildId, true);
    }

    private async Task DiscordSocketClientOnSlashCommandExecuted(SocketSlashCommand arg)
    {
        var temp = arg.Data.Options.First().Value as string;
        Console.WriteLine(arg.Data);
        await arg.RespondAsync($"J'ai changé la température à {temp?.Replace("°","").Replace("C","")}°C");
    }

    private Task IrOnLog(LogMessage arg)
    {
        Console.WriteLine(arg.Message);
        return Task.CompletedTask;
    }
}