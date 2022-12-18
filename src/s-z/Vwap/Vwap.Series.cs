namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED AVERAGE PRICE (SERIES)
public static partial class Indicator
{
    internal static List<VwapResult> CalcVwap(
        this List<QuoteD> qdList,
        DateTime? startDate = null)
    {
        // check parameter arguments
        ValidateVwap(qdList, startDate);

        // initialize
        int length = qdList.Count;
        List<VwapResult> results = new(length);

        if (length == 0)
        {
            return results;
        }

        startDate = (startDate == null) ? qdList[0].Date : startDate;

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

            VwapResult r = new(q.Date);
            results.Add(r);

            if (q.Date >= startDate)
            {
                cumVolume += v;
                cumVolumeTP += v * (h + l + c) / 3;

                r.Vwap = (cumVolume != 0) ? (cumVolumeTP / cumVolume) : null;
            }
        }

        return results;
    }

    // parameter validation
    private static void ValidateVwap(
        List<QuoteD> quotesList,
        DateTime? startDate)
    {
        // nothing to do for 0 length
        if (quotesList.Count == 0)
        {
            return;
        }

        // check parameter arguments (intentionally after quotes check)
        if (startDate < quotesList[0].Date)
        {
            throw new ArgumentOutOfRangeException(nameof(startDate), startDate,
                "Start Date must be within the quotes range for VWAP.");
        }
    }
}
