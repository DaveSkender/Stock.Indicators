/// <summary>
/// Provides methods for calculating the Hull Moving Average (HMA) indicator.
/// </summary>
public static partial class Hma
{
    /// <summary>
    /// Converts a list of time-series values to Hull Moving Average (HMA) results.
    /// </summary>
    /// <typeparam name="T">The type of the time-series values, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The list of time-series values to transform.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A list of HMA results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are less than 2.</exception>
    public static IReadOnlyList<HmaResult> ToHma<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        int shiftQty = lookbackPeriods - 1;
        List<IReusable> synthHistory = [];

        IReadOnlyList<WmaResult> wmaN1
            = source.ToWma(lookbackPeriods);

        IReadOnlyList<WmaResult> wmaN2
            = source.ToWma(lookbackPeriods / 2);

        // roll through source values, to get interim synthetic quotes
        for (int i = 0; i < length; i++)
        {
            T s = source[i];

            WmaResult w1 = wmaN1[i];
            WmaResult w2 = wmaN2[i];

            if (i < shiftQty)
            {
                continue;
            }

            QuotePart sh = new(
                s.Timestamp,
                (w2.Wma.Null2NaN() * 2d) - w1.Wma.Null2NaN());

            synthHistory.Add(sh);
        }

        // add back truncated null results
        int sqN = (int)Math.Sqrt(lookbackPeriods);

        List<HmaResult> results = source
            .Take(shiftQty)
            .Select(x => new HmaResult(x.Timestamp))
            .ToList();

        // calculate final HMA = WMA with period SQRT(n)
        List<HmaResult> hmaResults = synthHistory.ToWma(sqN)
            .Select(x => new HmaResult(
                Timestamp: x.Timestamp,
                Hma: x.Wma
             ))
            .ToList();

        // add WMA to results
        results.AddRange(hmaResults);

        return results.ToSortedList();
    }
}
