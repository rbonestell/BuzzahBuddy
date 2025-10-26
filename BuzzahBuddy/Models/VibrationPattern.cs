namespace BuzzahBuddy.Models;

/// <summary>
/// Represents a vibration pattern configuration for the BlueBuzzah glove.
/// </summary>
public class VibrationPattern
{
    /// <summary>
    /// Gets or sets the unique identifier for the pattern.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the display name of the pattern.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the vibration intensity (0-100).
    /// </summary>
    public int Intensity { get; set; } = 50;

    /// <summary>
    /// Gets or sets the duration of each vibration pulse in milliseconds.
    /// </summary>
    public int DurationMs { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the frequency of vibration pulses in Hz.
    /// </summary>
    public int FrequencyHz { get; set; } = 100;

    /// <summary>
    /// Gets or sets a value indicating whether this pattern is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the interval between pulses in milliseconds (for pulsed patterns).
    /// </summary>
    public int IntervalMs { get; set; } = 500;

    /// <summary>
    /// Gets or sets a value indicating whether this is a continuous or pulsed pattern.
    /// </summary>
    public bool IsContinuous { get; set; } = true;

    /// <summary>
    /// Gets or sets optional description of the pattern.
    /// </summary>
    public string? Description { get; set; }
}
