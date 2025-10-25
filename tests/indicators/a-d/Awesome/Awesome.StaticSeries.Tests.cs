namespace StaticSeries;

[TestClass]
public class Awesome : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<AwesomeResult> results = Quotes
            .ToAwesome();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(469, results.Where(x => x.Oscillator != null));

        // sample values
        AwesomeResult r1 = results[32];
        Assert.IsNull(r1.Oscillator);
        Assert.IsNull(r1.Normalized);

        AwesomeResult r2 = results[33];
        Assert.AreEqual(5.4756, r2.Oscillator.Round(4));
        Assert.AreEqual(2.4548, r2.Normalized.Round(4));

        AwesomeResult r3 = results[249];
        Assert.AreEqual(5.0618, r3.Oscillator.Round(4));
        Assert.AreEqual(1.9634, r3.Normalized.Round(4));

        AwesomeResult r4 = results[501];
        Assert.AreEqual(-17.7692, r4.Oscillator.Round(4));
        Assert.AreEqual(-7.2763, r4.Normalized.Round(4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<AwesomeResult> results = Quotes
            .Use(CandlePart.Close)
            .ToAwesome();

        Assert.HasCount(502, results);
        Assert.HasCount(469, results.Where(x => x.Oscillator != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<AwesomeResult> results = Quotes
            .ToSma(2)
            .ToAwesome();

        Assert.HasCount(502, results);
        Assert.HasCount(468, results.Where(x => x.Oscillator != null));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToAwesome()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(460, results.Where(x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<AwesomeResult> r = BadQuotes
            .ToAwesome();

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(x => x.Oscillator is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<AwesomeResult> r0 = Noquotes
            .ToAwesome();

        Assert.IsEmpty(r0);

        IReadOnlyList<AwesomeResult> r1 = Onequote
            .ToAwesome();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<AwesomeResult> results = Quotes
            .ToAwesome()
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - 33, results);

        AwesomeResult last = results[^1];
        Assert.AreEqual(-17.7692, last.Oscillator.Round(4));
        Assert.AreEqual(-7.2763, last.Normalized.Round(4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad fast period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToAwesome(0, 34));

        // bad slow period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => Quotes.ToAwesome(25, 25));
    }
}
