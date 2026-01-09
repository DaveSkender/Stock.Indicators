# Provider selection

## Provider classification

| Provider Base | Input | Output | Use Case | Examples |
| ------------- | ----- | ------ | -------- | -------- |
| `ChainHub<IReusable, TResult>` | Single value | `IReusable` | Chainable indicators | EMA, SMA, RSI, MACD |
| `ChainHub<IQuote, TResult>` | OHLCV | `IReusable` | Quote-driven, chainable output | ADX, ATR, CCI, OBV |
| `QuoteProvider<IQuote, TResult>` | OHLCV | `IResult` | Quote transformation | HeikinAshi, Renko |

## ChainHub<IReusable, TResult>

**Most common pattern** (~45 indicators): Chainable input and output, supports EMA → RSI → SMA chains

**Examples**: EMA, SMA, RSI, MACD, Trix, TSI, TEMA, DEMA

```csharp
public class EmaHub : ChainHub<IReusable, EmaResult>, IEma
{
    internal EmaHub(IChainProvider<IReusable> provider, int lookbackPeriods)
        : base(provider)
    {
        Ema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Reinitialize();
    }
}
```

## ChainHub<IQuote, TResult>

**Quote-driven input, chainable output** (~15 indicators): Requires OHLCV data, produces single chainable `IReusable` output

**Examples**: ADX, ATR, Aroon, CCI, CMF, OBV

```csharp
public class AdxHub : ChainHub<IQuote, AdxResult>, IAdx
{
    // Requires quote data, produces chainable AdxResult
}
```

## QuoteProvider<IQuote, TResult>

**Quote transformation** (~3 indicators): Quote input transforms to quote-like output (`IResult`, not necessarily `IQuote`)

**Examples**: HeikinAshi, Renko, QuoteHub

```csharp
public class HeikinAshiHub : QuoteProvider<IQuote, HeikinAshiResult>
{
    // Quote in, quote-like result out
}
```

## Test interface mapping

| Provider Base | Observer Interface | Provider Interface |
| ------------- | ------------------ | ------------------ |
| `ChainHub<IReusable, T>` | `ITestChainObserver` | `ITestChainProvider` |
| `ChainHub<IQuote, T>` | `ITestChainObserver` | `ITestChainProvider` |
| `QuoteProvider<IQuote, T>` | `ITestQuoteObserver` | `ITestChainProvider` |

---
Last updated: December 31, 2025
