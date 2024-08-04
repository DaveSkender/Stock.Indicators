namespace Skender.Stock.Indicators;

// STREAM HUB INTERFACES

/// <summary>
/// Streaming hub (observer and observable provider).
/// </summary>
public interface IStreamHub<in TIn, in TOut>
    where TIn : ISeries
{
    /// <summary>
    /// Add a single new observed item.
    /// We'll determine if it's new or an update.
    /// </summary>
    /// <param name="newIn" cref="ISeries">
    /// Observed item to add or update
    /// </param>
    void Add(TIn newIn);

    /// <summary>
    /// Add a batch of observed items.
    /// We'll determine if they're new or updated.
    /// </summary>
    /// <param name="batchIn" cref="ISeries">
    /// Batch of observed items to add or update
    /// </param>
    void Add(IEnumerable<TIn> batchIn);

    /// <summary>
    /// Delete an item from the cache.
    /// </summary>
    /// <param name="cachedItem">Cached item to delete</param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    public void Remove(TOut cachedItem);

    /// <summary>
    /// Delete an item from the cache.
    /// </summary>
    /// <param name="cacheIndex">Position in cache to delete</param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    /// <exception cref="ArgumentOutOfRangeException"/>
    void RemoveAt(int cacheIndex);

    /// <summary>
    /// Returns a short text label for the hub
    /// with parameter values, e.g. "EMA(10)"
    /// </summary>
    /// <returns>String label</returns>
    string ToString();
}
