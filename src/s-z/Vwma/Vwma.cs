namespace Skender.Stock.Indicators;

public static partial class Indicator
{
    // VOLUME WEIGHTED MOVING AVERAGE
    /// <include file='./info.xml' path='indicator/*' />
    ///
    public static IEnumerable<VwmaResult> GetVwma<TQuote>(
        this IEnumerable<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
    {
        // convert quotes
        List<QuoteD> quotesList = quotes.ToQuoteD();

        // check parameter arguments
        ValidateVwma(lookbackPeriods);

        // initialize
        int length = quotesList.Count;
        List<VwmaResult> results = new(length);

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotesList[i];

            VwmaResult result = new()
            {
                Date = q.Date
            };

            if (i + 1 >= lookbackPeriods)
            {
                double? sumCl = 0;
                double? sumVl = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    QuoteD d = quotesList[p];
                    double? c = d.Close;
                    double? v = d.Volume;

                    sumCl += c * v;
                    sumVl += v;
                }

                result.Vwma = sumVl != 0 ? (decimal?)(sumCl / sumVl) : null;
            }

            results.Add(result);
        }

        return results;
    }

    // parameter validation
    private static void ValidateVwma(
        int lookbackPeriods)
    {
        // check parameter arguments
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Vwma.");
        }
    }
}
