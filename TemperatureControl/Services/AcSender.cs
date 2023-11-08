using System.Diagnostics;
using TemperatureControl.Models;

namespace TemperatureControl.Services;

public class AcSender
{
    private readonly AcStatus _acStatus;

    // https://github.com/mattjm/Fujitsu_IR/blob/master/lircd_celcius.conf
    private readonly string[] _commands = new[]
    {
        "dry-on",
        "cool-on",
        "fan-on",
        "turn-off",
        "min-heat",
        "heat-auto-15.5C",
        "heat-auto-16.5C",
        "heat-auto-18C",
        "heat-auto-19C",
        "heat-auto-20C",
        "heat-auto-21C",
        "heat-auto-22C",
        "heat-auto-23.5C",
        "heat-auto-24.5C",
        "heat-auto-25.5C",
        "heat-auto-26.5C",
        "heat-high-15.5C",
        "heat-high-16.5C",
        "heat-high-18C",
        "heat-high-19C",
        "heat-high-20C",
        "heat-high-21C",
        "heat-high-22C",
        "heat-high-23.5C",
        "heat-high-24.5C",
        "heat-medium-15.5C",
        "heat-medium-16.5C",
        "heat-medium-18C",
        "heat-medium-19C",
        "heat-medium-20C",
        "heat-medium-21C",
        "heat-medium-22C",
        "heat-medium-23.5C",
        "heat-medium-24.5C",
        "heat-low-15.5C",
        "heat-low-16.5C",
        "heat-low-18C",
        "heat-low-19C",
        "heat-low-20C",
        "heat-low-21C",
        "heat-low-22C",
        "heat-low-23.5C",
        "heat-low-24.5C",
        "heat-quiet-15.5C",
        "heat-quiet-16.5C",
        "heat-quiet-18C",
        "heat-quiet-19C",
        "heat-quiet-20C",
        "heat-quiet-21C",
        "heat-quiet-22C",
        "heat-quiet-23.5C",
        "heat-quiet-24.5C",
        "cool-auto-18C",
        "cool-auto-19C",
        "cool-auto-20C",
        "cool-auto-21C",
        "cool-auto-22C",
        "cool-auto-23.5C",
        "cool-auto-24.5C",
        "cool-auto-25.5C",
        "cool-auto-26.5C",
        "cool-auto-28C",
        "cool-auto-29C",
        "cool-auto-30C",
        "cool-auto-31C",
        "cool-high-18C",
        "cool-high-19C",
        "cool-high-20C",
        "cool-high-21C",
        "cool-high-22C",
        "cool-high-23.5C",
        "cool-high-24.5C",
        "cool-high-25.5C",
        "cool-high-26.5C",
        "cool-high-28C",
        "cool-high-29C",
        "cool-high-30C",
        "cool-high-31C",
        "cool-quiet-18C",
        "cool-quiet-19C",
        "cool-quiet-20C",
        "cool-quiet-21C",
        "cool-quiet-22C",
        "cool-quiet-23.5C",
        "cool-quiet-24.5C",
        "cool-quiet-25.5C",
        "cool-quiet-26.5C",
        "cool-quiet-28C",
        "cool-quiet-29C",
        "cool-quiet-30C",
        "cool-quiet-31C",
        "cool-medium-18C",
        "cool-medium-19C",
        "cool-medium-20C",
        "cool-medium-21C",
        "cool-medium-22C",
        "cool-medium-23.5C",
        "cool-medium-24.5C",
        "cool-medium-25.5C",
        "cool-medium-26.5C",
        "cool-medium-28C",
        "cool-medium-29C",
        "cool-medium-30C",
        "cool-medium-31C",
        "cool-low-18C",
        "cool-low-19C",
        "cool-low-20C",
        "cool-low-21C",
        "cool-low-22C",
        "cool-low-23.5C",
        "cool-low-24.5C",
        "cool-low-25.5C",
        "cool-low-26.5C",
        "cool-low-28C",
        "cool-low-29C",
        "cool-low-30C",
        "cool-low-31C",
        "fan-auto-18C",
        "fan-high-18C",
        "fan-medium-18C",
        "fan-low-18C",
        "fan-quiet-18C",
        "dry-auto-18C",
        "dry-auto-19C",
        "dry-auto-20C",
        "dry-auto-21C",
        "dry-auto-22C",
        "dry-auto-23.5C",
        "dry-auto-24.5C",
        "dry-auto-25.5C",
        "dry-auto-26.5C",
        "dry-auto-28C",
        "dry-auto-29C",
        "dry-auto-30C",
        "dry-auto-31C",
        "dry-high-18C",
        "dry-high-19C",
        "dry-high-20C",
        "dry-high-21C",
        "dry-high-22C",
        "dry-high-23.5C",
        "dry-high-24.5C",
        "dry-high-25.5C",
        "dry-high-26.5C",
        "dry-high-28C",
        "dry-high-29C",
        "dry-high-30C",
        "dry-high-31C",
        "dry-medium-18C",
        "dry-medium-19C",
        "dry-medium-20C",
        "dry-medium-21C",
        "dry-medium-22C",
        "dry-medium-23.5C",
        "dry-medium-24.5C",
        "dry-medium-25.5C",
        "dry-medium-26.5C",
        "dry-medium-28C",
        "dry-medium-29C",
        "dry-medium-30C",
        "dry-medium-31C",
        "dry-low-18C",
        "dry-low-19C",
        "dry-low-20C",
        "dry-low-21C",
        "dry-low-22C",
        "dry-low-23.5C",
        "dry-low-24.5C",
        "dry-low-25.5C",
        "dry-low-26.5C",
        "dry-low-28C",
        "dry-low-29C",
        "dry-low-30C",
        "dry-low-31C",
        "dry-quiet-18C",
        "dry-quiet-19C",
        "dry-quiet-20C",
        "dry-quiet-21C",
        "dry-quiet-22C",
        "dry-quiet-23.5C",
        "dry-quiet-24.5C",
        "dry-quiet-25.5C",
        "dry-quiet-26.5C",
        "dry-quiet-28C",
        "dry-quiet-29C",
        "dry-quiet-30C",
        "dry-quiet-31C",
    };

    public AcSender(AcStatus acStatus)
    {
        _acStatus = acStatus;
    }

    private string? GetKeyFromStatus()
    {
        if (!_acStatus.On)
        {
            return "turn-off";
        }

        var mode = _acStatus.Mode switch
        {
            AcModes.Hot => "heat",
            AcModes.Cold => "cool",
            AcModes.Dry => "dry",
            _ => ""
        };

        var temperatures = new decimal[] { _acStatus.Temperature, _acStatus.Temperature + 0.5m, _acStatus.Temperature - 0.5m };

        foreach (var temperature in temperatures)
        {
            var key = $"{mode}-low-{temperature}C";
            Console.WriteLine($"Looking for command {key}");
            var command = this._commands.FirstOrDefault(k => k == key);
            if (command != null)
            {
                return command;
            }
        }

        return null;
    }

    public async Task SendAcValues(ActionType actionType)
    {
        Console.WriteLine("Sending raw command: "+ actionType);
        var command = GetKeyFromStatus();

        if (command == null)
        {
            Console.WriteLine("Could not find command matching current status");
            return;
        }

        // If we're turning on to a cool-request, we need to send a cool-on first
        if (actionType == ActionType.TurnOn && _acStatus.Mode == AcModes.Cold)
        {
            Console.WriteLine("We're turning on to a cool-request, so we need to send a cool-on first");
            await SendRawCommand("cool-on");
            await Task.Delay(2000);
        }

        await SendRawCommand(command);
    }

    private static async Task SendRawCommand(string command)
    {
        var raw = "send_once fujitsu_heat_ac " + command;
        var psi = new ProcessStartInfo
        {
            FileName = "irsend",
            Arguments = raw,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        Console.WriteLine("Sending command " + raw);

        var proc = new Process
        {
            StartInfo = psi
        };

        proc.Start();

        await Task.WhenAll(Task.Run(() =>
        {
            while (!proc.StandardOutput.EndOfStream)
            {
                var line = proc.StandardOutput.ReadLine();
                Console.WriteLine(line);
            }
        }), Task.Run(() =>
        {
            while (!proc.StandardError.EndOfStream)
            {
                var line = proc.StandardError.ReadLine();
                Console.WriteLine(line);
            }
        }));

        await proc.WaitForExitAsync();
        Console.WriteLine(proc.ExitCode);
    }
}

public enum ActionType
{
    TurnOn,
    TurnOff,
    ChangeTemperature,
    ChangeMode,
}