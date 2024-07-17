namespace Observables;

[TestClass]
public class ObserverTests : TestBase
{
    [TestMethod]
    public void RebuildCache()
    {
        int qtyQuotes = 5000;

        // setup: many random quotes (massive)
        IReadOnlyList<Quote> quotesList
            = Data.GetRandom(qtyQuotes).ToList();

        int length = quotesList.Count;

        length.Should().Be(qtyQuotes); // check rando

        QuoteHub<Quote> provider = new();

        QuotePartHub<Quote> observer = provider
            .ToQuotePart(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            provider.Add(quotesList[i]);
        }

        // original results
        IReadOnlyList<QuotePart> original = observer.Results.ToList();

        // quotes to replace
        Quote q1000original = quotesList[1000] with { /* copy */ };
        QuotePart r1000original = observer.Cache[1000] with { /* copy */ };

        // modify results (keeping provider intact)
        Quote q1000modified = quotesList[1000] with { Close = 12345m };
        QuotePart r1000modified = q1000modified.ToQuotePart(CandlePart.Close);

        observer.Modify(r1000modified);  // add directly to observer

        IReadOnlyList<QuotePart> modified = observer.Results.ToList();

        // precondition: prefilled, modified
        provider.Cache.Should().HaveCount(length);
        observer.Cache.Should().HaveCount(length);

        observer.Cache[1000].Value.Should().Be(12345);
        observer.Cache.Should().NotBeEquivalentTo(original);
        observer.Cache.Should().BeEquivalentTo(modified);

        // act: Rebuild()
        observer.RebuildCache();

        // assert: restored to original
        observer.Results.Should().HaveCount(length);
        observer.Results.Should().BeEquivalentTo(original);

        observer.Cache[1000].Value.Should().NotBe(12345);
        observer.Cache[1000].Value.Should().Be((double)quotesList[1000].Close);
    }
}
