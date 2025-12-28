namespace StreamHubs;

[TestClass]
public class DpoHubTests : StreamHubTestBase, ITestChainObserver, ITestChainProvider
{
    private const int lookbackPeriods = 14;

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        DpoHub observer = quoteHub.ToDpoHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<DpoResult> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
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

        IReadOnlyList<DpoResult> expectedOriginal = Quotes.ToDpo(lookbackPeriods);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<DpoResult> expectedRevised = RevisedQuotes.ToDpo(lookbackPeriods);
        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver_ChainedProvider_MatchesSeriesExactly()
    {
        const int dpoPeriods = 14;
        const int smaPeriods = 8;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        DpoHub observer = quoteHub
            .ToSmaHub(smaPeriods)
            .ToDpoHub(dpoPeriods);

        // emulate quote stream
        for (int i = 0; i < quotesCount; i++) { quoteHub.Add(Quotes[i]); }

        // final results
        IReadOnlyList<DpoResult> actuals = observer.Results;

        // time-series, for comparison
        IReadOnlyList<DpoResult> expected = Quotes
            .ToSma(smaPeriods)
            .ToDpo(dpoPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(quotesCount);
        actuals.IsExactly(expected);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        // DpoHub with downstream chained observers (e.g., SmaHub) now correctly handles
        // provider history mutations (Insert/Remove) after making StreamHub.Rebuild() and
        // OnRebuild() virtual methods. DpoHub overrides Rebuild() to notify downstream
        // observers of the adjusted rebuild position (accounting for backward offset).
        //
        // Example: When Insert(Quotes[80]) occurs with offset=11:
        //   1. DpoHub.RollbackState() correctly removes cache from position 69 ✓
        //   2. DpoHub.OnAdd() recalculates positions [69, 80] ✓
        //   3. DpoHub.Rebuild() notifies downstream from adjusted position 69 ✓
        //   4. Downstream SmaHub recalculates [69, end] correctly ✓

        const int dpoPeriods = 20;
        const int smaPeriods = 10;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToDpoHub(dpoPeriods)
            .ToSmaHub(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // final results
        IReadOnlyList<SmaResult> sut = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> expected = RevisedQuotes
            .ToDpo(dpoPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        sut.IsExactly(expected);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        DpoHub hub = new(new QuoteHub(), 14);
        hub.ToString().Should().Be("DPO(14)");
    }
}
