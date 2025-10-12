---
applyTo: "src/**/*.StreamHub.cs,tests/**/*.StreamHub.Tests.cs"
description: "Stream-style indicator development and testing guidelines"
---

# Stream indicator development guidelines

These instructions apply to stream-style indicators that support real-time data processing with stateful operations. Stream indicators maintain internal state and can process individual quotes as they arrive.

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
  - [ ] Inherits `StreamHubTestBase` and adds `ITestChainObserver`/`ITestChainProvider` as appropriate
  - [ ] Verifies stateful processing, reset behavior, and consistency with Series results
  - [ ] For dual-stream hubs: covers timestamp sync validation and sufficient data checks
- [ ] Common items: Complete regression, performance, docs, and migration per `.github/copilot-instructions.md` (Common indicator requirements)

## Stream Hub I/O Scenarios

The codebase implements several types of stream hub I/O patterns:

1. **IQuote → IReusable** (e.g., EMA, SMA): Takes quote input, produces single reusable value output
   - Uses `IChainProvider<TIn>` and extends `ChainProvider<TIn, TResult>`
   - Generic constraint: `where TIn : IReusable`
2. **IQuote → ISeries** (e.g., Alligator, AtrStop): Takes quote input, produces multi-value series output  
   - Uses `IChainProvider<TIn>` and extends `ChainProvider<TIn, TResult>`
   - Generic constraint: `where TIn : IReusable`
3. **IReusable → IReusable** (e.g., chained indicators): Takes reusable input, produces reusable output
   - Uses `IChainProvider<TIn>` and extends `ChainProvider<TIn, TResult>`
   - Generic constraint: `where TIn : IReusable`
4. **IQuote → IQuote** (e.g., Renko, Quote converters): Takes quote input, produces modified quote output
   - Uses `IQuoteProvider<TIn>` and extends `QuoteProvider<TIn, TResult>`
   - Generic constraint: `where TIn : IQuote`
5. **IQuote → VolumeWeighted** (e.g., VWMA): Takes quote input, requires both price and volume data
   - Uses `IQuoteProvider<TIn>` and extends `QuoteProvider<TIn, TResult>`
   - Generic constraint: `where TIn : IQuote`
6. **Dual IReusable → IReusable** (e.g., Correlation, Beta): Takes two synchronized reusable inputs, produces reusable output
   - Uses `IPairsProvider<TIn>` and extends `PairsProvider<TIn, TResult>`
   - Generic constraint: `where TIn : IReusable`
   - **NEW**: Added for dual-stream indicators requiring synchronized pair inputs

**Provider Selection Guidelines**:

- Use `IQuoteProvider<TIn>` and `QuoteProvider<TIn, TResult>` when the indicator requires multiple quote properties (e.g., OHLCV data)
- Use `IChainProvider<TIn>` and `ChainProvider<TIn, TResult>` when the indicator can work with single reusable values
- Use `IPairsProvider<TIn>` and `PairsProvider<TIn, TResult>` when the indicator requires synchronized dual inputs (e.g., Correlation, Beta)

Note: IQuote → QuotePart selectors exist but are rarely used for new indicators.

## File naming conventions

Stream indicators should follow these naming patterns:

- **Implementation**: `{IndicatorName}.StreamHub.cs`
- **Tests**: `{IndicatorName}.StreamHub.Tests.cs`

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

Stream indicators implement the `IStreamHub<TResult>` interface, most commonly via a `ChainProvider<TIn, {IndicatorName}Result>` base class for stateful processing.  Available base classes align with the I/O scenarios above.  Examples:

- `EmaHub<TIn> : ChainProvider<TIn, EmaResult>, IEma where TIn : IReusable` can consume chainable reusables types.
- `AdlHub<TIn> : ChainProvider<TIn, AdlResult> where TIn : IQuote` can only consume quote types.

```csharp
/// <summary>
/// Streaming hub for {IndicatorName} calculations
/// </summary>
public class {IndicatorName}Hub<TIn>
    : ChainProvider<TIn, {IndicatorName}Result>, I{IndicatorName}
    where TIn : IReusable
{
    private readonly string hubName;

    internal {IndicatorName}Hub(
        IChainProvider<TIn> provider,
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
public static {IndicatorName}Hub<TIn> To{IndicatorName}Hub<TIn>(
    this IChainProvider<TIn> chainProvider,
    int lookbackPeriods = {defaultValue})
    where TIn : IReusable
    => new(chainProvider, lookbackPeriods);
```

If the hub only supports `IQuote` input types (less common):

```csharp
/// <summary>
/// Creates a {IndicatorName} streaming hub from a quotes provider.
/// </summary>
public static {IndicatorName}Hub<TIn> To{IndicatorName}Hub<TIn>(
    this IQuoteProvider<TIn> quoteProvider,
    int lookbackPeriods = {defaultValue})
    where TIn : IQuote
    => new(quoteProvider, lookbackPeriods);
```

In both cases, `{defaultValue}` is only used for parity with Series `quotes.To{IndicatorName}()` extensions and may not always be implemented.

### Dual-stream hub structure (NEW)

For indicators requiring synchronized pairs of inputs (e.g., Correlation, Beta), use the `PairsProvider` base class:

```csharp
/// <summary>
/// Streaming hub for {IndicatorName} calculations between two synchronized series.
/// </summary>
public class {IndicatorName}Hub<TIn>
    : PairsProvider<TIn, {IndicatorName}Result>, I{IndicatorName}
    where TIn : IReusable
{
    private readonly string hubName;

    internal {IndicatorName}Hub(
        IChainProvider<TIn> providerA,
        IChainProvider<TIn> providerB,
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
        ToIndicator(TIn item, int? indexHint)
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
public static {IndicatorName}Hub<TIn> To{IndicatorName}Hub<TIn>(
    this IChainProvider<TIn> providerA,
    IChainProvider<TIn> providerB,
    int lookbackPeriods)
    where TIn : IReusable
    => new(providerA, providerB, lookbackPeriods);
```

**Important considerations for dual-stream hubs**:

1. Both input providers must be synchronized (matching timestamps)
2. `ValidateTimestampSync()` throws `InvalidQuotesException` on mismatch
3. `HasSufficientData()` ensures both caches have adequate data
4. Cannot be used with observer pattern (requires architectural changes for multi-provider synchronization)
5. Use `CorrelationHub` as reference implementation (see `src/a-d/Correlation/Correlation.StreamHub.cs`)

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

Use the `StreamHubTestBase` as the base for all stream hub test classes, and add `ITestChainObserver` and `ITestChainProvider` conditionally if they are of those types.

```csharp
[TestClass]
public class {IndicatorName}StreamHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    /* See `Ema.StreamHub.Tests.cs` as a reference implementation */
}
```

### Performance benchmarking

All stream indicators must include performance tests in the `tools/performance/Perf.Stream.cs` project file:

```csharp
[Benchmark]
public object {IndicatorName}Hub() => quoteHub.To{IndicatorName}Hub({params}).Results;
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

### State management

- Maintain minimal internal state necessary for calculations
- Implement efficient state updates (avoid recalculating from scratch)
- Handle edge cases in state transitions properly
- Ensure state consistency across all operations

### Real-time considerations

- Optimize for low-latency processing
- Minimize garbage collection pressure
- Use efficient data structures for rolling calculations
- Consider numerical stability in continuous operations

### Thread safety

- Stream hubs should be thread-safe for single-writer scenarios
- Document any multi-threading limitations
- Use appropriate synchronization when necessary

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

## Reference examples

Study these exemplary stream indicators:

- **EMA**: `src/e-k/Ema/Ema.StreamHub.cs`
- **SMA**: `src/s-z/Sma/Sma.StreamHub.cs`
- **ATRSTOP**: `src/a-d/AtrStop/AtrStop.StreamHub.cs`
- **ALLIGATOR**: `src/a-d/Alligator/Alligator.StreamHub.cs`

---
Last updated: October 12, 2025
