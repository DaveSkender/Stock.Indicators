﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ON-BALANCE VOLUME
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<ObvResult> GetObv<TQuote>(
            this IEnumerable<TQuote> quotes,
            int? smaPeriods = null)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> historyList = quotes.Sort();

            // check parameter arguments
            ValidateObv(quotes, smaPeriods);

            // initialize
            List<ObvResult> results = new(historyList.Count);

            decimal? prevClose = null;
            decimal obv = 0;

            // roll through quotes
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                if (prevClose == null || h.Close == prevClose)
                {
                    // no change to OBV
                }
                else if (h.Close > prevClose)
                {
                    obv += h.Volume;
                }
                else if (h.Close < prevClose)
                {
                    obv -= h.Volume;
                }

                ObvResult result = new()
                {
                    Date = h.Date,
                    Obv = obv
                };
                results.Add(result);

                prevClose = h.Close;

                // optional SMA
                if (smaPeriods != null && index > smaPeriods)
                {
                    decimal sumSma = 0m;
                    for (int p = index - (int)smaPeriods; p < index; p++)
                    {
                        sumSma += results[p].Obv;
                    }

                    result.ObvSma = sumSma / smaPeriods;
                }
            }

            return results;
        }


        // parameter validation
        private static void ValidateObv<TQuote>(
            IEnumerable<TQuote> quotes,
            int? smaPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (smaPeriods is not null and <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                    "SMA periods must be greater than 0 for OBV.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for On-balance Volume.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
