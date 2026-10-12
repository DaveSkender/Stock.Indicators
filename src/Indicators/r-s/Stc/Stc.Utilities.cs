namespace FacioQuo.Stock.Indicators;

/// <summary>
/// Provides utility methods for Schaff Trend Cycle (STC) calculations.
/// </summary>
public static partial class Stc
{
    /// <summary>
    /// Removes the warmup periods and the further periods Schaff Trend Cycle needs for its values to converge.
    /// </summary>
    /// <param name="results">Schaff Trend Cycle results to evaluate.</param>
    /// <returns>Schaff Trend Cycle results with the warmup periods removed.</returns>
    public static IReadOnlyList<StcResult> RemoveWarmupPeriods(
        this IReadOnlyList<StcResult> results)
        => results.RemoveBeforeFirstValue(
            static x => x.Stc != null,
            static i => i + 250);

    /// <summary>
    /// Scores a MACD value within its window's range, giving exactly 0 and 100 at the
    /// extremes as Series' stochastic does; the division can miss them by a digit.
    /// </summary>
    /// <param name="macd">MACD value to score.</param>
    /// <param name="highHigh">Highest MACD in the window.</param>
    /// <param name="lowLow">Lowest MACD in the window.</param>
    /// <returns>Raw %K from 0 to 100.</returns>
    internal static double RawK(double macd, double highHigh, double lowLow)
        => highHigh == lowLow || macd <= lowLow ? 0
         : macd >= highHigh ? 100
         : 100 * (macd - lowLow) / (highHigh - lowLow);

    /// <summary>
    /// Validates the parameters for STC calculation.
    /// </summary>
    /// <param name="cyclePeriods">Number of periods for the cycle.</param>
    /// <param name="fastPeriods">Number of fast periods for the MACD calculation.</param>
    /// <param name="slowPeriods">Number of slow periods for the MACD calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a parameter is out of range.</exception>
    internal static void Validate(
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods)
    {
        // check parameter arguments
        if (cyclePeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(cyclePeriods), cyclePeriods,
                "Trend Cycle periods must be greater than 0 for STC.");
        }

        if (fastPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fastPeriods), fastPeriods,
                "Fast periods must be greater than 0 for STC.");
        }

        if (slowPeriods <= fastPeriods)
        {
            throw new ArgumentOutOfRangeException(nameof(slowPeriods), slowPeriods,
                "Slow periods must be greater than the fast period for STC.");
        }
    }
}
