namespace StreamHubs;

[TestClass]
public class StochHubTests : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        const int lookbackPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 3;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (warmup coverage)
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        StochHub observer = quoteHub.ToStochHub(lookbackPeriods, signalPeriods, smoothPeriods);

        // fetch initial results (early)
        IReadOnlyList<StochResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival, should equal series
        quoteHub.Insert(Quotes[80]);

        IReadOnlyList<StochResult> expected = Quotes.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);
        actuals.Should().HaveCount(length);
        actuals.IsExactly(expected);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<StochResult> expectedRevised = RevisedQuotes.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        StochHub hub = new(new QuoteHub(), 14, 3, 3);
        hub.ToString().Should().Be("STOCH(14,3,3)");
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<StochResult> sut = Quotes.ToStochHub(14, 3, 3).Results;
        sut.IsBetween(static x => x.Oscillator, 0, 100);
        sut.IsBetween(static x => x.Signal, 0, 100);
    }

    [TestMethod]
    public void ExtendedParameters()
    {
        const int lookbackPeriods = 9;
        const int signalPeriods = 5;
        const int smoothPeriods = 2;
        const double kFactor = 5;
        const double dFactor = 4;
        const MaType movingAverageType = MaType.SMMA;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub and observer BEFORE adding data
        QuoteHub quoteHub = new();
        StochHub observer = quoteHub.ToStoch(
            lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        // add quotes
        quoteHub.Add(quotesList);

        // close observations
        quoteHub.EndTransmission();

        // verify against static series calculation
        IReadOnlyList<StochResult> expected = Quotes.ToStoch(
            lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        observer.Cache.Should().HaveCount(Quotes.Count);
        observer.Cache.IsExactly(expected);
    }

    [TestMethod]
    public void IncrementalUpdates()
    {
        const int lookbackPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 3;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub with incremental updates
        QuoteHub quoteHub = new();
        StochHub observer = quoteHub.ToStochHub(lookbackPeriods, signalPeriods, smoothPeriods);

        // add quotes one by one
        foreach (Quote quote in quotesList)
        {
            quoteHub.Add(quote);
        }

        // close observations
        quoteHub.EndTransmission();

        // verify consistency
        IReadOnlyList<StochResult> expected = Quotes.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);
        observer.Cache.IsExactly(expected);
    }

    [TestMethod]
    public void Properties()
    {
        const int lookbackPeriods = 21;
        const int signalPeriods = 5;
        const int smoothPeriods = 2;
        const double kFactor = 5;
        const double dFactor = 4;
        const MaType movingAverageType = MaType.SMMA;

        QuoteHub quoteHub = new();
        StochHub observer = quoteHub.ToStoch(
            lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        // verify properties
        observer.LookbackPeriods.Should().Be(lookbackPeriods);
        observer.SignalPeriods.Should().Be(signalPeriods);
        observer.SmoothPeriods.Should().Be(smoothPeriods);
        observer.KFactor.Should().Be(kFactor);
        observer.DFactor.Should().Be(dFactor);
        observer.MovingAverageType.Should().Be(movingAverageType);
        observer.ToString().Should().Be($"STOCH({lookbackPeriods},{signalPeriods},{smoothPeriods})");
    }

    [TestMethod]
    public void DefaultParameters()
    {
        QuoteHub quoteHub = new();
        StochHub observer = quoteHub.ToStochHub();

        // verify default properties
        observer.LookbackPeriods.Should().Be(14);
        observer.SignalPeriods.Should().Be(3);
        observer.SmoothPeriods.Should().Be(3);
        observer.KFactor.Should().Be(3);
        observer.DFactor.Should().Be(2);
        observer.MovingAverageType.Should().Be(MaType.SMA);
        observer.ToString().Should().Be("STOCH(14,3,3)");
    }

    [TestMethod]
    public void StreamingAccuracy()
    {
        // Test that streaming produces accurate results compared to batch processing
        const int lookbackPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 3;

        List<Quote> quotesList = Quotes.ToList();

        // streaming calculation
        QuoteHub quoteHub = new();
        StochHub streamObserver = quoteHub.ToStochHub(lookbackPeriods, signalPeriods, smoothPeriods);

        foreach (Quote quote in quotesList)
        {
            quoteHub.Add(quote);
        }

        quoteHub.EndTransmission();

        // batch calculation
        IReadOnlyList<StochResult> batchResults = Quotes.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

        // compare results with reasonable precision
        streamObserver.Cache.Should().HaveCount(batchResults.Count);

        for (int i = 0; i < streamObserver.Cache.Count; i++)
        {
            StochResult streamResult = streamObserver.Cache[i];
            StochResult batchResult = batchResults[i];

            streamResult.Timestamp.Should().Be(batchResult.Timestamp);
            streamResult.Oscillator.Should().Be(batchResult.Oscillator);
            streamResult.Signal.Should().Be(batchResult.Signal);
            streamResult.PercentJ.Should().Be(batchResult.PercentJ);
        }
    }
}
