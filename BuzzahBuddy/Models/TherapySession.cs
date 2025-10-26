namespace BuzzahBuddy.Models;

/// <summary>
/// Represents a therapy session with the BlueBuzzah glove.
/// Used for tracking usage and analyzing therapy effectiveness.
/// </summary>
public class TherapySession
{
    /// <summary>
    /// Gets or sets the unique identifier for the session.
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Gets or sets the start time of the therapy session.
    /// </summary>
    public DateTime StartTime { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the end time of the therapy session.
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// Gets the duration of the session.
    /// Returns TimeSpan.Zero if session is still in progress.
    /// </summary>
    public TimeSpan Duration => EndTime.HasValue
        ? EndTime.Value - StartTime
        : DateTime.Now - StartTime;

    /// <summary>
    /// Gets or sets the vibration pattern used during this session.
    /// </summary>
    public VibrationPattern? PatternUsed { get; set; }

    /// <summary>
    /// Gets or sets the ID of the pattern used (for serialization).
    /// </summary>
    public string? PatternId { get; set; }

    /// <summary>
    /// Gets or sets optional notes about the session (e.g., effectiveness, comfort level).
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the device used for this session.
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the session was completed successfully.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets the user's subjective rating of effectiveness (1-5 scale).
    /// </summary>
    public int? EffectivenessRating { get; set; }
}
