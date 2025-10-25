namespace StreamHub;

[TestClass]
public class CmfHubTests : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const int lookbackPeriods = 20;
    private static readonly IReadOnlyList<CmfResult> expectedOriginal = Quotes.ToCmf(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        CmfHub observer = quoteHub
            .ToCmfHub(lookbackPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<CmfResult> streamList = observer.Results;

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(expectedOriginal, static options => options.WithStrictOrdering());

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
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
    public override void CustomToString()
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
