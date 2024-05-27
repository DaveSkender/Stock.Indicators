namespace Tests.Common;

[TestClass]
public class QuoteProviderTests : StreamTestBase
{
    [TestMethod]
    public override void QuoteObserver()
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
        Assert.AreEqual(provider.Cache.Count, provider.Quotes.Count());

        // close observations
        provider.EndTransmission();
    }

    [TestMethod]
    public void LateArrival()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotesList.Count;

        // add base quotes
        QuoteProvider<Quote> provider = new();

        // emulate incremental quotes
        for (int i = 0; i < length; i++)
        {
            // skip one
            if (i == 100)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);
        }

        // late add
        provider.Add(quotesList[100]);

        // assert same as original
        for (int i = 0; i < length; i++)
        {
            Quote o = quotesList[i];
            Quote q = provider.Cache[i];

            bool areEqual = Equals(o, q);

            Assert.IsTrue(areEqual);
        }

        // close observations
        provider.EndTransmission();
    }

    [TestMethod]
    public void Overflow()
    {
        // setup quote provider
        QuoteProvider<Quote> provider = new();
        DateTime date = DateTime.Now;

        Assert.ThrowsException<OverflowException>(() => {
            // add too many duplicates
            for (int i = 0; i <= 101; i++)
            {
                // use newly defined quote each time
                provider.Add(new Quote() {
                    Timestamp = date,
                    Open = 2,
                    High = 4,
                    Low = 1,
                    Close = 3,
                    Volume = 500
                });
            }
        });

        Assert.AreEqual(1, provider.Quotes.Count());

        provider.EndTransmission();
    }

    [TestMethod]
    public void Exceptions()
    {
        // null quote added
        QuoteProvider<Quote> provider = new();

        // overflow, with undefined values
        Assert.ThrowsException<OverflowException>(() => {
            DateTime date = DateTime.Now;

            for (int i = 0; i <= 101; i++)
            {
                provider.Add(new Quote() { Timestamp = date });
            }
        });

        // null quote
        Assert.ThrowsException<ArgumentNullException>(() => {
            Quote quote = null;
            provider.Add(quote);
        });

        // close observations
        provider.EndTransmission();
    }
}
