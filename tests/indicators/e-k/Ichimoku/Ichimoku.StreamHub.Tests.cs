namespace StreamHub;

[TestClass]
public class IchimokuHubTests : StreamHubTestBase, ITestQuoteObserver
{
    private const int tenkanPeriods = 9;
    private const int kijunPeriods = 26;
    private const int senkouBPeriods = 52;

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (batch)
        quoteHub.Add(quotesList.Take(30));

        // initialize observer
        IchimokuHub observer = quoteHub
            .ToIchimokuHub(tenkanPeriods, kijunPeriods, senkouBPeriods);

        observer.Results.Should().HaveCount(30);

        // fetch initial results (early)
        IReadOnlyList<IchimokuResult> streamList
            = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 30; i < length; i++)
        {
            Quote q = quotesList[i];
            quoteHub.Add(q);
        }

        // time-series, for comparison
        IReadOnlyList<IchimokuResult> seriesList
           = quotesList
            .ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList, o => o.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactlyWithOffsets()
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
