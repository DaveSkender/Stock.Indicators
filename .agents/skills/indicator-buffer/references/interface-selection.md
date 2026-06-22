# Interface selection

## Decision tree

```text
Can indicator work with single chainable IReusable values?
├─ Yes → IIncrementFromChain (SMA, EMA, RSI, MACD)
└─ No → IIncrementFromBar (Stoch, ATR, VWAP, ADX)
         Only supports IBar inputs
```

## IIncrementFromChain

**Critical rules**:

- ✅ Constructor accepts `IReadOnlyList<IReusable>` (NOT `IBar`)
- ✅ Extension uses generic constraint with `IReusable`
- ✅ Most indicators use `value.Value` directly
- ✅ Some indicators use `value.Hl2OrValue()` for price (e.g., Alligator)
- ❌ No `Add(IBar)` methods in this interface

**Examples**: SMA, EMA, RSI, MACD, TEMA, DEMA, Trix

## IIncrementFromBar

**Critical rules**:

- ✅ Constructor accepts `IReadOnlyList<IBar>` (standard pattern)
- ✅ Extension uses `IBar` constraint only
- ✅ No chainable `Add(IReusable)` methods

**Examples**: Stochastic, ATR, ADX, VWAP, Chandelier, Aroon

## Test interface mapping

| Buffer Interface | Test Interface |
| ---------------- | -------------- |
| `IIncrementFromChain` | `ITestChainBufferList` |
| `IIncrementFromBar` | `ITestBarBufferList` |

Additional test interface for custom caches:

- `ITestCustomBufferListCache` — when using `List<T>` instead of `Queue<T>`
