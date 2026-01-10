namespace Skender.Stock.Indicators;

/// <summary>
/// Flexible IStreamObserver implementation with customizable callbacks for all lifecycle methods.
/// All callbacks are optional (nullable).
/// </summary>
public class ReusableObserver : IStreamObserver<IReusable>
{
    private readonly Func<bool> _isSubscribed;
    private readonly Action<Exception>? _onError;
    private readonly Action? _onCompleted;
    private readonly Action? _onUnsubscribe;
    private readonly Action<IReusable, bool, int?>? _onAdd;
    private readonly Action<DateTime>? _onRebuild;
    private readonly Action<DateTime>? _onPrune;
    private readonly Action? _onReinitialize;
    private readonly Action? _rebuild;
    private readonly Action<DateTime>? _rebuildTimestamp;
    private readonly Action<int>? _rebuildIndex;

    public bool IsSubscribed => _isSubscribed();

    public ReusableObserver(
        Func<bool> isSubscribed,
        Action<Exception>? onError = null,
        Action? onCompleted = null,
        Action? onUnsubscribe = null,
        Action<IReusable, bool, int?>? onAdd = null,
        Action<DateTime>? onRebuild = null,
        Action<DateTime>? onPrune = null,
        Action? onReinitialize = null,
        Action? rebuild = null,
        Action<DateTime>? rebuildTimestamp = null,
        Action<int>? rebuildIndex = null)
    {
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

    public void OnError(Exception exception) => _onError?.Invoke(exception);

    public void OnCompleted() => _onCompleted?.Invoke();

    public void Unsubscribe() { _onUnsubscribe?.Invoke(); }

    // OnAdd is called when a new item is added to the hub's cache
    public void OnAdd(IReusable item, bool notify, int? indexHint) => _onAdd?.Invoke(item, notify, indexHint);

    // OnRebuild is called when the hub recalculates from a specific timestamp
    public void OnRebuild(DateTime fromTimestamp) => _onRebuild?.Invoke(fromTimestamp);

    // OnPrune is called when old items are removed from the hub's cache
    public void OnPrune(DateTime toTimestamp) => _onPrune?.Invoke(toTimestamp);

    // Reinitialize is called to reset the observer state
    public void Reinitialize() { _onReinitialize?.Invoke(); }

    // Rebuild methods trigger recalculation
    public void Rebuild() => _rebuild?.Invoke();

    public void Rebuild(DateTime fromTimestamp) => _rebuildTimestamp?.Invoke(fromTimestamp);

    public void Rebuild(int fromIndex) => _rebuildIndex?.Invoke(fromIndex);
}
