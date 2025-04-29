namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for Stochastic RSI (Relative Strength Index) calculations.
/// </summary>
public static partial class StochRsi
{
    /// <summary>
    /// Converts a source list to a list of StochRsiResult.
    /// </summary>
    /// <typeparam name="T">The type of the source list elements.</typeparam>
    /// <param name="source">The source list.</param>
    /// <param name="rsiPeriods">The number of periods for RSI calculation.</param>
    /// <param name="stochPeriods">The number of periods for Stochastic calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing (default is 1).</param>
    /// <returns>A list of StochRsiResult.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    [Series("STOCH-RSI", "Stochastic RSI", Category.Oscillator, ChartType.Oscillator)]
    public static IReadOnlyList<StochRsiResult> ToStochRsi<T>(
        this IReadOnlyList<T> source,
        [ParamNum<int>("RSI Periods", 1, 250, 14)]
        int rsiPeriods,
        [ParamNum<int>("Stochastic Periods", 1, 250, 14)]
        int stochPeriods,
        [ParamNum<int>("Signal Periods", 1, 50, 3)]
        int signalPeriods,
        [ParamNum<int>("Smooth Periods", 1, 50, 1)]
        int smoothPeriods = 1)
        where T : IReusable
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
            T s = source[i];
            results.Add(new(s.Timestamp));
        }

        // get Stochastic of RSI
        List<StochResult> stoResults =
            source
            .ToRsi(rsiPeriods)
            .Remove(Math.Min(rsiPeriods, length)) // TODO: still need to Remove() here, or auto-healing?
            .Select(x => new QuoteD(
                Timestamp: x.Timestamp,
                Open: 0,
                High: x.Rsi.Null2NaN(),
                Low: x.Rsi.Null2NaN(),
                Close: x.Rsi.Null2NaN(),
                Volume: 0
             ))
            .ToList()
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
