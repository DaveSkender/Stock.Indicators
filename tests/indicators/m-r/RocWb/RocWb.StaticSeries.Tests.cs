namespace StaticSeries;

[TestClass]
public class RocWb : StaticSeriesTestBase
{
    [TestMethod]
    public override void DefaultParameters_ReturnsExpectedResults()
    {
        IReadOnlyList<RocWbResult> results = Quotes
            .ToRocWb(20, 3, 20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Roc != null));
        Assert.HasCount(480, results.Where(static x => x.RocEma != null));
        Assert.HasCount(463, results.Where(static x => x.UpperBand != null));
        Assert.HasCount(463, results.Where(static x => x.LowerBand != null));

        // sample values
        RocWbResult r19 = results[19];
        Assert.IsNull(r19.Roc);
        Assert.IsNull(r19.RocEma);
        Assert.IsNull(r19.UpperBand);
        Assert.IsNull(r19.LowerBand);

        RocWbResult r20 = results[20];
        Assert.AreEqual(1.0573, Math.Round(r20.Roc.Value, 4));
        Assert.IsNull(r20.RocEma);
        Assert.IsNull(r20.UpperBand);
        Assert.IsNull(r20.LowerBand);

        RocWbResult r22 = results[22];
        Assert.AreEqual(0.9617, Math.Round(r22.RocEma.Value, 4));
        Assert.IsNull(r22.UpperBand);
        Assert.IsNull(r22.LowerBand);

        RocWbResult r23 = results[23];
        Assert.AreEqual(0.8582, Math.Round(r23.RocEma.Value, 4));
        Assert.IsNull(r23.UpperBand);
        Assert.IsNull(r23.LowerBand);

        RocWbResult r38 = results[38];
        Assert.AreEqual(3.6872, Math.Round(r38.RocEma.Value, 4));
        Assert.IsNull(r38.UpperBand);
        Assert.IsNull(r38.LowerBand);

        RocWbResult r39 = results[39];
        Assert.AreEqual(4.5348, Math.Round(r39.RocEma.Value, 4));
        Assert.AreEqual(3.0359, Math.Round(r39.UpperBand.Value, 4));
        Assert.AreEqual(-3.0359, Math.Round(r39.LowerBand.Value, 4));

        RocWbResult r49 = results[49];
        Assert.AreEqual(2.3147, Math.Round(r49.RocEma.Value, 4));
        Assert.AreEqual(3.6761, Math.Round(r49.UpperBand.Value, 4));

        RocWbResult r149 = results[149];
        Assert.AreEqual(1.7377, Math.Round(r149.UpperBand.Value, 4));

        RocWbResult r249 = results[249];
        Assert.AreEqual(3.0683, Math.Round(r249.UpperBand.Value, 4));

        RocWbResult r501 = results[501];
        Assert.AreEqual(-8.2482, Math.Round(r501.Roc.Value, 4));
        Assert.AreEqual(-8.3390, Math.Round(r501.RocEma.Value, 4));
        Assert.AreEqual(6.1294, Math.Round(r501.UpperBand.Value, 4));
        Assert.AreEqual(-6.1294, Math.Round(r501.LowerBand.Value, 4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<RocWbResult> results = Quotes
            .Use(CandlePart.Close)
            .ToRocWb(20, 3, 20);

        Assert.HasCount(502, results);
        Assert.HasCount(482, results.Where(static x => x.Roc != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<RocWbResult> results = Quotes
            .ToSma(2)
            .ToRocWb(20, 3, 20);

        Assert.HasCount(502, results);
        Assert.HasCount(481, results.Where(static x => x.Roc != null));
    }

    [TestMethod]
    public void ChainingFromResults_WorksAsExpected()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToRocWb(20, 3, 20)
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(473, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadQuotes_DoesNotFail()
    {
        IReadOnlyList<RocWbResult> r = BadQuotes
            .ToRocWb(35, 3, 35);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Roc is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes_ReturnsEmpty()
    {
        IReadOnlyList<RocWbResult> r0 = Noquotes
            .ToRocWb(5, 3, 2);

        Assert.IsEmpty(r0);

        IReadOnlyList<RocWbResult> r1 = Onequote
            .ToRocWb(5, 3, 2);

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<RocWbResult> results = Quotes
            .ToRocWb(20, 3, 20)
            .RemoveWarmupPeriods();

        // assertions
        Assert.HasCount(502 - (20 + 3 + 100), results);

        RocWbResult last = results[^1];
        Assert.AreEqual(-8.2482, Math.Round(last.Roc.Value, 4));
        Assert.AreEqual(-8.3390, Math.Round(last.RocEma.Value, 4));
        Assert.AreEqual(6.1294, Math.Round(last.UpperBand.Value, 4));
        Assert.AreEqual(-6.1294, Math.Round(last.LowerBand.Value, 4));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToRocWb(0, 3, 12));

        // bad EMA period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToRocWb(14, 0, 14));

        // bad STDDEV period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToRocWb(15, 3, 16));
    }
}
