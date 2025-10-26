using BuzzahBuddy.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using Plugin.BLE.Abstractions.Exceptions;
using System.Collections.Concurrent;

namespace BuzzahBuddy.Services.Bluetooth;

/// <summary>
/// Service for managing Bluetooth Low Energy communication with BlueBuzzah gloves.
/// </summary>
public class BluetoothService : IBluetoothService
{
    private readonly IAdapter _adapter;
    private readonly IBluetoothLE _bluetoothLE;
    private IDevice? _connectedBleDevice;
    private readonly ConcurrentDictionary<string, GloveDevice> _discoveredDevices = new();

    public ConnectionState CurrentConnectionState { get; private set; } = ConnectionState.Disconnected;
    public GloveDevice? ConnectedDevice { get; private set; }

    public event EventHandler<GloveDevice>? DeviceDiscovered;
    public event EventHandler<ConnectionState>? ConnectionStateChanged;

    public BluetoothService()
    {
        _bluetoothLE = CrossBluetoothLE.Current;
        _adapter = CrossBluetoothLE.Current.Adapter;

        _adapter.DeviceDiscovered += OnDeviceDiscovered;
        _adapter.DeviceConnected += OnDeviceConnected;
        _adapter.DeviceDisconnected += OnDeviceDisconnected;
        _adapter.DeviceConnectionLost += OnDeviceConnectionLost;
    }

    public async Task<IEnumerable<GloveDevice>> ScanForDevicesAsync(
        TimeSpan timeout,
        CancellationToken cancellationToken = default)
    {
        _discoveredDevices.Clear();

        if (!await IsBluetoothEnabledAsync())
        {
            return Enumerable.Empty<GloveDevice>();
        }

        try
        {
            _adapter.ScanTimeout = (int)timeout.TotalMilliseconds;
            await _adapter.StartScanningForDevicesAsync(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Scan error: {ex.Message}");
        }

        return _discoveredDevices.Values;
    }

    public async Task StopScanAsync()
    {
        if (_adapter.IsScanning)
        {
            await _adapter.StopScanningForDevicesAsync();
        }
    }

    public async Task<bool> ConnectToDeviceAsync(
        GloveDevice device,
        CancellationToken cancellationToken = default)
    {
        try
        {
            UpdateConnectionState(ConnectionState.Connecting);

            // Find the BLE device
            var bleDevice = _adapter.ConnectedDevices.FirstOrDefault(d => d.Id.ToString() == device.Id)
                ?? _adapter.DiscoveredDevices.FirstOrDefault(d => d.Id.ToString() == device.Id);

            if (bleDevice == null)
            {
                UpdateConnectionState(ConnectionState.Error);
                return false;
            }

            var connectParameters = new ConnectParameters(autoConnect: false, forceBleTransport: true);
            await _adapter.ConnectToDeviceAsync(bleDevice, connectParameters, cancellationToken);

            _connectedBleDevice = bleDevice;
            ConnectedDevice = device;
            device.ConnectionState = ConnectionState.Connected;
            device.LastConnected = DateTime.Now;

            UpdateConnectionState(ConnectionState.Connected);
            return true;
        }
        catch (DeviceConnectionException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Connection error: {ex.Message}");
            UpdateConnectionState(ConnectionState.Error);
            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        if (_connectedBleDevice != null)
        {
            try
            {
                await _adapter.DisconnectDeviceAsync(_connectedBleDevice);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Disconnect error: {ex.Message}");
            }
            finally
            {
                _connectedBleDevice = null;
                ConnectedDevice = null;
                UpdateConnectionState(ConnectionState.Disconnected);
            }
        }
    }

    public async Task<bool> WriteCharacteristicAsync(
        Guid serviceId,
        Guid characteristicId,
        byte[] data)
    {
        if (_connectedBleDevice == null)
            return false;

        try
        {
            var service = await _connectedBleDevice.GetServiceAsync(serviceId);
            if (service == null)
                return false;

            var characteristic = await service.GetCharacteristicAsync(characteristicId);
            if (characteristic == null)
                return false;

            await characteristic.WriteAsync(data);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Write error: {ex.Message}");
            return false;
        }
    }

    public async Task<byte[]> ReadCharacteristicAsync(
        Guid serviceId,
        Guid characteristicId)
    {
        if (_connectedBleDevice == null)
            return Array.Empty<byte>();

        try
        {
            var service = await _connectedBleDevice.GetServiceAsync(serviceId);
            if (service == null)
                return Array.Empty<byte>();

            var characteristic = await service.GetCharacteristicAsync(characteristicId);
            if (characteristic == null)
                return Array.Empty<byte>();

            var result = await characteristic.ReadAsync();
            return result.data;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Read error: {ex.Message}");
            return Array.Empty<byte>();
        }
    }

    public Task<bool> IsBluetoothEnabledAsync()
    {
        return Task.FromResult(_bluetoothLE.IsOn);
    }

    public async Task<int?> GetBatteryLevelAsync()
    {
        if (_connectedBleDevice == null)
            return null;

        try
        {
            var data = await ReadCharacteristicAsync(
                BlueBuzzahConstants.PrimaryServiceUuid,
                BlueBuzzahConstants.BatteryLevelCharacteristicUuid);

            if (data.Length > 0)
            {
                return data[0]; // Battery level is typically a single byte (0-100)
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Battery read error: {ex.Message}");
        }

        return null;
    }

    private void OnDeviceDiscovered(object? sender, DeviceEventArgs e)
    {
        // Filter for BlueBuzzah devices only
        if (string.IsNullOrEmpty(e.Device.Name) ||
            !e.Device.Name.Contains(BlueBuzzahConstants.DeviceNamePrefix, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var gloveDevice = new GloveDevice
        {
            Id = e.Device.Id.ToString(),
            Name = e.Device.Name,
            SignalStrength = e.Device.Rssi,
            ConnectionState = ConnectionState.Disconnected
        };

        if (_discoveredDevices.TryAdd(gloveDevice.Id, gloveDevice))
        {
            DeviceDiscovered?.Invoke(this, gloveDevice);
        }
    }

    private void OnDeviceConnected(object? sender, DeviceEventArgs e)
    {
        if (ConnectedDevice != null)
        {
            ConnectedDevice.ConnectionState = ConnectionState.Connected;
        }
        UpdateConnectionState(ConnectionState.Connected);
    }

    private void OnDeviceDisconnected(object? sender, DeviceEventArgs e)
    {
        _connectedBleDevice = null;
        if (ConnectedDevice != null)
        {
            ConnectedDevice.ConnectionState = ConnectionState.Disconnected;
        }
        UpdateConnectionState(ConnectionState.Disconnected);
    }

    private void OnDeviceConnectionLost(object? sender, DeviceErrorEventArgs e)
    {
        _connectedBleDevice = null;
        if (ConnectedDevice != null)
        {
            ConnectedDevice.ConnectionState = ConnectionState.Error;
        }
        UpdateConnectionState(ConnectionState.Error);
    }

    private void UpdateConnectionState(ConnectionState newState)
    {
        if (CurrentConnectionState != newState)
        {
            CurrentConnectionState = newState;
            ConnectionStateChanged?.Invoke(this, newState);
        }
    }
}
