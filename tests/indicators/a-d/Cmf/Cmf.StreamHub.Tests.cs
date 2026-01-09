namespace StreamHubs;

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
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<CmfResult> expectedRevised = RevisedQuotes.ToCmf(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

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

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize chain: CMF then EMA over its Value
        EmaHub observer = quoteHub
            .ToCmfHub(cmfPeriods)
            .ToEmaHub(emaPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++)
        {
            if (i == 80) { continue; }  // Skip for late arrival

            Quote q = Quotes[i];
            quoteHub.Add(q);

            if (i is > 100 and < 105) { quoteHub.Add(q); }  // Duplicate quotes
        }

        quoteHub.Insert(Quotes[80]);  // Late arrival
        quoteHub.Remove(Quotes[removeAtIndex]);  // Remove

        // results from stream
        IReadOnlyList<EmaResult> sut = observer.Results;

        // time-series parity (revised)
        IReadOnlyList<EmaResult> expected = RevisedQuotes
            .ToCmf(cmfPeriods)
            .ToEma(emaPeriods);

        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void Results_AreAlwaysBounded()
    {
        IReadOnlyList<CmfResult> sut = Quotes.ToCmfHub(20).Results;
        sut.IsBetween(static x => x.Cmf, -1, 1);
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
        observer.Results.IsExactly(expectedOriginal);

        // Act: Remove a single historical value
        quoteHub.Remove(Quotes[removeAtIndex]);

        // Assert: Observer should have 501 results and match revised series
        IReadOnlyList<CmfResult> expectedRevised = RevisedQuotes.ToCmf(lookbackPeriods);

        observer.Results.Should().HaveCount(501);
        observer.Results.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
