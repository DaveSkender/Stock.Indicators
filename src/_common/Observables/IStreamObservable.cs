namespace Skender.Stock.Indicators;

// STREAM PROVIDER (OBSERVABLE) INTERFACES

#region chain and quote variants

/// <inheritdoc />
public interface IQuoteProvider<out T> : IChainProvider<T>
    where T : IQuote
{
    IReadOnlyList<T> Quotes { get; }
}

/// <inheritdoc cref="IStreamObservable{T}" />
public interface IChainProvider<out T> : IStreamObservable<T>
    where T : IReusable;
#endregion

/// <summary>
/// Streaming provider (observable cache)
/// </summary>
/// <typeparam name="T">
/// The object that provides notification information.
/// </typeparam>
public interface IStreamObservable<out T>
{
    /// <summary>
    /// Current number of subscribers
    /// </summary>
    int ObserverCount { get; }

    /// <summary>
    /// Provider currently has subscribers
    /// </summary>
    bool HasObservers { get; }

    /// <summary>
    /// Checks if a specific observer is subscribed
    /// </summary>
    /// <param name="observer">
    /// Subscriber <c>IStreamObserver</c> reference
    /// </param>
    /// <returns>True if subscribed/registered</returns>
    bool HasSubscriber(IStreamObserver<T> observer);

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
    IDisposable Subscribe(IStreamObserver<T> observer);

    /// <summary>
    /// Unsubscribe from the data provider.
    /// </summary>
    /// <inheritdoc cref="HashSet{T}.Remove(T)"/>
    bool Unsubscribe(IStreamObserver<T> observer);

    /// <summary>
    /// Unsubscribe all observers (subscribers)
    /// </summary>
    void EndTransmission();

    /// <summary>
    /// Get a readonly reference of the observable cache.
    /// </summary>
    /// <returns>Read-only list of cached items.</returns>
    IReadOnlyList<T> GetCacheRef();
}
