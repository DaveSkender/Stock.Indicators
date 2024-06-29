namespace Tests.Indicators.Series;

[TestClass]
public class AdxTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<AdxResult> results = Quotes
            .GetAdx()
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(475, results.Count(x => x.Adx != null));
        Assert.AreEqual(462, results.Count(x => x.Adxr != null));

        // sample values
        AdxResult r19 = results[19];
        Assert.AreEqual(21.0361, r19.Pdi.Round(4));
        Assert.AreEqual(25.0124, r19.Mdi.Round(4));
        Assert.AreEqual(null, r19.Adx);

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
        List<SmaResult> results = Quotes
            .GetAdx()
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(466, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<AdxResult> r = BadQuotes
            .GetAdx(20)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Adx is double.NaN));
    }

    [TestMethod]
    public void BigData()
    {
        List<AdxResult> r = BigQuotes
            .GetAdx(200)
            .ToList();

        Assert.AreEqual(1246, r.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<AdxResult> r0 = Noquotes
            .GetAdx(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<AdxResult> r1 = Onequote
            .GetAdx(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Issue859()
    {
        IOrderedEnumerable<Quote> test859 = File.ReadAllLines("a-d/Adx/issue859quotes.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(x => x.Timestamp);

        List<AdxResult> r = test859.GetAdx().ToList();

        Assert.AreEqual(0, r.Count(x => x.Adx is double.NaN));
        Assert.AreEqual(595, r.Count);
    }

    [TestMethod]
    public void Zeroes()
    {
        List<AdxResult> r = ZeroesQuotes.GetAdx().ToList();

        Assert.AreEqual(0, r.Count(x => x.Adx is double.NaN));
        Assert.AreEqual(200, r.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<AdxResult> r = Quotes.GetAdx()
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (2 * 14 + 100), r.Count);

        AdxResult last = r.LastOrDefault();
        Assert.AreEqual(17.7565, last.Pdi.Round(4));
        Assert.AreEqual(31.1510, last.Mdi.Round(4));
        Assert.AreEqual(34.2987, last.Adx.Round(4));
    }

    [TestMethod]
    public void Exceptions() =>

        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            Quotes.GetAdx(1));
}
