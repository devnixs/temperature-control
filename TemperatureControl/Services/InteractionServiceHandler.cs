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
    private readonly AcSender _acSender;
    private readonly StatusUpdater _statusUpdater;
    private readonly AutomationHandler _automationHandler;

    public InteractionServiceHandler(InteractionService ir,
        IServiceProvider serviceProvider,
        DiscordSocketClient discordSocketClient,
        AcMemory acMemory,
        AcStatus status,
        MessageSender messageSender,
        AcSender acSender,
        StatusUpdater statusUpdater,
        AutomationHandler automationHandler)
    {
        _ir = ir;
        _serviceProvider = serviceProvider;
        _discordSocketClient = discordSocketClient;
        _acMemory = acMemory;
        _status = status;
        _messageSender = messageSender;
        _acSender = acSender;
        _statusUpdater = statusUpdater;
        _automationHandler = automationHandler;
    }

    public async Task Initialize()
    {
        _ir.Log += IrOnLog;
        _discordSocketClient.SlashCommandExecuted += DiscordSocketClientOnSlashCommandExecuted;
        _discordSocketClient.Ready += DiscordSocketClientOnReady;
    }

    private async Task DiscordSocketClientOnReady()
    {
        await _ir.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        await _ir.RegisterCommandsGloballyAsync(true);

        await _messageSender.SendMessage($"OppyBot - Connecté! ✅");
    }

    private async Task DiscordSocketClientOnSlashCommandExecuted(SocketSlashCommand arg)
    {
        object? parameter1 = arg.Data.Options?.FirstOrDefault()?.Value;
        object? parameter2 = arg.Data.Options?.Count > 1 ? arg.Data.Options?.ElementAt(1)?.Value : null;
        object? parameter3 = arg.Data.Options?.Count > 2 ? arg.Data.Options?.ElementAt(2)?.Value : null;

        if (arg.CommandName == "on")
        {
            await _acMemory.Start();
            await _acSender.SendAcValues();
            await arg.RespondAsync($":thumbsup:");
        }
        else if (arg.CommandName == "off")
        {
            await _acMemory.Shutdown();
            await _acSender.SendAcValues();
            await arg.RespondAsync($":thumbsup:");
        }
        else if (arg.CommandName == "mode" && parameter1 is string parameterstring)
        {
            if (Enum.TryParse(parameterstring, true, out AcModes value))
            {
                await _acMemory.SetMode(value);
                await _acSender.SendAcValues();
                await arg.RespondAsync($":thumbsup:");
            }
            else
            {
                await arg.RespondAsync($"Je n'ai pas compris.");
            }
        }
        else if (arg.CommandName == "temperature" && parameter1 is long parameterlong and >= 18 and <= 30)
        {
            await _acMemory.SetTemperature((int)parameterlong);
            await _acSender.SendAcValues();
            await arg.RespondAsync($":thumbsup:");
        }
        else if (arg.CommandName == "automate" && parameter1 is string parameter1string && parameter2 is long parameter2long &&
                 parameter3 is string parameter3long)
        {
            var automation = _automationHandler.ParseAutomation(parameter1string, (int)parameter2long, parameter3long);
            if (automation != null)
            {
                await arg.RespondAsync($":thumbsup:");
                await _acMemory.AddAutomation(automation);
                await _statusUpdater.SendStatus();
            }
            else
            {
                await arg.RespondAsync($"Je n'ai pas compris.");
            }
        }
        else if (arg.CommandName == "cancel" && parameter1 is long parameter1long)
        {
            await arg.RespondAsync($":thumbsup:");
            await _acMemory.RemoveAutomation((int)parameter1long - 1);
            await _statusUpdater.SendStatus();
        }
        else if (arg.CommandName == "query")
        {
            await _statusUpdater.SendStatus();
            await arg.RespondAsync($":thumbsup:");
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