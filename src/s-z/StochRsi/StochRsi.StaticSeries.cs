namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for Stochastic RSI (Relative Strength Index) calculations.
/// </summary>
public static partial class StochRsi
{
    /// <summary>
    /// Converts a source list to a list of StochRsiResult.
    /// </summary>    /// <param name="source">The source list.</param>
    /// <param name="rsiPeriods">The number of periods for RSI calculation.</param>
    /// <param name="stochPeriods">The number of periods for Stochastic calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing (default is 1).</param>
    /// <returns>A list of StochRsiResult.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    public static IReadOnlyList<StochRsiResult> ToStochRsi(
        this IReadOnlyList<IReusable> source,
        int rsiPeriods = 14,
        int stochPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 1)
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        // initialize results
        int length = source.Count;
        int initPeriods = Math.Min(rsiPeriods + stochPeriods - 1, length);
        List<StochRsiResult> results = new(length);

        // add back auto-pruned results
        for (int i = 0; i < initPeriods; i++)
        {
            IReusable s = source[i];
            results.Add(new(s.Timestamp));
        }

        // get Stochastic of RSI
        List<StochResult> stoResults =
            source
            .ToRsi(rsiPeriods)
            .Remove(Math.Min(rsiPeriods, length)) // TODO: still need to Remove() here, or auto-healing?
            .ConvertAll(x => new QuoteD(
                Timestamp: x.Timestamp,
                Open: 0,
                High: x.Rsi.Null2NaN(),
                Low: x.Rsi.Null2NaN(),
                Close: x.Rsi.Null2NaN(),
                Volume: 0
             ))
            .CalcStoch(
                stochPeriods,
                signalPeriods,
                smoothPeriods, 3, 2, MaType.SMA)
            .ToList();

        // add stoch results
        for (int i = rsiPeriods + stochPeriods - 1; i < length; i++)
        {
            StochResult r = stoResults[i - rsiPeriods];

            results.Add(new StochRsiResult(
                Timestamp: r.Timestamp,
                StochRsi: r.Oscillator,
                Signal: r.Signal));
        }

        return results;
    }
}
