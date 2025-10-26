using BuzzahBuddy.Models;
using BuzzahBuddy.Services.Bluetooth;
using BuzzahBuddy.Services.Glove;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BuzzahBuddy.ViewModels;

/// <summary>
/// ViewModel for the main/home page of the application.
/// Displays connection status and provides quick actions.
/// </summary>
public partial class MainPageViewModel : BaseViewModel
{
    private readonly IBluetoothService _bluetoothService;
    private readonly IGloveControlService _gloveControlService;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private string _connectedDeviceName = "Not Connected";

    [ObservableProperty]
    private int _batteryLevel;

    [ObservableProperty]
    private ConnectionState _connectionState = ConnectionState.Disconnected;

    public MainPageViewModel(
        IBluetoothService bluetoothService,
        IGloveControlService gloveControlService)
    {
        _bluetoothService = bluetoothService;
        _gloveControlService = gloveControlService;

        Title = "BuzzahBuddy";

        // Subscribe to connection state changes
        _bluetoothService.ConnectionStateChanged += OnConnectionStateChanged;

        // Initialize connection state (fire-and-forget is acceptable in constructor for UI initialization)
        _ = UpdateConnectionInfo();
    }

    [RelayCommand]
    private async Task NavigateToDeviceListAsync()
    {
        await Shell.Current.GoToAsync("//devices");
    }

    [RelayCommand]
    private async Task NavigateToControlAsync()
    {
        if (!IsConnected)
        {
            await Shell.Current.DisplayAlert(
                "Not Connected",
                "Please connect to a BlueBuzzah glove first.",
                "OK");
            return;
        }

        await Shell.Current.GoToAsync("//control");
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;

        try
        {
            await UpdateConnectionInfo();

            if (IsConnected)
            {
                var battery = await _gloveControlService.GetBatteryLevelAsync();
                if (battery.HasValue)
                {
                    BatteryLevel = battery.Value;
                }
            }
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async Task UpdateConnectionInfo()
    {
        ConnectionState = _bluetoothService.CurrentConnectionState;
        IsConnected = ConnectionState == ConnectionState.Connected;

        if (IsConnected && _bluetoothService.ConnectedDevice != null)
        {
            ConnectedDeviceName = _bluetoothService.ConnectedDevice.Name;

            var battery = await _gloveControlService.GetBatteryLevelAsync();
            if (battery.HasValue)
            {
                BatteryLevel = battery.Value;
            }
        }
        else
        {
            ConnectedDeviceName = "Not Connected";
            BatteryLevel = 0;
        }
    }

    private async void OnConnectionStateChanged(object? sender, ConnectionState state)
    {
        // Update on UI thread
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await UpdateConnectionInfo();
        });
    }
}
