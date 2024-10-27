namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED AVERAGE PRICE (SERIES)

public static partial class Vwap
{
    public static IReadOnlyList<VwapResult> ToVwap<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        DateTime? startDate = null)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcVwap(startDate);

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
