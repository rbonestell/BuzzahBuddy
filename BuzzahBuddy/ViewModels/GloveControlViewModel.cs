using BuzzahBuddy.Models;
using BuzzahBuddy.Services.Bluetooth;
using BuzzahBuddy.Services.Glove;
using BuzzahBuddy.Services.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace BuzzahBuddy.ViewModels;

/// <summary>
/// ViewModel for the glove control page.
/// Handles vibration control, pattern selection, and intensity adjustment.
/// </summary>
public partial class GloveControlViewModel : BaseViewModel
{
    private readonly IGloveControlService _gloveControlService;
    private readonly IBluetoothService _bluetoothService;
    private readonly IDataStorageService _storageService;
    private TherapySession? _currentSession;

    [ObservableProperty]
    private ObservableCollection<VibrationPattern> _availablePatterns = new();

    [ObservableProperty]
    private VibrationPattern? _selectedPattern;

    [ObservableProperty]
    private bool _isVibrating;

    [ObservableProperty]
    private int _batteryLevel;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private string _vibrationButtonText = "Start Vibration";

    public GloveControlViewModel(
        IGloveControlService gloveControlService,
        IBluetoothService bluetoothService,
        IDataStorageService storageService)
    {
        _gloveControlService = gloveControlService;
        _bluetoothService = bluetoothService;
        _storageService = storageService;

        Title = "Glove Control";

        // Subscribe to events
        _gloveControlService.VibrationStateChanged += OnVibrationStateChanged;
        _bluetoothService.ConnectionStateChanged += OnConnectionStateChanged;

        // Initialize
        LoadPatterns();
        UpdateConnectionState();
    }

    [RelayCommand]
    private async Task ToggleVibrationAsync()
    {
        if (!IsConnected)
        {
            await Shell.Current.DisplayAlert(
                "Not Connected",
                "Please connect to a BlueBuzzah glove first.",
                "OK");
            return;
        }

        if (SelectedPattern == null)
        {
            await Shell.Current.DisplayAlert(
                "No Pattern Selected",
                "Please select a vibration pattern first.",
                "OK");
            return;
        }

        IsBusy = true;

        try
        {
            if (IsVibrating)
            {
                // Stop vibration
                var success = await _gloveControlService.StopVibrationAsync();

                if (success && _currentSession != null)
                {
                    _currentSession.EndTime = DateTime.Now;
                    _currentSession.IsCompleted = true;
                    await _storageService.SaveSessionAsync(_currentSession);
                    _currentSession = null;
                }
            }
            else
            {
                // Start vibration
                var success = await _gloveControlService.StartVibrationAsync(SelectedPattern);

                if (success)
                {
                    // Create new therapy session
                    _currentSession = new TherapySession
                    {
                        StartTime = DateTime.Now,
                        PatternUsed = SelectedPattern,
                        PatternId = SelectedPattern.Id,
                        DeviceId = _bluetoothService.ConnectedDevice?.Id
                    };
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Error",
                $"An error occurred: {ex.Message}",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        if (!IsConnected)
        {
            await Shell.Current.DisplayAlert(
                "Not Connected",
                "Please connect to a BlueBuzzah glove first.",
                "OK");
            return;
        }

        IsBusy = true;

        try
        {
            var success = await _gloveControlService.TestConnectionAsync();

            if (success)
            {
                await Shell.Current.DisplayAlert(
                    "Test Successful",
                    "Connection test completed successfully!",
                    "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert(
                    "Test Failed",
                    "Connection test failed. Please check your connection.",
                    "OK");
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RefreshBatteryAsync()
    {
        if (!IsConnected)
            return;

        var battery = await _gloveControlService.GetBatteryLevelAsync();
        if (battery.HasValue)
        {
            BatteryLevel = battery.Value;
        }
    }

    partial void OnSelectedPatternChanged(VibrationPattern? value)
    {
        // If pattern changed while vibrating, restart with new pattern
        if (IsVibrating && value != null)
        {
            _ = Task.Run(async () =>
            {
                await _gloveControlService.StopVibrationAsync();
                await Task.Delay(100);
                await _gloveControlService.StartVibrationAsync(value);
            });
        }
    }

    private async void LoadPatterns()
    {
        var defaultPatterns = await _gloveControlService.GetDefaultPatternsAsync();
        var savedPatterns = await _storageService.GetPatternsAsync();

        AvailablePatterns.Clear();

        foreach (var pattern in defaultPatterns)
        {
            AvailablePatterns.Add(pattern);
        }

        foreach (var pattern in savedPatterns)
        {
            AvailablePatterns.Add(pattern);
        }

        // Select first pattern by default
        if (AvailablePatterns.Count > 0)
        {
            SelectedPattern = AvailablePatterns[0];
        }
    }

    private async void UpdateConnectionState()
    {
        IsConnected = _bluetoothService.CurrentConnectionState == ConnectionState.Connected;

        if (IsConnected)
        {
            await RefreshBatteryAsync();
        }
    }

    private void OnVibrationStateChanged(object? sender, bool isVibrating)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsVibrating = isVibrating;
            VibrationButtonText = isVibrating ? "Stop Vibration" : "Start Vibration";
        });
    }

    private void OnConnectionStateChanged(object? sender, ConnectionState state)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            IsConnected = state == ConnectionState.Connected;

            // Stop vibration if disconnected
            if (!IsConnected && IsVibrating)
            {
                IsVibrating = false;
                VibrationButtonText = "Start Vibration";
            }
        });
    }
}
