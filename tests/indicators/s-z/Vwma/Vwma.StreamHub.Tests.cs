namespace StreamHubs;

[TestClass]
public class Vwma : StreamHubTestBase
{
    private const int lookbackPeriods = 10;

    [TestMethod]
    public override void QuoteObserver()
    {
        QuoteHub<Quote> provider = new();
        VwmaHub<Quote> observer = provider.ToVwma(lookbackPeriods);

        foreach (Quote quote in Quotes)
        {
            provider.Add(quote);
        }

        IReadOnlyList<VwmaResult> results = observer.Results;
        IReadOnlyList<VwmaResult> expected = Quotes.ToVwma(lookbackPeriods);

        results.Should().HaveCount(expected.Count);
        results.Should().BeEquivalentTo(expected);
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> provider = new();
        VwmaHub<Quote> observer = provider.ToVwma(lookbackPeriods);

        observer.ToString().Should().Be($"VWMA({lookbackPeriods})");
    }
}
