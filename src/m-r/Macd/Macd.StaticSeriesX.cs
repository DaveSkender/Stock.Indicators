namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the MACD (Moving Average Convergence Divergence) indicator.
/// Experimental implementation using QuoteX (internal long storage) for research purposes.
/// </summary>
public static partial class Macd
{
    private const double OaCurrencyScale = 10000d;

    /// <summary>
    /// Converts a list of QuoteX values to MACD (Moving Average Convergence Divergence) results.
    /// This is an experimental method using internal long storage for performance comparison.
    /// Uses long values directly from QuoteX and converts to double only at the result boundary.
    /// </summary>
    /// <param name="source">The list of QuoteX values to transform.</param>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 12.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 26.</param>
    /// <param name="signalPeriods">The number of periods for the signal line. Default is 9.</param>
    /// <returns>A list of MACD results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are out of their valid range.</exception>
    internal static IReadOnlyList<MacdResult> ToMacdX(
        this IReadOnlyList<QuoteX> source,
        int fastPeriods = 12,
        int slowPeriods = 26,
        int signalPeriods = 9)
    {
        ArgumentNullException.ThrowIfNull(source);
        Validate(fastPeriods, slowPeriods, signalPeriods);

        int length = source.Count;
        List<MacdResult> results = new(length);

        long lastEmaFast = 0;
        long lastEmaSlow = 0;
        long lastSignal = 0;

        bool hasEmaFast = false;
        bool hasEmaSlow = false;
        bool hasSignal = false;

        long[] macdValues = new long[length];
        bool[] macdReady = new bool[length];

        double kFast = 2d / (fastPeriods + 1);
        double kSlow = 2d / (slowPeriods + 1);
        double kMacd = 2d / (signalPeriods + 1);

        for (int i = 0; i < length; i++)
        {
            long closeValue = source[i].CloseLong;

            bool seedFast = i >= fastPeriods - 1 && !hasEmaFast;
            long emaFast = seedFast
                ? IncrementSmaLong(source, fastPeriods, i)
                : hasEmaFast
                    ? Increment(kFast, lastEmaFast, closeValue)
                    : lastEmaFast;
            hasEmaFast |= seedFast;

            bool seedSlow = i >= slowPeriods - 1 && !hasEmaSlow;
            long emaSlow = seedSlow
                ? IncrementSmaLong(source, slowPeriods, i)
                : hasEmaSlow
                    ? Increment(kSlow, lastEmaSlow, closeValue)
                    : lastEmaSlow;
            hasEmaSlow |= seedSlow;

            bool macdIsReady = hasEmaFast && hasEmaSlow;
            long macd = macdIsReady
                ? emaFast - emaSlow
                : 0;

            macdValues[i] = macd;
            macdReady[i] = macdIsReady;

            bool signalIsReady = false;
            long signal = 0;

            if (macdIsReady)
            {
                signalIsReady = i >= signalPeriods + slowPeriods - 2 && !hasSignal
                    ? SeedSignal(macdValues, macdReady, signalPeriods, i, out signal)
                    : hasSignal && Increment(kMacd, lastSignal, macd, out signal);

                if (signalIsReady)
                {
                    hasSignal = true;
                }
            }

            bool histogramReady = macdIsReady && signalIsReady;
            long histogram = histogramReady
                ? macd - signal
                : 0;

            results.Add(new MacdResult(
                Timestamp: source[i].Timestamp,
                Macd: macdIsReady ? ScaleToDouble(macd) : null,
                Signal: signalIsReady ? ScaleToDouble(signal) : null,
                Histogram: histogramReady ? ScaleToDouble(histogram) : null,
                FastEma: hasEmaFast ? ScaleToDouble(emaFast) : null,
                SlowEma: hasEmaSlow ? ScaleToDouble(emaSlow) : null));

            if (hasEmaFast)
            {
                lastEmaFast = emaFast;
            }

            if (hasEmaSlow)
            {
                lastEmaSlow = emaSlow;
            }
            if (signalIsReady)
            {
                lastSignal = signal;
            }
        }

        return results;
    }

    /// <summary>
    /// Increments the EMA value using the smoothing factor.
    /// </summary>
    /// <param name="k">The smoothing factor.</param>
    /// <param name="lastEma">The last EMA value.</param>
    /// <param name="newPrice">The new price value.</param>
    /// <returns>The incremented EMA value.</returns>
    private static long Increment(
        double k,
        long lastEma,
        long newPrice)
        => (long)(lastEma + (k * (newPrice - lastEma)));

    /// <summary>
    /// Helper method to calculate SMA for QuoteX using internal long values.
    /// Uses long arithmetic for summation and returns the scaled long average.
    /// </summary>
    private static long IncrementSmaLong(
        IReadOnlyList<QuoteX> source,
        int lookbackPeriods,
        int endIndex)
    {
        if (endIndex < lookbackPeriods - 1 || endIndex >= source.Count)
        {
            return 0;
        }

        long sumLong = 0;
        int startIndex = endIndex - lookbackPeriods + 1;

        for (int i = startIndex; i <= endIndex; i++)
        {
            sumLong += source[i].CloseLong;
        }

        return sumLong / lookbackPeriods;
    }

    /// <summary>
    /// Seeds the initial MACD signal average using long-based MACD values.
    /// </summary>
    private static bool SeedSignal(
        long[] macdValues,
        bool[] macdReady,
        int lookbackPeriods,
        int endIndex,
        out long signal)
    {
        signal = 0;

        if (endIndex < lookbackPeriods - 1)
        {
            return false;
        }

        long sum = 0;
        int startIndex = endIndex - lookbackPeriods + 1;

        for (int i = startIndex; i <= endIndex; i++)
        {
            if (!macdReady[i])
            {
                return false;
            }

            sum += macdValues[i];
        }

        signal = sum / lookbackPeriods;
        return true;
    }

    private static bool Increment(
        double k,
        long lastSignal,
        long macd,
        out long signal)
    {
        signal = (long)(lastSignal + (k * (macd - lastSignal)));
        return true;
    }

    private static double ScaleToDouble(long value)
        => value / OaCurrencyScale;
}
