using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TemperatureControl.Services;

public class AcMemory
{
    private const string Filename = "./ac-status.json";
    public bool On { get; set; }
    public int Temperature { get; set; }
    public AcModes Mode { get; set; }

    public AcMemory()
    {
    }

    private async Task LoadSavedData()
    {
        try
        {
            if (File.Exists(Filename))
            {
                var content = await File.ReadAllTextAsync(Filename);
                var data = JsonSerializer.Deserialize<AcMemory>(content);
                if (data != null)
                {
                    this.On = data.On;
                    this.Temperature = data.Temperature;
                    this.Mode = data.Mode;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to deserialize data");
            File.Delete(Filename);
        }
    }

    private async Task SaveData()
    {
        try
        {
            var data = JsonSerializer.Serialize(this);
            await File.WriteAllTextAsync(Filename, data);
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to deserialize data");
            File.Delete(Filename);
        }
    }

    public async Task SetTemperature(int temperature)
    {
        this.Temperature = temperature;
        await SaveData();
    }

    public async Task Start()
    {
        this.On = true;
        await SaveData();
    }

    public async Task Shutdown()
    {
        this.On = false;
        await SaveData();
    }

    public async Task SetMode(AcModes mode)
    {
        this.Mode = mode;
        await SaveData();
    }
}

public enum AcModes
{
    Hot,
    Cold,
    Dry,
}