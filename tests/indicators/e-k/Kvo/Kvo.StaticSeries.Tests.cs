namespace StaticSeries;

[TestClass]
public class Kvo : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<KvoResult> results =
            Quotes.ToKvo();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(446, results.Where(x => x.Oscillator != null));
        Assert.HasCount(434, results.Where(x => x.Signal != null));

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
        IReadOnlyList<SmaResult> results = Quotes
            .ToKvo()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(437, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<KvoResult> r = BadQuotes
            .ToKvo();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Oscillator is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<KvoResult> r0 = Noquotes
            .ToKvo();

        Assert.IsEmpty(r0);

        IReadOnlyList<KvoResult> r1 = Onequote
            .ToKvo();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<KvoResult> results = Quotes
            .ToKvo()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - (55 + 150), results);

        KvoResult last = results[^1];
        Assert.AreEqual(-539224047, Math.Round(last.Oscillator.Value, 0));
        Assert.AreEqual(-1548306127, Math.Round(last.Signal.Value, 0));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToKvo(2));

        // bad slow period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToKvo(20, 20));

        // bad signal period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToKvo(34, 55, 0));
    }
}
