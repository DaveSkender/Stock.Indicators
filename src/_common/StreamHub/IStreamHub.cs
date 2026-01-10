namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub: management of observer
/// and observable indicator data
/// </summary>
/// <typeparam name="TIn">
/// Type of inbound provider data.
/// </typeparam>
/// <typeparam name="TOut">
/// Type of outbound indicator data.
/// </typeparam>
public interface IStreamHub<in TIn, TOut> : IStreamHubBase<TOut>
    where TIn : IReusable
    where TOut : IReusable
{
    /// <summary>
    /// The cache and provider failed and is no longer operational.
    /// </summary>
    /// <remarks>
    /// This occurs when there is an overflow condition
    /// from a circular chain or
    /// when there were too many sequential duplicates.
    /// <para>
    /// Use <see cref="ResetFault()"/>
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
    /// <see cref="IStreamObserver{T}.Reinitialize()"/>, or
    /// <see cref="IStreamObserver{T}.Rebuild()"/>.
    /// </remarks>
    void ResetFault();

    /// <summary>
    /// Add a single new item.
    /// We'll determine if it's new or an update.
    /// </summary>
    /// <param name="newIn">
    /// New item to add
    /// </param>
    void Add(TIn newIn);

    /// <summary>
    /// Add a batch of new items.
    /// We'll determine if they're new or updated.
    /// </summary>
    /// <param name="batchIn">
    /// Batch of new items to add
    /// </param>
    void Add(IEnumerable<TIn> batchIn);

    /// <summary>
    /// Insert a new item without rebuilding the cache.
    /// </summary>
    /// <remarks>
    /// This is used in situations when inserting an older item
    /// and where newer cache entries do not need to be rebuilt.
    /// Typically, this is only used for provider-only hubs.
    /// </remarks>
    /// <param name="newIn">
    /// Item to insert
    /// </param>
    void Insert(TIn newIn);

    /// <summary>
    /// Delete an item from the cache.
    /// </summary>
    /// <param name="cachedItem">Cached item to delete</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    void Remove(TOut cachedItem);

    /// <summary>
    /// Delete an item from the cache, from a specific position.
    /// </summary>
    /// <param name="cacheIndex">Position in cache to delete</param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    void RemoveAt(int cacheIndex);

    /// <summary>
    /// Deletes newer cached records from point in time (inclusive).
    /// </summary>
    /// <remarks>
    /// For observers, if your intention is to rebuild from a provider,
    /// use alternate <see cref="IStreamObserver{T}.Rebuild(DateTime)"/>.
    /// </remarks>
    /// <param name="fromTimestamp">
    /// All periods (inclusive) after this DateTime will be removed.
    /// </param>
    /// <param name="notify">
    /// Notify subscribers of the delete point.
    /// </param>
    void RemoveRange(DateTime fromTimestamp, bool notify);

    /// <summary>
    /// Deletes newer cached records from an index position (inclusive).
    /// </summary>
    /// <remarks>
    /// For observers, if your intention is to rebuild from a provider,
    /// use alternate <see cref="IStreamObserver{T}.Rebuild(int)"/>.
    /// </remarks>
    /// <param name="fromIndex">From index, inclusive</param>
    /// <param name="notify">
    /// Notify subscribers of the delete position.
    /// </param>
    void RemoveRange(int fromIndex, bool notify);

    /// <summary>
    /// Returns a short text label for the hub
    /// with parameter values, e.g. "EMA(10)"
    /// </summary>
    /// <returns>String label</returns>
    string ToString();
}
