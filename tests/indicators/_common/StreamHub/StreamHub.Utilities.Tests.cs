namespace Observables;

[TestClass]
public class CacheUtilities : TestBase
{
    [TestMethod]
    public void ClearCacheByTimestamp()
    {

        // setup quote provider hub

        IReadOnlyList<Quote> quotesList = Quotes
            .Take(10)
            .ToList();

        int length = quotesList.Count;

        QuoteHub quoteHub = new();

        QuotePartHub observer = quoteHub
            .ToQuotePartHub(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        Quote q3 = quotesList[3];

        // act: clear cache
        observer.RemoveRange(q3.Timestamp, notify: false);

        // assert: cache is empty
        observer.Cache.Should().HaveCount(3);
        quoteHub.Cache.Should().HaveCount(10);

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
        // setup quote provider hub

        List<Quote> quotesList = Quotes
            .ToSortedList()
            .Take(10)
            .ToList();

        int length = quotesList.Count;

        QuoteHub quoteHub = new();

        QuotePartHub observer = quoteHub
            .ToQuotePartHub(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        Quote q3 = quotesList[3];

        // act: clear cache
        observer.RemoveRange(3, notify: true);

        // assert: cache is empty
        observer.Cache.Should().HaveCount(3);
        quoteHub.Cache.Should().HaveCount(10);

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
        // setup quote provider hub

        IReadOnlyList<Quote> quotesList = Quotes
            .Take(10)
            .ToList();

        int length = quotesList.Count;

        QuoteHub quoteHub = new();

        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // find position of quote
        Quote q = quotesList[4];

        int itemIndexEx = quoteHub.Cache.IndexOf(q, true);
        int timeIndexEx = quoteHub.Cache.IndexOf(q.Timestamp, true);

        // assert: same index
        itemIndexEx.Should().Be(4);
        timeIndexEx.Should().Be(4);

        // out of range (exceptions)
        Quote o = Quotes[10];

        Assert.ThrowsExactly<ArgumentException>(
            () => quoteHub.Cache.IndexOf(o, true));

        Assert.ThrowsExactly<ArgumentException>(
            () => quoteHub.Cache.IndexOf(o.Timestamp, true));

        // out of range (no exceptions)
        int itemIndexNo = quoteHub.Cache.IndexOf(o, false);
        int timeIndexNo = quoteHub.Cache.IndexOf(o.Timestamp, false);

        itemIndexNo.Should().Be(-1);
        timeIndexNo.Should().Be(-1);

        int timeInsertOut = quoteHub.Cache.IndexGte(o.Timestamp);
        int timeInsertIn = quoteHub.Cache.IndexGte(quotesList[2].Timestamp);

        timeInsertOut.Should().Be(-1);
        timeInsertIn.Should().Be(2);
    }

    [TestMethod]
    public void TryFindIndex()
    {

        // setup quote provider hub

        List<Quote> quotesList = Quotes
            .ToSortedList()
            .Take(10)
            .ToList();

        int length = quotesList.Count;

        QuoteHub quoteHub = new();

        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        Quote q = quotesList[4];

        // act: find index of quote

        // assert: correct index
        if (quoteHub.Cache.TryFindIndex(q.Timestamp, out int goodIndex))
        {
            goodIndex.Should().Be(4);
        }
        else
        {
            Assert.Fail("index not found");
        }

        // assert: out of range
        if (quoteHub.Cache.TryFindIndex(DateTime.MaxValue, out int badIndex))
        {
            Assert.Fail("unexpected index found");
        }
        else
        {
            badIndex.Should().Be(-1);
        }
    }
}
