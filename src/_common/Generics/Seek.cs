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
            .FirstOrDefault(x => x.TickDate == lookupDate);

    // FIND INDEX by DATE
    /// <include file='./info.xml' path='info/type[@name="FindIndex"]/*' />
    ///
    public static int FindIndex<TSeries>(
        this List<TSeries> series,
        DateTime lookupDate)
        where TSeries : ISeries => series == null
            ? -1
            : series.FindIndex(x => x.TickDate == lookupDate);

    // FIND INDEX by DATE
    /// <include file='./info.xml' path='info/type[@name="FindIndexChain"]/*' />
    ///
    public static int FindIndex(
        this List<(DateTime TickDate, double Value)> tuple,
        DateTime lookupDate)
        => tuple == null
            ? -1
            : tuple.FindIndex(x => x.TickDate == lookupDate);

}
