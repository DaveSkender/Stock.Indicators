using System.Collections.ObjectModel;

namespace Tests.Common;

[TestClass]
public class Transforms : TestBase
{
    [TestMethod]
    public void ToCollection()
    {
        Collection<Quote> collection = quotes
            .ToSortedList()
            .ToCollection();

        Assert.IsNotNull(collection);
        Assert.HasCount(502, collection);
        Assert.AreEqual(245.28m, collection.LastOrDefault().Close);
    }

    // null ToCollection
    [TestMethod]
    public void Exceptions()
    {
        List<Quote> nullQuotes = null;

        Assert.ThrowsExactly<ArgumentNullException>(
            () => nullQuotes.ToCollection());
    }
}
