using Discord.Interactions;

namespace TemperatureControl;

public class SetTemperatureModule :  InteractionModuleBase
{
    [SlashCommand("temperature", "Choisir la température")]
    public async Task Echo(int input)
    {
        await RespondAsync("OK");
    }
    
    [SlashCommand("on", "Allumer")]
    public async Task On()
    {
        await RespondAsync("OK");
    }
    
    [SlashCommand("off", "Eteindre")]
    public async Task Off()
    {
        await RespondAsync();
    }
    
    [SlashCommand("mode", "Changer de mode")]
    public async Task Mode([Choice("Chaud", "Hot"), Choice("Froid", "Cold"),Choice("Sec", "Dry")] string input)
    {
        await RespondAsync(input);
    }
}
