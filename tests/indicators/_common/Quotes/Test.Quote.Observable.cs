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

        QuoteProvider provider = new();
        provider.Add(quotesList.Take(200));

        for (int i = 200; i < quotes.Count(); i++)
        {
            Quote q = quotesList[i];
            provider.Add(q);

            Assert.AreEqual(q, provider.ProtectedQuotes[i]);
        }

        provider.EndTransmission();
    }
}
