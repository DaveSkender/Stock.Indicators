using System.Globalization;
using Sut;

namespace Customization;

// CUSTOM QUOTES

[TestClass, TestCategory("Integration")]
public class CustomQuotes
{
    private static readonly CultureInfo EnglishCulture
    = new("en-US", false);

    private static readonly DateTime EvalDate
        = DateTime.ParseExact(
            "12/31/2018", "MM/dd/yyyy", EnglishCulture);

    private static readonly IReadOnlyList<Quote> quotes = Data.GetDefault();
    private static readonly IReadOnlyList<Quote> intraday = Data.GetIntraday();

    [TestMethod]
    public void CustomQuoteSeries()
    {
        List<CustomQuote> myGenericHistory = quotes
            .Select(static x => new CustomQuote {
                CloseDate = x.Timestamp,
                Open = x.Open,
                High = x.High,
                Low = x.Low,
                CloseValue = x.Close,
                Volume = x.Volume,
                MyOtherProperty = 123456
            })
            .ToList();

        IReadOnlyList<EmaResult> results = myGenericHistory.ToEma(20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Ema != null));

        // sample values
        EmaResult r1 = results[501];
        Assert.AreEqual(249.3519m, Math.Round((decimal)r1.Ema, 4));

        EmaResult r2 = results[249];
        Assert.AreEqual(255.3873m, Math.Round((decimal)r2.Ema, 4));

        EmaResult r3 = results[29];
        Assert.AreEqual(216.6228m, Math.Round((decimal)r3.Ema, 4));
    }

    [TestMethod]
    public void CustomQuoteEquality()
    {
        CustomQuote q1 = new() {
            CloseDate = EvalDate,
            Open = 1m,
            High = 1m,
            Low = 1m,
            CloseValue = 1m,
            Volume = 100
        };

        CustomQuote q2 = new() {
            CloseDate = EvalDate,
            Open = 1m,
            High = 1m,
            Low = 1m,
            CloseValue = 1m,
            Volume = 100
        };

        CustomQuote q3 = new() {
            CloseDate = EvalDate,
            Open = 1m,
            High = 1m,
            Low = 1m,
            CloseValue = 2m,
            Volume = 99
        };

        Assert.IsTrue(Equals(q1, q2));
        Assert.IsFalse(Equals(q1, q3));

        Assert.IsTrue(q1.Equals(q2));
        Assert.IsFalse(q1.Equals(q3));

        Assert.IsTrue(q1 == q2, "== operator");
        Assert.IsFalse(q1 == q3, "== operator");

        Assert.IsFalse(q1 != q2, "!= operator");
        Assert.IsTrue(q1 != q3, "!= operator");
    }

    [TestMethod]
    public void CustomQuoteAggregate()
    {
        List<CustomQuote> myGenericHistory = intraday
            .Select(static x => new CustomQuote {
                CloseDate = x.Timestamp,
                Open = x.Open,
                High = x.High,
                Low = x.Low,
                CloseValue = x.Close,
                Volume = x.Volume,
                MyOtherProperty = 123456
            })
            .ToList();

        IReadOnlyList<Quote> quotesList = myGenericHistory
            .Aggregate(PeriodSize.TwoHours);

        // proper quantities
        Assert.HasCount(20, quotesList);

        // sample values
        Quote r19 = quotesList[19];
        Assert.AreEqual(369.04m, r19.Low);
    }

    [TestMethod]
    public void CustomQuoteAggregateTimeSpan()
    {
        List<CustomQuote> myGenericHistory = intraday
            .Select(static x => new CustomQuote {
                CloseDate = x.Timestamp,
                Open = x.Open,
                High = x.High,
                Low = x.Low,
                CloseValue = x.Close,
                Volume = x.Volume,
                MyOtherProperty = 123456
            })
            .ToList();

        IReadOnlyList<Quote> quotesList = myGenericHistory
            .Aggregate(TimeSpan.FromHours(2));

        // proper quantities
        Assert.HasCount(20, quotesList);

        // sample values
        Quote r19 = quotesList[19];
        Assert.AreEqual(369.04m, r19.Low);
    }

    [TestMethod]
    public void CustomQuoteInheritedSeries()
    {
        List<CustomQuoteInherited> myGenericHistory = quotes
            .Select(static x => new CustomQuoteInherited(
                CloseDate: x.Timestamp,
                Open: x.Open,
                High: x.High,
                Low: x.Low,
                Close: x.Close,
                Volume: x.Volume,
                MyOtherProperty: 123456))
            .ToList();

        IReadOnlyList<EmaResult> results = myGenericHistory.ToEma(20);

        // proper quantities
        Assert.HasCount(502, results);
        Assert.HasCount(483, results.Where(static x => x.Ema != null));

        // sample values
        EmaResult r1 = results[501];
        Assert.AreEqual(249.3519m, Math.Round((decimal)r1.Ema, 4));

        EmaResult r2 = results[249];
        Assert.AreEqual(255.3873m, Math.Round((decimal)r2.Ema, 4));

        EmaResult r3 = results[29];
        Assert.AreEqual(216.6228m, Math.Round((decimal)r3.Ema, 4));
    }
}
