namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Detrended Price Oscillator (DPO).
/// </summary>
public static partial class Dpo
{
    /// <summary>
    /// Converts a list of source data to Detrended Price Oscillator (DPO) results.
    /// </summary>    /// <param name="source">The list of source data.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A list of DPO results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static IReadOnlyList<DpoResult> ToDpo(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods = 14)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<DpoResult> results = new(length);

        int offset = (lookbackPeriods / 2) + 1;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            IReusable src = source[i];

            double? dpoSma = null;
            double? dpoVal = null;

            // calculate SMA at offset position
            if (i >= lookbackPeriods - offset - 1 && i < length - offset)
            {
                double smaValue = Sma.Increment(
                    source,
                    lookbackPeriods,
                    i + offset);

                if (!double.IsNaN(smaValue))
                {
                    dpoSma = smaValue;
                    dpoVal = src.Value - smaValue;
                }
            }

            DpoResult r = new(
                Timestamp: src.Timestamp,
                Dpo: dpoVal,
                Sma: dpoSma);

            results.Add(r);
        }

        return results;
    }
}
