namespace Skender.Stock.Indicators;

// HILBERT TRANSFORM - INSTANTANEOUS TRENDLINE (API)
public static partial class Indicator
{
    // SERIES, from CHAIN
    /// <summary>
    /// Hilbert Transform Instantaneous Trendline(HTL) is a 5-period trendline of high/low price that uses signal processing to reduce noise.
    /// <para>
    /// See
    /// <see href="https://dotnet.StockIndicators.dev/indicators/HtTrendline/#content?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded"> documentation</see>
    /// for more information.
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// <c>T</c> must be <see cref="IReusable"/> or <see cref="IQuote"/> type
    /// </typeparam>
    /// <param name="source">Time-series values to transform.</param>
    /// <returns>Time series of HTL values and smoothed price.</returns>
    public static IEnumerable<HtlResult> GetHtTrendline<T>(
        this IEnumerable<T> source)
        where T : IReusable
        => source
            .ToSortedList(CandlePart.HL2)
            .CalcHtTrendline();
}
