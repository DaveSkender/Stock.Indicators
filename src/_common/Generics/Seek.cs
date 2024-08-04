namespace Skender.Stock.Indicators;

// SEEK & FIND in SERIES

public static class Seeking
{
    // FIND SERIES by DATE
    /// <include file='./info.xml' path='info/type[@name="FindSeries"]/*' />
    ///
    public static TSeries? Find<TSeries>(
        this IEnumerable<TSeries> series,
        DateTime lookupDate)
        where TSeries : ISeries => series
            .FirstOrDefault(x => x.Date == lookupDate);

    // FIND INDEX by DATE
    /// <include file='./info.xml' path='info/type[@name="FindIndex"]/*' />
    ///
    public static int FindIndex<TSeries>(
        this List<TSeries> series,
        DateTime lookupDate)
        where TSeries : ISeries => series == null
            ? -1
            : series.FindIndex(x => x.Date == lookupDate);
}
