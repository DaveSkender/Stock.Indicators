namespace StreamHub;

[TestClass]
public class DojiHub : StreamHubTestBase
{
    [TestMethod]
    public override void QuoteObserver()
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
        DojiHub<Quote> dojiHub = quoteHub
            .ToDojiHub(0.1);

        // fetch initial results (early)
        IReadOnlyList<CandleResult> streamList
            = dojiHub.Results;

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
        IReadOnlyList<CandleResult> seriesList = quotesList.ToDoji(0.1);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        dojiHub.Unsubscribe();
        quoteHub.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> quoteHub = new();
        DojiHub<Quote> dojiHub = quoteHub.ToDojiHub(0.1);

        dojiHub.ToString().Should().Be("DOJI(0.1)");

        dojiHub.Unsubscribe();
        quoteHub.EndTransmission();
    }
}
