namespace StaticSeries;

[TestClass]
public class Wma : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<WmaResult> results = Quotes
            .GetWma(20);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Wma != null));

        // sample values
        WmaResult r1 = results[149];
        Assert.AreEqual(235.5253, r1.Wma.Round(4));

        WmaResult r2 = results[501];
        Assert.AreEqual(246.5110, r2.Wma.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<WmaResult> results = Quotes
            .Use(CandlePart.Close)
            .GetWma(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Wma != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<WmaResult> results = Quotes
            .ToSma(2)
            .GetWma(20);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Wma != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .GetWma(20)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(474, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void Chaining()
    {
        IReadOnlyList<WmaResult> standard = Quotes
            .GetWma(17);

        IReadOnlyList<WmaResult> results = Quotes
            .Use(CandlePart.Close)
            .GetWma(17);

        // assertions
        for (int i = 0; i < results.Count; i++)
        {
            WmaResult s = standard[i];
            WmaResult c = results[i];

            Assert.AreEqual(s.Timestamp, c.Timestamp);
            Assert.AreEqual(s.Wma, c.Wma);
        }
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<WmaResult> r = BadQuotes
            .GetWma(15);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Wma is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<WmaResult> r0 = Noquotes
            .GetWma(5);

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<WmaResult> r1 = Onequote
            .GetWma(5);

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<WmaResult> results = Quotes
            .GetWma(20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 19, results.Count);

        WmaResult last = results[^1];
        Assert.AreEqual(246.5110, last.Wma.Round(4));
    }

    // bad lookback period
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => Quotes.GetWma(0));
}
