# RollbackState patterns

## When to override RollbackState

Override `RollbackState(DateTime timestamp)` when the hub maintains stateful fields:

| State Type | Requires Override | Examples |
| ---------- | ----------------- | -------- |
| Rolling windows | Yes | `RollingWindowMax`, `RollingWindowMin` |
| Buffered values | Yes | Raw K buffer in Stoch |
| Running averages | Yes | EMA state, Wilder's smoothing |
| Previous values | Yes | `_prevValue`, `_prevHigh` |
| Simple counters | Maybe | `_warmupCount` if affects calculations |
| Stateless lookups | No | Pure cache-based calculations |

## Pattern: Simple rolling window

```csharp
private readonly RollingWindowMax<double> _window;

protected override void RollbackState(DateTime timestamp)
{
    int targetIndex = ProviderCache.IndexGte(timestamp);

    _window.Clear();

    if (targetIndex <= 0) return;

    int restoreIndex = targetIndex - 1;  // Rebuild up to but NOT including timestamp
    int startIdx = Math.Max(0, restoreIndex + 1 - LookbackPeriods);

    for (int p = startIdx; p <= restoreIndex; p++)
    {
        _window.Add(ProviderCache[p].Value);
    }
}
```

**Reference**: `ChandelierHub.RollbackState`

## Pattern: Complex with buffer prefill

```csharp
private readonly RollingWindowMax<double> _highWindow;
private readonly RollingWindowMin<double> _lowWindow;
private readonly Queue<double> _rawKBuffer;

protected override void RollbackState(DateTime timestamp)
{
    int targetIndex = ProviderCache.IndexGte(timestamp);

    // Clear all state
    _highWindow.Clear();
    _lowWindow.Clear();
    _rawKBuffer.Clear();

    if (targetIndex <= 0) return;

    int restoreIndex = targetIndex - 1;  // Rebuild up to but NOT including timestamp

    // Rebuild windows
    int windowStart = Math.Max(0, restoreIndex + 1 - LookbackPeriods);
    for (int p = windowStart; p <= restoreIndex; p++)
    {
        IQuote q = ProviderCache[p];
        _highWindow.Add((double)q.High);
        _lowWindow.Add((double)q.Low);
    }

    // Prefill buffer for smoothing
    int bufferStart = Math.Max(0, restoreIndex + 1 - SmoothPeriods);
    for (int p = bufferStart; p <= restoreIndex; p++)
    {
        double rawK = CalculateRawK(p);
        _rawKBuffer.Enqueue(rawK);
    }
}
```

**Reference**: `StochHub.RollbackState`

## Pattern: Wilder's smoothing state

```csharp
private double _avgGain = double.NaN;
private double _avgLoss = double.NaN;
private double _prevValue = double.NaN;

protected override void RollbackState(DateTime timestamp)
{
    int targetIndex = ProviderCache.IndexGte(timestamp);

    _avgGain = double.NaN;
    _avgLoss = double.NaN;
    _prevValue = double.NaN;

    if (targetIndex <= 0) return;

    int restoreIndex = targetIndex - 1;  // Rebuild up to but NOT including timestamp

    // Replay warmup period to rebuild Wilder's smoothing state
    int startIdx = Math.Max(0, restoreIndex + 1 - (2 * LookbackPeriods));
    for (int p = startIdx; p <= restoreIndex; p++)
    {
        double value = ProviderCache[p].Value;
        if (!double.IsNaN(_prevValue))
        {
            double gain = value > _prevValue ? value - _prevValue : 0;
            double loss = value < _prevValue ? _prevValue - value : 0;

            if (p >= LookbackPeriods)
            {
                _avgGain = ((_avgGain * (LookbackPeriods - 1)) + gain) / LookbackPeriods;
                _avgLoss = ((_avgLoss * (LookbackPeriods - 1)) + loss) / LookbackPeriods;
            }
        }
        _prevValue = value;
    }
}
```

**Reference**: `AdxHub.RollbackState`

## Pattern: Previous value tracking

```csharp
private double _prevEma = double.NaN;

protected override void RollbackState(DateTime timestamp)
{
    int targetIndex = ProviderCache.IndexGte(timestamp);

    if (targetIndex <= 0)
    {
        _prevEma = double.NaN;
        return;
    }

    // Restore previous EMA from cache (up to but NOT including timestamp)
    int restoreIndex = targetIndex - 1;
    if (restoreIndex >= LookbackPeriods)
    {
        EmaResult prior = Cache[restoreIndex];
        _prevEma = prior.Ema ?? double.NaN;
    }
    else
    {
        _prevEma = double.NaN;
    }
}
```

**Reference**: `EmaHub.RollbackState`

## Pattern: Compound hub state

For compound hubs that maintain state beyond the internal hub's results:

```csharp
private readonly RollingWindowMax<double> _rsiMaxWindow;
private readonly RollingWindowMin<double> _rsiMinWindow;
private readonly Queue<double> kBuffer;
private readonly Queue<double> signalBuffer;

protected override void RollbackState(DateTime timestamp)
{
    int targetIndex = ProviderCache.IndexGte(timestamp);

    // Clear all compound state
    _rsiMaxWindow.Clear();
    _rsiMinWindow.Clear();
    kBuffer.Clear();
    signalBuffer.Clear();

    if (targetIndex <= 0) return;

    int restoreIndex = targetIndex - 1;  // Rebuild up to but NOT including timestamp

    // Replay compound hub processing using cached internal hub results
    for (int i = 0; i <= restoreIndex; i++)
    {
        double rsiValue = ProviderCache[i].Value;  // ProviderCache holds RSI results
        if (!double.IsNaN(rsiValue))
        {
            _ = UpdateOscillatorState(rsiValue);  // Rebuild compound state
        }
    }
}
```

**Reference**: `StochRsiHub.RollbackState`

**Note**: Most compound hubs do NOT need RollbackState override. Only override when maintaining additional state beyond the internal hub's results. See `compound-hubs.md` for details.

## Key principles

1. **Clear all stateful fields first** - Reset to initial state
2. **Find the target index** - Use `ProviderCache.IndexGte(timestamp)`
3. **Calculate restore index** - Set `restoreIndex = targetIndex - 1` (rebuild up to but NOT including timestamp)
4. **Replay from warmup start** - Calculate startIdx with lookback period
5. **Rebuild incrementally** - Process each cached item in order up to `restoreIndex`
6. **Match ToIndicator logic** - Use same calculations as normal processing

**Critical**: The quote at the rollback timestamp will be recalculated when it arrives via normal processing. Do NOT include it in the replay loop.

## Anti-patterns to avoid

**Inline rebuild detection in ToIndicator**:

```csharp
// WRONG - Don't do this!
bool needsRebuild = (i != _lastProcessedIndex + 1);
if (needsRebuild) { /* rebuild logic */ }
```

**Use RollbackState override instead** - Framework calls it automatically.

---
Last updated: January 19, 2026
