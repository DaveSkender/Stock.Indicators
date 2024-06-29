using System.Collections.ObjectModel;

namespace Tests.Common;
// ReSharper disable All

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
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(firstDate, h[0].Timestamp);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(lastDate, h.LastOrDefault().Timestamp);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", englishCulture);
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
        DateTime firstDate = DateTime.ParseExact("01/18/2016", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(firstDate, h[0].Timestamp);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(lastDate, h.LastOrDefault().Timestamp);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", englishCulture);
        Assert.AreEqual(spotDate, h[50].Timestamp);
    }

    [TestMethod]
    public void QuoteToReusable()
    {
        DateTime d = DateTime.Parse("5/5/2055", englishCulture);

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
            q.ToReusable(CandlePart.Hl2).Value.Round(10));
        Assert.AreEqual(
            ((double)hlc3).Round(10),
            q.ToReusable(CandlePart.Hlc3).Value.Round(10));
        Assert.AreEqual(
            ((double)oc2).Round(10),
            q.ToReusable(CandlePart.Oc2).Value.Round(10));
        Assert.AreEqual(
            ((double)ohl3).Round(10),
            q.ToReusable(CandlePart.Ohl3).Value.Round(10));
        Assert.AreEqual(
            ((double)ohlc4).Round(10),
            q.ToReusable(CandlePart.Ohlc4).Value.Round(10));

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
        IReadOnlyList<Reusable> reusableList = Quotes
            .ToReusableList(CandlePart.Close);

        Assert.IsNotNull(reusableList);
        Assert.AreEqual(502, reusableList.Count);
        Assert.AreEqual(reusableList[^1].Value, 245.28d);
    }

    [TestMethod]
    public void QuoteDToReusable()
    {
        DateTime d = DateTime.Parse("5/5/2055", englishCulture);

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
            q.ToReusable(CandlePart.Hl2).Value.Round(10));
        Assert.AreEqual(
            hlc3.Round(10),
            q.ToReusable(CandlePart.Hlc3).Value.Round(10));
        Assert.AreEqual(
            oc2.Round(10),
            q.ToReusable(CandlePart.Oc2).Value.Round(10));
        Assert.AreEqual(
            ohl3.Round(10),
            q.ToReusable(CandlePart.Ohl3).Value.Round(10));
        Assert.AreEqual(
            ohlc4.Round(10),
            q.ToReusable(CandlePart.Ohlc4).Value.Round(10));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => q.ToReusable((CandlePart)999));
    }
}
