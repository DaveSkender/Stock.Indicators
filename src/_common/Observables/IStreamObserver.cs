namespace Skender.Stock.Indicators;

// OBSERVER INTERFACES

/// <summary>
/// Observer of a streamed quote source
/// </summary>
/// <typeparam name="TQuote"></typeparam>
public interface IQuoteObserver<TQuote> : IStreamObserver, IObserver<(Act act, TQuote quote)>
    where TQuote : struct, IQuote;

/// <summary>
/// Observer of a streamed chain source
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IChainObserver<TResult> : IStreamObserver, IObserver<(Act act, TResult result)>
    where TResult : struct, IReusableResult;

/// <summary>
/// Observer of a unchainable result source
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IResultObserver<TResult> : IStreamObserver, IObserver<(Act act, TResult result)>
    where TResult : struct, IResult;

/// <summary>
/// Observer of streamed chain or quote sources
/// </summary>
public interface IStreamObserver
{
    /// <summary>
    /// Current state of subscription to provider.
    /// </summary>
    bool IsSubscribed { get; }

    /// <summary>
    /// Unsubscribe from the data provider.
    /// </summary>
    void Unsubscribe();

    /// <summary>
    /// Reinitialize the cache to erase all stored values,
    /// and resubscribe to provider.
    /// </summary>
    void Reinitialize(bool withRebuild = true);

    /// <summary>
    /// Reset the entire results cache
    /// and rebuild it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    void RebuildCache();

    /// <summary>
    /// Reset the entire results cache from a known point in time
    /// and rebuild it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromTimestamp">
    /// All periods (inclusive) after this DateTime will
    /// be removed and recalculated.
    /// </param>
    void RebuildCache(DateTime fromTimestamp);

    /// <summary>
    /// Reset the entire results cache from a known point in time
    /// and rebuild it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromIndex">
    /// All periods (inclusive) after this index position will
    /// be removed and recalculated.
    /// </param>
    void RebuildCache(int fromIndex);
}
