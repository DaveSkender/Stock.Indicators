namespace StreamHub;

[TestClass]
public class ObvHubTests : StreamHubTestBase
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        StreamHub<Quote, ObvResult> observer = provider.ToObv();

        // fetch initial results (early)
        IReadOnlyList<ObvResult> streamList = observer.Results;

        // emulate adding quotes to provider
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                provider.Add(q);
            }
        }

        // late arrival
        provider.Insert(quotesList[80]);

        // removal
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        streamList = observer.Results;
        IReadOnlyList<ObvResult> seriesList = quotesList.ToObv();

        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        ObvHub<Quote> hub = new(new QuoteHub<Quote>());
        hub.ToString().Should().Be("OBV");
    }
}