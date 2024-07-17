using System.Collections.ObjectModel;

namespace Utilities;

[TestClass]
public class TransformTests : TestBase
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
        IReadOnlyList<Quote> nullQuotes = null;

        Assert.ThrowsException<ArgumentNullException>(()
            => nullQuotes.ToCollection());
    }
}
