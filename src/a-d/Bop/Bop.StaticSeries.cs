namespace Skender.Stock.Indicators;

/// <summary>
/// Balance of Power (BOP) on a series of quotes indicator.
/// </summary>
public static partial class Bop
{
    /// <summary>
    /// Calculates the Balance of Power (BOP) for a series of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="smoothPeriods">The number of periods to use for smoothing. Default is 14.</param>
    /// <returns>A read-only list of <see cref="BopResult"/> containing the BOP calculation results.</returns>
    public static IReadOnlyList<BopResult> ToBop(
        this IReadOnlyList<IQuote> quotes,
        int smoothPeriods = 14)
        => quotes
            .ToQuoteDList()
            .CalcBop(smoothPeriods);

    /// <summary>
    /// Calculates the Balance of Power (BOP) for a series of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="smoothPeriods">The number of periods to use for smoothing.</param>
    /// <returns>A list of <see cref="BopResult"/> containing the BOP calculation results.</returns>
    private static List<BopResult> CalcBop(
        this List<QuoteD> quotes,
        int smoothPeriods)
    {
        // check parameter arguments
        Validate(smoothPeriods);

        // initialize
        int length = quotes.Count;
        List<BopResult> results = new(length);

        double[] raw = quotes
            .Select(static x => x.High - x.Low != 0 ?
                (x.Close - x.Open) / (x.High - x.Low) : double.NaN)
            .ToArray();

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            double bop = double.NaN;

            if (i >= smoothPeriods - 1)
            {
                double sum = 0;
                for (int p = i - smoothPeriods + 1; p <= i; p++)
                {
                    sum += raw[p];
                }

                bop = sum / smoothPeriods;
            }

            results.Add(new(
                Timestamp: quotes[i].Timestamp,
                Bop: bop.NaN2Null()));
        }

        return results;
    }
}
