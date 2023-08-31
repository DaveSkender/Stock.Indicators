using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Skender.Stock.Indicators;
using Tests.Common;

namespace Tests.Indicators;

[TestClass]
public class ZigZagTests : TestBase
{
    [TestMethod]
    public void StandardClose()
    {
        List<ZigZagResult> results =
            quotes.GetZigZag(EndType.Close, 3)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(234, results.Count(x => x.ZigZag != null));
        Assert.AreEqual(234, results.Count(x => x.RetraceHigh != null));
        Assert.AreEqual(221, results.Count(x => x.RetraceLow != null));
        Assert.AreEqual(14, results.Count(x => x.PointType != null));

        // sample values
        ZigZagResult r0 = results[249];
        Assert.AreEqual(null, r0.ZigZag);
        Assert.AreEqual(null, r0.RetraceHigh);
        Assert.AreEqual(null, r0.RetraceLow);
        Assert.AreEqual(null, r0.PointType);

        ZigZagResult r1 = results[277];
        Assert.AreEqual(248.13m, r1.ZigZag);
        Assert.AreEqual(272.248m, r1.RetraceHigh);
        Assert.AreEqual(248.13m, r1.RetraceLow);
        Assert.AreEqual("L", r1.PointType);

        ZigZagResult r2 = results[483];
        Assert.AreEqual(272.52m, r2.ZigZag);
        Assert.AreEqual(272.52m, r2.RetraceHigh);
        Assert.AreEqual(248.799m, r2.RetraceLow);
        Assert.AreEqual("H", r2.PointType);

        ZigZagResult r3 = results[439];
        Assert.AreEqual(276.0133m, r3.ZigZag.Round(4));
        Assert.AreEqual(280.9158m, r3.RetraceHigh.Round(4));
        Assert.AreEqual(264.5769m, r3.RetraceLow.Round(4));
        Assert.AreEqual(null, r3.PointType);

        ZigZagResult r4 = results[500];
        Assert.AreEqual(241.4575m, r4.ZigZag.Round(4));
        Assert.AreEqual(246.7933m, r4.RetraceHigh.Round(4));
        Assert.AreEqual(null, r4.RetraceLow);
        Assert.AreEqual(null, r4.PointType);

        ZigZagResult r5 = results[501];
        Assert.AreEqual(245.28m, r5.ZigZag);
        Assert.AreEqual(245.28m, r5.RetraceHigh);
        Assert.AreEqual(null, r5.RetraceLow);
        Assert.AreEqual(null, r5.PointType);
    }

    [TestMethod]
    public void StandardHighLow()
    {
        List<ZigZagResult> results =
            quotes.GetZigZag(EndType.HighLow, 3)
            .ToList();

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(463, results.Count(x => x.ZigZag != null));
        Assert.AreEqual(463, results.Count(x => x.RetraceHigh != null));
        Assert.AreEqual(442, results.Count(x => x.RetraceLow != null));
        Assert.AreEqual(30, results.Count(x => x.PointType != null));

        // sample values
        ZigZagResult r38 = results[38];
        Assert.AreEqual(null, r38.ZigZag);
        Assert.AreEqual(null, r38.RetraceHigh);
        Assert.AreEqual(null, r38.RetraceLow);
        Assert.AreEqual(null, r38.PointType);

        ZigZagResult r277 = results[277];
        Assert.AreEqual(252.9550m, r277.ZigZag);
        Assert.AreEqual(262.8054m, r277.RetraceHigh.Round(4));
        Assert.AreEqual(245.4467m, r277.RetraceLow.Round(4));
        Assert.AreEqual(null, r277.PointType);

        ZigZagResult r316 = results[316];
        Assert.AreEqual(249.48m, r316.ZigZag);
        Assert.AreEqual(258.34m, r316.RetraceHigh);
        Assert.AreEqual(249.48m, r316.RetraceLow);
        Assert.AreEqual("L", r316.PointType);

        ZigZagResult r456 = results[456];
        Assert.AreEqual(261.3325m, r456.ZigZag.Round(4));
        Assert.AreEqual(274.3419m, r456.RetraceHigh.Round(4));
        Assert.AreEqual(256.1050m, r456.RetraceLow.Round(4));
        Assert.AreEqual(null, r456.PointType);

        ZigZagResult r500 = results[500];
        Assert.AreEqual(240.1667m, r500.ZigZag.Round(4));
        Assert.AreEqual(246.95083m, r500.RetraceHigh.Round(5));
        Assert.AreEqual(null, r500.RetraceLow);
        Assert.AreEqual(null, r500.PointType);

        ZigZagResult r501 = results[501];
        Assert.AreEqual(245.54m, r501.ZigZag);
        Assert.AreEqual(245.54m, r501.RetraceHigh);
        Assert.AreEqual(null, r501.RetraceLow);
        Assert.AreEqual(null, r501.PointType);
    }

    [TestMethod]
    public void Chainor()
    {
        List<SmaResult> results = quotes
            .GetZigZag(EndType.Close, 3)
            .GetSma(10)
            .ToList();

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(225, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void NoEntry()
    {
        // thresholds are never met
        string json = File.ReadAllText("./s-z/ZigZag/data.ethusdt.json");

        IReadOnlyCollection<Quote> quotes = JsonConvert
            .DeserializeObject<IReadOnlyCollection<Quote>>(json);

        List<ZigZagResult> results = quotes
            .GetZigZag(EndType.Close, 5m)
            .ToList();

        Assert.AreEqual(0, results.Count(x => x.PointType != null));
    }

    [TestMethod]
    public void Issue632()
    {
        // thresholds are never met
        string json = File.ReadAllText("./s-z/ZigZag/data.issue632.json");

        List<Quote> quotesList = JsonConvert
            .DeserializeObject<IReadOnlyCollection<Quote>>(json)
            .ToList();

        List<ZigZagResult> resultsList = quotesList
            .GetZigZag(EndType.Close, 5m)
            .ToList();

        Assert.AreEqual(17, resultsList.Count);
    }

    [TestMethod]
    public void BadData()
    {
        List<ZigZagResult> r1 = badQuotes
            .GetZigZag(EndType.Close)
            .ToList();

        Assert.AreEqual(502, r1.Count);

        List<ZigZagResult> r2 = badQuotes
            .GetZigZag(EndType.HighLow)
            .ToList();

        Assert.AreEqual(502, r2.Count);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<ZigZagResult> r0 = noquotes
            .GetZigZag()
            .ToList();

        Assert.AreEqual(0, r0.Count);

        List<ZigZagResult> r1 = onequote
            .GetZigZag()
            .ToList();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        List<ZigZagResult> results = quotes
            .GetZigZag(EndType.Close, 3)
            .Condense()
            .ToList();

        // assertions
        Assert.AreEqual(14, results.Count);
    }

    [TestMethod]
    public void SchrodingerScenario()
    {
        string json = File.ReadAllText("./s-z/ZigZag/data.schrodinger.json");

        List<Quote> h = JsonConvert
            .DeserializeObject<IReadOnlyCollection<Quote>>(json)
            .OrderBy(x => x.Date)
            .ToList();

        List<ZigZagResult> r1 = h.GetZigZag(EndType.Close, 0.25m).ToList();
        Assert.AreEqual(342, r1.Count);

        // first period has High/Low that exceeds threhold
        // where it is both a H and L pivot simultaenously
        List<ZigZagResult> r2 = h.GetZigZag(EndType.HighLow, 3).ToList();
        Assert.AreEqual(342, r2.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetZigZag(EndType.Close, 0));

        // bad end type
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => quotes.GetZigZag((EndType)int.MaxValue, 2));
    }
}
