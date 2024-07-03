namespace Skender.Stock.Indicators;

// STREAM CACHE INTERFACES

/// <summary>
/// Cache of stored values and related management
/// </summary>
public interface IStreamCache
{
    /// <summary>
    /// An error caused this cache handler to fail. />.
    /// </summary>
    bool IsFaulted { get; }

    /// <summary>
    /// Try to find index position of the provided timestamp
    /// </summary>
    /// <param name="timestamp">Timestamp to seek</param>
    /// <param name="index">Index of timestamp or -1 when not found</param>
    /// <returns>True if found</returns>
    bool TryFindIndex(DateTime timestamp, out int index);
}
