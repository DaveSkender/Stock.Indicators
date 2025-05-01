using Newtonsoft.Json;

namespace StaticSeries;

[TestClass]
public class ZigZag : StaticSeriesTestBase
{
    [TestMethod]
    public override void Standard() // on Close
    {
        IReadOnlyList<ZigZagResult> results =
            Quotes.ToZigZag(EndType.Close, 3);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(234, results.Count(x => x.ZigZag != null));
        Assert.AreEqual(234, results.Count(x => x.RetraceHigh != null));
        Assert.AreEqual(221, results.Count(x => x.RetraceLow != null));
        Assert.AreEqual(14, results.Count(x => x.PointType != null));

        // sample values
        ZigZagResult r0 = results[249];
        Assert.IsNull(r0.ZigZag);
        Assert.IsNull(r0.RetraceHigh);
        Assert.IsNull(r0.RetraceLow);
        Assert.IsNull(r0.PointType);

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
        Assert.IsNull(r3.PointType);

        ZigZagResult r4 = results[500];
        Assert.AreEqual(241.4575m, r4.ZigZag.Round(4));
        Assert.AreEqual(246.7933m, r4.RetraceHigh.Round(4));
        Assert.IsNull(r4.RetraceLow);
        Assert.IsNull(r4.PointType);

        ZigZagResult r5 = results[501];
        Assert.AreEqual(245.28m, r5.ZigZag);
        Assert.AreEqual(245.28m, r5.RetraceHigh);
        Assert.IsNull(r5.RetraceLow);
        Assert.IsNull(r5.PointType);
    }

    [TestMethod]
    public void StandardHighLow()
    {
        IReadOnlyList<ZigZagResult> results =
            Quotes.ToZigZag(EndType.HighLow, 3);

        // proper quantities
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(463, results.Count(x => x.ZigZag != null));
        Assert.AreEqual(463, results.Count(x => x.RetraceHigh != null));
        Assert.AreEqual(442, results.Count(x => x.RetraceLow != null));
        Assert.AreEqual(30, results.Count(x => x.PointType != null));

        // sample values
        ZigZagResult r38 = results[38];
        Assert.IsNull(r38.ZigZag);
        Assert.IsNull(r38.RetraceHigh);
        Assert.IsNull(r38.RetraceLow);
        Assert.IsNull(r38.PointType);

        ZigZagResult r277 = results[277];
        Assert.AreEqual(252.9550m, r277.ZigZag);
        Assert.AreEqual(262.8054m, r277.RetraceHigh.Round(4));
        Assert.AreEqual(245.4467m, r277.RetraceLow.Round(4));
        Assert.IsNull(r277.PointType);

        ZigZagResult r316 = results[316];
        Assert.AreEqual(249.48m, r316.ZigZag);
        Assert.AreEqual(258.34m, r316.RetraceHigh);
        Assert.AreEqual(249.48m, r316.RetraceLow);
        Assert.AreEqual("L", r316.PointType);

        ZigZagResult r456 = results[456];
        Assert.AreEqual(261.3325m, r456.ZigZag.Round(4));
        Assert.AreEqual(274.3419m, r456.RetraceHigh.Round(4));
        Assert.AreEqual(256.1050m, r456.RetraceLow.Round(4));
        Assert.IsNull(r456.PointType);

        ZigZagResult r500 = results[500];
        Assert.AreEqual(240.1667m, r500.ZigZag.Round(4));
        Assert.AreEqual(246.95083m, r500.RetraceHigh.Round(5));
        Assert.IsNull(r500.RetraceLow);
        Assert.IsNull(r500.PointType);

        ZigZagResult r501 = results[501];
        Assert.AreEqual(245.54m, r501.ZigZag);
        Assert.AreEqual(245.54m, r501.RetraceHigh);
        Assert.IsNull(r501.RetraceLow);
        Assert.IsNull(r501.PointType);
    }

    [TestMethod]
    public void Chainor()
    {
        IReadOnlyList<SmaResult> results = Quotes
            .ToZigZag(EndType.Close, 3)
            .ToSma(10);

        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(225, results.Count(x => x.Sma != null));
    }

    [TestMethod]
    public void NoEntry()
    {
        // thresholds are never met
        string json = File.ReadAllText("./s-z/ZigZag/data.ethusdt.json");

        IReadOnlyList<Quote> quotes = JsonConvert
            .DeserializeObject<IReadOnlyCollection<Quote>>(json)
            .ToList();

        IReadOnlyList<ZigZagResult> results = quotes
            .ToZigZag();

        Assert.AreEqual(0, results.Count(x => x.PointType != null));
    }

    [TestMethod]
    public void Issue632()
    {
        // thresholds are never met
        string json = File.ReadAllText("./s-z/ZigZag/data.issue632.json");

        IReadOnlyList<Quote> quotesList = JsonConvert
            .DeserializeObject<IReadOnlyCollection<Quote>>(json)
            .ToList();

        IReadOnlyList<ZigZagResult> resultsList = quotesList
            .ToZigZag();

        Assert.AreEqual(17, resultsList.Count);
    }

    [TestMethod]
    public override void BadData()
    {
        IReadOnlyList<ZigZagResult> r1 = BadQuotes
            .ToZigZag();

        Assert.AreEqual(502, r1.Count);

        IReadOnlyList<ZigZagResult> r2 = BadQuotes
            .ToZigZag(EndType.HighLow);

        Assert.AreEqual(502, r2.Count);
    }

    [TestMethod]
    public override void NoQuotes()
    {
        IReadOnlyList<ZigZagResult> r0 = Noquotes
            .ToZigZag();

        Assert.AreEqual(0, r0.Count);

        IReadOnlyList<ZigZagResult> r1 = Onequote
            .ToZigZag();

        Assert.AreEqual(1, r1.Count);
    }

    [TestMethod]
    public void Condense()
    {
        IReadOnlyList<ZigZagResult> results = Quotes
            .ToZigZag(EndType.Close, 3)
            .Condense();

        // assertions
        Assert.AreEqual(14, results.Count);
    }

    [TestMethod]
    public void SchrodingerScenario()
    {
        string json = File.ReadAllText("./s-z/ZigZag/data.schrodinger.json");

        IReadOnlyList<Quote> h = JsonConvert
            .DeserializeObject<IReadOnlyCollection<Quote>>(json)
            .OrderBy(x => x.Timestamp)
            .ToList();

        IReadOnlyList<ZigZagResult> r1 = h.ToZigZag(EndType.Close, 0.25m).ToList();
        Assert.AreEqual(342, r1.Count);

        // first period has High/Low that exceeds threhold
        // where it is both a H and L pivot simultaenously
        IReadOnlyList<ZigZagResult> r2 = h.ToZigZag(EndType.HighLow, 3).ToList();
        Assert.AreEqual(342, r2.Count);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()
            => Quotes.ToZigZag(EndType.Close, 0));

        // bad end type
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(()
            => Quotes.ToZigZag((EndType)int.MaxValue, 2));
    }
}
