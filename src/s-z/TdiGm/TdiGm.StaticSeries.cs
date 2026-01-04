namespace Skender.Stock.Indicators;

/// <summary>
/// TDI-GM indicator.
/// </summary>
public static partial class TdiGm
{
    /// <summary>
    /// Calculates the TDI-GM for a series of data.
    /// </summary>
    /// <param name="source">The source list of data.</param>
    /// <param name="rsiPeriod">The RSI period.</param>
    /// <param name="bandLength">The band length.</param>
    /// <param name="fastLength">The fast length.</param>
    /// <param name="slowLength">The slow length.</param>
    /// <returns>A list of TdiGmResult containing the TDI-GM values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
    public static IReadOnlyList<TdiGmResult> ToTdiGm(
        this IReadOnlyList<IReusable> source,
        int rsiPeriod = 21,
        int bandLength = 34,
        int fastLength = 2,
        int slowLength = 7)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(rsiPeriod, bandLength, fastLength, slowLength);

        // initialize
        int length = source.Count;
        List<TdiGmResult> results = new(length);

        // calculate RSI on the source data (RsiResult implements IReusable)
        IReadOnlyList<RsiResult> rsiResults = source.ToRsi(rsiPeriod);

        // calculate components on RSI values
        IReadOnlyList<SmaResult> middleBandResults = rsiResults.ToSma(bandLength);
        IReadOnlyList<StdDevResult> stdDevResults = rsiResults.ToStdDev(bandLength);
        IReadOnlyList<SmaResult> fastMaResults = rsiResults.ToSma(fastLength);
        IReadOnlyList<SmaResult> slowMaResults = rsiResults.ToSma(slowLength);

        // combine results
        for (int i = 0; i < length; i++)
        {
            double? upper = null;
            double? lower = null;
            double? middle = null;

            double? middleBand = middleBandResults[i].Sma;
            double? stdDev = stdDevResults[i].StdDev;

            if (middleBand != null && stdDev != null)
            {
                double offset = 1.6185 * stdDev.Value;
                upper = middleBand.Value + offset;
                lower = middleBand.Value - offset;
                middle = middleBand.Value;
            }

            results.Add(
                new TdiGmResult {
                    Timestamp = source[i].Timestamp,
                    Upper = upper,
                    Lower = lower,
                    Middle = middle,
                    Fast = fastMaResults[i].Sma,
                    Slow = slowMaResults[i].Sma
                });
        }

        return results;
    }
}
