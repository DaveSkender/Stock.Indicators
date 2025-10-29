namespace StreamHub;

[TestClass]
public class RenkoAtrHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    [TestMethod]
    public void QuoteObserver()
    {
        const int atrPeriods = 14;
        const EndType endType = EndType.Close;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        RenkoAtrHub observer = quoteHub
            .ToRenkoAtrHub(atrPeriods, endType);

        // fetch initial results (early)
        IReadOnlyList<RenkoResult> streamList
            = observer.Results;

        // emulate adding all quotes to provider hub
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // time-series, for comparison
        IReadOnlyList<RenkoResult> seriesList = quotesList
            .ToRenkoAtr(atrPeriods, endType);

        // assert, should equal series
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());
        streamList.Should().HaveCount(seriesList.Count);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int atrPeriods = 14;
        const EndType endType = EndType.Close;
        const int smaPeriods = 20;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToRenkoAtrHub(atrPeriods, endType)
            .ToSmaHub(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<SmaResult> seriesList = quotesList
            .ToRenkoAtr(atrPeriods, endType)
            .ToSma(smaPeriods);

        // assert, should equal series
        streamList.Should().BeEquivalentTo(seriesList, options => options.WithStrictOrdering());
        streamList.Should().HaveCount(seriesList.Count);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        RenkoAtrHub hub = new(new QuoteHub(), 14, EndType.Close);
        hub.ToString().Should().Be("RENKO-ATR(14,CLOSE)");
    }

    [TestMethod]
    public void SettingsInheritance()
    {
        // setup quote hub (1st level)
        QuoteHub quoteHub = new();

        // setup RenkoAtr hub (2nd level)
        RenkoAtrHub renkoAtrHub = quoteHub
            .ToRenkoAtrHub(atrPeriods: 14, endType: EndType.Close);

        // setup child hub (3rd level)
        SmaHub childHub = renkoAtrHub
            .ToSmaHub(lookbackPeriods: 5);

        // note: despite `quoteHub` being parentless,
        // it has default properties; it should not
        // inherit its own empty quoteHub settings

        // assert
        quoteHub.Properties.Settings.Should().Be(0b00000000, "is has default settings, not inherited");
        renkoAtrHub.Properties.Settings.Should().Be(0b00000010, "it has custom RenkoAtr properties");
        childHub.Properties.Settings.Should().Be(0b00000010, "it inherits RenkoAtr properties");
    }
}
