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
            int signalPeriods = 9,
            int shortSpanPeriods = 26,
            int longSpanPeriods = 52)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateIchimoku(quotes, signalPeriods, shortSpanPeriods, longSpanPeriods);

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
                CalcIchimokuTenkanSen(index, quotesList, result, signalPeriods);

                // kijun-sen base line
                CalcIchimokuKijunSen(index, quotesList, result, shortSpanPeriods);

                // senkou span A
                if (index >= 2 * shortSpanPeriods)
                {
                    IchimokuResult skq = results[index - shortSpanPeriods - 1];

                    if (skq != null && skq.TenkanSen != null && skq.KijunSen != null)
                    {
                        result.SenkouSpanA = (skq.TenkanSen + skq.KijunSen) / 2;
                    }
                }

                // senkou span B
                CalcIchimokuSenkouB(index, quotesList, result, shortSpanPeriods, longSpanPeriods);

                // chikou line
                if (index + shortSpanPeriods <= quotesList.Count)
                {
                    result.ChikouSpan = quotesList[index + shortSpanPeriods - 1].Close;
                }
                results.Add(result);
            }

            return results;
        }


        private static void CalcIchimokuTenkanSen<TQuote>(
            int index, List<TQuote> quotesList, IchimokuResult result, int signalPeriods)
            where TQuote : IQuote
        {
            if (index >= signalPeriods)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = index - signalPeriods; p < index; p++)
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
            int index, List<TQuote> quotesList, IchimokuResult result, int shortSpanPeriods)
            where TQuote : IQuote
        {
            if (index >= shortSpanPeriods)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = index - shortSpanPeriods; p < index; p++)
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
            int shortSpanPeriods, int longSpanPeriods)
            where TQuote : IQuote
        {
            if (index >= shortSpanPeriods + longSpanPeriods)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = index - shortSpanPeriods - longSpanPeriods;
                    p < index - shortSpanPeriods; p++)
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
            int signalPeriods,
            int shortSpanPeriods,
            int longSpanPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (signalPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                    "Signal periods must be greater than 0 for ICHIMOKU.");
            }

            if (shortSpanPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(shortSpanPeriods), shortSpanPeriods,
                    "Short span periods must be greater than 0 for ICHIMOKU.");
            }

            if (longSpanPeriods <= shortSpanPeriods)
            {
                throw new ArgumentOutOfRangeException(nameof(longSpanPeriods), longSpanPeriods,
                    "Long span periods must be greater than small span period for ICHIMOKU.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(signalPeriods, Math.Max(shortSpanPeriods, longSpanPeriods));
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for ICHIMOKU.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
