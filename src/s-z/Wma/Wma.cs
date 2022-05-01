namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // WEIGHTED MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<WmaResult> GetWma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> bdList = quotes.ToBasicClass(candlePart);

        // check parameter arguments
        ValidateWma(lookbackPeriods);

        // initialize
        List<WmaResult> results = new(bdList.Count);
        double divisor = (double)lookbackPeriods * (lookbackPeriods + 1) / 2d;

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            BasicData q = bdList[i];

            WmaResult result = new()
            {
                Date = q.Date
            };

            if (i + 1 >= lookbackPeriods)
            {
                double? wma = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    BasicData d = bdList[p];
                    wma += d.Value * (lookbackPeriods - (i + 1 - p - 1)) / divisor;
                }

                result.Wma = (decimal?)wma;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateWma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for WMA.");
        }
    }
}
