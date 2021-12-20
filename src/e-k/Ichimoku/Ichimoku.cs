using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ICHIMOKU CLOUD
        /// <include file='./info.xml' path='indicator/type[@name="Standard"]/*' />
        /// 
        public static IEnumerable<IchimokuResult> GetIchimoku<TQuote>(
            this IEnumerable<TQuote> quotes,
            int tenkanPeriods = 9,
            int kijunPeriods = 26,
            int senkouBPeriods = 52)
            where TQuote : IQuote
        {
            return quotes.GetIchimoku(
                tenkanPeriods,
                kijunPeriods,
                senkouBPeriods,
                kijunPeriods,
                kijunPeriods);
        }

        /// <include file='./info.xml' path='indicator/type[@name="Extended"]/*' />
        /// 
        public static IEnumerable<IchimokuResult> GetIchimoku<TQuote>(
            this IEnumerable<TQuote> quotes,
            int tenkanPeriods,
            int kijunPeriods,
            int senkouBPeriods,
            int offsetPeriods)
            where TQuote : IQuote
        {
            return quotes.GetIchimoku(
                tenkanPeriods,
                kijunPeriods,
                senkouBPeriods,
                offsetPeriods,
                offsetPeriods);
        }

        /// <include file='./info.xml' path='indicator/type[@name="Full"]/*' />
        /// 
        public static IEnumerable<IchimokuResult> GetIchimoku<TQuote>(
        this IEnumerable<TQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int senkouOffset,
        int chikouOffset)
        where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.SortToList();

            // check parameter arguments
            ValidateIchimoku(
                quotes,
                tenkanPeriods,
                kijunPeriods,
                senkouBPeriods,
                senkouOffset,
                chikouOffset);

            // initialize
            List<IchimokuResult> results = new(quotesList.Count);
            int senkouStartPeriod = Math.Max(2 * senkouOffset,
                                    Math.Max(tenkanPeriods, kijunPeriods)) - 1;

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];

                IchimokuResult result = new()
                {
                    Date = q.Date
                };
                results.Add(result);

                // tenkan-sen conversion line
                CalcIchimokuTenkanSen(i, quotesList, result, tenkanPeriods);

                // kijun-sen base line
                CalcIchimokuKijunSen(i, quotesList, result, kijunPeriods);

                // senkou span A
                if (i >= senkouStartPeriod)
                {
                    IchimokuResult skq = results[i - senkouOffset];

                    if (skq != null && skq.TenkanSen != null && skq.KijunSen != null)
                    {
                        result.SenkouSpanA = (skq.TenkanSen + skq.KijunSen) / 2;
                    }
                }

                // senkou span B
                CalcIchimokuSenkouB(i, quotesList, result, senkouOffset, senkouBPeriods);

                // chikou line
                if (i + chikouOffset < quotesList.Count)
                {
                    result.ChikouSpan = quotesList[i + chikouOffset].Close;
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

                result.TenkanSen = (min + max) / 2;
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

                result.KijunSen = (min + max) / 2;
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

                result.SenkouSpanB = (min + max) / 2;
            }
        }


        private static void ValidateIchimoku<TQuote>(
            IEnumerable<TQuote> quotes,
            int tenkanPeriods,
            int kijunPeriods,
            int senkouBPeriods,
            int senkouOffset,
            int chikouOffset)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (tenkanPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tenkanPeriods), tenkanPeriods,
                    "Tenkan periods must be greater than 0 for Ichimoku Cloud.");
            }

            if (kijunPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(kijunPeriods), kijunPeriods,
                    "Kijun periods must be greater than 0 for Ichimoku Cloud.");
            }

            if (senkouBPeriods <= kijunPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(senkouBPeriods), senkouBPeriods,
                    "Senkou B periods must be greater than Kijun periods for Ichimoku Cloud.");
            }

            if (senkouOffset < 0 || chikouOffset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(senkouOffset), senkouOffset,
                    "Senkou and Chikou offset periods must be non-negative for Ichimoku Cloud.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(tenkanPeriods,
                             Math.Max(kijunPeriods,
                             Math.Max(senkouBPeriods,
                             Math.Max(senkouOffset, chikouOffset))));
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Ichimoku Cloud.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
