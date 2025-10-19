namespace StreamHub;

[TestClass]
public class MaEnvelopesHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 20;
    private const double percentOffset = 2.5;
    private readonly IReadOnlyList<MaEnvelopeResult> expectedOriginal = Quotes.ToMaEnvelopes(lookbackPeriods, percentOffset);

    [TestMethod]
    public void QuoteObserver()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        Skender.Stock.Indicators.MaEnvelopesHub<IQuote> observer = quoteHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset);

        // fetch initial results (early)
        IReadOnlyList<MaEnvelopeResult> actuals = observer.Results;

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
        actuals.Should().BeEquivalentTo(expectedOriginal, options => options.WithStrictOrdering());

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<MaEnvelopeResult> expectedRevised = RevisedQuotes.ToMaEnvelopes(lookbackPeriods, percentOffset);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        const int smaPeriods = 12;
        const int maPeriods = 20;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        Skender.Stock.Indicators.MaEnvelopesHub<SmaResult> observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToMaEnvelopesHub(maPeriods, percentOffset);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<MaEnvelopeResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<MaEnvelopeResult> expected = Quotes
            .ToSma(smaPeriods)
            .ToMaEnvelopes(maPeriods, percentOffset);

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int maPeriods = 20;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // setup observer
        Skender.Stock.Indicators.MaEnvelopesHub<IQuote> maEnvHub = quoteHub.ToMaEnvelopesHub(maPeriods, percentOffset);

        // MaEnvelopeResult doesn't implement IReusable, so it cannot be used as a provider for other indicators
        // This test verifies the hub works correctly but doesn't chain to another indicator

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<MaEnvelopeResult> actuals = maEnvHub.Results;

        // time-series, for comparison
        IReadOnlyList<MaEnvelopeResult> expected = Quotes.ToMaEnvelopes(maPeriods, percentOffset);

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        // cleanup
        maEnvHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void WithDemaType()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with DEMA type
        Skender.Stock.Indicators.MaEnvelopesHub<IQuote> observer = quoteHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset, MaType.DEMA);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<MaEnvelopeResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<MaEnvelopeResult> expected = Quotes.ToMaEnvelopes(lookbackPeriods, percentOffset, MaType.DEMA);

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    public override void CustomToString()
    {
        Skender.Stock.Indicators.MaEnvelopesHub<IQuote> hub = new(new QuoteHub(), lookbackPeriods, percentOffset, MaType.SMA);
        hub.ToString().Should().Be($"MAENV({lookbackPeriods},{percentOffset},{MaType.SMA})");
    }
}
