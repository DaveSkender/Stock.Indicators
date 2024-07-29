namespace Skender.Stock.Indicators;

// ICHIMOKU CLOUD (SERIES)

public static partial class Indicator
{
    private static List<IchimokuResult> CalcIchimoku<TQuote>(
        this List<TQuote> quotesList,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int senkouOffset,
        int chikouOffset)
        where TQuote : IQuote
    {
        // check parameter arguments
        Ichimoku.Validate(
            tenkanPeriods,
            kijunPeriods,
            senkouBPeriods,
            senkouOffset,
            chikouOffset);

        // initialize
        int length = quotesList.Count;
        List<IchimokuResult> results = new(length);

        int senkouStartPeriod = Math.Max(
            2 * senkouOffset,
            Math.Max(tenkanPeriods, kijunPeriods)) - 1;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotesList[i];

            // tenkan-sen conversion line
            decimal? tenkanSen = CalcIchimokuTenkanSen(
                i, quotesList, tenkanPeriods);

            // kijun-sen base line
            decimal? kijunSen = CalcIchimokuKijunSen(
                i, quotesList, kijunPeriods);

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
                i, quotesList, senkouOffset, senkouBPeriods);

            // chikou line
            decimal? chikouSpan = null;

            if (i + chikouOffset < quotesList.Count)
            {
                chikouSpan = quotesList[i + chikouOffset].Close;
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
        int i, List<TQuote> quotesList, int tenkanPeriods)
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
            TQuote d = quotesList[p];

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
        List<TQuote> quotesList,
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
            TQuote d = quotesList[p];

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
        List<TQuote> quotesList,
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
            TQuote d = quotesList[p];

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
