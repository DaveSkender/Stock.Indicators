using System.Globalization;
using Sut;

namespace Customization;

// CUSTOM RESULTS

[TestClass]
[TestCategory("Integration")]
public class CustomResults
{
    private static readonly CultureInfo EnglishCulture
    = new("en-US", false);

    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();

    [TestMethod]
    public void CustomSeriesClass()
    {
        // can use a derive Indicator class
        CustomSeries myIndicator = new() {
            Timestamp = DateTime.Now,
            Ema = 123.456,
            MyProperty = false
        };

        Assert.AreEqual(false, myIndicator.MyProperty);
    }

    [TestMethod]
    public void CustomSeriesClassLinq()
    {
        IReadOnlyList<EmaResult> emaResults = quotes.ToEma(14);

        // can use a derive Indicator class using Linq

        IEnumerable<CustomSeries> myIndicatorResults = emaResults
            .Where(x => x.Ema != null)
            .Select(x => new CustomSeries {
                Timestamp = x.Timestamp,
                Ema = x.Ema,
                MyProperty = false
            });

        Assert.IsTrue(myIndicatorResults.Any());
    }

    [TestMethod]
    public void CustomSeriesClassFind()
    {
        List<EmaResult> emaResults
            = quotes.ToEma(20).ToList();

        // can use a derive Indicator class using Linq

        List<CustomSeries> myIndicatorResults = emaResults
            .Where(x => x.Ema != null)
            .Select(x => new CustomSeries {
                Id = 12345,
                Timestamp = x.Timestamp,
                Ema = x.Ema,
                MyProperty = false
            })
            .ToList();

        Assert.IsTrue(myIndicatorResults.Count > 0);

        // find specific date
        DateTime findDate = DateTime.ParseExact(
            "2018-12-31", "yyyy-MM-dd", EnglishCulture);

        CustomSeries i = myIndicatorResults.Find(x => x.Timestamp == findDate);
        Assert.AreEqual(12345, i.Id);

        EmaResult r = emaResults.Find(x => x.Timestamp == findDate);
        Assert.AreEqual(249.3519m, Math.Round((decimal)r.Ema, 4));
    }

    [TestMethod]
    public void CustomReusable() => Assert.Inconclusive("Test not implemented");

    [TestMethod]
    public void CustomReusableInherited() => Assert.Inconclusive("Test not implemented");

    [TestMethod]
    public void CustomInheritedEma() => Assert.Inconclusive("Test not implemented");
}
