namespace StreamHub;

[TestClass]
public class RenkoHub : StreamHubTestBase, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        const decimal brickSize = 2.5m;
        const EndType endType = EndType.HighLow;

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
        RenkoHub<Quote> observer = quoteHub
            .ToRenkoHub(brickSize, endType);

        // fetch initial results (early)
        IReadOnlyList<RenkoResult> streamList
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
        IReadOnlyList<RenkoResult> seriesList = quotesList
            .ToRenko(brickSize, endType);

        // assert, should equal series
        streamList.Should().BeEquivalentTo(seriesList);
        streamList.Should().HaveCount(159);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const decimal brickSize = 2.5m;
        const EndType endType = EndType.Close;
        const int smaPeriods = 50;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize observer
        SmaHub<RenkoResult> observer = quoteHub
            .ToRenkoHub(brickSize, endType)
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
            .ToRenko(brickSize, endType)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().BeEquivalentTo(seriesList);
        streamList.Should().HaveCount(112);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        RenkoHub<Quote> hub = new(new QuoteHub<Quote>(), 2.5m, EndType.Close);
        hub.ToString().Should().Be("RENKO(2.5,CLOSE)");
    }

    [TestMethod]
    public void SettingsInheritance()
    {
        // setup quote hub (1st level)
        QuoteHub<Quote> quoteHub = new();

        // setup renko hub (2nd level)
        RenkoHub<Quote> renkoHub = quoteHub
            .ToRenkoHub(brickSize: 2.5m, endType: EndType.Close);

        // setup child hub (3rd level)
        SmaHub<RenkoResult> childHub = renkoHub
            .ToSma(lookbackPeriods: 5);

        // note: dispite `quoteHub` being parentless,
        // it has default properties; it should not
        // inherit its own empty quoteHub settings

        // assert
        quoteHub.Properties.Settings.Should().Be(0b00000000, "is has default settings, not inherited");
        renkoHub.Properties.Settings.Should().Be(0b00000010, "it has custom Renko properties");
        childHub.Properties.Settings.Should().Be(0b00000010, "it inherits Renko properties");
    }
}
