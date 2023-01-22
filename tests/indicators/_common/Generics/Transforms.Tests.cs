using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Common;

[TestClass]
public class TransformTests : TestBase
{
    [TestMethod]
    public void ToCollection()
    {
        Collection<Quote> collection = quotes
            .ToSortedList()
            .ToCollection();

        Assert.IsNotNull(collection);
        Assert.AreEqual(502, collection.Count);
        Assert.AreEqual(collection.LastOrDefault().Close, 245.28m);
    }

    // null ToCollection
    [TestMethod]
    public void Exceptions()
    {
        List<Quote> nullQuotes = null;

        Assert.ThrowsException<ArgumentNullException>(()
            => nullQuotes.ToCollection());
    }
}
