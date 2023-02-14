using TemperatureControl.Models;

namespace TemperatureControl.Services;

public class AutomationHandler
{
    private readonly AcStatus _acStatus;
    private readonly AcMemory _acMemory;
    private readonly AcSender _acSender;
    private readonly MessageSender _messageSender;

    public AutomationHandler(AcStatus acStatus, AcMemory acMemory, AcSender acSender, MessageSender messageSender)
    {
        _acStatus = acStatus;
        _acMemory = acMemory;
        _acSender = acSender;
        _messageSender = messageSender;
    }

    private int? ParseDay(string day)
    {
        var index = Array.FindIndex(Automation.Days.Select(i => i.ToLowerInvariant()).ToArray(), i => i == day);

        return index >= 0 ? index : null;
    }

    public Automation? ParseAutomation(string action, int hour, string day)
    {
        if (int.TryParse(action, out int temperature) && temperature >= 18 && temperature <= 30)
        {
            return new Automation()
            {
                Temperature = temperature,
                DayOfWeek = ParseDay(day),
                HourOfDay = hour,
            };
        }
        else if (action == "on")
        {
            return new Automation()
            {
                On = true,
                DayOfWeek = ParseDay(day),
                HourOfDay = hour,
            };
        }
        else if (action == "off")
        {
            return new Automation()
            {
                On = false,
                DayOfWeek = ParseDay(day),
                HourOfDay = hour,
            };
        }
        else
        {
            return null;
        }
    }

    private async Task CheckAutomation()
    {
        var time = DateTime.Now;
        var dayOfWeekBase1 = (int)time.DayOfWeek == 0 ? 7 : (int)time.DayOfWeek;
        var hour = time.Hour;
        var minutes = time.Minute;
        if (minutes > 0)
        {
            return;
        }

        for (var index = 0; index < _acStatus.Automations.Length; index++)
        {
            var automation = _acStatus.Automations[index];
            if (automation.HourOfDay == hour && (automation.DayOfWeek == null || automation.DayOfWeek == dayOfWeekBase1))
            {
                await _messageSender.SendMessage($"Automation #{index+1:00} triggered");
                if (automation.On == true)
                {
                    await _acMemory.Start();
                    await _acSender.SendAcValues();
                }
                else if (automation.On == false)
                {
                    await _acMemory.Shutdown();
                    await _acSender.SendAcValues();
                }
                else if (automation.Temperature.HasValue)
                {
                    await _acMemory.SetTemperature(automation.Temperature.Value);
                    await _acSender.SendAcValues();
                }
            }
        }
    }

    public void Initialize()
    {
        var _ = Task.Run(async () =>
        {
            while (true)
            {
                await CheckAutomation();
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        });
    }
}