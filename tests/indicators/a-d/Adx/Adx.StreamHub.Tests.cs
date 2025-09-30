namespace StreamHub;

[TestClass]
public class AdxHub : StreamHubTestBase
{
    [TestMethod]
    public override void QuoteObserver()
    {
        const int lookbackPeriods = 14;

        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        AdxHub<Quote> observer = provider
            .ToAdx(lookbackPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // final results
        IReadOnlyList<AdxResult> streamList
            = observer.Results;

        // time-series, for comparison
        IReadOnlyList<AdxResult> seriesList = quotesList
            .ToAdx(lookbackPeriods);

        // assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> provider = new();

        AdxHub<Quote> hub = new(provider, 14);
        hub.ToString().Should().Be("ADX(14)");

        provider.Add(Quotes[0]);
        provider.Add(Quotes[1]);

        string s = $"ADX(14)({Quotes[0].Timestamp:d})";
        hub.ToString().Should().Be(s);
    }
}
