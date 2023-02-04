using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace TemperatureControl.Services;

public class CommandHandler
{
    private readonly DiscordSocketClient _discord;
    private readonly CommandService _commands;
    private readonly IServiceProvider _provider;

    public CommandHandler(
        DiscordSocketClient discord,
        CommandService commands,
        IServiceProvider provider)
    {
        _discord = discord;
        _commands = commands;
        _provider = provider;
    }

    public void Initialize()
    {
        _discord.MessageReceived += OnMessageReceivedAsync;
    }
        
    private async Task OnMessageReceivedAsync(SocketMessage s)
    {
        var msg = s as SocketUserMessage;     // Ensure the message is from a user/bot
        if (msg == null) return;
        if (msg.Author.Id == _discord.CurrentUser.Id) return;     // Ignore self when checking commands
            
        var context = new SocketCommandContext(_discord, msg);     // Create the command context

        int argPos = 0;     // Check if the message has a valid command prefix
        if (msg.HasStringPrefix("!", ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
        {
            var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command

            if (!result.IsSuccess)     // If not successful, reply with the error.
                await context.Channel.SendMessageAsync(result.ToString());
        }
    }
}