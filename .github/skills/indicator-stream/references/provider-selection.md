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

## StreamHub<TProviderResult, TResult>

**Compound hubs** (~2 indicators): Indicators that require an internal hub dependency

**Examples**: StochRSI (requires RSI), Gator (requires Alligator)

**Pattern**: Two constructors - one that creates the internal hub, one that accepts an existing hub:

```csharp
public class StochRsiHub : ChainHub<IReusable, StochRsiResult>
{
    // Constructor 1: Creates internal RSI hub from provider
    internal StochRsiHub(
        IChainProvider<IReusable> provider,
        int rsiPeriods = 14,
        int stochPeriods = 14)
        : this(provider.ToRsiHub(rsiPeriods), stochPeriods)  // Delegates to constructor 2
    { }

    // Constructor 2: Accepts existing RSI hub (used internally, avoids duplicate hubs)
    internal StochRsiHub(RsiHub rsiHub, int stochPeriods = 14)
        : base(rsiHub)  // Base receives the hub as provider
    {
        ArgumentNullException.ThrowIfNull(rsiHub);
        StochRsi.Validate(rsiHub.LookbackPeriods, stochPeriods);
        RsiPeriods = rsiHub.LookbackPeriods;
        StochPeriods = stochPeriods;
        Reinitialize();
    }
}
```

**Extension methods**: Provide both overloads with clear documentation:

```csharp
public static StochRsiHub ToStochRsiHub(
    this IChainProvider<IReusable> chainProvider,
    int rsiPeriods = 14,
    int stochPeriods = 14)
    => new(chainProvider, rsiPeriods, stochPeriods);

/// <summary>
/// Creates a new Stochastic RSI hub, using RSI values from an existing RSI hub.
/// </summary>
/// <remarks>
/// This extension overrides normal chaining and enables reuse of the existing
/// <see cref="RsiHub"/> in its internal construction.
/// <para>IMPORTANT: This is not a normal chaining approach.</para>
/// Do not use this if you want a StochRSI of an RSI hub.</remarks>
public static StochRsiHub ToStochRsiHub(
    this RsiHub rsiHub,
    int stochPeriods = 14)
    => new(rsiHub, stochPeriods);
```

**Key principles**:

1. **Avoid duplicate hubs** - Internal hub construction prevents redundant calculations
2. **Delegate constructors** - Constructor 1 calls constructor 2 with created hub
3. **Base receives hub** - Pass the internal hub to base class (not original provider)
4. **Document overrides** - Clearly mark the hub-accepting extension as non-standard chaining
5. **Avoid RollbackState override** - Compound hubs typically rely on their internal hub's state management; only override when maintaining additional derived state beyond what the internal hub exposes. Example: `StochRsiHub` overrides `RollbackState` to rebuild oscillator state (`_rsiMaxWindow`, `_rsiMinWindow`, `kBuffer`, `signalBuffer`) from cached RSI results. See "Compound hub state" pattern in `rollback-patterns.md` for the accepted exception and implementation pattern

## Test interface mapping

| Provider Base | Observer Interface | Provider Interface |
| ------------- | ------------------ | ------------------ |
| `ChainHub<IReusable, T>` | `ITestChainObserver` | `ITestChainProvider` |
| `ChainHub<IQuote, T>` | `ITestChainObserver` | `ITestChainProvider` |
| `QuoteProvider<IQuote, T>` | `ITestQuoteObserver` | `ITestChainProvider` |

---
Last updated: January 19, 2026
