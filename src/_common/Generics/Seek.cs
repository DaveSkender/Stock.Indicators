namespace Skender.Stock.Indicators;

// SEEK & FIND in SERIES

public static class Seeking
{
    // FIND by DATE
    public static T? Find<T>(
        this IReadOnlyList<T> series,
        DateTime lookupDate)
        where T : ISeries
    {
        ArgumentNullException.ThrowIfNull(series);

        int low = 0;
        int high = series.Count - 1;

        while (low <= high)
        {
            int mid = (low + high) >> 1;
            DateTime midTimestamp = series[mid].Timestamp;

            if (midTimestamp == lookupDate)
            {
                return series[mid]; // found
            }
            else if (midTimestamp < lookupDate)
            {
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }

        // not found
        return default;
    }
}
