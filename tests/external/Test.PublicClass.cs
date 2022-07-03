using System.Globalization;
using Internal.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

[assembly: CLSCompliant(true)]
namespace External.Other;

internal class MyQuote : Quote
{
    public bool MyProperty { get; set; }
    public decimal? MyClose { get; set; }
}

internal class MyEma : ResultBase
{
    public int Id { get; set; }
    public bool MyProperty { get; set; }
    public double? Ema { get; set; }
}

internal class MyGenericQuote : IQuote
{
    // required base properties
    DateTime IQuote.Date => CloseDate;
    public decimal Open { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    decimal IQuote.Close => CloseValue;
    public decimal Volume { get; set; }

    // custom properties
    public int MyOtherProperty { get; set; }
    public DateTime CloseDate { get; set; }
    public decimal CloseValue { get; set; }
}

[TestClass]
public class PublicClassTests
{
    internal static readonly CultureInfo EnglishCulture = new("en-US", false);

    [TestMethod]
    public void ValidateHistory()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        quotes.Validate();

        quotes.GetSma(6);
        Indicator.GetSma(quotes, 5);
    }

    [TestMethod]
    public void ReadQuoteClass()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<Quote> h = quotes.Validate();

        Quote f = h.FirstOrDefault();
        Console.WriteLine("Date:{0},Close:{1}", f.Date, f.Close);
    }

    [TestMethod]
    public void DerivedQuoteClass()
    {
        // can use a derive Quote class
        MyQuote myQuote = new()
        {
            Date = DateTime.Now,
            MyProperty = true
        };

        Assert.AreEqual(true, myQuote.MyProperty);
    }

    [TestMethod]
    public void DerivedQuoteClassLinq()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        quotes = quotes.Validate();

        // can use a derive Quote class using Linq

        IEnumerable<MyQuote> myHistory = quotes
            .Select(x => new MyQuote
            {
                Date = x.Date,
                MyClose = x.Close,
                MyProperty = false
            });

        Assert.IsTrue(myHistory.Any());
    }

    [TestMethod]
    public void CustomQuoteClass()
    {
        List<MyGenericQuote> myGenericHistory = TestData.GetDefault()
            .Select(x => new MyGenericQuote
            {
                CloseDate = x.Date,
                Open = x.Open,
                High = x.High,
                Low = x.Low,
                CloseValue = x.Close,
                Volume = x.Volume,
                MyOtherProperty = 123456
            })
            .ToList();

        List<EmaResult> results = myGenericHistory.GetEma(20)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(502, results.Count);
        Assert.AreEqual(483, results.Count(x => x.Ema != null));

        // sample values
        EmaResult r1 = results[501];
        Assert.AreEqual(249.3519m, Math.Round((decimal)r1.Ema, 4));

        EmaResult r2 = results[249];
        Assert.AreEqual(255.3873m, Math.Round((decimal)r2.Ema, 4));

        EmaResult r3 = results[29];
        Assert.AreEqual(216.6228m, Math.Round((decimal)r3.Ema, 4));
    }

    [TestMethod]
    public void CustomQuoteAggregate()
    {
        List<MyGenericQuote> myGenericHistory = TestData.GetIntraday()
            .Select(x => new MyGenericQuote
            {
                CloseDate = x.Date,
                Open = x.Open,
                High = x.High,
                Low = x.Low,
                CloseValue = x.Close,
                Volume = x.Volume,
                MyOtherProperty = 123456
            })
            .ToList();

        List<Quote> quotesList = myGenericHistory
            .Aggregate(PeriodSize.TwoHours)
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(20, quotesList.Count);

        // sample values
        Quote r19 = quotesList[19];
        Assert.AreEqual(369.04m, r19.Low);
    }

    [TestMethod]
    public void CustomQuoteAggregateTimeSpan()
    {
        List<MyGenericQuote> myGenericHistory = TestData.GetIntraday()
            .Select(x => new MyGenericQuote
            {
                CloseDate = x.Date,
                Open = x.Open,
                High = x.High,
                Low = x.Low,
                CloseValue = x.Close,
                Volume = x.Volume,
                MyOtherProperty = 123456
            })
            .ToList();

        List<Quote> quotesList = myGenericHistory
            .Aggregate(TimeSpan.FromHours(2))
            .ToList();

        // assertions

        // proper quantities
        // should always be the same number of results as there is quotes
        Assert.AreEqual(20, quotesList.Count);

        // sample values
        Quote r19 = quotesList[19];
        Assert.AreEqual(369.04m, r19.Low);
    }

    [TestMethod]
    public void CustomIndicatorClass()
    {
        // can use a derive Indicator class
        MyEma myIndicator = new()
        {
            Date = DateTime.Now,
            Ema = 123.456,
            MyProperty = false
        };

        Assert.AreEqual(false, myIndicator.MyProperty);
    }

    [TestMethod]
    public void CustomIndicatorClassLinq()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<EmaResult> emaResults = quotes.GetEma(14);

        // can use a derive Indicator class using Linq

        IEnumerable<MyEma> myIndicatorResults = emaResults
            .Where(x => x.Ema != null)
            .Select(x => new MyEma
            {
                Date = x.Date,
                Ema = x.Ema,
                MyProperty = false
            });

        Assert.IsTrue(myIndicatorResults.Any());
    }

    [TestMethod]
    public void CustomIndicatorClassFind()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<EmaResult> emaResults = Indicator.GetEma(quotes, 20);

        // can use a derive Indicator class using Linq

        IEnumerable<MyEma> myIndicatorResults = emaResults
            .Where(x => x.Ema != null)
            .Select(x => new MyEma
            {
                Id = 12345,
                Date = x.Date,
                Ema = x.Ema,
                MyProperty = false
            });

        Assert.IsTrue(myIndicatorResults.Any());

        // find specific date
        DateTime findDate = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", EnglishCulture);

        MyEma i = myIndicatorResults.Find(findDate);
        Assert.AreEqual(12345, i.Id);

        EmaResult r = emaResults.Find(findDate);
        Assert.AreEqual(249.3519m, Math.Round((decimal)r.Ema, 4));
    }
}
