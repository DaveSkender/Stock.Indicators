using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Internal.Tests;

[TestClass]
public class Results : TestBase
{
    [TestMethod]
    public void Find()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<EmaResult> emaResults = Indicator.GetEma(quotes, 20);

        // find specific date
        DateTime findDate = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", EnglishCulture);

        EmaResult r = emaResults.Find(findDate);
        Assert.AreEqual(249.3519, NullMath.Round(r.Ema, 4));
    }

    [TestMethod]
    public void Remove()
    {
        // specific periods
        IEnumerable<HeikinAshiResult> results =
            quotes.GetHeikinAshi()
              .RemoveWarmupPeriods(102);

        Assert.AreEqual(400, results.Count());

        // bad remove period
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            quotes.GetAdx(14).RemoveWarmupPeriods(-1));
    }

    [TestMethod]
    public void RemoveTooMany()
    {
        // more than available
        IEnumerable<HeikinAshiResult> results =
            quotes.GetHeikinAshi()
              .RemoveWarmupPeriods(600);

        Assert.AreEqual(0, results.Count());
    }

    [TestMethod]
    public void Condense()
    {
        List<AdxResult> x = quotes.GetAdx(14).ToList();

        // make a few more in the middle null and NaN
        x[249].Adx = null;
        x[345].Adx = double.NaN;

        IEnumerable<AdxResult> r = x.Condense();

        // assertions
        Assert.AreEqual(473, r.Count());

        AdxResult last = r.LastOrDefault();
        Assert.AreEqual(17.7565, NullMath.Round(last.Pdi, 4));
        Assert.AreEqual(31.1510, NullMath.Round(last.Mdi, 4));
        Assert.AreEqual(34.2987, NullMath.Round(last.Adx, 4));
    }

    [TestMethod]
    public void SyncIndex()
    {
        // baseline for comparison
        List<SmaResult> baseline = new()
        {
            new SmaResult(DateTime.Parse("1/1/2000", EnglishCulture)) { Sma = null },
            new SmaResult(DateTime.Parse("1/2/2000", EnglishCulture)) { Sma = null },
            new SmaResult(DateTime.Parse("1/3/2000", EnglishCulture)) { Sma = 3 },
            new SmaResult(DateTime.Parse("1/4/2000", EnglishCulture)) { Sma = 4 },
            new SmaResult(DateTime.Parse("1/5/2000", EnglishCulture)) { Sma = 5 },
            new SmaResult(DateTime.Parse("1/6/2000", EnglishCulture)) { Sma = 6 },
            new SmaResult(DateTime.Parse("1/7/2000", EnglishCulture)) { Sma = 7 },
            new SmaResult(DateTime.Parse("1/8/2000", EnglishCulture)) { Sma = double.NaN },
            new SmaResult(DateTime.Parse("1/9/2000", EnglishCulture)) { Sma = null },
        };

        // to be synced
        List<EmaResult> eval = new()
        {
            new EmaResult(DateTime.Parse("1/3/2000", EnglishCulture)) { Ema = 3 },
            new EmaResult(DateTime.Parse("1/4/2000", EnglishCulture)) { Ema = 4 },
            new EmaResult(DateTime.Parse("1/5/2000", EnglishCulture)) { Ema = 5 },
            new EmaResult(DateTime.Parse("1/6/2000", EnglishCulture)) { Ema = 6 },
            new EmaResult(DateTime.Parse("1/7/2000", EnglishCulture)) { Ema = 7 },
            new EmaResult(DateTime.Parse("1/9/2000", EnglishCulture)) { Ema = double.NaN },
            new EmaResult(DateTime.Parse("1/10/2000", EnglishCulture)) { Ema = null },
        };

        // prepend option
        List<EmaResult> prepend = eval.SyncIndex(baseline, SyncType.Prepend).ToList();

        Assert.AreEqual(9, prepend.Count);
        Assert.AreEqual(3, prepend.Count(x => x.Ema is null));

        for (int i = 0; i < 6; i++)
        {
            SmaResult b = baseline[i];
            EmaResult r = prepend[i];

            Assert.AreEqual(b.Date, r.Date);
        }

        // append option
        List<EmaResult> append = eval.SyncIndex(baseline, SyncType.AppendOnly).ToList();

        Assert.AreEqual(10, append.Count);
        Assert.AreEqual(4, append.Count(x => x.Ema is null));

        for (int i = 0; i < 8; i++)
        {
            SmaResult b = baseline[i];
            EmaResult r = append[i];

            // Console.WriteLine($"{b.Date:d} {r.Date:d} {r.Ema}");

            Assert.AreEqual(b.Date, r.Date);
        }

        // remove option
        List<EmaResult> remove = eval.SyncIndex(baseline, SyncType.RemoveOnly).ToList();

        Assert.AreEqual(6, remove.Count);
        Assert.AreEqual(0, remove.Count(x => x.Ema is null));
        Assert.AreEqual(0, remove.Count(x =>
            x.Date == DateTime.Parse("1/10/2000", EnglishCulture)));

        // full option
        List<EmaResult> fullmatch = eval.SyncIndex(baseline, SyncType.FullMatch).ToList();

        Assert.AreEqual(9, fullmatch.Count);
        Assert.AreEqual(3, fullmatch.Count(x => x.Ema is null));
        Assert.AreEqual(0, fullmatch.Count(x =>
            x.Date == DateTime.Parse("1/10/2000", EnglishCulture)));

        for (int i = 0; i < baseline.Count; i++)
        {
            SmaResult b = baseline[i];
            EmaResult r = fullmatch[i];

            Assert.AreEqual(b.Date, r.Date);
        }
    }
}
