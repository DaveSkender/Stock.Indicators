namespace StreamHub;

[TestClass]
public class Stoch : StreamHubTestBase
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = Quotes.Count;

        // add base quotes (batch)
        QuoteHub<Quote> provider = new();
        provider.Add(quotesList.Take(200));

        // add incremental quotes
        for (int i = 200; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        StochHub<Quote> observer = provider.ToStoch(14, 3, 3);

        // close observations
        provider.EndTransmission();

        // assert results
        observer.Cache.Should().HaveCount(length);

        // verify against static series calculation
        IReadOnlyList<StochResult> expected = Quotes.ToStoch(14, 3, 3);
        observer.Cache.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public override void CustomToString()
    {
        StochHub<Quote> hub = new(new QuoteHub<Quote>(), 14, 3, 3);
        hub.ToString().Should().Be("STOCH(14,3,3)");
    }

    [TestMethod]
    public void ExtendedParameters()
    {
        const int lookbackPeriods = 9;
        const int signalPeriods = 5;
        const int smoothPeriods = 2;
        const double kFactor = 5;
        const double dFactor = 4;
        const MaType movingAverageType = MaType.SMMA;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider
        QuoteHub<Quote> provider = new();
        provider.Add(quotesList);

        StochHub<Quote> observer = provider.ToStoch(
            lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        // close observations
        provider.EndTransmission();

        // verify against static series calculation
        IReadOnlyList<StochResult> expected = Quotes.ToStoch(
            lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        observer.Cache.Should().HaveCount(Quotes.Count);
        observer.Cache.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void IncrementalUpdates()
    {
        const int lookbackPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 3;

        List<Quote> quotesList = Quotes.ToList();

        // setup quote provider with incremental updates
        QuoteHub<Quote> provider = new();
        StochHub<Quote> observer = provider.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

        // add quotes one by one
        foreach (Quote quote in quotesList)
        {
            provider.Add(quote);
        }

        // close observations
        provider.EndTransmission();

        // verify consistency
        IReadOnlyList<StochResult> expected = Quotes.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);
        observer.Cache.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public void Properties()
    {
        const int lookbackPeriods = 21;
        const int signalPeriods = 5;
        const int smoothPeriods = 2;
        const double kFactor = 5;
        const double dFactor = 4;
        const MaType movingAverageType = MaType.SMMA;

        QuoteHub<Quote> provider = new();
        StochHub<Quote> observer = provider.ToStoch(
            lookbackPeriods, signalPeriods, smoothPeriods,
            kFactor, dFactor, movingAverageType);

        // verify properties
        observer.LookbackPeriods.Should().Be(lookbackPeriods);
        observer.SignalPeriods.Should().Be(signalPeriods);
        observer.SmoothPeriods.Should().Be(smoothPeriods);
        observer.KFactor.Should().Be(kFactor);
        observer.DFactor.Should().Be(dFactor);
        observer.MovingAverageType.Should().Be(movingAverageType);
        observer.ToString().Should().Be($"STOCH({lookbackPeriods},{signalPeriods},{smoothPeriods})");
    }

    [TestMethod]
    public void DefaultParameters()
    {
        QuoteHub<Quote> provider = new();
        StochHub<Quote> observer = provider.ToStoch();

        // verify default properties
        observer.LookbackPeriods.Should().Be(14);
        observer.SignalPeriods.Should().Be(3);
        observer.SmoothPeriods.Should().Be(3);
        observer.KFactor.Should().Be(3);
        observer.DFactor.Should().Be(2);
        observer.MovingAverageType.Should().Be(MaType.SMA);
        observer.ToString().Should().Be("STOCH(14,3,3)");
    }

    [TestMethod]
    public void StreamingAccuracy()
    {
        // Test that streaming produces accurate results compared to batch processing
        const int lookbackPeriods = 14;
        const int signalPeriods = 3;
        const int smoothPeriods = 3;

        List<Quote> quotesList = Quotes.ToList();

        // streaming calculation
        QuoteHub<Quote> provider = new();
        StochHub<Quote> streamObserver = provider.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

        foreach (Quote quote in quotesList)
        {
            provider.Add(quote);
        }

        provider.EndTransmission();

        // batch calculation  
        IReadOnlyList<StochResult> batchResults = Quotes.ToStoch(lookbackPeriods, signalPeriods, smoothPeriods);

        // compare results with reasonable precision
        streamObserver.Cache.Should().HaveCount(batchResults.Count);

        for (int i = 0; i < streamObserver.Cache.Count; i++)
        {
            StochResult streamResult = streamObserver.Cache[i];
            StochResult batchResult = batchResults[i];

            streamResult.Timestamp.Should().Be(batchResult.Timestamp);

            if (streamResult.Oscillator.HasValue && batchResult.Oscillator.HasValue)
            {
                streamResult.Oscillator.Should().BeApproximately(batchResult.Oscillator.Value, 0.001);
            }
            else
            {
                streamResult.Oscillator.Should().Be(batchResult.Oscillator);
            }

            if (streamResult.Signal.HasValue && batchResult.Signal.HasValue)
            {
                streamResult.Signal.Should().BeApproximately(batchResult.Signal.Value, 0.001);
            }
            else
            {
                streamResult.Signal.Should().Be(batchResult.Signal);
            }

            if (streamResult.PercentJ.HasValue && batchResult.PercentJ.HasValue)
            {
                streamResult.PercentJ.Should().BeApproximately(batchResult.PercentJ.Value, 0.001);
            }
            else
            {
                streamResult.PercentJ.Should().Be(batchResult.PercentJ);
            }
        }
    }

    [TestMethod]
    public void BoundaryValues()
    {
        // Test oscillator stays within 0-100 bounds
        QuoteHub<Quote> provider = new();
        StochHub<Quote> observer = provider.ToStoch(14, 3, 3);

        provider.Add(Quotes);
        provider.EndTransmission();

        foreach (StochResult result in observer.Cache)
        {
            if (result.Oscillator.HasValue)
            {
                result.Oscillator.Value.Should().BeInRange(0, 100);
            }

            if (result.Signal.HasValue)
            {
                result.Signal.Value.Should().BeInRange(0, 100);
            }
        }
    }
}
