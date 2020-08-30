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
            Cleaners.PrepareHistory(history);

            // check parameters
            ValidateIchimoku(history, signalPeriod, shortSpanPeriod, longSpanPeriod);

            // initialize
            List<IchimokuResult> results = new List<IchimokuResult>();

            // roll through history
            foreach (Quote h in history)
            {

                IchimokuResult result = new IchimokuResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                // tenkan-sen conversion line
                if (h.Index >= signalPeriod)
                {

                    List<Quote> tenkanPeriod = history
                        .Where(x => x.Index > (h.Index - signalPeriod) && x.Index <= h.Index)
                        .ToList();

                    decimal max = tenkanPeriod.Select(x => x.High).Max();
                    decimal min = tenkanPeriod.Select(x => x.Low).Min();

                    result.TenkanSen = (min + max) / 2;
                }

                // kijun-sen base line
                if (h.Index >= shortSpanPeriod)
                {
                    List<Quote> kijunPeriod = history
                        .Where(x => x.Index > (h.Index - shortSpanPeriod) && x.Index <= h.Index)
                        .ToList();

                    decimal max = kijunPeriod.Select(x => x.High).Max();
                    decimal min = kijunPeriod.Select(x => x.Low).Min();

                    result.KijunSen = (min + max) / 2;
                }

                // senkou span A
                if (h.Index >= 2 * shortSpanPeriod)
                {

                    IchimokuResult skq = results
                        .Where(x => x.Index == h.Index - shortSpanPeriod)
                        .FirstOrDefault();

                    if (skq != null && skq.TenkanSen != null && skq.KijunSen != null)
                    {
                        result.SenkouSpanA = (skq.TenkanSen + skq.KijunSen) / 2;
                    }
                }

                // senkou span B
                if (h.Index >= shortSpanPeriod + longSpanPeriod)
                {
                    List<Quote> senkauPeriod = history
                        .Where(x => x.Index > (h.Index - shortSpanPeriod - longSpanPeriod)
                            && x.Index <= h.Index - shortSpanPeriod)
                        .ToList();

                    decimal max = senkauPeriod.Select(x => x.High).Max();
                    decimal min = senkauPeriod.Select(x => x.Low).Min();

                    result.SenkauSpanB = (min + max) / 2;
                }

                // chikou line
                Quote chikouQuote = history
                    .Where(x => x.Index == h.Index + shortSpanPeriod)
                    .FirstOrDefault();

                if (chikouQuote != null)
                {
                    result.ChikouSpan = chikouQuote.Close;
                }

                results.Add(result);
            }

            return results;
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
                        string.Format(cultureProvider,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
