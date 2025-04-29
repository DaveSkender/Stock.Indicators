namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Ichimoku Cloud indicator.
/// </summary>
public static partial class Ichimoku
{
    /// <summary>
    /// Converts a list of quotes to Ichimoku Cloud results using default parameters.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quotes, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The list of quotes to transform.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line). Default is 9.</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line). Default is 26.</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B). Default is 52.</param>
    /// <returns>A list of Ichimoku Cloud results.</returns>
    [Series("ICHIMOKU", "Ichimoku Cloud", Category.PriceTrend, ChartType.Overlay)]
    public static IReadOnlyList<IchimokuResult> ToIchimoku<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        [ParamNum<int>("Tenkan Periods", 1, 250, 9)]
        int tenkanPeriods = 9,
        [ParamNum<int>("Kijun Periods", 2, 250, 26)]
        int kijunPeriods = 26,
        [ParamNum<int>("Senkou Periods", 3, 250, 52)]
        int senkouBPeriods = 52)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .ToIchimoku(
                tenkanPeriods,
                kijunPeriods,
                senkouBPeriods,
                kijunPeriods,
                kijunPeriods);

    /// <summary>
    /// Converts a list of quotes to Ichimoku Cloud results with specified parameters.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quotes, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The list of quotes to transform.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="offsetPeriods">The number of periods for the offset.</param>
    /// <returns>A list of Ichimoku Cloud results.</returns>
    [ExcludeFromCatalog]
    public static IReadOnlyList<IchimokuResult> ToIchimoku<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int offsetPeriods)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .ToIchimoku(
                tenkanPeriods,
                kijunPeriods,
                senkouBPeriods,
                offsetPeriods,
                offsetPeriods);

    /// <summary>
    /// Calculates the Ichimoku Cloud indicator.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quotes, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The list of quotes to transform.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="senkouOffset">The number of periods for the Senkou offset.</param>
    /// <param name="chikouOffset">The number of periods for the Chikou offset.</param>
    /// <returns>A list of Ichimoku Cloud results.</returns>
    [ExcludeFromCatalog]
    public static IReadOnlyList<IchimokuResult> ToIchimoku<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int senkouOffset,
        int chikouOffset)
        where TQuote : IQuote
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(
            tenkanPeriods,
            kijunPeriods,
            senkouBPeriods,
            senkouOffset,
            chikouOffset);

        // initialize
        int length = quotes.Count;
        List<IchimokuResult> results = new(length);

        int senkouStartPeriod = Math.Max(
            2 * senkouOffset,
            Math.Max(tenkanPeriods, kijunPeriods)) - 1;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotes[i];

            // tenkan-sen conversion line
            decimal? tenkanSen = CalcIchimokuTenkanSen(
                i, quotes, tenkanPeriods);

            // kijun-sen base line
            decimal? kijunSen = CalcIchimokuKijunSen(
                i, quotes, kijunPeriods);

            // senkou span A
            decimal? senkouSpanA = null;

            if (i >= senkouStartPeriod)
            {
                if (senkouOffset == 0)
                {
                    senkouSpanA = (tenkanSen + kijunSen) / 2;
                }
                else
                {
                    IchimokuResult skq = results[i - senkouOffset];
                    senkouSpanA = (skq.TenkanSen + skq.KijunSen) / 2;
                }
            }

            // senkou span B
            decimal? senkouSpanB = CalcIchimokuSenkouB(
                i, quotes, senkouOffset, senkouBPeriods);

            // chikou line
            decimal? chikouSpan = null;

            if (i + chikouOffset < quotes.Count)
            {
                chikouSpan = quotes[i + chikouOffset].Close;
            }

            results.Add(new(
                Timestamp: q.Timestamp,
                TenkanSen: tenkanSen,
                KijunSen: kijunSen,
                SenkouSpanA: senkouSpanA,
                SenkouSpanB: senkouSpanB,
                ChikouSpan: chikouSpan));
        }

        return results;
    }

    /// <summary>
    /// Calculates the Tenkan-sen (conversion line) for the Ichimoku Cloud indicator.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quotes, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="i">The current index in the quotes list.</param>
    /// <param name="quotes">The list of quotes to transform.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <returns>The Tenkan-sen value.</returns>
    private static decimal? CalcIchimokuTenkanSen<TQuote>(
        int i, IReadOnlyList<TQuote> quotes, int tenkanPeriods)
        where TQuote : IQuote
    {
        if (i < tenkanPeriods - 1)
        {
            return null;
        }

        decimal max = 0;
        decimal min = decimal.MaxValue;

        for (int p = i - tenkanPeriods + 1; p <= i; p++)
        {
            TQuote d = quotes[p];

            if (d.High > max)
            {
                max = d.High;
            }

            if (d.Low < min)
            {
                min = d.Low;
            }
        }

        return min == decimal.MaxValue ? null : (min + max) / 2;

    }

    /// <summary>
    /// Calculates the Kijun-sen (base line) for the Ichimoku Cloud indicator.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quotes, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="i">The current index in the quotes list.</param>
    /// <param name="quotes">The list of quotes to transform.</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <returns>The Kijun-sen value.</returns>
    private static decimal? CalcIchimokuKijunSen<TQuote>(
        int i,
        IReadOnlyList<TQuote> quotes,
        int kijunPeriods)
        where TQuote : IQuote
    {
        if (i < kijunPeriods - 1)
        {
            return null;
        }

        decimal max = 0;
        decimal min = decimal.MaxValue;

        for (int p = i - kijunPeriods + 1; p <= i; p++)
        {
            TQuote d = quotes[p];

            if (d.High > max)
            {
                max = d.High;
            }

            if (d.Low < min)
            {
                min = d.Low;
            }
        }

        return min == decimal.MaxValue ? null : (min + max) / 2;
    }

    /// <summary>
    /// Calculates the Senkou Span B (leading span B) for the Ichimoku Cloud indicator.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quotes, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="i">The current index in the quotes list.</param>
    /// <param name="quotes">The list of quotes to transform.</param>
    /// <param name="senkouOffset">The number of periods for the Senkou offset.</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <returns>The Senkou Span B value.</returns>
    private static decimal? CalcIchimokuSenkouB<TQuote>(
        int i,
        IReadOnlyList<TQuote> quotes,
        int senkouOffset,
        int senkouBPeriods)
        where TQuote : IQuote
    {
        if (i < senkouOffset + senkouBPeriods - 1)
        {
            return null;
        }

        decimal max = 0;
        decimal min = decimal.MaxValue;

        for (int p = i - senkouOffset - senkouBPeriods + 1;
             p <= i - senkouOffset; p++)
        {
            TQuote d = quotes[p];

            if (d.High > max)
            {
                max = d.High;
            }

            if (d.Low < min)
            {
                min = d.Low;
            }
        }

        return min == decimal.MaxValue ? null : (min + max) / 2;
    }
}
