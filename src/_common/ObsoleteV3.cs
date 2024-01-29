using System.Diagnostics.CodeAnalysis;

namespace Skender.Stock.Indicators;

// OBSOLETE IN v3
public static partial class Indicator
{
    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<AdlResult> GetAdl<TQuote>(
        this IEnumerable<TQuote> quotes,
        int smaPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        if (smaPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for ADL.");
        }

        // add SMA
        List<AdlResult> results = quotes
            .ToQuoteD()
            .CalcAdl();

        List<SmaResult> sma = results
            .GetSma(smaPeriods)
            .ToList();

        for (int i = 0; i < results.Count; i++)
        {
            results[i].AdlSma = sma[i].Sma;
        }

        return results;
    }

    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<ObvResult> GetObv<TQuote>(
        this IEnumerable<TQuote> quotes,
        int smaPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        if (smaPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for OBV.");
        }

        List<ObvResult> results = quotes
            .ToQuoteD()
            .CalcObv();

        List<SmaResult> sma = results
            .GetSma(smaPeriods)
            .ToList();

        for (int i = 0; i < results.Count; i++)
        {
            results[i].ObvSma = sma[i].Sma;
        }

        return results;
    }

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<PrsResult> GetPrs<TQuote>(
        this IEnumerable<TQuote> quotesEval,
        IEnumerable<TQuote> quotesBase,
        int lookbackPeriods,
        int smaPeriods)
        where TQuote : IQuote
    {
        if (smaPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for PRS.");
        }

        List<(DateTime, double)> tpListBase = quotesBase
            .ToTuple(CandlePart.Close);
        List<(DateTime, double)> tpListEval = quotesEval
            .ToTuple(CandlePart.Close);

        List<PrsResult> results = [.. CalcPrs(tpListEval, tpListBase, lookbackPeriods)];
        List<SmaResult> sma = results.GetSma(smaPeriods).ToList();

        for (int i = 0; i < results.Count; i++)
        {
            results[i].PrsSma = sma[i].Sma;
        }

        return results;
    }

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<RocResult> GetRoc<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int smaPeriods)
        where TQuote : IQuote
    {
        if (smaPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for ROC.");
        }

        List<RocResult> results = quotes
            .ToTuple(CandlePart.Close)
            .CalcRoc(lookbackPeriods);

        List<SmaResult> sma = results
            .GetSma(smaPeriods)
            .ToList();

        for (int i = 0; i < results.Count; i++)
        {
            results[i].RocSma = sma[i].Sma;
        }

        return results;
    }

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<StdDevResult> GetStdDev<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int smaPeriods)
        where TQuote : IQuote
    {
        if (smaPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for Standard Deviation.");
        }

        List<StdDevResult> results = quotes
            .ToTuple(CandlePart.Close)
            .CalcStdDev(lookbackPeriods);

        List<SmaResult> sma = results
            .GetSma(smaPeriods)
            .ToList();

        for (int i = 0; i < results.Count; i++)
        {
            results[i].StdDevSma = sma[i].Sma;
        }

        return results;
    }

    // 3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("Use a chained `results.GetSma(smaPeriods)` to generate a moving average.", false)]
    public static IEnumerable<TrixResult> GetTrix<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        int smaPeriods)
        where TQuote : IQuote
    {
        // check parameter arguments
        if (smaPeriods is <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                "SMA periods must be greater than 0 for TRIX.");
        }

        // add SMA
        List<TrixResult> results = quotes
            .ToTuple(CandlePart.Close)
            .CalcTrix(lookbackPeriods);

        List<SmaResult> sma = results
            .GetSma(smaPeriods)
            .ToList();

        for (int i = 0; i < results.Count; i++)
        {
            results[i].Signal = sma[i].Sma;
        }

        return results;
    }

    // v3.0.0
    [ExcludeFromCodeCoverage]
    [Obsolete("This method no longer defaults to Close.  Rename Use() to Use(CandlePart.Close) for an explicit conversion.", false)]
    public static IEnumerable<(DateTime TickDate, double Value)> Use<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes.Select(x => x.ToTuple(CandlePart.Close));
}

// v3.0.0
[ExcludeFromCodeCoverage]
[Obsolete("Rename `BasicData` to `BasicResult`", true)]
public sealed class BasicData : IReusableResult
{
    public DateTime TickDate { get; set; }
    public double Value { get; set; }
}
