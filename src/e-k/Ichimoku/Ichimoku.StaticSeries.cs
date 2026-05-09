namespace Skender.Stock.Indicators;

/// <summary>
/// Ichimoku Cloud indicator.
/// </summary>
public static partial class Ichimoku
{
    /// <summary>
    /// Converts a list of quotes to Ichimoku Cloud results using default parameters.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="tenkanPeriods">Number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">Number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">Number of periods for the Senkou Span B (leading span B). Default is 52.</param>
    /// <returns>A list of Ichimoku Cloud results.</returns>
    public static IReadOnlyList<IchimokuResult> ToIchimoku(
        this IReadOnlyList<IQuote> quotes,
        int tenkanPeriods = 9,
        int kijunPeriods = 26,
        int senkouBPeriods = 52)
        => quotes
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
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="tenkanPeriods">Number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">Number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">Number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="offsetPeriods">Number of periods for the offset.</param>
    /// <returns>A list of Ichimoku Cloud results.</returns>
    public static IReadOnlyList<IchimokuResult> ToIchimoku(
        this IReadOnlyList<IQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int offsetPeriods)
        => quotes
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
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="tenkanPeriods">Number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">Number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">Number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="senkouOffset">Number of periods for the Senkou offset.</param>
    /// <param name="chikouOffset">Number of periods for the Chikou offset.</param>
    /// <returns>A list of Ichimoku Cloud results.</returns>
    public static IReadOnlyList<IchimokuResult> ToIchimoku(
        this IReadOnlyList<IQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int senkouOffset,
        int chikouOffset)
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
            IQuote q = quotes[i];

            // tenkan-sen conversion line
            double? tenkanSen = CalcIchimokuTenkanSen(
                i, quotes, tenkanPeriods);

            // kijun-sen base line
            double? kijunSen = CalcIchimokuKijunSen(
                i, quotes, kijunPeriods);

            // senkou span A
            double? senkouSpanA = null;

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
            double? senkouSpanB = CalcIchimokuSenkouB(
                i, quotes, senkouOffset, senkouBPeriods);

            // chikou line
            double? chikouSpan = null;

            if (i + chikouOffset < quotes.Count)
            {
                chikouSpan = (double)quotes[i + chikouOffset].Close;
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
    /// <param name="i">Current index in the quotes list.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="tenkanPeriods">Number of periods for the Tenkan-sen (conversion line).</param>
    /// <returns>Tenkan-sen value.</returns>
    private static double? CalcIchimokuTenkanSen(
        int i, IReadOnlyList<IQuote> quotes, int tenkanPeriods)
    {
        if (i < tenkanPeriods - 1)
        {
            return null;
        }

        double max = 0;
        double min = double.MaxValue;

        for (int p = i - tenkanPeriods + 1; p <= i; p++)
        {
            IQuote d = quotes[p];

            if ((double)d.High > max)
            {
                max = (double)d.High;
            }

            if ((double)d.Low < min)
            {
                min = (double)d.Low;
            }
        }

        return min == double.MaxValue ? null : (double?)((min + max) / 2d);

    }

    /// <summary>
    /// Calculates the Kijun-sen (base line) for the Ichimoku Cloud indicator.
    /// </summary>
    /// <param name="i">Current index in the quotes list.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="kijunPeriods">Number of periods for the Kijun-sen (base line).</param>
    /// <returns>Kijun-sen value.</returns>
    private static double? CalcIchimokuKijunSen(
        int i,
        IReadOnlyList<IQuote> quotes,
        int kijunPeriods)
    {
        if (i < kijunPeriods - 1)
        {
            return null;
        }

        double max = 0;
        double min = double.MaxValue;

        for (int p = i - kijunPeriods + 1; p <= i; p++)
        {
            IQuote d = quotes[p];

            if ((double)d.High > max)
            {
                max = (double)d.High;
            }

            if ((double)d.Low < min)
            {
                min = (double)d.Low;
            }
        }

        return min == double.MaxValue ? null : (double?)((min + max) / 2d);
    }

    /// <summary>
    /// Calculates the Senkou Span B (leading span B) for the Ichimoku Cloud indicator.
    /// </summary>
    /// <param name="i">Current index in the quotes list.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="senkouOffset">Number of periods for the Senkou offset.</param>
    /// <param name="senkouBPeriods">Number of periods for the Senkou Span B (leading span B).</param>
    /// <returns>Senkou Span B value.</returns>
    private static double? CalcIchimokuSenkouB(
        int i,
        IReadOnlyList<IQuote> quotes,
        int senkouOffset,
        int senkouBPeriods)
    {
        if (i < senkouOffset + senkouBPeriods - 1)
        {
            return null;
        }

        double max = 0;
        double min = double.MaxValue;

        for (int p = i - senkouOffset - senkouBPeriods + 1;
             p <= i - senkouOffset; p++)
        {
            IQuote d = quotes[p];

            if ((double)d.High > max)
            {
                max = (double)d.High;
            }

            if ((double)d.Low < min)
            {
                min = (double)d.Low;
            }
        }

        return min == double.MaxValue ? null : (double?)((min + max) / 2d);
    }
}
