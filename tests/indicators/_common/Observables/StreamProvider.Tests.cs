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

        int itemIndex = provider.Position(q);
        int timeIndex = provider.Position(q.Timestamp);

        // assert: same index
        itemIndex.Should().Be(4);
        timeIndex.Should().Be(4);

        // out of range
        Quote o = Quotes.ToList()[10];

        Assert.ThrowsException<ArgumentException>(() => {
            provider.Position(o);
        });

        Assert.ThrowsException<ArgumentException>(() => {
            provider.Position(o.Timestamp);
        });
    }



    [TestMethod]
    public void RebuildCache()
    {

        List<Quote> quotesList = Quotes
            .ToSortedList()
            .Take(50)
            .ToList();

        int length = quotesList.Count;

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observer
        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        // prefill quotes to provider
        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // assert: prefilled
        provider.Cache.Should().HaveCount(50);
        observer.Cache.Should().HaveCount(50);

        // save list
        List<Reusable> original = observer.Results.ToList();

        // rebuild cache
        observer.RebuildCache();

        // assert: same dates
        observer.Results.Should().HaveCount(50);
        observer.Results.Should().BeEquivalentTo(original);
    }

    public void RebuildCacheFromTimestamp()
    {
        Assert.Fail("test not implemented");
    }

    public void RebuildCacheFromIndex()
    {
        Assert.Fail("test not implemented");
    }


    [TestMethod]
    public void Resend() => Assert.Inconclusive("test not implemented");
}
