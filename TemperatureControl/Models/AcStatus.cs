namespace TemperatureControl.Models;

public class AcStatus
{
    public bool On { get; set; }
    public int Temperature { get; set; }
    public AcModes Mode { get; set; }

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