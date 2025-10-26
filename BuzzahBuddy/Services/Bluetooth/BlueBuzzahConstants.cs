namespace BuzzahBuddy.Services.Bluetooth;

/// <summary>
/// Constants for BlueBuzzah glove Bluetooth Low Energy communication.
/// These UUIDs should be updated once the actual hardware specifications are available.
/// </summary>
public static class BlueBuzzahConstants
{
    /// <summary>
    /// The device name prefix for BlueBuzzah gloves.
    /// Used to filter devices during scanning.
    /// </summary>
    public const string DeviceNamePrefix = "BlueBuzzah";

    /// <summary>
    /// Primary service UUID for BlueBuzzah glove communication.
    /// TODO: Update with actual UUID from hardware specifications.
    /// </summary>
    public static readonly Guid PrimaryServiceUuid = Guid.Parse("00001800-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// Characteristic UUID for vibration control.
    /// TODO: Update with actual UUID from hardware specifications.
    /// </summary>
    public static readonly Guid VibrationControlCharacteristicUuid = Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// Characteristic UUID for reading battery level.
    /// TODO: Update with actual UUID from hardware specifications.
    /// </summary>
    public static readonly Guid BatteryLevelCharacteristicUuid = Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// Characteristic UUID for device status and firmware information.
    /// TODO: Update with actual UUID from hardware specifications.
    /// </summary>
    public static readonly Guid DeviceStatusCharacteristicUuid = Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// Characteristic UUID for pattern configuration.
    /// TODO: Update with actual UUID from hardware specifications.
    /// </summary>
    public static readonly Guid PatternConfigCharacteristicUuid = Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb");

    /// <summary>
    /// Default scan timeout in seconds.
    /// </summary>
    public const int DefaultScanTimeoutSeconds = 10;

    /// <summary>
    /// Connection timeout in seconds.
    /// </summary>
    public const int ConnectionTimeoutSeconds = 15;
}
