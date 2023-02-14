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
    public int? DayOfWeek { get; set; }

    public override string ToString()
    {
        string firstPart = $"{HourOfDay}h";
        string secondPart = DayOfWeek.HasValue ? $"- {Days[DayOfWeek.Value]} only" : "";
        string thirdPart = Temperature.HasValue ? Temperature.Value.ToString() : (On.Value ? "On" : "Off");

        return $"{firstPart} {secondPart.PadRight(15, ' ')} : {thirdPart}°C";
    }

    public static readonly string[] Days = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
}