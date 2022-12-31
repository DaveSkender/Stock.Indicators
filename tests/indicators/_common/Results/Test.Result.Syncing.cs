using Internal.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Skender.Stock.Indicators;

namespace Tests.Indicators;

[TestClass]
public class Syncing : TestBase
{
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
}
