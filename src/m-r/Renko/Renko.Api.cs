namespace Skender.Stock.Indicators;

// RENKO CHART - STANDARD (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/type[@name="standard"]/*' />
    ///
    public static IEnumerable<RenkoResult> GetRenko<TQuote>(
        this IEnumerable<TQuote> quotes,
        decimal brickSize,
        EndType endType = EndType.Close)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcRenko(brickSize, endType);
}
