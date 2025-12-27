namespace StreamHub;

[TestClass]
public class KvoHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        KvoHub observer = quoteHub.ToKvoHub(34, 55, 13);

        // fetch initial results (early)
        IReadOnlyList<KvoResult> streamList = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(quotesList[80]);

        // removal
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<KvoResult> seriesList = quotesList.ToKvo(34, 55, 13);

        streamList.Should().HaveCount(length - 1);
        streamList.IsExactly(seriesList);

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

        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer - chain KVO to SMA
        SmaHub observer = quoteHub
            .ToKvoHub(kvoFast, kvoSlow, kvoSignal)
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<SmaResult> streamList = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList = quotesList
            .ToKvo(kvoFast, kvoSlow, kvoSignal)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(
            seriesList,
            options => options.WithStrictOrdering());

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
