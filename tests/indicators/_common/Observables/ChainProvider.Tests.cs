namespace Tests.Indicators;

[TestClass]
public class ChainProviderTests : StreamTestBase
{
    [TestMethod]
    public override void QuoteObserver()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < 50; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // fetch initial results
        IEnumerable<Reusable> results = observer.Results;

        // emulate adding quotes to provider
        for (int i = 50; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        // final results
        List<Reusable> staticList
            = results.ToList();

        // time-series, for comparison
        List<Reusable> streamList = quotes
            .ToReusableList(CandlePart.Close);

        // assert, should equal series
        for (int i = 0; i < streamList.Count; i++)
        {
            Reusable s = streamList[i];
            Reusable r = staticList[i];

            Assert.AreEqual(s.Timestamp, r.Timestamp);
            Assert.AreEqual(s.Value, r.Value);
        }

        // confirm public interface
        Assert.AreEqual(observer.Cache.Count, observer.Results.Count);

        // confirm same length as provider cache
        Assert.AreEqual(observer.Cache.Count, provider.Results.Count);

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void Chainor()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // initialize observer
        Ema<Reusable> ema = provider
            .Use(CandlePart.HL2)
            .ToEma(11);

        // emulate adding quotes to provider
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        provider.EndTransmission();

        // stream results
        List<EmaResult> streamEma = ema
            .Results
            .ToList();

        // time-series, for comparison
        List<EmaResult> staticEma = quotes
            .Use<Quote>(CandlePart.HL2)
            .GetEma(11)
            .ToList();

        // assert, should equal series
        for (int i = 0; i < length; i++)
        {
            EmaResult s = staticEma[i];
            EmaResult r = streamEma[i];
            Reusable e = ema.Provider.Results[i];

            // compare series
            Assert.AreEqual(s.Timestamp, r.Timestamp);
            Assert.AreEqual(s.Ema, r.Ema);
        }
    }

    [TestMethod]
    public void LateArrival()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotes.Count();

        // add base quotes
        QuoteProvider<Quote> provider = new();

        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // emulate incremental quotes
        for (int i = 0; i < length; i++)
        {
            // skip one
            if (i == 100)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);
        }

        // add late
        provider.Add(quotesList[100]);

        // assert same as original
        for (int i = 0; i < length; i++)
        {
            Quote q = quotesList[i];
            Reusable r = observer.Cache[i];

            // compare quote to result cache
            Assert.AreEqual(q.Timestamp, r.Timestamp);
            Assert.AreEqual((double)q.Close, r.Value);
        }

        // close observations
        provider.EndTransmission();
    }

    [TestMethod]
    public void Overflow()
    {
        // initialize
        QuoteProvider<Quote> provider = new();

        Use<Quote> chainProvider = provider
            .Use(CandlePart.Close);

        // add too many duplicates
        Assert.ThrowsException<OverflowException>(() => {
            Quote q = new() { Timestamp = DateTime.Now };

            for (int i = 0; i <= 101; i++)
            {
                provider.Add(q);
            }
        });

        Assert.AreEqual(1, provider.Results.Count);
        Assert.AreEqual(1, chainProvider.Results.Count);
    }
}
