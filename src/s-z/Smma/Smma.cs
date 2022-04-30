namespace Skender.Stock.Indicators;
#nullable disable

public static partial class Indicator
{
    // SMOOTHED MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
    ///
    public static IEnumerable<SmmaResult> GetSmma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<BasicData> quotesList = quotes.ToBasicData(CandlePart.Close);

        // check parameter arguments
        ValidateSmma(lookbackPeriods);

        // initialize
        List<SmmaResult> results = new(quotesList.Count);
        double? prevValue = null;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            BasicData q = quotesList[i];

            SmmaResult result = new()
            {
                Date = q.Date
            };

            // calculate SMMA
            if (i + 1 > lookbackPeriods)
            {
                result.Smma = (decimal)((prevValue * (lookbackPeriods - 1)) + q.Value)
                            / lookbackPeriods;
            }

            // first SMMA calculated as simple SMA
            else if (i + 1 == lookbackPeriods)
            {
                double sumClose = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    BasicData d = quotesList[p];
                    sumClose += d.Value;
                }

                result.Smma = (decimal)(sumClose / lookbackPeriods);
            }

            prevValue = (double?)result.Smma;
            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateSmma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for SMMA.");
        }
    }
}
