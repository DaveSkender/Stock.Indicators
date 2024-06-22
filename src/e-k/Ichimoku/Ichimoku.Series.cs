namespace Skender.Stock.Indicators;

// ICHIMOKU CLOUD (SERIES)

public static partial class Indicator
{
    internal static List<IchimokuResult> CalcIchimoku<TQuote>(
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

        // roll through quotes
        for (int i = 0; i < length; i++)
        {
            TQuote q = quotesList[i];

            IchimokuResult r = new() { Timestamp = q.Timestamp };
            results.Add(r);

            // tenkan-sen conversion line
            CalcIchimokuTenkanSen(i, quotesList, r, tenkanPeriods);

            // kijun-sen base line
            CalcIchimokuKijunSen(i, quotesList, r, kijunPeriods);

            // senkou span A
            if (i >= senkouStartPeriod)
            {
                IchimokuResult skq = results[i - senkouOffset];

                if (skq.TenkanSen != null && skq.KijunSen != null)
                {
                    r.SenkouSpanA = (skq.TenkanSen + skq.KijunSen) / 2;
                }
            }

            // senkou span B
            CalcIchimokuSenkouB(i, quotesList, r, senkouOffset, senkouBPeriods);

            // chikou line
            if (i + chikouOffset < quotesList.Count)
            {
                r.ChikouSpan = quotesList[i + chikouOffset].Close;
            }
        }

        return results;
    }

    private static void CalcIchimokuTenkanSen<TQuote>(
        int i, List<TQuote> quotesList, IchimokuResult result, int tenkanPeriods)
        where TQuote : IQuote
    {
        if (i >= tenkanPeriods - 1)
        {
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

            result.TenkanSen = (min == decimal.MaxValue) ? null : (min + max) / 2;
        }
    }

    private static void CalcIchimokuKijunSen<TQuote>(
        int i,
        List<TQuote> quotesList,
        IchimokuResult result,
        int kijunPeriods)
        where TQuote : IQuote
    {
        if (i >= kijunPeriods - 1)
        {
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

            result.KijunSen = (min == decimal.MaxValue) ? null : (min + max) / 2;
        }
    }

    private static void CalcIchimokuSenkouB<TQuote>(
        int i,
        List<TQuote> quotesList,
        IchimokuResult result,
        int senkouOffset,
        int senkouBPeriods)
        where TQuote : IQuote
    {
        if (i >= senkouOffset + senkouBPeriods - 1)
        {
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

            result.SenkouSpanB = (min == decimal.MaxValue) ? null : (min + max) / 2;
        }
    }
}
