using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable all

namespace Skender.Stock.Indicators;

// OBSOLETE IN v3
public static partial class Indicator
{
    [ExcludeFromCodeCoverage]
    [Obsolete("Use alternate 'GetX' variant.", false)] // v3.0.0
    public static IEnumerable<AlligatorResult> GetAlligator(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
        => priceTuples
           .Select(t => new QuotePart(t.d, t.v))
           .ToList()
           .CalcAlligator(
            jawPeriods, jawOffset,
            teethPeriods, teethOffset,
            lipsPeriods, lipsOffset);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", true)] // v3.0.0
    public static IEnumerable<AdlResult> GetAdl<TQuote>(
        this IEnumerable<TQuote> quotes, int smaPeriods)
        where TQuote : IQuote
        => quotes.ToSortedList().CalcAdl();

    [ExcludeFromCodeCoverage]
    [Obsolete("Replace `GetEma(..)` with `ToEma(..)`", true)] // v3.0.0
    public static IEnumerable<EmaResult> GetEma<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods)
        where TQuote : IQuote
        => quotes.ToEma(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", true)] // v3.0.0
    public static IEnumerable<ObvResult> GetObv<TQuote>(
        this IEnumerable<TQuote> quotes, int smaPeriods)
        where TQuote : IQuote
        => quotes.GetObv();

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", true)] // v3.0.0
    public static IEnumerable<PrsResult> GetPrs<TQuote>(
        this IEnumerable<TQuote> quotesEval, IEnumerable<TQuote> quotesBase, int lookbackPeriods, int smaPeriods)
        where TQuote : IQuote
        => quotesEval.Use(CandlePart.Close).GetPrs(quotesBase.Use(CandlePart.Close), lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", true)] // v3.0.0
    public static IEnumerable<RocResult> GetRoc<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods, int smaPeriods)
        where TQuote : IQuote
        => quotes.Use(CandlePart.Close).GetRoc(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", true)] // v3.0.0
    public static IEnumerable<StdDevResult> GetStdDev<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods, int smaPeriods)
        where TQuote : IQuote
        => quotes.Use(CandlePart.Close).ToList().CalcStdDev(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", true)] // v3.0.0
    public static IEnumerable<TrixResult> GetTrix<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods, int smaPeriods)
        where TQuote : IQuote
        => quotes.Use(CandlePart.Close).ToList().CalcTrix(lookbackPeriods);

    [ExcludeFromCodeCoverage]
    [Obsolete("This method no longer defaults to Close.  Rename Use() to Use(CandlePart.Close) for an explicit conversion.", false)] // v3.0.0
    public static IEnumerable<(DateTime Timestamp, double Value)> Use<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes.Select(x => (x.Timestamp, x.Value));

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `ToReusable()`", true)] // v3.0.0
    public static Collection<(DateTime Timestamp, double Value)> ToTupleChainable<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusable
        => reusable.Select(x => (x.Timestamp, x.Value)).OrderBy(x => x.Timestamp).ToCollection();


    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List.First(c => c.Timestamp == lookupDate)`", false)] // v3.0.0
    public static TSeries Find<TSeries>(this IEnumerable<TSeries> series, DateTime lookupDate)
        where TSeries : ISeries => series.First(x => x.Timestamp == lookupDate);

    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `List.FindIndex(c => c.Timestamp == lookupDate)`", false)] // v3.0.0
    public static int FindIndex<TSeries>(this List<TSeries> series, DateTime lookupDate)
        where TSeries : ISeries => series?.FindIndex(x => x.Timestamp == lookupDate) ?? -1;
}

[Obsolete("Rename `IReusableResult` to `IReusable`", true)] // v3.0.0
public interface IReusableResult : IReusable;

[ExcludeFromCodeCoverage]
[Obsolete("Rename `BasicData` to `QuotePart`", true)] // v3.0.0
public sealed class BasicData : IReusable
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
}
