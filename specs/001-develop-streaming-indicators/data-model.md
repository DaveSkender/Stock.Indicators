# Data model: streaming indicators framework

**Feature**: 001-develop-streaming-indicators | **Generated**: 2025-10-02

> **IMPORTANT**: This document contains conceptual planning examples that do not match the actual codebase implementation patterns. The real API patterns, class names, and interfaces are defined in:
>
> - `.github/instructions/buffer-indicators.instructions.md` (authoritative for BufferList pattern)
> - `.github/instructions/stream-indicators.instructions.md` (authoritative for StreamHub pattern)
> - Existing implementations in `src/**/*.BufferList.cs` and `src/**/*.StreamHub.cs`
>
> Always reference the instruction files and actual codebase implementations as the source of truth, not the examples in this planning document.

## Overview

This document defines entities, interfaces, and state management for streaming technical indicators. All types support incremental updates (O(1) per quote) and maintain parity with batch calculations.

## Core entities

### IStreamingIndicator<TQuote, TResult>

**Purpose**: Common interface for all streaming indicators

**Definition**:

```csharp
namespace Skender.Stock.Indicators;

/// <summary>
/// Defines a streaming indicator that processes quotes incrementally
/// </summary>
/// <typeparam name="TQuote">Quote type (typically <see cref="Quote"/>)</typeparam>
/// <typeparam name="TResult">Indicator result type (e.g., <see cref="SmaResult"/>)</typeparam>
public interface IStreamingIndicator<in TQuote, out TResult>
    where TQuote : IQuote
{
    /// <summary>
    /// Adds a new quote and computes the indicator value
    /// </summary>
    /// <param name="quote">Market quote with OHLCV data</param>
    /// <returns>
    /// Indicator result if warmed up; otherwise null
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when quote is null</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when quote timestamp is not strictly ascending
    /// </exception>
    TResult? Add(TQuote quote);

    /// <summary>
    /// Resets all internal state and warmup counters
    /// </summary>
    void Reset();

    /// <summary>
    /// Gets the minimum number of quotes required before returning results
    /// </summary>
    int WarmupPeriod { get; }

    /// <summary>
    /// Indicates whether enough quotes have been added to produce results
    /// </summary>
    bool IsWarmedUp { get; }
}
```

**Validation rules**:

- `Add()` must enforce strictly ascending timestamps (reject duplicates/out-of-order)
- `Add()` must return null when `IsWarmedUp == false`
- `Reset()` must clear all buffers and set `IsWarmedUp = false`
- `WarmupPeriod` must match the batch indicator's documented warmup requirement

**State transitions**:

```text
[Initial] --Add(quote)--> [Accumulating] (IsWarmedUp = false)
[Accumulating] --Add(quote N where N = WarmupPeriod)--> [Warmed Up] (IsWarmedUp = true)
[Warmed Up] --Add(quote)--> [Warmed Up] (continues processing)
[Any State] --Reset()--> [Initial]
```

### StreamingState (enum)

**Purpose**: Tracks indicator warmup status

**Definition**:

```csharp
namespace Skender.Stock.Indicators;

/// <summary>
/// Represents the warmup state of a streaming indicator
/// </summary>
public enum StreamingState
{
    /// <summary>
    /// Indicator has not accumulated enough quotes to produce results
    /// </summary>
    NotWarmedUp = 0,

    /// <summary>
    /// Indicator has sufficient history and produces valid results
    /// </summary>
    Ready = 1
}
```

**Usage**: Embedded in result types to communicate warmup status to consumers

### StreamingResult (optional wrapper)

**Purpose**: Augments indicator results with timestamp and state metadata

**Definition**:

```csharp
namespace Skender.Stock.Indicators;

/// <summary>
/// Wraps an indicator result with streaming context
/// </summary>
/// <typeparam name="T">Underlying result type</typeparam>
public sealed record StreamingResult<T>
{
    /// <summary>
    /// Quote timestamp for this result
    /// </summary>
    public required DateTime Timestamp { get; init; }

    /// <summary>
    /// Indicator calculation result
    /// </summary>
    public required T Value { get; init; }

    /// <summary>
    /// Current warmup state
    /// </summary>
    public required StreamingState State { get; init; }
}
```

**Note**: This wrapper is optional. Many indicators can return their native result types directly (e.g., `SmaResult?`) instead of wrapping in `StreamingResult<SmaResult>`. Use native types for simplicity unless additional metadata is required.

## Implementation entities

### BufferList indicator (list-backed)

**Purpose**: Simple streaming implementation using `List<T>` for state storage

**Characteristics**:

- Uses `List<decimal>` or `List<TQuote>` for historical data
- Bounded capacity (removes oldest when exceeding lookback)
- Suitable for moderate frequency (<1k ticks/sec)
- Easier to debug (standard collection semantics)

**Generic pattern**:

```csharp
public sealed class [IndicatorName]BufferList : IStreamingIndicator<Quote, [IndicatorName]Result>
{
    private readonly int period;
    private readonly List<decimal> buffer;
    private DateTime lastTimestamp;

    public int WarmupPeriod => period;
    public bool IsWarmedUp => buffer.Count >= period;

    public [IndicatorName]BufferList(int period)
    {
        if (period < 1)
            throw new ArgumentOutOfRangeException(nameof(period), "Period must be positive");

        this.period = period;
        this.buffer = new List<decimal>(capacity: period + 1);
        this.lastTimestamp = DateTime.MinValue;
    }

    public [IndicatorName]Result? Add(Quote quote)
    {
        // Validate timestamp ordering
        if (quote.Date <= lastTimestamp)
            throw new ArgumentException("Quote timestamp must be strictly ascending");

        lastTimestamp = quote.Date;

        // Update buffer (bounded)
        buffer.Add(quote.Close);
        if (buffer.Count > period)
            buffer.RemoveAt(0);

        // Return null until warmed up
        if (!IsWarmedUp)
            return null;

        // Compute indicator value from buffer
        decimal value = /* calculation logic */;

        return new [IndicatorName]Result
        {
            Date = quote.Date,
            [Property] = value
        };
    }

    public void Reset()
    {
        buffer.Clear();
        lastTimestamp = DateTime.MinValue;
    }
}
```

### StreamHub indicator (span-optimized)

**Purpose**: High-performance streaming using circular buffers

**Characteristics**:

- Uses fixed-size `decimal[]` with head/tail pointers
- Circular wraparound (no shifting/removal)
- Suitable for high frequency (>10k ticks/sec)
- Minimal allocations (spans, no List growth)

**Generic pattern**:

```csharp
public sealed class [IndicatorName]StreamHub : IStreamingIndicator<Quote, [IndicatorName]Result>
{
    private readonly int period;
    private readonly decimal[] buffer;
    private int head;
    private int count;
    private DateTime lastTimestamp;

    public int WarmupPeriod => period;
    public bool IsWarmedUp => count >= period;

    public [IndicatorName]StreamHub(int period)
    {
        if (period < 1)
            throw new ArgumentOutOfRangeException(nameof(period), "Period must be positive");

        this.period = period;
        this.buffer = new decimal[period];
        this.head = 0;
        this.count = 0;
        this.lastTimestamp = DateTime.MinValue;
    }

    public [IndicatorName]Result? Add(Quote quote)
    {
        // Validate timestamp ordering
        if (quote.Date <= lastTimestamp)
            throw new ArgumentException("Quote timestamp must be strictly ascending");

        lastTimestamp = quote.Date;

        // Update circular buffer
        buffer[head] = quote.Close;
        head = (head + 1) % period;
        if (count < period)
            count++;

        // Return null until warmed up
        if (!IsWarmedUp)
            return null;

        // Compute indicator value from span
        Span<decimal> window = buffer.AsSpan();
        decimal value = /* calculation logic using window */;

        return new [IndicatorName]Result
        {
            Date = quote.Date,
            [Property] = value
        };
    }

    public void Reset()
    {
        Array.Clear(buffer, 0, buffer.Length);
        head = 0;
        count = 0;
        lastTimestamp = DateTime.MinValue;
    }
}
```

## Initial indicator coverage

### Phase 1 indicators (5 total)

1. **SMA (Simple Moving Average)**:
   - Warmup: `period` quotes
   - Calculation: Average of last `period` close prices

2. **EMA (Exponential Moving Average)**:
   - Warmup: `period` quotes (initial SMA seed)
   - Calculation: Recursive EMA formula

3. **RSI (Relative Strength Index)**:
   - Warmup: `period + 1` quotes
   - Calculation: RS = avgGain / avgLoss; RSI = 100 - (100 / (1 + RS))

4. **MACD (Moving Average Convergence Divergence)**:
   - Warmup: `slowPeriod` quotes
   - Calculation: MACD = EMA(12) - EMA(26), Signal = EMA(9) of MACD

5. **Bollinger Bands**:
   - Warmup: `period` quotes
   - Calculation: SMA ± (stdDev * multiplier)

## Relationships

**IMPORTANT**: The actual class names and patterns differ from preliminary planning examples. See existing implementations in `src/**/*.BufferList.cs` and `src/**/*.StreamHub.cs` for authoritative patterns.

**Actual naming conventions**:

- BufferList style: `{IndicatorName}List` (e.g., `SmaList`, `EmaList`, `RsiList`)
- StreamHub style: `{IndicatorName}Hub<TIn>` (e.g., `SmaHub<TIn>`, `EmaHub<TIn>`, `RsiHub<TIn>`)

**Inheritance patterns**:

- BufferList implementations inherit from `BufferList<TResult>` base class
- StreamHub implementations extend `ChainProvider<TIn, TResult>` or `QuoteProvider<TIn, TResult>`

Refer to `.github/instructions/buffer-indicators.instructions.md` and `.github/instructions/stream-indicators.instructions.md` for complete details.

## Validation matrix

| Entity | Validation | Error Type |
|--------|-----------|-----------|
| IStreamingIndicator.Add() | quote != null | ArgumentNullException |
| IStreamingIndicator.Add() | quote.Date > lastTimestamp | ArgumentException |
| BufferList constructor | period > 0 | ArgumentOutOfRangeException |
| StreamHub constructor | period > 0 | ArgumentOutOfRangeException |
| BufferList.Add() | Buffer capacity bounded | (automatic removal) |
| StreamHub.Add() | Circular wraparound | (automatic overwrite) |

## Performance targets

| Metric | BufferList | StreamHub | Rationale |
|--------|-----------|-----------|-----------|
| Latency (avg) | <5ms | <2ms | StreamHub optimized for low latency |
| Latency (p95) | <10ms | <5ms | Tighter bounds for high-frequency |
| Memory per instance | <10KB | <5KB | Bounded buffers, no unbounded growth |
| Throughput | 1k ticks/sec | 20k ticks/sec | BufferList simpler, StreamHub faster |

## Testing entities

### Streaming parity test fixture

**Purpose**: Validate streaming output matches batch calculation

**Pattern**:

```csharp
[TestClass]
public class [Indicator]ParityTests
{
    [TestMethod]
    public void BufferList_MatchesBatch()
    {
        // Arrange
        IEnumerable<Quote> quotes = TestData.GetDefault();
        var expected = quotes.Get[Indicator]();
        var streaming = new [Indicator]BufferList(period);

        // Act
        List<[Indicator]Result> actual = [];
        foreach (var quote in quotes)
        {
            var result = streaming.Add(quote);
            if (result != null)
                actual.Add(result);
        }

        // Assert
        Assert.AreEqual(expected.Count(), actual.Count);
        for (int i = 0; i < actual.Count; i++)
        {
            Assert.AreEqual(expected.ElementAt(i).Date, actual[i].Date);
            Assert.AreEqual(expected.ElementAt(i).[Property], actual[i].[Property], 1e-12);
        }
    }
}
```

---

*See plan.md for implementation roadmap and Phase 2 task generation approach*

---
Last updated: October 6, 2025
