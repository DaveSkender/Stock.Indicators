namespace Skender.Stock.Indicators;

// ICHIMOKU CLOUD (SERIES)

public static partial class Ichimoku
{
    public static IReadOnlyList<IchimokuResult> ToIchimoku<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int tenkanPeriods = 9,
        int kijunPeriods = 26,
        int senkouBPeriods = 52)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcIchimoku(
                tenkanPeriods,
                kijunPeriods,
                senkouBPeriods,
                kijunPeriods,
                kijunPeriods);

    public static IReadOnlyList<IchimokuResult> GetIchimoku<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int offsetPeriods)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcIchimoku(
                tenkanPeriods,
                kijunPeriods,
                senkouBPeriods,
                offsetPeriods,
                offsetPeriods);

    public static IReadOnlyList<IchimokuResult> GetIchimoku<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int senkouOffset,
        int chikouOffset)
        where TQuote : IQuote => quotes
            .ToSortedList()
            .CalcIchimoku(
                tenkanPeriods,
                kijunPeriods,
                senkouBPeriods,
                senkouOffset,
                chikouOffset);

    private static List<IchimokuResult> CalcIchimoku<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int senkouOffset,
        int chikouOffset)
        where TQuote : IQuote
    {
        // check parameter arguments
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
