namespace Tests.Indicators.Stream;

[TestClass]
public class AlligatorTests : StreamTestBase, ITestChainObserver
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 20; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        Alligator<Quote> observer = provider
            .ToAlligator();

        // fetch initial results (early)
        IReadOnlyList<AlligatorResult> streamList
            = observer.Results;

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
        provider.Add(quotesList[80]);

        // delete
        provider.Delete(quotesList[400]);
        quotesList.RemoveAt(400);

        // time-series, for comparison
        List<AlligatorResult> seriesList
           = quotesList
            .GetAlligator()
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length - 1; i++)
        {
            Quote q = quotesList[i];
            AlligatorResult s = seriesList[i];
            AlligatorResult r = streamList[i];

            r.Timestamp.Should().Be(q.Timestamp);
            r.Timestamp.Should().Be(s.Timestamp);
            r.Jaw.Should().Be(s.Jaw);
            r.Lips.Should().Be(s.Lips);
            r.Teeth.Should().Be(s.Teeth);
            r.Should().Be(s);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ChainObserver()
    {
        List<Quote> quotesList = Quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        Alligator<SmaResult> observer = provider
            .ToSma(10)
            .ToAlligator();

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
        IReadOnlyList<AlligatorResult> streamList
            = observer.Results;

        // time-series, for comparison
        List<AlligatorResult> seriesList
           = quotesList
            .GetSma(10)
            .GetAlligator()
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length - 1; i++)
        {
            Quote q = quotesList[i];
            AlligatorResult s = seriesList[i];
            AlligatorResult r = streamList[i];

            r.Timestamp.Should().Be(q.Timestamp);
            r.Timestamp.Should().Be(s.Timestamp);
            r.Jaw.Should().Be(s.Jaw);
            r.Lips.Should().Be(s.Lips);
            r.Teeth.Should().Be(s.Teeth);
            r.Should().Be(s);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }
}
