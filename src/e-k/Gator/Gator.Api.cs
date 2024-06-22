namespace Skender.Stock.Indicators;

// GATOR OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from TQuote
    /// <summary>
    /// Gator Oscillator is an expanded view of Williams Alligator.
    /// <para>
    /// See
    /// <see href="https://dotnet.StockIndicators.dev/indicators/Gator/#content?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded"> documentation</see>
    /// for more information.
    /// </para>
    /// </summary>
    /// <typeparam name = "TQuote" > Configurable Quote type.  See Guide for more information.</typeparam>
    /// <param name = "quotes" > Historical price quotes.</param>
    /// <param name="candlePart"></param>
    /// <returns>Time series of Gator values.</returns>
    public static IEnumerable<GatorResult> GetGator<TQuote>(
        this IEnumerable<TQuote> quotes,
        CandlePart candlePart = CandlePart.HL2)
        where TQuote : IQuote
        => quotes
            .Use(candlePart)
            .GetAlligator()
            .ToList()
            .CalcGator();

    // SERIES, from [custom] Alligator
    public static IEnumerable<GatorResult> GetGator(
        this IEnumerable<AlligatorResult> alligator) => alligator
            .ToList()
            .CalcGator();

    // SERIES, from CHAIN
    public static IEnumerable<GatorResult> GetGator<T>(
        this IEnumerable<T> results)
        where T : IReusableResult
        => results
            .GetAlligator()
            .ToList()
            .CalcGator();
}
