namespace StreamHubs;

[TestClass]
public class KvoHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // initialize observer
        KvoHub observer = quoteHub.ToKvoHub(34, 55, 13);

        // fetch initial results (early)
        IReadOnlyList<KvoResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // removal
        quoteHub.Remove(Quotes[removeAtIndex]);

        // final results
        IReadOnlyList<KvoResult> expected = RevisedQuotes.ToKvo(34, 55, 13);

        actuals.Should().HaveCount(quotesCount - 1);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        KvoHub hub = new(new QuoteHub(), 34, 55, 13);
        hub.ToString().Should().Be("KVO(34,55,13)");
    }

    [TestMethod]
    public void Standard()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        KvoHub observer = quoteHub.ToKvoHub(34, 55, 13);

        // emulate quote stream
        foreach (Quote q in Quotes)
        {
            quoteHub.Add(q);
        }

        // final results
        IReadOnlyList<KvoResult> streamList = observer.Results;

        // time-series, for comparison
        IReadOnlyList<KvoResult> seriesList = Quotes.ToKvo(34, 55, 13);

        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Variants()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with different parameters
        KvoHub observer = quoteHub.ToKvoHub(20, 40, 10);

        // emulate quote stream
        foreach (Quote q in Quotes)
        {
            quoteHub.Add(q);
        }

        // final results
        IReadOnlyList<KvoResult> streamList = observer.Results;

        // time-series, for comparison
        IReadOnlyList<KvoResult> seriesList = Quotes.ToKvo(20, 40, 10);

        streamList.Should().HaveCount(length);
        streamList.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int kvoFast = 34;
        const int kvoSlow = 55;
        const int kvoSignal = 13;
        const int smaPeriods = 10;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer - chain KVO to SMA
        SmaHub observer = quoteHub
            .ToKvoHub(kvoFast, kvoSlow, kvoSignal)
            .ToSmaHub(smaPeriods);

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

        // final results
        IReadOnlyList<SmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> expected = RevisedQuotes
            .ToKvo(kvoFast, kvoSlow, kvoSignal)
            .ToSma(smaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void BadData()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // bad fast periods
        Action act = () => quoteHub.ToKvoHub(2, 55, 13);
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("fastPeriods");

        // bad slow periods (less than fast)
        act = () => quoteHub.ToKvoHub(34, 30, 13);
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("slowPeriods");

        // bad signal periods
        act = () => quoteHub.ToKvoHub(34, 55, 0);
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("signalPeriods");

        quoteHub.EndTransmission();
    }
}
