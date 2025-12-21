namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the MACD (Moving Average Convergence Divergence) indicator.
/// </summary>
public static partial class Macd
{
    /// <summary>
    /// Experimental MACD calculation that operates directly on <see cref="QuoteD"/> to avoid
    /// intermediate conversions when benchmarking double-based quotes.
    /// </summary>
    internal static IReadOnlyList<MacdResult> ToMacdQuoteD(
        this IReadOnlyList<QuoteD> source,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
    {
        ArgumentNullException.ThrowIfNull(source);
        Validate(fastPeriods, slowPeriods, signalPeriods);

        int length = source.Count;
        List<MacdResult> results = new(length);

        double lastEmaFast = double.NaN;
        double lastEmaSlow = double.NaN;
        double lastEmaMacd = double.NaN;

        double kFast = 2d / (fastPeriods + 1);
        double kSlow = 2d / (slowPeriods + 1);
        double kMacd = 2d / (signalPeriods + 1);

        for (int i = 0; i < length; i++)
        {
            double close = source[i].Close;

            double emaFast
                = i >= fastPeriods - 1 && results[i - 1].FastEma is null
                ? IncrementSmaDouble(source, fastPeriods, i)
                : Ema.Increment(kFast, lastEmaFast, close);

            double emaSlow
                = i >= slowPeriods - 1 && results[i - 1].SlowEma is null
                ? IncrementSmaDouble(source, slowPeriods, i)
                : Ema.Increment(kSlow, lastEmaSlow, close);

            double macd = emaFast - emaSlow;

            double signal;

            if (i >= signalPeriods + slowPeriods - 2 && results[i - 1].Signal is null)
            {
                double sum = macd;
                for (int p = i - signalPeriods + 1; p < i; p++)
                {
                    sum += results[p].Value;
                }

                signal = sum / signalPeriods;
            }
            else
            {
                signal = Ema.Increment(kMacd, lastEmaMacd, macd);
            }

            results.Add(new MacdResult(
                Timestamp: source[i].Timestamp,
                Macd: macd.NaN2Null(),
                Signal: signal.NaN2Null(),
                Histogram: (macd - signal).NaN2Null(),
                FastEma: emaFast.NaN2Null(),
                SlowEma: emaSlow.NaN2Null()));

            lastEmaMacd = signal;
            lastEmaFast = emaFast;
            lastEmaSlow = emaSlow;
        }

        return results;
    }

    private static double IncrementSmaDouble(
        IReadOnlyList<QuoteD> source,
        int lookbackPeriods,
        int endIndex)
    {
        if (endIndex < lookbackPeriods - 1 || endIndex >= source.Count)
        {
            return double.NaN;
        }

        double sum = 0;
        int startIndex = endIndex - lookbackPeriods + 1;

        for (int i = startIndex; i <= endIndex; i++)
        {
            sum += source[i].Close;
        }

        return sum / lookbackPeriods;
    }
}
