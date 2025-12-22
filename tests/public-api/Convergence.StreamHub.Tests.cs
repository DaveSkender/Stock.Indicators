#nullable enable

namespace Behavioral;

/// <summary>
/// StreamHub convergence tests verify that StreamHub implementations
/// produce consistent results when streaming data incrementally vs batch processing
/// and match Series calculations at convergence point.
/// </summary>
/// <remarks>
/// Convergence tests are the only kind of test allowed to use
/// <c>BeApproximately()</c> precision instead of exact matching.
/// </remarks>
[TestClass, TestCategory("Integration")]
public class ConvergenceStreamHubs : TestBase
{
    private static readonly int[] QuotesQuantities =
        [14, 28, 40, 50, 75, 100, 150, 200, 250, 350, 500];

    // Convergence thresholds: quantity needed for stable final value
    private const int AdxConvergence = 350;
    private const int AtrConvergence = 200;
    private const int EmaConvergence = 150;
    private const int MacdConvergence = 250;
    private const int RsiConvergence = 200;
    private const int SmaConvergence = 50;
    private const int StochConvergence = 200;

    // Precision tolerance for StreamHub convergence tests (14th decimal place)
    // Required because SMMA calculations retrieve previous values from Cache
    // which have ToPrecision applied, creating minor cascading precision differences
    private const double PrecisionTolerance = 1e-13;

    /// <summary>
    /// Configures BeEquivalentTo to use approximate matching for double values.
    /// Required for StreamHub convergence tests due to minor precision differences
    /// from SMMA calculations retrieving rounded previous values from Cache.
    /// </summary>
    private static EquivalencyAssertionOptions<T> WithPrecisionTolerance<T>(EquivalencyAssertionOptions<T> options)
        => options
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, PrecisionTolerance))
            .WhenTypeIs<double>()
            .Using<double?>(ctx =>
            {
                if (ctx.Expectation.HasValue)
                {
                    ctx.Subject.Should().BeApproximately(ctx.Expectation.Value, PrecisionTolerance);
                }
                else
                {
                    ctx.Subject.Should().BeNull();
                }
            })
            .WhenTypeIs<double?>();

    [TestMethod]
    public void Adx()
    {
        const int lookback = 14;
        AdxResult? convergedResult = null;

        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            AdxHub hub = provider.ToAdxHub(lookback);

            foreach (Quote q in qts)
            {
                provider.Add(q);
            }

            AdxResult l = hub.Results[^1];
            Console.WriteLine($"ADX({lookback}) StreamHub on {l.Timestamp:d} with {qts.Count,4} streaming qts: {l.Adx:N8}");

            // At convergence point, verify matches Series (with tolerance for floating-point precision)
            if (qty == AdxConvergence)
            {
                IReadOnlyList<AdxResult> series = qts.ToAdx(lookback);
                l.Should().BeEquivalentTo(series[^1], WithPrecisionTolerance);
                convergedResult = l;
            }

            // After convergence, verify stability
            if (qty > AdxConvergence && convergedResult != null)
            {
                l.Adx.Should().BeApproximately(convergedResult.Adx!.Value, 0.0001);
            }
        }
    }

    [TestMethod]
    public void Atr()
    {
        const int lookback = 14;
        AtrResult? convergedResult = null;

        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            AtrHub hub = provider.ToAtrHub(lookback);

            foreach (Quote q in qts)
            {
                provider.Add(q);
            }

            AtrResult l = hub.Results[^1];
            Console.WriteLine($"ATR({lookback}) StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.Atr:N8}");

            // At convergence point, verify matches Series
            if (qty == AtrConvergence)
            {
                IReadOnlyList<AtrResult> series = qts.ToAtr(lookback);
                l.Should().BeEquivalentTo(series[^1]);
                convergedResult = l;
            }

            // After convergence, verify stability
            if (qty > AtrConvergence && convergedResult != null)
            {
                l.Atr.Should().BeApproximately(convergedResult.Atr!.Value, 0.0001);
            }
        }
    }

    [TestMethod]
    public void Ema()
    {
        const int lookback = 14;
        EmaResult? convergedResult = null;

        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            EmaHub hub = provider.ToEmaHub(lookback);

            foreach (Quote q in qts)
            {
                provider.Add(q);
            }

            EmaResult l = hub.Results[^1];
            Console.WriteLine($"EMA({lookback}) StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.Ema:N8}");

            // At convergence point, verify matches Series
            if (qty == EmaConvergence)
            {
                IReadOnlyList<EmaResult> series = qts.ToEma(lookback);
                l.Should().BeEquivalentTo(series[^1]);
                convergedResult = l;
            }

            // After convergence, verify stability
            if (qty > EmaConvergence && convergedResult != null)
            {
                l.Ema.Should().BeApproximately(convergedResult.Ema!.Value, 0.0001);
            }
        }
    }

    [TestMethod]
    public void Macd()
    {
        const int fast = 12, slow = 26, signal = 9;
        MacdResult? convergedResult = null;

        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            MacdHub hub = provider.ToMacdHub(fast, slow, signal);

            foreach (Quote q in qts)
            {
                provider.Add(q);
            }

            MacdResult l = hub.Results[^1];
            Console.WriteLine($"MACD StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.Macd:N8}");

            // At convergence point, verify matches Series
            if (qty == MacdConvergence)
            {
                IReadOnlyList<MacdResult> series = qts.ToMacd(fast, slow, signal);
                l.Should().BeEquivalentTo(series[^1]);
                convergedResult = l;
            }

            // After convergence, verify stability
            if (qty > MacdConvergence && convergedResult != null)
            {
                l.Macd.Should().BeApproximately(convergedResult.Macd!.Value, 0.0001);
                l.Signal.Should().BeApproximately(convergedResult.Signal!.Value, 0.0001);
                l.Histogram.Should().BeApproximately(convergedResult.Histogram!.Value, 0.0001);
            }
        }
    }

    [TestMethod]
    public void Rsi()
    {
        const int lookback = 14;
        RsiResult? convergedResult = null;

        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            RsiHub hub = provider.ToRsiHub(lookback);

            foreach (Quote q in qts)
            {
                provider.Add(q);
            }

            RsiResult l = hub.Results[^1];
            Console.WriteLine($"RSI({lookback}) StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.Rsi:N8}");

            // At convergence point, verify matches Series
            if (qty == RsiConvergence)
            {
                IReadOnlyList<RsiResult> series = qts.ToRsi(lookback);
                l.Should().BeEquivalentTo(series[^1]);
                convergedResult = l;
            }

            // After convergence, verify stability
            if (qty > RsiConvergence && convergedResult != null)
            {
                l.Rsi.Should().BeApproximately(convergedResult.Rsi!.Value, 0.0001);
            }
        }
    }

    [TestMethod]
    public void Sma()
    {
        const int lookback = 14;
        SmaResult? convergedResult = null;

        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            SmaHub hub = provider.ToSmaHub(lookback);

            foreach (Quote q in qts)
            {
                provider.Add(q);
            }

            SmaResult l = hub.Results[^1];
            Console.WriteLine($"SMA({lookback}) StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.Sma:N8}");

            // At convergence point, verify matches Series
            if (qty == SmaConvergence)
            {
                IReadOnlyList<SmaResult> series = qts.ToSma(lookback);
                l.Should().BeEquivalentTo(series[^1]);
                convergedResult = l;
            }

            // After convergence, verify stability
            if (qty > SmaConvergence && convergedResult != null)
            {
                l.Sma.Should().BeApproximately(convergedResult.Sma!.Value, 0.0001);
            }
        }
    }

    [TestMethod]
    public void Stoch()
    {
        const int lookbackPeriods = 14, signalPeriods = 3, smoothPeriods = 3;
        StochResult? convergedResult = null;

        foreach (int qty in QuotesQuantities)
        {
            IReadOnlyList<Quote> qts = Data.GetLongish(qty);
            QuoteHub provider = new();
            StochHub hub = provider.ToStochHub(lookbackPeriods, signalPeriods, smoothPeriods);

            foreach (Quote q in qts)
            {
                provider.Add(q);
            }

            StochResult l = hub.Results[^1];
            Console.WriteLine($"STOCH StreamHub on {l.Timestamp:d} with {qts.Count,4} periods: {l.K:N8}");

            // At convergence point, verify matches Series
            if (qty == StochConvergence)
            {
                IReadOnlyList<StochResult> series = qts.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);
                l.Should().BeEquivalentTo(series[^1]);
                convergedResult = l;
            }

            // After convergence, verify stability
            if (qty > StochConvergence && convergedResult != null)
            {
                l.K.Should().BeApproximately(convergedResult.K!.Value, 0.0001);
                l.D.Should().BeApproximately(convergedResult.D!.Value, 0.0001);
            }
        }
    }
}
