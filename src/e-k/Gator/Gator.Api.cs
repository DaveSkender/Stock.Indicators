namespace Skender.Stock.Indicators;

// GATOR OSCILLATOR (API)
public static partial class Indicator
{
    // SERIES, from CHAIN (or QUOTES)
    /// <summary>
    /// Gator Oscillator is an expanded view of Williams Alligator.
    /// <para>
    /// See
    /// <see href="https://dotnet.StockIndicators.dev/indicators/Gator/#content?utm_source=library&amp;utm_medium=inline-help&amp;utm_campaign=embedded"> documentation</see>
    /// for more information.
    /// </para>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <typeparam name="T">"T" should be IReusableResult or IQuote type</typeparam>
    /// <param name = "source" > Historical price quotes.</param>
    /// <returns>Time series of Gator values.</returns>

    // See Alligator API for explanation of unusual setup.
    public static IEnumerable<GatorResult> GetGator<T>(
        this IEnumerable<T> source)
        where T : struct, IReusableResult
        => source
            .GetAlligator()
            .ToList()
            .CalcGator();



    // SERIES, from [custom] Alligator
    public static IEnumerable<GatorResult> GetGator(
        this IEnumerable<AlligatorResult> alligator)
        => alligator
            .ToList()
            .CalcGator();
}
