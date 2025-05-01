namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Detrended Price Oscillator (DPO).
/// </summary>
public static partial class Dpo
{
    /// <summary>
    /// Converts a list of source data to Detrended Price Oscillator (DPO) results.
    /// </summary>
    /// <typeparam name="T">The type of the source data.</typeparam>
    /// <param name="source">The list of source data.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A list of DPO results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    [Series("DPO", "Detrended Price Oscillator", Category.Oscillator, ChartType.Oscillator)]
    public static IReadOnlyList<DpoResult> ToDpo<T>(
        this IReadOnlyList<T> source,
        [ParamNum<int>("Lookback Periods", 14, 2, 250)]
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<DpoResult> results = new(length);

        int offset = (lookbackPeriods / 2) + 1;

        IReadOnlyList<SmaResult> sma
            = source.ToSma(lookbackPeriods);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T src = source[i];

            double? dpoSma = null;
            double? dpoVal = null;

            if (i >= lookbackPeriods - offset - 1 && i < length - offset)
            {
                SmaResult s = sma[i + offset];
                dpoSma = s.Sma;
                dpoVal = s.Sma is null ? null : src.Value - s.Sma;
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
