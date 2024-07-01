namespace Skender.Stock.Indicators;

// RENKO CHART - STANDARD (API)
public static partial class Renko
{
    // SERIES, from TQuote
    public static IEnumerable<RenkoResult> GetRenko<TQuote>(
        this IEnumerable<TQuote> quotes,
        decimal brickSize,
        EndType endType = EndType.Close)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcRenko(brickSize, endType);

    // OBSERVER, from Quote Provider
    public static Renko<TQuote> ToRenko<TQuote>(
        this IQuoteProvider<TQuote> quoteProvider,
        decimal brickSize,
        EndType endType = EndType.Close)
        where TQuote : struct, IQuote
        => new(quoteProvider, brickSize, endType);

}
