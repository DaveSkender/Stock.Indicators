using System.Collections.ObjectModel;
using Internal.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

[TestClass]
public class Results : TestBase
{
    [TestMethod]
    public void Find()
    {
        IEnumerable<Quote> quotes = TestData.GetDefault();
        IEnumerable<EmaResult> emaResults = quotes.GetEma(20);

        // find specific date
        DateTime findDate = DateTime.ParseExact("2018-12-31", "yyyy-MM-dd", EnglishCulture);

        EmaResult r = emaResults.Find(findDate);
        Assert.AreEqual(249.3519, r.Ema.Round(4));
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
        _ = Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
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

        List<AdxResult> r = x.Condense().ToList();

        // proper quantities
        Assert.AreEqual(473, r.Count);

        // sample values
        AdxResult last = r.LastOrDefault();
        Assert.AreEqual(17.7565, last.Pdi.Round(4));
        Assert.AreEqual(31.1510, last.Mdi.Round(4));
        Assert.AreEqual(34.2987, last.Adx.Round(4));
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

        // no results
        List<SmaResult> noBaseline = new();
        List<EmaResult> noEval = new();

        IEnumerable<EmaResult> noBaseResults = eval.SyncIndex(noBaseline);
        IEnumerable<EmaResult> noEvalResults = noEval.SyncIndex(baseline);

        Assert.IsFalse(noBaseResults.Any());
        Assert.IsFalse(noEvalResults.Any());
    }

    [TestMethod]
    public void ToTuple()
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

        // default to NaN with pruning
        List<(DateTime Date, double Value)> naNresults = baseline.ToTuple();

        Assert.AreEqual(5, naNresults.Count(x => !double.IsNaN(x.Value)));
        Assert.AreEqual(2, naNresults.Count(x => double.IsNaN(x.Value)));

        // with null option
        List<(DateTime Date, double? Value)> nullResults = baseline.ToTuple(NullTo.Null);

        Assert.AreEqual(3, nullResults.Count(x => x.Value is null));
        Assert.AreEqual(1, nullResults.Count(x => x.Value is double.NaN));

        // with explicit nullable NaN option
        List<(DateTime Date, double? Value)> nullableResults = baseline.ToTuple(NullTo.NaN);

        Assert.AreEqual(0, nullableResults.Count(x => x.Value is null));
        Assert.AreEqual(4, nullableResults.Count(x => x.Value is double.NaN));

        // PUBLIC VARIANT
        // default to NaN with pruning
        Collection<(DateTime Date, double Value)> cnaNresults = baseline.ToTupleCollection();

        Assert.AreEqual(5, cnaNresults.Count(x => !double.IsNaN(x.Value)));
        Assert.AreEqual(2, cnaNresults.Count(x => double.IsNaN(x.Value)));

        // with null option
        Collection<(DateTime Date, double? Value)> cnullResults = baseline.ToTupleCollection(NullTo.Null);

        Assert.AreEqual(3, cnullResults.Count(x => x.Value is null));
        Assert.AreEqual(1, cnullResults.Count(x => x.Value is double.NaN));

        // with explicit nullable NaN option
        Collection<(DateTime Date, double? Value)> cnullableResults = baseline.ToTupleCollection(NullTo.NaN);

        Assert.AreEqual(0, cnullableResults.Count(x => x.Value is null));
        Assert.AreEqual(4, cnullableResults.Count(x => x.Value is double.NaN));

        // PUBLIC VARIANT Sorted Collection
        Collection<SmaResult> sortResults = baseline
            .ToSortedCollection();

        Assert.AreEqual(5, sortResults[4].Sma);
    }
}
