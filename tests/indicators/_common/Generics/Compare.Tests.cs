using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Common;

[TestClass]
public class CompareTests : TestBase
{
    [TestMethod]
    public void IsEqualQuotes()
    {

        Quote q1 = new()
        {
            Date = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture),
            Open = 1m,
            High = 1m,
            Low = 1m,
            Close = 1m,
            Volume = 100
        };

        Quote q2 = new()
        {
            Date = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture),
            Open = 1m,
            High = 1m,
            Low = 1m,
            Close = 1m,
            Volume = 100
        };

        Quote q3 = new()
        {
            Date = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture),
            Open = 1m,
            High = 1m,
            Low = 1m,
            Close = 2m,
            Volume = 99
        };

        Assert.IsTrue(q1.IsEqual(q2));
        Assert.IsFalse(q1.IsEqual(q3));
    }

    [TestMethod]
    public void IsEqualReusableResults()
    {

        SmaResult q1 = new()
        {
            Date = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture),
            Sma = 1d
        };

        SmaResult q2 = new()
        {
            Date = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture),
            Sma = 1d
        };

        SmaResult q3 = new()
        {
            Date = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture),
            Sma = 2d
        };

        Assert.IsTrue(q1.IsEqual(q2));
        Assert.IsFalse(q1.IsEqual(q3));
    }
}
