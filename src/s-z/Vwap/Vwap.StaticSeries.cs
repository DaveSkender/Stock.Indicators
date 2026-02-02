namespace Skender.Stock.Indicators;

/// <summary>
/// VWAP (Volume Weighted Average Price) indicator.
/// </summary>
public static partial class Vwap
{
    /// <summary>
    /// Calculates the VWAP for a series of quotes starting from a specific date.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="startDate">The start date for the VWAP calculation.</param>
    /// <returns>A list of VwapResult containing the VWAP values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="quotes"/> is null.</exception>
    public static IReadOnlyList<VwapResult> ToVwap(
        this IReadOnlyList<IQuote> quotes,
        DateTime startDate)
        => quotes
            .ToQuoteDList()
            .CalcVwap(startDate);

    /// <summary>
    /// Calculates the VWAP for a series of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>A list of VwapResult containing the VWAP values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="quotes"/> is null.</exception>
    public static IReadOnlyList<VwapResult> ToVwap(
        this IReadOnlyList<IQuote> quotes)
        => quotes?.Count is null or 0
            ? []
            : quotes
                .ToQuoteDList()
                .CalcVwap(quotes[0].Timestamp);

    /// <summary>
    /// Calculates the VWAP for a series of quotes.
    /// </summary>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="startDate">The optional start date for the VWAP calculation. If not provided, the calculation starts from the first quote.</param>
    /// <returns>A list of VwapResult containing the VWAP values.</returns>
    private static List<VwapResult> CalcVwap(
        this List<QuoteD> quotes,
        DateTime startDate)
    {
        // If caller passed default(DateTime) treat as unspecified and start at first quote
        if (startDate == default && quotes.Count > 0)
        {
            startDate = quotes[0].Timestamp;
        }

        // check parameter arguments
        Validate(quotes, startDate);

        // initialize
        int length = quotes.Count;
        List<VwapResult> results = new(length);

        if (length == 0)
        {
            return results;
        }

        double? cumVolume = 0;
        double? cumVolumeTp = 0;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            QuoteD q = quotes[i];

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
