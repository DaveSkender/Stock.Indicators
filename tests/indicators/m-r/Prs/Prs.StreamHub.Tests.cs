namespace StreamHubs;

[TestClass]
public class PrsHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private static readonly IReadOnlyList<Quote> RevisedOtherQuotes
        = OtherQuotes.Where(static (_, idx) => idx != removeAtIndex).ToList();

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hubs
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubBase = new();

        // prefill quotes at both providers
        quoteHubEval.Add(OtherQuotes.Take(20));
        quoteHubBase.Add(Quotes.Take(20));

        // initialize observer
        PrsHub observer = quoteHubEval.ToPrsHub(quoteHubBase);

        // fetch initial results (early)
        IReadOnlyList<PrsResult> sut = observer.Results;

        // emulate adding quotes to provider hubs
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote qEval = OtherQuotes[i];
            Quote qBase = Quotes[i];

            // IMPORTANT: Add to base BEFORE eval to ensure base cache is updated first
            quoteHubBase.Add(qBase);
            quoteHubEval.Add(qEval);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHubBase.Add(qBase);
                quoteHubEval.Add(qEval);
            }
        }

        // late arrival, should equal series
        quoteHubBase.Insert(Quotes[80]);
        quoteHubEval.Insert(OtherQuotes[80]);

        IReadOnlyList<PrsResult> expectedOriginal = OtherQuotes.ToPrs(Quotes);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHubBase.Remove(Quotes[removeAtIndex]);
        quoteHubEval.Remove(OtherQuotes[removeAtIndex]);
        IReadOnlyList<PrsResult> expectedRevised = RevisedOtherQuotes.ToPrs(RevisedQuotes);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubBase.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserver_WithLookbackPeriod_MatchesSeriesExactly()
    {
        const int lookbackPeriods = 14;

        // setup quote provider hubs
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubBase = new();

        // prefill quotes at both providers
        quoteHubEval.Add(OtherQuotes.Take(20));
        quoteHubBase.Add(Quotes.Take(20));

        // initialize observer with lookback
        PrsHub observer = quoteHubEval.ToPrsHub(quoteHubBase, lookbackPeriods);

        // emulate adding quotes to provider hubs
        for (int i = 20; i < quotesCount; i++)
        {
            Quote qEval = OtherQuotes[i];
            Quote qBase = Quotes[i];

            // IMPORTANT: Add to base BEFORE eval
            quoteHubBase.Add(qBase);
            quoteHubEval.Add(qEval);
        }

        // final results
        IReadOnlyList<PrsResult> sut = observer.Results;

        // time-series, for comparison
        IReadOnlyList<PrsResult> expected = OtherQuotes.ToPrs(Quotes, lookbackPeriods);

        // assert, should equal series
        sut.IsExactly(expected);
        sut.Should().HaveCount(quotesCount);

        // cleanup
        observer.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubBase.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 12;

        // setup quote provider hubs
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubBase = new();

        // initialize observer with chained provider
        PrsHub observer = quoteHubEval
            .ToEmaHub(emaPeriods)
            .ToPrsHub(quoteHubBase);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++)
        {
            // IMPORTANT: Add to base BEFORE eval
            quoteHubBase.Add(Quotes[i]);
            quoteHubEval.Add(OtherQuotes[i]);
        }

        // final results
        IReadOnlyList<PrsResult> sut = observer.Results;

        // time-series, for comparison
        IReadOnlyList<PrsResult> expected = OtherQuotes
            .ToEma(emaPeriods)
            .ToPrs(Quotes);

        // assert, should equal series
        sut.IsExactly(expected);
        sut.Should().HaveCount(quotesCount);

        // cleanup
        observer.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubBase.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int emaPeriods = 12;

        // setup quote provider hubs
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubBase = new();

        // initialize observer with PRS as provider
        EmaHub observer = quoteHubEval
            .ToPrsHub(quoteHubBase)
            .ToEmaHub(emaPeriods);

        // emulate adding quotes to provider hubs
        for (int i = 0; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote qEval = OtherQuotes[i];
            Quote qBase = Quotes[i];

            // IMPORTANT: Add to base BEFORE eval
            quoteHubBase.Add(qBase);
            quoteHubEval.Add(qEval);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHubBase.Add(qBase);
                quoteHubEval.Add(qEval);
            }
        }

        // late arrival
        quoteHubBase.Insert(Quotes[80]);
        quoteHubEval.Insert(OtherQuotes[80]);

        // delete
        quoteHubBase.Remove(Quotes[removeAtIndex]);
        quoteHubEval.Remove(OtherQuotes[removeAtIndex]);

        // final results
        IReadOnlyList<EmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<EmaResult> expected = RevisedOtherQuotes
            .ToPrs(RevisedQuotes)
            .ToEma(emaPeriods);

        // assert, should equal series
        sut.Should().HaveCount(quotesCount - 1);
        sut.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubBase.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub hubEval = new();
        QuoteHub hubBase = new();
        PrsHub hub = new(hubEval, hubBase, int.MinValue);
        hub.ToString().Should().Be("PRS");

        PrsHub hubWithLookback = new(hubEval, hubBase, 14);
        hubWithLookback.ToString().Should().Be("PRS(14)");
    }
}
