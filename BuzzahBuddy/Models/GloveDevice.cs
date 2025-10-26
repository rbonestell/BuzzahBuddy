namespace BuzzahBuddy.Models;

/// <summary>
/// Represents a BlueBuzzah glove device discovered via Bluetooth.
/// </summary>
public class GloveDevice
{
    /// <summary>
    /// Gets or sets the unique identifier for the device.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the device.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MAC address of the device.
    /// </summary>
    public string MacAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the battery level percentage (0-100).
    /// </summary>
    public int BatteryLevel { get; set; }

    /// <summary>
    /// Gets or sets the current connection state of the device.
    /// </summary>
    public ConnectionState ConnectionState { get; set; } = ConnectionState.Disconnected;

    /// <summary>
    /// Gets or sets the firmware version of the device.
    /// </summary>
    public string? FirmwareVersion { get; set; }

    /// <summary>
    /// Gets or sets the signal strength (RSSI) of the Bluetooth connection.
    /// </summary>
    public int SignalStrength { get; set; }

    /// <summary>
    /// Gets or sets the last time the device was successfully connected.
    /// </summary>
    public DateTime? LastConnected { get; set; }
}
