---
description: Buffer-style indicator development and testing guidelines
applyTo: 'src/**/*.BufferList.cs,tests/**/*.BufferList.Tests.cs'
---

# Buffer indicator development guidelines

These instructions apply to buffer-style indicators that process data incrementally with efficient buffering mechanisms.

## Code completion checklist

When implementing or updating an indicator, you must complete:

- [ ] Source code: `src/**/{IndicatorName}.BufferList.cs` file exists and adheres to these instructions
  - [ ] Inherits `BufferList<TResult>` and implements the correct increment interface (`IIncrementFromChain` | `IIncrementFromQuote` | `IIncrementFromPairs`)
  - [ ] Provides two constructors: primary parameters only, and parameters + `IReadOnlyList<IQuote> quotes` (chained via `: this(... ) => Add(quotes);`)
  - [ ] Implements required Add overloads for the chosen interface and uses `BufferListUtilities.Update()` or `UpdateWithDequeue()`
  - [ ] `Clear()` resets results and all internal buffers/caches
  - [ ] Member order matches conventions: Fields → Constructors → Properties → Methods → Extension
- [ ] Catalog: `src/**/{IndicatorName}.Catalog.cs` exists, is accurate, and registered in `src\_common\Catalog\Catalog.Listings.cs` (`PopulateCatalog`)
- [ ] Unit testing: `tests/indicators/**/{IndicatorName}.BufferList.Tests.cs` file exists and adheres to these instructions
  - [ ] Inherits `BufferListTestBase` (not `TestBase`) and implements the correct test interface(s)
  - [ ] Implements the 5 required tests from the base and covers reusable/quotes/pairs paths as applicable
  - [ ] Verifies equivalence with the corresponding Series results for the same inputs
- [ ] Common items: Complete regression, performance, docs, and migration per `.github/copilot-instructions.md` (Common indicator requirements)

## Universal buffer utilities

All buffer indicators must use the common `BufferListUtilities` extension methods from `src/_common/BufferLists/BufferListUtilities.cs` for consistent buffer management.

## File naming conventions

- **Implementation**: `{IndicatorName}.BufferList.cs`
- **Tests**: `{IndicatorName}.BufferList.Tests.cs`
- **User docs**: `docs/_indicators/{IndicatorName}.md`

## Interface selection

BufferList implementations must implement ONE of three interfaces based on the related buffer style indicator:

- `IIncrementFromChain` - most common, for chainable indicators (SMA, EMA, RSI, MACD)
- `IIncrementFromQuote` - requires multiple OHLC values (VWMA, Stoch, VWAP)
- `IIncrementFromPairs` - dual-input indicators (Correlation, Beta)

### Test interfaces

All buffer test classes inherit from `BufferListTestBase` and implement the corresponding test interface:

- `ITestChainBufferList` when implementing `IIncrementFromChain`
- `ITestQuoteBufferList` when implementing `IIncrementFromQuote`
- `ITestPairsBufferList` when implementing `IIncrementFromPairs`
- `ITestCustomBufferListCache` when using custom non-`Queue<T>` caches (for example, `List<T>`)

## Implementation requirements

### Base class pattern

**ALL buffer implementations MUST inherit from `BufferList<TResult>`** and follow this structure:

```csharp
public class {IndicatorName}List : BufferList<{IndicatorName}Result>, I{IncrementInterface}, I{IndicatorName}
{
    private readonly Queue<double> _buffer;
    
    public {IndicatorName}List(int lookbackPeriods)
    {
        {IndicatorName}.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _buffer = new Queue<double>(lookbackPeriods);
    }

    public {IndicatorName}List(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods) => Add(quotes);

    public int LookbackPeriods { get; init; }

    public void Add(DateTime timestamp, double value)
    {
        _buffer.Update(LookbackPeriods, value);
        double? result = CalculateIndicator();
        AddInternal(new {IndicatorName}Result(timestamp, result));
    }

    public override void Clear()
    {
        base.Clear();
        _buffer.Clear();
    }
}
```

### Constructor requirements

- **Two constructors required**: Standard parameters only, and parameters + `IReadOnlyList<IQuote> quotes`
- **Use constructor chaining**: `: this(parameters) => Add(quotes);`
- **Expression-bodied quotes constructor**: `=> Add(quotes);`

### Member ordering

1. Fields
2. Constructors
3. Properties
4. Methods: `Add(DateTime, double)`, `Add(IReusable)`, `Add(IReadOnlyList<>)`, `Clear()`
5. Extension method

## Interface selection guidelines

### `IIncrementFromChain` - Chainable indicators

**Use for**: Single-value indicators that can chain (SMA, EMA, RSI, MACD)

**Required methods**:

- `Add(DateTime timestamp, double value)`
- `Add(IReusable value)`
- `Add(IReadOnlyList<IReusable> values)`

**Critical rules**:

- ✅ Constructor accepts `IReadOnlyList<IReusable>` (NOT `IQuote`)
- ✅ Extension uses generic constraint with `IReusable` (NOT with `IQuote`)
- ❌ No `Add(IQuote)` methods
- ✅ Most indicators use `value.Value` directly
- ✅ OHLC indicators use `value.Hl2OrValue()` or `value.QuotePartOrValue(CandlePart.HL2)`

### `IIncrementFromQuote` - Multi-value OHLC indicators

**Use for**: Indicators requiring multiple OHLC values (VWMA, Stoch, VWAP)

**Required methods**:

- `Add(IQuote quote)`
- `Add(IReadOnlyList<IQuote> quotes)`

### `IIncrementFromPairs` - Dual-input indicators

**Use for**: Indicators requiring two synchronized series (Correlation, Beta)

**Required methods**:

- `Add(DateTime timestamp, double valueA, double valueB)`
- `Add(IReusable valueA, IReusable valueB)`
- `Add(IReadOnlyList<IReusable> valuesA, IReadOnlyList<IReusable> valuesB)`

**Critical rules**:

- ✅ Constructor accepts two `IReadOnlyList<IReusable>` parameters
- ✅ Must validate timestamp matching between pairs
- ✅ Must validate equal list counts

## Buffer management patterns

### Standard buffer operations

```csharp
// Standard rolling buffer
_buffer.Update(LookbackPeriods, value);

// With dequeue tracking for sums
double? dequeued = _buffer.UpdateWithDequeue(LookbackPeriods, value);
if (dequeued.HasValue) _sum = _sum - dequeued.Value + value;
else _sum += value;
```

### Buffer state patterns

**Prefer tuples for internal state**:

```csharp
private readonly Queue<(double High, double Low, double Close)> _buffer;

// Use named tuple fields
_buffer.Update(2, (quote.High, quote.Low, quote.Close));
```

**Avoid custom structs** for internal-only buffer state.

## Testing requirements

### Critical: Test base class inheritance

**IMPORTANT**: All BufferList test classes MUST inherit from `BufferListTestBase`, NOT from `TestBase` directly.

- ✅ Correct: `public class MyIndicatorTests : BufferListTestBase`
- ❌ Incorrect: `public class MyIndicatorTests : TestBase`

**Consequences of incorrect inheritance**:

- Missing abstract method implementations required by `BufferListTestBase`
- Compilation errors for `AddQuote_IncrementsResults()`, `AddQuotesBatch_IncrementsResults()`, `QuotesCtor_OnInstantiation_IncrementsResults()`, `Clear_WithState_ResetsState()`, and `PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()`
- Tests won't validate essential BufferList behaviors

The `BufferListTestBase` abstract class defines 5 required test methods that ensure BufferList implementations work correctly:

1. `AddQuote_IncrementsResults()` - Individual quote addition
2. `AddQuotesBatch_IncrementsResults()` - Batch quote addition via collection initializer
3. `QuotesCtor_OnInstantiation_IncrementsResults()` - Constructor with quotes parameter
4. `Clear_WithState_ResetsState()` - State reset functionality
5. `PruneList_OverMaxListSize_AutoAdjustsListAndBuffers()` - List size management

### Test structure

```csharp
[TestClass]
public class {IndicatorName}BufferListTests : BufferListTestBase, ITestChainBufferList
{
    private const int lookbackPeriods = 14;
    private static readonly IReadOnlyList<IReusable> reusables = Quotes.Cast<IReusable>().ToList();
    private static readonly IReadOnlyList<{IndicatorName}Result> series = Quotes.To{IndicatorName}(lookbackPeriods);

    [TestMethod]
    public void AddDateAndValue_IncrementsResults()
    {
        {IndicatorName}List sut = new(lookbackPeriods);
        foreach (IReusable item in reusables)
            sut.Add(item.Timestamp, item.Value);
        
        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    // Additional required test methods...
}
```

See also: Common indicator requirements and Series-as-canonical policy in `.github/copilot-instructions.md`.

## Quality standards

### Universal buffer utility usage

- **Always use `BufferListUtilities` extension methods** - Never implement custom buffer management logic
- **Choose the right extension method**:
  - Use `buffer.Update()` for standard rolling buffer scenarios
  - Use `buffer.UpdateWithDequeue()` when tracking removed values for sums or calculations
- **Type safety** - Extension methods are generic and work with any data type
- **Null safety** - Extension methods include proper argument validation

## Common anti-patterns to avoid

### ❌ Manual buffer management

```csharp
// DON'T: Implement custom buffer logic
if (_buffer.Count == capacity)
{
    _buffer.Dequeue();
}
_buffer.Enqueue(value);
```

```csharp
// DO: Use extension methods for buffer management
_buffer.Update(capacity, value);
```

### ❌ Using custom structs for internal buffer state

```csharp
// DON'T: Define custom struct for internal-only buffer state
private readonly struct BufferState(double high, double low, double close) : IEquatable<BufferState>
{
    // Unnecessary boilerplate for internal use
}

// DO: Use named tuples for internal buffer state
private readonly Queue<(double High, double Low, double Close)> _buffer;
```

## Reference examples

Study these exemplary buffer indicators that demonstrate proper use of universal buffer utilities:

- **WMA**: `src/s-z/Wma/Wma.BufferList.cs` - Standard buffer management with WMA calculation
- **SMA**: `src/s-z/Sma/Sma.BufferList.cs` - Simple buffer management with sum calculation
- **EMA**: `src/e-k/Ema/Ema.BufferList.cs` - Buffer management with dequeue tracking for running sum
- **HMA**: `src/e-k/Hma/Hma.BufferList.cs` - Multi-buffer management for complex calculations
- **ADX**: `src/a-d/Adx/Adx.BufferList.cs` - Complex object buffer management
- **MAMA**: `src/m-r/Mama/Mama.BufferList.cs` - List-based state with separate state array pruning
- **Volatility Stop (historical repaint)**: `src/s-z/VolatilityStop/VolatilityStop.BufferList.cs` — Demonstrates correct handling of trailing stop recalculation and repainting of historical values when new extrema arrive. Use this as the canonical pattern for indicators that legitimately revise past outputs based on future bars.
- **Dual-input (pairs)**:
  - `src/a-d/Correlation/Correlation.BufferList.cs` — Pairwise rolling statistics with timestamp alignment requirements
  - `src/a-d/Beta/Beta.BufferList.cs` — Dual-series regression/risk example built on the same pairs pattern

When implementing other complex or previously deferred indicators (for example: Fractal, HtTrendline, Hurst, Ichimoku, Slope), prefer adapting from the closest matching reference above rather than writing bespoke buffer logic. In particular:

- For multi-buffer pipelines, start from HMA.
- For complex state objects, start from ADX.
- For legitimate historical repaint behavior, start from Volatility Stop.
- For synchronized dual-series inputs, start from Correlation/Beta.

> [!NOTE]
> For PRs, use the dev-facing Code completion checklist above, and the contributor-facing checklists in SpecKit:
>
> - BufferList tests: `.specify/specs/001-develop-streaming-indicators/checklists/buffer-list-tests.md`
> - StreamHub tests: `.specify/specs/001-develop-streaming-indicators/checklists/stream-hub-tests.md`

---
Last updated: October 28, 2025
