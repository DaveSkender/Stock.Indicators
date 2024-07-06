namespace Tests.Common.Observables;

[TestClass]
public class ObserverTests : TestBase
{
    [TestMethod]
    public void RebuildCache()
    {
        // setup: pre-populated observer
        List<Quote> quotesList = LongestQuotes
            .ToSortedList()
            .ToList();

        int length = quotesList.Count;

        QuoteHub<Quote> provider = new();

        Use<Quote> observer = provider
            .Use(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // original results
        List<Reusable> original = observer.Results.ToList();

        // quotes to replace
        Quote q1000original = quotesList[1000] with { /* copy */ };
        Reusable r1000original = observer.StreamCache.Cache[1000] with { /* copy */ };

        // modify results (keeping provider intact)
        Quote q1000modified = quotesList[1000] with { Close = 12345m };
        Reusable r1000modified = q1000modified.ToReusable(CandlePart.Close);

        observer.StreamCache.Modify(r1000modified);  // add directly to observer

        List<Reusable> modified = observer.Results.ToList();

        // precondition: prefilled, modified
        provider.StreamCache.Cache.Should().HaveCount(15821);
        observer.StreamCache.Cache.Should().HaveCount(15821);

        observer.StreamCache.ReadCache[1000].Value.Should().Be(12345);
        observer.StreamCache.Cache.Should().NotBeEquivalentTo(original);
        observer.StreamCache.Cache.Should().BeEquivalentTo(modified);

        // act: Rebuild()
        observer.RebuildCache();

        // assert: restored to original
        observer.Results.Should().HaveCount(15821);
        observer.Results.Should().BeEquivalentTo(original);

        observer.StreamCache.ReadCache[1000].Value.Should().NotBe(12345);
        observer.StreamCache.ReadCache[1000].Value.Should().Be((double)quotesList[1000].Close);
    }
}
