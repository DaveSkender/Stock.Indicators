namespace Tests.Common;

[TestClass]
public class Reusable : TestBase
{
    [TestMethod]
    public void Condense()
    {
        List<AdxResult> results = Quotes
            .GetAdx()
            .ToList();

        // make a few more in the middle null and NaN
        results[249] = results[249] with { Value = double.NaN, Adx = null };
        results[345] = results[345] with { Value = double.NaN, Adx = double.NaN };

        List<AdxResult> r = results.Condense().ToList();

        // proper quantities
        Assert.AreEqual(473, r.Count);

        // sample values
        AdxResult last = r.LastOrDefault();
        Assert.AreEqual(17.7565, last.Pdi.Round(4));
        Assert.AreEqual(31.1510, last.Mdi.Round(4));
        Assert.AreEqual(34.2987, last.Adx.Round(4));
    }

    [TestMethod]
    public void ToReusableList()
    {
        var reusableList = Quotes
            .ToReusableList(CandlePart.Close);

        Assert.IsNotNull(reusableList);
        Assert.AreEqual(502, reusableList.Count);
        Assert.AreEqual(reusableList[^1].Value, 245.28d);
    }

    [TestMethod]
    public void QuoteToReusable()
    {
        DateTime t = DateTime.Parse("5/5/2055", englishCulture);

        decimal l = 111111111111111m;
        decimal o = 222222222222222m;
        decimal c = 333333333333333m;
        decimal h = 444444444444444m;
        decimal v = 555555555555555m;
        decimal hl2 = (h + l) / 2m;
        decimal hlc3 = (h + l + c) / 3m;
        decimal oc2 = (o + c) / 2m;
        decimal ohl3 = (o + h + l) / 3m;
        decimal ohlc4 = (o + h + l + c) / 4m;

        Quote q = new(t, o, h, l, c, v);

        Assert.AreEqual(
            ((double)o).Round(10),
            q.ToReusable(CandlePart.Open).Value.Round(10));
        Assert.AreEqual(
            ((double)h).Round(10),
            q.ToReusable(CandlePart.High).Value.Round(10));
        Assert.AreEqual(
            ((double)l).Round(10),
            q.ToReusable(CandlePart.Low).Value.Round(10));
        Assert.AreEqual(
            ((double)c).Round(10),
            q.ToReusable(CandlePart.Close).Value.Round(10));
        Assert.AreEqual(
            ((double)v).Round(10),
            q.ToReusable(CandlePart.Volume).Value.Round(10));
        Assert.AreEqual(
            ((double)hl2).Round(10),
            q.ToReusable(CandlePart.HL2).Value.Round(10));
        Assert.AreEqual(
            ((double)hlc3).Round(10),
            q.ToReusable(CandlePart.HLC3).Value.Round(10));
        Assert.AreEqual(
            ((double)oc2).Round(10),
            q.ToReusable(CandlePart.OC2).Value.Round(10));
        Assert.AreEqual(
            ((double)ohl3).Round(10),
            q.ToReusable(CandlePart.OHL3).Value.Round(10));
        Assert.AreEqual(
            ((double)ohlc4).Round(10),
            q.ToReusable(CandlePart.OHLC4).Value.Round(10));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => q.ToReusable((CandlePart)999));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => q.ToReusable((CandlePart)999));
    }

    [TestMethod]
    public void QuoteDToReusable()
    {
        DateTime t = DateTime.Parse("5/5/2055", englishCulture);

        double l = 111111111111111;
        double o = 222222222222222;
        double c = 333333333333333;
        double h = 444444444444444;
        double v = 555555555555555;
        double hl2 = (h + l) / 2;
        double hlc3 = (h + l + c) / 3;
        double oc2 = (o + c) / 2;
        double ohl3 = (o + h + l) / 3;
        double ohlc4 = (o + h + l + c) / 4;

        QuoteD q = new(t, o, h, l, c, v);

        Assert.AreEqual(
            o.Round(10),
            q.ToReusable(CandlePart.Open).Value.Round(10));
        Assert.AreEqual(
            h.Round(10),
            q.ToReusable(CandlePart.High).Value.Round(10));
        Assert.AreEqual(
            l.Round(10),
            q.ToReusable(CandlePart.Low).Value.Round(10));
        Assert.AreEqual(
            c.Round(10),
            q.ToReusable(CandlePart.Close).Value.Round(10));
        Assert.AreEqual(
            v.Round(10),
            q.ToReusable(CandlePart.Volume).Value.Round(10));
        Assert.AreEqual(
            hl2.Round(10),
            q.ToReusable(CandlePart.HL2).Value.Round(10));
        Assert.AreEqual(
            hlc3.Round(10),
            q.ToReusable(CandlePart.HLC3).Value.Round(10));
        Assert.AreEqual(
            oc2.Round(10),
            q.ToReusable(CandlePart.OC2).Value.Round(10));
        Assert.AreEqual(
            ohl3.Round(10),
            q.ToReusable(CandlePart.OHL3).Value.Round(10));
        Assert.AreEqual(
            ohlc4.Round(10),
            q.ToReusable(CandlePart.OHLC4).Value.Round(10));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => q.ToReusable((CandlePart)999));
    }
}
