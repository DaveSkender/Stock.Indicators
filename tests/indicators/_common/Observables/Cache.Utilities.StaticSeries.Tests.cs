namespace Observables;

[TestClass]
public class CacheUtilities : TestBase
{
    [TestMethod]
    public void ClearCacheByTimestamp()
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
        observer.ClearCache(q3.Timestamp);

        // assert: cache is empty
        observer.Cache.Should().HaveCount(3);
        provider.Cache.Should().HaveCount(10);

        List<QuotePart> cacheOver
            = observer.Results
                .Where(c => c.Timestamp >= q3.Timestamp).ToList();

        List<QuotePart> cacheUndr
            = observer.Results
                .Where(c => c.Timestamp <= q3.Timestamp).ToList();

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

        List<QuotePart> cacheOver
            = observer.Results
                .Where(c => c.Timestamp >= q3.Timestamp).ToList();

        List<QuotePart> cacheUndr
            = observer.Results
                .Where(c => c.Timestamp <= q3.Timestamp).ToList();

        cacheOver.Should().BeEmpty();
        cacheUndr.Should().HaveCount(3);
    }

    [TestMethod]
    public void GetIndex()
    {
        // setup quote provider

        IReadOnlyList<Quote> quotesList = Quotes
            .Take(10)
            .ToList();

        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // find position of quote
        Quote q = quotesList[4];

        int itemIndexEx = provider.GetIndex(q, true);
        int timeIndexEx = provider.GetIndex(q.Timestamp, true);

        // assert: same index
        itemIndexEx.Should().Be(4);
        timeIndexEx.Should().Be(4);

        // out of range (exceptions)
        Quote o = Quotes[10];

        Assert.ThrowsException<ArgumentException>(() => {
            provider.GetIndex(o, true);
        });

        Assert.ThrowsException<ArgumentException>(() => {
            provider.GetIndex(o.Timestamp, true);
        });

        // out of range (no exceptions)
        int itemIndexNo = provider.GetIndex(o, false);
        int timeIndexNo = provider.GetIndex(o.Timestamp, false);

        itemIndexNo.Should().Be(-1);
        timeIndexNo.Should().Be(-1);

        int timeInsertOut = provider.GetIndexGte(o.Timestamp);
        int timeInsertIn = provider.GetIndexGte(quotesList[2].Timestamp);

        timeInsertOut.Should().Be(-1);
        timeInsertIn.Should().Be(2);
    }

    [TestMethod]
    public void TryFindIndex()
    {

        // setup quote provider

        List<Quote> quotesList = Quotes
            .ToSortedList()
            .Take(10)
            .ToList();

        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        Quote q = quotesList[4];

        // act: find index of quote

        // assert: correct index
        if (provider.TryFindIndex(q.Timestamp, out int goodIndex))
        {
            goodIndex.Should().Be(4);
        }
        else
        {
            Assert.Fail("index not found");
        }

        // assert: out of range
        if (provider.TryFindIndex(DateTime.MaxValue, out int badIndex))
        {
            Assert.Fail("unexpected index found");
        }
        else
        {
            badIndex.Should().Be(-1);
        }
    }
}
