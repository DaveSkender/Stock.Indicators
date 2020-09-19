using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ON-BALANCE VOLUME
        public static IEnumerable<ObvResult> GetObv(IEnumerable<Quote> history, int? smaPeriod = null)
        {

            // clean quotes
            Cleaners.PrepareHistory(history);

            // check parameters
            ValidateObv(history, smaPeriod);

            // initialize
            List<ObvResult> results = new List<ObvResult>();

            decimal? prevClose = null;
            decimal obv = 0;

            foreach (Quote h in history)
            {
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
                    Index = (int)h.Index,
                    Date = h.Date,
                    Obv = obv
                };
                results.Add(result);

                prevClose = h.Close;

                // optional SMA
                if (smaPeriod != null && h.Index > smaPeriod)
                {
                    decimal sumSma = 0m;
                    for (int p = (int)h.Index - (int)smaPeriod; p < h.Index; p++)
                    {
                        sumSma += results[p].Obv;
                    }

                    result.Sma = sumSma / smaPeriod;
                }
            }

            return results;
        }


        private static void ValidateObv(IEnumerable<Quote> history, int? smaPeriod)
        {

            // check parameters
            if (smaPeriod != null && smaPeriod <= 0)
            {
                throw new BadParameterException("SMA period must be greater than 0 for ADL.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for On-balance Volume.  " +
                        string.Format(englishCulture,
                        "You provided {0} periods of history when at least {1} is required.",
                        qtyHistory, minHistory));
            }

        }
    }

}
