using System.Collections.ObjectModel;

namespace Tests.Common;

[TestClass]
public class TransformTests : SeriesTestBase
{
    [TestMethod]
    public void ToCollection()
    {
        Collection<Quote> collection = Quotes
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
