using System.Collections.ObjectModel;

namespace Tests.Common;

[TestClass]
public class Sorting : SeriesTestBase
{
    [TestMethod]
    public void ToSortedCollection()
    {
        // baseline for comparison
        List<SmaResult> baseline =
        [
            new(Timestamp: DateTime.Parse("1/1/2000", englishCulture), Sma: null),
            new(Timestamp: DateTime.Parse("1/2/2000", englishCulture), Sma: null),
            new(Timestamp: DateTime.Parse("1/9/2000", englishCulture), Sma: null),
            new(Timestamp: DateTime.Parse("1/3/2000", englishCulture), Sma: 3),
            new(Timestamp: DateTime.Parse("1/4/2000", englishCulture), Sma: 4),
            new(Timestamp: DateTime.Parse("1/5/2000", englishCulture), Sma: 5),
            new(Timestamp: DateTime.Parse("1/6/2000", englishCulture), Sma: 6),
            new(Timestamp: DateTime.Parse("1/7/2000", englishCulture), Sma: 7),
            new(Timestamp: DateTime.Parse("1/8/2000", englishCulture), Sma: double.NaN)
        ];

        // PUBLIC VARIANT, generic sorted Collection
        Collection<SmaResult> sortResults = baseline
            .ToSortedCollection();

        Assert.AreEqual(5, sortResults[4].Sma);
        Assert.AreEqual(DateTime.Parse("1/9/2000", englishCulture), sortResults.LastOrDefault().Timestamp);
    }
}
