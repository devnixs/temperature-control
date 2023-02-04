using TemperatureControl.Models;

namespace TemperatureControl.Services;

public class AcSender
{
    private readonly AcStatus _acStatus;

    public AcSender(AcStatus acStatus)
    {
        _acStatus = acStatus;
    }

    public async Task SendAcValues()
    {
        
    }
}