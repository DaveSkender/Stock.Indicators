using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class NewHighsNewLowsTests : TestBase
{
    [TestMethod]
    public void Standard()
    {
        List<NewHighsNewLowsResult> results = quotes
            .GetNewHighsNewLows()
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);

        // sample values
        NewHighsNewLowsResult r1 = results[0];
        Assert.AreEqual(0, r1.NewHighs.Round(4));

        NewHighsNewLowsResult r2 = results[0];
        Assert.AreEqual(0, r2.NewLows.Round(4));

        NewHighsNewLowsResult r3 = results[0];
        Assert.AreEqual(0, r3.Net.Round(4));

        NewHighsNewLowsResult r4 = results[1];
        Assert.AreEqual(1, r4.NewHighs.Round(4));

        NewHighsNewLowsResult r5 = results[1];
        Assert.AreEqual(0, r5.NewLows.Round(4));

        NewHighsNewLowsResult r6 = results[1];
        Assert.AreEqual(1, r6.Net.Round(4));

        NewHighsNewLowsResult r7 = results[439];
        Assert.AreEqual(51, r7.NewHighs.Round(4));

        NewHighsNewLowsResult r8 = results[439];
        Assert.AreEqual(0, r8.NewLows.Round(4));

        NewHighsNewLowsResult r9 = results[439];
        Assert.AreEqual(51, r9.Net.Round(4));

        NewHighsNewLowsResult r10 = results.Last();
        Assert.AreEqual(71.875, r10.RecordHighPercent.Round(4));

        NewHighsNewLowsResult r12 = results.Last();
        Assert.AreEqual(21699, r12.Cumulative.Round(4));
    }

    [TestMethod]
    public void Market()
    {
        List<NewHighsNewLowsResult> r1 = quotes
            .GetNewHighsNewLows()
            .ToList();

        List<NewHighsNewLowsResult> r2 = quotes
            .GetNewHighsNewLows()
            .ToList();

        var rs = new List<List<NewHighsNewLowsResult>>();
        rs.Add(r1);
        rs.Add(r2);

        List<NewHighsNewLowsResult> r3 = rs
            .GetNewHighsNewLows()
            .ToList();

        Assert.AreEqual(502, r3.Count);
    }

    [TestMethod]
    public void BadData()
    {
        List<NewHighsNewLowsResult> r = badQuotes
            .GetNewHighsNewLows()
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<NewHighsNewLowsResult> r0 = noquotes
            .GetNewHighsNewLows()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<NewHighsNewLowsResult> r1 = onequote
            .GetNewHighsNewLows()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        List<NewHighsNewLowsResult> r = quotes
            .GetNewHighsNewLows()
            .Condense()
            .ToList();

        Assert.AreEqual(502, r.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad minimum body percent values
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetNewHighsNewLows(tradingDays: 0));
    }
}
