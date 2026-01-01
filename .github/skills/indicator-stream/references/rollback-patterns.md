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
    _window.Clear();

    int index = ProviderCache.IndexGte(timestamp);
    if (index <= 0) return;

    int targetIndex = index - 1;
    int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

    for (int p = startIdx; p <= targetIndex; p++)
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
    // Clear all state
    _highWindow.Clear();
    _lowWindow.Clear();
    _rawKBuffer.Clear();

    int index = ProviderCache.IndexGte(timestamp);
    if (index <= 0) return;

    int targetIndex = index - 1;

    // Rebuild windows
    int windowStart = Math.Max(0, targetIndex + 1 - LookbackPeriods);
    for (int p = windowStart; p <= targetIndex; p++)
    {
        IQuote q = ProviderCache[p];
        _highWindow.Add((double)q.High);
        _lowWindow.Add((double)q.Low);
    }

    // Prefill buffer for smoothing
    int bufferStart = Math.Max(0, targetIndex + 1 - SmoothPeriods);
    for (int p = bufferStart; p <= targetIndex; p++)
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
    _avgGain = double.NaN;
    _avgLoss = double.NaN;
    _prevValue = double.NaN;

    int index = ProviderCache.IndexGte(timestamp);
    if (index <= 0) return;

    int targetIndex = index - 1;

    // Replay warmup period to rebuild Wilder's smoothing state
    int startIdx = Math.Max(0, targetIndex + 1 - (2 * LookbackPeriods));
    for (int p = startIdx; p <= targetIndex; p++)
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
    int index = ProviderCache.IndexGte(timestamp);
    if (index <= 0)
    {
        _prevEma = double.NaN;
        return;
    }

    // Restore previous EMA from cache
    int priorIndex = index - 1;
    if (priorIndex >= LookbackPeriods)
    {
        EmaResult prior = Cache[priorIndex];
        _prevEma = prior.Ema ?? double.NaN;
    }
    else
    {
        _prevEma = double.NaN;
    }
}
```

**Reference**: `EmaHub.RollbackState`

## Key principles

1. **Clear all stateful fields first** - Reset to initial state
2. **Find the target index** - Use `ProviderCache.IndexGte(timestamp)`
3. **Replay from warmup start** - Calculate startIdx with lookback period
4. **Rebuild incrementally** - Process each cached item in order
5. **Match ToIndicator logic** - Use same calculations as normal processing

## Anti-patterns to avoid

**Inline rebuild detection in ToIndicator**:

```csharp
// WRONG - Don't do this!
bool needsRebuild = (i != _lastProcessedIndex + 1);
if (needsRebuild) { /* rebuild logic */ }
```

**Use RollbackState override instead** - Framework calls it automatically.

---
Last updated: December 31, 2025
