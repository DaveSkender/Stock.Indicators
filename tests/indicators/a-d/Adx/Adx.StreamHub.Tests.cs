namespace StreamHub;

[TestClass]
public class AdxHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const int lookbackPeriods = 14;
    private static readonly IReadOnlyList<AdxResult> expectedOriginal = Quotes.ToAdx(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        AdxHub observer = quoteHub
            .ToAdxHub(lookbackPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<AdxResult> streamList
            = observer.Results;

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        // ADX emits IReusable results (AdxResult implements IReusable with Value = Adx),
        // so it can act as a chain provider for downstream indicators.

        const int adxPeriods = 14;
        const int emaPeriods = 10;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize chain: ADX then EMA over its Value
        EmaHub observer = quoteHub
            .ToAdxHub(adxPeriods)
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
            .ToAdx(adxPeriods)
            .ToEma(emaPeriods);

        streamList.Should().HaveCount(seriesList.Count);
        streamList.Should().BeEquivalentTo(seriesList, static o => o.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub quoteHub = new();

        AdxHub hub = new(quoteHub, 14);
        hub.ToString().Should().Be("ADX(14)");

        quoteHub.Add(Quotes[0]);
        quoteHub.Add(Quotes[1]);

        string s = $"ADX(14)({Quotes[0].Timestamp:d})";
        hub.ToString().Should().Be(s);
    }

    [TestMethod]
    public void RollbackValidation()
    {
        QuoteHub quoteHub = new();

        // Precondition: Normal quote stream with 502 expected entries
        AdxHub observer = quoteHub.ToAdxHub(lookbackPeriods);
        quoteHub.Add(Quotes);

        observer.Results.Should().HaveCount(502);
        observer.Results.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        // Act: Remove a single historical value
        quoteHub.Remove(Quotes[removeAtIndex]);

        // Assert: Observer should have 501 results and match revised series
        IReadOnlyList<AdxResult> expectedRevised = RevisedQuotes.ToAdx(lookbackPeriods);

        observer.Results.Should().HaveCount(501);
        observer.Results.Should().BeEquivalentTo(expectedRevised, static options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
