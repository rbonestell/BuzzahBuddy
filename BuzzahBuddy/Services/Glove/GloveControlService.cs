using BuzzahBuddy.Models;
using BuzzahBuddy.Services.Bluetooth;

namespace BuzzahBuddy.Services.Glove;

/// <summary>
/// Service for controlling BlueBuzzah glove vibration patterns and settings.
/// </summary>
public class GloveControlService : IGloveControlService
{
    private readonly IBluetoothService _bluetoothService;

    public bool IsVibrating { get; private set; }
    public VibrationPattern? CurrentPattern { get; private set; }

    public event EventHandler<bool>? VibrationStateChanged;

    public GloveControlService(IBluetoothService bluetoothService)
    {
        _bluetoothService = bluetoothService;
    }

    public async Task<bool> StartVibrationAsync(VibrationPattern pattern)
    {
        if (_bluetoothService.CurrentConnectionState != ConnectionState.Connected)
        {
            return false;
        }

        try
        {
            // Build command bytes based on pattern
            // TODO: Update this based on actual BlueBuzzah hardware protocol
            var command = BuildVibrationCommand(pattern, isStart: true);

            var success = await _bluetoothService.WriteCharacteristicAsync(
                BlueBuzzahConstants.PrimaryServiceUuid,
                BlueBuzzahConstants.VibrationControlCharacteristicUuid,
                command);

            if (success)
            {
                IsVibrating = true;
                CurrentPattern = pattern;
                pattern.IsActive = true;
                VibrationStateChanged?.Invoke(this, true);
            }

            return success;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Start vibration error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> StopVibrationAsync()
    {
        if (_bluetoothService.CurrentConnectionState != ConnectionState.Connected)
        {
            return false;
        }

        try
        {
            // Send stop command
            // TODO: Update this based on actual BlueBuzzah hardware protocol
            var command = new byte[] { 0x00 }; // 0x00 = stop

            var success = await _bluetoothService.WriteCharacteristicAsync(
                BlueBuzzahConstants.PrimaryServiceUuid,
                BlueBuzzahConstants.VibrationControlCharacteristicUuid,
                command);

            if (success)
            {
                IsVibrating = false;
                if (CurrentPattern != null)
                {
                    CurrentPattern.IsActive = false;
                }
                CurrentPattern = null;
                VibrationStateChanged?.Invoke(this, false);
            }

            return success;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Stop vibration error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SetIntensityAsync(int intensity)
    {
        if (CurrentPattern == null || !IsVibrating)
        {
            return false;
        }

        // Clamp intensity to valid range
        intensity = Math.Clamp(intensity, 0, 100);

        CurrentPattern.Intensity = intensity;

        // Restart vibration with new intensity
        return await StartVibrationAsync(CurrentPattern);
    }

    public Task<IEnumerable<VibrationPattern>> GetDefaultPatternsAsync()
    {
        var patterns = new List<VibrationPattern>
        {
            new VibrationPattern
            {
                Name = "Gentle",
                Description = "Low intensity, continuous vibration",
                Intensity = 30,
                DurationMs = 1000,
                FrequencyHz = 80,
                IsContinuous = true
            },
            new VibrationPattern
            {
                Name = "Moderate",
                Description = "Medium intensity, continuous vibration",
                Intensity = 50,
                DurationMs = 1000,
                FrequencyHz = 100,
                IsContinuous = true
            },
            new VibrationPattern
            {
                Name = "Strong",
                Description = "High intensity, continuous vibration",
                Intensity = 75,
                DurationMs = 1000,
                FrequencyHz = 120,
                IsContinuous = true
            },
            new VibrationPattern
            {
                Name = "Pulsed",
                Description = "Medium intensity, pulsed vibration",
                Intensity = 50,
                DurationMs = 500,
                FrequencyHz = 100,
                IsContinuous = false,
                IntervalMs = 500
            },
            new VibrationPattern
            {
                Name = "Rapid Pulse",
                Description = "Medium intensity, rapid pulsed vibration",
                Intensity = 60,
                DurationMs = 200,
                FrequencyHz = 120,
                IsContinuous = false,
                IntervalMs = 200
            }
        };

        return Task.FromResult<IEnumerable<VibrationPattern>>(patterns);
    }

    public Task<int?> GetBatteryLevelAsync()
    {
        return _bluetoothService.GetBatteryLevelAsync();
    }

    public async Task<bool> TestConnectionAsync()
    {
        // Send a brief test pulse
        var testPattern = new VibrationPattern
        {
            Name = "Test",
            Intensity = 50,
            DurationMs = 200,
            FrequencyHz = 100,
            IsContinuous = false
        };

        var started = await StartVibrationAsync(testPattern);
        if (started)
        {
            // Wait for the pulse duration
            await Task.Delay(testPattern.DurationMs);
            await StopVibrationAsync();
        }

        return started;
    }

    /// <summary>
    /// Builds a byte array command for controlling vibration.
    /// TODO: Update this based on actual BlueBuzzah hardware protocol specification.
    /// </summary>
    private byte[] BuildVibrationCommand(VibrationPattern pattern, bool isStart)
    {
        if (!isStart)
        {
            return new byte[] { 0x00 }; // Stop command
        }

        // Placeholder command structure
        // Byte 0: Command (0x01 = start)
        // Byte 1: Intensity (0-100)
        // Byte 2-3: Duration (milliseconds, little-endian)
        // Byte 4: Frequency
        // Byte 5: Mode (0x00 = continuous, 0x01 = pulsed)
        var command = new byte[6];
        command[0] = 0x01; // Start command
        command[1] = (byte)Math.Clamp(pattern.Intensity, 0, 100);
        command[2] = (byte)(pattern.DurationMs & 0xFF);
        command[3] = (byte)((pattern.DurationMs >> 8) & 0xFF);
        command[4] = (byte)Math.Clamp(pattern.FrequencyHz, 0, 255);
        command[5] = pattern.IsContinuous ? (byte)0x00 : (byte)0x01;

        return command;
    }
}
