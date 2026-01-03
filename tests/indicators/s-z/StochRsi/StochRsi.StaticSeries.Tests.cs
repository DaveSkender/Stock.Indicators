namespace StaticSeries;

[TestClass]
public class StochRsi : StaticSeriesTestBase
{
    /// <summary>
    /// Fast RSI
    /// </summary>
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 1;

        IReadOnlyList<StochRsiResult> sut =
            Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // assertions

        // proper quantities
        sut.Should().HaveCount(502);
        Assert.HasCount(475, sut.Where(static x => x.StochRsi != null));
        Assert.HasCount(473, sut.Where(static x => x.Signal != null));

        // sample values
        StochRsiResult r1 = sut[31];
        r1.StochRsi.Should().BeApproximately(93.3333, Money4);
        r1.Signal.Should().BeApproximately(97.7778, Money4);

        StochRsiResult r2 = sut[152];
        r2.StochRsi.Should().Be(0);
        r2.Signal.Should().Be(0);

        StochRsiResult r3 = sut[249];
        r3.StochRsi.Should().BeApproximately(36.5517, Money4);
        r3.Signal.Should().BeApproximately(27.3094, Money4);

        StochRsiResult r4 = sut[501];
        r4.StochRsi.Should().BeApproximately(97.5244, Money4);
        r4.Signal.Should().BeApproximately(89.8385, Money4);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<StochRsiResult> sut = Quotes.ToStochRsi(14, 14, 3, 1);
        sut.IsBetween(static x => x.StochRsi, 0, 100);
        sut.IsBetween(static x => x.Signal, 0, 100);
    }

    [TestMethod]
    public void SlowRsi()
    {
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 3;

        IReadOnlyList<StochRsiResult> sut =
            Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // assertions

        // proper quantities
        sut.Should().HaveCount(502);
        Assert.HasCount(473, sut.Where(static x => x.StochRsi != null));
        Assert.HasCount(471, sut.Where(static x => x.Signal != null));

        // sample values
        StochRsiResult r1 = sut[31];
        r1.StochRsi.Should().BeApproximately(97.7778, Money4);
        r1.Signal.Should().BeApproximately(99.2593, Money4);

        StochRsiResult r2 = sut[152];
        r2.StochRsi.Should().Be(0);
        r2.Signal.Should().BeApproximately(20.0263, Money4);

        StochRsiResult r3 = sut[249];
        r3.StochRsi.Should().BeApproximately(27.3094, Money4);
        r3.Signal.Should().BeApproximately(33.2716, Money4);

        StochRsiResult r4 = sut[501];
        r4.StochRsi.Should().BeApproximately(89.8385, Money4);
        r4.Signal.Should().BeApproximately(73.4176, Money4);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<StochRsiResult> sut = Quotes
            .Use(CandlePart.Close)
            .ToStochRsi(14, 14, 3);

        sut.Should().HaveCount(502);
        Assert.HasCount(475, sut.Where(static x => x.StochRsi != null));
        sut.Where(static x => x.StochRsi is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<StochRsiResult> sut = Quotes
            .ToSma(2)
            .ToStochRsi(14, 14, 3);

        sut.Should().HaveCount(502);
        Assert.HasCount(474, sut.Where(static x => x.StochRsi != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> sut = Quotes
            .ToStochRsi(14, 14, 3, 3)
            .ToSma(10);

        sut.Should().HaveCount(502);
        Assert.HasCount(464, sut.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<StochRsiResult> r = BadQuotes
            .ToStochRsi(15, 20, 3, 2);

        r.Should().HaveCount(502);
        r.Where(static x => x.StochRsi is double v && double.IsNaN(v)).Should().BeEmpty();
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<StochRsiResult> r0 = Noquotes
            .ToStochRsi(10, 20, 3);

        r0.Should().BeEmpty();

        IReadOnlyList<StochRsiResult> r1 = Onequote
            .ToStochRsi(8, 13, 2);

        r1.Should().HaveCount(1);
    }

    [TestMethod]
    public void Removed()
    {
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 3;

        IReadOnlyList<StochRsiResult> sut = Quotes
            .ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods)
            .RemoveWarmupPeriods();

        // assertions
        const int removeQty = rsiPeriods + stochPeriods + smoothPeriods + 100;
        sut.Should().HaveCount(502 - removeQty);

        StochRsiResult last = sut[^1];
        last.StochRsi.Should().BeApproximately(89.8385, Money4);
        last.Signal.Should().BeApproximately(73.4176, Money4);
    }

    [TestMethod]
    public void AutoHealing_WorksWithoutExplicitRemove()
    {
        // Test that auto-healing handles RSI warmup periods correctly
        // without explicit Remove() call
        const int rsiPeriods = 14;
        const int stochPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 1;

        // Get results with current implementation (with Remove)
        IReadOnlyList<StochRsiResult> withRemove =
            Quotes.ToStochRsi(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // Manually implement version WITHOUT Remove() to test auto-healing
        IReadOnlyList<RsiResult> rsiResults = Quotes.ToRsi(rsiPeriods);

        // Convert RSI to QuoteD without Remove()
        List<QuoteD> quotesFromRsi = rsiResults
            .Select(static x => new QuoteD(
                Timestamp: x.Timestamp,
                Open: 0,
                High: x.Rsi.Null2NaN(),
                Low: x.Rsi.Null2NaN(),
                Close: x.Rsi.Null2NaN(),
                Volume: 0))
            .ToList();

        // Calculate Stoch on RSI values (will have NaN for first rsiPeriods)
        List<StochResult> stoResults = quotesFromRsi
            .CalcStoch(stochPeriods, signalPeriods, smoothPeriods, 3, 2, MaType.SMA);

        // Build results the same way as ToStochRsi
        int length = Quotes.Count;
        int initPeriods = Math.Min(rsiPeriods + stochPeriods - 1, length);
        List<StochRsiResult> withoutRemove = new(length);

        // Add back auto-pruned results
        for (int i = 0; i < initPeriods; i++)
        {
            withoutRemove.Add(new(Quotes[i].Timestamp));
        }

        // Add stoch results - key difference: no offset needed
        for (int i = rsiPeriods + stochPeriods - 1; i < length; i++)
        {
            StochResult r = stoResults[i]; // Direct indexing, not i - rsiPeriods

            withoutRemove.Add(new StochRsiResult(
                Timestamp: r.Timestamp,
                StochRsi: r.Oscillator,
                Signal: r.Signal));
        }

        // Compare results
        withRemove.Should().HaveCount(withoutRemove.Count);

        // Check same non-null counts
        int withRemoveNonNull = withRemove.Count(x => x.StochRsi != null);
        int withoutRemoveNonNull = withoutRemove.Count(x => x.StochRsi != null);
        withRemoveNonNull.Should().Be(withoutRemoveNonNull);

        // Compare specific values
        for (int i = 0; i < withRemove.Count; i++)
        {
            StochRsiResult expected = withRemove[i];
            StochRsiResult actual = withoutRemove[i];

            actual.Timestamp.Should().Be(expected.Timestamp);

            if (expected.StochRsi == null)
            {
                actual.StochRsi.Should().BeNull();
            }
            else
            {
                actual.StochRsi.Should().BeApproximately(expected.StochRsi.Value, Money4);
            }

            if (expected.Signal == null)
            {
                actual.Signal.Should().BeNull();
            }
            else
            {
                actual.Signal.Should().BeApproximately(expected.Signal.Value, Money4);
            }
        }
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad RSI period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(0, 14, 3));

        // bad STO period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(14, 0, 3, 3));

        // bad STO signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(14, 14, 0));

        // bad STO smoothing period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToStochRsi(14, 14, 3, 0));
    }
}
