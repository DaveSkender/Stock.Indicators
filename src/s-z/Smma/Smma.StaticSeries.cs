namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Smoothed Moving Average (SMMA) for a series of data.
/// </summary>
public static partial class Smma
{
    /// <summary>
    /// Calculates the Smoothed Moving Average (SMMA) for a series of data.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source list, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The source list of data.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the SMMA calculation.</param>
    /// <returns>A list of <see cref="SmmaResult"/> containing the SMMA values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than 1.</exception>
    public static IReadOnlyList<SmmaResult> ToSmma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<SmmaResult> results = new(length);
        double prevSmma = double.NaN;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            // skip incalculable periods
            if (i < lookbackPeriods - 1)
            {
                results.Add(new(s.Timestamp));
                continue;
            }

            double smma;

            // when no prior SMMA, reset as SMA
            if (double.IsNaN(prevSmma))
            {
                double sum = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    T ps = source[p];
                    sum += ps.Value;
                }

                smma = sum / lookbackPeriods;
            }

            // normal SMMA
            else
            {
                smma = ((prevSmma * (lookbackPeriods - 1)) + s.Value) / lookbackPeriods;
            }

            results.Add(new SmmaResult(
                Timestamp: s.Timestamp,
                Smma: smma.NaN2Null()));

            prevSmma = smma;
        }

        return results;
    }

    /// <summary>
    /// Creates a <see cref="SmmaList"/> from a list of quotes
    /// for incremental calculation of the Smoothed Moving Average.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quotes, which must implement IQuote.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A <see cref="SmmaList"/> populated with the provided quotes.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static SmmaList ToSmmaBufferList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(lookbackPeriods);

        // initialize buffer list
        SmmaList bufferList = new(lookbackPeriods);

        // add quotes to buffer list
        bufferList.Add(quotes);

        return bufferList;
    }

    /// <summary>
    /// Creates a <see cref="SmmaList"/> from a list of reusable values
    /// for incremental calculation of the Smoothed Moving Average.
    /// </summary>
    /// <typeparam name="T">The type of the reusable values, which must implement IReusable.</typeparam>
    /// <param name="source">The list of reusable values.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A <see cref="SmmaList"/> populated with the provided values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static SmmaList ToSmmaBufferList<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods = 20)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize buffer list
        SmmaList bufferList = new(lookbackPeriods);

        // add values to buffer list
        bufferList.Add(source);

        return bufferList;
    }
}
