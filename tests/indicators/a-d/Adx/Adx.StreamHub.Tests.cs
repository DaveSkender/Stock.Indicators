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

        // setup quote provider hub
        QuoteHub<Quote> quoteHub = new();

        // initialize observer
        AdxHub<Quote> observer = quoteHub
            .ToAdxHub(lookbackPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
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
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();

        AdxHub<Quote> hub = new(quoteHub, 14);
        hub.ToString().Should().Be("ADX(14)");

        quoteHub.Add(Quotes[0]);
        quoteHub.Add(Quotes[1]);

        string s = $"ADX(14)({Quotes[0].Timestamp:d})";
        hub.ToString().Should().Be(s);
    }
}
