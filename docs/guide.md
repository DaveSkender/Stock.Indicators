---
title: Guide and Pro tips
description: Learn how to use the Stock Indicators for .NET Nuget library in your own software tools and platforms.  Whether you're just getting started or an advanced professional, this guide explains how to get setup, example usage code, and instructions on how to use historical price quotes, make custom quote classes, chain indicators of indicators, and create custom technical indicators.
---

# Getting started

## Installation and setup

Find and install the [Skender.Stock.Indicators](https://www.nuget.org/packages/Skender.Stock.Indicators) NuGet package into your Project.  See more [help for installing packages](https://www.google.com/search?q=install+nuget+package).

```powershell
# dotnet CLI example
dotnet add package Skender.Stock.Indicators

# package manager example
Install-Package Skender.Stock.Indicators
```

## Prerequisite data

Most indicators require that you provide historical quote data and additional configuration parameters.

You must get historical quotes from your own market data provider.  For clarification, the `GetQuotesFromFeed()` method shown in the example below **is not part of this library**, but rather an example to represent your own acquisition of historical quotes.

Historical price data can be provided as a `List`, `IReadOnlyList`, or `ICollection` of the `Quote` class ([see below](#historical-quotes)); however, it can also be supplied as a generic [custom TQuote type](#using-custom-quote-classes) if you prefer to use your own quote model.

For additional configuration parameters, default values are provided when there is an industry standard.  You can, of course, override these and provide your own values.

## Implementation pattern

Each [indicator style](#indicator-styles-and-features) available (series, buffer list, and stream hub) will have a slightly different [implementation syntax](#example-usage); however, all will follow a common overall pattern.

```csharp
using Skender.Stock.Indicators;

[..]

// step 1: get quote(s) from your source
// step 2: calculate indicator value(s)
```

See [usage examples](#example-usage) for additional details.

## Indicator styles and features

This library has three indicator styles available to support different uses cases.

| Style        | Use case                                     | Best for                       |
| ------------ | -------------------------------------------- | ------------------------------ |
| Series batch | Convert full quote collections to indicators | Once-and-done bulk conversions |
| Buffer lists | Standalone incrementing `ICollection` lists  | Self-managed incrementing data |
| Stream hub   | Subscription based hub-observer pattern      | Streaming or live data sources |

### Style comparison

| Feature        | Series batch    | Buffer lists  | Stream hub   |
| -------------- | --------------- | ------------- | ------------ |
| Incrementing   | no              | yes           | yes          |
| Batch speed    | fastest         | faster        | fast         |
| Scaling        | low             | moderate      | high         |
| Class type     | static          | instance      | instance     |
| Base interface | `IReadOnlyList` | `ICollection` | `IStreamHub` |
| Complexity     | lowest          | moderate      | highest      |
| Chainable      | yes             | yes           | yes          |
| Pruning        | with utility    | auto-preset   | auto-preset  |

<!-- TODO: deduplicate from Features page -->

## Example usage

### Series (batch) style usage example

All series-style indicators will produce all possible results for the provided historical quotes as a time series dataset -- it is not just a single data point returned.  For example, if you provide 3 years worth of historical quotes for the SMA method, you'll get 3 years of SMA result values.

```csharp
using Skender.Stock.Indicators;

[..]

// fetch historical quotes from your feed (your method)
IReadOnlyList<Quote> quotes = GetQuotesFromFeed("MSFT");

// calculate 20-period SMA
IReadOnlyList<SmaResult> results = quotes
  .ToSma(20);

// use results as needed for your use case (example only)
foreach (SmaResult r in results)
{
    Console.WriteLine($"SMA on {r.Timestamp:d} was ${r.Sma:N4}");
}
```

```console
SMA on 4/19/2018 was $255.0590
SMA on 4/20/2018 was $255.2015
SMA on 4/23/2018 was $255.6135
SMA on 4/24/2018 was $255.5105
SMA on 4/25/2018 was $255.6570
SMA on 4/26/2018 was $255.9705
..
```

### Buffer list style usage example

Buffer list style indicators maintain incremental state as you add new data points. This is ideal for scenarios where you're building up historical data over time or processing data incrementally without needing a full hub infrastructure.

```csharp
using Skender.Stock.Indicators;

[..]

// create buffer list with lookback period
SmaList smaList = new(20);

// add quotes incrementally (from your data source)
foreach (IQuote quote in quotes)  // simulating stream
{
    smaList.Add(quote);
}

// access results as ICollection
IReadOnlyList<SmaResult> results = smaList;

// or get the latest result
SmaResult latest = smaList.LastOrDefault();
```

**Key features:**

- Implements `ICollection<TResult>` for standard collection operations
- Automatically manages internal buffers for efficient calculations
- Supports `.Add()` for individual quotes or `.Add(IReadOnlyList)` for batches
- Auto-prunes results when exceeding `MaxListSize` (default ~1.9B elements)
- Can be cleared and reused with `.Clear()`

### Stream hub style usage example

Stream hub style uses the observer pattern where multiple indicators can subscribe to a central `QuoteHub`. This provides coordinated real-time updates for live data feeds and WebSocket integration.

```csharp
using Skender.Stock.Indicators;

[..]

// create quote hub and subscribe indicators
QuoteHub<Quote> quoteHub = new();
SmaHub<Quote> smaHub = quoteHub.ToSma(20);
RsiHub<Quote> rsiHub = quoteHub.ToRsi(14);
MacdHub<Quote> macdHub = quoteHub.ToMacd();

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

**Key features:**

- Observable pattern with hub-observer architecture
- Single quote update propagates to all subscribed indicators
- Supports state management and rollback for late-arriving data
- Indicators can be chained: `quoteHub.ToEma(20).ToRsi(14)`
- Optimized for low-latency real-time scenarios
- Results accessible via `.Results` property

See [individual indicator pages](/indicators) for specific usage guidance.

More examples available:

- [Example usage code](/examples/) on GitHub
- [Demo site](https://charts.stockindicators.dev) (a stock chart)

## Historical quotes

You must provide historical price quotes to the library in the standard OHLCV `IReadOnlyList<Quote>` or a compatible `List` or `ICollection` format.  It should have a consistent period frequency (day, hour, minute, etc).  See [using custom quote classes](#using-custom-quote-classes) if you prefer to use your own quote class.

| name        | type     | notes       |
| ----------- | -------- | ----------- |
| `Timestamp` | DateTime | Close date  |
| `Open`      | decimal  | Open price  |
| `High`      | decimal  | High price  |
| `Low`       | decimal  | Low price   |
| `Close`     | decimal  | Close price |
| `Volume`    | decimal  | Volume      |

### Where can I get historical quote data?

There are many places to get financial market data.  Check with your brokerage or other commercial sites.  If you're looking for a free developer API, see our ongoing [discussion on market data](https://github.com/DaveSkender/Stock.Indicators/discussions/579) for ideas.

### How much historical quote data do I need?

Each indicator will need different amounts of price `quotes` to calculate.  You can find guidance on the individual indicator documentation pages for minimum requirements; however, **most use cases will require that you provide more than the minimum**.  As a general rule of thumb, you will be safe if you provide 750 points of historical quote data (e.g. 3 years of daily data).

::: warning 🚩 IMPORTANT
Applying the _minimum_ amount of quote history as possible is NOT a good way to optimize your system. Some indicators use a smoothing technique that converges to better precision over time. While you can calculate these with the minimum amount of quote data, the precision to two decimal points often requires 250 or more preceding historical records.

For example, if you are using daily data and want one year of precise EMA(250) data, you need to provide 3 years of historical quotes (1 extra year for the lookback period and 1 extra year for convergence); thereafter, you would discard or not use the first two years of results. Occasionally, even more is required for optimal precision.

See [discussion on warmup and convergence](https://github.com/DaveSkender/Stock.Indicators/discussions/688) for more information.
:::

### Using custom quote classes

If you would like to use your own custom `MyCustomQuote` class, to avoid needing to transpose into the library `Quote` class, you only need to add the `IQuote` interface.

```csharp
using Skender.Stock.Indicators;

[..]

public record MyCustomQuote : IQuote
{
    // required base properties
    public DateTime Timestamp { get; set; }
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    public decimal Volume { get; set; }

    // custom properties
    public int MyOtherProperty { get; set; }
}
```

```csharp
// fetch historical quotes from your favorite feed
IReadOnlyList<MyCustomQuote> myQuotes = GetQuotesFromFeed("MSFT");

// example: get 20-period simple moving average
IReadOnlyList<SmaResult> results = myQuotes.ToSma(20);
```

::: warning Custom quotes must have value based equality
When implementing your custom quote type, it must be either `record` class or implement `IEquality` to be compatible with streaming hubs
:::

#### Using custom quote property names

If you have a model that has different properties names, but the same meaning, you only need to map them.  For example, if your class has a property called `CloseDate` instead of `Timestamp`, it could be represented like this:

```csharp
// if using record type
public record class MyCustomQuote : IQuote
{
    // redirect required base properties
    // with your custom properties
    public DateTime Timestamp => CloseDate;
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public decimal Close { get; set; }
    decimal IQuote.Volume => Vol;

    // custom properties
    public int MyOtherProperty { get; set; }
    public DateTime CloseDate { get; set; }
    public decimal Vol { get; set; }
}
```

Note the use of explicit interface (property declaration is `ISeries.Timestamp`), this is because having two properties that expose the same information can be confusing, this way `Timestamp` property is only accessible when working with the included `Quote` type, while if you are working with a `MyCustomQuote` the `Timestamp` property will be hidden, avoiding confusion.

For more information on explicit interfaces, refer to the [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/interfaces/explicit-interface-implementation).

## Chaining: indicator of indicators

If you want to compute an indicator of indicators, such as an SMA of an ADX or an [RSI of an OBV](https://medium.com/@robswc/this-is-what-happens-when-you-combine-the-obv-and-rsi-indicators-6616d991773d), use _**chaining**_ to calculate an indicator from prior results.
Example:

```csharp
// fetch historical quotes from your feed (your method)
IReadOnlyList<Quote> quotes = GetQuotesFromFeed("SPY");

// calculate RSI of OBV
IReadOnlyList<RsiResult> results
  = quotes
    .ToObv()
    .ToRsi(14);

// or with two separate operations
IReadOnlyList<ObvResult> obvResults = quotes.ToObv();
IReadOnlyList<RsiResult> rsiOfObv = obvResults.ToRsi(14);
```

## Candlestick patterns

[Candlestick Patterns](/indicators/Doji) are a unique form of indicator and have a common output model.

### Match

When a candlestick pattern is recognized, it produces a matching signal.  In some cases, an intrinsic confirmation is also available after the signal.  In cases where previous bars were used to identify a pattern, they are indicated as the basis for the signal.  This `enum` can also be referenced as an `int` value.  [Documentation for each candlestick pattern](/indicators/Doji) will indicate whether confirmation and/or basis information is produced.

| type                  |  int | description                         |
| --------------------- | ---: | ----------------------------------- |
| `Match.BullConfirmed` |  200 | Confirmation of a prior bull signal |
| `Match.BullSignal`    |  100 | Bullish signal                      |
| `Match.BullBasis`     |   10 | Bars supporting a bullish signal    |
| `Match.Neutral`       |    1 | Signal for non-directional patterns |
| `Match.None`          |    0 | No match                            |
| `Match.BearBasis`     |  -10 | Bars supporting a bearish signal    |
| `Match.BearSignal`    | -100 | Bearish signal                      |
| `Match.BearConfirmed` | -200 | Confirmation of a prior bear signal |

### Candle

The `CandleProperties` class is an extended version of `Quote`, and contains additional calculated properties.  `TQuote` classes can be converted to `CandleProperties` with the `.ToCandle()` [utility](/utilities/quotes#extended-candle-properties), and further used as the basis for calculating indicators.

## Incremental buffer style indicators

Buffer list style indicators provide efficient incremental processing for growing datasets. Use this style when you need to add data points one at a time without the overhead of a full hub infrastructure.

### When to use Buffer lists

**Ideal for:**

- Building up historical data incrementally
- Processing data feeds where quotes arrive sequentially
- Self-managed incremental calculations
- Scenarios where you don't need multi-indicator coordination
- Memory-efficient processing with auto-pruning

**Not ideal for:**

- Complete historical datasets (use Series style instead)
- Multiple indicators needing coordinated updates (use StreamHub instead)
- One-time batch calculations (use Series style instead)

### Buffer list implementation pattern

```csharp
// Create buffer list with parameters
{IndicatorName}List indicatorList = new(lookbackPeriods);

// Add quotes incrementally
foreach (IQuote quote in quotes)
{
    indicatorList.Add(quote);
}

// Access results as ICollection
IReadOnlyList<{IndicatorName}Result> results = indicatorList;

// Or get latest value
{IndicatorName}Result latest = indicatorList.LastOrDefault();

// Clear and reuse if needed
indicatorList.Clear();
```

### Memory management

Buffer lists automatically manage memory with the `MaxListSize` property (default ~1.9B elements). When the list exceeds this size, older results are automatically pruned. You can customize this behavior:

```csharp
SmaList smaList = new(20)
{
    MaxListSize = 1000  // Keep only last 1000 results
};
```

### Buffer list performance characteristics

- **Overhead**: ~10-20% slower than Series style for the same dataset
- **Memory**: Maintains internal buffers for lookback periods
- **Latency**: Optimized for incremental updates, O(1) or O(log n) per quote

See individual indicator documentation for specific examples.

## Streaming hub style indicators

Stream hub style provides real-time processing with observable patterns and state management. Multiple indicators can subscribe to a single `QuoteHub` for coordinated updates.

### When to use Stream hubs

**Ideal for:**

- Live data feeds and WebSocket integration
- Multiple indicators requiring synchronized updates
- Trading applications with low-latency requirements
- Real-time dashboards and monitoring
- Complex event-driven architectures

**Not ideal for:**

- One-time historical analysis (use Series style instead)
- Simple incremental processing (use Buffer lists instead)
- Scenarios without real-time requirements

### Stream hub implementation pattern

```csharp
// Create quote hub
QuoteHub<Quote> quoteHub = new();

// Subscribe indicators (observers)
{IndicatorName}Hub<Quote> hub1 = quoteHub.To{IndicatorName}(params);
{IndicatorName}Hub<Quote> hub2 = quoteHub.To{IndicatorName}(params);

// Stream quotes
foreach (Quote quote in liveQuotes)
{
    quoteHub.Add(quote);  // Propagates to all observers
    
    // Access results
    var result1 = hub1.Results.LastOrDefault();
    var result2 = hub2.Results.LastOrDefault();
}
```

### Chaining indicators

Stream hubs support indicator chaining for derived indicators:

```csharp
QuoteHub<Quote> quoteHub = new();

// Chain RSI from EMA
EmaHub<Quote> emaHub = quoteHub.ToEma(20);
RsiHub<Quote> rsiHub = emaHub.ToRsi(14);  // RSI of EMA

// Or chain directly
RsiHub<Quote> rsiOfEma = quoteHub.ToEma(20).ToRsi(14);
```

### State management and rollback

Stream hubs support late-arriving data and corrections:

```csharp
QuoteHub<Quote> quoteHub = new();
SmaHub<Quote> smaHub = quoteHub.ToSma(20);

// Add quotes
quoteHub.Add(quote1);
quoteHub.Add(quote2);

// Late-arriving data with earlier timestamp
quoteHub.Insert(lateQuote);  // Triggers recalculation

// Remove incorrect quote
quoteHub.Remove(badQuote);   // Triggers recalculation
```

### Stream hub performance characteristics

- **Overhead**: ~20-30% slower than Series style for the same dataset
- **Memory**: Maintains cache and state for all subscribed indicators
- **Latency**: Optimized for real-time per-quote updates, typically <1ms per quote
- **Scalability**: Supports multiple concurrent observers with single propagation

See individual indicator documentation for specific streaming examples.

## Thread safety and concurrency

Understanding thread-safety is critical when working with streaming indicators and live data feeds. This section clarifies the library's threading model and provides guidance on safely integrating with real-time data sources.

### Library threading model

**Stock.Indicators processes stream events serially** - one event at a time. The library is designed for single-threaded sequential processing and is **not thread-safe by default**.

- **Series style**: Results are immutable and thread-safe; calculations themselves are not thread-safe
- **Buffer lists**: Not thread-safe; synchronize external access if sharing across threads
- **Stream hubs**: Not thread-safe; designed for single-threaded inputs like WebSocket/SSE

A `StreamHub`, `QuoteHub`, or `BufferList` instance reads one quote at a time and is **not meant to be shared across threads** without external synchronization.

### Why streaming is often single-threaded

Thread-safety limitations in streaming contexts are normal and expected throughout the .NET ecosystem and common data feed technologies:

**Common streaming technologies:**

- **WebSocket connections**: Typically maintain a single connection with sequential message delivery. The underlying TCP stream is inherently sequential.
- **Server-Sent Events (SSE)**: HTTP-based event streams deliver events one at a time over a single connection.
- **Stock quote feeds** (IEX, Alpaca, Interactive Brokers, etc.): Most real-time market data APIs deliver quotes sequentially over WebSocket or similar connections.
- **SignalR hubs**: .NET's real-time framework delivers messages to hub methods serially by default.
- **gRPC streams**: Bidirectional streaming maintains message order within a single stream.

**Why this design is optimal:**

- **Order preservation**: Financial calculations require chronological processing; parallel processing would require complex synchronization to maintain order
- **Performance**: Serial processing eliminates lock contention and coordination overhead for the common case
- **Simplicity**: Most streaming data sources are inherently sequential; the library matches this natural flow

::: details read more: How common is thread safety in .NET asynchronous environments?

Common real‑time technologies in .NET are built around asynchronous I/O, but most of them are **not inherently thread‑safe**.  Each framework has its own rules for how concurrent access is allowed:

- **SignalR:** A SignalR server processes many requests concurrently.  The Microsoft Q&A documentation notes that if you have shared state in your hub, **you must make access to that state thread‑safe** by locking around the shared resource.  Splitting code across multiple hubs doesn’t change this, because the underlying threads can still access the same objects.  Also, `HubConnection` objects on the client are not thread‑safe; instance members should not be called from multiple threads at once.  For safe broadcasting, queue messages and use a single sending loop for each connection rather than firing `SendAsync` from several tasks concurrently.

- **WebSockets:** The underlying `ClientWebSocket` class allows **only one send and one receive** to be in progress at a time.  The official API docs say that one send and one receive may run in parallel, but issuing multiple sends or multiple receives concurrently “is not supported and will result in undefined behaviour”.  If you need to send messages from multiple producers, serialize calls to `SendAsync` (for example, via a `ConcurrentQueue` and a dedicated sender task).

- **Server‑Sent Events (SSE):** SSE streams are unidirectional and typically implemented by returning an `IAsyncEnumerable<T>` or reading from a `StreamReader`.  A `StreamReader` is **not thread‑safe by default**.  If multiple threads need to read from the same stream, wrap it using `TextReader.Synchronized` or provide each consumer with its own reader.  In most SSE patterns, only one enumeration reads the stream, so events are delivered serially and no additional locking is needed.  If you share a single event source among multiple clients, protect shared buffers with thread‑safe collections such as `BlockingCollection<T>`.

- **Popular market‑data providers:** Many third‑party libraries use WebSocket connections under the hood.  Some, like JKorf’s `Binance.Net`/`CryptoExchange.Net`, document that only one subscriber should read a given WebSocket stream at a time, and their classes are not guaranteed to be thread‑safe.  Use separate client instances per subscription or consult the library’s documentation for concurrency guidelines.  When consuming data from these libraries, apply the same WebSocket rules above—queue outbound messages and avoid simultaneous sends or receives on the same socket.

**Summary:** Real‑time components in .NET are designed for asynchronous I/O but not for free‑form multithreaded access.  Treat hubs, WebSocket clients and SSE stream readers as single‑consumer objects.  Protect shared state with locks or concurrent collections, and queue messages so that only one send or receive call is active at a time.  If you need to broadcast to multiple consumers, create separate connections or use thread‑safe collections to manage shared data.

:::

### Idiomatic usage patterns (no locks needed)

#### WebSocket example with serial processing

```csharp
using System.Net.WebSockets;
using Skender.Stock.Indicators;

QuoteHub<Quote> quoteHub = new();
SmaHub<Quote> smaHub = quoteHub.ToSma(20);

async Task ProcessWebSocketStream(ClientWebSocket ws, string uri)
{
    await ws.ConnectAsync(new Uri(uri), CancellationToken.None);
    
    byte[] buffer = new byte[4096];
    
    while (ws.State == WebSocketState.Open)
    {
        var result = await ws.ReceiveAsync(
            new ArraySegment<byte>(buffer), 
            CancellationToken.None);
        
        if (result.MessageType == WebSocketMessageType.Text)
        {
            // parse quote from JSON message (your method)
            Quote quote = ParseQuoteFromJson(buffer, result.Count);
            
            // process serially - no locks needed
            quoteHub.Add(quote);
            
            var latest = smaHub.Results.LastOrDefault();
            // use results
        }
    }
}
```

#### Server-Sent Events (SSE) example

```csharp
using Skender.Stock.Indicators;

QuoteHub<Quote> quoteHub = new();
RsiHub<Quote> rsiHub = quoteHub.ToRsi(14);

async Task ProcessSSEStream(string sseEndpoint)
{
    using var client = new HttpClient();
    using var stream = await client.GetStreamAsync(sseEndpoint);
    using var reader = new StreamReader(stream);
    
    while (!reader.EndOfStream)
    {
        string? line = await reader.ReadLineAsync();
        
        if (line?.StartsWith("data:") == true)
        {
            // parse quote from SSE data line (your method)
            Quote quote = ParseQuoteFromSSE(line);
            
            // process serially
            quoteHub.Add(quote);
            
            var latest = rsiHub.Results.LastOrDefault();
            // use results
        }
    }
}
```

#### Multiple independent streams

If you have multiple indicators that **don't need synchronization**, create separate instances per thread:

```csharp
// thread 1: processes MSFT
Task.Run(() => {
    QuoteHub<Quote> msftHub = new();
    SmaHub<Quote> msftSma = msftHub.ToSma(20);
    
    foreach (var quote in msftStream)
    {
        msftHub.Add(quote);  // no locks needed
    }
});

// thread 2: processes AAPL
Task.Run(() => {
    QuoteHub<Quote> aaplHub = new();
    SmaHub<Quote> aaplSma = aaplHub.ToSma(20);
    
    foreach (var quote in aaplStream)
    {
        aaplHub.Add(quote);  // no locks needed
    }
});
```

### Troubleshooting concurrent access

If your application architecture requires sharing a hub instance across threads or processing quotes from multiple concurrent sources, you'll need to add external synchronization. This section shows how to diagnose and fix concurrency issues.

**Common symptoms:**

- Inconsistent or corrupted results
- `InvalidOperationException` from collection modification
- Race conditions or unexpected behavior
- Debugging shows multiple threads accessing the same hub

#### Using locks for shared access

If you must share a hub across threads, use a dedicated lock object:

```csharp
using Skender.Stock.Indicators;

QuoteHub<Quote> quoteHub = new();
SmaHub<Quote> smaHub = quoteHub.ToSma(20);

// dedicated lock object (never lock on quoteHub directly)
private readonly object _hubLock = new();

// thread 1: processing quotes
void ProcessQuotes(Quote quote)
{
    lock (_hubLock)
    {
        quoteHub.Add(quote);
    }
}

// thread 2: reading results
void ReadResults()
{
    lock (_hubLock)
    {
        var latest = smaHub.Results.LastOrDefault();
        // use latest result
    }
}
```

::: warning Never lock on the hub instance directly
Do NOT use `lock (quoteHub)` - always create a dedicated `private readonly object` for locking. Locking on public instances can cause deadlocks and violates encapsulation.
:::

#### Using Channel for producer-consumer coordination

For high-throughput scenarios where you need to decouple quote reception from processing, use `System.Threading.Channels`:

```csharp
using System.Threading.Channels;
using Skender.Stock.Indicators;

Channel<Quote> quoteChannel = Channel.CreateUnbounded<Quote>();

// producer thread: receives quotes from WebSocket
async Task ProduceQuotes(WebSocket ws)
{
    while (ws.State == WebSocketState.Open)
    {
        Quote quote = await ReceiveQuoteFromWebSocket(ws);
        await quoteChannel.Writer.WriteAsync(quote);
    }
    
    quoteChannel.Writer.Complete();
}

// consumer thread: processes quotes through indicators
async Task ConsumeQuotes()
{
    QuoteHub<Quote> quoteHub = new();
    SmaHub<Quote> smaHub = quoteHub.ToSma(20);
    
    await foreach (Quote quote in quoteChannel.Reader.ReadAllAsync())
    {
        quoteHub.Add(quote);  // serial processing
        
        var latest = smaHub.Results.LastOrDefault();
        // use results
    }
}

// start both tasks
await Task.WhenAll(
    ProduceQuotes(websocket),
    ConsumeQuotes()
);
```

### When to use external synchronization

**You need locks or channels when:**

- Sharing a single hub instance across multiple threads
- Multiple threads call `.Add()`, `.Insert()`, or `.Remove()` on the same hub
- Reading results from one thread while another thread updates the hub
- Processing quotes from multiple concurrent sources into one hub

**You DON'T need locks when:**

- Processing a single sequential stream (WebSocket, SSE, file reading)
- Each thread has its own independent hub instances
- Using Series style for batch calculations (results are immutable)
- Reading from BufferList or StreamHub from the same thread that updates it

### Performance considerations

- **Locking overhead**: For high-frequency updates (>10k quotes/sec), channel-based coordination typically outperforms simple locks
- **Lock contention**: Minimize lock hold time; read results outside the lock if possible
- **False sharing**: If running multiple independent hubs, ensure they don't share cache lines

See [Performance](/performance) for detailed benchmarks and optimization guidance.

## Utilities

See [Utilities and helper functions](/utilities/) for additional tools.
