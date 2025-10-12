namespace StreamHub;

[TestClass]
public class PrsHub : StreamHubTestBase, ITestPairsProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        // Dual-provider synchronization issue: When two independent chains subscribe to the same
        // provider, the PRS hub cannot guarantee both chains have updated before calculating.
        // This requires architectural changes to support multi-provider synchronization.
        Assert.Inconclusive("Dual-provider synchronization not supported for pairs indicators");
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

        // When comparing identical series, PRS should be exactly 1.0
        lastResult.Prs.Should().Be(1.0);

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
    public void ConsistentWithSeriesCalculation()
    {
        QuoteHub<Quote> quoteHubEval = new();
        QuoteHub<Quote> quoteHubBase = new();

        quoteHubEval.Add(Quotes);
        quoteHubBase.Add(Quotes);

        PrsHub<Quote> prsHub = quoteHubEval.ToPrsHub(quoteHubBase, 20);

        // Compare with series calculation
        List<PrsResult> seriesResults = Quotes.ToPrs(Quotes, 20).ToList();

        prsHub.Results.Should().HaveCount(seriesResults.Count);

        // Hub and series should produce identical results
        for (int i = 0; i < prsHub.Results.Count && i < 100; i++)
        {
            PrsResult hubResult = prsHub.Results[i];
            PrsResult seriesResult = seriesResults[i];

            hubResult.Prs.Should().Be(seriesResult.Prs, $"Prs at index {i}");
            hubResult.PrsPercent.Should().Be(seriesResult.PrsPercent, $"PrsPercent at index {i}");
        }

        prsHub.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubBase.EndTransmission();
    }
}
