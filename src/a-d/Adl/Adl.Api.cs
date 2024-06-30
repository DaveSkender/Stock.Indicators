namespace Skender.Stock.Indicators;

// ACCUMULATION/DISTRIBUTION LINE (API)

public static partial class Adl
{
    // SERIES, from TQuote
    ///<summary>
    ///Accumulation / Distribution Line(ADL) is a rolling accumulation of Chaikin Money Flow Volume.
    ///<para>
    ///See
    ///<see href="https://dotnet.StockIndicators.dev/indicators/Adl/#content?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded"> documentation</see>
    ///for more information.
    ///</para>
    ///</summary>
    ///<typeparam name = "TQuote" > Configurable Quote type. See Guide for more information.</typeparam>
    ///<param name="quotes">Historical price quotes.</param>
    ///<returns>Time series of ADL values.</returns>
    public static IEnumerable<AdlResult> GetAdl<TQuote>(
        this IEnumerable<TQuote> quotes)
        where TQuote : IQuote
        => quotes
            .ToQuoteD()
            .CalcAdl();

    // OBSERVER, from Quote Provider
    public static Adl<TQuote> ToAdl<TQuote>(
        this IQuoteProvider<TQuote> quoteProvider)
        where TQuote : struct, IQuote
        => new(quoteProvider);
}
