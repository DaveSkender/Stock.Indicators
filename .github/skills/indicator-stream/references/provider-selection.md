# StreamHub provider selection guide

Provider selection is determined by the indicator specification—it's not a design choice.

## Provider classification

| Provider Base | Input | Output | Use Case | Examples |
| ------------- | ----- | ------ | -------- | -------- |
| `ChainProvider<IReusable, TResult>` | Single value | `IReusable` | Chainable indicators | EMA, SMA, RSI, MACD |
| `ChainProvider<IQuote, TResult>` | OHLCV | `IReusable` | Quote-driven, chainable output | ADX, ATR, CCI, OBV |
| `QuoteProvider<IQuote, TResult>` | OHLCV | `IResult` | Quote transformation | HeikinAshi, Renko |
| `PairsProvider<IReusable, TResult>` | Dual `IReusable` | `IReusable` | Synchronized dual inputs | Correlation, Beta |

## ChainProvider<IReusable, TResult>

**Use when**: Indicator can work with single reusable values

**Characteristics**:

- Most common pattern (~45 indicators)
- Chainable input and output
- Supports EMA → RSI → SMA chains
- Implements `IChainProvider<IReusable>`

**Examples**: EMA, SMA, RSI, MACD, Trix, TSI, TEMA, DEMA

```csharp
public class EmaHub : ChainProvider<IReusable, EmaResult>, IEma
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

## ChainProvider<IQuote, TResult>

**Use when**: Needs OHLCV data but produces chainable output

**Characteristics**:

- Quote-driven input
- Single chainable output (`IReusable`)
- Can be observed by other chain providers
- ~15 indicators

**Examples**: ADX, ATR, Aroon, CCI, CMF, OBV

```csharp
public class AdxHub : ChainProvider<IQuote, AdxResult>, IAdx
{
    // Requires quote data, produces chainable AdxResult
}
```

## QuoteProvider<IQuote, TResult>

**Use when**: Quote input transforms to quote-like output

**Characteristics**:

- Quote-to-quote transformation
- Output implements `IResult` (not necessarily `IQuote`)
- Used for quote preprocessing
- ~3 indicators

**Examples**: HeikinAshi, Renko, QuoteHub

```csharp
public class HeikinAshiHub : QuoteProvider<IQuote, HeikinAshiResult>
{
    // Quote in, quote-like result out
}
```

## PairsProvider<IReusable, TResult>

**Use when**: Needs synchronized dual inputs

**Characteristics**:

- Two synchronized reusable inputs
- Single chainable output
- Must validate timestamp matching
- ~3 indicators

**Examples**: Correlation, Beta, PRS

```csharp
public class CorrelationHub : PairsProvider<IReusable, CorrelationResult>
{
    internal CorrelationHub(
        IChainProvider<IReusable> providerA,
        IChainProvider<IReusable> providerB,
        int lookbackPeriods) : base(providerA, providerB)
    {
        ArgumentNullException.ThrowIfNull(providerB);
        Correlation.Validate(lookbackPeriods);
        Reinitialize();
    }
}
```

## Test interface mapping

| Provider Base | Observer Interface | Provider Interface |
| ------------- | ------------------ | ------------------ |
| `ChainProvider<IReusable, T>` | `ITestChainObserver` | `ITestChainProvider` |
| `ChainProvider<IQuote, T>` | `ITestChainObserver` | `ITestChainProvider` |
| `QuoteProvider<IQuote, T>` | `ITestQuoteObserver` | `ITestChainProvider` |
| `PairsProvider<IReusable, T>` | `ITestPairsObserver` | — |

---
Last updated: December 30, 2025
