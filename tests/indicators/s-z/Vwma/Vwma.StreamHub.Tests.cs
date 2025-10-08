namespace StreamHubs;

[TestClass]
public class Vwma : StreamHubTestBase
{
    private const int lookbackPeriods = 10;

    [TestMethod]
    public override void QuoteObserver()
    {
        QuoteHub<Quote> quoteHub = new();
        VwmaHub<Quote> observer = quoteHub.ToVwmaHub(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            quoteHub.Add(quote);
        }

        IReadOnlyList<VwmaResult> results = observer.Results;
        IReadOnlyList<VwmaResult> expected = Quotes.ToVwma(lookbackPeriods);

        results.Should().HaveCount(expected.Count);
        results.Should().BeEquivalentTo(expected);
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
