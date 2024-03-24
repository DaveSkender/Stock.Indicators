namespace Tests.Indicators;

[TestClass]
public class AlligatorStreamTests : StreamTestBase, ITestChainObserver
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // initialize observer
        Alligator observer = provider
            .AttachAlligator(13, 8, 8, 5, 5, 3);

        // fetch initial results (early)
        IEnumerable<AlligatorResult> results
            = observer.Results;

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
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
        provider.Add(quotesList[80]);

        // delete
        provider.Delete(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        List<AlligatorResult> streamList
            = results.ToList();

        // time-series, for comparison
        List<AlligatorResult> seriesList = quotesList
            .GetAlligator(13, 8, 8, 5, 5, 3)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            AlligatorResult s = seriesList[i];
            AlligatorResult r = streamList[i];

            r.Timestamp.Should().Be(s.Timestamp);
            r.Jaw.Should().Be(s.Jaw);
            r.Lips.Should().Be(s.Lips);
            r.Teeth.Should().Be(s.Teeth);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void Chainee()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // initialize observer
        Alligator observer = provider
            .AttachSma(10)
            .AttachAlligator(13, 8, 8, 5, 5, 3);

        // emulate adding quotes out of order
        // note: this works when graceful order
        for (int i = 0; i < length; i++)
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
        provider.Add(quotesList[80]);

        // delete
        provider.Delete(quotesList[400]);
        quotesList.RemoveAt(400);

        // final results
        List<AlligatorResult> streamList
            = [.. observer.Results];

        // time-series, for comparison
        List<AlligatorResult> seriesList = quotesList
            .GetSma(10)
            .GetAlligator(13, 8, 8, 5, 5, 3)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < seriesList.Count; i++)
        {
            AlligatorResult s = seriesList[i];
            AlligatorResult r = streamList[i];

            r.Timestamp.Should().Be(s.Timestamp);
            r.Jaw.Should().Be(s.Jaw);
            r.Lips.Should().Be(s.Lips);
            r.Teeth.Should().Be(s.Teeth);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
