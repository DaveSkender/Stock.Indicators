using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ICHIMOKU CLOUD
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<IchimokuResult> GetIchimoku<TQuote>(
            this IEnumerable<TQuote> quotes,
            int tenkanPeriods = 9,
            int kijunPeriods = 26,
            int senkouBPeriods = 52)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateIchimoku(quotes, tenkanPeriods, kijunPeriods, senkouBPeriods);

            // initialize
            List<IchimokuResult> results = new(quotesList.Count);

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                IchimokuResult result = new()
                {
                    Date = q.Date
                };

                // tenkan-sen conversion line
                CalcIchimokuTenkanSen(index, quotesList, result, tenkanPeriods);

                // kijun-sen base line
                CalcIchimokuKijunSen(index, quotesList, result, kijunPeriods);

                // senkou span A
                if (index >= 2 * kijunPeriods)
                {
                    IchimokuResult skq = results[index - kijunPeriods - 1];

                    if (skq != null && skq.TenkanSen != null && skq.KijunSen != null)
                    {
                        result.SenkouSpanA = (skq.TenkanSen + skq.KijunSen) / 2;
                    }
                }

                // senkou span B
                CalcIchimokuSenkouB(index, quotesList, result, kijunPeriods, senkouBPeriods);

                // chikou line
                if (index + kijunPeriods <= quotesList.Count)
                {
                    result.ChikouSpan = quotesList[index + kijunPeriods - 1].Close;
                }
                results.Add(result);
            }

            return results;
        }


        private static void CalcIchimokuTenkanSen<TQuote>(
            int index, List<TQuote> quotesList, IchimokuResult result, int tenkanPeriods)
            where TQuote : IQuote
        {
            if (index >= tenkanPeriods)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = index - tenkanPeriods; p < index; p++)
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
            int index, List<TQuote> quotesList, IchimokuResult result, int kijunPeriods)
            where TQuote : IQuote
        {
            if (index >= kijunPeriods)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = index - kijunPeriods; p < index; p++)
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
            int index, List<TQuote> quotesList, IchimokuResult result,
            int kijunPeriods, int senkouBPeriods)
            where TQuote : IQuote
        {
            if (index >= kijunPeriods + senkouBPeriods)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = index - kijunPeriods - senkouBPeriods;
                    p < index - kijunPeriods; p++)
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
            int senkouBPeriods)
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

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(tenkanPeriods, Math.Max(kijunPeriods, senkouBPeriods));
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
