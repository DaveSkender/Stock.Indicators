namespace StreamHub;

[TestClass]
public class AroonHub : StreamHubTestBase, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes.ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 30; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        AroonHub<Quote> observer = provider
            .ToAroon(25);

        // fetch initial results (early)
        IReadOnlyList<AroonResult> streamList
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 30; i < length; i++)
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

        // delete
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IReadOnlyList<AroonResult> seriesList = quotesList.ToAroon(25);

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        QuoteHub<Quote> provider = new();
        AroonHub<Quote> observer = provider.ToAroon(25);

        observer.ToString().Should().Be("AROON(25)");

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainProvider()
    {
        List<Quote> quotesList = Quotes.ToList();
        int length = quotesList.Count;

        // Setup quote provider
        QuoteHub<Quote> provider = new();

        // Initialize observer - Aroon as provider feeding into EMA
        EmaHub<AroonResult> observer = provider
            .ToAroon(25)
            .ToEma(12);

        // Emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // Final results
        IReadOnlyList<EmaResult> streamList = observer.Results;

        // Time-series, for comparison
        IReadOnlyList<EmaResult> seriesList = quotesList
            .ToAroon(25)
            .ToEma(12);

        // Assert, should equal series
        streamList.Should().HaveCount(length);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
