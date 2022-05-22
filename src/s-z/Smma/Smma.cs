namespace Skender.Stock.Indicators;

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
        List<BasicData> quotesList = quotes.ToBasicClass(CandlePart.Close);

        // check parameter arguments
        ValidateSmma(lookbackPeriods);

        // initialize
        List<SmmaResult> results = new(quotesList.Count);
        double prevValue = double.NaN;

        // roll through quotes
        for (int i = 0; i < quotesList.Count; i++)
        {
            double smma = double.NaN;
            BasicData q = quotesList[i];

            SmmaResult result = new()
            {
                Date = q.Date
            };

            // calculate SMMA
            if (i + 1 > lookbackPeriods)
            {
                smma = ((prevValue * (lookbackPeriods - 1)) + q.Value) / lookbackPeriods;
                result.Smma = smma;
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

                smma = sumClose / lookbackPeriods;
                result.Smma = smma;
            }

            prevValue = smma;
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
