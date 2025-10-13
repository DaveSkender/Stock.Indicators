namespace StreamHubs;

[TestClass]
public class Vwma : StreamHubTestBase, ITestQuoteObserver, ITestChainProvider
{
    private const int lookbackPeriods = 10;
    private readonly IReadOnlyList<VwmaResult> expectedOriginal = Quotes.ToVwma(lookbackPeriods);

    [TestMethod]
    public void QuoteObserver()
    {
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // prefill quotes at provider
        quoteHub.Add(Quotes.Take(20));

        // initialize observer
        VwmaHub<Quote> observer = quoteHub.ToVwmaHub(lookbackPeriods);

        // fetch initial results (early)
        IReadOnlyList<VwmaResult> actuals = observer.Results;

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
        actuals.Should().BeEquivalentTo(expectedOriginal, options => options.WithStrictOrdering());

        // delete, should equal series (revised)
        quoteHub.Remove(Quotes[removeAtIndex]);

        IReadOnlyList<VwmaResult> expectedRevised = RevisedQuotes.ToVwma(lookbackPeriods);

        actuals.Should().HaveCount(501);
        actuals.Should().BeEquivalentTo(expectedRevised, options => options.WithStrictOrdering());

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        const int vwmaPeriods = 20;
        const int smaPeriods = 10;
        int length = Quotes.Count;

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize observer
        SmaHub<VwmaResult> observer = quoteHub
            .ToVwmaHub(vwmaPeriods)
            .ToSma(smaPeriods);

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
            .ToVwma(vwmaPeriods)
            .ToSma(smaPeriods);

        // assert, should equal series
        actuals.Should().HaveCount(length - 1);
        actuals.Should().BeEquivalentTo(seriesList);

        // cleanup
        observer.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();
        VwmaHub<Quote> observer = quoteHub.ToVwmaHub(lookbackPeriods);

        observer.ToString().Should().Be($"VWMA({lookbackPeriods})");
    }

    [TestMethod]
    public void EmptyProvider()
    {
        QuoteHub<Quote> quoteHub = new();
        VwmaHub<Quote> observer = quoteHub.ToVwmaHub(lookbackPeriods);

        IReadOnlyList<VwmaResult> results = observer.Results;
        results.Should().BeEmpty();
    }

    [TestMethod]
    public void InsufficientQuotes()
    {
        QuoteHub<Quote> quoteHub = new();
        VwmaHub<Quote> observer = quoteHub.ToVwmaHub(lookbackPeriods);

        // Add fewer quotes than required
        for (int i = 0; i < lookbackPeriods - 1; i++)
        {
            quoteHub.Add(Quotes[i]);
        }

        IReadOnlyList<VwmaResult> results = observer.Results;
        results.Should().HaveCount(lookbackPeriods - 1);
        results.All(r => r.Vwma == null).Should().BeTrue();
    }

    [TestMethod]
    public void ZeroVolume()
    {
        QuoteHub<Quote> quoteHub = new();
        VwmaHub<Quote> observer = quoteHub.ToVwmaHub(lookbackPeriods);

        // Create quotes with zero volume
        List<Quote> zeroVolumeQuotes = Quotes.Take(20).Select(q => new Quote {
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

        IReadOnlyList<VwmaResult> results = observer.Results;

        // Results with sufficient data but zero volume should have null VWMA
        results.Skip(lookbackPeriods - 1).All(r => r.Vwma == null).Should().BeTrue();
    }
}
