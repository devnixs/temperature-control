using Discord;
using Discord.Interactions;
using Discord.Interactions.Builders;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using SlashCommandBuilder = Discord.Interactions.Builders.SlashCommandBuilder;

namespace TemperatureControl.Services;

public class SlashCommandHandler
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;

    public SlashCommandHandler(DiscordSocketClient discordSocketClient, InteractionService interactionService, IServiceProvider serviceProvider)
    {
        _discordSocketClient = discordSocketClient;
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
    }

    public async Task Initialize()
    {
//        _discordSocketClient.SlashCommandExecuted += DiscordSocketClientOnSlashCommandExecuted;
    }

    // private async Task DiscordSocketClientOnSlashCommandExecuted(SocketSlashCommand arg)
    // {
    //     Console.WriteLine(arg.CommandName);
    //     await arg.RespondAsync("Hello");
    // }

    // private async Task DiscordSocketClientOnReady()
    // {
    //     var guildId = ulong.Parse(Environment.GetEnvironmentVariable("GUILD_ID"));
    //     // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
    //     var guild = _discordSocketClient.GetGuild(guildId);
    //
    //     // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
    //     var guildCommand = new SlashCommandBuilder(new ModuleBuilder(_interactionService, "Test1"), "Test2", Callback);
    //
    //     // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
    //     guildCommand.WithName("temperature");
    //
    //     // Descriptions can have a max length of 100.
    //     guildCommand.WithDescription("Choisir la température du thermostat");
    //
    //     
    //     try
    //     {
    //         // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
    //         await guild.CreateApplicationCommandAsync(guildCommand.);
    //     }
    //     catch(HttpException exception)
    //     {
    //         // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
    //         var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
    //
    //         // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
    //         Console.WriteLine(json);
    //     }
    // }

    // private Task Callback(IInteractionContext context, object[] args, IServiceProvider serviceprovider, ICommandInfo commandinfo)
    // {
    //     _interactionService.ExecuteCommandAsync(context, _serviceProvider);
    // }
}