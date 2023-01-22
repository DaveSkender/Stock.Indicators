using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class FractalTests : TestBase
{
    [TestMethod]
    public void StandardSpan2()
    {
        List<FractalResult> results = quotes
            .GetFractal(2, EndType.HighLow)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(63, results.Count(x => x.FractalBear != null));
        Assert.AreEqual(71, results.Count(x => x.FractalBull != null));

        // sample values
        FractalResult r1 = results[1];
        Assert.AreEqual(null, r1.FractalBear);
        Assert.AreEqual(null, r1.FractalBull);

        FractalResult r2 = results[3];
        Assert.AreEqual(215.17m, r2.FractalBear);
        Assert.AreEqual(null, r2.FractalBull);

        FractalResult r3 = results[133];
        Assert.AreEqual(234.53m, r3.FractalBear);
        Assert.AreEqual(null, r3.FractalBull);

        FractalResult r4 = results[180];
        Assert.AreEqual(239.74m, r4.FractalBear);
        Assert.AreEqual(238.52m, r4.FractalBull);

        FractalResult r5 = results[250];
        Assert.AreEqual(null, r5.FractalBear);
        Assert.AreEqual(256.81m, r5.FractalBull);

        FractalResult r6 = results[500];
        Assert.AreEqual(null, r6.FractalBear);
        Assert.AreEqual(null, r6.FractalBull);
    }

    [TestMethod]
    public void StandardSpan4()
    {
        List<FractalResult> results = quotes
            .GetFractal(4, 4, EndType.HighLow)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(35, results.Count(x => x.FractalBear != null));
        Assert.AreEqual(34, results.Count(x => x.FractalBull != null));

        // sample values
        FractalResult r1 = results[3];
        Assert.AreEqual(null, r1.FractalBear);
        Assert.AreEqual(null, r1.FractalBull);

        FractalResult r2 = results[7];
        Assert.AreEqual(null, r2.FractalBear);
        Assert.AreEqual(212.53m, r2.FractalBull);

        FractalResult r3 = results[120];
        Assert.AreEqual(233.02m, r3.FractalBear);
        Assert.AreEqual(null, r3.FractalBull);

        FractalResult r4 = results[180];
        Assert.AreEqual(239.74m, r4.FractalBear);
        Assert.AreEqual(null, r4.FractalBull);

        FractalResult r5 = results[250];
        Assert.AreEqual(null, r5.FractalBear);
        Assert.AreEqual(256.81m, r5.FractalBull);

        FractalResult r6 = results[500];
        Assert.AreEqual(null, r6.FractalBear);
        Assert.AreEqual(null, r6.FractalBull);
    }

    [TestMethod]
    public void BadData()
    {
        List<FractalResult> r = badQuotes
            .GetFractal()
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<FractalResult> r0 = noquotes
            .GetFractal()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<FractalResult> r1 = onequote
            .GetFractal()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        List<FractalResult> r = quotes
            .GetFractal()
            .Condense()
            .ToList();

        Assert.AreEqual(129, r.Count);
    }

    // bad window span
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetFractal(1));
}
