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
        Random rand = new();

        decimal o = (decimal)rand.NextDouble();
        decimal h = (decimal)rand.NextDouble();
        decimal l = (decimal)rand.NextDouble();
        decimal c = (decimal)rand.NextDouble();
        decimal v = (decimal)rand.NextDouble();
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
            NullMath.Round((double)o, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.Open).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)h, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.High).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)l, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.Low).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)c, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.Close).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)v, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.Volume).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)hl2, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.HL2).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)hlc3, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.HLC3).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)oc2, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.OC2).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)ohl3, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.OHL3).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)ohlc4, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.OHLC4).Item2, 15));
    }

    [TestMethod]
    public void QuoteDToBasicTuple()
    {
        DateTime d = DateTime.Parse("5/5/2055", EnglishCulture);
        Random rand = new();

        double o = rand.NextDouble();
        double h = rand.NextDouble();
        double l = rand.NextDouble();
        double c = rand.NextDouble();
        double v = rand.NextDouble();
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
            NullMath.Round((double)o, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.Open).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)h, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.High).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)l, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.Low).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)c, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.Close).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)v, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.Volume).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)hl2, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.HL2).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)hlc3, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.HLC3).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)oc2, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.OC2).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)ohl3, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.OHL3).Item2, 15));
        Assert.AreEqual(
            NullMath.Round((double)ohlc4, 15),
            NullMath.Round(q.ToBasicTuple(CandlePart.OHLC4).Item2, 15));
    }
}
