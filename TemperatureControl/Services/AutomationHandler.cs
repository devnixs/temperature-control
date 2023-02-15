using TemperatureControl.Models;
using DayOfWeek = TemperatureControl.Models.DayOfWeek;

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

    private DayOfWeek? ParseDay(string day)
    {
        if (day.ToLower() == "week")
        {
            return DayOfWeek.MondayFriday;
        }

        if (day.ToLower() == "weekend")
        {
            return DayOfWeek.Weekend;
        }

        if (Enum.TryParse(day, out DayOfWeek parsed))
        {
            return parsed;
        }
        else
        {
            return null;
        }
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

        var isWeekend = dayOfWeekBase1 == 6 || dayOfWeekBase1 == 7;
        var isWeek = dayOfWeekBase1 < 6;
        var hour = time.Hour;
        var minutes = time.Minute;
        if (minutes > 0)
        {
            return;
        }

        var validAutomationsAtThisTime = _acStatus.Automations.Select((a, i) => new { Automation = a, Index = i }).Where((i) =>
            i.Automation.HourOfDay == hour &&
            (i.Automation.DayOfWeek == null ||
             (int) i.Automation.DayOfWeek == dayOfWeekBase1 ||
             i.Automation.DayOfWeek == DayOfWeek.MondayFriday && isWeek ||
             i.Automation.DayOfWeek == DayOfWeek.Weekend && isWeekend
            ));

        var mostSpecific = validAutomationsAtThisTime.MaxBy(i => i.Automation.DayOfWeek.HasValue);

        if (mostSpecific != null)
        {
            await _messageSender.SendMessage($"Automation #{mostSpecific.Index + 1:00} triggered");
            if (mostSpecific.Automation.On == true)
            {
                await _acMemory.Start();
                await _acSender.SendAcValues();
            }
            else if (mostSpecific.Automation.On == false)
            {
                await _acMemory.Shutdown();
                await _acSender.SendAcValues();
            }
            else if (mostSpecific.Automation.Temperature.HasValue)
            {
                await _acMemory.SetTemperature(mostSpecific.Automation.Temperature.Value);
                await _acSender.SendAcValues();
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