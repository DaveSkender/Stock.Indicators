namespace Skender.Stock.Indicators;

/// <summary>
/// Force Index indicator.
/// </summary>
public static partial class ForceIndex
{
    /// <summary>
    /// Converts a list of quotes to Force Index results.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation. Default is 2.</param>
    /// <returns>A list of Force Index results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public static IReadOnlyList<ForceIndexResult> ToForceIndex(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 2)
        => quotes
            .ToQuoteDList()
            .CalcForceIndex(lookbackPeriods);

    /// <summary>
    /// Calculates the Force Index for a list of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of Force Index results.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="quotes"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    private static List<ForceIndexResult> CalcForceIndex(
        this List<QuoteD> quotes,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = quotes.Count;
        List<ForceIndexResult> results = new(length);
        double? prevFi = null;
        double? sumRawFi = 0;
        double k = 2d / (lookbackPeriods + 1);

        // skip first period
        if (length > 0)
        {
            results.Add(new(quotes[0].Timestamp));
        }

        // roll through source values
        for (int i = 1; i < length; i++)
        {
            QuoteD q = quotes[i];
            double? fi = null;

            // raw Force Index
            double? rawFi = q.Volume * (q.Close - quotes[i - 1].Close);

            // calculate EMA
            if (i >= lookbackPeriods)
            {
                if (prevFi is null)
                {
                    // first EMA value
                    sumRawFi += rawFi;
                    fi = sumRawFi / lookbackPeriods;
                }
                else
                {
                    fi = prevFi + (k * (rawFi - prevFi));
                }
            }

            // initialization period
            else
            {
                sumRawFi += rawFi;
            }

            results.Add(new ForceIndexResult(
                Timestamp: q.Timestamp,
                ForceIndex: fi));

            prevFi = fi;
        }

        return results;
    }
}
