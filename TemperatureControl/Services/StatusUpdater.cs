using TemperatureControl.Models;

namespace TemperatureControl.Services;

public class StatusUpdater
{
    private readonly TemperatureReader _temperatureReader;
    private readonly MessageSender _messageSender;
    private readonly AcStatus _acStatus;

    public StatusUpdater(TemperatureReader temperatureReader, MessageSender messageSender, AcStatus acStatus)
    {
        _temperatureReader = temperatureReader;
        _messageSender = messageSender;
        _acStatus = acStatus;
    }

    public async Task SendStatus()
    {
        var readings = _temperatureReader.Read();
        await _messageSender.SendStatus(readings.Temperature,
            readings.TemperatureLastUpdateDate,
            readings.Humidity,
            readings.HumidityLastUpdateDate,
            _acStatus);

        // Health Check
        try
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(10);
            await client.GetAsync("https://hc-ping.com/3b3f7596-8d96-48dc-bad9-e314acd67188");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ping failed: {ex.Message}");
        }
    }

    public void Initialize()
    {
        var _ = Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromMinutes(1));
            while (true)
            {
                await SendStatus();
                await Task.Delay(TimeSpan.FromHours(1));
            }
        });
    }
}