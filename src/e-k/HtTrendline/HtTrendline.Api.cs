namespace Skender.Stock.Indicators;

// HILBERT TRANSFORM - INSTANTANEOUS TRENDLINE (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <summary>
    /// Hilbert Transform Instantaneous Trendline(HTL) is a 5-period trendline of high/low price that uses signal processing to reduce noise.
    /// <para>
    /// See
    /// <see href="https://dotnet.StockIndicators.dev/indicators/HtTrendline/#content?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded"> documentation</see>
    /// for more information.
    /// </para>
    /// </summary>
    /// <typeparam name="TQuote">Configurable Quote type.  See Guide for more information.</typeparam>
    /// <param name="quotes">Historical price quotes.</param>
    /// <param name="candlePart">Optional.  Default is HL2.</param>
    /// <returns>Time series of HTL values and smoothed price.</returns>
    public static IEnumerable<HtlResult> GetHtTrendline<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart = CandlePart.HL2)
        where TQuote : IQuote => quotes
            .Use(candlePart)
            .ToList()
            .CalcHtTrendline();

    // SERIES, from CHAIN
    public static IEnumerable<HtlResult> GetHtTrendline<T>(
        this IEnumerable<T> results)
        where T : IReusable
        => results
            .ToSortedList()
            .CalcHtTrendline();
}
