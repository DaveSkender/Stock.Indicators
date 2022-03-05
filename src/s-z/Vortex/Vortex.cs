namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // VORTEX INDICATOR
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<VortexResult> GetVortex<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ConvertToList();

        // check parameter arguments
        ValidateVortex(lookbackPeriods);

        // initialize
        int length = quotesList.Count;
        List<VortexResult> results = new(length);

        double[] tr = new double[length];
        double[] pvm = new double[length];
        double[] nvm = new double[length];

        double prevHigh = 0;
        double prevLow = 0;
        double prevClose = 0;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotesList[i];
            int index = i + 1;

            VortexResult result = new()
            {
                Date = q.Date
            };

            // skip first period
            if (index == 1)
            {
                results.Add(result);
                prevHigh = q.High;
                prevLow = q.Low;
                prevClose = q.Close;
                continue;
            }

            // trend information
            double highMinusPrevClose = Math.Abs(q.High - prevClose);
            double lowMinusPrevClose = Math.Abs(q.Low - prevClose);

            tr[i] = Math.Max(q.High - q.Low, Math.Max(highMinusPrevClose, lowMinusPrevClose));
            pvm[i] = Math.Abs(q.High - prevLow);
            nvm[i] = Math.Abs(q.Low - prevHigh);

            prevHigh = q.High;
            prevLow = q.Low;
            prevClose = q.Close;

            // vortex indicator
            if (index > lookbackPeriods)
            {
                double sumTr = 0;
                double sumPvm = 0;
                double sumNvm = 0;

                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    sumTr += tr[p];
                    sumPvm += pvm[p];
                    sumNvm += nvm[p];
                }

                if (sumTr is not 0)
                {
                    result.Pvi = sumPvm / sumTr;
                    result.Nvi = sumNvm / sumTr;
                }
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateVortex(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 1 for VI.");
        }
    }
}
