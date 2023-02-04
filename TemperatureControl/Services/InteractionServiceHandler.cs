using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using TemperatureControl.Models;

namespace TemperatureControl.Services;

public class InteractionServiceHandler
{
    private readonly InteractionService _ir;
    private readonly IServiceProvider _serviceProvider;
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly AcMemory _acMemory;
    private readonly AcStatus _status;
    private readonly MessageSender _messageSender;

    public InteractionServiceHandler(InteractionService ir,
        IServiceProvider serviceProvider,
        DiscordSocketClient discordSocketClient,
        AcMemory acMemory, AcStatus status, MessageSender messageSender)
    {
        _ir = ir;
        _serviceProvider = serviceProvider;
        _discordSocketClient = discordSocketClient;
        _acMemory = acMemory;
        _status = status;
        _messageSender = messageSender;
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

        await _messageSender.SendMessage($"OppyBot - Prêt! \n{_status}");
    }

    private async Task DiscordSocketClientOnSlashCommandExecuted(SocketSlashCommand arg)
    {
        object? parameter = arg.Data.Options?.FirstOrDefault()?.Value;
        if (arg.CommandName == "on")
        {
            await _acMemory.Start();
            await arg.RespondAsync(_status.ToString());
        }
        else if (arg.CommandName == "off")
        {
            await _acMemory.Shutdown();
            await arg.RespondAsync(_status.ToString());
        }
        else if (arg.CommandName == "mode" && parameter is string parameterstring)
        {
            if (Enum.TryParse(parameterstring, true, out AcModes value))
            {
                await _acMemory.SetMode(value);
                await arg.RespondAsync(_status.ToString());
            }
            else
            {
                await arg.RespondAsync($"Je n'ai pas compris.");
            }
        }
        else if (arg.CommandName == "temperature" && parameter is long parameterlong)
        {
            await _acMemory.SetTemperature((int)parameterlong);
            await arg.RespondAsync(_status.ToString());
        }
        else
        {
            await arg.RespondAsync($"Je n'ai pas compris.");
        }

        Console.WriteLine(arg.Data);
    }

    private Task IrOnLog(LogMessage arg)
    {
        Console.WriteLine(arg.Message);
        return Task.CompletedTask;
    }
}