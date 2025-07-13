namespace Skender.Stock.Indicators;

/// <summary>
/// Management of observing + processing of streamed inbound data.
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
    /// Provides the observer with new data.
    /// </summary>
    /// <param name="item">
    /// The current notification information.
    /// </param>
    /// <param name="notify">
    /// Notify subscribers of the new item.
    /// </param>
    /// <param name="indexHint">
    /// Provider index hint, if known.
    /// </param>
    void OnAdd(T item, bool notify, int? indexHint);

    /// <summary>
    /// Provides the observer with starting point in timeline
    /// to rebuild and cascade to all its own subscribers.
    /// </summary>
    /// <param name="fromTimestamp">
    /// Starting point in timeline to rebuild.
    /// </param>
    void OnRebuild(DateTime fromTimestamp);

    /// <summary>
    /// Provides the observer with notification to prune data.
    /// </summary>
    /// <param name="toTimestamp">
    /// Ending point in timeline to prune.
    /// </param>
    void OnPrune(DateTime toTimestamp);

    /// <summary>
    /// Provides the observer with errors from the provider
    /// that have produced a faulted state.
    /// </summary>
    /// <param name="exception">
    /// An exception with additional information about the error.
    /// </param>
    void OnError(Exception exception);

    /// <summary>
    /// Provides the observer with final notice that the data
    /// provider has finished sending push-based notifications.
    /// </summary>
    /// <remarks>
    /// Completion indicates that publisher will never send
    /// additional data. This is only used for finite data
    /// streams; and is different from faulted OnError().
    /// </remarks>
    void OnCompleted();

    /// <summary>
    /// Full reset of the provider subscription.
    /// </summary>
    /// <remarks>
    /// This unsubscribes from the provider,
    /// rebuilds the cache, resets faulted states,
    /// and then re-subscribes to the provider.
    /// <para>
    /// This is done automatically on hub
    /// instantiation, so it's only needed if you
    /// want to manually reset the hub.
    /// </para>
    /// <para>
    /// If you only need to rebuild the cache,
    /// use <see cref="Rebuild()"/> instead.
    /// </para>
    /// </remarks>
    void Reinitialize();

    /// <summary>
    /// Resets the entire results cache
    /// and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <remarks>
    /// This is different from <see cref="Reinitialize()"/>.
    /// It does not reset the provider subscription.
    /// </remarks>
    void Rebuild();

    /// <summary>
    /// Resets the results cache from a point in time
    /// and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromTimestamp">
    /// All periods (inclusive) after this date/time
    /// will be removed and recalculated.
    /// </param>
    void Rebuild(DateTime fromTimestamp);

    /// <summary>
    /// Resets the results cache from an index position
    /// and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromIndex">
    /// All periods (inclusive) after this index position
    /// will be removed and recalculated.
    /// </param>
    void Rebuild(int fromIndex);
}
