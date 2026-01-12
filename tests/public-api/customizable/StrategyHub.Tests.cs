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

    [TestMethod]
    public void GoldenCross()
    {
        // Buy when the fast SMA crosses above the slow SMA
        // Sell when the fast SMA crosses below the slow SMA

        double balance = 10000.0;
        double units = 0.0;

        QuoteHub quoteHub = new();
        StrategyHub<IQuote> strategy = new(quoteHub);

        const int fastLookback = 50;
        const int slowLookback = 200;

        SmaHub fastHub = strategy.Use<SmaHub, SmaResult>(quoteHub.ToSmaHub(fastLookback));
        SmaHub slowHub = strategy.Use<SmaHub, SmaResult>(quoteHub.ToSmaHub(slowLookback));

        int length = LongestQuotes.Count;

        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(LongestQuotes[i]);
        }

        bool hasPairs = strategy.TryGetLatest(
            fastHub,
            slowHub,
            out (SmaResult previous, SmaResult current) fastPair,
            out (SmaResult previous, SmaResult current) slowPair);

        hasPairs.Should().BeTrue();

        fastPair.previous.Should().Be(fastHub.Results[^2]);
        fastPair.current.Should().Be(fastHub.Results[^1]);
        slowPair.previous.Should().Be(slowHub.Results[^2]);
        slowPair.current.Should().Be(slowHub.Results[^1]);

        bool crossover = fastPair.previous.Sma <= slowPair.previous.Sma
            && fastPair.current.Sma > slowPair.current.Sma;

        bool crossunder = fastPair.previous.Sma >= slowPair.previous.Sma
            && fastPair.current.Sma < slowPair.current.Sma;

        double price = strategy.Provider.Results[^1].Value;

        if (crossover)
        {
            // Place buy order
            units = balance / price;
        }
        else if (crossunder)
        {
            // Place sell order
            units = 0;
        }

        if (units > 0)
        {
            balance = units * price;
        }

        Console.WriteLine("Balance is {0:N6}", balance);
    }
}
