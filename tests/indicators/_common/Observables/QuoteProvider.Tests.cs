namespace Tests.Common.Observables;

[TestClass]
public class QuoteProviderTests : TestBase
{
    [TestMethod]
    public void AddQuote()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotes.Count();

        // add base quotes (batch)
        QuoteProvider<Quote> provider = new();
        provider.Add(quotesList.Take(200));

        // emulate incremental quotes
        for (int i = 200; i < length; i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);
        }

        // assert same as original
        for (int i = 0; i < length; i++)
        {
            Quote o = quotesList[i];
            Quote q = provider.Cache[i];

            Assert.AreEqual(o, q);  // same ref
        }

        // confirm public interfaces
        Assert.AreEqual(provider.Cache.Count, provider.Results.Count);

        // close observations
        provider.EndTransmission();
    }
}
