---
applyTo: "src/**/*.StreamHub.cs,tests/**/*.StreamHub.Tests.cs"
description: "Stream-style indicator development and testing guidelines"
---

# Stream indicator development guidelines

These instructions apply to stream-style indicators that support real-time data processing with stateful operations. Stream indicators maintain internal state and can process individual quotes as they arrive.

**Important test rule**: Each StreamHub test class must implement exactly one observer interface (ITestQuoteObserver OR ITestChainObserver OR ITestPairsObserver) and at most one provider interface (ITestChainProvider).

## Code completion checklist

When implementing or updating an indicator, you must complete:

- [ ] Source code: `src/**/{IndicatorName}.StreamHub.cs` file exists and adheres to these instructions
  - [ ] Uses the appropriate provider base (`ChainProvider`, `QuoteProvider`, or `PairsProvider`) for the I/O scenario
  - [ ] Validates parameters in constructor and calls `Reinitialize()` as needed
  - [ ] Implements required `Add(...)` or conversion methods per provider base; maintains O(1) state updates where possible
  - [ ] Overrides `ToString()` with concise hub name; implements `Reset()`/state reinit patterns as applicable
  - [ ] Member order matches conventions in this document
- [ ] Catalog: `src/**/{IndicatorName}.Catalog.cs` exists, is accurate, and registered in `src\_common\Catalog\Catalog.Listings.cs` (`PopulateCatalog`)
- [ ] Unit testing: `tests/indicators/**/{IndicatorName}.StreamHub.Tests.cs` file exists and adheres to these instructions
  - [ ] Inherits `StreamHubTestBase` and implements test interfaces based on provider pattern (see test interface selection guide)
  - [ ] Verifies stateful processing, reset behavior, and consistency with Series results
  - [ ] For dual-stream hubs: covers timestamp sync validation and sufficient data checks
    - [ ] Require comprehensive rollback validation: warmup prefill, duplicate arrivals, provider history mutations (Insert and Remove), and strict Series parity checks; follow EMA hub test pattern
- [ ] Common items: Complete regression, performance, docs, and migration per `.github/copilot-instructions.md` (Common indicator requirements)

## Stream Hub I/O Scenarios

The codebase implements several types of stream hub I/O patterns:

1. **IQuote → IReusable** (e.g., EMA, SMA, ADX): Takes quote input, produces single reusable value output
   - Uses `IChainProvider<IReusable>` and extends `ChainProvider<IReusable, TResult>`
   - Generic constraint: `where TIn : IReusable`
2. **IQuote → ISeries** (e.g., Alligator, AtrStop): Takes quote input, produces multi-value series output  
   - Uses `IChainProvider<IQuote>` and extends `ChainProvider<TIn, TResult>`
   - Generic constraint: `where TIn : IReusable`
3. **IReusable → IReusable** (e.g., chained indicators): Takes reusable input, produces reusable output
   - Uses `IChainProvider<IReusable>` and extends `ChainProvider<IReusable, TResult>`
   - Generic constraint: `where TIn : IReusable`
4. **IQuote → IQuote** (e.g., Renko, Quote converters): Takes quote input, produces modified quote output
   - Uses `IQuoteProvider<IQuote>` and extends `QuoteProvider<TIn, TResult>`
   - Generic constraint: `where TIn : IQuote`
5. **IQuote → VolumeWeighted** (e.g., VWMA): Takes quote input, requires both price and volume data
   - Uses `IQuoteProvider<IQuote>` and extends `QuoteProvider<TIn, TResult>`
   - Generic constraint: `where TIn : IQuote`
6. **Dual IReusable → IReusable** (e.g., Correlation, Beta): Takes two synchronized reusable inputs, produces reusable output
   - Uses `IPairsProvider<TIn>` and extends `PairsProvider<TIn, TResult>`
   - Generic constraint: `where TIn : IReusable`
   - **NEW**: Added for dual-stream indicators requiring synchronized pair inputs

**Provider Selection Guidelines**:

- Use `IQuoteProvider<IQuote>` and `QuoteProvider<TIn, TResult>` when the indicator requires multiple quote properties (e.g., OHLCV data)
- Use `IChainProvider<IReusable>` and `ChainProvider<IReusable, TResult>` when the indicator can work with single reusable values. Heuristic: if the result type implements `IReusable` and exposes a chainable `Value` property, the hub should act as a chain provider (for example, `AdxResult : IReusable` with `Value => Adx`).
- Use `IPairsProvider<TIn>` and `PairsProvider<TIn, TResult>` when the indicator requires synchronized dual inputs (e.g., Correlation, Beta)

Note: IQuote → QuotePart selectors exist but are rarely used for new indicators.

## File naming conventions

Stream indicators should follow these naming patterns:

- **Implementation**: `{IndicatorName}.StreamHub.cs`
- **Tests**: `{IndicatorName}.StreamHub.Tests.cs`

## Reference implementations

Use these concrete hubs and tests as canonical patterns when implementing new stream indicators:

- Chain provider (single-input reusable):
  - `src/e-k/Ema/Ema.StreamHub.cs` — canonical chain provider hub (minimal hot-path allocations, simple state rollback)
  - `tests/indicators/e-k/Ema/Ema.StreamHub.Tests.cs` — canonical test coverage including provider history Insert/Remove scenarios

- Rolling window optimizations:
  - `src/a-d/Chandelier/Chandelier.StreamHub.cs` — RollingWindowMax/Min usage, RollbackState for window rebuild
  - `src/s-z/Stoch/Stoch.StreamHub.cs` — Complex rollback with window rebuild + buffer prefill

- Multi-series outputs from quotes:
  - `src/a-d/AtrStop/AtrStop.StreamHub.cs` — quote-driven series output pattern (stop bands)
  - `src/a-d/Alligator/Alligator.StreamHub.cs` — multi-line series output with shared state

- Complex state management:
  - `src/a-d/Adx/Adx.StreamHub.cs` — Wilder's smoothing state, comprehensive RollbackState with full warmup replay

- Dual-input (pairs) hubs:
  - `src/a-d/Correlation/Correlation.StreamHub.cs` — synchronized pairs with `PairsProvider`
  - `src/a-d/Beta/Beta.StreamHub.cs` — regression/risk variant on pairs pattern

- Quote-only provider:
  - `src/m-r/Renko/Renko.StreamHub.cs` — quote provider pattern that cannot observe chains

Previously deferred indicators (Fractal, HtTrendline, Hurst, Ichimoku, Slope) are complex but not blocked. Choose the closest reference above (multi-series, multi-buffer, or pairs) and follow the member ordering, provider selection, and test coverage rules in this file.

## Canonical stream hub member order

When implementing the stream hub indicators, we expect consistent member order in the source code files.  Only implement members that are appropriate for the indicator type and unique requirements of the indicator.

**In the `src/*.StreamHub.cs` implementation** members are ordered as follows:

```csharp
// Keep members in this general order (only include what you need):
// 1. Fields / constants
// 2. Constructors (validate inputs; call Reinitialize())
// 3. Public properties (e.g., LookbackPeriods)
// 4. Public methods (Add/Reset, etc.)
// 5. Protected overrides (ToIndicator, OnReset, etc.)
// 6. Private helpers
```

**In the `tests/*.StreamHub.Tests.cs` implementation** members are ordered as follows:

```csharp
// Recommended order (include what you need):
// 1. Constants/fields (lookback, shared test data)
// 2. Setup/fixtures (TestInitialize, test hubs/providers)
// 3. Happy path tests (standard processing)
// 4. Boundary/minimum periods tests
// 5. Bad/insufficient data tests
// 6. Reset/state tests (Reset(), reinitialize behavior)
// 7. Consistency tests (parity with Series/Buffer where applicable)
// 8. Performance placeholder (if present)
// 9. Private helpers
```

Minimal happy-path example:

```csharp
[TestMethod]
public void Standard()
{
    IQuoteProvider<IQuote> quotes = GetQuotesProvider();
    var sut = quotes.To{IndicatorName}Hub({params});
    foreach (IQuote q in Quotes)
        _ = sut.Add(q);

    // Compare to canonical Series results (see copilot-instructions.md)
    var series = Quotes.To{IndicatorName}({seriesParams});
    sut.Results.Should().BeEquivalentTo(series, o => o.WithStrictOrdering());
}
```

## Implementation requirements

### Core stream hub structure

Stream indicators implement the `IStreamHub<TResult>` interface, most commonly via a `ChainProvider<IReusable, {IndicatorName}Result>` base class for stateful processing.  Available base classes align with the I/O scenarios above.  Examples:

- `EmaHub : ChainProvider<IReusable, EmaResult>, IEma where TIn : IReusable` can consume chainable reusables types.
- `AdlHub : ChainProvider<IQuote, AdlResult>` can only consume quote types.

```csharp
/// <summary>
/// Streaming hub for {IndicatorName} calculations
/// </summary>
public class {IndicatorName}Hub
    : ChainProvider<IReusable, {IndicatorName}Result>, I{IndicatorName}
 {
    private readonly string hubName;

    internal {IndicatorName}Hub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        {IndicatorName}.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"{IndicatorUiid}({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /* other implementation details */

    /// <inheritdoc/>
    public override string ToString() => hubName;
}
```

### Extension method

If the hub supports chainable `IReusable` types:

```csharp
/// <summary>
/// Creates a {IndicatorName} streaming hub from a chain provider.
/// </summary>
public static {IndicatorName}Hub To{IndicatorName}Hub(
    this IChainProvider<IReusable> chainProvider,
    int lookbackPeriods = {defaultValue})
     => new(chainProvider, lookbackPeriods);
```

If the hub only supports `IQuote` input types (less common):

```csharp
/// <summary>
/// Creates a {IndicatorName} streaming hub from a quotes provider.
/// </summary>
public static {IndicatorName}Hub To{IndicatorName}Hub(
    this IQuoteProvider<IQuote> quoteProvider,
    int lookbackPeriods = {defaultValue})
     => new(quoteProvider, lookbackPeriods);
```

In both cases, `{defaultValue}` is only used for parity with Series `quotes.To{IndicatorName}()` extensions and may not always be implemented.

### Dual-stream hub structure (NEW)

For indicators requiring synchronized pairs of inputs (e.g., Correlation, Beta), use the `PairsProvider` base class:

```csharp
/// <summary>
/// Streaming hub for {IndicatorName} calculations between two synchronized series.
/// </summary>
public class {IndicatorName}Hub
    : PairsProvider<IReusable, {IndicatorName}Result>, I{IndicatorName}
 {
    private readonly string hubName;

    internal {IndicatorName}Hub(
        IChainProvider<IReusable> providerA,
        IChainProvider<IReusable> providerB,
        int lookbackPeriods) : base(providerA, providerB)
    {
        ArgumentNullException.ThrowIfNull(providerB);
        {IndicatorName}.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        hubName = $"{IndicatorUiid}({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override ({IndicatorName}Result result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Check if we have enough data in both caches
        if (HasSufficientData(i, LookbackPeriods))
        {
            // Validate timestamps match
            ValidateTimestampSync(i, item);

            // Extract data from both caches (ProviderCache and ProviderCacheB)
            // ... perform calculations ...

            // Use existing period calculation
            {IndicatorName}Result r = {IndicatorName}.PeriodCalculation(...);
            return (r, i);
        }
        else
        {
            // Not enough data yet
            {IndicatorName}Result r = new(Timestamp: item.Timestamp);
            return (r, i);
        }
    }
}
```

**Key utilities provided by `PairsProvider` base class**:

- `ProviderCache` - Cache reference for first provider (from `StreamHub` base)
- `ProviderCacheB` - Cache reference for second provider (from `PairsProvider`)
- `HasSufficientData(index, minimumPeriods)` - Checks both caches have sufficient data
- `ValidateTimestampSync(index, item)` - Validates timestamps match between both caches

**Dual-stream extension method pattern**:

```csharp
/// <summary>
/// Creates a {IndicatorName} hub from two synchronized chain providers.
/// Note: Both providers must be synchronized (same timestamps).
/// </summary>
public static {IndicatorName}Hub To{IndicatorName}Hub(
    this IChainProvider<IReusable> providerA,
    IChainProvider<IReusable> providerB,
    int lookbackPeriods)
     => new(providerA, providerB, lookbackPeriods);
```

**Important considerations for dual-stream hubs**:

1. Both input providers must be synchronized (matching timestamps)
2. `ValidateTimestampSync()` throws `InvalidQuotesException` on mismatch
3. `HasSufficientData()` ensures both caches have adequate data
4. Cannot be used with observer pattern (requires architectural changes for multi-provider synchronization)
5. Use `CorrelationHub` as reference implementation (see `src/a-d/Correlation/Correlation.StreamHub.cs`)
6. Test class must implement `ITestPairsObserver` interface for dual-stream validation

## Testing requirements

### Test coverage expectations

Stream indicator tests must cover:

1. **Stateful processing** - Verify state maintained correctly across quotes
2. **Real-time updates** - Test with live-like data streaming scenarios
3. **State reset** - Verify Reset() method clears all internal state
4. **Memory management** - Confirm no memory leaks with continuous processing
5. **Consistency validation** - Results must match series calculations
6. **Performance benchmarks** - Must support high-frequency data processing

### Test structure pattern

Use `StreamHubTestBase` as the base for all stream hub test classes, and implement the appropriate test interfaces based on your indicator's provider pattern and capabilities. The canonical reference for chain observer/provider test structure is `Ema.StreamHub.Tests.cs`.

#### Test interface selection guide

**Required for all StreamHub tests**: `StreamHubTestBase`

**Additional interfaces based on implementation:**

| Provider Base Class | Test Interfaces Required | Notes |
|---------------------|-------------------------|-------|
| `ChainProvider<TIn, TResult>` | `ITestChainProvider` | Always required for chainable indicators |
| `ChainProvider<TIn, TResult>` + supports chaining | `ITestChainProvider`, `ITestChainObserver` | Most indicators support both providing and observing |
| `QuoteProvider<TIn, TResult>` | `ITestQuoteObserver`, `ITestChainProvider` | Quote providers require quote observer and chain provider tests |
| `PairsProvider<TIn, TResult>` | `ITestPairsObserver` | Dual-stream indicators with synchronized inputs (must not also implement `ITestQuoteObserver`) |

Note: `ITestChainObserver` inherits `ITestQuoteObserver`. Do not redundantly implement both on the same class.

(Removed rollback-specific interface; provider history coverage is part of standard QuoteObserver tests.)

#### Interface selection examples

```csharp
// Single-input chainable indicator (most common)
[TestClass]
public class EmaHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    /* See Ema.StreamHub.Tests.cs for canonical pattern */
}

// Quote-only provider (cannot observe chains)
[TestClass]
public class RenkoHub : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    /* Quote provider pattern */
}

// Dual-stream indicator
[TestClass]
public class CorrelationHub : StreamHubTestBase, ITestPairsObserver
{
    /* Dual-stream pattern */
}

```

#### ITestQuoteObserver interface

The `ITestQuoteObserver` interface is required for all indicators that support direct quote provider observation. It replaces the need to override `QuoteObserver()` in the test class. Implement this interface and provide a `QuoteObserver()` method to test hub compatibility with quote providers. See `Ema.StreamHub.Tests.cs` for a reference implementation.

**When to use:**

- All indicators that can be observed directly from a quote provider (e.g., EMA, SMA, Renko, etc.)
- Not required for dual-stream (pairs) indicators

**Do not override `QuoteObserver()` in the test class; implement `ITestQuoteObserver` instead.**

### Comprehensive rollback validation (required)

Every StreamHub QuoteObserver test must cover these scenarios (EMA hub test is the canonical pattern):

- Prefill a small window of quotes before subscribing (warmup coverage)
- Stream quotes in order, including a few duplicate arrivals
- Insert a late historical quote (Insert) and verify recalculation parity
- Remove a historical quote (Remove) and verify parity against a revised series
- Compare Results to Series with strict ordering
- Clean up with Unsubscribe() and EndTransmission()

These scenarios replace the need for a separate rollback-specific interface.

### Provider history (Insert/Remove) testing

Provider history mutations are required and are part of the “Comprehensive rollback validation” section above (see EMA hub tests for the canonical pattern). Use `ProviderHistoryTesting()` in `StreamHubTestBase` as needed for indicator-specific logic.

### Performance benchmarking

All stream indicators must include performance tests in the `tools/performance/Perf.Stream.cs` project file:

```csharp
[Benchmark] public object {IndicatorName}Hub() => quoteHub.To{IndicatorName}Hub({params}).Results;
```

Example:

```csharp
[Benchmark]
public object EmaHub() => quoteHub.ToEmaHub(14).Results;
```

**Performance expectations**:

- Stream processing should handle high-frequency updates (1000+ quotes/second)
- Memory usage should remain bounded during continuous operation
- State updates should be O(1) complexity when possible

See also: Common indicator requirements and Series-as-canonical policy in `.github/copilot-instructions.md`.

## Quality standards

### Performance optimization

**Target**: StreamHub implementations should be ≤1.5x slower than Series (batch) implementations.

#### ❌ Anti-Pattern 1: O(n²) Complexity - Recalculating from Scratch

**WRONG**: Calling Series methods on every new tick creates O(n²) complexity (see RSI hub - 391x slower before fix):

```csharp
// ❌ WRONG - O(n²) recalculation
protected override (RsiResult result, int index) ToIndicator(IReusable item, int? indexHint)
{
    int i = indexHint ?? ProviderCache.IndexOf(item, true);
    
    if (i >= LookbackPeriods)
    {
        // Building subset and recalculating ENTIRE history every tick
        List<IReusable> subset = [];
        for (int k = 0; k <= i; k++)
        {
            subset.Add(ProviderCache[k]);
        }
        
        // O(n) calculation on O(n) history = O(n²)
        IReadOnlyList<RsiResult> seriesResults = subset.ToRsi(LookbackPeriods);
        rsi = seriesResults[seriesResults.Count - 1]?.Rsi;
    }
    
    return (new RsiResult(item.Timestamp, rsi), i);
}
```

**Impact**: For 1000 quotes: 1,000 × 1,000 = 1,000,000 operations vs. 1,000 operations (1000x slower!)

**✅ CORRECT**: Use incremental state management with O(1) updates per tick:

```csharp
// ✅ CORRECT - Maintain state variables for O(1) updates
public class RsiHub : ChainProvider<IReusable, RsiResult>
{
    // State variables for incremental calculation
    private double _prevValue = double.NaN;
    private double _avgGain = double.NaN;
    private double _avgLoss = double.NaN;
    private int _warmupCount = 0;
    
    protected override (RsiResult result, int index) ToIndicator(IReusable item, int? indexHint)
    {
        double currentValue = item.Value;
        double? rsi = null;
        
        // Calculate gain/loss incrementally - O(1)!
        double gain = currentValue > _prevValue ? currentValue - _prevValue : 0;
        double loss = currentValue < _prevValue ? _prevValue - currentValue : 0;
        
        if (_warmupCount >= LookbackPeriods)
        {
            // Wilder's smoothing - O(1) update
            _avgGain = ((_avgGain * (LookbackPeriods - 1)) + gain) / LookbackPeriods;
            _avgLoss = ((_avgLoss * (LookbackPeriods - 1)) + loss) / LookbackPeriods;
            
            rsi = _avgLoss > 0 ? 100 - (100 / (1 + (_avgGain / _avgLoss))) : 100;
        }
        
        _prevValue = currentValue;
        _warmupCount++;
        
        return (new RsiResult(item.Timestamp, rsi), indexHint ?? 0);
    }
}
```

#### ❌ Anti-Pattern 2: O(n) Window Operations - Linear Scans

**WRONG**: Scanning entire window for max/min on every tick (see Donchian hub - 20x slower before fix):

```csharp
// ❌ WRONG - O(n) linear scan every tick
protected override (DonchianResult result, int index) ToIndicator(IQuote item, int? indexHint)
{
    int i = indexHint ?? ProviderCache.IndexOf(item, true);
    
    if (i < LookbackPeriods)
        return (new DonchianResult(item.Timestamp), i);
    
    // Linear scan for max/min EVERY tick - O(n) per tick!
    decimal highHigh = decimal.MinValue;
    decimal lowLow = decimal.MaxValue;
    
    for (int p = i - LookbackPeriods; p < i; p++)
    {
        IQuote quote = ProviderCache[p];
        if (quote.High > highHigh) highHigh = quote.High;
        if (quote.Low < lowLow) lowLow = quote.Low;
    }
    
    return (new DonchianResult(item.Timestamp, highHigh, lowLow, ...), i);
}
```

**Impact**: For 20-period lookback with 1000 quotes: 1,000 × 20 = 20,000 operations vs. 1,000 operations (20x slower!)

**✅ CORRECT**: Use monotonic deque for O(1) amortized max/min tracking:

```csharp
// ✅ CORRECT - Use RollingWindowMax/Min utilities (from src/_common/StreamHub/)
public class DonchianHub : StreamHub<IQuote, DonchianResult>
{
    private readonly RollingWindowMax<decimal> _highWindow;
    private readonly RollingWindowMin<decimal> _lowWindow;
    
    internal DonchianHub(IQuoteProvider<IQuote> provider, int lookbackPeriods) : base(provider)
    {
        LookbackPeriods = lookbackPeriods;
        _highWindow = new RollingWindowMax<decimal>(lookbackPeriods);
        _lowWindow = new RollingWindowMin<decimal>(lookbackPeriods);
        Reinitialize();
    }
    
    protected override (DonchianResult result, int index) ToIndicator(IQuote item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);
        
        // O(1) amortized add operation
        _highWindow.Add(item.High);
        _lowWindow.Add(item.Low);
        
        if (i < LookbackPeriods)
            return (new DonchianResult(item.Timestamp), i);
        
        // O(1) max/min retrieval!
        decimal highHigh = _highWindow.Max;
        decimal lowLow = _lowWindow.Min;
        
        return (new DonchianResult(item.Timestamp, highHigh, lowLow, ...), i);
    }
}
```

#### ✅ Pattern: Wilder's Smoothing Helper

**Use Case**: RSI, CMO, ATR, ADX, SMMA, Alligator, Stochastic smoothing

**Formula**: `smoothedValue = ((prevSmoothed × (period - 1)) + currentValue) / period`

**Helper Method** (to be added to `src/_common/StreamHub/Smoothing.cs`):

```csharp
public static class Smoothing
{
    /// <summary>
    /// Applies Wilder's smoothing (modified moving average) to a value.
    /// </summary>
    public static double WilderSmoothing(double prevSmoothed, double currentValue, int period)
        => ((prevSmoothed * (period - 1)) + currentValue) / period;
}
```

**Usage Example**:

```csharp
// Instead of:
_avgGain = ((_avgGain * (LookbackPeriods - 1)) + gain) / LookbackPeriods;
_avgLoss = ((_avgLoss * (LookbackPeriods - 1)) + loss) / LookbackPeriods;

// Use:
_avgGain = Smoothing.WilderSmoothing(_avgGain, gain, LookbackPeriods);
_avgLoss = Smoothing.WilderSmoothing(_avgLoss, loss, LookbackPeriods);
```

### State management and rollback handling

#### Core state management principles

- Maintain minimal internal state necessary for calculations
- Implement efficient state updates (avoid recalculating from scratch)
- Handle edge cases in state transitions properly
- Ensure state consistency across all operations

#### RollbackState override pattern

**When to override `RollbackState`**: Any StreamHub that maintains stateful fields (beyond simple cache lookups) MUST override `RollbackState(DateTime timestamp)` to handle Insert/Remove operations and explicit Rebuild() calls.

**Common scenarios requiring RollbackState**:
1. Rolling windows (RollingWindowMax/Min) - must be rebuilt from cache
2. Buffered historical values (e.g., raw K buffer in Stoch) - must be prefilled
3. Running totals or averages (EMA state, Wilder's smoothing) - must be recalculated from cache
4. Previous value tracking (_prevValue, _prevHigh, etc.) - must be restored from cache

**❌ WRONG - Inline rebuild detection in ToIndicator**:

```csharp
// ❌ ANTI-PATTERN - Don't do this!
public class BadHub : StreamHub<IQuote, BadResult>
{
    private int _lastProcessedIndex = -1;
    private readonly RollingWindowMax<double> _window;
    
    protected override (BadResult result, int index) ToIndicator(IQuote item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);
        
        // ❌ WRONG: Detecting and handling rollback in ToIndicator
        bool needsRebuild = (i != _lastProcessedIndex + 1) && (_lastProcessedIndex != -1);
        if (needsRebuild)
        {
            _window.Clear();
            // Rebuild logic inline...
        }
        
        _lastProcessedIndex = i;
        _window.Add(item.Value);
        // ... rest of calculation
    }
}
```

**✅ CORRECT - Override RollbackState**:

```csharp
// ✅ CORRECT: Separation of concerns
public class GoodHub : StreamHub<IQuote, GoodResult>
{
    private readonly RollingWindowMax<double> _window;
    
    protected override (GoodResult result, int index) ToIndicator(IQuote item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);
        
        // Normal incremental processing only
        _window.Add(item.Value);
        
        if (i < LookbackPeriods)
            return (new GoodResult(item.Timestamp), i);
            
        double max = _window.Max;
        return (new GoodResult(item.Timestamp, max), i);
    }
    
    /// <summary>
    /// Restores the rolling window state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear state
        _window.Clear();
        
        // Rebuild from ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0) return;
        
        int targetIndex = index - 1;
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);
        
        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            _window.Add(quote.Value);
        }
    }
}
```

**Reference implementations**:
- Simple rolling window: `ChandelierHub.RollbackState` (high/low windows only)
- Complex with buffering: `StochHub.RollbackState` (windows + rawK buffer prefill)
- Running state variables: `AdxHub.RollbackState` (Wilder's smoothing state)
- Previous value tracking: `EmaHub.RollbackState` (prior EMA value restoration)

**Key benefits of RollbackState pattern**:
1. **Separation of concerns** - `ToIndicator` handles normal streaming, `RollbackState` handles cache rebuilds
2. **Framework integration** - StreamHub base class automatically calls `RollbackState` when needed
3. **Cleaner hot path** - No conditional logic in performance-critical `ToIndicator` method
4. **Consistent with patterns** - Follows established patterns in AdxHub, AtrStopHub, StochRsiHub, etc.

### Real-time considerations

- Optimize for low-latency processing
- Minimize garbage collection pressure
- Use efficient data structures for rolling calculations
- Consider numerical stability in continuous operations

### Thread safety

- Stream hubs should be thread-safe for single-writer scenarios
- Document any multi-threading limitations
- Use appropriate synchronization when necessary

> [!NOTE]
> Contributor-facing checklist: see `specs/001-develop-streaming-indicators/checklists/stream-hub-tests.md`.

## Integration patterns

### Chaining with other indicators

Stream indicators should support chaining with other stream indicators:

```csharp
// Example: EMA of RSI streaming
var rsiStream = quotes.ToRsiHub(14);
var emaOfRsiStream = new EmaHub(20);

foreach (var quote in liveQuotes)
{
    var rsiResult = rsiStream.Add(quote);
    if (rsiResult.Rsi.HasValue)
    {
        var pseudoQuote = new Quote 
        { 
            Date = quote.Date, 
            Close = rsiResult.Rsi.Value 
        };
        var emaResult = emaOfRsiStream.Add(pseudoQuote);
    }
}
```

---
Last updated: October 24, 2025
