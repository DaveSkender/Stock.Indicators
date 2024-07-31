namespace Skender.Stock.Indicators;

// STREAM PROVIDER (OBSERVABLE) INTERFACES

/// <inheritdoc />
public interface IQuoteProvider<TQuote>
    : IChainProvider<TQuote>
    where TQuote : IQuote;

/// <inheritdoc />
public interface IChainProvider<TReusable>
    : IStreamObservable<TReusable>
    where TReusable : IReusable;

/// <summary>
/// Streaming provider (observable cache)
/// </summary>
public interface IStreamObservable<TSeries>
    : IStreamCache<TSeries>
    where TSeries : ISeries
{
    /// <summary>
    /// Currently has subscribers
    /// </summary>
    bool HasSubscribers { get; }

    /// <summary>
    /// Current number of subscribers
    /// </summary>
    int SubscriberCount { get; }

    /// <summary>
    /// Notifies the provider that an observer is to receive notifications.
    /// </summary>
    /// <param name="observer">
    /// The object that is to receive notifications.
    /// </param>
    /// <returns>
    /// A reference to an interface that allows observers
    /// to stop receiving notifications before the provider
    /// has finished sending them.
    /// </returns>
    IDisposable Subscribe(IStreamObserver<(Act, TSeries, int?)> observer);

    /// <summary>
    /// Checks if a specific observer is subscribed
    /// </summary>
    /// <param name="observer">
    /// Subscriber <c>IStreamObserver</c> reference
    /// </param>
    /// <returns>True if subscribed/registered</returns>
    bool HasSubscriber(
        IStreamObserver<(Act, TSeries, int?)> observer);

    /// <summary>
    /// Unsubscribe all observers (subscribers)
    /// </summary>
    void EndTransmission();

    /// <summary>
    /// Deletes newer cached records from point in time.
    /// </summary>
    /// <remarks>
    /// For observers, if your intention is to rebuild from a provider,
    /// use alternate <see cref="IStreamObserver{T}.RebuildCache(DateTime)"/>.
    /// </remarks>
    /// <param name="fromTimestamp">
    /// All periods (inclusive) after this DateTime will be removed.
    /// </param>
    void ClearCache(DateTime fromTimestamp);

    /// <summary>
    /// Deletes newer cached records from an index position (inclusive).
    /// </summary>
    /// <remarks>
    /// For observers, if your intention is to rebuild from a provider,
    /// use alternate <see cref="IStreamObserver{T}.RebuildCache(int)"/>.
    /// </remarks>
    /// <param name="fromIndex">From index, inclusive</param>
    void ClearCache(int fromIndex);
}
