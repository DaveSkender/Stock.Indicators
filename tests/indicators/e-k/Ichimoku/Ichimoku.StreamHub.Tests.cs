namespace StreamHubs;

[TestClass]
public class IchimokuHubTests : StreamHubTestBase, ITestQuoteObserver
{
    private const int tenkanPeriods = 9;
    private const int kijunPeriods = 26;
    private const int senkouBPeriods = 52;

    private static readonly IReadOnlyList<IchimokuResult> expectedOriginal
        = Quotes.ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (warmup coverage)
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        IchimokuHub sut = quoteHub
            .ToIchimokuHub(tenkanPeriods, kijunPeriods, senkouBPeriods);

        // fetch initial results (early)
        IReadOnlyList<IchimokuResult> actuals = sut.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
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

        actuals.Should().HaveCount(length);
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<IchimokuResult> expectedRevised
            = RevisedQuotes.ToIchimoku(tenkanPeriods, kijunPeriods, senkouBPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        sut.Unsubscribe();
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
        IchimokuHub sut = quoteHub
            .ToIchimokuHub(3, 13, 40, 0);

        // add quotes to quoteHub
        quoteHub.Add(quotesList);

        // stream results
        IReadOnlyList<IchimokuResult> streamList
            = sut.Results;

        // time-series, for comparison
        IReadOnlyList<IchimokuResult> seriesList
           = quotesList
            .ToIchimoku(3, 13, 40, 0);

        // assert, should equal series
        streamList.Should().HaveCount(quotesList.Count);
        streamList.IsExactly(seriesList);

        sut.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        IchimokuHub sut = new(new QuoteHub(), 9, 26, 52, 26, 26);
        sut.ToString().Should().Be("ICHIMOKU(9,26,52)");
    }
}
