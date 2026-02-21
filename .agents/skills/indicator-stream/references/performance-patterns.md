# Performance patterns

## Performance target

**Goal**: StreamHub ≤ 1.5x slower than Series (batch) implementation.

## Anti-pattern: O(n²) recalculation

**Impact**: 1000x slowdown for large datasets

```csharp
// WRONG - O(n²) complexity
protected override (RsiResult result, int index) ToIndicator(IReusable item, int? indexHint)
{
    int i = indexHint ?? ProviderCache.IndexOf(item, true);

    // Building subset and recalculating ENTIRE history every tick
    List<IReusable> subset = [];
    for (int k = 0; k <= i; k++)
    {
        subset.Add(ProviderCache[k]);
    }

    // O(n) calculation on O(n) history = O(n²)
    IReadOnlyList<RsiResult> seriesResults = subset.ToRsi(LookbackPeriods);
    rsi = seriesResults[seriesResults.Count - 1]?.Rsi;

    return (new RsiResult(item.Timestamp, rsi), i);
}
```

**CORRECT**: Maintain incremental state or reference cache:

```csharp
// CORRECT - O(1) per tick using cache reference
protected override (EmaResult result, int index) ToIndicator(IReusable item, int? indexHint)
{
    int i = indexHint ?? ProviderCache.IndexOf(item, true);

    double ema = i >= LookbackPeriods - 1
        ? Cache[i - 1].Ema is not null
            // normal: reference previous result from cache
            ? Ema.Increment(K, Cache[i - 1].Value, item.Value)
            // re/initialize as SMA
            : Sma.Increment(ProviderCache, LookbackPeriods, i)
        : double.NaN;

    return (new EmaResult(item.Timestamp, ema.NaN2Null()), i);
}
```

**Note**: Prefer referencing `Cache[i - 1]` instead of storing duplicate state when the previous result is available.

## Anti-pattern: O(n) window scans

**Impact**: 20x slowdown for window-based indicators

```csharp
// WRONG - O(n) linear scan every tick
decimal highHigh = decimal.MinValue;
for (int p = i - LookbackPeriods; p < i; p++)
{
    if (ProviderCache[p].High > highHigh)
        highHigh = ProviderCache[p].High;
}
```

**CORRECT**: Use RollingWindow utilities:

```csharp
// CORRECT - O(1) amortized operations
private readonly RollingWindowMax<decimal> _highWindow;
private readonly RollingWindowMin<decimal> _lowWindow;

internal DonchianHub(IQuoteProvider<IQuote> provider, int lookbackPeriods)
    : base(provider)
{
    _highWindow = new RollingWindowMax<decimal>(lookbackPeriods);
    _lowWindow = new RollingWindowMin<decimal>(lookbackPeriods);
    Reinitialize();
}

protected override (DonchianResult result, int index) ToIndicator(IQuote item, int? indexHint)
{
    // O(1) amortized add operation
    _highWindow.Add(item.High);
    _lowWindow.Add(item.Low);

    // O(1) max/min retrieval
    decimal highHigh = _highWindow.Max;
    decimal lowLow = _lowWindow.Min;

    return (new DonchianResult(item.Timestamp, highHigh, lowLow), indexHint ?? 0);
}
```

## Wilder's smoothing helper

**Use case**: RSI, CMO, ATR, ADX, SMMA, Alligator smoothing

```csharp
// Formula: smoothedValue = ((prevSmoothed × (period - 1)) + currentValue) / period

public static double WilderSmoothing(double prevSmoothed, double currentValue, int period)
    => ((prevSmoothed * (period - 1)) + currentValue) / period;

// Usage:
_avgGain = Smoothing.WilderSmoothing(_avgGain, gain, LookbackPeriods);
```

## EMA incremental pattern

**Use case**: Any exponential moving average calculation

```csharp
// Calculate multiplier once in constructor
private readonly double _multiplier;

internal EmaHub(IChainProvider<IReusable> provider, int lookbackPeriods)
    : base(provider)
{
    _multiplier = 2.0 / (lookbackPeriods + 1);
    Reinitialize();
}

// O(1) EMA update
double ema = (_multiplier * (currentValue - _prevEma)) + _prevEma;
```

## Running sum pattern

**Use case**: SMA, standard deviation components

```csharp
private double _runningSum;
private readonly Queue<double> _buffer;

protected override (SmaResult result, int index) ToIndicator(IReusable item, int? indexHint)
{
    double value = item.Value;

    if (_buffer.Count == LookbackPeriods)
    {
        _runningSum -= _buffer.Dequeue();
    }

    _buffer.Enqueue(value);
    _runningSum += value;

    double sma = _runningSum / _buffer.Count;
    return (new SmaResult(item.Timestamp, sma), indexHint ?? 0);
}
```

## Performance checklist

- [ ] No Series method calls inside ToIndicator
- [ ] No loops scanning provider cache on every tick
- [ ] State variables maintained incrementally
- [ ] RollingWindow utilities for max/min operations
- [ ] Multipliers and constants calculated once in constructor
- [ ] Memory allocations minimized in hot path

## Benchmark validation

Add benchmark to `tools/performance/Perf.Stream.cs`:

```csharp
[Benchmark]
public object MyIndicatorHub() => quoteHub.ToMyIndicatorHub(14).Results;
```

Run benchmarks:

```bash
cd tools/performance
dotnet run -c Release --filter *.MyIndicatorHub
```

---
Last updated: December 31, 2025
