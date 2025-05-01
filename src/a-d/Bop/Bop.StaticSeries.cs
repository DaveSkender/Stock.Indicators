namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Balance of Power (BOP) on a series of quotes.
/// </summary>
public static partial class Bop
{
    /// <summary>
    /// Calculates the Balance of Power (BOP) for a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the elements in the quotes list, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="smoothPeriods">The number of periods to use for smoothing. Default is 14.</param>
    /// <returns>A read-only list of <see cref="BopResult"/> containing the BOP calculation results.</returns>
    [Series("BOP", "Balance of Power (BOP)", Category.Oscillator, ChartType.Oscillator)]
    public static IReadOnlyList<BopResult> ToBop<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        [ParamNum<int>("Smooth Periods", 14, 1, 250)]
        int smoothPeriods = 14)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcBop(smoothPeriods);

    /// <summary>
    /// Calculates the Balance of Power (BOP) for a series of quotes.
    /// </summary>
    /// <param name="source">The source list of quotes.</param>
    /// <param name="smoothPeriods">The number of periods to use for smoothing.</param>
    /// <returns>A list of <see cref="BopResult"/> containing the BOP calculation results.</returns>
    private static List<BopResult> CalcBop(
        this List<QuoteD> source,
        int smoothPeriods)
    {
        // check parameter arguments
        Validate(smoothPeriods);

        // initialize
        int length = source.Count;
        List<BopResult> results = new(length);

        double[] raw = source
            .Select(x => x.High - x.Low != 0 ?
                (x.Close - x.Open) / (x.High - x.Low) : double.NaN)
            .ToArray();

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            double? bop = null;

            if (i >= smoothPeriods - 1)
            {
                double sum = 0;
                for (int p = i - smoothPeriods + 1; p <= i; p++)
                {
                    sum += raw[p];
                }

                bop = (sum / smoothPeriods).NaN2Null();
            }

            results.Add(new(
                Timestamp: source[i].Timestamp,
                Bop: bop));
        }

        return results;
    }
}
