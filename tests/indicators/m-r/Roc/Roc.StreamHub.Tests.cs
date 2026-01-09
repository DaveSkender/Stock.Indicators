namespace StreamHubs;

[TestClass]
public class RocHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        RocHub observer = quoteHub.ToRocHub(20);

        // fetch initial results (early)
        IReadOnlyList<RocResult> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
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

        IReadOnlyList<RocResult> expectedOriginal = Quotes.ToRoc(20);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);
        IReadOnlyList<RocResult> expectedRevised = RevisedQuotes.ToRoc(20);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 12;
        const int rocPeriods = 20;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        RocHub observer = quoteHub
            .ToEmaHub(emaPeriods)
            .ToRocHub(rocPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<RocResult> sut = observer.Results;

        // time-series, for comparison
        IReadOnlyList<RocResult> expected = Quotes
            .ToEma(emaPeriods)
            .ToRoc(rocPeriods);

        // assert, should equal series
        sut.IsExactly(expected);
        sut.Should().HaveCount(quotesCount);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int rocPeriods = 20;
        const int emaPeriods = 12;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        EmaHub observer = quoteHub
            .ToRocHub(rocPeriods)
            .ToEmaHub(emaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // final results
        IReadOnlyList<EmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToRoc(rocPeriods)
            .ToEma(emaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        RocHub hub = new(new QuoteHub(), 20);
        hub.ToString().Should().Be("ROC(20)");
    }
}
