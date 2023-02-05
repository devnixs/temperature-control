using System.Device.Gpio;
using System.Text.Json;
using Iot.Device.DHTxx;
using UnitsNet;

namespace TemperatureControl.Services;

public class TemperatureReader
{
    private readonly Dht11 _dht11;

    private int? _humidity;
    private DateTimeOffset? _humidityLastUpdateDate;
    private decimal? _temperature;
    private DateTimeOffset? _temperatureLastUpdateDate;

    public TemperatureReader()
    {
        _dht11 = new Dht11(13, PinNumberingScheme.Logical);
    }

    public TemperatureAndHumidityReadings Read()
    {
        return new TemperatureAndHumidityReadings
        {
            Humidity = _humidity,
            Temperature = _temperature,
            HumidityLastUpdateDate = _humidityLastUpdateDate,
            TemperatureLastUpdateDate = _temperatureLastUpdateDate,
        };
    }

    public void Initialize()
    {
        var _ = Task.Run(async () =>
        {
            while (true)
            {
                if (_dht11.TryReadHumidity(out RelativeHumidity h))
                {
                    _humidity = (int)h.Percent;
                    _humidityLastUpdateDate = DateTimeOffset.UtcNow;
                }

                if (_dht11.TryReadTemperature(out Temperature t))
                {
                    _temperature = (decimal)t.DegreesCelsius;
                    _temperatureLastUpdateDate = DateTimeOffset.UtcNow;
                }
                await Task.Delay(TimeSpan.FromSeconds(30));
            }
        });
    }
}

public class TemperatureAndHumidityReadings
{
    public int? Humidity { get; set; }
    public DateTimeOffset? HumidityLastUpdateDate { get; set; }
    public decimal? Temperature { get; set; }
    public DateTimeOffset? TemperatureLastUpdateDate { get; set; }
}