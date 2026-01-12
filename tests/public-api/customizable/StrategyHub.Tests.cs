namespace Customization;

// STRATEGY HUB

[TestClass, TestCategory("Integration")]
public class StrategyHubTests : TestBase
{
    [TestMethod]
    public void AlignsLatestPairs()
    {
        QuoteHub quoteHub = new();
        StrategyHub<IQuote> strategy = new(quoteHub);

        const int fastLookback = 10;
        const int slowLookback = 20;

        EmaHub fastHub = strategy.Use<EmaHub, EmaResult>(quoteHub.ToEmaHub(fastLookback));
        EmaHub slowHub = strategy.Use<EmaHub, EmaResult>(quoteHub.ToEmaHub(slowLookback));

        const int sampleSize = 60;

        for (int i = 0; i < sampleSize; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        bool hasPairs = strategy.TryGetLatest(
            fastHub,
            slowHub,
            out (EmaResult previous, EmaResult current) fastPair,
            out (EmaResult previous, EmaResult current) slowPair);

        hasPairs.Should().BeTrue();

        fastPair.previous.Should().Be(fastHub.Results[^2]);
        fastPair.current.Should().Be(fastHub.Results[^1]);
        slowPair.previous.Should().Be(slowHub.Results[^2]);
        slowPair.current.Should().Be(slowHub.Results[^1]);

        bool strategyCross = fastPair.previous.Ema < slowPair.previous.Ema
            && fastPair.current.Ema > slowPair.current.Ema;

        bool manualCross = fastHub.Results[^2].Ema < slowHub.Results[^2].Ema
            && fastHub.Results[^1].Ema > slowHub.Results[^1].Ema;

        strategyCross.Should().Be(manualCross);
    }
}
