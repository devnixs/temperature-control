
using System.Text.Json.Serialization;

namespace TemperatureControl.Models;

public class AcStatus
{
    public bool On { get; set; }
    public int Temperature { get; set; }
    public AcModes Mode { get; set; }

    public Automation[] Automations { get; set; } = Array.Empty<Automation>();

    public override string ToString()
    {
        if (On)
        {
            switch (Mode)
            {
                case AcModes.Hot:
                    return $"Chaud - {Temperature}°C";
                    break;
                case AcModes.Cold:
                    return $"Froid - {Temperature}°C";
                    break;
                case AcModes.Dry:
                    return $"Sec - {Temperature}°C";
                    break;
                default:
                    return $"??? - {Temperature}°C";
            }
        }
        else
        {
            return "Off";
        }
    }
}

public class Automation
{
    public int? Temperature { get; set; }
    public bool? On { get; set; }

    public int HourOfDay { get; set; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DayOfWeek? DayOfWeek { get; set; }

    public override string ToString()
    {
        string firstPart = $"{HourOfDay}h";
        string secondPart = DayOfWeek.HasValue ? $"- {DayOfWeek.Value} only" : "";
        string thirdPart = Temperature.HasValue ? Temperature.Value.ToString() : (On.Value ? "On" : "Off");

        return $"{firstPart} {secondPart.PadRight(15, ' ')} : {thirdPart}°C";
    }
}

public enum DayOfWeek
{
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
    Sunday = 7,
    MondayFriday = 8,
    Weekend = 9,
}