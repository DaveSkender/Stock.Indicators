namespace Skender.Stock.Indicators;

/// <summary>
/// VWMA (Volume Weighted Moving Average) indicator.
/// </summary>
public static partial class Vwma
{
    /// <summary>
    /// Calculates the VWMA for a series of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of VwmaResult containing the VWMA values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="quotes"/> is null.</exception>
    public static IReadOnlyList<VwmaResult> ToVwma(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
        => quotes
            .ToQuoteDList()
            .CalcVwma(lookbackPeriods);

    /// <summary>
    /// Calculates the VWMA for a series of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A list of VwmaResult containing the VWMA values.</returns>
    private static List<VwmaResult> CalcVwma(
        this List<QuoteD> quotes,
        int lookbackPeriods)
    {
        // check parameter arguments
        Validate(lookbackPeriods);

        // initialize
        int length = quotes.Count;
        List<VwmaResult> results = new(length);

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotes[i];

            double vwma;

            if (i + 1 >= lookbackPeriods)
            {
                double sumCl = 0;
                double sumVl = 0;
                for (int p = i + 1 - lookbackPeriods; p <= i; p++)
                {
                    QuoteD d = quotes[p];
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
