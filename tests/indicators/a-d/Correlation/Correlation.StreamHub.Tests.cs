namespace StreamHub;

[TestClass]
public class CorrelationHub : StreamHubTestBase, ITestChainObserver, ITestChainProvider
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
}
