namespace StreamHub;

[TestClass]
public class CmfHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const int lookbackPeriods = 20;
    private static readonly IReadOnlyList<CmfResult> expectedOriginal = Quotes.ToCmf(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider (warmup coverage)
        quoteHub.Add(Quotes.Take(25));

        // initialize observer
        CmfHub observer = quoteHub.ToCmfHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<CmfResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 25; i < length; i++)
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
        actuals.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<CmfResult> expectedRevised = RevisedQuotes.ToCmf(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        // CMF emits IReusable results (CmfResult implements IReusable with Value = Cmf),
        // so it can act as a chain provider for downstream indicators.

        const int cmfPeriods = 20;
        const int emaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize chain: CMF then EMA over its Value
        EmaHub observer = quoteHub
            .ToCmfHub(cmfPeriods)
            .ToEmaHub(emaPeriods);

        // stream quotes
        foreach (Quote q in quotesList)
        {
            quoteHub.Add(q);
        }

        // results from stream
        IReadOnlyList<EmaResult> streamList = observer.Results;

        // time-series parity
        IReadOnlyList<EmaResult> seriesList = quotesList
            .ToCmf(cmfPeriods)
            .ToEma(emaPeriods);

        streamList.Should().HaveCount(seriesList.Count);
        streamList.Should().BeEquivalentTo(seriesList, static o => o.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();

        CmfHub hub = new(quoteHub, lookbackPeriods);
        hub.ToString().Should().Be($"CMF({lookbackPeriods})");

        quoteHub.Add(Quotes[0]);
        quoteHub.Add(Quotes[1]);

        string s = $"CMF({lookbackPeriods})({Quotes[0].Timestamp:d})";
        hub.ToString().Should().Be(s);
    }

    [TestMethod]
    public void RollbackValidation()
    {
        QuoteHub quoteHub = new();

        // Precondition: Normal quote stream with 502 expected entries
        CmfHub observer = quoteHub.ToCmfHub(lookbackPeriods);
        quoteHub.Add(Quotes);

        observer.Results.Should().HaveCount(502);
        observer.Results.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        // Act: Remove a single historical value
        quoteHub.Remove(Quotes[removeAtIndex]);

        // Assert: Observer should have 501 results and match revised series
        IReadOnlyList<CmfResult> expectedRevised = RevisedQuotes.ToCmf(lookbackPeriods);

        observer.Results.Should().HaveCount(501);
        observer.Results.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
