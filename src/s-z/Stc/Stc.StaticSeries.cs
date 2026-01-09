namespace Skender.Stock.Indicators;

/// <summary>
/// Schaff Trend Cycle (STC) indicator.
/// </summary>
public static partial class Stc
{
    /// <summary>
    /// Converts a series of values to Schaff Trend Cycle (STC) series.
    /// </summary>
    /// <param name="source">The source series of values.</param>
    /// <param name="cyclePeriods">The number of periods for the cycle.</param>
    /// <param name="fastPeriods">The number of fast periods for the MACD calculation.</param>
    /// <param name="slowPeriods">The number of slow periods for the MACD calculation.</param>
    /// <returns>A list of <see cref="StcResult"/> containing the STC values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source series is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are less than or equal to 0.</exception>
    public static IReadOnlyList<StcResult> ToStc(
        this IReadOnlyList<IReusable> source,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(cyclePeriods, fastPeriods, slowPeriods);

        // initialize results
        int length = source.Count;
        List<StcResult> results = new(length);

        // get stochastic of macd
        List<StochResult> stochMacd = source
          .ToMacd(fastPeriods, slowPeriods, 1)
          .Select(static x => new QuoteD(
              x.Timestamp, 0,
              x.Macd.Null2NaN(),
              x.Macd.Null2NaN(),
              x.Macd.Null2NaN(), 0))
          .ToList()
          .CalcStoch(cyclePeriods, 1, 3, 3, 2, MaType.SMA);

        // add stoch results
        for (int i = 0; i < length; i++)
        {
            StochResult r = stochMacd[i];

            results.Add(new StcResult(
                Timestamp: r.Timestamp,
                Stc: r.Oscillator));
        }

        return results;
    }
}
