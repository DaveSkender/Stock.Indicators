using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Pivots : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<PivotsResult> results = quotes.GetPivots(4, 4, 20, EndType.HighLow)
            .ToList();

        // assertions

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(35, results.Count(x => x.HighPoint != null));
        Assert.AreEqual(333, results.Count(x => x.HighTrend != null));
        Assert.AreEqual(338, results.Count(x => x.HighLine != null));
        Assert.AreEqual(34, results.Count(x => x.LowPoint != null));
        Assert.AreEqual(328, results.Count(x => x.LowTrend != null));
        Assert.AreEqual(333, results.Count(x => x.LowLine != null));

        // sample values
        PivotsResult r3 = results[3];
        Assert.AreEqual(null, r3.HighPoint);
        Assert.AreEqual(null, r3.HighTrend);
        Assert.AreEqual(null, r3.HighLine);
        Assert.AreEqual(null, r3.LowPoint);
        Assert.AreEqual(null, r3.LowTrend);
        Assert.AreEqual(null, r3.LowLine);

        PivotsResult r7 = results[7];
        Assert.AreEqual(null, r7.HighPoint);
        Assert.AreEqual(null, r7.HighTrend);
        Assert.AreEqual(null, r7.HighLine);
        Assert.AreEqual(212.53m, r7.LowPoint);
        Assert.AreEqual(null, r7.LowTrend);
        Assert.AreEqual(212.53m, r7.LowLine);

        PivotsResult r120 = results[120];
        Assert.AreEqual(233.02m, r120.HighPoint);
        Assert.AreEqual(PivotTrend.LH, r120.HighTrend);
        Assert.AreEqual(233.02m, r120.HighLine);
        Assert.AreEqual(null, r120.LowPoint);
        Assert.AreEqual(PivotTrend.LL, r120.LowTrend);
        Assert.AreEqual(228.9671m, NullMath.Round(r120.LowLine, 4));

        PivotsResult r180 = results[180];
        Assert.AreEqual(239.74m, r180.HighPoint);
        Assert.AreEqual(PivotTrend.HH, r180.HighTrend);
        Assert.AreEqual(239.74m, r180.HighLine);
        Assert.AreEqual(null, r180.LowPoint);
        Assert.AreEqual(PivotTrend.HL, r180.LowTrend);
        Assert.AreEqual(236.7050m, NullMath.Round(r180.LowLine, 4));

        PivotsResult r250 = results[250];
        Assert.AreEqual(null, r250.HighPoint);
        Assert.AreEqual(null, r250.HighTrend);
        Assert.AreEqual(null, r250.HighLine);
        Assert.AreEqual(256.81m, r250.LowPoint);
        Assert.AreEqual(null, r250.LowTrend);
        Assert.AreEqual(null, r250.LowLine);

        PivotsResult r472 = results[472];
        Assert.AreEqual(null, r472.HighPoint);
        Assert.AreEqual(PivotTrend.LH, r472.HighTrend);
        Assert.AreEqual(274.14m, r472.HighLine);
        Assert.AreEqual(null, r472.LowPoint);
        Assert.AreEqual(PivotTrend.HL, r472.LowTrend);
        Assert.AreEqual(255.8078m, NullMath.Round(r472.LowLine, 4));

        PivotsResult r497 = results[497];
        Assert.AreEqual(null, r497.HighPoint);
        Assert.AreEqual(null, r497.HighTrend);
        Assert.AreEqual(null, r497.HighLine);
        Assert.AreEqual(null, r497.LowPoint);
        Assert.AreEqual(null, r497.LowTrend);
        Assert.AreEqual(null, r497.LowLine);

        PivotsResult r498 = results[498];
        Assert.AreEqual(null, r498.HighPoint);
        Assert.AreEqual(null, r498.HighTrend);
        Assert.AreEqual(null, r498.HighLine);
        Assert.AreEqual(null, r498.LowPoint);
        Assert.AreEqual(null, r498.LowTrend);
        Assert.AreEqual(null, r498.LowLine);
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<PivotsResult> r = Indicator.GetPivots(badQuotes);
        Assert.AreEqual(502, r.Count());
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<PivotsResult> r0 = noquotes.GetPivots();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<PivotsResult> r1 = onequote.GetPivots();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Condense()
    {
        IEnumerable<PivotsResult> r = quotes.GetPivots(4, 4, 20, EndType.HighLow)
            .Condense();

        Assert.AreEqual(67, r.Count());
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad left span
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPivots(quotes, 1));

        // bad right span
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPivots(quotes, 2, 1));

        // bad lookback window
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetPivots(quotes, 20, 10, 20, EndType.Close));
    }
}
