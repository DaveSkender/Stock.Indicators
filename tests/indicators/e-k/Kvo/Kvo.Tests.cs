using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Klinger : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<KvoResult> results =
            quotes.GetKvo(34, 55, 13)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(446, results.Count(x => x.Oscillator != null));
        Assert.AreEqual(434, results.Count(x => x.Signal != null));

        // sample values
        KvoResult r55 = results[55];
        Assert.IsNull(r55.Oscillator);
        Assert.IsNull(r55.Signal);

        KvoResult r56 = results[56];
        Assert.AreEqual(-2138454001, Math.Round(r56.Oscillator.Value, 0));
        Assert.IsNull(r56.Signal);

        KvoResult r57 = results[57];
        Assert.AreEqual(-2265495450, Math.Round(r57.Oscillator.Value, 0));
        Assert.IsNull(r57.Signal);

        KvoResult r68 = results[68];
        Assert.AreEqual(-1241548491, Math.Round(r68.Oscillator.Value, 0));
        Assert.AreEqual(-1489659254, Math.Round(r68.Signal.Value, 0));

        KvoResult r149 = results[149];
        Assert.AreEqual(-62800843, Math.Round(r149.Oscillator.Value, 0));
        Assert.AreEqual(-18678832, Math.Round(r149.Signal.Value, 0));

        KvoResult r249 = results[249];
        Assert.AreEqual(-51541005, Math.Round(r249.Oscillator.Value, 0));
        Assert.AreEqual(135207969, Math.Round(r249.Signal.Value, 0));

        KvoResult r501 = results[501];
        Assert.AreEqual(-539224047, Math.Round(r501.Oscillator.Value, 0));
        Assert.AreEqual(-1548306127, Math.Round(r501.Signal.Value, 0));
    }

    [TestMethod]
    public void Chainor()
    {
        IEnumerable<SmaResult> results = quotes
            .GetKvo()
            .GetSma(10);

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(437, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<KvoResult> r = Indicator.GetKvo(badQuotes);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Oscillator is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<KvoResult> r0 = noquotes.GetKvo();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<KvoResult> r1 = onequote.GetKvo();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Removed()
    {
        List<KvoResult> results =
            quotes.GetKvo(34, 55, 13)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (55 + 150), results.Count);

        KvoResult last = results.LastOrDefault();
        Assert.AreEqual(-539224047, Math.Round(last.Oscillator.Value, 0));
        Assert.AreEqual(-1548306127, Math.Round(last.Signal.Value, 0));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetKvo(quotes, 2));

        // bad slow period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetKvo(quotes, 20, 20));

        // bad signal period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Indicator.GetKvo(quotes, 34, 55, 0));
    }
}
