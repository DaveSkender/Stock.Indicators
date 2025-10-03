---
applyTo: "src/**/*.StreamHub.cs,tests/**/*.StreamHub.Tests.cs"
description: "Stream-style indicator development and testing guidelines"
---

# Stream indicator development guidelines

These instructions apply to stream-style indicators that support real-time data processing with stateful operations. Stream indicators maintain internal state and can process individual quotes as they arrive.

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

**Provider Selection Guidelines**:

- Use `IQuoteProvider<TIn>` and `QuoteProvider<TIn, TResult>` when the indicator requires multiple quote properties (e.g., OHLCV data)
- Use `IChainProvider<TIn>` and `ChainProvider<TIn, TResult>` when the indicator can work with single reusable values

Note: IQuote → QuotePart selectors exist but are rarely used for new indicators.

## File naming conventions

Stream indicators should follow these naming patterns:

- **Implementation**: `{IndicatorName}.StreamHub.cs`
- **Tests**: `{IndicatorName}.StreamHub.Tests.cs`

## Implementation requirements

### Core stream hub structure

Stream indicators implement the `IStreamHub<TResult>` interface for stateful processing:

```csharp
/// <summary>
/// Stream hub for {IndicatorName} indicator calculations
/// </summary>
public sealed class {IndicatorName}StreamHub : IStreamHub<{IndicatorName}Result>
{
    private readonly int _lookbackPeriods;
    private readonly Queue<double> _values;
    private double _currentSum;
    private bool _isInitialized;

    public {IndicatorName}StreamHub(int lookbackPeriods)
    {
        _lookbackPeriods = lookbackPeriods;
        _values = new Queue<double>(lookbackPeriods);
    }

    /// <inheritdoc />
    public {IndicatorName}Result Add<TQuote>(TQuote quote) where TQuote : IQuote
    {
        // Process quote and update internal state
        // Return calculated result
    }

    /// <inheritdoc />
    public void Reset()
    {
        _values.Clear();
        _currentSum = 0;
        _isInitialized = false;
    }
}
```

### Extension method

```csharp
/// <summary>
/// Creates a stream hub for real-time {IndicatorName} calculations
/// </summary>
public static {IndicatorName}StreamHub To{IndicatorName}StreamHub<TQuote>(
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

    // Initialize and populate stream hub
    {IndicatorName}StreamHub streamHub = new(lookbackPeriods);
    
    foreach (TQuote quote in quotes)
    {
        streamHub.Add(quote);
    }
    
    return streamHub;
}
```

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

```csharp
[TestClass]
public class {IndicatorName}StreamHubTests : TestBase
{
    [TestMethod]
    public void Add()
    {
        // Test streaming with state maintenance
        {IndicatorName}StreamHub streamHub = new(lookbackPeriods);
        List<{IndicatorName}Result> streamResults = new();
        
        foreach (Quote quote in quotes)
        {
            {IndicatorName}Result result = streamHub.Add(quote);
            streamResults.Add(result);
        }
        
        // Verify against expected results
        {IndicatorName}Result lastResult = streamResults.Last();
        lastResult.{Property}.Should().BeApproximately(expectedValue, precision);
    }

    [TestMethod]
    public void Reset()
    {
        {IndicatorName}StreamHub streamHub = quotes.To{IndicatorName}StreamHub();
        streamHub.Reset();
        
        // Verify state is completely reset
        {IndicatorName}Result result = streamHub.Add(quotes.First());
        result.{Property}.Should().BeNull();
    }

    [TestMethod]
    public void ConsistencyWithSeries()
    {
        // Compare stream results with series results
        List<{IndicatorName}Result> streamResults = new();
        {IndicatorName}StreamHub streamHub = new(lookbackPeriods);
        
        foreach (Quote quote in quotes)
        {
            streamResults.Add(streamHub.Add(quote));
        }
        
        IReadOnlyList<{IndicatorName}Result> seriesResults = quotes.To{IndicatorName}();
        
        streamResults.Should().BeEquivalentTo(seriesResults);
    }

    [TestMethod]
    public void RealTimeSimulation()
    {
        // Simulate real-time data processing
        {IndicatorName}StreamHub streamHub = new(lookbackPeriods);
        
        // Process initial historical data
        foreach (Quote quote in quotes.Take(100))
        {
            streamHub.Add(quote);
        }
        
        // Process new incoming quotes
        foreach (Quote quote in quotes.Skip(100).Take(10))
        {
            {IndicatorName}Result result = streamHub.Add(quote);
            
            // Verify real-time calculations
            if (result.{Property}.HasValue)
            {
                result.{Property}.Should().BeInRange(minExpected, maxExpected);
            }
        }
    }
}
```

### Performance benchmarking

Stream indicators must include performance tests in the `tools/performance` project for high-frequency scenarios:

```csharp
// In tools/performance project
[MethodImpl(MethodImplOptions.NoInlining)]
public void StreamIndicator{IndicatorName}()
{
    {IndicatorName}StreamHub streamHub = new(lookbackPeriods);
    
    foreach (Quote quote in quotes)
    {
        _ = streamHub.Add(quote);
    }
}
```

**Performance expectations**:

- Stream processing should handle high-frequency updates (1000+ quotes/second)
- Memory usage should remain bounded during continuous operation
- State updates should be O(1) complexity when possible

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

## Stream patterns

### Efficient rolling calculations

```csharp
public {IndicatorName}Result Add<TQuote>(TQuote quote) where TQuote : IQuote
{
    double newValue = quote.Close;
    
    if (_values.Count >= _lookbackPeriods)
    {
        // Remove oldest value from running calculation
        double oldValue = _values.Dequeue();
        _currentSum -= oldValue;
    }
    
    // Add new value
    _values.Enqueue(newValue);
    _currentSum += newValue;
    
    // Calculate result
    double? calculatedValue = _values.Count >= _lookbackPeriods 
        ? _currentSum / _lookbackPeriods 
        : null;
    
    return new {IndicatorName}Result
    {
        Timestamp = quote.Date,
        Value = calculatedValue
    };
}
```

### State initialization patterns

```csharp
private void EnsureInitialized<TQuote>(TQuote quote) where TQuote : IQuote
{
    if (!_isInitialized)
    {
        // Initialize any required state
        _previousValue = quote.Close;
        _isInitialized = true;
    }
}
```

### Complex state management

```csharp
public sealed class {IndicatorName}StreamHub : IStreamHub<{IndicatorName}Result>
{
    private readonly struct StateSnapshot
    {
        public double Value { get; init; }
        public DateTime Timestamp { get; init; }
        public int Index { get; init; }
    }
    
    private readonly CircularBuffer<StateSnapshot> _history;
    private int _currentIndex;
    
    // Efficient state updates with history tracking
}
```

## Integration patterns

### Chaining with other indicators

Stream indicators should support chaining with other stream indicators:

```csharp
// Example: EMA of RSI streaming
var rsiStream = quotes.ToRsiStreamHub(14);
var emaOfRsiStream = new EmaStreamHub(20);

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

## Error handling

### Graceful degradation

Stream indicators should handle problematic data gracefully:

```csharp
public {IndicatorName}Result Add<TQuote>(TQuote quote) where TQuote : IQuote
{
    try
    {
        // Validate quote data
        if (double.IsNaN(quote.Close) || double.IsInfinity(quote.Close))
        {
            return new {IndicatorName}Result { Timestamp = quote.Date };
        }
        
        // Normal processing
        return ProcessQuote(quote);
    }
    catch (Exception ex)
    {
        // Log error and return null result
        return new {IndicatorName}Result { Timestamp = quote.Date };
    }
}
```

---
Last updated: September 28, 2025
