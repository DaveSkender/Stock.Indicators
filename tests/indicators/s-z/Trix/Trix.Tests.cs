namespace Tests.Indicators.Series;

[TestClass]
public class TrixTests : SeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        List<TrixResult> results = Quotes
            .GetTrix(20)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Ema3 != null));
        Assert.AreEqual(482, results.Count(x => x.Trix != null));

        // sample values
        TrixResult r24 = results[24];
        Assert.AreEqual(214.5486, r24.Ema3.Round(4));
        Assert.AreEqual(0.005047, r24.Trix.Round(6));

        TrixResult r67 = results[67];
        Assert.AreEqual(221.7837, r67.Ema3.Round(4));
        Assert.AreEqual(0.050030, r67.Trix.Round(6));

        TrixResult r249 = results[249];
        Assert.AreEqual(249.4469, r249.Ema3.Round(4));
        Assert.AreEqual(0.121781, r249.Trix.Round(6));

        TrixResult r501 = results[501];
        Assert.AreEqual(263.3216, r501.Ema3.Round(4));
        Assert.AreEqual(-0.230742, r501.Trix.Round(6));
    }

    [TestMethod]
    public void UseReusable()
    {
        List<TrixResult> results = Quotes
            .Use(CandlePart.Close)
            .GetTrix(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Trix != null));
    }

    [TestMethod]
    public void Chainee()
    {
        List<TrixResult> results = Quotes
            .GetSma(2)
            .GetTrix(20)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.Trix != null));
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = Quotes
            .GetTrix(20)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(473, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        List<TrixResult> r = BadQuotes
            .GetTrix(15)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Trix is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        List<TrixResult> r0 = Noquotes
            .GetTrix(5)
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<TrixResult> r1 = Onequote
            .GetTrix(5)
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        List<TrixResult> results = Quotes
            .GetTrix(20)
            .RemoveWarmupPeriods()
            .ToList();

        // assertions
        Assert.AreEqual(502 - (3 * 20 + 100), results.Count);

        TrixResult last = results.LastOrDefault();
        Assert.AreEqual(263.3216, last.Ema3.Round(4));
        Assert.AreEqual(-0.230742, last.Trix.Round(6));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetTrix(0));
}
