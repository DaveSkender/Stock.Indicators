using System.Collections.ObjectModel;

namespace Tests.Common;

[TestClass]
public class QuoteUtilityTests : TestBase
{
    [TestMethod]
    public void QuoteToSortedCollection()
    {
        IEnumerable<Quote> quotes = TestData.GetMismatch();

        Collection<Quote> h = quotes.ToSortedCollection();

        // proper quantities
        Assert.AreEqual(502, h.Count);

        // check first date
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(firstDate, h[0].Timestamp);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, h.LastOrDefault().Timestamp);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(spotDate, h[50].Timestamp);
    }

    [TestMethod]
    public void QuoteToSortedList()
    {
        IEnumerable<Quote> quotes = TestData.GetMismatch();

        List<Quote> h = quotes.ToSortedList();

        // proper quantities
        Assert.AreEqual(502, h.Count);

        // check first date
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(firstDate, h[0].Timestamp);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, h.LastOrDefault().Timestamp);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(spotDate, h[50].Timestamp);
    }

    [TestMethod]
    public void QuoteToReusable()
    {
        DateTime d = DateTime.Parse("5/5/2055", EnglishCulture);

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

        Quote q = new() {
            Timestamp = d,
            Open = o,
            High = h,
            Low = l,
            Close = c,
            Volume = v
        };

        Assert.AreEqual(
            NullMath.Round((double)o, 10),
            NullMath.Round(q.ToReusable(CandlePart.Open).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)h, 10),
            NullMath.Round(q.ToReusable(CandlePart.High).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)l, 10),
            NullMath.Round(q.ToReusable(CandlePart.Low).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)c, 10),
            NullMath.Round(q.ToReusable(CandlePart.Close).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)v, 10),
            NullMath.Round(q.ToReusable(CandlePart.Volume).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)hl2, 10),
            NullMath.Round(q.ToReusable(CandlePart.HL2).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)hlc3, 10),
            NullMath.Round(q.ToReusable(CandlePart.HLC3).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)oc2, 10),
            NullMath.Round(q.ToReusable(CandlePart.OC2).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohl3, 10),
            NullMath.Round(q.ToReusable(CandlePart.OHL3).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohlc4, 10),
            NullMath.Round(q.ToReusable(CandlePart.OHLC4).Value, 10));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => q.ToReusable((CandlePart)999));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => q.ToReusable((CandlePart)999));
    }

    [TestMethod]
    public void ToReusableList()
    {
        var reusableList = quotes
            .ToReusableList(CandlePart.Close);

        Assert.IsNotNull(reusableList);
        Assert.AreEqual(502, reusableList.Count);
        Assert.AreEqual(reusableList.LastOrDefault().Value, 245.28d);
    }

    [TestMethod]
    public void QuoteDToReusable()
    {
        DateTime d = DateTime.Parse("5/5/2055", EnglishCulture);

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

        QuoteD q = new() {
            Timestamp = d,
            Open = o,
            High = h,
            Low = l,
            Close = c,
            Volume = v
        };

        Assert.AreEqual(
            NullMath.Round((double)o, 10),
            NullMath.Round(q.ToReusable(CandlePart.Open).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)h, 10),
            NullMath.Round(q.ToReusable(CandlePart.High).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)l, 10),
            NullMath.Round(q.ToReusable(CandlePart.Low).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)c, 10),
            NullMath.Round(q.ToReusable(CandlePart.Close).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)v, 10),
            NullMath.Round(q.ToReusable(CandlePart.Volume).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)hl2, 10),
            NullMath.Round(q.ToReusable(CandlePart.HL2).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)hlc3, 10),
            NullMath.Round(q.ToReusable(CandlePart.HLC3).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)oc2, 10),
            NullMath.Round(q.ToReusable(CandlePart.OC2).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohl3, 10),
            NullMath.Round(q.ToReusable(CandlePart.OHL3).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohlc4, 10),
            NullMath.Round(q.ToReusable(CandlePart.OHLC4).Value, 10));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => q.ToReusable((CandlePart)999));
    }
}
