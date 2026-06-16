---
title: Testing consumers of Stock Indicators
description: Recommended patterns for testing code that consumes Stock Indicators — when to use canned fixtures, when to wrap, and why we don't recommend mocking.
---

# Testing consumers of Stock Indicators

This guide is for developers writing **automated tests for application code that consumes Stock Indicators** — trading systems, analytics services, dashboards, alerting pipelines, etc. It is not about testing indicators themselves; the library's own test suite handles that.

The short version:

- **Use canned `IQuote` fixtures and assert against known indicator values.** This is the same pattern the library's own test suite uses, and it gives the strongest correctness signal for the lowest maintenance cost.
- **Don't mock the indicator types.** Indicators are deterministic pure functions of their inputs; mocking a deterministic function adds no signal.
- **Wrap our types behind your own interface only when your service layer genuinely needs that abstraction** — for example, when you need to swap between live, backtest, and replay data sources.

## Recommended pattern: canned fixtures + known results

Indicators in this library are deterministic: given the same input quotes and parameters, you get the same output values to within `double` precision. The cleanest way to test code that consumes those outputs is to:

1. Build (or check in) a small, fixed `IReadOnlyList<IQuote>` fixture.
2. Run the indicator to get the canonical results.
3. Assert your consumer code does the right thing with those results.

```csharp
using FluentAssertions;
using Skender.Stock.Indicators;

[TestMethod]
public void StrategyEnters_WhenRsiCrossesBelow30()
{
    // 1. canned input fixture (replace with your own)
    IReadOnlyList<Quote> quotes = CannedQuotes.OneMonthOfMsft();

    // 2. compute the indicator the consumer depends on
    IReadOnlyList<RsiResult> rsi = quotes.ToRsi(14);

    // 3. exercise consumer logic
    var strategy = new MeanReversionStrategy();
    StrategyDecision decision = strategy.Decide(quotes[^1], rsi[^1]);

    decision.Action.Should().Be(StrategyAction.Enter);
    decision.Reason.Should().Be("RSI(14) crossed below 30");
}
```

This is exactly the shape of the library's own indicator tests (see e.g. `tests/indicators/m-r/Rsi/Rsi.StaticSeries.Tests.cs`). It validates the *interaction* between your code and the real indicator output, which is what production will actually run.

### Where to source canned quotes

Most consumer test suites end up keeping their own `IQuote` fixtures — typically a CSV or JSON file checked into the test project with 100–500 bars of representative market data. The library's test suite uses `tests/indicators/_testdata/quotes/default.csv` (502 bars of historical S&P 500 daily quotes) as its canonical fixture; you can mirror that approach or use your own production-representative sample.

A small helper class loads the CSV once per test class:

```csharp
internal static class CannedQuotes
{
    private static readonly Lazy<IReadOnlyList<Quote>> _oneMonth = new(
        () => LoadCsv("Data/msft-1mo.csv"));

    public static IReadOnlyList<Quote> OneMonthOfMsft() => _oneMonth.Value;

    private static IReadOnlyList<Quote> LoadCsv(string relativePath)
    {
        // your CSV loader — return List<Quote> sorted by Timestamp ascending
        ...
    }
}
```

## Why we don't recommend mocking the indicator types

A common instinct is to mock `IReadOnlyList<EmaResult>` or to introduce a wrapper interface so the indicator can be substituted in tests. We recommend against this for indicator-output consumption:

- **The indicators are pure functions.** `quotes.ToEma(20)` has no I/O, no clock, no randomness, no external dependencies. Mocking it just lets you hardcode whatever return value you want — which validates nothing about the consumer's interaction with real indicator behavior. The hardcoded mock will agree with itself in every test, even if the consumer logic is wrong.
- **Mocks drift from reality.** If a future library version corrects a calculation rounding or changes a warmup-period default, mocked tests keep passing while real production behavior diverges. Canned-fixture tests catch this immediately.
- **Per-type result interfaces (`IEmaResult`, `ISmaResult`, …) actively encourage anti-patterns.** They invite user-defined result subclasses that diverge from canonical formulas without solving any real testing problem. The library deliberately does not provide them.

The right unit-test boundary is **between your business logic and the indicator output**, not between your business logic and the indicator type. Inject the *result list*, not a mocked indicator service:

```csharp
// GOOD: inject the result list
public StrategyDecision Decide(IQuote latest, RsiResult rsi)
{
    if (rsi.Rsi is < 30) return new(StrategyAction.Enter, "RSI(14) crossed below 30");
    ...
}

// AVOID: injecting a "rsi service" wrapper just so it can be mocked
public StrategyDecision Decide(IQuote latest, IRsiService rsi)  // anti-pattern
{
    var rsiResult = rsi.Calculate(...);  // pointless layer
    ...
}
```

## When wrapping our types is appropriate

There **are** cases where a thin wrapper around the library makes sense — but the motivation is data-source abstraction, not mocking. If your service layer needs to swap between live broker quotes, backtest CSVs, and replay buffers, a `IMarketDataSource` interface that returns `IReadOnlyList<IQuote>` from each implementation is fine:

```csharp
public interface IMarketDataSource
{
    IReadOnlyList<IQuote> GetQuotes(string symbol, DateTime from, DateTime to);
}

public sealed class CsvBacktestDataSource : IMarketDataSource { ... }
public sealed class LiveBrokerDataSource  : IMarketDataSource { ... }
public sealed class ReplayDataSource      : IMarketDataSource { ... }
```

This abstracts **where the quotes come from**, which is a real concern. It does not abstract the indicator, which doesn't need abstracting. Your test code then exercises real indicator calls against the canned-CSV source:

```csharp
[TestMethod]
public void Strategy_OnTwoYearsOfMsft_HasPositiveSharpe()
{
    IMarketDataSource source = new CsvBacktestDataSource("Data/msft-2y.csv");
    var quotes = source.GetQuotes("MSFT", from, to);

    var rsi  = quotes.ToRsi(14);
    var sma  = quotes.ToSma(50);
    var decisions = strategy.RunBacktest(quotes, rsi, sma);

    decisions.SharpeRatio().Should().BeGreaterThan(0);
}
```

## Streaming-specific testing

For consumers that integrate with `StreamHub` or `BufferList`, the same canned-fixture pattern applies — feed the same fixture as a stream and assert on the observer side.

### Replaying a fixture through a stream

```csharp
[TestMethod]
public void Alerting_FiresOnRsiOversold_AcrossLiveStream()
{
    IReadOnlyList<Quote> quotes = CannedQuotes.OneMonthOfMsft();

    QuoteHub quoteHub = new();
    RsiHub rsiHub = quoteHub.ToRsiHub(14);

    var captured = new List<RsiResult>();
    using var observer = new RsiAlertObserver(rsiHub, captured.Add);

    // replay the fixture as a "live" feed
    foreach (Quote q in quotes)
    {
        quoteHub.Add(q);
    }

    captured.Should().Contain(r => r.Rsi is < 30);
}
```

### Asserting Stream/Buffer parity with Series

The library guarantees that Series, BufferList, and StreamHub produce numerically identical results for the same inputs. Consumer tests can rely on this — pick whichever style fits the test scenario (typically Series for speed, StreamHub for behavioral tests of observer wiring). When your test needs to assert your code behaves identically regardless of style, a direct equality assertion works:

```csharp
IReadOnlyList<EmaResult> series = quotes.ToEma(20);
EmaList               buffer    = quotes.ToEmaList(20);

buffer.Should().Equal(series, (b, s) => b.Timestamp == s.Timestamp && b.Ema == s.Ema);
```

This is the same parity invariant the library's own test suite asserts via `IsExactly(series)` — see e.g. `tests/indicators/e-k/Ema/Ema.BufferList.Tests.cs`.

### Testing custom observers

For consumer code that implements `IStreamObserver<T>` directly (UI dispatchers, persistence layers, alert pipelines), the same approach works — drive the source hub with a canned fixture and assert on the observer's captured side effects. See [Custom observers](/guide/custom-observers) for the observer-implementation patterns.

## See also

- [Stream hubs](/guide/styles/stream) — source-side streaming guide
- [Custom observers](/guide/custom-observers) — implementing `IStreamObserver<T>` for external integration
- [Custom indicators](/guide/customization) — adding indicator math, not consuming output
- The library's own test suite under `tests/indicators/` for representative shapes
