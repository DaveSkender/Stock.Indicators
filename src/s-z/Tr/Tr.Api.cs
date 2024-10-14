namespace Skender.Stock.Indicators;

// TRUE RANGE (API)

public static partial class Tr
{
    // SERIES, from TQuote
    /// <include file='./info.xml' path='info/type[@name="standard"]/*' />
    ///
    public static IReadOnlyList<TrResult> ToTr<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote => quotes
            .ToQuoteDList()
            .CalcTr();

    // OBSERVER, from Quote Provider
    public static TrHub<TIn> ToTr<TIn>(
        this IQuoteProvider<TIn> quoteProvider)
        where TIn : IQuote
        => new(quoteProvider);
}
