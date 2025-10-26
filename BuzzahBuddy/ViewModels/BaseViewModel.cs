using CommunityToolkit.Mvvm.ComponentModel;

namespace BuzzahBuddy.ViewModels;

/// <summary>
/// Base class for all ViewModels in the application.
/// Provides common properties and functionality for MVVM pattern.
/// </summary>
public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private bool _isRefreshing;

    /// <summary>
    /// Gets a value indicating whether the ViewModel is not busy.
    /// Useful for enabling/disabling UI elements.
    /// </summary>
    public bool IsNotBusy => !IsBusy;

    partial void OnIsBusyChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotBusy));
    }
}
