namespace StaticSeries;

[TestClass]
public class FisherTransform : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<FisherTransformResult> results = Quotes
            .ToFisherTransform();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(501, results.Where(static x => x.Fisher != 0));

        // sample values
        Assert.AreEqual(0, results[0].Fisher);
        Assert.IsNull(results[0].Trigger);

        Assert.AreEqual(0.3428, Math.Round(results[1].Fisher.Value, 4));
        Assert.AreEqual(0, results[1].Trigger);

        Assert.AreEqual(0.6873, Math.Round(results[2].Fisher.Value, 4));
        Assert.AreEqual(0.3428, Math.Round(results[2].Trigger.Value, 4));

        Assert.AreEqual(1.3324, Math.Round(results[9].Fisher.Value, 4));
        Assert.AreEqual(1.4704, Math.Round(results[9].Trigger.Value, 4));

        Assert.AreEqual(0.9790, Math.Round(results[10].Fisher.Value, 4));
        Assert.AreEqual(1.3324, Math.Round(results[10].Trigger.Value, 4));

        Assert.AreEqual(6.1509, Math.Round(results[35].Fisher.Value, 4));
        Assert.AreEqual(4.7014, Math.Round(results[35].Trigger.Value, 4));

        Assert.AreEqual(5.4455, Math.Round(results[36].Fisher.Value, 4));
        Assert.AreEqual(6.1509, Math.Round(results[36].Trigger.Value, 4));

        Assert.AreEqual(1.0349, Math.Round(results[149].Fisher.Value, 4));
        Assert.AreEqual(0.7351, Math.Round(results[149].Trigger.Value, 4));

        Assert.AreEqual(1.3496, Math.Round(results[249].Fisher.Value, 4));
        Assert.AreEqual(1.4408, Math.Round(results[249].Trigger.Value, 4));

        Assert.AreEqual(-1.2876, Math.Round(results[501].Fisher.Value, 4));
        Assert.AreEqual(-2.0071, Math.Round(results[501].Trigger.Value, 4));
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<FisherTransformResult> results = Quotes
            .Use(CandlePart.Close)
            .ToFisherTransform();

        Assert.HasCount(502, results);
        Assert.HasCount(501, results.Where(static x => x.Fisher != 0));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<FisherTransformResult> results = Quotes
            .ToSma(2)
            .ToFisherTransform();

        Assert.HasCount(502, results);
        Assert.HasCount(501, results.Where(static x => x.Fisher != 0));
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToFisherTransform()
            .ToSma(10);

        Assert.HasCount(502, results);
        Assert.HasCount(493, results.Where(static x => x.Sma != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<FisherTransformResult> r = BadQuotes
            .ToFisherTransform(9);

        Assert.HasCount(502, r);
        Assert.IsEmpty(r.Where(static x => x.Fisher is double v && double.IsNaN(v)));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<FisherTransformResult> r0 = Noquotes
            .ToFisherTransform();

        Assert.IsEmpty(r0);

        IReadOnlyList<FisherTransformResult> r1 = Onequote
            .ToFisherTransform();

        Assert.HasCount(1, r1);
    }

    /// <summary>
    /// bad lookback period
    /// </summary>
    [TestMethod]
    public void Exceptions()
        => Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => Quotes.ToFisherTransform(0));
}
