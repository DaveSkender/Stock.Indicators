namespace Skender.Stock.Indicators;

// FORCE INDEX (SERIES)

public static partial class ForceIndex
{
    public static IReadOnlyList<ForceIndexResult> ToForceIndex<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 2)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcForceIndex(lookbackPeriods);

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
