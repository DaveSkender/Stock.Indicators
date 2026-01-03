namespace StreamHubs;

[TestClass]
public class SmiHubTest : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const int lookbackPeriods = 13;
    private const int firstSmoothPeriods = 25;
    private const int secondSmoothPeriods = 2;
    private const int signalPeriods = 3;

    private static readonly IReadOnlyList<SmiResult> expectedOriginal
        = Quotes.ToSmi(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(quotesList.Take(20));

        // initialize observer
        SmiHub observer = quoteHub.ToSmiHub(
            lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);

        // fetch initial results (early)
        IReadOnlyList<SmiResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = quotesList[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival, should equal series
        quoteHub.Insert(quotesList[80]);
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(quotesList[removeAtIndex]);

        IReadOnlyList<SmiResult> expectedRevised = RevisedQuotes.ToSmi(
            lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        // SMI emits IReusable results (SmiResult implements IReusable with Value = Smi),
        // so it can act as a chain provider for downstream indicators.

        const int emaPeriods = 10;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize chain: SMI then EMA over its Value
        EmaHub observer = quoteHub
            .ToSmiHub(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods)
            .ToEmaHub(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip for late arrival

            Quote q = Quotes[i];
            quoteHub.Add(q);

            if (i is > 100 and < 105) { quoteHub.Add(q); }  // Duplicate quotes
        }

        quoteHub.Insert(Quotes[80]);  // Late arrival
        quoteHub.Remove(Quotes[removeAtIndex]);  // Remove

        // results from stream
        IReadOnlyList<EmaResult> sut = observer.Results;

        // time-series parity (revised)
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToSmi(lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods)
            .ToEma(emaPeriods);

        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();

        SmiHub hub = new(
            quoteHub, lookbackPeriods, firstSmoothPeriods, secondSmoothPeriods, signalPeriods);
        hub.ToString().Should().Be($"SMI({lookbackPeriods},{firstSmoothPeriods},{secondSmoothPeriods},{signalPeriods})");
    }

    [TestMethod]
    public void IncrementalUpdates()
    {
        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub with incremental updates
        QuoteHub quoteHub = new();
        SmiHub observer = quoteHub.ToSmiHub(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        // add quotes one by one
        foreach (Quote quote in quotesList)
        {
            quoteHub.Add(quote);
        }

        // close observations
        quoteHub.EndTransmission();

        // verify consistency
        IReadOnlyList<SmiResult> expected = Quotes.ToSmi(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        observer.Cache.IsExactly(expected);
    }

    [TestMethod]
    public void Properties()
    {
        const int testLookbackPeriods = 21;
        const int testFirstSmoothPeriods = 30;
        const int testSecondSmoothPeriods = 5;
        const int testSignalPeriods = 7;

        QuoteHub quoteHub = new();
        SmiHub observer = quoteHub.ToSmiHub(
            testLookbackPeriods,
            testFirstSmoothPeriods,
            testSecondSmoothPeriods,
            testSignalPeriods);

        // verify properties
        observer.LookbackPeriods.Should().Be(testLookbackPeriods);
        observer.FirstSmoothPeriods.Should().Be(testFirstSmoothPeriods);
        observer.SecondSmoothPeriods.Should().Be(testSecondSmoothPeriods);
        observer.SignalPeriods.Should().Be(testSignalPeriods);
        observer.K1.Should().Be(2d / (testFirstSmoothPeriods + 1));
        observer.K2.Should().Be(2d / (testSecondSmoothPeriods + 1));
        observer.KS.Should().Be(2d / (testSignalPeriods + 1));
        observer.ToString().Should().Be($"SMI({testLookbackPeriods},{testFirstSmoothPeriods},{testSecondSmoothPeriods},{testSignalPeriods})");
    }

    [TestMethod]
    public void DefaultParameters()
    {
        QuoteHub quoteHub = new();
        SmiHub observer = quoteHub.ToSmiHub();

        // verify default properties
        observer.LookbackPeriods.Should().Be(13);
        observer.FirstSmoothPeriods.Should().Be(25);
        observer.SecondSmoothPeriods.Should().Be(2);
        observer.SignalPeriods.Should().Be(3);
        observer.ToString().Should().Be("SMI(13,25,2,3)");
    }

    [TestMethod]
    public void StreamingAccuracy()
    {
        // Test that streaming produces accurate results compared to batch processing
        List<Quote> quotesList = Quotes.ToList();

        // streaming calculation
        QuoteHub quoteHub = new();
        SmiHub streamObserver = quoteHub.ToSmiHub(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        foreach (Quote quote in quotesList)
        {
            quoteHub.Add(quote);
        }

        quoteHub.EndTransmission();

        // batch calculation
        IReadOnlyList<SmiResult> batchResults = Quotes.ToSmi(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        // compare results with strict ordering
        streamObserver.Cache.Should().HaveCount(batchResults.Count);
        streamObserver.Cache.IsExactly(batchResults);
    }

    [TestMethod]
    public void BatchProcessing()
    {
        // Test batch processing with all quotes added at once
        QuoteHub quoteHub = new();
        SmiHub observer = quoteHub.ToSmiHub(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        // add all quotes at once
        quoteHub.Add(Quotes);
        quoteHub.EndTransmission();

        // verify against static series calculation
        IReadOnlyList<SmiResult> expected = Quotes.ToSmi(
            lookbackPeriods,
            firstSmoothPeriods,
            secondSmoothPeriods,
            signalPeriods);

        observer.Cache.Should().HaveCount(Quotes.Count);
        observer.Cache.IsExactly(expected);
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<SmiResult> sut = Quotes.ToSmiHub(14, 20, 5, 3).Results;
        sut.IsBetween(static x => x.Smi, -100, 100);
        sut.IsBetween(static x => x.Signal, -100, 100);
    }
}
