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
        QuoteHub<Quote> provider = new();

        // prefill quotes to provider
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // initialize observer
        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // assert: prefilled
        provider.StreamCache.Cache.Should().HaveCount(50);
        observer.StreamCache.Cache.Should().HaveCount(50);

        // assert: same dates
        for (int i = 0; i < 50; i++)
        {
            IReusable r = observer.StreamCache.Cache[i];
            IReusable q = provider.StreamCache.Cache[i];

            r.Timestamp.Should().Be(q.Timestamp);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ClearCache()
    {
        // setup quote provider

        List<Quote> quotesList = Quotes
            .ToSortedList()
            .Take(10)
            .ToList();

        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // act: clear cache
        observer.ClearCache();

        // assert: cache is empty
        observer.StreamCache.Cache.Should().BeEmpty();
        provider.StreamCache.Cache.Should().HaveCount(10);
    }

    [TestMethod]
    public void ClearCacheByTimestamp()
    {

        // setup quote provider

        List<Quote> quotesList = Quotes
            .ToSortedList()
            .Take(10)
            .ToList();

        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        Quote q3 = quotesList[3];

        // act: clear cache
        observer.ClearCache(q3.Timestamp);

        // assert: cache is empty
        observer.StreamCache.Cache.Should().HaveCount(3);
        provider.StreamCache.Cache.Should().HaveCount(10);

        List<Reusable> cacheOver = observer.Results.Where(c => c.Timestamp >= q3.Timestamp).ToList();
        List<Reusable> cacheUndr = observer.Results.Where(c => c.Timestamp <= q3.Timestamp).ToList();

        cacheOver.Should().BeEmpty();
        cacheUndr.Should().HaveCount(3);
    }

    [TestMethod]
    public void ClearCacheByIndex()
    {

        // setup quote provider

        List<Quote> quotesList = Quotes
            .ToSortedList()
            .Take(10)
            .ToList();

        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        Quote q3 = quotesList[3];

        // act: clear cache
        observer.ClearCache(3);

        // assert: cache is empty
        observer.StreamCache.Cache.Should().HaveCount(3);
        provider.StreamCache.Cache.Should().HaveCount(10);

        List<Reusable> cacheOver = observer.Results.Where(c => c.Timestamp >= q3.Timestamp).ToList();
        List<Reusable> cacheUndr = observer.Results.Where(c => c.Timestamp <= q3.Timestamp).ToList();

        cacheOver.Should().BeEmpty();
        cacheUndr.Should().HaveCount(3);
    }
}
