using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class AlligatorTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<AlligatorResult> results = quotes
            .GetAlligator()
            .ToList();

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
    public void UseTuple()
    {
        List<AlligatorResult> results = quotes
            .Use(CandlePart.HL2)
            .GetAlligator()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(482, results.Count(x => x.Jaw != null));

        AlligatorResult last = results.LastOrDefault();
        Assert.AreEqual(244.29591, last.Lips.Round(5));
    }

    [TestMethod]
    public void TupleNaN()
    {
        List<AlligatorResult> r = tupleNanny
            .GetAlligator()
            .ToList();

        Assert.AreEqual(200, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Lips is double and double.NaN));
    }

    [TestMethod]
    public void Chainee()
    {
        List<AlligatorResult> results = quotes
            .GetSma(2)
            .GetAlligator()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(481, results.Count(x => x.Jaw != null));
    }

    [TestMethod]
    public void Sync()
    {
        List<AlligatorResult> results = quotes
            .GetSma(3)
            .GetAlligator()
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(480, results.Count(x => x.Jaw != null));
    }

    [TestMethod]
    public void BadData()
    {
        List<AlligatorResult> r = badQuotes
            .GetAlligator(3, 3, 2, 1, 1, 1)
            .ToList();

        Assert.AreEqual(502, r.Count);
        Assert.AreEqual(0, r.Count(x => x.Jaw is double and double.NaN));
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<AlligatorResult> r0 = noquotes
            .GetAlligator()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<AlligatorResult> r1 = onequote
            .GetAlligator()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        List<AlligatorResult> r = quotes
            .GetAlligator()
            .Condense()
            .ToList();

        Assert.AreEqual(495, r.Count);

        AlligatorResult last = r.LastOrDefault();
        Assert.AreEqual(260.98953, last.Jaw.Round(5));
        Assert.AreEqual(253.53576, last.Teeth.Round(5));
        Assert.AreEqual(244.29591, last.Lips.Round(5));
    }

    [TestMethod]
    public void Removed()
    {
        List<AlligatorResult> r = quotes
            .GetAlligator(13, 8)
            .RemoveWarmupPeriods()
            .ToList();

        Assert.AreEqual(502 - 21 - 250, r.Count);

        AlligatorResult last = r.LastOrDefault();
        Assert.AreEqual(260.98953, last.Jaw.Round(5));
        Assert.AreEqual(253.53576, last.Teeth.Round(5));
        Assert.AreEqual(244.29591, last.Lips.Round(5));
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
