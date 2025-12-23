namespace StreamHub;

[TestClass]
public class BetaHubTests : StreamHubTestBase, ITestPairsObserver
{
    [TestMethod]
    public void PairsObserver_SynchronizedProviders_MatchesSeriesExactly()
    {
        // Test dual-provider pattern with direct providers
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubMrkt = new();

        // Add same quotes to both providers
        quoteHubEval.Add(Quotes);
        quoteHubMrkt.Add(Quotes);

        // Create beta hub from two providers
        BetaHub betaHub = quoteHubEval.ToBetaHub(quoteHubMrkt, 20);

        // Verify results
        betaHub.Results.Should().NotBeEmpty();
        betaHub.Results.Count.Should().Be(Quotes.Count);

        // Verify calculation works for sufficient data
        BetaResult lastResult = betaHub.Results[^1];
        lastResult.Should().NotBeNull();
        lastResult.Beta.Should().NotBeNull();

        // When comparing identical series, beta should be exactly 1.0
        lastResult.Beta.Should().Be(1.0);

        // Cleanup
        betaHub.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubMrkt.EndTransmission();
    }

    [TestMethod]
    public void PairsObserver_TimestampMismatch_ThrowsInvalidQuotesException()
    {
        // Create two providers with mismatched timestamps
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubMrkt = new();

        // Add quotes with offset timestamps to force mismatch
        List<Quote> quotesEval = Quotes.Take(30).ToList();
        List<Quote> quotesMrkt = Quotes.Take(30).Select(q => new Quote {
            Timestamp = q.Timestamp.AddDays(1), // Offset timestamps by 1 day
            Open = q.Open,
            High = q.High,
            Low = q.Low,
            Close = q.Close,
            Volume = q.Volume
        }).ToList();

        quoteHubEval.Add(quotesEval);
        quoteHubMrkt.Add(quotesMrkt);

        // Creating beta hub should trigger timestamp validation during Reinitialize and throw exception
        Action act = () => quoteHubEval.ToBetaHub(quoteHubMrkt, 20);

        act.Should().Throw<InvalidQuotesException>()
            .WithMessage("*Timestamp sequence does not match*");

        // Cleanup
        quoteHubEval.EndTransmission();
        quoteHubMrkt.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        SmaHub smaHubEval = quoteHub.ToSmaHub(10);
        SmaHub smaHubMrkt = quoteHub.ToSmaHub(20);
        BetaHub betaHub = smaHubEval.ToBetaHub(smaHubMrkt, 20);

        betaHub.ToString().Should().Be("BETA(20,Standard)");

        betaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void BetaTypeAll()
    {
        // Test Beta with All type calculation
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubMrkt = new();

        quoteHubEval.Add(Quotes);
        quoteHubMrkt.Add(Quotes);

        BetaHub betaHub = quoteHubEval.ToBetaHub(quoteHubMrkt, 20, BetaType.All);

        // Verify all beta types are calculated
        betaHub.Results.Should().NotBeEmpty();
        BetaResult lastResult = betaHub.Results[^1];

        lastResult.Beta.Should().NotBeNull();
        lastResult.BetaUp.Should().NotBeNull();
        lastResult.BetaDown.Should().NotBeNull();
        lastResult.Ratio.Should().NotBeNull();
        lastResult.Convexity.Should().NotBeNull();

        // Cleanup
        betaHub.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubMrkt.EndTransmission();
    }

    [TestMethod]
    public void InsufficientData()
    {
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubMrkt = new();

        // Add only 10 quotes for 20-period lookback
        List<Quote> insufficientQuotes = Quotes.Take(10).ToList();
        quoteHubEval.Add(insufficientQuotes);
        quoteHubMrkt.Add(insufficientQuotes);

        BetaHub betaHub = quoteHubEval.ToBetaHub(quoteHubMrkt, 20);

        // Verify beta is null when insufficient data
        betaHub.Results.Should().HaveCount(10);
        betaHub.Results.Should().AllSatisfy(static r => r.Beta.Should().BeNull());

        betaHub.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubMrkt.EndTransmission();
    }

    [TestMethod]
    public void SequentialQuoteProcessing()
    {
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubMrkt = new();

        // Add quotes one by one to verify stateful processing
        // Add to both providers before creating beta hub to ensure synchronization
        foreach (Quote quote in Quotes.Take(50))
        {
            quoteHubEval.Add(quote);
            quoteHubMrkt.Add(quote);
        }

        // Create beta hub after data is added to ensure proper synchronization
        BetaHub betaHub = quoteHubEval.ToBetaHub(quoteHubMrkt, 20);

        betaHub.Results.Should().HaveCount(50);

        // Verify that results are generated for all periods
        betaHub.Results.Should().OnlyContain(static r => r.Timestamp != default);

        // Verify returns are calculated after first period
        betaHub.Results.Skip(1).Should().OnlyContain(static r => r.ReturnsEval.HasValue || r.ReturnsEval == 0);
        betaHub.Results.Skip(1).Should().OnlyContain(static r => r.ReturnsMrkt.HasValue || r.ReturnsMrkt == 0);

        betaHub.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubMrkt.EndTransmission();
    }

    [TestMethod]
    public void ConsistentWithSeriesCalculation()
    {
        QuoteHub quoteHubEval = new();
        QuoteHub quoteHubMrkt = new();

        quoteHubEval.Add(Quotes);
        quoteHubMrkt.Add(Quotes);

        BetaHub betaHub = quoteHubEval.ToBetaHub(quoteHubMrkt, 20);

        // Compare with series calculation
        List<BetaResult> seriesResults = Quotes.ToBeta(Quotes, 20).ToList();

        betaHub.Results.Should().HaveCount(seriesResults.Count);

        // Check consistency for results where both have values
        // Hub and series should produce identical results
        for (int i = 20; i < betaHub.Results.Count && i < 100; i++)
        {
            BetaResult hubResult = betaHub.Results[i];
            BetaResult seriesResult = seriesResults[i];

            // Both should have beta values after warmup period
            if (hubResult.Beta.HasValue && seriesResult.Beta.HasValue)
            {
                hubResult.Beta.Should().Be(seriesResult.Beta, $"Beta at index {i}");
            }

            // Verify returns match as well
            hubResult.ReturnsEval.Should().Be(seriesResult.ReturnsEval, $"ReturnsEval at index {i}");
            hubResult.ReturnsMrkt.Should().Be(seriesResult.ReturnsMrkt, $"ReturnsMrkt at index {i}");
        }

        betaHub.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubMrkt.EndTransmission();
    }
}
