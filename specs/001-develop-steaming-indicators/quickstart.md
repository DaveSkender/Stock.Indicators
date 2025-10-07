# Quickstart: streaming indicators

**Feature**: 001-develop-steaming-indicators | **Generated**: 2025-10-02

## Goal

Demonstrate real-time SMA calculation using both BufferList and StreamHub streaming styles. This quickstart validates streaming parity with batch calculations and serves as integration test.

## Prerequisites

- .NET 8.0 or 9.0 SDK installed
- Stock.Indicators library built with streaming support
- Test data: 502 quotes from `TestData.GetDefault()` or similar

## Quickstart code

### BufferList example

```csharp
using Skender.Stock.Indicators;

// Initialize streaming SMA with 20-period
var sma = new SmaBufferList(period: 20);

// Simulate live data feed
foreach (var quote in TestData.GetDefault())
{
    var result = sma.Add(quote);
    
    if (result != null)
    {
        Console.WriteLine(
            $"{result.Date:yyyy-MM-dd} | " +
            $"SMA: {result.Sma:F4} | " +
            $"Warmed: {sma.IsWarmedUp}");
    }
    else
    {
        Console.WriteLine(
            $"{quote.Date:yyyy-MM-dd} | " +
            $"Warming up... ({sma.WarmupPeriod - (20 - sma.WarmupPeriod)} of {sma.WarmupPeriod})");
    }
}

// Reset and reprocess
Console.WriteLine("\n--- Resetting indicator ---\n");
sma.Reset();

foreach (var quote in TestData.GetDefault().Take(25))
{
    var result = sma.Add(quote);
    Console.WriteLine(
        result != null 
            ? $"{result.Date:yyyy-MM-dd} | SMA: {result.Sma:F4}"
            : $"{quote.Date:yyyy-MM-dd} | Not warmed up");
}
```

### StreamHub example

```csharp
using Skender.Stock.Indicators;

// Initialize high-performance streaming SMA
var smaHub = new SmaStreamHub(period: 20);

// Process high-frequency data
foreach (var quote in TestData.GetDefault())
{
    var result = smaHub.Add(quote);
    
    if (result != null)
    {
        Console.WriteLine(
            $"{result.Date:yyyy-MM-dd} | " +
            $"SMA: {result.Sma:F4}");
    }
}
```

### Batch vs streaming parity validation

```csharp
using Skender.Stock.Indicators;

// Get batch results
IEnumerable<Quote> quotes = TestData.GetDefault();
var batchResults = quotes.GetSma(period: 20);

// Get streaming results
var streaming = new SmaBufferList(period: 20);
List<SmaResult> streamingResults = [];

foreach (var quote in quotes)
{
    var result = streaming.Add(quote);
    if (result != null)
        streamingResults.Add(result);
}

// Compare results
Console.WriteLine($"Batch results: {batchResults.Count()}");
Console.WriteLine($"Streaming results: {streamingResults.Count}");
Console.WriteLine($"Counts match: {batchResults.Count() == streamingResults.Count}");

// Validate numeric parity
double maxDifference = 0;
for (int i = 0; i < streamingResults.Count; i++)
{
    var batch = batchResults.ElementAt(i);
    var stream = streamingResults[i];
    
    double diff = Math.Abs((double)(batch.Sma - stream.Sma)!.Value);
    maxDifference = Math.Max(maxDifference, diff);
    
    if (diff > 1e-12)
    {
        Console.WriteLine(
            $"MISMATCH at {batch.Date:yyyy-MM-dd}: " +
            $"batch={batch.Sma:F10}, stream={stream.Sma:F10}, diff={diff:E}");
    }
}

Console.WriteLine($"\nMax difference: {maxDifference:E}");
Console.WriteLine($"Parity check: {(maxDifference < 1e-12 ? "PASS" : "FAIL")}");
```

## Expected Output

### BufferList output (first 25 lines)

```text
2024-09-24 | Warming up... (1 of 20)
2024-09-25 | Warming up... (2 of 20)
...
2024-10-13 | Warming up... (19 of 20)
2024-10-14 | SMA: 368.4250 | Warmed: True
2024-10-15 | SMA: 369.1135 | Warmed: True
...
```

### Parity validation output

```text
Batch results: 483
Streaming results: 483
Counts match: True

Max difference: 0.000000E+00
Parity check: PASS
```

## Error handling examples

### Duplicate timestamp

```csharp
var sma = new SmaBufferList(period: 5);

var quote1 = new Quote { Date = DateTime.Parse("2024-01-01"), Close = 100 };
var quote2 = new Quote { Date = DateTime.Parse("2024-01-01"), Close = 101 };

sma.Add(quote1);

try
{
    sma.Add(quote2); // Throws ArgumentException
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    // Output: "Error: Quote timestamp must be strictly ascending"
}
```

### Out-of-order timestamp

```csharp
var sma = new SmaBufferList(period: 5);

var quote1 = new Quote { Date = DateTime.Parse("2024-01-02"), Close = 100 };
var quote2 = new Quote { Date = DateTime.Parse("2024-01-01"), Close = 101 };

sma.Add(quote1);

try
{
    sma.Add(quote2); // Throws ArgumentException
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

### Invalid period

```csharp
try
{
    var sma = new SmaBufferList(period: 0); // Throws ArgumentOutOfRangeException
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    // Output: "Error: Period must be positive (Parameter 'period')"
}
```

## Performance characteristics

### BufferList benchmark

```csharp
using System.Diagnostics;

var quotes = TestData.GetDefault().ToList();
var sma = new SmaBufferList(period: 20);
var sw = Stopwatch.StartNew();

foreach (var quote in quotes)
{
    sma.Add(quote);
}

sw.Stop();
Console.WriteLine($"BufferList: {quotes.Count} quotes in {sw.Elapsed.TotalMilliseconds:F2}ms");
Console.WriteLine($"Avg per quote: {sw.Elapsed.TotalMilliseconds / quotes.Count:F4}ms");
// Expected: <5ms avg per quote
```

### StreamHub benchmark

```csharp
using System.Diagnostics;

var quotes = TestData.GetDefault().ToList();
var smaHub = new SmaStreamHub(period: 20);
var sw = Stopwatch.StartNew();

foreach (var quote in quotes)
{
    smaHub.Add(quote);
}

sw.Stop();
Console.WriteLine($"StreamHub: {quotes.Count} quotes in {sw.Elapsed.TotalMilliseconds:F2}ms");
Console.WriteLine($"Avg per quote: {sw.Elapsed.TotalMilliseconds / quotes.Count:F4}ms");
// Expected: <2ms avg per quote
```

## Integration test template

```csharp
[TestClass]
public class StreamingIntegrationTests
{
    [TestMethod]
    public void SmaBufferList_QuickstartScenario()
    {
        // Arrange
        var quotes = TestData.GetDefault();
        var sma = new SmaBufferList(period: 20);

        // Act
        List<SmaResult> results = [];
        foreach (var quote in quotes)
        {
            var result = sma.Add(quote);
            if (result != null)
                results.Add(result);
        }

        // Assert
        Assert.AreEqual(483, results.Count);
        Assert.IsTrue(sma.IsWarmedUp);
        Assert.AreEqual(20, sma.WarmupPeriod);
    }

    [TestMethod]
    public void SmaStreamHub_HighFrequencyScenario()
    {
        // Arrange
        var quotes = TestData.GetDefault();
        var smaHub = new SmaStreamHub(period: 20);

        // Act
        var sw = Stopwatch.StartNew();
        int resultCount = 0;
        
        foreach (var quote in quotes)
        {
            var result = smaHub.Add(quote);
            if (result != null)
                resultCount++;
        }
        
        sw.Stop();

        // Assert
        Assert.AreEqual(483, resultCount);
        Assert.IsTrue(sw.Elapsed.TotalMilliseconds < 50, 
            $"Expected <50ms, got {sw.Elapsed.TotalMilliseconds:F2}ms");
    }

    [TestMethod]
    public void SmaBufferList_ParityWithBatch()
    {
        // Arrange
        var quotes = TestData.GetDefault();
        var batch = quotes.GetSma(period: 20);
        var streaming = new SmaBufferList(period: 20);

        // Act
        List<SmaResult> streamResults = [];
        foreach (var quote in quotes)
        {
            var result = streaming.Add(quote);
            if (result != null)
                streamResults.Add(result);
        }

        // Assert
        Assert.AreEqual(batch.Count(), streamResults.Count);
        
        for (int i = 0; i < streamResults.Count; i++)
        {
            var b = batch.ElementAt(i);
            var s = streamResults[i];
            
            Assert.AreEqual(b.Date, s.Date);
            Assert.AreEqual(b.Sma, s.Sma, 1e-12, 
                $"Mismatch at index {i}: batch={b.Sma}, stream={s.Sma}");
        }
    }
}
```

## Next steps

1. Run quickstart code to validate streaming behavior
2. Execute integration tests to verify parity with batch
3. Benchmark performance to confirm <5ms avg latency
4. Extend to other indicators (EMA, RSI, MACD, Bollinger Bands)

## Troubleshooting

| Issue | Cause | Solution |
|-------|-------|----------|
| Results always null | Not enough quotes for warmup | Feed at least `WarmupPeriod` quotes |
| ArgumentException on Add() | Duplicate or out-of-order timestamps | Ensure strictly ascending `quote.Date` |
| Parity check fails | Math precision or buffer logic error | Review circular buffer wraparound logic |
| Performance below target | Excessive allocations or LINQ | Profile with BenchmarkDotNet, optimize hot paths |

---

*See plan.md for full implementation roadmap and data-model.md for entity definitions*

---
Last updated: October 6, 2025
