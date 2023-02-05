// See https://aka.ms/new-console-template for more information

using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using TemperatureControl;
using TemperatureControl.Models;
using TemperatureControl.Services;
using RunMode = Discord.Interactions.RunMode;

public class Program
{
    public Program()
    {
        _serviceProvider = CreateProvider();
        DotNetEnv.Env.Load();
    }

    public static Task Main(string[] args) => new Program().MainAsync();
    private readonly IServiceProvider _serviceProvider;

    public async Task MainAsync()
    {
        _serviceProvider.GetRequiredService<LoggingService>().Initialize();      // Start the logging service
        _serviceProvider.GetRequiredService<CommandHandler>().Initialize(); 		// Start the command handler service
        _serviceProvider.GetRequiredService<TemperatureReader>().Initialize();
        await _serviceProvider.GetRequiredService<InteractionServiceHandler>().Initialize(); 		// Start the command handler service
        await _serviceProvider.GetRequiredService<SlashCommandHandler>().Initialize(); 		// Start the command handler service

        await _serviceProvider.GetRequiredService<StartupService>().StartAsync();       // Start the startup service
        await _serviceProvider.GetRequiredService<AcMemory>().Initialize();
        _serviceProvider.GetRequiredService<StatusUpdater>().Initialize();
        
        await Task.Delay(-1);                               // Keep the program alive
    }


    static IServiceProvider CreateProvider()
    {
        var collection = new ServiceCollection();
        collection.AddHttpClient();
        collection.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
        {
            // Add discord to the collection
            LogLevel = LogSeverity.Verbose, // Tell the logger to give Verbose amount of info
            MessageCacheSize = 1000 // Cache 1,000 messages per channel
        }));
        collection.AddTransient<SetTemperatureModule>();
        collection.AddSingleton<InteractionService>(svc => new InteractionService(svc.GetRequiredService<DiscordSocketClient>()));
        collection.AddSingleton(new CommandService(new CommandServiceConfig
            {
                // Add the command service to the collection
                LogLevel = LogSeverity.Verbose, // Tell the logger to give Verbose amount of info
                DefaultRunMode = Discord.Commands.RunMode.Async, // Force all commands to run async by default
            }))
            .AddSingleton<Random>()        
            .AddSingleton<CommandHandler>()
            .AddSingleton<InteractionServiceHandler>()
            .AddSingleton<StartupService>()
            .AddSingleton<SlashCommandHandler>()
            .AddSingleton<AcSender>()
            .AddSingleton<AcMemory>()
            .AddSingleton<MessageSender>()
            .AddSingleton<AcStatus>()
            .AddSingleton<StatusUpdater>()
            .AddSingleton<TemperatureReader>()
            .AddSingleton<LoggingService>();

        return collection.BuildServiceProvider();
    }


}