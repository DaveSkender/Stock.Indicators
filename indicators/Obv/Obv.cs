using System;
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
            IEnumerable<TQuote> history,
            int? smaPeriod = null)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateObv(history, smaPeriod);

            // initialize
            List<ObvResult> results = new List<ObvResult>(historyList.Count);

            decimal? prevClose = null;
            decimal obv = 0;

            // roll through history
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

                ObvResult result = new ObvResult
                {
                    Date = h.Date,
                    Obv = obv
                };
                results.Add(result);

                prevClose = h.Close;

                // optional SMA
                if (smaPeriod != null && index > smaPeriod)
                {
                    decimal sumSma = 0m;
                    for (int p = index - (int)smaPeriod; p < index; p++)
                    {
                        sumSma += results[p].Obv;
                    }

                    result.ObvSma = sumSma / smaPeriod;
                }
            }

            return results;
        }


        private static void ValidateObv<TQuote>(
            IEnumerable<TQuote> history,
            int? smaPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (smaPeriod is not null and <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriod), smaPeriod,
                    "SMA period must be greater than 0 for OBV.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for On-balance Volume.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
