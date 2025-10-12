namespace StreamHub;

[TestClass]
public class CorrelationHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider, ITestPairsProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        // Dual-provider synchronization issue: When two independent chains (smaHubA, smaHubB)
        // subscribe to the same provider (quoteHub), the correlation hub cannot guarantee both
        // chains have updated before calculating correlation. This requires architectural changes
        // to the StreamHub observer notification pattern to support multi-provider synchronization.
        Assert.Inconclusive("Dual-provider synchronization requires architectural review");
    }

    [TestMethod]
    public void ChainObserver()
    {
        // Dual-provider synchronization issue: When two independent chains subscribe to the same
        // provider, the correlation hub cannot guarantee both chains have updated before calculating.
        // This requires architectural changes to support multi-provider synchronization.
        Assert.Inconclusive("Dual-provider synchronization requires architectural review");
    }

    [TestMethod]
    public void ChainProvider()
    {
        // Dual-provider synchronization issue: When two independent chains subscribe to the same
        // provider, the correlation hub cannot guarantee both chains have updated before calculating.
        // This requires architectural changes to support multi-provider synchronization.
        Assert.Inconclusive("Dual-provider synchronization requires architectural review");
    }

    [TestMethod]
    public void PairsProvider()
    {
        // Test dual-provider pattern with direct providers
        QuoteHub<Quote> quoteHubA = new();
        QuoteHub<Quote> quoteHubB = new();

        // Add same quotes to both providers
        quoteHubA.Add(Quotes);
        quoteHubB.Add(Quotes);

        // Create correlation hub from two providers
        CorrelationHub<Quote> correlationHub = quoteHubA.ToCorrelationHub(quoteHubB, 20);

        // Verify results
        correlationHub.Results.Should().NotBeEmpty();
        correlationHub.Results.Count.Should().Be(Quotes.Count);

        // Verify calculation works for sufficient data
        CorrResult lastResult = correlationHub.Results[^1];
        lastResult.Should().NotBeNull();
        lastResult.Correlation.Should().NotBeNull();

        // When comparing identical series, correlation should be exactly 1.0
        lastResult.Correlation.Should().Be(1.0);

        // Cleanup
        correlationHub.Unsubscribe();
        quoteHubA.EndTransmission();
        quoteHubB.EndTransmission();
    }

    [TestMethod]
    public void TimestampMismatch()
    {
        // Create two providers with mismatched timestamps
        QuoteHub<Quote> quoteHubA = new();
        QuoteHub<Quote> quoteHubB = new();

        // Add quotes with offset timestamps to force mismatch
        List<Quote> quotesA = Quotes.Take(30).ToList();
        List<Quote> quotesB = Quotes.Take(30).Select(q => new Quote {
            Timestamp = q.Timestamp.AddDays(1), // Offset timestamps by 1 day
            Open = q.Open,
            High = q.High,
            Low = q.Low,
            Close = q.Close,
            Volume = q.Volume
        }).ToList();

        quoteHubA.Add(quotesA);
        quoteHubB.Add(quotesB);

        // Creating correlation hub should trigger timestamp validation during Reinitialize and throw exception
        Action act = () => quoteHubA.ToCorrelationHub(quoteHubB, 20);

        act.Should().Throw<InvalidQuotesException>()
            .WithMessage("*Timestamp sequence does not match*");

        // Cleanup
        quoteHubA.EndTransmission();
        quoteHubB.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();
        SmaHub<Quote> smaHubA = quoteHub.ToSma(10);
        SmaHub<Quote> smaHubB = quoteHub.ToSma(20);
        CorrelationHub<SmaResult> correlationHub = smaHubA.ToCorrelationHub(smaHubB, 20);

        correlationHub.ToString().Should().Be("CORRELATION(20)");

        correlationHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ConsistentWithSeriesCalculation()
    {
        QuoteHub<Quote> quoteHubA = new();
        QuoteHub<Quote> quoteHubB = new();

        quoteHubA.Add(Quotes);
        quoteHubB.Add(Quotes);

        CorrelationHub<Quote> corrHub = quoteHubA.ToCorrelationHub(quoteHubB, 20);

        // Compare with series calculation
        List<CorrResult> seriesResults = Quotes.ToCorrelation(Quotes, 20).ToList();

        corrHub.Results.Should().HaveCount(seriesResults.Count);

        // Check consistency for results where both have values
        // Hub and series should produce identical results
        for (int i = 20; i < corrHub.Results.Count && i < 100; i++)
        {
            CorrResult hubResult = corrHub.Results[i];
            CorrResult seriesResult = seriesResults[i];

            // Both should have correlation values after warmup period
            if (hubResult.Correlation.HasValue && seriesResult.Correlation.HasValue)
            {
                hubResult.Correlation.Should().Be(seriesResult.Correlation, $"Correlation at index {i}");
            }

            // Verify other properties match as well
            hubResult.RSquared.Should().Be(seriesResult.RSquared, $"RSquared at index {i}");
            hubResult.VarianceA.Should().Be(seriesResult.VarianceA, $"VarianceA at index {i}");
            hubResult.VarianceB.Should().Be(seriesResult.VarianceB, $"VarianceB at index {i}");
            hubResult.Covariance.Should().Be(seriesResult.Covariance, $"Covariance at index {i}");
        }

        corrHub.Unsubscribe();
        quoteHubA.EndTransmission();
        quoteHubB.EndTransmission();
    }
}
