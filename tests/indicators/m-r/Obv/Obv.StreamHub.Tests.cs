namespace StreamHub;

[TestClass]
public class ObvHubTests : StreamHubTestBase
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        // Add initial quote
        provider.Add(quotesList[0]);

        ObvHub<Quote> observer = provider.ToObv();

        for (int i = 1; i < length; i++)
        {
            if (i == 80)
            {
                continue;
            }

            Quote quote = quotesList[i];
            provider.Add(quote);

            if (i is > 110 and < 115)
            {
                provider.Add(quote);
            }
        }

        provider.Insert(quotesList[80]);

        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        IReadOnlyList<ObvResult> streamList = observer.Results;
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