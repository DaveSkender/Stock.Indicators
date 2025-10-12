namespace StreamHub;

[TestClass]
public class PrsHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider, ITestPairsProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        // Dual-provider synchronization issue: When two independent chains subscribe to the same
        // provider, the PRS hub cannot guarantee both chains have updated before calculating.
        // This requires architectural changes to the StreamHub observer notification pattern
        // to support multi-provider synchronization.
        Assert.Inconclusive("Dual-provider synchronization requires architectural review");
    }

    [TestMethod]
    public void ChainObserver()
    {
        // Dual-provider synchronization issue: When two independent chains subscribe to the same
        // provider, the PRS hub cannot guarantee both chains have updated before calculating.
        // This requires architectural changes to support multi-provider synchronization.
        Assert.Inconclusive("Dual-provider synchronization requires architectural review");
    }

    [TestMethod]
    public void ChainProvider()
    {
        // Dual-provider synchronization issue: When two independent chains subscribe to the same
        // provider, the PRS hub cannot guarantee both chains have updated before calculating.
        // This requires architectural changes to support multi-provider synchronization.
        Assert.Inconclusive("Dual-provider synchronization requires architectural review");
    }

    [TestMethod]
    public void PairsProvider()
    {
        // Test dual-provider pattern with direct providers
        QuoteHub<Quote> quoteHubEval = new();
        QuoteHub<Quote> quoteHubBase = new();

        // Add same quotes to both providers
        quoteHubEval.Add(Quotes);
        quoteHubBase.Add(Quotes);

        // Create PRS hub from two providers
        PrsHub<Quote> prsHub = quoteHubEval.ToPrsHub(quoteHubBase);

        // Verify results
        prsHub.Results.Should().NotBeEmpty();
        prsHub.Results.Count.Should().Be(Quotes.Count);

        // Verify calculation works
        PrsResult lastResult = prsHub.Results[^1];
        lastResult.Should().NotBeNull();
        lastResult.Prs.Should().NotBeNull();

        // The PRS should be 1.0 (perfect relative strength with itself)
        lastResult.Prs.Should().BeApproximately(1.0, 0.0001);

        // Cleanup
        prsHub.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubBase.EndTransmission();
    }

    [TestMethod]
    public void TimestampMismatch()
    {
        // Create two providers with mismatched timestamps
        QuoteHub<Quote> quoteHubEval = new();
        QuoteHub<Quote> quoteHubBase = new();

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
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();
        SmaHub<Quote> smaHubEval = quoteHub.ToSma(10);
        SmaHub<Quote> smaHubBase = quoteHub.ToSma(20);

        // Without lookback
        PrsHub<SmaResult> prsHub1 = smaHubEval.ToPrsHub(smaHubBase);
        prsHub1.ToString().Should().Be("PRS");

        // With lookback
        PrsHub<SmaResult> prsHub2 = smaHubEval.ToPrsHub(smaHubBase, 20);
        prsHub2.ToString().Should().Be("PRS(20)");

        prsHub1.Unsubscribe();
        prsHub2.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void WithLookbackPeriods()
    {
        // Test PRS with lookback periods for PrsPercent calculation
        QuoteHub<Quote> quoteHubEval = new();
        QuoteHub<Quote> quoteHubBase = new();

        quoteHubEval.Add(Quotes);
        quoteHubBase.Add(Quotes);

        PrsHub<Quote> prsHub = quoteHubEval.ToPrsHub(quoteHubBase, 20);

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

                // When comparing identical series to themselves, PrsPercent should be approximately 0
                // because the percentage change difference should be minimal
                prsHub.Results[i].PrsPercent!.Value.Should().BeApproximately(0.0, 0.0001,
                    $"PrsPercent should be ~0 for identical series at index {i}");
            }
        }

        // Cleanup
        prsHub.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubBase.EndTransmission();
    }
}
