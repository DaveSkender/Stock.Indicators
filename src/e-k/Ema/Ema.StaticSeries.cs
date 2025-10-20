namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Exponential Moving Average (EMA) indicator.
/// </summary>
public static partial class Ema
{
    /// <summary>
    /// Converts a list of source data to EMA results.
    /// </summary>
    /// <param name="source">The list of source data.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A list of EMA results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static IReadOnlyList<EmaResult> ToEma(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        EmaResult[] results = new EmaResult[length];

        double lastEma = double.NaN;
        double k = 2d / (lookbackPeriods + 1);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            IReusable s = source[i];

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
                results[i] = new EmaResult(Timestamp: s.Timestamp);
                continue;
            }

            double ema = !double.IsNaN(lastEma)

                // calculate EMA (normally)
                ? Ema.Increment(k, lastEma, s.Value)

                // when no prior EMA, reset as SMA
                : Sma.Increment(source, lookbackPeriods, i);

            results[i] = new EmaResult(
                Timestamp: s.Timestamp,
                Ema: ema.NaN2Null());

            lastEma = ema;
        }

        return new List<EmaResult>(results);
    }
}
