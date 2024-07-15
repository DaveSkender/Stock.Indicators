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

        QuotePartHub<Quote> observer = provider
            .ToQuotePart(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // original results
        List<QuotePart> original = observer.Results.ToList();

        // quotes to replace
        Quote q1000original = quotesList[1000] with { /* copy */ };
        QuotePart r1000original = observer.Cache[1000] with { /* copy */ };

        // modify results (keeping provider intact)
        Quote q1000modified = quotesList[1000] with { Close = 12345m };
        QuotePart r1000modified = q1000modified.ToQuotePart(CandlePart.Close);

        observer.Modify(r1000modified);  // add directly to observer

        List<QuotePart> modified = observer.Results.ToList();

        // precondition: prefilled, modified
        provider.Cache.Should().HaveCount(15821);
        observer.Cache.Should().HaveCount(15821);

        observer.Cache[1000].Value.Should().Be(12345);
        observer.Cache.Should().NotBeEquivalentTo(original);
        observer.Cache.Should().BeEquivalentTo(modified);

        // act: Rebuild()
        observer.RebuildCache();

        // assert: restored to original
        observer.Results.Should().HaveCount(15821);
        observer.Results.Should().BeEquivalentTo(original);

        observer.Cache[1000].Value.Should().NotBe(12345);
        observer.Cache[1000].Value.Should().Be((double)quotesList[1000].Close);
    }
}
