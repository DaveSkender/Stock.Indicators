namespace Observables;

[TestClass]
public class ProviderTests : TestBase
{
    [TestMethod]
    public void Prefill()
    {
        IReadOnlyList<Quote> quotesList = Quotes
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
        QuotePartHub<Quote> observer = provider
            .ToQuotePart(CandlePart.Close);

        // assert: prefilled
        provider.Cache.Should().HaveCount(50);
        observer.Cache.Should().HaveCount(50);

        // assert: same dates
        for (int i = 0; i < 50; i++)
        {
            IReusable r = observer.Cache[i];
            IReusable q = provider.Cache[i];

            r.Timestamp.Should().Be(q.Timestamp);
        }

        observer.Unsubscribe();
        provider.EndTransmission();
    }

    [TestMethod]
    public void ClearCache()
    {
        // setup quote provider

        IReadOnlyList<Quote> quotesList = Quotes
            .Take(10)
            .ToList();

        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        QuotePartHub<Quote> observer = provider
            .ToQuotePart(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // act: clear cache
        observer.ClearCache();

        // assert: cache is empty
        observer.Cache.Should().BeEmpty();
        provider.Cache.Should().HaveCount(10);
    }

    [TestMethod]
    public void ClearCacheByTimestamp()
    {

        // setup quote provider

        IReadOnlyList<Quote> quotesList = Quotes
            .Take(10)
            .ToSortedList();

        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        QuotePartHub<Quote> observer = provider
            .ToQuotePart(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        Quote q3 = quotesList[3];

        // act: clear cache
        observer.ClearCache(q3.Timestamp);

        // assert: cache is empty
        observer.Cache.Should().HaveCount(3);
        provider.Cache.Should().HaveCount(10);

        IReadOnlyList<QuotePart> cacheOver
            = observer.Results
                .Where(c => c.Timestamp >= q3.Timestamp).ToList();

        IReadOnlyList<QuotePart> cacheUndr
            = observer.Results
                .Where(c => c.Timestamp <= q3.Timestamp).ToList();

        cacheOver.Should().BeEmpty();
        cacheUndr.Should().HaveCount(3);
    }

    [TestMethod]
    public void ClearCacheByIndex()
    {

        // setup quote provider

        IReadOnlyList<Quote> quotesList = Quotes
            .Take(10)
            .ToList();

        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        QuotePartHub<Quote> observer = provider
            .ToQuotePart(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        Quote q3 = quotesList[3];

        // act: clear cache
        observer.ClearCache(3);

        // assert: cache is empty
        observer.Cache.Should().HaveCount(3);
        provider.Cache.Should().HaveCount(10);

        IReadOnlyList<QuotePart> cacheOver
            = observer.Results
                .Where(c => c.Timestamp >= q3.Timestamp).ToList();

        IReadOnlyList<QuotePart> cacheUndr
            = observer.Results
                .Where(c => c.Timestamp <= q3.Timestamp).ToList();

        cacheOver.Should().BeEmpty();
        cacheUndr.Should().HaveCount(3);
    }
}
