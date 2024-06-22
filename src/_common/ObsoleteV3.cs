using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// OBSOLETE IN v3
public static partial class Indicator
{
    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use alternate 'GetX' variant.", false)]
    public static IEnumerable<AlligatorResult> GetAlligator(
        this IEnumerable<(DateTime, double)> priceTuples,
        int jawPeriods = 13,
        int jawOffset = 8,
        int teethPeriods = 8,
        int teethOffset = 5,
        int lipsPeriods = 5,
        int lipsOffset = 3)
        => priceTuples.ToSortedList().CalcAlligator(
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
        => quotesEval.ToTuple(CandlePart.Close).GetPrs(quotesBase.ToTuple(CandlePart.Close), lookbackPeriods);

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<RocResult> GetRoc<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods, int smaPeriods)
        where TQuote : IQuote
        => quotes.ToTuple(CandlePart.Close).GetRoc(lookbackPeriods);

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<StdDevResult> GetStdDev<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods, int smaPeriods)
        where TQuote : IQuote
        => quotes.ToTuple(CandlePart.Close).CalcStdDev(lookbackPeriods);

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<TrixResult> GetTrix<TQuote>(
        this IEnumerable<TQuote> quotes, int lookbackPeriods, int smaPeriods)
        where TQuote : IQuote
        => quotes.ToTuple(CandlePart.Close).CalcTrix(lookbackPeriods);

    // v3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("This method no longer defaults to Close.  Rename Use() to Use(CandlePart.Close) for an explicit conversion.", false)]
    public static IEnumerable<(DateTime Timestamp, double Value)> Use<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes.Select(x => x.ToTuple(CandlePart.Close));
}

// v3.0.0
[ExcludeFromCodeCoverage]
[Obsolete("Rename `BasicData` to `BasicResult`", true)]
public sealed class BasicData : IReusableResult
{
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
}
