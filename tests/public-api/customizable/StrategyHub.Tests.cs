namespace Customization;

// STRATEGY HUB

[TestClass, TestCategory("Integration")]
public class StrategyHubTests : TestBase
{
    [TestMethod]
    public void AlignsLatestPairs()
    {
        QuoteHub quoteHub = new();

        const int fastLookback = 10;
        const int slowLookback = 20;

        EmaHub fastHub = quoteHub.ToEmaHub(fastLookback);
        EmaHub slowHub = quoteHub.ToEmaHub(slowLookback);
        StrategyGroup<EmaResult, EmaResult> group = new(fastHub, slowHub);

        const int sampleSize = 60;

        for (int i = 0; i < sampleSize; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        bool hasPairs = group.TryGetBackPair(
            out BackPair<EmaResult> fastPair,
            out BackPair<EmaResult> slowPair);

        hasPairs.Should().BeTrue();

        fastPair.Previous.Should().Be(fastHub.Results[^2]);
        fastPair.Current.Should().Be(fastHub.Results[^1]);
        slowPair.Previous.Should().Be(slowHub.Results[^2]);
        slowPair.Current.Should().Be(slowHub.Results[^1]);

        bool strategyCross = fastPair.Previous.Ema < slowPair.Previous.Ema
            && fastPair.Current.Ema > slowPair.Current.Ema;

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

        const int fastLookback = 50;
        const int slowLookback = 200;

        SmaHub fastHub = quoteHub.ToSmaHub(fastLookback);
        SmaHub slowHub = quoteHub.ToSmaHub(slowLookback);
        StrategyGroup<SmaResult, SmaResult> group = new(fastHub, slowHub);

        int length = LongestQuotes.Count;

        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(LongestQuotes[i]);
        }

        bool hasPairs = group.TryGetBackPair(
            out BackPair<SmaResult> fastPair,
            out BackPair<SmaResult> slowPair);

        hasPairs.Should().BeTrue();

        fastPair.Previous.Should().Be(fastHub.Results[^2]);
        fastPair.Current.Should().Be(fastHub.Results[^1]);
        slowPair.Previous.Should().Be(slowHub.Results[^2]);
        slowPair.Current.Should().Be(slowHub.Results[^1]);

        bool crossover = fastPair.Previous.Sma <= slowPair.Previous.Sma
            && fastPair.Current.Sma > slowPair.Current.Sma;

        bool crossunder = fastPair.Previous.Sma >= slowPair.Previous.Sma
            && fastPair.Current.Sma < slowPair.Current.Sma;

        double price = (double)quoteHub.Results[^1].Close;

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
