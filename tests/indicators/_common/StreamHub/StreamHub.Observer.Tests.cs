namespace Observables;

[TestClass]
public class StreamObservers : TestBase
{
    [TestMethod]
    public void RebuildCache()
    {
        const int qtyQuotes = 5000;

        // setup: many random quotes (massive)
        IReadOnlyList<Quote> quotesList
            = Data.GetRandom(qtyQuotes).ToList();

        int length = quotesList.Count;

        length.Should().Be(qtyQuotes); // check rando

        QuoteHub quoteHub = new();

        QuotePartHub observer = quoteHub
            .ToQuotePartHub(CandlePart.Close);

        for (int i = 0; i < length; i++)
        {
            quoteHub.Add(quotesList[i]);
        }

        // original results
        IReadOnlyList<QuotePart> original = observer.Results.ToList();

        // quotes to replace
        Quote q1000original = quotesList[1000] with { /* copy */ };
        QuotePart r1000original = observer.Cache[1000] with { /* copy */ };

        // modify results (keeping quoteHub intact)
        Quote q1000modified = quotesList[1000] with { Close = 12345m };
        QuotePart r1000modified = q1000modified.ToQuotePart(CandlePart.Close);

        observer.Cache.Insert(1000, r1000modified); // add directly to cache

        IReadOnlyList<QuotePart> modified = observer.Results.ToList();

        // precondition: prefilled, modified
        quoteHub.Cache.Should().HaveCount(length);
        observer.Cache.Should().HaveCount(length + 1);

        observer.Cache[1000].Value.Should().Be(12345);
        observer.Cache.Should().NotBeEquivalentTo(original);
        observer.Cache.IsExactly(modified);

        // act: Rebuild()
        observer.Rebuild();

        // assert: restored to original
        observer.Results.Should().HaveCount(length);
        observer.Results.IsExactly(original);

        observer.Cache[1000].Value.Should().NotBe(12345);
        observer.Cache[1000].Value.Should().Be((double)quotesList[1000].Close);
    }
}
