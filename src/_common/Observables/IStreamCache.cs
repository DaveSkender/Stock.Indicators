namespace Skender.Stock.Indicators;

// STREAM CACHE INTERFACES

/// <summary>
/// Cache of stored values and related management
/// </summary>
public interface IStreamCache<out TSeries>
    where TSeries : struct, ISeries
{
    /// <summary>
    /// Read-only cache of results
    /// </summary>
    IReadOnlyList<TSeries> Results { get; }

    /// <summary>
    /// An error caused this observer/observable handler
    /// to stop and terminated all subscriptions. />.
    /// </summary>
    bool IsFaulted { get; }

    /// <summary>
    /// Finds the index position in the cache, of the provided timestamp
    /// </summary>
    /// <param name="timeStamp"></param>
    /// <returns>Index value or -1 when not found</returns>
    int FindIndex(DateTime timeStamp);

    /// <summary>
    /// Deletes all cached time-series records,
    /// without restore.  When applicable,
    /// it will cascade delete commands to subscribers.
    /// </summary>
    /// <remarks>
    /// For observers, if your intention is to rebuild from a provider,
    /// use alternate <see cref="IStreamObserver.RebuildCache()"/>.
    /// </remarks>
    void ClearCache() => ClearCache(0);

    /// <summary>
    /// Deletes newer cached records from point in time,
    /// without restore.  When applicable, it will cascade delete
    /// commands to subscribers.
    /// </summary>
    /// <remarks>
    /// For observers, if your intention is to rebuild from a provider,
    /// use alternate <see cref="IStreamObserver.RebuildCache(DateTime)"/>.
    /// </remarks>
    /// <param name="fromTimestamp">
    /// All periods (inclusive) after this DateTime will be removed.
    /// </param>
    void ClearCache(DateTime fromTimestamp);

    /// <summary>
    /// Deletes newer cached records from an index position (inclusive),
    /// without restore.  When applicable, it will cascade delete
    /// commands to subscribers.
    /// </summary>
    /// <remarks>
    /// For observers, if your intention is to rebuild from a provider,
    /// use alternate <see cref="IStreamObserver.RebuildCache(int)"/>.
    /// </remarks>
    /// <param name="fromIndex">From index, inclusive</param>
    void ClearCache(int fromIndex);
}
