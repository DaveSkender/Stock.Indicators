namespace Skender.Stock.Indicators;

// ZIG ZAG (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/*' />
    ///
    public static IEnumerable<ZigZagResult> GetZigZag<TQuote>(
        this IEnumerable<TQuote> quotes,
        EndType endType = EndType.Close,
        decimal percentChange = 5)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcZigZag(endType, percentChange);
}
