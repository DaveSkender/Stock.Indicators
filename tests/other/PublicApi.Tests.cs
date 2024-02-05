using System.Globalization;

[assembly: CLSCompliant(true)]
namespace Tests.PublicApi;

internal sealed record MyExtendedQuote : Quote
{
    public bool MyProperty { get; set; }
    public decimal? MyClose { get; set; }
}

internal sealed class MyEma : IResult
{
    public DateTime TickDate { get; set; }
    public int Id { get; set; }
    public bool MyProperty { get; set; }
    public double? Ema { get; set; }
}

internal sealed class MyCustomQuote
    : EquatableQuote, IQuote
{
    // override, redirect required properties
    public override DateTime TickDate => CloseDate;
    public override decimal Close => CloseValue;

    // custom properties
    public int MyOtherProperty { get; set; }
    public DateTime CloseDate { get; set; }
    public decimal CloseValue { get; set; }
}

[TestClass]
public class PublicClassTests
{
    internal static readonly CultureInfo EnglishCulture = new("en-US", false);
    internal static readonly DateTime evalDate
        = DateTime.ParseExact("12/31/2018", "MM/dd/yyyy", EnglishCulture);

    [TestMethod]
    public void ValidateHistory()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();

        quotes.Validate();
        quotes.GetSma(6);
        quotes.GetEma(5);
    }

    [TestMethod]
    public void ReadQuoteClass()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<Quote> h = quotes.Validate();

        Quote f = h.FirstOrDefault();
        Console.WriteLine($"Date:{f.TickDate},Close:{f.Close}");
    }

    [TestMethod]
    public void DerivedQuoteClass()
    {
        // can use a derive Quote class
        MyExtendedQuote myQuote = new()
        {
            TickDate = DateTime.Now,
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

        IEnumerable<MyExtendedQuote> myHistory = quotes
            .Select(x => new MyExtendedQuote
            {
                TickDate = x.TickDate,
                MyClose = x.Close,
                MyProperty = false
            });

        Assert.IsTrue(myHistory.Any());
    }

    [TestMethod]
    public void CustomQuoteClass()
    {
        List<MyCustomQuote> myGenericHistory = TestData.GetDefault()
            .Select(x => new MyCustomQuote
            {
                CloseDate = x.TickDate,
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
        MyCustomQuote q1 = new()
        {
            CloseDate = evalDate,
            Open = 1m,
            High = 1m,
            Low = 1m,
            CloseValue = 1m,
            Volume = 100
        };

        MyCustomQuote q2 = new()
        {
            CloseDate = evalDate,
            Open = 1m,
            High = 1m,
            Low = 1m,
            CloseValue = 1m,
            Volume = 100
        };

        MyCustomQuote q3 = new()
        {
            CloseDate = evalDate,
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

        Assert.IsTrue(q1 == q2);
        Assert.IsFalse(q1 == q3);

        Assert.IsFalse(q1 != q2);
        Assert.IsTrue(q1 != q3);
    }

    [TestMethod]
    public void CustomQuoteAggregate()
    {
        List<MyCustomQuote> myGenericHistory = TestData.GetIntraday()
            .Select(x => new MyCustomQuote
            {
                CloseDate = x.TickDate,
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
            .Select(x => new MyCustomQuote
            {
                CloseDate = x.TickDate,
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
        MyEma myIndicator = new()
        {
            TickDate = DateTime.Now,
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
            .Select(x => new MyEma
            {
                TickDate = x.TickDate,
                Ema = x.Ema,
                MyProperty = false
            });

        Assert.IsTrue(myIndicatorResults.Any());
    }

    [TestMethod]
    public void CustomResultClassFind()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<EmaResult> emaResults = Indicator.GetEma(quotes, 20);

        // can use a derive Indicator class using Linq

        IEnumerable<MyEma> myIndicatorResults = emaResults
            .Where(x => x.Ema != null)
            .Select(x => new MyEma
            {
                Id = 12345,
                TickDate = x.TickDate,
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
        QuoteProvider<Quote> provider = new();

        // initialize observers, get static results for comparison (later)
        Ema observeEma = provider.AttachEma(20);
        List<EmaResult> staticEma = quotesList.GetEma(20).ToList();

        Sma observeSma = provider.AttachSma(20);
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

            Assert.AreEqual(sEma.TickDate, rEma.TickDate);
            Assert.AreEqual(sEma.Ema, rEma.Ema);

            SmaResult sSma = staticSma[i];
            SmaResult rSma = streamSma[i];

            Assert.AreEqual(sSma.TickDate, rSma.TickDate);
            Assert.AreEqual(sSma.Sma, rSma.Sma);
        }
    }
}
