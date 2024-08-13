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
        observer.RemoveRange(q3.Timestamp, false);

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
        observer.RemoveRange(3, true);

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

        int itemIndexEx = provider.Cache.GetIndex(q, true);
        int timeIndexEx = provider.Cache.GetIndex(q.Timestamp, true);

        // assert: same index
        itemIndexEx.Should().Be(4);
        timeIndexEx.Should().Be(4);

        // out of range (exceptions)
        Quote o = Quotes[10];

        Assert.ThrowsException<ArgumentException>(() => {
            provider.Cache.GetIndex(o, true);
        });

        Assert.ThrowsException<ArgumentException>(() => {
            provider.Cache.GetIndex(o.Timestamp, true);
        });

        // out of range (no exceptions)
        int itemIndexNo = provider.Cache.GetIndex(o, false);
        int timeIndexNo = provider.Cache.GetIndex(o.Timestamp, false);

        itemIndexNo.Should().Be(-1);
        timeIndexNo.Should().Be(-1);

        int timeInsertOut = provider.Cache.GetIndexGte(o.Timestamp);
        int timeInsertIn = provider.Cache.GetIndexGte(quotesList[2].Timestamp);

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
        if (provider.Cache.TryFindIndex(q.Timestamp, out int goodIndex))
        {
            goodIndex.Should().Be(4);
        }
        else
        {
            Assert.Fail("index not found");
        }

        // assert: out of range
        if (provider.Cache.TryFindIndex(DateTime.MaxValue, out int badIndex))
        {
            Assert.Fail("unexpected index found");
        }
        else
        {
            badIndex.Should().Be(-1);
        }
    }
}
