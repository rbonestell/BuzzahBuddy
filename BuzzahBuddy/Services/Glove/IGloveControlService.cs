using BuzzahBuddy.Models;

namespace BuzzahBuddy.Services.Glove;

/// <summary>
/// Service interface for controlling BlueBuzzah glove vibration patterns and settings.
/// </summary>
public interface IGloveControlService
{
    /// <summary>
    /// Gets a value indicating whether vibration is currently active.
    /// </summary>
    bool IsVibrating { get; }

    /// <summary>
    /// Gets the currently active vibration pattern, if any.
    /// </summary>
    VibrationPattern? CurrentPattern { get; }

    /// <summary>
    /// Event raised when vibration state changes.
    /// </summary>
    event EventHandler<bool>? VibrationStateChanged;

    /// <summary>
    /// Starts vibration with the specified pattern.
    /// </summary>
    /// <param name="pattern">The vibration pattern to use.</param>
    /// <returns>True if vibration started successfully, false otherwise.</returns>
    Task<bool> StartVibrationAsync(VibrationPattern pattern);

    /// <summary>
    /// Stops the current vibration.
    /// </summary>
    /// <returns>True if vibration stopped successfully, false otherwise.</returns>
    Task<bool> StopVibrationAsync();

    /// <summary>
    /// Sets the vibration intensity without changing other pattern parameters.
    /// </summary>
    /// <param name="intensity">Intensity level (0-100).</param>
    /// <returns>True if intensity set successfully, false otherwise.</returns>
    Task<bool> SetIntensityAsync(int intensity);

    /// <summary>
    /// Gets the default vibration patterns available.
    /// </summary>
    /// <returns>A collection of predefined vibration patterns.</returns>
    Task<IEnumerable<VibrationPattern>> GetDefaultPatternsAsync();

    /// <summary>
    /// Gets the current battery level of the connected glove.
    /// </summary>
    /// <returns>Battery level percentage (0-100), or null if unavailable.</returns>
    Task<int?> GetBatteryLevelAsync();

    /// <summary>
    /// Tests the glove connection with a brief vibration pulse.
    /// </summary>
    /// <returns>True if test successful, false otherwise.</returns>
    Task<bool> TestConnectionAsync();
}
