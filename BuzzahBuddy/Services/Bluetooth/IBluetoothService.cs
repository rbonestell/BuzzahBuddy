using BuzzahBuddy.Models;
using Plugin.BLE.Abstractions.Contracts;

namespace BuzzahBuddy.Services.Bluetooth;

/// <summary>
/// Service interface for Bluetooth Low Energy communication with BlueBuzzah gloves.
/// </summary>
public interface IBluetoothService
{
    /// <summary>
    /// Gets the current connection state.
    /// </summary>
    ConnectionState CurrentConnectionState { get; }

    /// <summary>
    /// Gets the currently connected device, if any.
    /// </summary>
    GloveDevice? ConnectedDevice { get; }

    /// <summary>
    /// Event raised when a device is discovered during scanning.
    /// </summary>
    event EventHandler<GloveDevice>? DeviceDiscovered;

    /// <summary>
    /// Event raised when the connection state changes.
    /// </summary>
    event EventHandler<ConnectionState>? ConnectionStateChanged;

    /// <summary>
    /// Scans for available BlueBuzzah glove devices.
    /// </summary>
    /// <param name="timeout">Maximum duration to scan for devices.</param>
    /// <param name="cancellationToken">Cancellation token to stop scanning.</param>
    /// <returns>A collection of discovered devices.</returns>
    Task<IEnumerable<GloveDevice>> ScanForDevicesAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the current device scan if one is in progress.
    /// </summary>
    Task StopScanAsync();

    /// <summary>
    /// Connects to the specified BlueBuzzah glove device.
    /// </summary>
    /// <param name="device">The device to connect to.</param>
    /// <param name="cancellationToken">Cancellation token to abort connection.</param>
    /// <returns>True if connection successful, false otherwise.</returns>
    Task<bool> ConnectToDeviceAsync(GloveDevice device, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects from the currently connected device.
    /// </summary>
    Task DisconnectAsync();

    /// <summary>
    /// Writes data to a specified Bluetooth characteristic.
    /// </summary>
    /// <param name="serviceId">The UUID of the service containing the characteristic.</param>
    /// <param name="characteristicId">The UUID of the characteristic to write to.</param>
    /// <param name="data">The data to write.</param>
    /// <returns>True if write successful, false otherwise.</returns>
    Task<bool> WriteCharacteristicAsync(Guid serviceId, Guid characteristicId, byte[] data);

    /// <summary>
    /// Reads data from a specified Bluetooth characteristic.
    /// </summary>
    /// <param name="serviceId">The UUID of the service containing the characteristic.</param>
    /// <param name="characteristicId">The UUID of the characteristic to read from.</param>
    /// <returns>The data read from the characteristic, or empty array if read failed.</returns>
    Task<byte[]> ReadCharacteristicAsync(Guid serviceId, Guid characteristicId);

    /// <summary>
    /// Checks if Bluetooth is enabled on the device.
    /// </summary>
    /// <returns>True if Bluetooth is enabled, false otherwise.</returns>
    Task<bool> IsBluetoothEnabledAsync();

    /// <summary>
    /// Gets the current battery level of the connected device.
    /// </summary>
    /// <returns>Battery level percentage (0-100), or null if unavailable.</returns>
    Task<int?> GetBatteryLevelAsync();
}
