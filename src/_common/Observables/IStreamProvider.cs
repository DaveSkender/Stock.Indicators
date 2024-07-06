namespace Skender.Stock.Indicators;

// STREAM OBSERVABLE INTERFACES (PROVIDERS)
// with Quote, Chain, Result variants (at bottom)

/// <summary>
/// Streaming provider interface (observable)
/// </summary>
/// <typeparam name="TSeries"></typeparam>
public interface IStreamProvider<TSeries> : IObservable<(Act, TSeries)>
    where TSeries : struct, ISeries
{
    /// <inheritdoc cref="IStreamCache.IsFaulted" />
    bool IsFaulted { get; }

    /// <summary>
    /// Currently has subscribers
    /// </summary>
    bool HasSubscribers { get; }

    /// <summary>
    /// Current number of subscribers
    /// </summary>
    int SubscriberCount { get; }

    /// <summary>
    /// Unsubscribe all observers (subscribers)
    /// </summary>
    void EndTransmission();

    /// <summary>
    /// Resends all newer values to a requesting observer,
    /// starting from a specific timestamp.
    /// </summary>
    /// <param name="toObserver">Observer recipient</param>
    /// <param name="fromTimestamp">From timestamp, inclusive</param>
    /// <param name="act">Caching instruction override</param>
    /// <exception cref="ArgumentException">
    /// Observer is not subscribed to the provider.
    /// </exception>
    void Resend<T>(
        IObserverHub<TSeries, T> toObserver,
        DateTime fromTimestamp,
        Act act) where T : struct, ISeries;

    /// <summary>
    /// Resends all newer values to a requesting observer,
    /// starting at an index position.
    /// </summary>
    /// <param name="toObserver">Observer recipient</param>
    /// <param name="fromIndex">From index, inclusive</param>
    /// <param name="act">Caching instruction override</param>
    /// <exception cref="ArgumentException">
    /// Observer is not subscribed to the provider.
    /// </exception>
    void Resend<T>(
        IObserverHub<TSeries, T> toObserver,
        int fromIndex,
        Act act) where T : struct, ISeries;

    /// <summary>
    /// Resends all values in a range to a requesting observer,
    /// starting and ending at an index position.
    /// </summary>
    /// <param name="toObserver">Observer recipient</param>
    /// <param name="fromIndex">Starting index, inclusive</param>
    /// <param name="toIndex">Ending index, inclusive</param>
    /// <param name="act">Caching instruction override</param>
    /// <exception cref="ArgumentException">
    /// Observer is not subscribed to the provider.
    /// </exception>
    void Resend<T>(
        IObserverHub<TSeries, T> toObserver,
        int fromIndex,
        int toIndex,
        Act act) where T : struct, ISeries;

    /// <summary>
    /// Deletes all cached time-series records,
    /// without restore.  When applicable,
    /// it will cascade delete commands to subscribers.
    /// </summary>
    /// <remarks>
    /// For observers, if your intention is to rebuild from a provider,
    /// use alternate <see cref="IStreamObserver{TIn}.RebuildCache()"/>.
    /// </remarks>
    void ClearCache() => ClearCache(0);

    /// <summary>
    /// Deletes newer cached records from point in time,
    /// without restore.  When applicable, it will cascade delete
    /// commands to subscribers.
    /// </summary>
    /// <remarks>
    /// For observers, if your intention is to rebuild from a provider,
    /// use alternate <see cref="IStreamObserver{TIn}.RebuildCache(DateTime)"/>.
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
    /// use alternate <see cref="IStreamObserver{TIn}.RebuildCache(int)"/>.
    /// </remarks>
    /// <param name="fromIndex">From index, inclusive</param>
    void ClearCache(int fromIndex);
}

#region QUOTE, CHAIN, RESULT PROVIDER variants

// these contrain specific struct types

/// <summary>
/// Quote provider interface (observable)
/// </summary>
/// <typeparam name="TQuote"></typeparam>
public interface IQuoteProvider<TQuote> : IStreamProvider<TQuote>
    where TQuote : struct, IQuote;

/// <summary>
/// Chainable provider interface (observable)
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IChainProvider<TResult> : IStreamProvider<TResult>
    where TResult : struct, IReusable;

/// <summary>
/// Non-chainable provider interface (observable)
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IResultProvider<TResult> : IStreamProvider<TResult>
    where TResult : struct, IResult;
#endregion
