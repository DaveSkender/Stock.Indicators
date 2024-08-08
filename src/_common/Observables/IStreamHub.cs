namespace Skender.Stock.Indicators;

// STREAM HUB INTERFACES

/// <summary>
/// Streaming hub (observer and observable provider).
/// </summary>
public interface IStreamHub<in TIn, in TOut>
    where TIn : ISeries
{
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
    /// Returns a short text label for the hub
    /// with parameter values, e.g. "EMA(10)"
    /// </summary>
    /// <returns>String label</returns>
    string ToString();
}
