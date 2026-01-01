# Interface selection

## Decision tree

```text
Can indicator work with single chainable IReusable values?
├─ Yes → IIncrementFromChain (SMA, EMA, RSI, MACD)
└─ No → IIncrementFromQuote (Stoch, ATR, VWAP, ADX)
         Only supports IQuote inputs
```

## IIncrementFromChain

**Critical rules**:

- ✅ Constructor accepts `IReadOnlyList<IReusable>` (NOT `IQuote`)
- ✅ Extension uses generic constraint with `IReusable`
- ✅ Most indicators use `value.Value` directly
- ✅ Some indicators use `value.Hl2OrValue()` for price (e.g., Alligator)
- ❌ No `Add(IQuote)` methods in this interface

**Examples**: SMA, EMA, RSI, MACD, TEMA, DEMA, Trix

## IIncrementFromQuote

**Critical rules**:

- ✅ Constructor accepts `IReadOnlyList<IQuote>` (standard pattern)
- ✅ Extension uses `IQuote` constraint only
- ✅ No chainable `Add(IReusable)` methods

**Examples**: Stochastic, ATR, ADX, VWAP, Chandelier, Aroon

## Test interface mapping

| Buffer Interface | Test Interface |
| ---------------- | -------------- |
| `IIncrementFromChain` | `ITestChainBufferList` |
| `IIncrementFromQuote` | `ITestQuoteBufferList` |

Additional test interface for custom caches:

- `ITestCustomBufferListCache` - When using `List<T>` instead of `Queue<T>`

---
Last updated: December 31, 2025
