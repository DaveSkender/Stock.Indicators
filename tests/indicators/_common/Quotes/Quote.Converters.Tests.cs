using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Common;

[TestClass]
public class QuoteUtility : TestBase
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
        Assert.AreEqual(firstDate, h[0].Date);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, h.LastOrDefault().Date);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(spotDate, h[50].Date);
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
        Assert.AreEqual(firstDate, h[0].Date);

        // check last date
        DateTime lastDate = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(lastDate, h.LastOrDefault().Date);

        // spot check an out of sequence date
        DateTime spotDate = DateTime.ParseExact("03/16/2017", "MM/dd/yyyy", EnglishCulture);
        Assert.AreEqual(spotDate, h[50].Date);
    }

    [TestMethod]
    public void QuoteToTuple()
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

        Quote q = new()
        {
            Date = d,
            Open = o,
            High = h,
            Low = l,
            Close = c,
            Volume = v
        };

        Assert.AreEqual(
            NullMath.Round((double)o, 10),
            NullMath.Round(q.ToTuple(CandlePart.Open).value, 10));
        Assert.AreEqual(
            NullMath.Round((double)h, 10),
            NullMath.Round(q.ToTuple(CandlePart.High).value, 10));
        Assert.AreEqual(
            NullMath.Round((double)l, 10),
            NullMath.Round(q.ToTuple(CandlePart.Low).value, 10));
        Assert.AreEqual(
            NullMath.Round((double)c, 10),
            NullMath.Round(q.ToTuple(CandlePart.Close).value, 10));
        Assert.AreEqual(
            NullMath.Round((double)v, 10),
            NullMath.Round(q.ToTuple(CandlePart.Volume).value, 10));
        Assert.AreEqual(
            NullMath.Round((double)hl2, 10),
            NullMath.Round(q.ToTuple(CandlePart.HL2).value, 10));
        Assert.AreEqual(
            NullMath.Round((double)hlc3, 10),
            NullMath.Round(q.ToTuple(CandlePart.HLC3).value, 10));
        Assert.AreEqual(
            NullMath.Round((double)oc2, 10),
            NullMath.Round(q.ToTuple(CandlePart.OC2).value, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohl3, 10),
            NullMath.Round(q.ToTuple(CandlePart.OHL3).value, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohlc4, 10),
            NullMath.Round(q.ToTuple(CandlePart.OHLC4).value, 10));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => q.ToTuple((CandlePart)999));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => q.ToBasicData((CandlePart)999));
    }

    [TestMethod]
    public void ToTupleCollection()
    {
        Collection<(DateTime, double)> collection = quotes
            .OrderBy(x => x.Date)
            .ToTupleCollection(CandlePart.Close);

        Assert.IsNotNull(collection);
        Assert.AreEqual(502, collection.Count);
        Assert.AreEqual(collection.LastOrDefault().Item2, 245.28d);
    }

    [TestMethod]
    public void ToSortedList()
    {
        Collection<(DateTime, double)> collection = quotes
            .OrderBy(x => x.Date)
            .ToTuple(CandlePart.Close)
            .ToSortedCollection();

        Assert.IsNotNull(collection);
        Assert.AreEqual(502, collection.Count);
        Assert.AreEqual(collection.LastOrDefault().Item2, 245.28d);
    }

    [TestMethod]
    public void QuoteToBasicData()
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

        Quote q = new()
        {
            Date = d,
            Open = o,
            High = h,
            Low = l,
            Close = c,
            Volume = v
        };

        Assert.AreEqual(
            NullMath.Round((double)o, 10),
            NullMath.Round(q.ToBasicData(CandlePart.Open).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)h, 10),
            NullMath.Round(q.ToBasicData(CandlePart.High).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)l, 10),
            NullMath.Round(q.ToBasicData(CandlePart.Low).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)c, 10),
            NullMath.Round(q.ToBasicData(CandlePart.Close).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)v, 10),
            NullMath.Round(q.ToBasicData(CandlePart.Volume).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)hl2, 10),
            NullMath.Round(q.ToBasicData(CandlePart.HL2).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)hlc3, 10),
            NullMath.Round(q.ToBasicData(CandlePart.HLC3).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)oc2, 10),
            NullMath.Round(q.ToBasicData(CandlePart.OC2).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohl3, 10),
            NullMath.Round(q.ToBasicData(CandlePart.OHL3).Value, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohlc4, 10),
            NullMath.Round(q.ToBasicData(CandlePart.OHLC4).Value, 10));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => q.ToBasicData((CandlePart)999));
    }

    [TestMethod]
    public void QuoteDToTuple()
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

        QuoteD q = new()
        {
            Date = d,
            Open = o,
            High = h,
            Low = l,
            Close = c,
            Volume = v
        };

        Assert.AreEqual(
            NullMath.Round((double)o, 10),
            NullMath.Round(q.ToTuple(CandlePart.Open).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)h, 10),
            NullMath.Round(q.ToTuple(CandlePart.High).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)l, 10),
            NullMath.Round(q.ToTuple(CandlePart.Low).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)c, 10),
            NullMath.Round(q.ToTuple(CandlePart.Close).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)v, 10),
            NullMath.Round(q.ToTuple(CandlePart.Volume).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)hl2, 10),
            NullMath.Round(q.ToTuple(CandlePart.HL2).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)hlc3, 10),
            NullMath.Round(q.ToTuple(CandlePart.HLC3).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)oc2, 10),
            NullMath.Round(q.ToTuple(CandlePart.OC2).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohl3, 10),
            NullMath.Round(q.ToTuple(CandlePart.OHL3).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohlc4, 10),
            NullMath.Round(q.ToTuple(CandlePart.OHLC4).Item2, 10));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(()
            => q.ToTuple((CandlePart)999));
    }
}
