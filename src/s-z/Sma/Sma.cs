namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // SIMPLE MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<SmaResult> GetSma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods,
        CandlePart candlePart = CandlePart.Close)
        where TQuote : IQuote
    {
        // check parameter arguments
        ValidateSma(lookbackPeriods);

        // initialize
        List<SimplePrice> bdList = quotes.ToPrice(candlePart);

        // calculate
        return bdList.CalcSma(lookbackPeriods);
    }

    // internals
    private static IEnumerable<SmaResult> CalcSma(
        this List<SimplePrice> bdList,
        int lookbackPeriods)
    {
        // note: pre-validated
        // initialize
        List<SmaResult> results = new(bdList.Count);

        // roll through quotes
        for (int i = 0; i < bdList.Count; i++)
        {
            SimplePrice q = bdList[i];
            int index = i + 1;

            SmaResult result = new()
            {
                Date = q.Date
            };

            if (index >= lookbackPeriods)
            {
                double sumSma = 0;
                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    SimplePrice d = bdList[p];
                    sumSma += d.Value;
                }

                result.Sma = (decimal)sumSma / lookbackPeriods;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateSma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMA.");
        }
    }
}
