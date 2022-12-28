using System.Collections.ObjectModel;
using Internal.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

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
    [ExpectedException(typeof(ArgumentNullException), "Bad collection transform.")]
    public void ToTransformNull()
    {
        List<Quote> nullQuotes = null;
        Collection<Quote> collection = nullQuotes.ToCollection();
    }
}
