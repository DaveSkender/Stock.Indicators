namespace StreamHubs;

[TestClass]
public class MaEnvelopesHubTests : StreamHubTestBase, ITestChainObserver
{
    private const int lookbackPeriods = 20;
    private const double percentOffset = 2.5;
    private readonly IReadOnlyList<MaEnvelopeResult> expectedOriginal = Quotes.ToMaEnvelopes(lookbackPeriods, percentOffset);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        MaEnvelopesHub observer = quoteHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset);

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
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<MaEnvelopeResult> expectedRevised = RevisedQuotes.ToMaEnvelopes(lookbackPeriods, percentOffset);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int smaPeriods = 12;
        const int maPeriods = 20;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        MaEnvelopesHub observer = quoteHub
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
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void WithDemaType()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer with DEMA type
        MaEnvelopesHub observer = quoteHub.ToMaEnvelopesHub(lookbackPeriods, percentOffset, MaType.DEMA);

        // emulate quote stream
        for (int i = 0; i < length; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<MaEnvelopeResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<MaEnvelopeResult> expected = Quotes.ToMaEnvelopes(lookbackPeriods, percentOffset, MaType.DEMA);

        // assert, should equal series
        actuals.Should().HaveCount(length);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    public override void ToStringOverride_ReturnsExpectedName()
    {
        MaEnvelopesHub hub = new(new QuoteHub(), lookbackPeriods, percentOffset, MaType.SMA);
        hub.ToString().Should().Be($"MAENV({lookbackPeriods},{percentOffset},{MaType.SMA})");
    }
}
