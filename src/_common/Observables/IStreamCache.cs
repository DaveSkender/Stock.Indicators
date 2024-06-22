namespace Skender.Stock.Indicators;

// STREAM CACHE INTERFACES

/// <summary>
/// Cache of stored values and related management
/// </summary>
public interface IStreamCache<TSeries>
    where TSeries : struct, ISeries
{
    /// <summary>
    /// Read-only cache of results
    /// </summary>
    IReadOnlyList<TSeries> Results { get; }

    /// <summary>
    /// An error caused this provider to stop
    /// and terminated all subscriptions. />.
    /// </summary>
    bool IsFaulted { get; }

    /// <summary>
    /// Get a segment (window) of cached values.
    /// </summary>
    /// <inheritdoc cref="List{T}.GetRange(int,int)"/>
    IReadOnlyList<TSeries> GetRange(int index, int count);

    /// <summary>
    /// Finds the index position of the provided timestamp
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns>Index value or -1 when not found</returns>
    int FindIndex(DateTime timeStamp);

    /// <summary>
    /// Deletes all cached time-series records,
    /// without restore.  When applicable,
    /// it will cascade delete commands to subscribers.
    /// </summary>
    void ClearCache();

    /// <summary>
    /// Deletes newer cached time-series records from point in time,
    /// without restore.  When applicable, it will cascade delete
    /// commands to subscribers.
    /// </summary>
    /// <param name="fromTimestamp">
    /// All periods (inclusive) after this DateTime will be removed.
    /// </param>
    void ClearCache(DateTime fromTimestamp);
}
