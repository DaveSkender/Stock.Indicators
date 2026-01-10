namespace Skender.Stock.Indicators;

/// <summary>
/// Flexible IStreamObserver implementation with customizable callbacks for all lifecycle methods.
/// Most callbacks are optional (nullable) except onNext which is required.
/// </summary>
public class ReusableObserver : IStreamObserver<IReusable>
{
    private readonly Action<IReusable> _onNext;
    private readonly Action<Exception>? _onError;
    private readonly Action? _onCompleted;
    private readonly Action? _onUnsubscribe;
    private readonly Action<IReusable, bool, int?>? _onAdd;
    private readonly Action<DateTime>? _onRebuildFromTimestamp;
    private readonly Action<DateTime>? _onPrune;
    private readonly Action? _onReinitialize;
    private readonly Action? _onRebuild;
    private readonly Action<DateTime>? _onRebuildTimestamp;
    private readonly Action<int>? _onRebuildIndex;
    private bool _isSubscribed = false;

    public bool IsSubscribed => _isSubscribed;

    public ReusableObserver(
        Action<IReusable> onNext,
        Action<Exception>? onError = null,
        Action? onCompleted = null,
        Action? onUnsubscribe = null,
        Action<IReusable, bool, int?>? onAdd = null,
        Action<DateTime>? onRebuildFromTimestamp = null,
        Action<DateTime>? onPrune = null,
        Action? onReinitialize = null,
        Action? onRebuild = null,
        Action<DateTime>? onRebuildTimestamp = null,
        Action<int>? onRebuildIndex = null)
    {
        _onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));
        _onError = onError;
        _onCompleted = onCompleted;
        _onUnsubscribe = onUnsubscribe;
        _onAdd = onAdd;
        _onRebuildFromTimestamp = onRebuildFromTimestamp;
        _onPrune = onPrune;
        _onReinitialize = onReinitialize;
        _onRebuild = onRebuild;
        _onRebuildTimestamp = onRebuildTimestamp;
        _onRebuildIndex = onRebuildIndex;
    }

    public void OnNext(IReusable value) { _onNext(value); }

    public void OnError(Exception error) => _onError?.Invoke(error);

    public void OnCompleted() => _onCompleted?.Invoke();

    public void Unsubscribe() { _isSubscribed = false; _onUnsubscribe?.Invoke(); }

    // OnAdd is called when a new item is added to the hub's cache
    public void OnAdd(IReusable item, bool notify, int? indexHint) => _onAdd?.Invoke(item, notify, indexHint);

    // OnRebuild is called when the hub recalculates from a specific timestamp
    public void OnRebuild(DateTime fromTimestamp) => _onRebuildFromTimestamp?.Invoke(fromTimestamp);

    // OnPrune is called when old items are removed from the hub's cache
    public void OnPrune(DateTime toTimestamp) => _onPrune?.Invoke(toTimestamp);

    // Reinitialize is called to reset the observer state
    public void Reinitialize() { _isSubscribed = true; _onReinitialize?.Invoke(); }

    // Rebuild methods trigger recalculation
    public void Rebuild() => _onRebuild?.Invoke();

    public void Rebuild(DateTime fromTimestamp) => _onRebuildTimestamp?.Invoke(fromTimestamp);

    public void Rebuild(int fromIndex) => _onRebuildIndex?.Invoke(fromIndex);
}
