namespace Skender.Stock.Indicators;

// STREAM OBSERVER INTERFACES

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
    where TResult : struct, IReusable;

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
    /// Full reset of the provider subscription.
    /// <remarks>
    /// This unsubscribes from the provider,
    /// clears cache, cascading deletes to subscribers,
    /// then re-subscribes to the provider (with rebuild).
    /// </remarks>
    /// </summary>
    void Reinitialize();

    /// <summary>
    /// Reset the entire results cache
    /// and rebuild it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <remarks>
    /// This is different from <see cref="Reinitialize()"/>.
    /// It does not reset the provider subscription.
    /// </remarks>
    void RebuildCache();

    /// <summary>
    /// Reset the entire results cache from a known point in time
    /// and rebuild it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromTimestamp">
    /// All periods (inclusive) after this date/time will
    /// be removed and recalculated.
    /// </param>
    void RebuildCache(DateTime fromTimestamp);

    /// <summary>
    /// Reset the entire results cache from a known index position
    /// and rebuild it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromIndex">
    /// All periods (inclusive) after this index position will
    /// be removed and recalculated.
    /// </param>
    void RebuildCache(int fromIndex);
}
