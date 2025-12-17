namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the MACD (Moving Average Convergence Divergence) indicator.
/// Experimental implementation using QuoteX (internal long storage) for research purposes.
/// </summary>
public static partial class Macd
{
    /// <summary>
    /// Converts a list of QuoteX values to MACD (Moving Average Convergence Divergence) results.
    /// This is an experimental method using internal long storage for performance comparison.
    /// </summary>
    /// <param name="source">The list of QuoteX values to transform.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 12.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 26.</param>
    /// <param name="signalPeriods">The number of periods for the signal line. Default is 9.</param>
    /// <returns>A list of MACD results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of their valid range.</exception>
    internal static IReadOnlyList<MacdResult> ToMacdX(
        this List<QuoteX> source,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(fastPeriods, slowPeriods, signalPeriods);

        // initialize
        int length = source.Count;
        List<MacdResult> results = new(length);

        double lastEmaFast = double.NaN;
        double lastEmaSlow = double.NaN;
        double lastEmaMacd = double.NaN;

        double kFast = 2d / (fastPeriods + 1);
        double kSlow = 2d / (slowPeriods + 1);
        double kMacd = 2d / (signalPeriods + 1);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            // Convert from internal long to double for calculations
            // In a real scenario, we'd do more long-based math here
            double closeValue = (double)source[i].Close;

            // Fast EMA
            double emaFast
                = i >= fastPeriods - 1 && results[i - 1].FastEma is null
                ? IncrementSmaX(source, fastPeriods, i)
                : Ema.Increment(kFast, lastEmaFast, closeValue);

            // Slow EMA
            double emaSlow
                = i >= slowPeriods - 1 && results[i - 1].SlowEma is null
                ? IncrementSmaX(source, slowPeriods, i)
                : Ema.Increment(kSlow, lastEmaSlow, closeValue);

            // MACD
            double macd = emaFast - emaSlow;

            // Signal
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

            // results
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

    /// <summary>
    /// Helper method to calculate SMA for QuoteX using internal long values where possible.
    /// </summary>
    private static double IncrementSmaX(
        List<QuoteX> source,
        int lookbackPeriods,
        int endIndex)
    {
        if (endIndex < lookbackPeriods - 1 || endIndex >= source.Count)
        {
            return double.NaN;
        }

        // Use long arithmetic for summation to leverage internal storage
        // Then convert to double at the end
        long sumLong = 0;
        int startIndex = endIndex - lookbackPeriods + 1;

        for (int i = startIndex; i <= endIndex; i++)
        {
            sumLong += source[i].CloseLong;
        }

        // Convert OACurrency long back to decimal, then to double
        // This mimics what would happen in a fully long-based calculation
        return (double)decimal.FromOACurrency(sumLong) / lookbackPeriods;
    }
}
