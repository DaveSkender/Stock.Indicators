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
        Assert.HasCount(502, collection);
        Assert.AreEqual(245.28m, collection.LastOrDefault().Close);
    }

    /// <summary>
    /// null ToCollection
    /// </summary>
    [TestMethod]
    public void ToCollectionNullExceptions()
    {
        IReadOnlyList<Quote> nullQuotes = null;

        Assert.ThrowsExactly<ArgumentNullException>(
            () => nullQuotes.ToCollection());
    }
}
