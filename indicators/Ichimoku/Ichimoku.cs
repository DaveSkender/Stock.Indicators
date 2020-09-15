using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ICHIMOKU CLOUD
        public static IEnumerable<IchimokuResult> GetIchimoku(IEnumerable<Quote> history,
            int signalPeriod = 9, int shortSpanPeriod = 26, int longSpanPeriod = 52)
        {

            // clean quotes
            List<Quote> historyList = Cleaners.PrepareHistory(history).ToList();

            // check parameters
            ValidateIchimoku(history, signalPeriod, shortSpanPeriod, longSpanPeriod);

            // initialize
            List<IchimokuResult> results = new List<IchimokuResult>();

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];

                IchimokuResult result = new IchimokuResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                // tenkan-sen conversion line
                CalcIchimokuTenkanSen(historyList, result, h, signalPeriod);

                // kijun-sen base line
                CalcIchimokuKijunSen(historyList, result, h, shortSpanPeriod);

                // senkou span A
                if (h.Index >= 2 * shortSpanPeriod)
                {
                    IchimokuResult skq = results[(int)h.Index - shortSpanPeriod - 1];

                    if (skq != null && skq.TenkanSen != null && skq.KijunSen != null)
                    {
                        result.SenkouSpanA = (skq.TenkanSen + skq.KijunSen) / 2;
                    }
                }

                // senkou span B
                CalcIchimokuSenkouB(historyList, result, h, shortSpanPeriod, longSpanPeriod);

                // chikou line
                if (h.Index + shortSpanPeriod <= historyList.Count)
                {
                    result.ChikouSpan = historyList[(int)h.Index + shortSpanPeriod - 1].Close;
                }
                results.Add(result);
            }

            return results;
        }


        private static void CalcIchimokuTenkanSen(
            List<Quote> historyList,
            IchimokuResult result,
            Quote h, int signalPeriod)
        {
            if (h.Index >= signalPeriod)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = (int)h.Index - signalPeriod; p < h.Index; p++)
                {
                    Quote d = historyList[p];

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


        private static void CalcIchimokuKijunSen(
            List<Quote> historyList, 
            IchimokuResult result, 
            Quote h, int shortSpanPeriod)
        {
            if (h.Index >= shortSpanPeriod)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = (int)h.Index - shortSpanPeriod; p < h.Index; p++)
                {
                    Quote d = historyList[p];

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


        private static void CalcIchimokuSenkouB(
            List<Quote> historyList,
            IchimokuResult result,
            Quote h, int shortSpanPeriod, int longSpanPeriod)
        {
            if (h.Index >= shortSpanPeriod + longSpanPeriod)
            {
                decimal max = 0;
                decimal min = decimal.MaxValue;

                for (int p = (int)h.Index - shortSpanPeriod - longSpanPeriod;
                    p < h.Index - shortSpanPeriod; p++)
                {
                    Quote d = historyList[p];

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


        private static void ValidateIchimoku(IEnumerable<Quote> history,
            int signalPeriod, int shortSpanPeriod, int longSpanPeriod)
        {

            // check parameters
            if (signalPeriod <= 0)
            {
                throw new BadParameterException("Signal period must be greater than 0 for ICHIMOKU.");
            }

            if (shortSpanPeriod <= 0)
            {
                throw new BadParameterException("Short span period must be greater than 0 for ICHIMOKU.");
            }

            if (longSpanPeriod <= shortSpanPeriod)
            {
                throw new BadParameterException("Long span period must be greater than small span period for ICHIMOKU.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(signalPeriod, Math.Max(shortSpanPeriod, longSpanPeriod));
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for ICHIMOKU.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
