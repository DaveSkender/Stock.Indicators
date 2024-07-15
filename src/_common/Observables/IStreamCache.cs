namespace Skender.Stock.Indicators;

// STREAM CACHE INTERFACES

/// <summary>
/// Stored value cache for streaming hubs
/// </summary>
public interface IStreamCache<TSeries>
    where TSeries : ISeries
{
    /// <summary>
    /// Read-only list of the stored values cache.
    /// </summary>
    IReadOnlyList<TSeries> Results { get; }

    /// <summary>
    /// The cache and provider failed and is no longer operational.
    /// </summary>
    /// <remarks>
    /// This occurs when there is an overflow condition
    /// from a circular chain or
    /// when there were too many sequential duplicates.
    /// <para>
    /// Use <see cref="IStreamCache{TSeries}.ResetFault()"/>
    /// to remove this flag.
    /// </para>
    /// </remarks>
    bool IsFaulted { get; }

    /// <summary>
    /// Resets the <see cref="IsFaulted"/> flag and
    /// overflow counter.  Use this after recovering
    /// from an error.
    /// </summary>
    /// <remarks>
    /// You may also need to
    /// <see cref="IStreamHub{TIn, TOut}.Reinitialize()"/>,
    /// <see cref="IStreamHub{TIn, TOut}.RebuildCache()"/>, or
    /// <see cref="ClearCache()"/> before resuming.
    /// </remarks>
    void ResetFault();

    /// <summary>
    /// Try to find index position of the provided timestamp
    /// </summary>
    /// <param name="timestamp">Timestamp to seek</param>
    /// <param name="index">
    /// Index of timestamp or -1 when not found
    /// </param>
    /// <returns>True if found</returns>
    bool TryFindIndex(DateTime timestamp, out int index);

    /// <summary>
    /// Get the cache index based on item equality.
    /// </summary>
    /// <param name="cachedItem">
    /// Timeseries object to find in cache
    /// </param>
    /// <param name="noException">
    /// Disable exception when item is not found
    /// </param>
    /// <returns>Index position</returns>
    /// <exception cref="ArgumentException">
    /// When items is not found (should never happen).
    /// </exception>
    int GetIndex(TSeries cachedItem, bool noException);

    /// <summary>
    /// Get the cache index based on a timestamp.
    /// </summary>
    /// <remarks>
    /// Only use this when you are looking for a point in time
    /// without a matching item for context.  In most cases
    /// <see cref="GetIndex(TSeries,bool)"/> is more appropriate.
    /// </remarks>
    /// <param name="timestamp">
    /// Timestamp of cached item
    /// </param>
    /// <param name="noException">
    /// Disable exception when item is not found
    /// </param>
    /// <returns>Index position</returns>
    /// <exception cref="ArgumentException">
    /// When timestamp is not found (should never happen).
    /// </exception>
    int GetIndex(DateTime timestamp, bool noException);

    /// <summary>
    /// Get the first cache index on or after a timestamp.
    /// </summary>
    /// <remarks>
    /// Only use this when you are looking for a point in time
    /// without a matching item for context.  In most cases
    /// <see cref="GetIndex(TSeries,bool)"/> is more appropriate.
    /// </remarks>
    /// <param name="timestamp">
    /// Timestamp of cached item
    /// </param>
    /// <returns>First index position or -1 if not found</returns>
    int GetInsertIndex(DateTime timestamp);

    /// <summary>
    /// Deletes all cached time-series records,
    /// without restore.  When applicable,
    /// it will cascade delete commands to subscribers.
    /// </summary>
    /// <remarks>
    /// For observers, if your intention is to rebuild from a provider,
    /// use alternate <see cref="IStreamHub{TIn, TOut}.RebuildCache()"/>.
    /// </remarks>
    void ClearCache();

    /// <summary>
    /// Deletes newer cached records from point in time,
    /// without restore.  When applicable, it will cascade delete
    /// commands to subscribers.
    /// </summary>
    /// <remarks>
    /// For observers, if your intention is to rebuild from a provider,
    /// use alternate <see cref="IStreamHub{TIn, TOut}.RebuildCache(DateTime)"/>.
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
    /// use alternate <see cref="IStreamHub{TIn, TOut}.RebuildCache(int)"/>.
    /// </remarks>
    /// <param name="fromIndex">From index, inclusive</param>
    void ClearCache(int fromIndex);
}
