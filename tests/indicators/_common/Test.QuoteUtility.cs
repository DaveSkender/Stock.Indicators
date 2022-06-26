using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class QuoteUtility : TestBase
{
    [TestMethod]
    public void QuoteToBasicTuple()
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
            NullMath.Round(q.ToBasicTuple(CandlePart.Open).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)h, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.High).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)l, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.Low).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)c, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.Close).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)v, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.Volume).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)hl2, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.HL2).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)hlc3, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.HLC3).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)oc2, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.OC2).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohl3, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.OHL3).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohlc4, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.OHLC4).Item2, 10));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            q.ToBasicTuple((CandlePart)999));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            q.ToBasicData((CandlePart)999));
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
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            q.ToBasicData((CandlePart)999));
    }

    [TestMethod]
    public void QuoteDToBasicTuple()
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
            NullMath.Round(q.ToBasicTuple(CandlePart.Open).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)h, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.High).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)l, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.Low).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)c, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.Close).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)v, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.Volume).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)hl2, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.HL2).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)hlc3, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.HLC3).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)oc2, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.OC2).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohl3, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.OHL3).Item2, 10));
        Assert.AreEqual(
            NullMath.Round((double)ohlc4, 10),
            NullMath.Round(q.ToBasicTuple(CandlePart.OHLC4).Item2, 10));

        // bad argument
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            q.ToBasicTuple((CandlePart)999));
    }
}
