namespace StreamHub;

[TestClass]
public class MarubozuHub : StreamHubTestBase, ITestQuoteObserver
{
    [TestMethod]
    public void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> quoteHub = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // initialize observer
        MarubozuHub<Quote> marubozuHub = quoteHub
            .ToMarubozuHub(95);

        // fetch initial results (early)
        IReadOnlyList<CandleResult> streamList
            = marubozuHub.Results;

        // emulate adding quotes to provider
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            quoteHub.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                quoteHub.Add(q);
            }
        }

        // late arrival
        quoteHub.Insert(quotesList[80]);

        // delete
        quoteHub.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<CandleResult> seriesList = quotesList.ToMarubozu(95);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        marubozuHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();
        MarubozuHub<Quote> marubozuHub = quoteHub.ToMarubozuHub(95);

        marubozuHub.ToString().Should().Be("MARUBOZU(95.0)");

        marubozuHub.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
