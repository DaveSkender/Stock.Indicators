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

Buffer indicators extend the `IBufferList<TResult>` interface and provide efficient incremental processing using universal buffer utilities:

```csharp
/// <summary>
/// Buffer implementation for {IndicatorName} indicator
/// </summary>
public class {IndicatorName}List : List<{IndicatorName}Result>, I{IndicatorName}, IBufferList, IBufferReusable
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

    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Use universal buffer extension method for consistent buffer management
        _buffer.Update(LookbackPeriods, value);
        
        // Calculate result using buffer data
        double? result = CalculateIndicator();
        
        base.Add(new {IndicatorName}Result(timestamp, result));
    }

    /// <inheritdoc />
    public void Clear()
    {
        base.Clear();
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

> **Interface Selection Guidelines**:
>
> - Use `IBufferList, IBufferReusable` when the indicator's static series can accept `IReusable` values (single values like SMA, EMA)
> - Use only `IBufferList` when the indicator's static series requires `IQuote` (multiple values like VWMA needs price+volume, ADX needs OHLC)
> - Match the interface pattern to what the static series implementation supports
>
> **Note**: The current codebase uses `Queue<T>` for efficient FIFO buffering operations. `Queue<T>` provides O(1) enqueue/dequeue operations and is well-suited for sliding window calculations where you need to remove the oldest value when adding a new one.

### Extension method

```csharp
/// <summary>
/// Creates a buffer list for {IndicatorName} calculations
/// </summary>
public static {IndicatorName}List To{IndicatorName}BufferList<TQuote>(
    this IReadOnlyList<TQuote> quotes,
    int lookbackPeriods = {defaultValue})
    where TQuote : IQuote
{
    // Input validation
    quotes.ThrowIfNull();
    
    if (lookbackPeriods <= 0)
    {
        throw new ArgumentOutOfRangeException(nameof(lookbackPeriods));
    }

    // Initialize buffer and populate
    {IndicatorName}List bufferList = new(lookbackPeriods);
    
    foreach (TQuote quote in quotes)
    {
        bufferList.Add(quote);
    }
    
    return bufferList;
}
```

## Testing requirements

### Test coverage expectations

Buffer indicator tests must cover:

1. **Incremental processing** - Verify correct calculation as quotes are added
2. **Buffer capacity** - Test behavior at and beyond buffer limits
3. **State management** - Verify Clear() method resets state properly
4. **Memory efficiency** - Confirm no memory leaks or excessive allocations
5. **Performance benchmarks** - Must not exceed expected performance thresholds

### Test structure pattern

```csharp
[TestClass]
public class {IndicatorName}BufferListTests : BufferListTestBase
{
    private const int lookbackPeriods = 14;

    private static readonly IReadOnlyList<IReusable> reusables
       = Quotes
        .Cast<IReusable>()
        .ToList();

    private static readonly IReadOnlyList<{IndicatorName}Result> series
       = Quotes.To{IndicatorName}(lookbackPeriods);

    [TestMethod]
    public void FromReusableSplit()
    {
        {IndicatorName}List sut = new(lookbackPeriods);

        foreach (IReusable item in reusables)
        {
            sut.Add(item.Timestamp, item.Value);
        }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableItem()
    {
        {IndicatorName}List sut = new(lookbackPeriods);

        foreach (IReusable item in reusables) { sut.Add(item); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void FromReusableBatch()
    {
        {IndicatorName}List sut = new(lookbackPeriods) { reusables };

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuote()
    {
        {IndicatorName}List sut = new(lookbackPeriods);

        foreach (Quote q in Quotes) { sut.Add(q); }

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public override void FromQuoteBatch()
    {
        {IndicatorName}List sut = new(lookbackPeriods) { Quotes };

        IReadOnlyList<{IndicatorName}Result> series
            = Quotes.To{IndicatorName}(lookbackPeriods);

        sut.Should().HaveCount(Quotes.Count);
        sut.Should().BeEquivalentTo(series);
    }

    [TestMethod]
    public void ClearResetsState()
    {
        List<Quote> subset = Quotes.Take(80).ToList();

        {IndicatorName}List sut = new(lookbackPeriods);

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        sut.Should().HaveCount(subset.Count);

        sut.Clear();

        sut.Should().BeEmpty();

        foreach (Quote quote in subset)
        {
            sut.Add(quote);
        }

        IReadOnlyList<{IndicatorName}Result> expected = subset.To{IndicatorName}(lookbackPeriods);

        sut.Should().HaveCount(expected.Count);
        sut.Should().BeEquivalentTo(expected);
    }
}
```

### Performance benchmarking

Buffer indicators must include performance tests in the `tests/performance` project that verify efficiency:

```csharp
// In tests/performance project
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
    
    base.Add(new {IndicatorName}Result(timestamp, result));
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
    
    base.Add(new {IndicatorName}Result(timestamp, result));
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
        base.Add(new {IndicatorName}Result(timestamp, result));
    }
    else
    {
        base.Add(new {IndicatorName}Result(timestamp));
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
    
    base.Add(new SmaResult(timestamp, sma));
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
    
    base.Add(new EmaResult(timestamp, result));
}
```

---
Last updated: September 29, 2025
