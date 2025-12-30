# StreamHub provider selection guide

Use this reference to select the correct provider base class for StreamHub implementations.

## Provider decision tree

```text
Does indicator produce chainable output (single Value)?
├─ Yes → Does it need OHLCV data?
│  ├─ Yes → Does it need TWO synchronized series?
│  │  ├─ Yes → PairsProvider<IReusable, TResult>
│  │  └─ No → ChainProvider<IQuote, TResult>
│  └─ No → ChainProvider<IReusable, TResult>
└─ No → Does output transform quote to quote?
   ├─ Yes → QuoteProvider<IQuote, TResult>
   └─ No → StreamHub<TIn, TResult> (ISeries output)
```

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
- Single chainable output (IReusable)
- Cannot chain from other indicators
- ~15 indicators

**Examples**: ADX, ATR, Aroon, CCI, CMF, OBV

```csharp
public class AdxHub : ChainProvider<IQuote, AdxResult>, IAdx
{
    // Requires quote data but produces chainable AdxResult
}
```

## QuoteProvider<IQuote, TResult>

**Use when**: Quote input transforms to quote output

**Characteristics**:

- Quote-to-quote transformation
- Cannot observe chain providers
- Output is IQuote type
- ~3 indicators

**Examples**: QuoteHub, HeikinAshi, Renko

```csharp
public class RenkoHub : QuoteProvider<IQuote, RenkoResult>
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
