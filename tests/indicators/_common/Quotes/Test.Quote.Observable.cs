using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class QuoteSourceTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<Quote> quotesList = quotes
            .ToSortedList();

        int length = quotes.Count();

        // add base quotes
        QuoteProvider provider = new();
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
            Quote q = provider.ProtectedQuotes[i];

            Assert.AreEqual(o, q);
        }

        provider.EndTransmission();
    }
}
