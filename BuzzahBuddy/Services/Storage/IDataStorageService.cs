using BuzzahBuddy.Models;

namespace BuzzahBuddy.Services.Storage;

/// <summary>
/// Service interface for storing and retrieving application data.
/// </summary>
public interface IDataStorageService
{
    /// <summary>
    /// Saves a therapy session to storage.
    /// </summary>
    /// <param name="session">The session to save.</param>
    Task SaveSessionAsync(TherapySession session);

    /// <summary>
    /// Retrieves the history of therapy sessions.
    /// </summary>
    /// <param name="limit">Maximum number of sessions to retrieve (0 for all).</param>
    /// <returns>A collection of therapy sessions, ordered by start time (most recent first).</returns>
    Task<IEnumerable<TherapySession>> GetSessionHistoryAsync(int limit = 0);

    /// <summary>
    /// Gets a specific therapy session by ID.
    /// </summary>
    /// <param name="sessionId">The ID of the session to retrieve.</param>
    /// <returns>The session if found, null otherwise.</returns>
    Task<TherapySession?> GetSessionByIdAsync(string sessionId);

    /// <summary>
    /// Deletes a therapy session from storage.
    /// </summary>
    /// <param name="sessionId">The ID of the session to delete.</param>
    Task DeleteSessionAsync(string sessionId);

    /// <summary>
    /// Saves a custom vibration pattern to storage.
    /// </summary>
    /// <param name="pattern">The pattern to save.</param>
    Task SavePatternAsync(VibrationPattern pattern);

    /// <summary>
    /// Retrieves all saved vibration patterns.
    /// </summary>
    /// <returns>A collection of saved vibration patterns.</returns>
    Task<IEnumerable<VibrationPattern>> GetPatternsAsync();

    /// <summary>
    /// Deletes a vibration pattern from storage.
    /// </summary>
    /// <param name="patternId">The ID of the pattern to delete.</param>
    Task DeletePatternAsync(string patternId);

    /// <summary>
    /// Saves the last connected device information.
    /// </summary>
    /// <param name="device">The device to save.</param>
    Task SaveLastDeviceAsync(GloveDevice device);

    /// <summary>
    /// Gets the last connected device information.
    /// </summary>
    /// <returns>The last connected device, or null if none saved.</returns>
    Task<GloveDevice?> GetLastDeviceAsync();

    /// <summary>
    /// Clears all stored data (sessions, patterns, device info).
    /// </summary>
    Task ClearAllDataAsync();
}
