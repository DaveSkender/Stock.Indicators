namespace StreamHubs;

[TestClass]
public class ForceIndex : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const int lookbackPeriods = 2;
    private readonly IReadOnlyList<ForceIndexResult> expectedOriginal = Quotes.ToForceIndex(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver_WithWarmupLateArrivalAndRemoval_MatchesSeriesExactly()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        ForceIndexHub observer = quoteHub.ToForceIndexHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<ForceIndexResult> actuals = observer.Results;

        // emulate adding quotes to provider hub
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival, should equal series
        quoteHub.Insert(Quotes[80]);
        actuals.IsExactly(expectedOriginal);

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<ForceIndexResult> expectedRevised = RevisedQuotes.ToForceIndex(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.IsExactly(expectedRevised);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider_MatchesSeriesExactly()
    {
        const int forcePeriods = 13;
        const int smaPeriods = 10;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub quoteHub = new();

        // initialize observer
        SmaHub observer = quoteHub
            .ToForceIndexHub(forcePeriods)
            .ToSmaHub(smaPeriods);

        // emulate adding quotes to provider hub
        for (int i = 0; i < length; i++)
        {
            // skip one (add later)
            if (i == 80) { continue; }

            Quote q = Quotes[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105) { quoteHub.Add(q); }
        }

        // late arrival
        quoteHub.Insert(Quotes[80]);

        // delete
        quoteHub.Remove(Quotes[removeAtIndex]);

        // final results
        IReadOnlyList<SmaResult> actuals
            = observer.Results;

        // time-series, for comparison (revised)
        IReadOnlyList<SmaResult> seriesList = RevisedQuotes
            .ToForceIndex(forcePeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(length - 1);
        actuals.IsExactly(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void ToStringOverride_ReturnsExpectedName()
    {
        QuoteHub quoteHub = new();
        ForceIndexHub observer = quoteHub.ToForceIndexHub(lookbackPeriods);

        observer.ToString().Should().Be($"FORCE({lookbackPeriods})");
    }

    [TestMethod]
    public void EmptyProvider()
    {
        QuoteHub quoteHub = new();
        ForceIndexHub observer = quoteHub.ToForceIndexHub(lookbackPeriods);

        IReadOnlyList<ForceIndexResult> sut = observer.Results;
        sut.Should().BeEmpty();
    }

    [TestMethod]
    public void InsufficientQuotes()
    {
        QuoteHub quoteHub = new();
        ForceIndexHub observer = quoteHub.ToForceIndexHub(lookbackPeriods);

        // Add fewer quotes than required (need at least lookbackPeriods + 1)
        for (int i = 0; i < lookbackPeriods; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        IReadOnlyList<ForceIndexResult> sut = observer.Results;
        sut.Should().HaveCount(lookbackPeriods);
        sut.All(static r => r.ForceIndex == null).Should().BeTrue();
    }

    [TestMethod]
    public void ZeroVolume()
    {
        QuoteHub quoteHub = new();
        ForceIndexHub observer = quoteHub.ToForceIndexHub(lookbackPeriods);

        // Create quotes with zero volume
        List<Quote> zeroVolumeQuotes = Quotes.Take(20).Select(static q => new Quote {
            Timestamp = q.Timestamp,
            Open = q.Open,
            High = q.High,
            Low = q.Low,
            Close = q.Close,
            Volume = 0
        }).ToList();

        foreach (Quote quote in zeroVolumeQuotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<ForceIndexResult> sut = observer.Results;

        // With zero volume, raw FI should be zero, leading to EMA converging to zero
        sut.Skip(lookbackPeriods).All(static r => r.ForceIndex == 0).Should().BeTrue();
    }

    [TestMethod]
    public void NoPriceChange()
    {
        QuoteHub quoteHub = new();
        ForceIndexHub observer = quoteHub.ToForceIndexHub(lookbackPeriods);

        // Create quotes with no price change
        List<Quote> flatQuotes = Enumerable.Range(0, 20).Select(static i => new Quote {
            Timestamp = DateTime.Now.AddDays(i),
            Open = 100,
            High = 100,
            Low = 100,
            Close = 100,
            Volume = 1000
        }).ToList();

        foreach (Quote quote in flatQuotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<ForceIndexResult> sut = observer.Results;

        // With no price change, raw FI should be zero, leading to EMA converging to zero
        sut.Skip(lookbackPeriods).All(static r => r.ForceIndex == 0).Should().BeTrue();
    }
}
