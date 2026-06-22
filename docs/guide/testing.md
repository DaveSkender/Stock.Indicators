---
title: Testing your analysis code
description: How to write automated tests for trading and technical-analysis code that consumes Stock Indicators — signals, confluence rules, backtests, and live alerts.
---

# Testing your analysis code

This guide is for the **technical analyst or trading-system developer** writing tests for *their own* code on top of Stock Indicators — signal generators, entry/exit rules, screeners, backtests, and live alerts.

You don't need to test the indicators. The library already proves that `ToRsi(14)` returns the right numbers and that every output style agrees. Your tests should prove **your trading logic**: that a golden cross fires a buy, that an oversold reading inside an uptrend triggers an entry, that your alert raises at the right moment.

Two habits make that easy:

1. **Keep each rule a pure function of indicator results.** Then a test is just *"given these values, expect this signal."*
2. **Feed it real result objects — never a mock.** Indicators are deterministic; constructing an `RsiResult` with the value you want is the real type your code receives.

## Make your trading rules testable

Pull each decision into a small function that takes indicator results and returns a signal. It becomes trivially testable — no data feed, no mocking framework.

### Moving-average crossover

A classic entry/exit rule: buy when a fast average crosses above a slow one, sell on the reverse.

```csharp
public enum Signal { None, Buy, Sell }

// buy on an up-cross, sell on a down-cross
public static Signal Crossover(
    SmaResult fastPrev, SmaResult fastNow,
    SmaResult slowPrev, SmaResult slowNow)
{
    if (fastPrev.Sma is null || slowPrev.Sma is null
     || fastNow.Sma is null || slowNow.Sma is null)
    {
        return Signal.None;   // still warming up
    }

    bool wasBelow = fastPrev.Sma <= slowPrev.Sma;
    bool isAbove = fastNow.Sma > slowNow.Sma;

    return (wasBelow, isAbove) switch {
        (true, true) => Signal.Buy,     // crossed up
        (false, false) => Signal.Sell,  // crossed down
        _ => Signal.None
    };
}
```

```csharp
[TestMethod]
public void Crossover_FastRisesAboveSlow_IsBuy()
{
    DateTime t = default;  // timestamps don't affect the rule

    Signal signal = Strategy.Crossover(
        fastPrev: new(t, 99), fastNow: new(t, 101),
        slowPrev: new(t, 100), slowNow: new(t, 100));

    signal.Should().Be(Signal.Buy);
}
```

### Multi-indicator confluence

Real strategies combine indicators. Here: only enter long when momentum is oversold **and** price sits above its long-term trend.

```csharp
public static bool ShouldEnterLong(IBar bar, RsiResult rsi, SmaResult trend)
    => rsi.Rsi <= 30
    && trend.Sma is not null
    && (double)bar.Close > trend.Sma;
```

```csharp
[TestMethod]
public void ShouldEnterLong_OversoldInUptrend_IsTrue()
{
    DateTime t = default;
    IBar bar = new Bar(t, Open: 104, High: 106, Low: 103, Close: 105, Volume: 0);

    bool enter = Strategy.ShouldEnterLong(
        bar,
        rsi: new(t, Rsi: 25),       // oversold
        trend: new(t, Sma: 100));   // price (105) is above trend

    enter.Should().BeTrue();
}
```

Test the *near misses* too — an oversold reading **below** the trend line should not enter. Those are the cases real money depends on.

## Backtesting over historical bars

For end-to-end coverage, run the indicators over a fixed set of bars and assert on **your** output — the signals and metrics your code produces, not the indicator values:

```csharp
[TestMethod]
public void GoldenCrossBacktest_SignalsAlternate()
{
    IReadOnlyList<Bar> bars = TestData.DailyBars();

    IReadOnlyList<SmaResult> fast = bars.ToSma(50);
    IReadOnlyList<SmaResult> slow = bars.ToSma(200);

    List<Signal> signals = [];
    for (int i = 1; i < bars.Count; i++)
    {
        Signal s = Strategy.Crossover(fast[i - 1], fast[i], slow[i - 1], slow[i]);
        if (s != Signal.None) { signals.Add(s); }
    }

    // a real invariant of crossovers: you can't buy twice without selling between
    signals.Should().NotBeEmpty();
    for (int i = 1; i < signals.Count; i++)
    {
        signals[i].Should().NotBe(signals[i - 1]);
    }
}
```

Assert on properties that hold for *any* representative data — signal counts, alternation, that a backtest's net result is finite — rather than hard-coding values tied to one dataset.

## Live streaming alerts

If your code reacts to a live feed, wire your observer to a hub and replay a fixture as the "live" stream, then assert on the side effects your code produced. Here `RsiAlertObserver` is your own `IStreamObserver<RsiResult>` that invokes `onAlert` whenever `Rsi <= threshold`; see [Custom observers](/guide/custom-observers) for that pattern.

```csharp
[TestMethod]
public void Alerting_OnStreamedBars_FiresOnlyWhenOversold()
{
    IReadOnlyList<Bar> bars = TestData.DailyBars();

    BarHub barHub = new();
    RsiHub rsiHub = barHub.ToRsiHub(14);

    List<RsiResult> alerts = [];
    using var observer = new RsiAlertObserver(rsiHub, threshold: 30, onAlert: alerts.Add);

    foreach (Bar bar in bars) { barHub.Add(bar); }   // replay the feed

    alerts.Should().OnlyContain(r => r.Rsi <= 30);

    barHub.EndTransmission();
}
```

You can also test the observer with no hub at all — call its `OnAdd(...)` directly with constructed results and assert it raised the right alert. That isolates your reaction logic completely.

## Don't mock the indicators

Avoid mocking the result types or hiding indicators behind a wrapper interface just to substitute them:

- **They're pure functions.** `bars.ToEma(20)` has no I/O, clock, or randomness. A mock just returns whatever you hardcode — it agrees with itself even when your logic is wrong.
- **Mocks drift from reality.** If a future version changes a warmup default or rounding, mocked tests keep passing while production diverges. Tests built on real results catch it.

Inject the **result**, not a mockable service. The right seam is between your logic and the indicator *output*, which is exactly what the pure-function rules above use.

## When a wrapper helps

A thin wrapper is warranted for **data-source abstraction**, not mocking — for example, swapping live, backtest, and replay feeds behind one interface:

```csharp
public interface IMarketDataSource
{
    IReadOnlyList<IBar> GetBars(string symbol, DateTime from, DateTime to);
}
```

This abstracts *where bars come from* (a real concern); it does not abstract the indicator (which doesn't need it). Tests then run real indicator calls against a CSV-backed source.

## Where to get bars for tests

Keep a small fixture — a CSV or JSON of 100–500 representative bars — and load it once:

```csharp
internal static class TestData
{
    private static readonly Lazy<IReadOnlyList<Bar>> _daily = new(
        () => LoadCsv("Data/sp500-daily.csv"));  // return bars sorted by Timestamp

    public static IReadOnlyList<Bar> DailyBars() => _daily.Value;
}
```

Need a starting fixture? The library ships representative daily data you can copy from `tests/indicators/_testdata/quotes/default.csv` (~502 S&P 500 bars), and the [example projects](/examples/) include downloadable sample quote files.

## See also

- [Example usage code](/examples/) — complete working console, backtest, and streaming projects
- [Custom observers](/guide/custom-observers) — implementing `IStreamObserver<T>` for live integration
- [Custom indicators](/guide/customization) — adding indicator math, not consuming output
