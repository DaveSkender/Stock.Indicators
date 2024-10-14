namespace Utilities;

[TestClass]
public class Reusable : TestBase
{
    [TestMethod]
    public void Condense()
    {
        List<AdxResult> original = Quotes
            .ToAdx()
            .ToList();

        // make a few more in the middle null and NaN
        original[249] = original[249] with { Adx = null };
        original[345] = original[345] with { Adx = double.NaN };

        IReadOnlyList<AdxResult> results
            = original.Condense();

        // proper quantities
        Assert.AreEqual(473, results.Count);

        // sample values
        AdxResult last = results[^1];
        Assert.AreEqual(17.7565, last.Pdi.Round(4));
        Assert.AreEqual(31.1510, last.Mdi.Round(4));
        Assert.AreEqual(34.2987, last.Adx.Round(4));
    }

    [TestMethod]
    public void ToReusableList()
    {
        IReadOnlyList<IReusable> reusableList = Quotes
            .ToReusableList(CandlePart.Close);

        Assert.IsNotNull(reusableList);
        Assert.AreEqual(502, reusableList.Count);
        Assert.AreEqual(245.28d, reusableList[^1].Value);
    }

    [TestMethod]
    public void QuoteToReusable()
    {
        DateTime t = DateTime.Parse("5/5/2055", invariantCulture);

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
}
