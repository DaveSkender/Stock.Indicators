namespace Skender.Stock.Indicators;

/// <summary>
/// Flexible IStreamObserver implementation with customizable callbacks for all lifecycle methods.
/// All callbacks are optional (nullable).
/// </summary>
public class ReusableObserver<TResult> : IStreamObserver<TResult>
    where TResult : ISeries
{
    private readonly Func<bool> _isSubscribed;
    private readonly Action<Exception>? _onError;
    private readonly Action? _onCompleted;
    private readonly Action? _onUnsubscribe;
    private readonly Action<TResult, bool, int?>? _onAdd;
    private readonly Action<DateTime>? _onRebuild;
    private readonly Action<DateTime>? _onPrune;
    private readonly Action? _onReinitialize;
    private readonly Action? _rebuild;
    private readonly Action<DateTime>? _rebuildTimestamp;
    private readonly Action<int>? _rebuildIndex;

    /// <summary>
    /// Gets a value indicating whether the observer is currently subscribed to the data provider.
    /// </summary>
    /// <value>
    /// <c>true</c> if the observer is subscribed; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    /// This property is thread-safe and returns the current subscription state
    /// by invoking the backing delegate <see cref="_isSubscribed"/>.
    /// The value is determined by the provider at the time of invocation.
    /// </remarks>
    public bool IsSubscribed => _isSubscribed();

    /// <summary>
    /// Initializes a new instance of the <see cref="ReusableObserver{TResult}"/> class
    /// with customizable callbacks for all lifecycle methods.
    /// </summary>
    /// <param name="isSubscribed">
    /// A function that returns the current subscription state. This parameter is required.
    /// </param>
    /// <param name="onError">
    /// Optional callback invoked when an error occurs in the data provider.
    /// </param>
    /// <param name="onCompleted">
    /// Optional callback invoked when the data provider has finished sending notifications.
    /// </param>
    /// <param name="onUnsubscribe">
    /// Optional callback invoked when unsubscribing from the data provider.
    /// </param>
    /// <param name="onAdd">
    /// Optional callback invoked when a new item is added to the hub's cache.
    /// </param>
    /// <param name="onRebuild">
    /// Optional callback invoked when the hub recalculates from a specific timestamp.
    /// </param>
    /// <param name="onPrune">
    /// Optional callback invoked when old items are removed from the hub's cache.
    /// </param>
    /// <param name="onReinitialize">
    /// Optional callback invoked to reset the observer state.
    /// </param>
    /// <param name="rebuild">
    /// Optional callback invoked to trigger a full recalculation of the cache.
    /// </param>
    /// <param name="rebuildTimestamp">
    /// Optional callback invoked to trigger a recalculation from a specific timestamp.
    /// </param>
    /// <param name="rebuildIndex">
    /// Optional callback invoked to trigger a recalculation from a specific index position.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="isSubscribed"/> is <c>null</c>.
    /// </exception>
    /// <remarks>
    /// All callback parameters except <paramref name="isSubscribed"/> are optional (nullable).
    /// The <paramref name="isSubscribed"/> delegate is assigned to the <see cref="_isSubscribed"/> field
    /// and must not be <c>null</c>.
    /// </remarks>
    public ReusableObserver(
        Func<bool> isSubscribed,
        Action<Exception>? onError = null,
        Action? onCompleted = null,
        Action? onUnsubscribe = null,
        Action<TResult, bool, int?>? onAdd = null,
        Action<DateTime>? onRebuild = null,
        Action<DateTime>? onPrune = null,
        Action? onReinitialize = null,
        Action? rebuild = null,
        Action<DateTime>? rebuildTimestamp = null,
        Action<int>? rebuildIndex = null)
    {
        ArgumentNullException.ThrowIfNull(isSubscribed);

        _isSubscribed = isSubscribed;
        _onError = onError;
        _onCompleted = onCompleted;
        _onUnsubscribe = onUnsubscribe;
        _onAdd = onAdd;
        _onRebuild = onRebuild;
        _onPrune = onPrune;
        _onReinitialize = onReinitialize;
        _rebuild = rebuild;
        _rebuildTimestamp = rebuildTimestamp;
        _rebuildIndex = rebuildIndex;
    }

    /// <summary>
    /// Provides the observer with errors from the provider that have produced a faulted state.
    /// </summary>
    /// <param name="exception">
    /// An exception with additional information about the error.
    /// </param>
    /// <remarks>
    /// Invokes the optional <c>_onError</c> callback if provided during construction.
    /// </remarks>
    public void OnError(Exception exception) => _onError?.Invoke(exception);

    /// <summary>
    /// Provides the observer with final notice that the data provider has finished sending
    /// push-based notifications.
    /// </summary>
    /// <remarks>
    /// Completion indicates that publisher will never send additional data.
    /// This is only used for finite data streams and is different from faulted <see cref="OnError(Exception)"/>.
    /// Invokes the optional <c>_onCompleted</c> callback if provided during construction.
    /// </remarks>
    public void OnCompleted() => _onCompleted?.Invoke();

    /// <summary>
    /// Unsubscribe from the data provider.
    /// </summary>
    /// <remarks>
    /// Invokes the optional <c>_onUnsubscribe</c> callback if provided during construction.
    /// </remarks>
    public void Unsubscribe() { _onUnsubscribe?.Invoke(); }

    /// <summary>
    /// Provides the observer with new data when an item is added to the hub's cache.
    /// </summary>
    /// <param name="item">
    /// The current notification information containing the data item to add.
    /// </param>
    /// <param name="notify">
    /// Notify subscribers of the new item.
    /// </param>
    /// <param name="indexHint">
    /// Provider index hint, if known. This parameter is nullable.
    /// </param>
    /// <remarks>
    /// Invokes the optional <c>_onAdd</c> callback if provided during construction.
    /// </remarks>
    public void OnAdd(TResult item, bool notify, int? indexHint) => _onAdd?.Invoke(item, notify, indexHint);

    /// <summary>
    /// Provides the observer with starting point in timeline to rebuild
    /// and cascade to all its own subscribers.
    /// </summary>
    /// <param name="fromTimestamp">
    /// Starting point in timeline to rebuild. All periods (inclusive) after this date/time
    /// will be removed and recalculated.
    /// </param>
    /// <remarks>
    /// Invokes the optional <c>_onRebuild</c> callback if provided during construction.
    /// </remarks>
    public void OnRebuild(DateTime fromTimestamp) => _onRebuild?.Invoke(fromTimestamp);

    /// <summary>
    /// Provides the observer with notification to prune data.
    /// </summary>
    /// <param name="toTimestamp">
    /// Ending point in timeline to prune. Old items up to this timestamp
    /// will be removed from the hub's cache.
    /// </param>
    /// <remarks>
    /// Invokes the optional <c>_onPrune</c> callback if provided during construction.
    /// </remarks>
    public void OnPrune(DateTime toTimestamp) => _onPrune?.Invoke(toTimestamp);

    /// <summary>
    /// Full reset of the provider subscription.
    /// </summary>
    /// <remarks>
    /// This unsubscribes from the provider,
    /// rebuilds the cache, resets faulted states,
    /// and then re-subscribes to the provider.
    /// Invokes the optional <c>_onReinitialize</c> callback if provided during construction.
    /// <para>
    /// This is done automatically on hub instantiation, so it's only needed if you
    /// want to manually reset the hub.
    /// </para>
    /// <para>
    /// If you only need to rebuild the cache, use <see cref="Rebuild()"/> instead.
    /// </para>
    /// </remarks>
    public void Reinitialize() { _onReinitialize?.Invoke(); }

    /// <summary>
    /// Resets the entire results cache and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <remarks>
    /// This is different from <see cref="Reinitialize()"/>.
    /// It does not reset the provider subscription.
    /// Invokes the optional <c>_rebuild</c> callback if provided during construction.
    /// </remarks>
    public void Rebuild() => _rebuild?.Invoke();

    /// <summary>
    /// Resets the results cache from a point in time and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromTimestamp">
    /// All periods (inclusive) after this date/time will be removed and recalculated.
    /// </param>
    /// <remarks>
    /// Invokes the optional <c>_rebuildTimestamp</c> callback if provided during construction.
    /// </remarks>
    public void Rebuild(DateTime fromTimestamp) => _rebuildTimestamp?.Invoke(fromTimestamp);

    /// <summary>
    /// Resets the results cache from an index position and rebuilds it from provider sources,
    /// with cascading updates to subscribers.
    /// </summary>
    /// <param name="fromIndex">
    /// All periods (inclusive) after this index position will be removed and recalculated.
    /// </param>
    /// <remarks>
    /// Invokes the optional <c>_rebuildIndex</c> callback if provided during construction.
    /// </remarks>
    public void Rebuild(int fromIndex) => _rebuildIndex?.Invoke(fromIndex);
}
