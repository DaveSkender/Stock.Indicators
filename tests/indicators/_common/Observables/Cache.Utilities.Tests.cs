namespace Observables;

[TestClass]
public class CacheUtilsTests : TestBase
{
    [TestMethod]
    public void TryFindIndex()
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

        int itemIndexEx = provider.GetIndex(q, false);
        int timeIndexEx = provider.GetIndex(q.Timestamp, false);

        // assert: same index
        itemIndexEx.Should().Be(4);
        timeIndexEx.Should().Be(4);

        // out of range (exceptions)
        Quote o = Quotes[10];

        Assert.ThrowsException<ArgumentException>(() => {
            provider.GetIndex(o, false);
        });

        Assert.ThrowsException<ArgumentException>(() => {
            provider.GetIndex(o.Timestamp, false);
        });

        // out of range (no exceptions)
        int itemIndexNo = provider.GetIndex(o, true);
        int timeIndexNo = provider.GetIndex(o.Timestamp, true);

        itemIndexNo.Should().Be(-1);
        timeIndexNo.Should().Be(-1);

        int timeInsertOut = provider.GetInsertIndex(o.Timestamp);
        int timeInsertIn = provider.GetInsertIndex(quotesList[2].Timestamp);

        timeInsertOut.Should().Be(-1);
        timeInsertIn.Should().Be(2);
    }
}
