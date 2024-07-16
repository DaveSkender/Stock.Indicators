namespace Stream;

[TestClass]
public class QuoteTests : StreamTestBase, ITestChainProvider
{
    [TestMethod]
    public override void QuoteObserver()
    {
        // tests quote redistribution

        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = Quotes.Count;

        // add base quotes (batch)
        QuoteHub<Quote> provider = new();

        provider.Add(quotesList.Take(200));

        // add incremental quotes
        for (int i = 200; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        QuoteHub<Quote> observer
            = provider.ToQuote();

        // assert same as original
        for (int i = 0; i < length; i++)
        {
            Quote o = quotesList[i];
            Quote q = provider.Cache[i];
            Quote r = observer.Cache[i];

            q.Should().Be(o);
            r.Should().Be(q);
        }

        // close observations
        provider.EndTransmission();

        // confirm public interfaces
        provider.Cache.Should().HaveCount(length);
        observer.Cache.Should().HaveCount(length);
        observer.Cache.Should().BeEquivalentTo(provider.Cache);
    }

    [TestMethod]
    public void ChainProvider()
    {
        int smaPeriods = 8;

        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        SmaHub<Quote> observer
           = provider
            .ToSma(smaPeriods);

        // emulate quote stream
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // delete
        provider.RemoveAt(400);
        quotesList.RemoveAt(400);

        // final results
        IReadOnlyList<SmaResult> streamList
            = observer.Results;

        // time-series, for comparison
        List<SmaResult> seriesList
           = quotesList
            .GetSma(smaPeriods)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length - 1; i++)
        {
            Quote q = quotesList[i];
            SmaResult s = seriesList[i];
            SmaResult r = streamList[i];

            r.Timestamp.Should().Be(q.Timestamp);
            r.Timestamp.Should().Be(s.Timestamp);
            r.Sma.Should().Be(s.Sma);
            r.Should().Be(s);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
