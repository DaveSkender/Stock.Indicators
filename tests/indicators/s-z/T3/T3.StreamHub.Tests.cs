namespace StreamHubs;

[TestClass]
public class T3HubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        const int lookbackPeriods = 5;
        const double volumeFactor = 0.7;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 50; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        T3Hub observer = quoteHub
            .ToT3Hub(lookbackPeriods, volumeFactor);

        // fetch initial results (early)
        IReadOnlyList<T3Result> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 50; i < length; i++)
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

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<T3Result> seriesList = quotesList
            .ToT3(lookbackPeriods, volumeFactor);

        // assert, should equal series
        streamList.Should().BeEquivalentTo(seriesList);
        streamList.Should().HaveCount(501);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int t3Periods = 5;
        const double volumeFactor = 0.7;
        const int smaPeriods = 10;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer - chain SMA to T3
        T3Hub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToT3Hub(t3Periods, volumeFactor);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        // final results
        IReadOnlyList<T3Result> streamList = observer.Results;

        // time-series, for comparison
        IReadOnlyList<T3Result> seriesList = Quotes
            .ToSma(smaPeriods)
            .ToT3(t3Periods, volumeFactor);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(
            seriesList,
            options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int t3Periods = 5;
        const double volumeFactor = 0.7;
        const int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToT3Hub(t3Periods, volumeFactor)
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList = quotesList
            .ToT3(t3Periods, volumeFactor)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().BeEquivalentTo(seriesList);
        streamList.Should().HaveCount(501);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        T3Hub hub = new(new QuoteHub(), 5, 0.7);
        hub.ToString().Should().Be("T3(5,0.7)");
    }

    [TestMethod]
    public void SettingsInheritance()
    {
        // setup quote hub (1st level)
        QuoteHub quoteHub = new();

        // setup t3 hub (2nd level)
        T3Hub t3Hub = quoteHub
            .ToT3Hub(lookbackPeriods: 5, volumeFactor: 0.7);

        // setup child hub (3rd level)
        SmaHub childHub = t3Hub
            .ToSmaHub(lookbackPeriods: 5);

        // note: despite `quoteHub` being parentless,
        // it has default properties; it should not
        // inherit its own empty quoteHub settings

        // assert
        quoteHub.Properties.Settings.Should().Be(0b00000000, "it has default settings, not inherited");
        t3Hub.Properties.Settings.Should().Be(0b00000000, "it has default properties");
        childHub.Properties.Settings.Should().Be(0b00000000, "it inherits default properties");
    }
}
