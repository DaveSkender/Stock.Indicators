namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Force Index indicator.
/// </summary>
public static partial class ForceIndex
{
    /// <summary>
    /// Converts a list of quotes to Force Index results.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote data.</typeparam>
    /// <param name="quotes">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation. Default is 2.</param>
    /// <returns>A list of Force Index results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static IReadOnlyList<ForceIndexResult> ToForceIndex<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 2)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcForceIndex(lookbackPeriods);

    /// <summary>
    /// Calculates the Force Index for a list of quotes.
    /// </summary>
    /// <param name="source">The list of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A list of Force Index results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    private static List<ForceIndexResult> CalcForceIndex(
        this IReadOnlyList<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<ForceIndexResult> results = new(length);
        double? prevFi = null;
        double? sumRawFi = 0;
        double k = 2d / (lookbackPeriods + 1);

        // skip first period
        if (length > 0)
        {
            results.Add(new(source[0].Timestamp));
        }

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            QuoteD q = source[i];
            double? fi = null;

            // raw Force Index
            double? rawFi = q.Volume * (q.Close - source[i - 1].Close);

            // calculate EMA
            if (i > lookbackPeriods)
            {
                fi = prevFi + (k * (rawFi - prevFi));
            }

            // initialization period
            // TODO: update healing, without requiring specific indexing
            else
            {
                sumRawFi += rawFi;

                // first EMA value
                if (i == lookbackPeriods)
                {
                    fi = sumRawFi / lookbackPeriods;
                }
            }

            results.Add(new ForceIndexResult(
                Timestamp: q.Timestamp,
                ForceIndex: fi));

            prevFi = fi;
        }

        return results;
    }
}
