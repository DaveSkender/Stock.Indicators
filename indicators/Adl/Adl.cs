using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ACCUMULATION/DISTRIBUTION LINE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<AdlResult> GetAdl<TQuote>(
            this IEnumerable<TQuote> history,
            int? smaPeriods = null)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateAdl(history, smaPeriods);

            // initialize
            List<AdlResult> results = new(historyList.Count);
            decimal prevAdl = 0;

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                decimal mfm = (h.High == h.Low) ? 0 : ((h.Close - h.Low) - (h.High - h.Close)) / (h.High - h.Low);
                decimal mfv = mfm * h.Volume;
                decimal adl = mfv + prevAdl;

                AdlResult result = new()
                {
                    Date = h.Date,
                    MoneyFlowMultiplier = mfm,
                    MoneyFlowVolume = mfv,
                    Adl = adl
                };
                results.Add(result);

                prevAdl = adl;

                // optional SMA
                if (smaPeriods != null && index >= smaPeriods)
                {
                    decimal sumSma = 0m;
                    for (int p = index - (int)smaPeriods; p < index; p++)
                    {
                        sumSma += results[p].Adl;
                    }

                    result.AdlSma = sumSma / smaPeriods;
                }
            }

            return results;
        }


        // parameter validation
        private static void ValidateAdl<TQuote>(
            IEnumerable<TQuote> history,
            int? smaPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (smaPeriods is not null and <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                    "SMA periods must be greater than 0 for ADL.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message = string.Format(EnglishCulture,
                    "Insufficient history provided for Accumulation/Distribution Line.  " +
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
