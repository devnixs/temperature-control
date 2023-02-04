using Newtonsoft.Json;
using TemperatureControl.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TemperatureControl.Services;

public class AcMemory
{
    private readonly AcStatus _acStatus;
    private const string Filename = "./ac-status.json";

    public AcMemory(AcStatus acStatus)
    {
        _acStatus = acStatus;
    }

    public async Task Initialize()
    {
        await LoadSavedData();
    }

    private async Task LoadSavedData()
    {
        try
        {
            if (File.Exists(Filename))
            {
                var content = await File.ReadAllTextAsync(Filename);
                var data = JsonSerializer.Deserialize<AcStatus>(content);
                if (data != null)
                {
                    _acStatus.On = data.On;
                    _acStatus.Temperature = data.Temperature;
                    _acStatus.Mode = data.Mode;
                }
            }
            InitDefaultValues();
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to deserialize data");
            File.Delete(Filename);
            InitDefaultValues();
        }
    }

    private void InitDefaultValues()
    {
        if (DateTimeOffset.UtcNow.Month < 5 || DateTimeOffset.UtcNow.Month > 10)
        {
            Console.WriteLine("Initialized default winter values");
            _acStatus.Mode = AcModes.Hot;
            _acStatus.On = true;
            _acStatus.Temperature = 22;
        }
        else
        {
            Console.WriteLine("Initialized default summer values");
            _acStatus.Mode = AcModes.Cold;
            _acStatus.On = true;
            _acStatus.Temperature = 25;
        }
    }

    private async Task SaveData()
    {
        try
        {
            var data = JsonSerializer.Serialize(_acStatus);
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
        _acStatus.Temperature = temperature;
        await SaveData();
    }

    public async Task Start()
    {
        _acStatus.On = true;
        await SaveData();
    }

    public async Task Shutdown()
    {
        _acStatus.On = false;
        await SaveData();
    }

    public async Task SetMode(AcModes mode)
    {
        _acStatus.Mode = mode;
        await SaveData();
    }
}