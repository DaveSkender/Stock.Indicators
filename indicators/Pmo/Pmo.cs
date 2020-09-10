using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RATE OF CHANGE (PMO)
        public static IEnumerable<PmoResult> GetPmo(
            IEnumerable<Quote> history,
            int timePeriod = 35,
            int smoothingPeriod = 20,
            int signalPeriod = 10)
        {

            // clean quotes
            Cleaners.PrepareHistory(history);

            // check parameters
            ValidatePmo(history, timePeriod, smoothingPeriod, signalPeriod);

            // initialize
            List<PmoResult> results = new List<PmoResult>();
            List<RocResult> roc = GetRoc(history, 1).ToList();

            int startIndex = 0;
            decimal smoothingMultiplier = 2m / timePeriod;
            decimal smoothingConstant = 2m / smoothingPeriod;
            decimal signalConstant = 2m / (signalPeriod + 1);
            decimal? lastRocEma = null;
            decimal? lastPmo = null;
            decimal? lastSignal = null;

            // get ROC EMA variant
            startIndex = timePeriod + 1;

            for (int i = 0; i < roc.Count; i++)
            {
                RocResult r = roc[i];

                PmoResult result = new PmoResult
                {
                    Index = r.Index,
                    Date = r.Date
                };

                if (r.Index > startIndex)
                {
                    result.RocEma = r.Roc * smoothingMultiplier + lastRocEma * (1 - smoothingMultiplier);
                }
                else if (r.Index == startIndex)
                {
                    result.RocEma = roc
                        .Where(x => x.Index > r.Index - timePeriod && x.Index <= r.Index)
                        .ToList()
                        .Select(x => x.Roc)
                        .Average();
                }

                lastRocEma = result.RocEma;
                result.RocEma *= 10;
                results.Add(result);
            }

            // calculate PMO
            startIndex = timePeriod + smoothingPeriod;

            for (int i = startIndex - 1; i < results.Count; i++)
            {
                PmoResult p = results[i];

                if (p.Index > startIndex)
                {
                    p.Pmo = (p.RocEma - lastPmo) * smoothingConstant + lastPmo;
                }
                else if (p.Index == startIndex)
                {
                    p.Pmo = results
                        .Where(x => x.Index > p.Index - smoothingPeriod && x.Index <= p.Index)
                        .ToList()
                        .Select(x => x.RocEma)
                        .Average();
                }

                lastPmo = p.Pmo;
            }

            // add Signal
            startIndex = timePeriod + smoothingPeriod + signalPeriod - 1;

            for (int i = startIndex - 1; i < results.Count; i++)
            {
                PmoResult p = results[i];

                if (p.Index > startIndex)
                {
                    p.Signal = (p.Pmo - lastSignal) * signalConstant + lastSignal;
                }
                else if (p.Index == startIndex)
                {
                    p.Signal = results
                        .Where(x => x.Index > p.Index - signalPeriod && x.Index <= p.Index)
                        .ToList()
                        .Select(x => x.Pmo)
                        .Average();
                }

                lastSignal = p.Signal;
            }

            return results;
        }


        private static void ValidatePmo(
            IEnumerable<Quote> history,
            int timePeriod,
            int smoothingPeriod,
            int signalPeriod)
        {

            // check parameters
            if (timePeriod <= 1)
            {
                throw new BadParameterException("Time period must be greater than 1 for PMO.");
            }

            if (smoothingPeriod <= 0)
            {
                throw new BadParameterException("Smoothing period must be greater than 0 for PMO.");
            }

            if (signalPeriod <= 0)
            {
                throw new BadParameterException("Signal period must be greater than 0 for PMO.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = timePeriod + smoothingPeriod;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for PMO.  " +
                       string.Format(englishCulture,
                       "You provided {0} periods of history when at least {1} is required.  "
                         + "Since this uses a several smoothing operations, "
                         + "we recommend you use at least {2} data points prior to the intended "
                         + "usage date for maximum precision.",
                         qtyHistory, minHistory, minHistory + signalPeriod + 250));
            }

        }
    }

}
