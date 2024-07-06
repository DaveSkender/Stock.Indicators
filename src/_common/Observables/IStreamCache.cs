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
    /// Use this to reset the <see cref="IsFaulted"/> flag
    /// and OverflowCount after recovering from an error.
    /// </summary>
    /// <remarks>
    /// You may also need to <see cref="IStreamObserver{TIn}.Reinitialize()"/>,
    /// <see cref="IStreamObserver{TSeries}.RebuildCache()"/>, or
    /// <see cref="IStreamProvider{TSeries}.ClearCache()"/> before resuming.
    /// </remarks>
    void Reset();

    /// <summary>
    /// Try to find index position of the provided timestamp
    /// </summary>
    /// <param name="timestamp">Timestamp to seek</param>
    /// <param name="index">Index of timestamp or -1 when not found</param>
    /// <returns>True if found</returns>
    bool TryFindIndex(DateTime timestamp, out int index);
}
