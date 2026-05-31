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

- One-time historical analysis (use [Batch style](/guide/batch))
- Simple incremental processing (use [Buffer lists](/guide/buffer))
- Scenarios without real-time requirements

## Basic usage

Create a quote hub and subscribe indicators as observers:

```csharp
using Skender.Stock.Indicators;

// create quote hub (the data source)
QuoteHub quoteHub = new();

// subscribe indicators to the hub
SmaHub smaHub = quoteHub.ToSmaHub(20);
RsiHub rsiHub = quoteHub.ToRsiHub(14);

// stream quotes as they arrive
foreach (Quote quote in liveQuotes)
{
    // adding to quoteHub automatically updates all subscribers
    quoteHub.Add(quote);

    // safely get latest results
    if (smaHub.Results.Count > 0)
    {
        SmaResult sma = smaHub.Results[^1];
        RsiResult rsi = rsiHub.Results[^1];

        // use results for trading logic, alerts, etc.
        if (sma.Sma is not null && rsi.Rsi > 70)
        {
            Console.WriteLine($"{quote.Timestamp:d}: Overbought at {quote.Close:C2}");
        }
    }
}
```

::: tip
Using `Results[^1]` on an empty collection throws `ArgumentOutOfRangeException`. Always check `Results.Count > 0` first.
:::

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
quoteHub.Add(lateQuote);  // triggers recalculation in dependent hubs

// correct a bad value: re-add a quote with the same timestamp
quoteHub.Add(correctedQuote);  // same timestamp, revised values → rebuild

// or remove a bad quote (and everything at/after its timestamp)
quoteHub.RemoveRange(badQuote.Timestamp, notify: true);  // triggers recalculation
```

The hub automatically handles state rollback and recalculation when data arrives out of order or needs correction. To revise a single value in place, re-`Add` a quote with the same `Timestamp`; to drop data, remove it by timestamp with `RemoveRange(fromTimestamp, notify)` or by position with `RemoveAt(cacheIndex)`. Always apply these mutations to the **root** hub — the `QuoteHub` (or `TickHub`) you add quotes to — which cascades the change to every dependent hub; don't mutate a subscribed/chained hub directly (see [Thread safety](#thread-safety)).

## Performance characteristics

- **Overhead:** ~20-30% slower than batch style for equivalent datasets
- **Memory:** Maintains cache and state for all subscribed indicators
- **Latency:** Optimized for real-time updates, typically <1ms per quote
- **Scalability:** Supports multiple concurrent observers with single propagation

## Thread safety

Stream hubs follow a **single-writer** model. Internal locking protects cache *integrity* during the rebuild and rollback that out-of-order data triggers — but it does **not** make concurrent mutation safe. Two threads calling mutating methods at the same time can still interleave incorrectly.

- **Internal locking** guards the cache so a rebuild never exposes a half-updated state to readers.
- **Serialize all mutating calls** — `Add`, `RemoveAt`, `RemoveRange`, `Reinitialize`, and `Rebuild` must come from a single writer (one thread, or funneled through a lock / `Channel<Quote>`).
- **Single-threaded usage** requires no additional synchronization.
- **Multi-threaded usage** must serialize every external call that mutates the hub; reads of `Results` should also be coordinated with the writer (see the example below).

### What's safe, and what to avoid

The library deliberately keeps the mutation surface small: `Results` is a read-only view, so you can't reach the underlying cache with `List` / `ICollection` methods. Stay within that surface:

- ✅ **Do** feed and correct data through the **root** hub — the `QuoteHub` (or `TickHub`) you created and add quotes to. It cascades every change to the dependent hubs automatically.
- ✅ **Do** read results through `Results`; if you hand them to another thread, copy or snapshot what you need rather than enumerating the live view while the writer mutates it.
- ❌ **Don't** call `Add` / `RemoveAt` / `RemoveRange` on a *subscribed* (chained) hub such as a `SmaHub`. Those hubs are driven by their provider; mutating one directly desynchronizes it from that provider, and a later rebuild can produce wrong results the hub can't heal from.
- ❌ **Don't** mutate from more than one thread at a time (see the example below).

This single-writer expectation isn't unique to this library: most built-in .NET collections (`List<T>`, `Dictionary<TKey,TValue>`, `Queue<T>`) are likewise unsafe for concurrent writers. The hub adds internal locking to keep its *own* cache consistent during a rebuild, but coordinating *your* calls is still your responsibility — exactly as it would be for any ordinary collection.

### Thread-safe external access example

```csharp
QuoteHub quoteHub = new();
SmaHub smaHub = quoteHub.ToSmaHub(20);

// Use lock for coordinated multi-threaded access
object hubLock = new();

// Thread 1: Adding quotes
Task producer = Task.Run(() =>
{
    foreach (Quote quote in liveQuotes)
    {
        lock (hubLock)
        {
            quoteHub.Add(quote);
        }
    }
});

// Thread 2: Reading results
Task consumer = Task.Run(() =>
{
    while (running)
    {
        SmaResult? latest;
        lock (hubLock)
        {
            latest = smaHub.Results.Count > 0 ? smaHub.Results[^1] : null;
        }
        ProcessResult(latest);
    }
});
```

::: warning
Internal thread safety protects cache integrity during rebuild operations (triggered by out-of-order data). However, external synchronization is still required when multiple threads access the same hub instance concurrently.
:::

## Advanced patterns

### Reactive strategies

```csharp
QuoteHub quoteHub = new();

EmaHub emaFast = quoteHub.ToEmaHub(50);
EmaHub emaSlow = quoteHub.ToEmaHub(200);

// add quotes to quoteHub (from stream)
quoteHub.Add(newQuote);
// and the 2 EmaHub will be in sync

if(emaFast.Results[^2].Ema < emaSlow.Results[^2].Ema
&& emaFast.Results[^1].Ema > emaSlow.Results[^1].Ema)
{
    // cross over occurred
}
```

### Event-driven alerts

```csharp
QuoteHub quoteHub = new();
RsiHub rsiHub = quoteHub.ToRsiHub(14);

void ProcessLiveData(Quote quote)
{
    quoteHub.Add(quote);
    
    RsiResult latest = rsiHub.Results[^1];
    
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

This example demonstrates how to connect stream hubs to a live WebSocket feed. The pattern applies to any real-time data source (WebSocket, SSE, message queue, etc.) where quotes arrive asynchronously. The hub's `Add` method integrates each incoming quote, automatically propagating updates to all subscribed indicators.

Because socket callbacks can fire concurrently, every `Add` must go through a single writer (see [Thread safety](#thread-safety) above). The handler below serializes with a lock; for higher throughput, post incoming quotes onto a single-consumer `Channel<Quote>` and call `Add` from one drain loop instead.

```csharp
// setup hubs
QuoteHub quoteHub = new();
SmaHub smaHub = quoteHub.ToSmaHub(20);

// single-writer gate: serialize every Add to the hub
object hubLock = new();

// WebSocket message handler (may be invoked concurrently)
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

    // update hub through the single-writer gate -
    // all observers cascade automatically
    lock (hubLock)
    {
        quoteHub.Add(quote);
    }

    // in this example, the subscribing SmaHub will
    // auto-generate the next corresponding SMA value
}
```

## Memory management

Stream hubs automatically prune old results when the cache exceeds the configured maximum size:

```csharp
// default max cache size (100,000 items)
QuoteHub quoteHub = new();

// or configure custom max cache size
QuoteHub limitedHub = new(maxCacheSize: 500);

// automatic FIFO pruning when limit reached
SmaHub smaHub = limitedHub.ToSmaHub(20);

// as new quotes arrive, oldest results are removed automatically
foreach (Quote quote in liveQuotes)
{
    limitedHub.Add(quote);  // oldest pruned if over limit
}
```

The default cache size is 100,000 items. For applications with different requirements, specify a custom `maxCacheSize` when creating the QuoteHub.

## Fault handling and recovery

A hub guards against runaway feedback (for example, an accidental circular chain, or a provider stuck re-sending the same tick). If the **same timestamp with identical values** is re-sent more than 100 times in a row, the hub faults:

- `IsFaulted` flips to `true`,
- subscribed observers receive `OnError(OverflowException)`, and
- the offending `Add` throws `OverflowException` ("A repeated stream update exceeded the 100 attempt threshold…").

```csharp
QuoteHub quoteHub = new();
SmaHub smaHub = quoteHub.ToSmaHub(20);

try
{
    quoteHub.Add(quote);
}
catch (OverflowException)
{
    // a circular chain or a stuck provider re-sent an identical tick 100+ times
}

if (quoteHub.IsFaulted)
{
    // clear the fault and resume streaming
    quoteHub.ResetFault();
}
```

`ResetFault()` clears the faulted state so the hub can keep processing. The threshold only trips on *byte-identical* repeats — a normal correction (same timestamp, **different** values) is handled as a rollback, not a fault. If your feed legitimately re-sends identical trade prints, dedupe upstream or vary a field before calling `Add` so a real burst of identical ticks doesn't trip the guard.

::: warning Reinitialize is a single-writer operation
`Reinitialize()` unsubscribes, rebuilds the cache from the provider, then re-subscribes. Quotes that arrive in the brief window between the rebuild and the re-subscribe can be missed. Call `Reinitialize()` only when no concurrent `Add` is in flight — i.e. from the same single writer that feeds the hub.
:::

::: info Cache size inheritance
Hubs will inherit the `maxCacheSize` of its provider.  For example, if you set a size of 1,000 for your `QuoteHub`, then a chained `SmaHub` will also have a maximum cache size of 1,000.
:::

::: tip ✨ ✨ Optimize cache size for your use case

Set your `maxCacheSize` according to how you use the data produced in the hub cache. If you only need the latest indicator values and don't read history, a smaller cache saves memory.

```csharp
// configure a modest cache that still clears every warmup floor
QuoteHub limitedHub = new(maxCacheSize: 500);

// automatic FIFO pruning when limit reached
SmaHub smaHub = limitedHub.ToSmaHub(20);
```

Don't size the cache down to the indicator's lookback, though — two things bite:

- **Inheritance.** The `QuoteHub`'s `maxCacheSize` flows to every chained hub (above), so the cache must satisfy the *largest* warmup in the whole chain, not just one indicator.
- **Warmup floor.** Each hub validates `maxCacheSize` against its own warmup requirement at construction and throws `ArgumentOutOfRangeException` if it's too small. That requirement is often a multiple of the lookback: RSI needs ~2× its period, TEMA and TRIX ~3×, and the Hilbert-transform trendline needs a fixed 63 periods regardless of lookback.

Pick a floor comfortably above the deepest warmup in the chain (with headroom for late-arrival rollbacks), rather than the bare indicator minimum.

:::

## See also

- [Batch style](/guide/batch) for one-time calculations
- [Buffer lists](/guide/buffer) for simple incremental processing
- [Custom observers](/guide/custom-observers) for wrapping a hub to push results into a UI, persistence, or alerting pipeline
- [Indicators](/indicators) for available stream hub indicators
