using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Common;

[TestClass]
public class Sorting : TestBase
{
    [TestMethod]
    public void ToSortedCollection()
    {
        // baseline for comparison
        List<SmaResult> baseline =
        [
            new SmaResult(DateTime.Parse("1/1/2000", EnglishCulture)) { Sma = null },
            new SmaResult(DateTime.Parse("1/2/2000", EnglishCulture)) { Sma = null },
            new SmaResult(DateTime.Parse("1/9/2000", EnglishCulture)) { Sma = null },
            new SmaResult(DateTime.Parse("1/3/2000", EnglishCulture)) { Sma = 3 },
            new SmaResult(DateTime.Parse("1/4/2000", EnglishCulture)) { Sma = 4 },
            new SmaResult(DateTime.Parse("1/5/2000", EnglishCulture)) { Sma = 5 },
            new SmaResult(DateTime.Parse("1/6/2000", EnglishCulture)) { Sma = 6 },
            new SmaResult(DateTime.Parse("1/7/2000", EnglishCulture)) { Sma = 7 },
            new SmaResult(DateTime.Parse("1/8/2000", EnglishCulture)) { Sma = double.NaN },
        ];

        // PUBLIC VARIANT, generic sorted Collection
        Collection<SmaResult> sortResults = baseline
            .ToSortedCollection();

        Assert.AreEqual(5, sortResults[4].Sma);
        Assert.AreEqual(DateTime.Parse("1/9/2000", EnglishCulture), sortResults.LastOrDefault().Date);
    }
}
