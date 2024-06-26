using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// OBSOLETE IN v3
public static partial class Indicator
{
    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use alternate 'GetX' variant.", false)]
    public static IEnumerable<AlligatorResult> GetAlligator(
        this IEnumerable<(DateTime d, double v)> priceTuples,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
        => priceTuples
           .Select(t => new Reusable(t.d, t.v))
           .ToList()
           .CalcAlligator(
            jawPeriods, jawOffset,
            teethPeriods, teethOffset,
            lipsPeriods, lipsOffset);

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<AdlResult> GetAdl<TQuote>(
        this IEnumerable<TQuote> quotes, int smaPeriods)
        where TQuote : IQuote
        => quotes.ToQuoteD().CalcAdl();

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<ObvResult> GetObv<TQuote>(
        this IEnumerable<TQuote> quotes, int smaPeriods)
        where TQuote : IQuote
        => quotes.ToQuoteD().CalcObv();

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<PrsResult> GetPrs<TQuote>(
        this IEnumerable<TQuote> quotesEval, IEnumerable<TQuote> quotesBase, int lookbackPeriods, int smaPeriods)
        where TQuote : IQuote
        => quotesEval.Use(CandlePart.Close).GetPrs(quotesBase.Use(CandlePart.Close), lookbackPeriods);

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<RocResult> GetRoc<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods, int smaPeriods)
        where TQuote : IQuote
        => quotes.Use(CandlePart.Close).GetRoc(lookbackPeriods);

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<StdDevResult> GetStdDev<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods, int smaPeriods)
        where TQuote : IQuote
        => quotes.Use(CandlePart.Close).ToList().CalcStdDev(lookbackPeriods);

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<TrixResult> GetTrix<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods, int smaPeriods)
        where TQuote : IQuote
        => quotes.Use(CandlePart.Close).ToList().CalcTrix(lookbackPeriods);

    // v3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("This method no longer defaults to Close.  Rename Use() to Use(CandlePart.Close) for an explicit conversion.", false)]
    public static IEnumerable<(DateTime Timestamp, double Value)> Use<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes.Select(x => (x.Timestamp, x.Value));

    // v3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Refactor to use `ToReusable()`", true)]
    public static Collection<(DateTime Timestamp, double Value)> ToTupleChainable<TResult>(
        this IEnumerable<TResult> reusable)
        where TResult : IReusableResult
        => reusable
            .Select(x => (x.Timestamp, x.Value))
            .OrderBy(x => x.Timestamp)
            .ToCollection();
}

// v3.0.0
[ExcludeFromCodeCoverage]
[Obsolete("Rename `BasicData` to `BasicResult`", true)]
public sealed class BasicData : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
}
