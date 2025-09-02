namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Awesome Oscillator.
/// </summary>
public static partial class Awesome
{
    /// <summary>
    /// Calculates the Awesome Oscillator for a series of data.
    /// </summary>
    /// <typeparam name="T">The type of the source data.</typeparam>
    /// <param name="source">The source data.</param>
    /// <param name="fastPeriods">The number of periods for the fast moving average. Default is 5.</param>
    /// <param name="slowPeriods">The number of periods for the slow moving average. Default is 34.</param>
    /// <returns>A list of Awesome Oscillator results.</returns>
    public static IReadOnlyList<AwesomeResult> ToAwesome<T>(
        this IReadOnlyList<T> source,
        int fastPeriods = 5,
        int slowPeriods = 34)
        where T : IReusable
    {
        // check parameter arguments
        Validate(fastPeriods, slowPeriods);

        // prefer HL2 when IQuote
        IReadOnlyList<IReusable> values
            = source.ToPreferredList(CandlePart.HL2);

        // initialize
        int length = values.Count;
        List<AwesomeResult> results = new(length);
        double[] pr = new double[length];

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            IReusable s = values[i];
            pr[i] = s.Value;

            double? oscillator = null;
            double? normalized = null;

            if (i >= slowPeriods - 1)
            {
                double sumSlow = 0;
                double sumFast = 0;

                for (int p = i + 1 - slowPeriods; p <= i; p++)
                {
                    sumSlow += pr[p];

                    if (p >= i + 1 - fastPeriods)
                    {
                        sumFast += pr[p];
                    }
                }

                oscillator = ((sumFast / fastPeriods) - (sumSlow / slowPeriods)).NaN2Null();
                normalized = pr[i] != 0 ? 100 * oscillator / pr[i] : null;
            }

            AwesomeResult r = new(
                Timestamp: s.Timestamp,
                Oscillator: oscillator,
                Normalized: normalized);

            results.Add(r);
        }

        return results;
    }
}
