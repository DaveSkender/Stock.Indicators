# BufferList interface selection guide

Use this reference to select the correct interface for BufferList implementations.

## Interface decision tree

```text
Does indicator need OHLCV data (High, Low, Open, Close, Volume)?
├─ Yes → Does it need TWO synchronized series?
│  ├─ Yes → IIncrementFromPairs (Correlation, Beta)
│  └─ No → IIncrementFromQuote (Stoch, ATR, VWAP, ADX)
└─ No → Can work with single chainable value?
   └─ Yes → IIncrementFromChain (SMA, EMA, RSI, MACD)
```

## IIncrementFromChain

**Required methods**:

```csharp
void Add(DateTime timestamp, double value);
void Add(IReusable value);
void Add(IReadOnlyList<IReusable> values);
```

**Critical rules**:

- ✅ Constructor accepts `IReadOnlyList<IReusable>` (NOT `IQuote`)
- ✅ Extension uses generic constraint with `IReusable`
- ✅ Most indicators use `value.Value` directly
- ✅ OHLC indicators use `value.Hl2OrValue()` for price
- ❌ No `Add(IQuote)` methods

**Examples**: SMA, EMA, RSI, MACD, TEMA, DEMA, Trix

## IIncrementFromQuote

**Required methods**:

```csharp
void Add(IQuote quote);
void Add(IReadOnlyList<IQuote> quotes);
```

**When to use**:

- Needs multiple OHLC values in calculation
- Cannot work with single `Value` property
- Volume-weighted indicators

**Examples**: Stochastic, ATR, ADX, VWAP, Chandelier, Aroon

## IIncrementFromPairs

**Required methods**:

```csharp
void Add(DateTime timestamp, double valueA, double valueB);
void Add(IReusable valueA, IReusable valueB);
void Add(IReadOnlyList<IReusable> valuesA, IReadOnlyList<IReusable> valuesB);
```

**Critical rules**:

- ✅ Constructor accepts TWO `IReadOnlyList<IReusable>` parameters
- ✅ Must validate timestamp matching between pairs
- ✅ Must validate equal list counts
- ✅ Both inputs must be synchronized

**Examples**: Correlation, Beta, PRS

## Test interface mapping

| Buffer Interface | Test Interface |
| ---------------- | -------------- |
| `IIncrementFromChain` | `ITestChainBufferList` |
| `IIncrementFromQuote` | `ITestQuoteBufferList` |
| `IIncrementFromPairs` | `ITestPairsBufferList` |

Additional test interface for custom caches:

- `ITestCustomBufferListCache` - When using `List<T>` instead of `Queue<T>`

## Buffer state patterns

**Simple running sum**:

```csharp
private double _sum;
private readonly Queue<double> _buffer;

public void Add(DateTime timestamp, double value)
{
    double? dequeued = _buffer.UpdateWithDequeue(LookbackPeriods, value);
    _sum = dequeued.HasValue ? _sum - dequeued.Value + value : _sum + value;
}
```

**Tuple-based state**:

```csharp
private readonly Queue<(double High, double Low, double Close)> _buffer;

public void Add(IQuote quote)
{
    _buffer.Update(LookbackPeriods, (quote.High, quote.Low, quote.Close));
}
```

---
Last updated: December 30, 2025
