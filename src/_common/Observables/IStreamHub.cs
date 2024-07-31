namespace Skender.Stock.Indicators;

// STREAM HUB INTERFACES

#region hub variants

public interface IQuoteHub<TIn, TOut>
    : IStreamHub<TIn, TOut>, IQuoteProvider<TOut>, IChainProvider<TOut>
    where TIn : IQuote
    where TOut : IQuote;

/// <inheritdoc />
public interface IReusableHub<TIn, TOut>
    : IStreamHub<TIn, TOut>, IChainProvider<TOut>
    where TIn : ISeries
    where TOut : IReusable;

/// <inheritdoc />
public interface IResultHub<TIn, TOut>
    : IStreamHub<TIn, TOut>
    where TIn : ISeries
    where TOut : ISeries;
#endregion

/// <summary>
/// Streaming hub (observer and observable provider).
/// </summary>
public interface IStreamHub<TIn, TOut>
    : IStreamObserver<(Act, TIn, int?)>, IStreamObservable<TOut>
    where TIn : ISeries
    where TOut : ISeries
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
    Act Remove(TOut cachedItem);

    /// <summary>
    /// Delete an item from the cache.
    /// </summary>
    /// <param name="cacheIndex">Position in cache to delete</param>
    /// <returns cref="Act">Action taken (outcome)</returns>
    /// <exception cref="ArgumentOutOfRangeException"/>
    Act RemoveAt(int cacheIndex);

    /// <summary>
    /// Returns a short text label for the hub
    /// with parameter values, e.g. "EMA(10)"
    /// </summary>
    /// <returns>String label</returns>
    string ToString();
}
