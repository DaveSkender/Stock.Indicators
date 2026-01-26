---
title: Buffer list style
description: Learn how to use Buffer list style indicators for efficient incremental quote-by-quote processing
---

# Buffer style indicators for incremental processing

Buffer list style provides efficient incremental processing for growing datasets. Use this when quotes arrive sequentially and you need to add them one at a time without the overhead of a full hub infrastructure.

## When to use buffer lists

**Ideal for:**

- Building up historical data incrementally
- Processing data feeds where quotes arrive sequentially
- Self-managed incremental calculations
- Scenarios without multi-indicator coordination needs
- Memory-efficient processing with auto-pruning

**Not ideal for:**

- Complete historical datasets (use [Batch style](/features/batch))
- Multiple indicators needing coordinated updates (use [Stream hubs](/features/stream))
- One-time calculations (use [Batch style](/features/batch))

## Basic usage

Buffer lists maintain incremental state as you add new quotes:

```csharp
using Skender.Stock.Indicators;

// create buffer list with lookback period
SmaList smaList = new(lookbackPeriods: 20);

// add quotes incrementally (e.g., from a data feed)
foreach (Quote quote in quotes)
{
    smaList.Add(quote);

    // safely get latest result
    if (smaList.Count > 0)
    {
        SmaResult r = smaList[^1];

        // use result (SMA is null during warmup period)
        if (r.Sma is not null)
        {
            Console.WriteLine($"{r.Timestamp:d}: SMA = {r.Sma:N2}");
        }
    }
}
```

::: tip
Using `smaList[^1]` on an empty list throws `IndexOutOfRangeException`. Always check `Count > 0` first, or use `smaList.LastOrDefault()` which returns `null` when empty.
:::

## Key features

### Collection interface

Buffer lists implement `ICollection<TResult>` for standard operations:

```csharp
SmaList smaList = new(20);

// add individual quotes
smaList.Add(quote);

// or add batches
smaList.Add(quoteList);

// standard collection operations
int count = smaList.Count;
bool isEmpty = smaList.Count == 0;
```

### Automatic buffer management

Buffer lists automatically manage internal buffers needed for calculations:

- Maintains lookback periods internally
- No manual state management required
- Efficient incremental updates (typically O(1) or O(log n))

### Memory management

Control memory usage with `MaxListSize`:

```csharp
SmaList smaList = new(20)
{
    MaxListSize = 1000  // keep only last 1000 results
};
```

When the list exceeds `MaxListSize`, older results are automatically pruned. Default is 100,000 elements.

## Chaining indicators

Chain buffer lists for derived indicators:

```csharp
// create OBV buffer list
ObvList obvList = new();

// add quotes to OBV
foreach (var quote in quotes)
{
    obvList.Add(quote);
}

// chain RSI from OBV results
RsiList rsiList = new(14);
foreach (var obvResult in obvList)
{
    rsiList.Add(obvResult);
}

// get latest RSI of OBV
RsiResult latest = rsiList[^1];
```

## Performance characteristics

- **Overhead:** ~10-20% slower than batch style for equivalent datasets
- **Memory:** Maintains internal buffers for lookback periods
- **Latency:** Optimized for per-quote updates, typically O(1) or O(log n)
- **Thread safety:** Not thread-safe; synchronize external access if needed

## Usage patterns

### Simulating a data stream

```csharp
SmaList smaList = new(20);

// add new quote
smaList.Add(quote);
    
// list auto-adds incremental SMA value
SmaResult latest = smaList[^1];
Console.WriteLine($"{latest.Timestamp:d}: SMA = {latest.Sma:N2}");
```

### Batch addition with incremental updates

```csharp
SmaList smaList = new(20);

// add initial batch
smaList.Add(historicalQuotes);

// then add new quotes incrementally
while (newQuote = GetNextQuote())
{
    smaList.Add(newQuote);
    ProcessLatestResult(smaList[^1]);
}
```

## See also

- [Batch style](/features/batch) for one-time calculations
- [Stream hubs](/features/stream) for coordinated real-time updates
- [Guide](/guide#incremental-buffer-style-indicators) for detailed patterns
- [Indicators](/indicators) for available buffer list indicators
