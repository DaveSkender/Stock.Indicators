using System.Collections.ObjectModel;

namespace Utilities;

[TestClass]
public class Transforms : TestBase
{
    [TestMethod]
    public void ToCollection()
    {
        Collection<Quote> collection = Quotes
            .ToSortedList()
            .ToCollection();

        Assert.IsNotNull(collection);
        Assert.AreEqual(502, collection.Count);
        Assert.AreEqual(245.28m, collection[^1].Close);
    }

    // null ToCollection
    [TestMethod]
    public void ToCollectionNullExceptions()
    {
        IReadOnlyList<Quote> nullQuotes = null;

        Assert.ThrowsException<ArgumentNullException>(()
            => nullQuotes.ToCollection());
    }
}
