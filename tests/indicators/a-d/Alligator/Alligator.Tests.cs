using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Alligator : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<AlligatorResult> results = quotes.GetAlligator().ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
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
        Assert.AreEqual(213.81269, NullMath.Round(results[20].Jaw, 5));
        Assert.AreEqual(213.79287, NullMath.Round(results[21].Jaw, 5));
        Assert.AreEqual(225.60571, NullMath.Round(results[99].Jaw, 5));
        Assert.AreEqual(260.98953, NullMath.Round(results[501].Jaw, 5));

        Assert.AreEqual(213.699375, NullMath.Round(results[12].Teeth, 6));
        Assert.AreEqual(213.80008, NullMath.Round(results[13].Teeth, 5));
        Assert.AreEqual(226.12157, NullMath.Round(results[99].Teeth, 5));
        Assert.AreEqual(253.53576, NullMath.Round(results[501].Teeth, 5));

        Assert.AreEqual(213.63500, NullMath.Round(results[7].Lips, 5));
        Assert.AreEqual(213.74900, NullMath.Round(results[8].Lips, 5));
        Assert.AreEqual(226.35353, NullMath.Round(results[99].Lips, 5));
        Assert.AreEqual(244.29591, NullMath.Round(results[501].Lips, 5));
    }

    [TestMethod]
    public void UseTuple()
    {
        IEnumerable<AlligatorResult> results = quotes
            .Use(CandlePart.HL2)
            .GetAlligator();

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(482, results.Count(x => x.Jaw != null));

        AlligatorResult last = results.LastOrDefault();
        Assert.AreEqual(244.29591, NullMath.Round(last.Lips, 5));
    }

    [TestMethod]
    public void TupleNaN()
    {
        IEnumerable<AlligatorResult> r = tupleNanny.GetAlligator();

        Assert.AreEqual(200, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Lips is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        IEnumerable<AlligatorResult> results = quotes
            .GetSma(2)
            .GetAlligator();

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(481, results.Count(x => x.Jaw != null));
    }

    [TestMethod]
    public void Sync()
    {
        IEnumerable<AlligatorResult> results = quotes
            .GetSma(3)
            .GetAlligator();

        Assert.AreEqual(502, results.Count());
        Assert.AreEqual(480, results.Count(x => x.Jaw != null));
    }

    [TestMethod]
    public void BadData()
    {
        IEnumerable<AlligatorResult> r = badQuotes.GetAlligator(3, 3, 2, 1, 1, 1);
        Assert.AreEqual(502, r.Count());
        Assert.AreEqual(0, r.Count(x => x.Jaw is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        IEnumerable<AlligatorResult> r0 = noquotes.GetAlligator();
        Assert.AreEqual(0, r0.Count());

        IEnumerable<AlligatorResult> r1 = onequote.GetAlligator();
        Assert.AreEqual(1, r1.Count());
    }

    [TestMethod]
    public void Condense()
    {
        IEnumerable<AlligatorResult> r = quotes.GetAlligator()
            .Condense();

        Assert.AreEqual(495, r.Count());

        AlligatorResult last = r.LastOrDefault();
        Assert.AreEqual(260.98953, NullMath.Round(last.Jaw, 5));
        Assert.AreEqual(253.53576, NullMath.Round(last.Teeth, 5));
        Assert.AreEqual(244.29591, NullMath.Round(last.Lips, 5));
    }

    [TestMethod]
    public void Removed()
    {
        IEnumerable<AlligatorResult> r = quotes.GetAlligator(13, 8)
            .RemoveWarmupPeriods();

        Assert.AreEqual(502 - 21 - 250, r.Count());

        AlligatorResult last = r.LastOrDefault();
        Assert.AreEqual(260.98953, NullMath.Round(last.Jaw, 5));
        Assert.AreEqual(253.53576, NullMath.Round(last.Teeth, 5));
        Assert.AreEqual(244.29591, NullMath.Round(last.Lips, 5));
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad jaw lookback periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAlligator(13, 8, 13, 5, 5, 3));

        // bad teeth lookback periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAlligator(13, 8, 8, 5, 8, 3));

        // bad lips lookback periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAlligator(13, 8, 8, 5, 0, 3));

        // bad jaw offset periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAlligator(13, 0, 8, 5, 5, 3));

        // bad teeth offset periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAlligator(13, 8, 8, 0, 5, 3));

        // bad lips offset periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAlligator(13, 8, 8, 5, 5, 0));

        // bad jaw + offset periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAlligator(13, 8, 12, 11, 5, 3));

        // bad teeth + offset periods
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAlligator(13, 8, 8, 5, 7, 7));
    }
}
