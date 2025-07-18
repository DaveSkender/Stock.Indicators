namespace StaticSeries;

[TestClass]
public class Alligator : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard()
    {
        IReadOnlyList<AlligatorResult> results = Quotes
            .ToAlligator();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Jaw != null));
        Assert.AreEqual(490, results.Count(x => x.Teeth != null));
        Assert.AreEqual(495, results.Count(x => x.Lips != null));

        // starting calculations at proper index
        Assert.IsNull(results[19].Jaw);
        Assert.IsNotNull(results[20].Jaw);

        Assert.IsNull(results[11].Teeth);
        Assert.IsNotNull(results[12].Teeth);

        Assert.IsNull(results[6].Lips);
        Assert.IsNotNull(results[7].Lips);

        // sample values
        Assert.AreEqual(213.81269, results[20].Jaw.Round(5));
        Assert.AreEqual(213.79287, results[21].Jaw.Round(5));
        Assert.AreEqual(225.60571, results[99].Jaw.Round(5));
        Assert.AreEqual(260.98953, results[501].Jaw.Round(5));

        Assert.AreEqual(213.699375, results[12].Teeth.Round(6));
        Assert.AreEqual(213.80008, results[13].Teeth.Round(5));
        Assert.AreEqual(226.12157, results[99].Teeth.Round(5));
        Assert.AreEqual(253.53576, results[501].Teeth.Round(5));

        Assert.AreEqual(213.63500, results[7].Lips.Round(5));
        Assert.AreEqual(213.74900, results[8].Lips.Round(5));
        Assert.AreEqual(226.35353, results[99].Lips.Round(5));
        Assert.AreEqual(244.29591, results[501].Lips.Round(5));
    }

    [TestMethod]
    public void Chainee()
    {
        IReadOnlyList<AlligatorResult> results = Quotes
            .ToSma(2)
            .ToAlligator();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.Jaw != null));
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<AlligatorResult> r = BadQuotes
            .ToAlligator(3, 3, 2, 1, 1, 1);

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Jaw is double.NaN));
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<AlligatorResult> r0 = Noquotes
            .ToAlligator();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<AlligatorResult> r1 = Onequote
            .ToAlligator();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<AlligatorResult> results = Quotes
            .ToAlligator()
            .Condense();

        Assert.AreEqual(495, results.Count);

        AlligatorResult last = results[^1];
        Assert.AreEqual(260.98953, last.Jaw.Round(5));
        Assert.AreEqual(253.53576, last.Teeth.Round(5));
        Assert.AreEqual(244.29591, last.Lips.Round(5));
    }

    [TestMethod]
    public void Removed()
    {
        IReadOnlyList<AlligatorResult> results = Quotes
            .ToAlligator()
            .RemoveWarmupPeriods();

        Assert.AreEqual(502 - 21 - 250, results.Count);

        AlligatorResult last = results[^1];
        Assert.AreEqual(260.98953, last.Jaw.Round(5));
        Assert.AreEqual(253.53576, last.Teeth.Round(5));
        Assert.AreEqual(244.29591, last.Lips.Round(5));
    }

    [TestMethod]
    public void Equality()
    {
        AlligatorResult r1 = new(EvalDate, 1d, null, null);
        AlligatorResult r2 = new(EvalDate, 1d, null, null);
        AlligatorResult r3 = new(EvalDate, 2d, null, null); // abberent

        Assert.IsTrue(Equals(r1, r2));
        Assert.IsFalse(Equals(r1, r3));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad jaw lookback periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToAlligator(13, 8, 13));

        // bad teeth lookback periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToAlligator(13, 8, 8, 5, 8));

        // bad lips lookback periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToAlligator(13, 8, 8, 5, 0));

        // bad jaw offset periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToAlligator(13, 0));

        // bad teeth offset periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToAlligator(13, 8, 8, 0));

        // bad lips offset periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToAlligator(13, 8, 8, 5, 5, 0));

        // bad jaw + offset periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToAlligator(13, 8, 12, 11));

        // bad teeth + offset periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => Quotes.ToAlligator(13, 8, 8, 5, 7, 7));
    }
}
