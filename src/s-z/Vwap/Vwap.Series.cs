namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED AVERAGE PRICE (SERIES)

public static partial class Indicator
{
    internal static List<VwapResult> CalcVwap(
        this List<QuoteD> qdList,
        DateTime? startDate = null)
    {
        // check parameter arguments
        Vwap.Validate(qdList, startDate);

        // initialize
        int length = qdList.Count;
        List<VwapResult> results = new(length);

        if (length == 0)
        {
            return results;
        }

        startDate ??= qdList[0].Timestamp;

        double? cumVolume = 0;
        double? cumVolumeTP = 0;

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            QuoteD q = qdList[i];
            double? v = q.Volume;
            double? h = q.High;
            double? l = q.Low;
            double? c = q.Close;

            VwapResult r = new() { Timestamp = q.Timestamp };
            results.Add(r);

            if (q.Timestamp >= startDate)
            {
                cumVolume += v;
                cumVolumeTP += v * (h + l + c) / 3;

                r.Vwap = (cumVolume != 0) ? (cumVolumeTP / cumVolume) : null;
            }
        }

        return results;
    }
}
