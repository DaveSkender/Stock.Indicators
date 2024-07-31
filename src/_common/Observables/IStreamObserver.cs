namespace Skender.Stock.Indicators;

/// <summary>
/// Provides a mechanism for receiving push-based notifications.
/// </summary>
/// <typeparam name="T">
/// The object that provides notification information.
/// </typeparam>
public interface IStreamObserver<in T>
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
    /// </summary>
    /// <remarks>
    /// This unsubscribes from the provider,
    /// rebuilds the cache, resets faulted states,
    /// and then will re-subscribe to the provider.
    /// <para>
    /// This is done automatically on on hub
    /// construction, so it's only needed if you
    /// want to manually reset the hub.
    /// </para>
    /// <para>
    /// If you only need to rebuild the cache,
    /// use <see cref="RebuildCache()"/> instead.
    /// </para>
    /// </remarks>
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
    /// Reset the entire results cache from a point in time
    /// and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromTimestamp">
    /// All periods (inclusive) after this date/time
    /// will be removed and recalculated.
    /// </param>
    void RebuildCache(DateTime fromTimestamp);

    /// <summary>
    /// Resets the results cache from an index position
    /// and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromIndex">
    /// All periods (inclusive) after this index position
    /// will be removed and recalculated.
    /// </param>
    void RebuildCache(int fromIndex);

    /// <summary>
    /// Notifies the observer that the provider has
    /// finished sending push-based notifications.
    /// </summary>
    void OnCompleted();

    /// <summary>
    /// Notifies the observer that the provider has
    /// experienced an error condition.
    /// </summary>
    /// <param name="exception">
    /// An object that provides additional information
    /// about the error.
    /// </param>
    void OnError(Exception exception);

    /// <summary>
    /// Provides the observer with new data.
    /// </summary>
    /// <param name="value">
    /// The current notification information.
    /// </param>
    void OnNext(T value);
}
