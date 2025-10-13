namespace StreamHub;

[TestClass]
public class T3Hub : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver()
    {
        const int lookbackPeriods = 5;
        const double volumeFactor = 0.7;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // prefill quotes at provider
        for (int i = 0; i < 50; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        T3Hub<Quote> observer = quoteHub
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
    public void ChainProvider()
    {
        const int t3Periods = 5;
        const double volumeFactor = 0.7;
        const int smaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize observer
        SmaHub<T3Result> observer = quoteHub
            .ToT3Hub(t3Periods, volumeFactor)
            .ToSma(smaPeriods);

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
    public override void CustomToString()
    {
        T3Hub<Quote> hub = new(new QuoteHub<Quote>(), 5, 0.7);
        hub.ToString().Should().Be("T3(5,0.7)");
    }

    [TestMethod]
    public void SettingsInheritance()
    {
        // setup quote hub (1st level)
        QuoteHub<Quote> quoteHub = new();

        // setup t3 hub (2nd level)
        T3Hub<Quote> t3Hub = quoteHub
            .ToT3Hub(lookbackPeriods: 5, volumeFactor: 0.7);

        // setup child hub (3rd level)
        SmaHub<T3Result> childHub = t3Hub
            .ToSma(lookbackPeriods: 5);

        // note: despite `quoteHub` being parentless,
        // it has default properties; it should not
        // inherit its own empty quoteHub settings

        // assert
        quoteHub.Properties.Settings.Should().Be(0b00000000, "it has default settings, not inherited");
        t3Hub.Properties.Settings.Should().Be(0b00000000, "it has default properties");
        childHub.Properties.Settings.Should().Be(0b00000000, "it inherits default properties");
    }
}
