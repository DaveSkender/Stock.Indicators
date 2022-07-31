namespace Skender.Stock.Indicators;

// WILLIAMS FRACTAL (API)
public static partial class Indicator
{
    /// <include file='./info.xml' path='info/type[@name="standard"]/*' />
    ///
    public static IEnumerable<FractalResult> GetFractal<TQuote>(
        this IEnumerable<TQuote> quotes,
        int windowSpan = 2,
        EndType endType = EndType.HighLow)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcFractal(windowSpan, windowSpan, endType);

    // more configurable version (undocumented)
    /// <include file='./info.xml' path='info/type[@name="alt"]/*' />
    ///
    public static IEnumerable<FractalResult> GetFractal<TQuote>(
        this IEnumerable<TQuote> quotes,
        int leftSpan,
        int rightSpan,
        EndType endType = EndType.HighLow)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcFractal(leftSpan, rightSpan, endType);
}
