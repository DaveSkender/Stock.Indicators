using System.Globalization;

[assembly: CLSCompliant(true)]
namespace Tests.PublicApi;
// ReSharper disable All

internal sealed class MyEma : IResult
{
    public DateTime Timestamp { get; init; }
    public int Id { get; init; }
    public bool MyProperty { get; init; }
    public double? Ema { get; set; }
}

internal record struct MyCustomQuote : IQuote
{
    // override, redirect required properties
    readonly DateTime ISeries.Timestamp
        => CloseDate;

    readonly decimal IQuote.Close
        => CloseValue;

    // custom properties
    public int MyOtherProperty { get; set; }
    public DateTime CloseDate { get; init; }
    public decimal CloseValue { get; init; }

    // required base properties
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Volume { get; init; }

    readonly double IReusable.Value
        => (double)CloseValue;
}

[TestClass]
public class PublicClassTests
{
    private static readonly CultureInfo EnglishCulture
        = new("en-US", false);

    private static readonly DateTime EvalDate
        = DateTime.ParseExact(
            "12/31/2018", "MM/dd/yyyy", EnglishCulture);

    [TestMethod]
    public void ValidateHistory()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();

        IEnumerable<Quote> enumerable = quotes.ToList();

        enumerable.Validate();
        enumerable.GetSma(6);
        enumerable.GetEma(5);
    }

    [TestMethod]
    public void ReadQuoteClass()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<Quote> h = quotes.Validate();

        Quote f = h.FirstOrDefault();
        Console.WriteLine($"Quote:{f}");
    }

    [TestMethod]
    public void CustomQuoteSeries()
    {
        List<MyCustomQuote> myGenericHistory = TestData
            .GetDefault()
            .Select(x => new MyCustomQuote {
                CloseDate = x.Timestamp,
                Open = x.Open,
                High = x.High,
                Low = x.Low,
                CloseValue = x.Close,
                Volume = x.Volume,
                MyOtherProperty = 123456
            })
            .ToList();

        List<EmaResult> results = myGenericHistory
            .GetEma(20)
            .ToList();

        // proper quantities
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
    public void EqualCustomQuotes()
    {
        MyCustomQuote q1 = new() {
            CloseDate = EvalDate,
            Open = 1m,
            High = 1m,
            Low = 1m,
            CloseValue = 1m,
            Volume = 100
        };

        MyCustomQuote q2 = new() {
            CloseDate = EvalDate,
            Open = 1m,
            High = 1m,
            Low = 1m,
            CloseValue = 1m,
            Volume = 100
        };

        MyCustomQuote q3 = new() {
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

        //Assert.IsTrue(q1 == q2, "== operator");
        //Assert.IsFalse(q1 == q3, "== operator");

        //Assert.IsFalse(q1 != q2, "!= operator");
        //Assert.IsTrue(q1 != q3, "!= operator");
    }

    [TestMethod]
    public void CustomQuoteAggregate()
    {
        List<MyCustomQuote> myGenericHistory = TestData.GetIntraday()
            .Select(x => new MyCustomQuote {
                CloseDate = x.Timestamp,
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

        // proper quantities
        Assert.AreEqual(20, quotesList.Count);

        // sample values
        Quote r19 = quotesList[19];
        Assert.AreEqual(369.04m, r19.Low);
    }

    [TestMethod]
    public void CustomQuoteAggregateTimeSpan()
    {
        List<MyCustomQuote> myGenericHistory = TestData.GetIntraday()
            .Select(x => new MyCustomQuote {
                CloseDate = x.Timestamp,
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

        // proper quantities
        Assert.AreEqual(20, quotesList.Count);

        // sample values
        Quote r19 = quotesList[19];
        Assert.AreEqual(369.04m, r19.Low);
    }

    [TestMethod]
    public void CustomResultClass()
    {
        // can use a derive Indicator class
        MyEma myIndicator = new() {
            Timestamp = DateTime.Now,
            Ema = 123.456,
            MyProperty = false
        };

        Assert.AreEqual(false, myIndicator.MyProperty);
    }

    [TestMethod]
    public void CustomResultClassLinq()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<EmaResult> emaResults = quotes.GetEma(14);

        // can use a derive Indicator class using Linq

        IEnumerable<MyEma> myIndicatorResults = emaResults
            .Where(x => x.Ema != null)
            .Select(x => new MyEma {
                Timestamp = x.Timestamp,
                Ema = x.Ema,
                MyProperty = false
            });

        Assert.IsTrue(myIndicatorResults.Any());
    }

    [TestMethod]
    public void CustomResultClassFind()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();

        List<EmaResult> emaResults
            = quotes.GetEma(20).ToList();

        // can use a derive Indicator class using Linq

        List<MyEma> myIndicatorResults = emaResults
            .Where(x => x.Ema != null)
            .Select(x => new MyEma {
                Id = 12345,
                Timestamp = x.Timestamp,
                Ema = x.Ema,
                MyProperty = false
            })
            .ToList();

        Assert.IsTrue(myIndicatorResults.Count > 0);

        // find specific date
        DateTime findDate = DateTime.ParseExact(
            "2018-12-31", "yyyy-MM-dd", EnglishCulture);

        MyEma i = myIndicatorResults.Find(x => x.Timestamp == findDate);
        Assert.AreEqual(12345, i.Id);

        EmaResult r = emaResults.Find(x => x.Timestamp == findDate);
        Assert.AreEqual(249.3519m, Math.Round((decimal)r.Ema, 4));
    }


    [TestMethod]
    public void StreamAll() // from quote provider
    {
        /******************************************************
         * Attaches all stream observers to one Quote provider
         * for a full sprectrum stream collective.
         *
         * Currently, it does not include any [direct] chains;
         * however, under the hood many of these are streaming
         * through an underlying Use<TQuote> converter.
         *
         * This test covers most of the unusual test cases, like:
         *
         *  - out of order quotes (late arrivals)
         *  - duplicates, but not to an overflow situation
         *
         *  TODO: add all indicators to test, when available
         *
         ******************************************************/

        // source quotes (out of order, messy use case)
        List<Quote> quotesList = TestData.GetDefault().ToList();

        // setup quote provider
        QuoteHub<Quote> provider = new();

        // initialize observers, get static results for comparison (later)
        EmaHub<Quote> observeEma = provider.ToEma(20);
        List<EmaResult> staticEma = quotesList.GetEma(20).ToList();

        SmaHub<Quote> observeSma = provider.ToSma(20);
        List<SmaResult> staticSma = quotesList.GetSma(20).ToList();

        // emulate adding quotes to provider
        for (int i = 0; i < quotesList.Count; i++)
        {
            // skip one (add later)
            if (i == 80)
            {
                continue;
            }

            Quote q = quotesList[i];
            provider.Add(q);

            // resend duplicate quotes
            if (i is > 100 and < 105)
            {
                provider.Add(q);
            }
        }

        // late arrival
        provider.Add(quotesList[80]);

        // end all observations
        provider.EndTransmission();

        // final results should persist in scope
        List<EmaResult> streamEma = observeEma.Results.ToList();
        List<SmaResult> streamSma = observeSma.Results.ToList();

        // assert, should equal static series
        for (int i = 0; i < quotesList.Count; i++)
        {
            EmaResult sEma = staticEma[i];
            EmaResult rEma = streamEma[i];

            Assert.AreEqual(sEma.Timestamp, rEma.Timestamp);
            Assert.AreEqual(sEma.Ema, rEma.Ema);

            SmaResult sSma = staticSma[i];
            SmaResult rSma = streamSma[i];

            Assert.AreEqual(sSma.Timestamp, rSma.Timestamp);
            Assert.AreEqual(sSma.Sma, rSma.Sma);
        }
    }
}
