using Test.Data;

namespace StaticSeries;

[TestClass]
public class Adx : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<AdxResult> results = Quotes.ToAdx();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(488, results.Where(x => x.Dx != null));
        Assert.HasCount(475, results.Where(x => x.Adx != null));
        Assert.HasCount(462, results.Where(x => x.Adxr != null));

        // sample values
        AdxResult r13 = results[13];
        Assert.IsNull(r13.Pdi);
        Assert.IsNull(r13.Mdi);
        Assert.IsNull(r13.Dx);
        Assert.IsNull(r13.Adx);

        AdxResult r14 = results[14];
        Assert.AreEqual(21.9669, r14.Pdi.Round(4));
        Assert.AreEqual(18.5462, r14.Mdi.Round(4));
        Assert.AreEqual(8.4433, r14.Dx.Round(4));
        Assert.IsNull(r14.Adx);

        AdxResult r19 = results[19];
        Assert.AreEqual(21.0361, r19.Pdi.Round(4));
        Assert.AreEqual(25.0124, r19.Mdi.Round(4));
        Assert.AreEqual(8.6351, r19.Dx.Round(4));
        Assert.IsNull(r19.Adx);

        AdxResult r26 = results[26];
        Assert.IsNull(r26.Adx);

        AdxResult r27 = results[27];
        Assert.AreEqual(15.9459, r27.Adx.Round(4));

        AdxResult r29 = results[29];
        Assert.AreEqual(37.9719, r29.Pdi.Round(4));
        Assert.AreEqual(14.1658, r29.Mdi.Round(4));
        Assert.AreEqual(45.6600, r29.Dx.Round(4));
        Assert.AreEqual(19.7949, r29.Adx.Round(4));

        AdxResult r39 = results[39];
        Assert.IsNull(r39.Adxr);

        AdxResult r40 = results[40];
        Assert.AreEqual(29.1062, r40.Adxr.Round(4));

        AdxResult r248 = results[248];
        Assert.AreEqual(32.3167, r248.Pdi.Round(4));
        Assert.AreEqual(18.2471, r248.Mdi.Round(4));
        Assert.AreEqual(27.8255, r248.Dx.Round(4));
        Assert.AreEqual(30.5903, r248.Adx.Round(4));
        Assert.AreEqual(29.1252, r248.Adxr.Round(4));

        AdxResult r501 = results[501];
        Assert.AreEqual(17.7565, r501.Pdi.Round(4));
        Assert.AreEqual(31.1510, r501.Mdi.Round(4));
        Assert.AreEqual(27.3873, r501.Dx.Round(4));
        Assert.AreEqual(34.2987, r501.Adx.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToAdx()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(466, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<AdxResult> r = BadQuotes.ToAdx(20);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Adx is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void BigData()
    {
        IReadOnlyList<AdxResult> r = BigQuotes.ToAdx(200);

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<AdxResult> r0 = Noquotes.ToAdx(5);

        Assert.IsEmpty(r0);

        IReadOnlyList<AdxResult> r1 = Onequote.ToAdx(5);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Issue859()
    {
        List<Quote> test859 = File.ReadAllLines("a-d/Adx/issue859quotes.csv")
            .Skip(1)
            .Select(Test.Data.Convert.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp)
            .ToList();

        IReadOnlyList<AdxResult> r = test859.ToAdx();

        Assert.IsEmpty(r.Where(x => x.Adx is double v && double.IsNaN(v)));
        Assert.HasCount(595, r);
    }

    [TestMethod]
    public void Zeroes()
    {
        IReadOnlyList<AdxResult> r = ZeroesQuotes.ToAdx();

        Assert.IsEmpty(r.Where(x => x.Adx is double v && double.IsNaN(v)));
        Assert.HasCount(200, r);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<AdxResult> results = Quotes
            .ToAdx()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - ((2 * 14) + 100), results);

        AdxResult last = results[^1];
        Assert.AreEqual(17.7565, last.Pdi.Round(4));
        Assert.AreEqual(31.1510, last.Mdi.Round(4));
        Assert.AreEqual(34.2987, last.Adx.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => Quotes.ToAdx(1));
}
