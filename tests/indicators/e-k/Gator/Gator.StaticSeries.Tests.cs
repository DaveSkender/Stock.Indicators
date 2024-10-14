namespace StaticSeries;

[TestClass]
public class Gator : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<GatorResult> results = Quotes
            .ToGator();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Upper != null));
        Assert.AreEqual(490, results.Count(x => x.Lower != null));
        Assert.AreEqual(481, results.Count(x => x.UpperIsExpanding != null));
        Assert.AreEqual(489, results.Count(x => x.LowerIsExpanding != null));

        // sample values
        GatorResult r11 = results[11];
        Assert.IsNull(r11.Upper);
        Assert.IsNull(r11.Lower);
        Assert.IsNull(r11.UpperIsExpanding);
        Assert.IsNull(r11.LowerIsExpanding);

        GatorResult r12 = results[12];
        Assert.IsNull(r12.Upper);
        Assert.AreEqual(-0.1402, Math.Round(r12.Lower.Value, 4));
        Assert.IsNull(r12.UpperIsExpanding);
        Assert.IsNull(r12.LowerIsExpanding);

        GatorResult r13 = results[13];
        Assert.IsNull(r13.Upper);
        Assert.AreEqual(-0.0406, Math.Round(r13.Lower.Value, 4));
        Assert.IsNull(r13.UpperIsExpanding);
        Assert.IsFalse(r13.LowerIsExpanding);

        GatorResult r19 = results[19];
        Assert.IsNull(r19.Upper);
        Assert.AreEqual(-1.0018, Math.Round(r19.Lower.Value, 4));
        Assert.IsNull(r19.UpperIsExpanding);
        Assert.IsTrue(r19.LowerIsExpanding);

        GatorResult r20 = results[20];
        Assert.AreEqual(0.4004, Math.Round(r20.Upper.Value, 4));
        Assert.AreEqual(-1.0130, Math.Round(r20.Lower.Value, 4));
        Assert.IsNull(r20.UpperIsExpanding);
        Assert.IsTrue(r20.LowerIsExpanding);

        GatorResult r21 = results[21];
        Assert.AreEqual(0.7298, Math.Round(r21.Upper.Value, 4));
        Assert.AreEqual(-0.6072, Math.Round(r21.Lower.Value, 4));
        Assert.IsTrue(r21.UpperIsExpanding);
        Assert.IsFalse(r21.LowerIsExpanding);

        GatorResult r99 = results[99];
        Assert.AreEqual(0.5159, Math.Round(r99.Upper.Value, 4));
        Assert.AreEqual(-0.2320, Math.Round(r99.Lower.Value, 4));
        Assert.IsFalse(r99.UpperIsExpanding);
        Assert.IsTrue(r99.LowerIsExpanding);

        GatorResult r249 = results[249];
        Assert.AreEqual(3.1317, Math.Round(r249.Upper.Value, 4));
        Assert.AreEqual(-1.8058, Math.Round(r249.Lower.Value, 4));
        Assert.IsTrue(r249.UpperIsExpanding);
        Assert.IsFalse(r249.LowerIsExpanding);

        GatorResult r501 = results[501];
        Assert.AreEqual(7.4538, Math.Round(r501.Upper.Value, 4));
        Assert.AreEqual(-9.2399, Math.Round(r501.Lower.Value, 4));
        Assert.IsTrue(r501.UpperIsExpanding);
        Assert.IsTrue(r501.LowerIsExpanding);
    }

    [TestMethod]
    public void FromAlligator()
    {
        IReadOnlyList<GatorResult> results = Quotes
            .ToAlligator()
            .ToGator();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Upper != null));
        Assert.AreEqual(490, results.Count(x => x.Lower != null));
        Assert.AreEqual(481, results.Count(x => x.UpperIsExpanding != null));
        Assert.AreEqual(489, results.Count(x => x.LowerIsExpanding != null));

        // sample values
        GatorResult r11 = results[11];
        Assert.IsNull(r11.Upper);
        Assert.IsNull(r11.Lower);
        Assert.IsNull(r11.UpperIsExpanding);
        Assert.IsNull(r11.LowerIsExpanding);

        GatorResult r12 = results[12];
        Assert.IsNull(r12.Upper);
        Assert.AreEqual(-0.1402, Math.Round(r12.Lower.Value, 4));
        Assert.IsNull(r12.UpperIsExpanding);
        Assert.IsNull(r12.LowerIsExpanding);

        GatorResult r13 = results[13];
        Assert.IsNull(r13.Upper);
        Assert.AreEqual(-0.0406, Math.Round(r13.Lower.Value, 4));
        Assert.IsNull(r13.UpperIsExpanding);
        Assert.IsFalse(r13.LowerIsExpanding);

        GatorResult r19 = results[19];
        Assert.IsNull(r19.Upper);
        Assert.AreEqual(-1.0018, Math.Round(r19.Lower.Value, 4));
        Assert.IsNull(r19.UpperIsExpanding);
        Assert.IsTrue(r19.LowerIsExpanding);

        GatorResult r20 = results[20];
        Assert.AreEqual(0.4004, Math.Round(r20.Upper.Value, 4));
        Assert.AreEqual(-1.0130, Math.Round(r20.Lower.Value, 4));
        Assert.IsNull(r20.UpperIsExpanding);
        Assert.IsTrue(r20.LowerIsExpanding);

        GatorResult r21 = results[21];
        Assert.AreEqual(0.7298, Math.Round(r21.Upper.Value, 4));
        Assert.AreEqual(-0.6072, Math.Round(r21.Lower.Value, 4));
        Assert.IsTrue(r21.UpperIsExpanding);
        Assert.IsFalse(r21.LowerIsExpanding);

        GatorResult r99 = results[99];
        Assert.AreEqual(0.5159, Math.Round(r99.Upper.Value, 4));
        Assert.AreEqual(-0.2320, Math.Round(r99.Lower.Value, 4));
        Assert.IsFalse(r99.UpperIsExpanding);
        Assert.IsTrue(r99.LowerIsExpanding);

        GatorResult r249 = results[249];
        Assert.AreEqual(3.1317, Math.Round(r249.Upper.Value, 4));
        Assert.AreEqual(-1.8058, Math.Round(r249.Lower.Value, 4));
        Assert.IsTrue(r249.UpperIsExpanding);
        Assert.IsFalse(r249.LowerIsExpanding);

        GatorResult r501 = results[501];
        Assert.AreEqual(7.4538, Math.Round(r501.Upper.Value, 4));
        Assert.AreEqual(-9.2399, Math.Round(r501.Lower.Value, 4));
        Assert.IsTrue(r501.UpperIsExpanding);
        Assert.IsTrue(r501.LowerIsExpanding);
    }

    [TestMethod]
    public void UseReusable()
    {
        IReadOnlyList<GatorResult> results = Quotes
            .Use(CandlePart.Close)
            .ToGator();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Upper != null));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<GatorResult> results = Quotes
            .ToSma(2)
            .ToGator();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.Upper != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<GatorResult> r = BadQuotes
            .ToGator();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Upper is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<GatorResult> r0 = Noquotes
            .ToGator();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<GatorResult> r1 = Onequote
            .ToGator();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<GatorResult> results = Quotes
            .ToGator()
            .Condense();

        // assertions
        Assert.AreEqual(490, results.Count);

        GatorResult last = results[^1];
        Assert.AreEqual(7.4538, Math.Round(last.Upper.Value, 4));
        Assert.AreEqual(-9.2399, Math.Round(last.Lower.Value, 4));
        Assert.IsTrue(last.UpperIsExpanding);
        Assert.IsTrue(last.LowerIsExpanding);
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<GatorResult> results = Quotes
            .ToGator()
            .RemoveWarmupPeriods();

        // assertions
        Assert.AreEqual(502 - 150, results.Count);

        GatorResult last = results[^1];
        Assert.AreEqual(7.4538, Math.Round(last.Upper.Value, 4));
        Assert.AreEqual(-9.2399, Math.Round(last.Lower.Value, 4));
        Assert.IsTrue(last.UpperIsExpanding);
        Assert.IsTrue(last.LowerIsExpanding);
    }
}
