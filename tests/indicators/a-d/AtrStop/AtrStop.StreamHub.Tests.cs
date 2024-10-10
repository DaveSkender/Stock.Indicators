namespace StreamHub;

[TestClass]
public class AtrStop : StreamHubTestBase
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider (batch)
        provider.Add(quotesList.Take(20));

        // initialize observer
        AtrStopHub<Quote> observer = provider
            .ToAtrStop();

        observer.Results.Should().HaveCount(20);

        // fetch initial results (early)
        IReadOnlyList<AtrStopResult> streamList
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 20; i < length; i++)
        {
            // skip one (add later)
            if (i is 30 or 80)
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

        // late arrivals
        provider.Insert(quotesList[30]);  // rebuilds complete series
        provider.Insert(quotesList[80]);  // rebuilds from last reversal

        // delete
        provider.Remove(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        IEnumerable<AtrStopResult> seriesList
           = quotesList
            .GetAtrStop();

        // assert, should equal series
        streamList.Should().HaveCount(length - 1);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void QuoteObserverHighLow()
    {
        // simple test, just to check High/Low variant

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        AtrStopHub<Quote> observer = provider
            .ToAtrStop(endType: EndType.HighLow);

        // add quotes to provider
        provider.Add(Quotes);

        // stream results
        IReadOnlyList<AtrStopResult> streamList
            = observer.Results;

        // time-series, for comparison
        IEnumerable<AtrStopResult> seriesList
           = Quotes
            .GetAtrStop(endType: EndType.HighLow);

        // assert, should equal series
        streamList.Should().HaveCount(Quotes.Count);
        streamList.Should().BeEquivalentTo(seriesList);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public override void CustomToString()
    {
        AtrStopHub<Quote> hub = new(new QuoteHub<Quote>(), 14, 3, EndType.Close);
        hub.ToString().Should().Be("ATR-STOP(14,3,CLOSE)");
    }
}
