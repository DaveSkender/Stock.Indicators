namespace Skender.Stock.Indicators;

// DOJI (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <summary>
    /// Doji is a single candlestick pattern where open and close price are virtually identical, representing market indecision.
    /// <para>
    /// See
    /// <see href="https://dotnet.StockIndicators.dev/indicators/Doji/#content?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded"> documentation</see>
    /// for more information.
    /// </para>
    /// </summary>
    /// <typeparam name = "TQuote" > Configurable Quote type.  See Guide for more information.</typeparam>
    /// <param name = "quotes" > Historical price quotes.</param>
    /// <param name = "maxPriceChangePercent" > Optional.Maximum absolute percent difference in open and close price.</param>
    /// <returns>Time series of Doji values.</returns>
    /// <exception cref = "ArgumentOutOfRangeException" > Invalid parameter value provided.</exception>
    public static IReadOnlyList<CandleResult> ToDoji<TQuote>(
        this IEnumerable<TQuote> quotes,
        double maxPriceChangePercent = 0.1)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcDoji(maxPriceChangePercent);
}
