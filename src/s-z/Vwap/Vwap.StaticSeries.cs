namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the VWAP (Volume Weighted Average Price) indicator.
/// </summary>
public static partial class Vwap
{
    /// <summary>
    /// Calculates the VWAP for a series of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the elements in the source list, which must implement IQuote.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="startDate">The optional start date for the VWAP calculation. If not provided, the calculation starts from the first quote.</param>
    /// <returns>A list of VwapResult containing the VWAP values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source is null.</exception>
    public static IReadOnlyList<VwapResult> ToVwap<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        DateTime? startDate = null)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcVwap(startDate);

    /// <summary>
    /// Calculates the VWAP for a series of quotes.
    /// </summary>
    /// <param name="source">The source list of quotes.</param>
    /// <param name="startDate">The optional start date for the VWAP calculation. If not provided, the calculation starts from the first quote.</param>
    /// <returns>A list of VwapResult containing the VWAP values.</returns>
    private static List<VwapResult> CalcVwap(
        this IReadOnlyList<QuoteD> source,
        DateTime? startDate = null)
    {
        // check parameter arguments
        Validate(source, startDate);

        // initialize
        int length = source.Count;
        List<VwapResult> results = new(length);

        if (length == 0)
        {
            return results;
        }

        startDate ??= source[0].Timestamp;

        double? cumVolume = 0;
        double? cumVolumeTp = 0;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = source[i];

            double? v = q.Volume;
            double? h = q.High;
            double? l = q.Low;
            double? c = q.Close;

            double? vwap;

            if (q.Timestamp >= startDate)
            {
                cumVolume += v;
                cumVolumeTp += v * (h + l + c) / 3;

                vwap = cumVolume != 0 ? cumVolumeTp / cumVolume : null;
            }
            else
            {
                vwap = null;
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                Vwap: vwap));
        }

        return results;
    }
}
