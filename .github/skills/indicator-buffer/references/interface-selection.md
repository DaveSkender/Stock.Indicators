# BufferList interface selection guide

Use this reference to select the correct interface for BufferList implementations.

## Interface overview

All BufferList implementations support `IQuote` inputs from the abstract base class. The interface determines what *additional* input types are supported for chainable scenarios.

## Interface decision tree

```text
Can indicator work with single chainable IReusable values?
├─ Yes → IIncrementFromChain (SMA, EMA, RSI, MACD)
└─ No → IIncrementFromQuote (Stoch, ATR, VWAP, ADX)
         Only supports IQuote inputs
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
- ✅ Some indicators use `value.Hl2OrValue()` for price (e.g., Alligator)
- ❌ No `Add(IQuote)` methods in this interface

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

## Test interface mapping

| Buffer Interface | Test Interface |
| ---------------- | -------------- |
| `IIncrementFromChain` | `ITestChainBufferList` |
| `IIncrementFromQuote` | `ITestQuoteBufferList` |

Additional test interface for custom caches:

- `ITestCustomBufferListCache` - When using `List<T>` instead of `Queue<T>`

**Note**: `IIncrementFromPairs` and `ITestPairsBufferList` are being removed in PR #1821 for future reintroduction with improved design.

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
Last updated: December 31, 2025
