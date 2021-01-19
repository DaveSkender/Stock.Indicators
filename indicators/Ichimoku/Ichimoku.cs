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
            IEnumerable<TQuote> history,
            int signalPeriod = 9,
            int shortSpanPeriod = 26,
            int longSpanPeriod = 52)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateIchimoku(history, signalPeriod, shortSpanPeriod, longSpanPeriod);

            // initialize
            List<IchimokuResult> results = new List<IchimokuResult>(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                IchimokuResult result = new IchimokuResult
                {
                    Date = h.Date
                };

                // tenkan-sen conversion line
                CalcIchimokuTenkanSen(index, historyList, result, signalPeriod);

                // kijun-sen base line
                CalcIchimokuKijunSen(index, historyList, result, shortSpanPeriod);

                // senkou span A
                if (index >= 2 * shortSpanPeriod)
                {
                    IchimokuResult skq = results[index - shortSpanPeriod - 1];

                    if (skq != null && skq.TenkanSen != null && skq.KijunSen != null)
                    {
                        result.SenkouSpanA = (skq.TenkanSen + skq.KijunSen) / 2;
                    }
                }

                // senkou span B
                CalcIchimokuSenkouB(index, historyList, result, shortSpanPeriod, longSpanPeriod);

                // chikou line
                if (index + shortSpanPeriod <= historyList.Count)
                {
                    result.ChikouSpan = historyList[index + shortSpanPeriod - 1].Close;
                }
                results.Add(result);
            }

            return results;
        }


        private static void CalcIchimokuTenkanSen<TQuote>(
            int index, List<TQuote> historyList, IchimokuResult result, int signalPeriod)
            where TQuote : IQuote
        {
            if (index >= signalPeriod)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = index - signalPeriod; p < index; p++)
                {
                    TQuote d = historyList[p];

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
            int index, List<TQuote> historyList, IchimokuResult result, int shortSpanPeriod)
            where TQuote : IQuote
        {
            if (index >= shortSpanPeriod)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = index - shortSpanPeriod; p < index; p++)
                {
                    TQuote d = historyList[p];

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
            int index, List<TQuote> historyList, IchimokuResult result,
            int shortSpanPeriod, int longSpanPeriod)
            where TQuote : IQuote
        {
            if (index >= shortSpanPeriod + longSpanPeriod)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = index - shortSpanPeriod - longSpanPeriod;
                    p < index - shortSpanPeriod; p++)
                {
                    TQuote d = historyList[p];

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
            IEnumerable<TQuote> history,
            int signalPeriod,
            int shortSpanPeriod,
            int longSpanPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (signalPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriod), signalPeriod,
                    "Signal period must be greater than 0 for ICHIMOKU.");
            }

            if (shortSpanPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(shortSpanPeriod), shortSpanPeriod,
                    "Short span period must be greater than 0 for ICHIMOKU.");
            }

            if (longSpanPeriod <= shortSpanPeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(longSpanPeriod), longSpanPeriod,
                    "Long span period must be greater than small span period for ICHIMOKU.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(signalPeriod, Math.Max(shortSpanPeriod, longSpanPeriod));
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for ICHIMOKU.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
