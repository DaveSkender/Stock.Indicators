namespace Skender.Stock.Indicators;

// STREAM PROVIDER INTERFACES

/// <inheritdoc />
public interface IQuoteProvider<TQuote>
    : IChainProvider<TQuote>
    where TQuote : IQuote;

/// <inheritdoc />
public interface IChainProvider<TReusable>
    : IStreamProvider<TReusable>
    where TReusable : IReusable;

/// <summary>
/// Streaming provider interface (observable cache)
/// </summary>
public interface IStreamProvider<TSeries>
    : IObservable<(Act, TSeries, int?)>, IStreamCache<TSeries>
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
    /// Checks if a specific observer is subscribed
    /// </summary>
    /// <param name="observer">
    /// Subscriber <c>IObserver</c> reference
    /// </param>
    /// <returns>True if subscribed/registered</returns>
    bool HasSubscriber(
        IObserver<(Act, TSeries, int?)> observer);

    /// <summary>
    /// Unsubscribe all observers (subscribers)
    /// </summary>
    void EndTransmission();
}
