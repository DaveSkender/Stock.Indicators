namespace Tests.Common.Observables;

[TestClass]
public class ProviderTests : TestBase
{
    [TestMethod]
    public void Prefill()
    {
        List<Quote> quotesList = Quotes
            .ToSortedList()
            .Take(50)
            .ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteProvider<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // assert: prefilled
        Assert.AreEqual(50, provider.Cache.Count);
        Assert.AreEqual(50, observer.Cache.Count);

        // assert: same dates
        for (int i = 0; i < 50; i++)
        {
            IReusable r = observer.Cache[i];
            IReusable q = provider.Cache[i];

            Assert.AreEqual(q.Timestamp, r.Timestamp);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void FindIndex()
    {
        Assert.Inconclusive("test not implemented");
    }

    [TestMethod]
    public void RebuildCache()
    {
        Assert.Inconclusive("test not implemented");
    }

    [TestMethod]
    public void Resend()
    {
        Assert.Inconclusive("test not implemented");
    }
}
