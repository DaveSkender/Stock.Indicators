using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Donchian : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<DonchianResult> results = quotes.GetDonchian(20)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Centerline != null));
        Assert.AreEqual(482, results.Count(x => x.UpperBand != null));
        Assert.AreEqual(482, results.Count(x => x.LowerBand != null));
        Assert.AreEqual(482, results.Count(x => x.Width != null));

        // sample values
        DonchianResult r1 = results[19];
        Assert.AreEqual(null, r1.Centerline);
        Assert.AreEqual(null, r1.UpperBand);
        Assert.AreEqual(null, r1.LowerBand);
        Assert.AreEqual(null, r1.Width);

        DonchianResult r2 = results[20];
        Assert.AreEqual(214.2700m, NullMath.Round(r2.Centerline, 4));
        Assert.AreEqual(217.0200m, NullMath.Round(r2.UpperBand, 4));
        Assert.AreEqual(211.5200m, NullMath.Round(r2.LowerBand, 4));
        Assert.AreEqual(0.025669m, NullMath.Round(r2.Width, 6));

        DonchianResult r3 = results[249];
        Assert.AreEqual(254.2850m, NullMath.Round(r3.Centerline, 4));
        Assert.AreEqual(258.7000m, NullMath.Round(r3.UpperBand, 4));
        Assert.AreEqual(249.8700m, NullMath.Round(r3.LowerBand, 4));
        Assert.AreEqual(0.034725m, NullMath.Round(r3.Width, 6));

        DonchianResult r4 = results[485];
        Assert.AreEqual(265.5350m, NullMath.Round(r4.Centerline, 4));
        Assert.AreEqual(274.3900m, NullMath.Round(r4.UpperBand, 4));
        Assert.AreEqual(256.6800m, NullMath.Round(r4.LowerBand, 4));
        Assert.AreEqual(0.066696m, NullMath.Round(r4.Width, 6));

        DonchianResult r5 = results[501];
        Assert.AreEqual(251.5050m, NullMath.Round(r5.Centerline, 4));
        Assert.AreEqual(273.5900m, NullMath.Round(r5.UpperBand, 4));
        Assert.AreEqual(229.4200m, NullMath.Round(r5.LowerBand, 4));
        Assert.AreEqual(0.175623m, NullMath.Round(r5.Width, 6));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<DonchianResult> r = Indicator.GetDonchian(badQuotes, 15);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<DonchianResult> r0 = noquotes.GetDonchian();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<DonchianResult> r1 = onequote.GetDonchian();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Condense()
    {
        IEnumerable<DonchianResult> r = quotes.GetDonchian(20)
            .Condense();

        // assertions
        Assert.AreEqual(502 - 20, r.Count());

        DonchianResult last = r.LastOrDefault();
        Assert.AreEqual(251.5050m, NullMath.Round(last.Centerline, 4));
        Assert.AreEqual(273.5900m, NullMath.Round(last.UpperBand, 4));
        Assert.AreEqual(229.4200m, NullMath.Round(last.LowerBand, 4));
        Assert.AreEqual(0.175623m, NullMath.Round(last.Width, 6));
    }

    [TestMethod]
    public void Removed()
    {
        IEnumerable<DonchianResult> results = quotes.GetDonchian(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 20, results.Count());

        DonchianResult last = results.LastOrDefault();
        Assert.AreEqual(251.5050m, NullMath.Round(last.Centerline, 4));
        Assert.AreEqual(273.5900m, NullMath.Round(last.UpperBand, 4));
        Assert.AreEqual(229.4200m, NullMath.Round(last.LowerBand, 4));
        Assert.AreEqual(0.175623m, NullMath.Round(last.Width, 6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Indicator.GetDonchian(quotes, 0));
}
