using Newtonsoft.Json;

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
        Assert.HasCount(502, results);
        Assert.HasCount(234, results.Where(static x => x.ZigZag != null));
        Assert.HasCount(234, results.Where(static x => x.RetraceHigh != null));
        Assert.HasCount(221, results.Where(static x => x.RetraceLow != null));
        Assert.HasCount(14, results.Where(static x => x.PointType != null));

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
        List<ZigZagResult> results =
            quotes.GetZigZag(EndType.HighLow, 3)
            .ToList();

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(463, results.Where(static x => x.ZigZag != null));
        Assert.HasCount(463, results.Where(static x => x.RetraceHigh != null));
        Assert.HasCount(442, results.Where(static x => x.RetraceLow != null));
        Assert.HasCount(30, results.Where(static x => x.PointType != null));

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
        List<SmaResult> results = quotes
            .GetZigZag(EndType.Close, 3)
            .GetSma(10)
            .ToList();

        Assert.HasCount(502, results);
        Assert.HasCount(225, results.Where(static x => x.Sma != null));
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

        Assert.IsEmpty(results.Where(static x => x.PointType != null));
    }

    [TestMethod]
    public void Issue0632()
    {
        // thresholds are never met
        string json = File.ReadAllText("./s-z/ZigZag/data.issue0632.json");

        List<Quote> quotes = JsonConvert
            .DeserializeObject<IReadOnlyCollection<Quote>>(json)
            .ToList();

        List<ZigZagResult> resultsList = quotes
            .GetZigZag(EndType.Close, 5m)
            .ToList();

        Assert.HasCount(17, resultsList);
    }

    [TestMethod]
    public void Issue1949()
    {
        IOrderedEnumerable<Quote> quotes = File.ReadAllLines("s-z/ZigZag/data.issue1949.csv")
            .Skip(1)
            .Select(Importer.QuoteFromCsv)
            .OrderByDescending(static x => x.Date);

        List<ZigZagResult> r = quotes
            .GetZigZag(endType: EndType.HighLow, percentChange: 5.0m)
            .ToList();

        string msg = "results size should match original quotes size";

        r.Should().HaveCount(1430, msg);
        r.Should().HaveCount(quotes.Count(), msg);
        r.Where(static x => x.ZigZag is not null).Should().HaveCount(726);
        r.Where(static x => x.PointType is not null).Should().HaveCount(1);
        r[704].ZigZag.Should().Be(75540.8m);
        r[704].PointType.Should().Be("L");
    }

    [TestMethod]
    public void BadData()
    {
        List<ZigZagResult> r1 = badQuotes
            .GetZigZag(EndType.Close)
            .ToList();

        Assert.HasCount(502, r1);

        List<ZigZagResult> r2 = badQuotes
            .GetZigZag(EndType.HighLow)
            .ToList();

        Assert.HasCount(502, r2);
    }

    [TestMethod]
    public void NoQuotes()
    {
        List<ZigZagResult> r0 = noquotes
            .GetZigZag()
            .ToList();

        Assert.IsEmpty(r0);

        List<ZigZagResult> r1 = onequote
            .GetZigZag()
            .ToList();

        Assert.HasCount(1, r1);
    }

    [TestMethod]
    public void Condense()
    {
        List<ZigZagResult> results = quotes
            .GetZigZag(EndType.Close, 3)
            .Condense()
            .ToList();

        // assertions
        Assert.HasCount(14, results);
    }

    [TestMethod]
    public void SchrodingerScenario()
    {
        string json = File.ReadAllText("./s-z/ZigZag/data.schrodinger.json");

        List<Quote> h = JsonConvert
            .DeserializeObject<IReadOnlyCollection<Quote>>(json)
            .OrderBy(static x => x.Date)
            .ToList();

        List<ZigZagResult> r1 = h.GetZigZag(EndType.Close, 0.25m).ToList();
        Assert.HasCount(342, r1);

        // first period has High/Low that exceeds threhold
        // where it is both a H and L pivot simultaenously
        List<ZigZagResult> r2 = h.GetZigZag(EndType.HighLow, 3).ToList();
        Assert.HasCount(342, r2);
    }

    [TestMethod]
    public void Exceptions()
    {
        // bad lookback period
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetZigZag(EndType.Close, 0));

        // bad end type
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            static () => quotes.GetZigZag((EndType)int.MaxValue, 2));
    }
}
