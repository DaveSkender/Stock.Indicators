namespace Skender.Stock.Indicators;

// SEEK & FIND in SERIES

public static class Seeking
{
    // FIND by DATE
    /// <include file='./info.xml' path='info/type[@name="Find"]/*' />
    ///
    public static TSeries? Find<TSeries>(
        this IEnumerable<TSeries> series,
        DateTime lookupDate)
        where TSeries : ISeries => series
            .FirstOrDefault(x => x.Date == lookupDate);
}
