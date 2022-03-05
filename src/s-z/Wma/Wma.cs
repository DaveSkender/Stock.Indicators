namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // WEIGHTED MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<WmaResult> GetWma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

        // check parameter arguments
        ValidateWma(lookbackPeriods);

        // initialize
        List<WmaResult> results = new(bdList.Count);
        double divisor = lookbackPeriods * (lookbackPeriods + 1) / 2d;

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            BasicD q = bdList[i];
            int index = i + 1;

            WmaResult result = new()
            {
                Date = q.Date
            };

            if (index >= lookbackPeriods)
            {
                double wma = 0;
                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    BasicD d = bdList[p];
                    wma += (double)d.Value * (lookbackPeriods - (index - p - 1)) / divisor;
                }

                result.Wma = (decimal)wma;
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
