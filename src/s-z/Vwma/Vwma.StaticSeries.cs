namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the VWMA (Volume Weighted Moving Average) indicator.
/// </summary>
public static partial class Vwma
{
    /// <summary>
    /// Calculates the VWMA for a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the elements in the source list, which must implement IQuote.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>A list of VwmaResult containing the VWMA values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    public static IReadOnlyList<VwmaResult> ToVwma<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcVwma(lookbackPeriods);

    /// <summary>
    /// Calculates the VWMA for a series of quotes.
    /// </summary>
    /// <param name="source">The source list of quotes.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>A list of VwmaResult containing the VWMA values.</returns>
    private static List<VwmaResult> CalcVwma(
        this List<QuoteD> source,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = source.Count;
        List<VwmaResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            double vwma;

            if (i + 1 >= lookbackPeriods)
            {
                double sumCl = 0;
                double sumVl = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    QuoteD d = source[p];
                    double c = d.Close;
                    double v = d.Volume;

                    sumCl += c * v;
                    sumVl += v;
                }

                vwma = sumVl != 0 ? sumCl / sumVl : double.NaN;
            }
            else
            {
                vwma = double.NaN;
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                Vwma: vwma.NaN2Null()));
        }

        return results;
    }
}
