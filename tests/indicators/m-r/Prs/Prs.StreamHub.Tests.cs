
namespace StreamHubs;

[TestClass]
public class PrsHubTests : StreamHubTestBase, ITestPairsObserver
{
    [TestMethod]
    public void PairsObserver_SynchronizedProviders_MatchesSeriesExactly()
    {
        const int lookbackPeriods = 10;

        IReadOnlyList<Quote> quotesEval = Quotes;
        IReadOnlyList<Quote> quotesBase = OtherQuotes;

        List<Quote> quotesEvalRevised = quotesEval.ToList(); quotesEvalRevised.RemoveAt(removeAtIndex);
        List<Quote> quotesBaseRevised = quotesBase.ToList(); quotesBaseRevised.RemoveAt(removeAtIndex);

        // setup quote provider hubs
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubBase = new();

        // prefill quotes at providers
        quoteHubEval.Add(quotesEval.Take(20));
        quoteHubBase.Add(quotesBase.Take(20));

        // initialize observer
        PrsHub observer = quoteHubEval.ToPrsHub(quoteHubBase, lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<PrsResult> sut = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < quotesCount; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote qe = quotesEval[i];
            Quote qb = quotesBase[i];

            quoteHubEval.Add(qe);
            quoteHubBase.Add(qb);

            // resend duplicate quotes for eval
            if (i is > 95 and < 103) { quoteHubEval.Add(qe); }

            // resend duplicate quotes for base, inconsistently
            if (i is > 100 and < 105) { quoteHubEval.Add(qb); }
        }

        // late arrival, should equal series
        quoteHubEval.Insert(quotesEval[80]);
        quoteHubBase.Insert(quotesBase[80]);

        IReadOnlyList<PrsResult> expectedOriginal = quotesEval.ToPrs(quotesBase, lookbackPeriods);
        sut.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHubEval.Remove(quotesEval[removeAtIndex]);
        quoteHubBase.Remove(quotesBase[removeAtIndex]);

        // TODO: test and handle matching removals in paired observers
        // Removing from one side should auto-remove from both sides; however, there's a challenging synchronization issue here overall.
        // If there's a "fix" situation, where one side is then restored we'd need to remember and restore.
        // Rules we really need:
        // 1. Adding a quote to one side is "staged" but not finalized until matching quote is added to other side.
        // 2. Removing a quote from one side automatically re-"stages" from other side (waiting)
        // 3. It's likely okay to "skip" staged items and continue with the next matching pairs, which aligns with removing an old one that's now staged.

        // also revise quotes for expected calculation
        IReadOnlyList<PrsResult> expectedRevised
            = quotesEvalRevised.ToPrs(quotesBaseRevised, lookbackPeriods);

        sut.IsExactly(expectedRevised);
        sut.Should().HaveCount(quotesCount - 1);

        // cleanup
        observer.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubBase.EndTransmission();
    }

    [TestMethod]
    public void PairsObserver_TimestampMismatch_ThrowsInvalidQuotesException()
    {
        // Create two providers with mismatched timestamps
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubBase = new();

        // Add quotes with offset timestamps to force mismatch
        List<Quote> quotesEval = Quotes.Take(30).ToList();
        List<Quote> quotesBase = Quotes.Take(30).Select(q => new Quote {
            Timestamp = q.Timestamp.AddDays(1), // Offset timestamps by 1 day
            Open = q.Open,
            High = q.High,
            Low = q.Low,
            Close = q.Close,
            Volume = q.Volume
        }).ToList();

        quoteHubEval.Add(quotesEval);
        quoteHubBase.Add(quotesBase);

        // Creating PRS hub should trigger timestamp validation during Reinitialize and throw exception
        Action act = () => quoteHubEval.ToPrsHub(quoteHubBase, 10);

        act.Should().Throw<InvalidQuotesException>()
            .WithMessage("*Timestamp sequence does not match*");

        // Cleanup
        quoteHubEval.EndTransmission();
        quoteHubBase.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        SmaHub smaHubEval = quoteHub.ToSmaHub(10);
        SmaHub smaHubBase = quoteHub.ToSmaHub(20);

        // Without lookback
        PrsHub prsHub1 = smaHubEval.ToPrsHub(smaHubBase);
        prsHub1.ToString().Should().Be("PRS");

        // With lookback
        PrsHub prsHub2 = smaHubEval.ToPrsHub(smaHubBase, 20);
        prsHub2.ToString().Should().Be("PRS(20)");

        prsHub1.Unsubscribe();
        prsHub2.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void WithLookbackPeriods()
    {
        // Test PRS with lookback periods for PrsPercent calculation
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubBase = new();

        quoteHubEval.Add(Quotes);
        quoteHubBase.Add(Quotes);

        PrsHub prsHub = quoteHubEval.ToPrsHub(quoteHubBase, 20);

        // Verify PrsPercent is calculated after lookback periods
        prsHub.Results.Should().NotBeEmpty();
        prsHub.Results.Count.Should().Be(Quotes.Count);

        // Check that PrsPercent is null before lookback periods
        for (int i = 0; i < 20; i++)
        {
            prsHub.Results[i].PrsPercent.Should().BeNull();
        }

        // Check that PrsPercent is calculated after lookback periods
        for (int i = 20; i < Quotes.Count; i++)
        {
            if (prsHub.Results[i].Prs.HasValue)
            {
                // PrsPercent should exist when Prs exists (after lookback)
                prsHub.Results[i].PrsPercent.Should().NotBeNull();

                // When comparing identical series, PrsPercent should be exactly 0
                // because (pctEval - pctBase) = 0 when both series are identical
                prsHub.Results[i].PrsPercent.Should().Be(0.0,
                    $"PrsPercent should be exactly 0 for identical series at index {i}");
            }
        }

        // Cleanup
        prsHub.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubBase.EndTransmission();
    }

    [TestMethod]
    public void PairsObserver_WithSameProvider_HasFlatlineResults()
    {
        PrsHub prsHub = Quotes.ToPrsHub(Quotes, 20);
        IReadOnlyList<PrsResult> sut = prsHub.Results;

        // all values should be flatlined
        sut.TakeLast(quotesCount - 20)
           .Should().AllSatisfy(r => {
               r.Prs.Should().Be(1);
               r.PrsPercent.Should().Be(0);
           });
    }
}
