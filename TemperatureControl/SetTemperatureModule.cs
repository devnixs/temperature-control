using Discord.Interactions;

namespace TemperatureControl;

public class SetTemperatureModule :  InteractionModuleBase
{
    [SlashCommand("temperature", "Set temperature")]
    public async Task Echo(int input)
    {
        await RespondAsync("OK");
    }
    
    [SlashCommand("on", "Turn on")]
    public async Task On()
    {
        await RespondAsync("OK");
    }
    
    [SlashCommand("off", "Turn off")]
    public async Task Off()
    {
        await RespondAsync();
    }
    
    [SlashCommand("mode", "Switch mode")]
    public async Task Mode([Choice("Hot", "Hot"), Choice("Cold", "Cold"),Choice("Dry", "Dry")] string input)
    {
        await RespondAsync(input);
    }
    
    [SlashCommand("automate", "/automate 22 8 monday")]
    public async Task AddAutomation(
        [Summary("Command", "Can be: on, off, or a temperature")] string command, 
        [Summary("Hour","Hour of the day")] int hour,
        [Summary("DayOfWeek", "Day of week. Can be set to *")] string dayOfWeek)
    {
        await RespondAsync(command);
    }
    
    [SlashCommand("cancel", "Remove a scheduled automation")]
    public async Task Cancel([Summary("AutomationNumber","Automation Number")] int hour)
    {
        await RespondAsync("OK");
    }
    
    
    [SlashCommand("status", "Get Current Status")]
    public async Task Cancel()
    {
        await RespondAsync("OK");
    }
}
