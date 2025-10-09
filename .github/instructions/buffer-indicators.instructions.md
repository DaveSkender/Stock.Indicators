---
applyTo: "src/**/*.BufferList.cs,tests/**/*.BufferList.Tests.cs"
description: "Buffer-style indicator development and testing guidelines"
---

# Buffer indicator development guidelines

These instructions apply to buffer-style indicators that process data incrementally with efficient buffering mechanisms. Buffer indicators optimize memory usage and processing for scenarios involving incremental data updates.

## Universal buffer utilities

All buffer indicators must use the common `BufferUtilities` extension methods from `src/_common/BufferLists/BufferUtilities.cs` for consistent buffer management across the codebase.

## File naming conventions

Buffer indicators should follow these naming patterns:

- **Implementation**: `{IndicatorName}.BufferList.cs`
- **Tests**: `{IndicatorName}.BufferList.Tests.cs`

## Implementation requirements

### Core structure

Buffer indicators inherit from the `BufferList<TResult>` base class and implement appropriate interfaces (`IBufferList` or `IBufferReusable`):

```csharp
/// <summary>
/// Buffer implementation for {IndicatorName} indicator
/// </summary>
public class {IndicatorName}List : BufferList<{IndicatorName}Result>, IBufferReusable, I{IndicatorName}
{
    private readonly Queue<double> _buffer;
    
    public {IndicatorName}List(
        int lookbackPeriods
    )
    {
        {IndicatorName}.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<double>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public {IndicatorName}List(
        int lookbackPeriods,
        IReadOnlyList<IQuote> quotes
    )
        : this(lookbackPeriods)
        => Add(quotes);

    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Use universal buffer extension method for consistent buffer management
        _buffer.Update(LookbackPeriods, value);
        
        // Calculate result using buffer data
        double? result = CalculateIndicator();
        
        // AddInternal automatically prunes the list when MaxListSize is exceeded
        AddInternal(new {IndicatorName}Result(timestamp, result));
    }

    /// <inheritdoc />
    public override void Clear()
    {
        ClearInternal();
        _buffer.Clear();
    }
    
    private double? CalculateIndicator()
    {
        // Implement indicator-specific calculation logic
        // Return null if insufficient data
        if (_buffer.Count < LookbackPeriods)
        {
            return null;
        }
        
        // Perform calculation using buffer data
        return /* calculated value */;
    }
}
```

**Base Class Pattern**:

- **ALL buffer list implementations MUST inherit from `BufferList<TResult>`** instead of `List<TResult>`
- The base class provides:
  - `AddInternal(TResult item)` - Protected method to add items to the internal list and automatically prune when `MaxListSize` is exceeded
  - `ClearInternal()` - Protected method to clear the internal list
  - `RemoveAtInternal(int index)` - Protected method to remove specific items (used by the base pruner)
  - `PruneList()` - Virtual hook for custom pruning logic (rarely needed; base implementation trims overflow only)
  - `MaxListSize` property - Configurable limit (default is 90% of `int.MaxValue`)
  - `abstract void Clear()` - Derived classes must override to reset both list and buffers
  - `ICollection<TResult>` and `IReadOnlyList<TResult>` interfaces
  - Read-only indexer and Count property
  - Blocks problematic operations (`ICollection<T>.Add()` and `ICollection<T>.Remove()`)
- **Auto-pruning is automatic**: Always call `AddInternal()` (never `base.Add()`). Do **not** call `PruneList()` from indicator code—the base class removes only the overflow entries after each addition.
- **Do not shadow base members**: Never add a `DefaultMaxListSize` constant or re-declare the `MaxListSize` property in derived classes, and never mutate `_internalList` directly. Configure `MaxListSize` via object initializers when a smaller cap is necessary (for example, tests).
- **Custom state pruning**: Indicators that maintain additional `List<T>` buffers (for example, `MamaList`) must prune those lists themselves while keeping at least the minimum lookback length. `Queue<T>` buffers stay bounded automatically through the extension helpers and do not need manual trimming.

**Constructor Pattern**:

- **ALL buffer list implementations MUST provide two constructors**:
  1. Standard constructor with parameters only
  2. Constructor with parameters PLUS `IReadOnlyList<IQuote> quotes` as the LAST parameter
- **Use constructor chaining** with `: this(...)` to avoid duplicating initialization logic
- **Use expression-bodied syntax** `=> Add(quotes);` for the quotes constructor body
- For parameterless buffer lists (like AdlList, ObvList, TrList):
  - Primary: `ClassName()`
  - Quotes: `ClassName(IReadOnlyList<IQuote> quotes) : this() => Add(quotes);`
- For buffer lists with parameters:
  - Primary: `SmaList(int lookbackPeriods)`
  - Quotes: `SmaList(int lookbackPeriods, IReadOnlyList<IQuote> quotes) : this(lookbackPeriods) => Add(quotes);`
- For buffer lists with multiple parameters:
  - Primary: `AlmaList(int lookbackPeriods, double offset = 0.85, double sigma = 6)`
  - Quotes: `AlmaList(int lookbackPeriods, double offset, double sigma, IReadOnlyList<IQuote> quotes) : this(lookbackPeriods, offset, sigma) => Add(quotes);`

**Interface Selection Guidelines**:

- Use `IBufferReusable` when the indicator's static series can accept `IReusable` values (single values like SMA, EMA)
  - Note: `IBufferReusable` extends `IBufferList`, so only `IBufferReusable` needs to be declared
- Use only `IBufferList` when the indicator's static series requires `IQuote` (multiple values like VWMA needs price+volume, ADX needs OHLC)
- Match the interface pattern to what the static series implementation supports

> **Note**: The current codebase uses `Queue<T>` for efficient FIFO buffering operations. `Queue<T>` provides O(1) enqueue/dequeue operations and is well-suited for sliding window calculations where you need to remove the oldest value when adding a new one.

### Extension method

```csharp
/// <summary>
/// Creates a buffer list for {IndicatorName} calculations
/// </summary>
public static {IndicatorName}List To{IndicatorName}List<TQuote>(
    this IReadOnlyList<TQuote> quotes,
    int lookbackPeriods = {defaultValue})
    where TQuote : IQuote
    => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
```

## Testing requirements

### Critical: Test base class inheritance

**IMPORTANT**: All BufferList test classes MUST inherit from `BufferListTestBase`, NOT from `TestBase` directly.

- ✅ Correct: `public class MyIndicatorTests : BufferListTestBase`
- ❌ Incorrect: `public class MyIndicatorTests : TestBase`

**Consequences of incorrect inheritance**:

- Missing abstract method implementations required by `BufferListTestBase`
- Compilation errors for `AddQuotes()`, `AddQuotesBatch()`, `WithQuotesCtor()`, `ClearResetsState()`, and `AutoListPruning()`
- Tests won't validate essential BufferList behaviors

The `BufferListTestBase` abstract class defines 5 required test methods that ensure BufferList implementations work correctly:

1. `AddQuotes()` - Individual quote addition
2. `AddQuotesBatch()` - Batch quote addition via collection initializer
3. `WithQuotesCtor()` - Constructor with quotes parameter
4. `ClearResetsState()` - State reset functionality
5. `AutoListPruning()` - List size management

**Additional test interfaces** (implement when applicable):

- Implement `ITestReusableBufferList` when indicator supports `IReusable` inputs (provides 3 additional test methods)
- Implement `ITestNonStandardBufferListCache` when using List-based state caches instead of `Queue<T>` (provides `AutoBufferPruning()` test)

### Test coverage expectations

Buffer indicator tests must cover:

1. **Incremental processing** - Verify correct calculation as quotes are added
2. **Buffer capacity** - Test behavior at and beyond buffer limits
3. **State management** - Verify Clear() method resets state properly
4. **Auto-pruning verification** - Exercise overflow scenarios for indicators that extend the base behavior (see notes below)
5. **Memory efficiency** - Confirm no memory leaks or excessive allocations
6. **Performance benchmarks** - Must not exceed expected performance thresholds

### Test structure pattern

```csharp
[TestClass]
public class {IndicatorName}BufferListTests : BufferListTestBase, ITestReusableBufferList
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<{IndicatorName}Result> series
       = Quotes.To{IndicatorName}(lookbackPeriods);

    [TestMethod]
    public void AddDiscreteValues()
    {
        {IndicatorName}List sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItems()
    {
        {IndicatorName}List sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public void AddReusableItemsBatch()
    {
        {IndicatorName}List sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotes()
    {
        {IndicatorName}List sut = new(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AddQuotesBatch()
    {
        {IndicatorName}List sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<{IndicatorName}Result> expectedSeries
            = Quotes.To{IndicatorName}(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(expectedSeries, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void WithQuotesCtor()
    {
        {IndicatorName}List sut = new(lookbackPeriods, Quotes);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();
        IReadOnlyList<{IndicatorName}Result> expected = subset.To{IndicatorName}(lookbackPeriods);

        {IndicatorName}List sut = new(lookbackPeriods, subset);

        sut.Should().HaveCount(subset.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        sut.Clear();

        sut.Should().BeEmpty();

        sut.Add(subset);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }

    [TestMethod]
    public override void AutoListPruning()
    {
        const int maxListSize = 120;

        {IndicatorName}List sut = new(lookbackPeriods)
        {
            MaxListSize = maxListSize
        };

        sut.Add(Quotes);

        IReadOnlyList<{IndicatorName}Result> expected
            = series.Skip(series.Count - maxListSize).ToList();

        sut.Should().HaveCount(maxListSize);
        sut.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
    }
}
```

> **Test Pattern Notes**:
>
> - The `AddQuotes()` test validates single-quote Add usage (iterative adding)
> - The `AddQuotesBatch()` test validates collection initializer syntax using `Add(IReadOnlyList<IQuote>)` method
> - The new `WithQuotesCtor()` test validates the constructor with quotes parameter
> - Use the `AutoListPruning()` pattern override of `BufferListTestBase` to cover the base-class list pruning behavior.
> - The `ClearResetsState()` test should use the quotes constructor since `AddQuotes()` already covers single-quote add
> - Implement `ITestReusableBufferList` on buffer-list tests when the indicator supports `IReusable` inputs. Provide `AddReusableItems`, `AddReusableItemsBatch`, and `AddDiscreteValues` test methods to satisfy the interface contract.
> - For indicators that maintain non-`Queue<T>` caches (for example, custom `List<T>` history buffers), also implement `ITestNonStandardBufferListCache` and add an `AutoBufferPruning()` test that exercises list-level auto-pruning alongside cache pruning.
> - All `BeEquivalentTo` assertions **must** call `options => options.WithStrictOrdering()` to enforce chronological ordering in test comparisons.

### Performance benchmarking

Buffer indicators must include performance tests in the `tools/performance` project that verify efficiency:

```csharp
// In tools/performance project
[MethodImpl(MethodImplOptions.NoInlining)]
public void BufferIndicator{IndicatorName}()
{
    {IndicatorName}BufferList bufferList = new(lookbackPeriods);
    
    foreach (Quote quote in quotes)
    {
        _ = bufferList.Add(quote);
    }
}
```

**Performance expectations**:

- Buffer processing provides memory-efficient incremental processing for streaming scenarios
- Memory usage should remain constant regardless of dataset size
- Should handle real-time data updates efficiently

## Quality standards

### Universal buffer utility usage

- **Always use `BufferUtilities` extension methods** - Never implement custom buffer management logic
- **Choose the right extension method**:
  - Use `buffer.Update()` for standard rolling buffer scenarios
  - Use `buffer.UpdateWithDequeue()` when tracking removed values for sums or calculations
- **Type safety** - Extension methods are generic and work with any data type
- **Null safety** - Extension methods include proper argument validation

### Memory management

- Use `Queue<T>` to maintain efficient FIFO operations with constant memory usage
- Leverage `BufferUtilities` extension methods for consistent memory patterns across all indicators
- Implement proper disposal patterns when applicable
- Avoid unnecessary object allocations in hot paths
- Profile memory usage with large datasets

### Thread safety considerations

- Buffer implementations should be thread-safe for single-writer scenarios
- Document any thread safety limitations
- Consider using appropriate synchronization primitives if needed

### State management

- Implement proper state reset in Clear() method
- Ensure buffer state is consistent across operations
- Handle edge cases in buffer wraparound scenarios

## Buffer utility patterns

### Standard buffer management

Use `buffer.Update()` extension method for most buffer management scenarios:

```csharp
/// <inheritdoc />
public void Add(DateTime timestamp, double value)
{
    // Standard buffer management using extension method
    _buffer.Update(LookbackPeriods, value);
    
    // Calculate when buffer has sufficient data
    double? result = _buffer.Count == LookbackPeriods 
        ? CalculateIndicator() 
        : null;
    
    AddInternal(new {IndicatorName}Result(timestamp, result));
}
```

### Buffer management with dequeue tracking

Use `buffer.UpdateWithDequeue()` extension method when you need to track removed values (e.g., for running sums):

```csharp
private double _runningSum;

/// <inheritdoc />
public void Add(DateTime timestamp, double value)
{
    // Track dequeued value for running sum maintenance using extension method
    double? dequeuedValue = _buffer.UpdateWithDequeue(LookbackPeriods, value);
    
    // Update running sum efficiently
    if (_buffer.Count == LookbackPeriods && dequeuedValue.HasValue)
    {
        _runningSum = _runningSum - dequeuedValue.Value + value;
    }
    else
    {
        _runningSum += value;
    }
    
    // Calculate result using running sum
    double? result = _buffer.Count == LookbackPeriods 
        ? _runningSum / LookbackPeriods 
        : null;
    
    AddInternal(new {IndicatorName}Result(timestamp, result));
}
```

### Complex buffer scenarios

For indicators requiring multiple buffers (like HMA), use the extension methods consistently:

```csharp
/// <inheritdoc />
public void Add(DateTime timestamp, double value)
{
    // Update all buffers using extension methods
    _bufferN1.Update(_periodsN1, value);
    _bufferN2.Update(_periodsN2, value);
    
    // Perform multi-stage calculations
    double? intermediate = CalculateIntermediate();
    if (intermediate.HasValue)
    {
        _synthBuffer.Update(_synthPeriods, intermediate.Value);
        
        double? result = CalculateFinal();
        AddInternal(new {IndicatorName}Result(timestamp, result));
    }
    else
    {
        AddInternal(new {IndicatorName}Result(timestamp));
    }
}
```

## Reference examples

Study these exemplary buffer indicators that demonstrate proper use of universal buffer utilities:

- **WMA**: `src/s-z/Wma/Wma.BufferList.cs` - Standard buffer management with WMA calculation
- **SMA**: `src/s-z/Sma/Sma.BufferList.cs` - Simple buffer management with sum calculation
- **EMA**: `src/e-k/Ema/Ema.BufferList.cs` - Buffer management with dequeue tracking for running sum
- **HMA**: `src/e-k/Hma/Hma.BufferList.cs` - Multi-buffer management for complex calculations
- **ADX**: `src/a-d/Adx/Adx.BufferList.cs` - Complex object buffer management
- **MAMA**: `src/m-r/Mama/Mama.BufferList.cs` - List-based state with separate state array pruning (result list pruning is automatic)

## Integration with other styles

Buffer indicators should maintain consistency with their series counterparts:

- Results should be mathematically identical to series calculations
- Parameter validation should be consistent
- Error handling should follow the same patterns
- Documentation should reference the relationship between styles

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

### ❌ Inconsistent buffer patterns

```csharp
// DON'T: Mix manual and extension-based buffer management
_buffer1.Update(capacity, value);
if (_buffer2.Count == capacity) _buffer2.Dequeue(); // Inconsistent!
_buffer2.Enqueue(value);
```

```csharp
// DO: Use extension methods consistently across all buffers
_buffer1.Update(capacity1, value);
_buffer2.Update(capacity2, value);
```

### ❌ Ignoring dequeue tracking for sums

```csharp
// DON'T: Recalculate sum every time (inefficient)
_buffer.Update(capacity, value);
double sum = _buffer.Sum(); // O(n) operation every time
```

```csharp
// DO: Use dequeue tracking extension method for efficient sum maintenance
double? dequeued = _buffer.UpdateWithDequeue(capacity, value);
if (dequeued.HasValue) _sum = _sum - dequeued.Value + value;
else _sum += value;
```

## Best practices

- **Consistency**: Always use `BufferUtilities` extension methods for all buffer operations
- **Efficiency**: Choose `UpdateWithDequeue()` when tracking removed values
- **Type safety**: Leverage generic extension methods for any data type
- **Validation**: Trust extension methods to handle null safety and argument validation
- **Performance**: Use running calculations when possible instead of recalculating from scratch

## Auto-pruning for long-running scenarios

- Each `BufferList<TResult>` defaults `MaxListSize` to 90% of `int.MaxValue`. Override it with an object initializer when a smaller cap is required (for example, tests or constrained-memory pipelines).
- The base implementation of `AddInternal()` automatically trims only the overflow entries. Do **not** add `DefaultMaxListSize` constants, redeclare `MaxListSize`, or call `PruneList()` manually.
- When indicators maintain additional state backed by `List<T>`, prune those lists after calculating the current value while keeping the minimum lookback. Example:

  ```csharp
  private const int MinBufferSize = 7;
  private const int MaxBufferSize = 1000;

  private void PruneStateBuffers()
  {
      if (_state.Count <= MaxBufferSize)
      {
          return;
      }

      int remove = _state.Count - MinBufferSize;
      if (remove > 0)
      {
          _state.RemoveRange(0, remove);
          _stateAux.RemoveRange(0, remove); // keep parallel lists aligned
      }
  }
  ```

- `Queue<T>` buffers rarely need extra work—the `BufferUtilities` helpers bound them automatically.
- When custom pruning occurs, add a unit test that overflows both the result list and the auxiliary buffers to prove the indicator continues to match the static series output.

## Reference implementations

### Simple buffer indicator (SMA pattern)

```csharp
/// <inheritdoc />
public void Add(DateTime timestamp, double value)
{
    _buffer.Update(LookbackPeriods, value);
    
    double? sma = null;
    if (_buffer.Count == LookbackPeriods)
    {
        double sum = 0;
        foreach (double val in _buffer)
        {
            sum += val;
        }
        sma = sum / LookbackPeriods;
    }
    
    AddInternal(new SmaResult(timestamp, sma));
}
```

### Running sum indicator (EMA pattern)

```csharp
/// <inheritdoc />
public void Add(DateTime timestamp, double value)
{
    double? dequeuedValue = _buffer.UpdateWithDequeue(LookbackPeriods, value);
    
    if (_buffer.Count == LookbackPeriods && dequeuedValue.HasValue)
    {
        _bufferSum = _bufferSum - dequeuedValue.Value + value;
    }
    else
    {
        _bufferSum += value;
    }
    
    // Use _bufferSum for efficient calculations
    double? result = _buffer.Count == LookbackPeriods 
        ? _bufferSum / LookbackPeriods 
        : null;
    
    AddInternal(new EmaResult(timestamp, result));
}
```

---
Last updated: September 29, 2025
