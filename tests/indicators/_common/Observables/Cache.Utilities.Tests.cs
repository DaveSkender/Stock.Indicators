namespace Tests.Common.Observables;

[TestClass]
public class CacheUtilsTests : TestBase
{
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
        if (provider.StreamCache.TryFindIndex(q.Timestamp, out int goodIndex))
        {
            goodIndex.Should().Be(4);
        }
        else
        {
            Assert.Fail("index not found");
        }

        // assert: out of range
        if (provider.StreamCache.TryFindIndex(DateTime.MaxValue, out int badIndex))
        {
            Assert.Fail("unexpected index found");
        }
        else
        {
            badIndex.Should().Be(-1);
        }
    }

    [TestMethod]
    public void FindPosition()
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

        // find position of quote
        Quote q = quotesList[4];

        int itemIndex = provider.StreamCache.Position(q);
        int timeIndex = provider.StreamCache.Position(q.Timestamp);

        // assert: same index
        itemIndex.Should().Be(4);
        timeIndex.Should().Be(4);

        // out of range
        Quote o = Quotes.ToList()[10];

        Assert.ThrowsException<ArgumentException>(() => {
            provider.StreamCache.Position(o);
        });

        Assert.ThrowsException<ArgumentException>(() => {
            provider.StreamCache.Position(o.Timestamp);
        });
    }
}
