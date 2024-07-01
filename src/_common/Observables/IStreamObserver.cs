namespace Skender.Stock.Indicators;

// STREAM OBSERVER INTERFACES

/// <summary>
/// Observer of a streamed quote source.
/// </summary>
/// <remarks>
/// This is used internal to the library,
/// but can be implemented to build your own observers.
/// </remarks>
/// <typeparam name="TQuote"></typeparam>
public interface IQuoteObserver<TQuote> : IStreamObserver, IObserver<(Act act, TQuote quote)>
    where TQuote : struct, IQuote
{
    IQuoteProvider<TQuote> Provider { get; }
}

/// <summary>
/// Observer of a streamed chain source.
/// </summary>
/// <remarks>
/// This is used internal to the library,
/// but can be implemented to build your own observers.
/// </remarks>
/// <typeparam name="TReusable"></typeparam>
public interface IChainObserver<TReusable> : IStreamObserver, IObserver<(Act act, TReusable result)>
    where TReusable : struct, IReusable
{
    IChainProvider<TReusable> Provider { get; }
}

/// <summary>
/// Observer of a unchainable result source.
/// </summary>
/// <remarks>
/// This is not used internally by the library,
/// but can be implemented to build your own observers.
/// </remarks>
/// <typeparam name="TResult"></typeparam>
public interface IResultObserver<TResult> : IStreamObserver, IObserver<(Act act, TResult result)>
    where TResult : struct, IResult
{
    IResultProvider<TResult> Provider { get; }
}

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
    /// clears cache, cascades deletes to subscribers,
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
    void RebuildCache() => RebuildCache(0);

    /// <summary>
    /// Reset the entire results cache from a point in time
    /// and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromTimestamp">
    /// All periods (inclusive) after this date/time will
    /// be removed and recalculated.
    /// </param>
    void RebuildCache(DateTime fromTimestamp);

    /// <summary>
    /// Reset the entire results cache from an index position
    /// and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromIndex">
    /// All periods (inclusive) after this index position will
    /// be removed and recalculated.
    /// </param>
    void RebuildCache(int fromIndex);
}
