using Discord.Interactions;

namespace TemperatureControl;

public class SetTemperatureModule :  InteractionModuleBase
{
    [SlashCommand("temperature", "Choisir la température")]
    public async Task Echo(string input)
    {
        await RespondAsync("OK");
    }
    
    [SlashCommand("on", "Allumer")]
    public async Task On( )
    {
        await RespondAsync("OK");
    }
    
    [SlashCommand("off", "Eteindre")]
    public async Task Off()
    {
        await RespondAsync();
    }
    
    
    [SlashCommand("mode", "Changer de mode")]
    public async Task Mode([Choice("Chaud", "hot"), Choice("Froid", "cold"),Choice("Sec", "dry")] string input)
    {
        await RespondAsync(input);
    }
}
