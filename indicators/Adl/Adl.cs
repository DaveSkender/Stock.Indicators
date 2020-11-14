using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ACCUMULATION / DISTRIBUTION LINE
        public static IEnumerable<AdlResult> GetAdl(IEnumerable<Quote> history, int? smaPeriod = null)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check parameters
            ValidateAdl(history, smaPeriod);

            // initialize
            List<AdlResult> results = new List<AdlResult>();
            decimal prevAdl = 0;

            // get money flow multiplier
            foreach (Quote h in history)
            {
                decimal mfm = (h.High == h.Low) ? 0 : ((h.Close - h.Low) - (h.High - h.Close)) / (h.High - h.Low);
                decimal mfv = mfm * h.Volume;
                decimal adl = mfv + prevAdl;

                AdlResult result = new AdlResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    MoneyFlowMultiplier = mfm,
                    MoneyFlowVolume = mfv,
                    Adl = adl
                };
                results.Add(result);

                prevAdl = adl;

                // optional SMA
                if (smaPeriod != null && h.Index >= smaPeriod)
                {
                    decimal sumSma = 0m;
                    for (int p = (int)h.Index - (int)smaPeriod; p < h.Index; p++)
                    {
                        sumSma += results[p].Adl;
                    }

                    result.Sma = sumSma / smaPeriod;
                }
            }

            return results;
        }


        private static void ValidateAdl(IEnumerable<Quote> history, int? smaPeriod)
        {

            // check parameters
            if (smaPeriod != null && smaPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriod), smaPeriod,
                    "SMA period must be greater than 0 for ADL.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message =
                    "Insufficient history provided for Accumulation Distribution Line.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }

        }
    }

}
