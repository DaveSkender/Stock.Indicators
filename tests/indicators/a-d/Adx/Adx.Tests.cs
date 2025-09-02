namespace Tests.Indicators;

[TestClass]
public class AdxTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<AdxResult> results = quotes
            .GetAdx(14)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.AreEqual(475, results.Count(x => x.Adx != null));
        Assert.AreEqual(462, results.Count(x => x.Adxr != null));

        // sample values
        AdxResult r19 = results[19];
        Assert.AreEqual(21.0361, r19.Pdi.Round(4));
        Assert.AreEqual(25.0124, r19.Mdi.Round(4));
        Assert.IsNull(r19.Adx);

        AdxResult r29 = results[29];
        Assert.AreEqual(37.9719, r29.Pdi.Round(4));
        Assert.AreEqual(14.1658, r29.Mdi.Round(4));
        Assert.AreEqual(19.7949, r29.Adx.Round(4));

        AdxResult r39 = results[39];
        Assert.IsNull(r39.Adxr);

        AdxResult r40 = results[40];
        Assert.AreEqual(29.1062, r40.Adxr.Round(4));

        AdxResult r248 = results[248];
        Assert.AreEqual(32.3167, r248.Pdi.Round(4));
        Assert.AreEqual(18.2471, r248.Mdi.Round(4));
        Assert.AreEqual(30.5903, r248.Adx.Round(4));
        Assert.AreEqual(29.1252, r248.Adxr.Round(4));

        AdxResult r501 = results[501];
        Assert.AreEqual(17.7565, r501.Pdi.Round(4));
        Assert.AreEqual(31.1510, r501.Mdi.Round(4));
        Assert.AreEqual(34.2987, r501.Adx.Round(4));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetAdx(14)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.AreEqual(466, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<AdxResult> r = badQuotes
            .GetAdx(20)
            .ToList();

        Assert.HasCount(502, r);
        Assert.AreEqual(0, r.Count(x => x.Adx is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public void BigData()
    {
        List<AdxResult> r = bigQuotes
            .GetAdx(200)
            .ToList();

        Assert.HasCount(1246, r);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<AdxResult> r0 = noquotes
            .GetAdx(5)
            .ToList();

        Assert.IsEmpty(r0);

        List<AdxResult> r1 = onequote
            .GetAdx(5)
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Issue859()
    {
        IOrderedEnumerable<Quote> test859 = File.ReadAllLines("a-d/Adx/issue859quotes.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Date);

        List<AdxResult> r = test859.GetAdx(14).ToList();

        Assert.AreEqual(0, r.Count(x => x.Adx is double v && double.IsNaN(v)));
        Assert.HasCount(595, r);
    }

    [TestMethod]
    public void Zeroes()
    {
        List<AdxResult> r = zeroesQuotes.GetAdx(14).ToList();

        Assert.AreEqual(0, r.Count(x => x.Adx is double v && double.IsNaN(v)));
        Assert.HasCount(200, r);
    }

    [TestMethod]
    public void Removed()
    {
        List<AdxResult> r = quotes.GetAdx(14)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.HasCount(502 - ((2 * 14) + 100), r);

        AdxResult last = r.LastOrDefault();
        Assert.AreEqual(17.7565, last.Pdi.Round(4));
        Assert.AreEqual(31.1510, last.Mdi.Round(4));
        Assert.AreEqual(34.2987, last.Adx.Round(4));
    }

    [TestMethod] // bad lookback period
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => quotes.GetAdx(1));
}
