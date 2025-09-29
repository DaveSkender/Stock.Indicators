---
applyTo: "src/**/*.BufferList.cs,tests/**/*.BufferList.Tests.cs"
description: "Buffer-style indicator development and testing guidelines"
---

# Buffer indicator development guidelines

These instructions apply to buffer-style indicators that process data incrementally with efficient buffering mechanisms. Buffer indicators optimize memory usage and processing for scenarios involving incremental data updates.

## File naming conventions

Buffer indicators should follow these naming patterns:

- **Implementation**: `{IndicatorName}.BufferList.cs`
- **Tests**: `{IndicatorName}.BufferList.Tests.cs`

## Implementation requirements

### Core structure

Buffer indicators extend the `IBufferList<TResult>` interface and provide efficient incremental processing:

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
        // Add quote to buffer and calculate result
    }

    /// <inheritdoc />
    public void Clear()
    {
        _buffer.Clear();
    }
}

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

### Memory management

- Use `Queue<T>` to maintain efficient FIFO operations with constant memory usage
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

## Buffer patterns

### Efficient buffer usage

```csharp
private readonly Queue<double> _values;
private readonly Queue<DateTime> _timestamps;

public {IndicatorName}BufferList(int lookbackPeriods)
{
    _values = new Queue<double>(lookbackPeriods);
    _timestamps = new Queue<DateTime>(lookbackPeriods);
}

public {IndicatorName}Result Add<TQuote>(TQuote quote) where TQuote : IQuote
{
    _values.Enqueue(quote.Close);
    _timestamps.Enqueue(quote.Date);
    
    if (_values.Count > _lookbackPeriods)
    {
        _values.Dequeue();
        _timestamps.Dequeue();
    }
    
    if (_values.Count < _lookbackPeriods)
    {
        return new {IndicatorName}Result { Timestamp = quote.Date };
    }
    
    // Calculate using buffer data
    double calculatedValue = CalculateFromBuffer();
    
    return new {IndicatorName}Result
    {
        Timestamp = quote.Date,
        Value = calculatedValue
    };
}
```

### Efficient calculations

```csharp
private double CalculateFromBuffer()
{
    // Use buffer data efficiently
    double sum = 0;
    for (int i = 0; i < _values.Count; i++)
    {
        sum += _values[i];
    }
    return sum / _values.Count;
}
```

## Reference examples

Study these exemplary buffer indicators:

- **EMA**: `src/e-k/Ema/Ema.BufferList.cs`
- **SMA**: `src/s-z/Sma/Sma.BufferList.cs` (if implemented)

## Integration with other styles

Buffer indicators should maintain consistency with their series counterparts:

- Results should be mathematically identical to series calculations
- Parameter validation should be consistent
- Error handling should follow the same patterns
- Documentation should reference the relationship between styles

---
Last updated: December 28, 2024
