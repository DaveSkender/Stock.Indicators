---
title: Stream hub style
description: Learn how to use Stream hub style for real-time indicator processing with observable patterns and coordinated updates
---

# Hub style indicators for advanced streaming

Stream hub style provides real-time processing with observable patterns and state management. Multiple indicators can subscribe to a central `QuoteHub` for coordinated updates, making it ideal for live data feeds and complex event-driven architectures.

## When to use stream hubs

**Ideal for:**

- Live data feeds and WebSocket integration
- Multiple indicators requiring synchronized updates
- Trading applications with low-latency requirements
- Real-time dashboards and monitoring
- Complex event-driven architectures

**Not ideal for:**

- One-time historical analysis (use [Batch style](/features/batch))
- Simple incremental processing (use [Buffer lists](/features/buffer))
- Scenarios without real-time requirements

## Basic usage

Create a quote hub and subscribe indicators as observers:

```csharp
using Skender.Stock.Indicators;

// create quote hub
QuoteHub quoteHub = new();

// subscribe indicators (observers)
SmaHub smaHub = quoteHub.ToSmaHub(20);
RsiHub rsiHub = quoteHub.ToRsiHub(14);
MacdHub macdHub = quoteHub.ToMacdHub();

// stream quotes as they arrive
foreach (Quote quote in liveQuotes)
{
    // single update propagates to all observers
    quoteHub.Add(quote);
    
    // access latest results from each indicator
    SmaResult sma = smaHub.Results.LastOrDefault();
    RsiResult rsi = rsiHub.Results.LastOrDefault();
    MacdResult macd = macdHub.Results.LastOrDefault();
    
    // use results for trading logic, alerts, etc.
}
```

## Key features

### Observable pattern

The hub-observer architecture ensures coordinated updates:

- Single quote update propagates to all subscribed indicators
- Automatic cascade execution in correct sequence
- Reduced coordination complexity
- Minimal latency overhead

### Chaining indicators

Create sophisticated derived indicators:

```csharp
QuoteHub quoteHub = new();

// chain RSI from EMA
EmaHub emaHub = quoteHub.ToEmaHub(20);
RsiHub rsiHub = emaHub.ToRsiHub(14);  // RSI of EMA

// or chain directly
RsiHub rsiOfEma = quoteHub
    .ToEmaHub(20)
    .ToRsiHub(14);

// publish quote - both EMA and RSI update automatically
quoteHub.Add(newQuote);
```

### State management and rollback

Handle late-arriving data and corrections:

```csharp
QuoteHub quoteHub = new();
SmaHub smaHub = quoteHub.ToSmaHub(20);

// add quotes normally
quoteHub.Add(quote1);
quoteHub.Add(quote2);

// late-arriving data with earlier timestamp
quoteHub.Insert(lateQuote);  // triggers recalculation

// remove incorrect quote
quoteHub.Remove(badQuote);   // triggers recalculation
```

The hub automatically handles state rollback and recalculation when data arrives out of order or needs correction.

## Performance characteristics

- **Overhead:** ~20-30% slower than batch style for equivalent datasets
- **Memory:** Maintains cache and state for all subscribed indicators
- **Latency:** Optimized for real-time updates, typically <1ms per quote
- **Scalability:** Supports multiple concurrent observers with single propagation
- **Thread safety:** Not thread-safe by default; synchronize external access

## Advanced patterns

### Multiple symbol tracking

```csharp
// track multiple symbols with separate hubs
Dictionary<string, QuoteHub> hubs = new();

void ProcessQuote(string symbol, Quote quote)
{
    if (!hubs.ContainsKey(symbol))
    {
        hubs[symbol] = new QuoteHub();
    }
    
    hubs[symbol].Add(quote);
}
```

### Coordinated indicator updates

```csharp
QuoteHub quoteHub = new();

// create multiple indicator hubs
var sma20 = quoteHub.ToSmaHub(20);
var sma50 = quoteHub.ToSmaHub(50);
var rsi = quoteHub.ToRsiHub(14);
var macd = quoteHub.ToMacdHub();

// single update cascades to all
quoteHub.Add(newQuote);

// all indicators now have synchronized timestamps
bool aboveGoldenCross = 
    sma20.Results.[^1].Sma > sma50.Results.[^1].Sma;
```

### Event-driven alerts

```csharp
QuoteHub quoteHub = new();
RsiHub rsiHub = quoteHub.ToRsiHub(14);

void ProcessLiveData(Quote quote)
{
    quoteHub.Add(quote);
    
    RsiResult latest = rsiHub.Results.LastOrDefault();
    
    if (latest?.Rsi > 70)
    {
        TriggerAlert("Overbought", quote.Close, latest.Rsi);
    }
    else if (latest?.Rsi < 30)
    {
        TriggerAlert("Oversold", quote.Close, latest.Rsi);
    }
}
```

## WebSocket integration example

```csharp
// setup hubs
QuoteHub quoteHub = new();
SmaHub smaHub = quoteHub.ToSmaHub(20);

// WebSocket message handler
async Task OnQuoteReceived(WebSocketQuote wsQuote)
{
    // convert WebSocket quote to library Quote
    Quote quote = new()
    {
        Timestamp = wsQuote.Timestamp,
        Open = wsQuote.Open,
        High = wsQuote.High,
        Low = wsQuote.Low,
        Close = wsQuote.Close,
        Volume = wsQuote.Volume
    };
    
    // update hub - all observers cascade automatically
    quoteHub.Add(quote);
    
    // broadcast updated indicators to clients
    await BroadcastIndicators(smaHub.Results.Last());
}
```

## Memory management

Stream hubs automatically prune old results when the cache exceeds the configured maximum size:

```csharp
// default max cache size (~1.9 billion items)
QuoteHub quoteHub = new();

// or configure custom max cache size
QuoteHub limitedHub = new(maxCacheSize: 10000);

// automatic FIFO pruning when limit reached
SmaHub smaHub = limitedHub.ToSmaHub(20);

// as new quotes arrive, oldest results are removed automatically
foreach (Quote quote in liveQuotes)
{
    limitedHub.Add(quote);  // oldest pruned if over limit
}
```

The default cache size is very large (90% of `int.MaxValue`) to accommodate long-running streams. For applications with memory constraints, specify a smaller `maxCacheSize` when creating the QuoteHub.

## See also

- [Batch style](/features/batch) for one-time calculations
- [Buffer lists](/features/buffer) for simple incremental processing
- [Guide](/guide#streaming-hub-style-indicators) for detailed examples
- [Indicators](/indicators) for available stream hub indicators
