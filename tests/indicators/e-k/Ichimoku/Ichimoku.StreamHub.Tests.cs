namespace StreamHub;

[TestClass]
public class IchimokuHubTests : StreamHubTestBase, ITestQuoteObserver
{
    private const int tenkanPeriods = 9;
    private const int kijunPeriods = 26;
    private const int senkouBPeriods = 52;

    [TestMethod]
    public void QuoteObserver()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (warmup coverage)
        quoteHub.Add(Quotes.Take(30));

        // initialize observer
        IchimokuHub observer = quoteHub.ToIchimokuHub(tenkanPeriods, kijunPeriods, senkouBPeriods);

        // fetch initial results (early)
        IReadOnlyList<IchimokuResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 30; i < length; i++)
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

        IReadOnlyList<IchimokuResult> expected = Quotes.ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods);
        actuals.Should().HaveCount(length);
        actuals.Should().BeEquivalentTo(expected, static options => options.WithStrictOrdering());

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<IchimokuResult> expectedRevised = RevisedQuotes.ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserverWithOffsets()
    {
        // Simple test for different offset parameters

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with custom offsets
        IchimokuHub observer = quoteHub
            .ToIchimokuHub(3, 13, 40, 0);

        // add quotes to quoteHub
        quoteHub.Add(quotesList);

        // stream results
        IReadOnlyList<IchimokuResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<IchimokuResult> seriesList
           = quotesList
            .ToIchimoku(3, 13, 40, 0);

        // assert, should equal series
        streamList.Should().HaveCount(quotesList.Count);
        streamList.Should().BeEquivalentTo(seriesList, o => o.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        IchimokuHub hub = new(new QuoteHub(), 9, 26, 52, 26, 26);
        hub.ToString().Should().Be("ICHIMOKU(9,26,52)");
    }
}
