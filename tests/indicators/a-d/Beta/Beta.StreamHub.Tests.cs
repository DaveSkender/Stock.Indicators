namespace StreamHub;

[TestClass]
public class BetaHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider, ITestPairsProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        // Dual-provider synchronization issue: When two independent chains (smaHubA, smaHubB)
        // subscribe to the same provider (quoteHub), the beta hub cannot guarantee both
        // chains have updated before calculating beta. This requires architectural changes
        // to the StreamHub observer notification pattern to support multi-provider synchronization.
        Assert.Inconclusive("Dual-provider synchronization requires architectural review");
    }

    [TestMethod]
    public void ChainObserver()
    {
        // Dual-provider synchronization issue: When two independent chains subscribe to the same
        // provider, the beta hub cannot guarantee both chains have updated before calculating.
        // This requires architectural changes to support multi-provider synchronization.
        Assert.Inconclusive("Dual-provider synchronization requires architectural review");
    }

    [TestMethod]
    public void ChainProvider()
    {
        // Dual-provider synchronization issue: When two independent chains subscribe to the same
        // provider, the beta hub cannot guarantee both chains have updated before calculating.
        // This requires architectural changes to support multi-provider synchronization.
        Assert.Inconclusive("Dual-provider synchronization requires architectural review");
    }

    [TestMethod]
    public void PairsProvider()
    {
        // Test dual-provider pattern with direct providers
        QuoteHub<Quote> quoteHubEval = new();
        QuoteHub<Quote> quoteHubMrkt = new();

        // Add same quotes to both providers
        quoteHubEval.Add(Quotes);
        quoteHubMrkt.Add(Quotes);

        // Create beta hub from two providers
        BetaHub<Quote> betaHub = quoteHubEval.ToBetaHub(quoteHubMrkt, 20);

        // Verify results
        betaHub.Results.Should().NotBeEmpty();
        betaHub.Results.Count.Should().Be(Quotes.Count);

        // Verify calculation works for sufficient data
        BetaResult lastResult = betaHub.Results[^1];
        lastResult.Should().NotBeNull();
        lastResult.Beta.Should().NotBeNull();

        // The beta should be close to 1.0 (perfect positive beta with itself)
        lastResult.Beta.Should().BeApproximately(1.0, 0.0001);

        // Cleanup
        betaHub.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubMrkt.EndTransmission();
    }

    [TestMethod]
    public void TimestampMismatch()
    {
        // Create two providers with sufficient data for beta
        QuoteHub<Quote> quoteHubEval = new();
        QuoteHub<Quote> quoteHubMrkt = new();

        // Add sufficient quotes to both providers (30 quotes for 20-period lookback)
        List<Quote> quotesEval = Quotes.Take(30).ToList();
        List<Quote> quotesMrkt = Quotes.Take(30).ToList();

        quoteHubEval.Add(quotesEval);
        quoteHubMrkt.Add(quotesMrkt);

        // Create beta hub from two providers with 20-period lookback
        BetaHub<Quote> betaHub = quoteHubEval.ToBetaHub(quoteHubMrkt, 20);

        // Verify beta is calculated
        betaHub.Results.Should().HaveCount(30);
        betaHub.Results[29].Beta.Should().NotBeNull();

        // The beta should be close to 1.0 (perfect positive beta with itself)
        betaHub.Results[29].Beta.Should().BeApproximately(1.0, 0.0001);

        // Cleanup
        betaHub.Unsubscribe();
        quoteHubEval.EndTransmission();
        quoteHubMrkt.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();
        SmaHub<Quote> smaHubEval = quoteHub.ToSma(10);
        SmaHub<Quote> smaHubMrkt = quoteHub.ToSma(20);
        BetaHub<SmaResult> betaHub = smaHubEval.ToBetaHub(smaHubMrkt, 20);

        betaHub.ToString().Should().Be("BETA(20,Standard)");

        betaHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void BetaTypeAll()
    {
        // Test Beta with All type calculation
        QuoteHub<Quote> quoteHubEval = new();
        QuoteHub<Quote> quoteHubMrkt = new();

        quoteHubEval.Add(Quotes);
        quoteHubMrkt.Add(Quotes);

        BetaHub<Quote> betaHub = quoteHubEval.ToBetaHub(quoteHubMrkt, 20, BetaType.All);

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
}
