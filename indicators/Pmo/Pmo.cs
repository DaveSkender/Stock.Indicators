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
            decimal smoothingMultiplier = 2m / timePeriod;
            decimal smoothingConstant = 2m / smoothingPeriod;
            decimal signalConstant = 2m / (signalPeriod + 1);
            decimal? lastRocEma = null;
            decimal? lastPmo = null;
            decimal? lastSignal = null;

            // get ROC EMA variant
            int startIndex = timePeriod + 1;

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
                    decimal sumRoc = 0;
                    for (int p = r.Index - timePeriod; p < r.Index; p++)
                    {
                        RocResult d = roc[p];
                        sumRoc += (decimal)d.Roc;
                    }
                    result.RocEma = sumRoc / timePeriod;
                }

                lastRocEma = result.RocEma;
                result.RocEma *= 10;
                results.Add(result);
            }

            // calculate PMO
            startIndex = timePeriod + smoothingPeriod;

            for (int i = startIndex - 1; i < results.Count; i++)
            {
                PmoResult pr = results[i];

                if (pr.Index > startIndex)
                {
                    pr.Pmo = (pr.RocEma - lastPmo) * smoothingConstant + lastPmo;
                }
                else if (pr.Index == startIndex)
                {
                    decimal sumRocEma = 0;
                    for (int p = pr.Index - smoothingPeriod; p < pr.Index; p++)
                    {
                        PmoResult d = results[p];
                        sumRocEma += (decimal)d.RocEma;
                    }
                    pr.Pmo = sumRocEma / smoothingPeriod;
                }

                lastPmo = pr.Pmo;
            }

            // add Signal
            startIndex = timePeriod + smoothingPeriod + signalPeriod - 1;

            for (int i = startIndex - 1; i < results.Count; i++)
            {
                PmoResult pr = results[i];

                if (pr.Index > startIndex)
                {
                    pr.Signal = (pr.Pmo - lastSignal) * signalConstant + lastSignal;
                }
                else if (pr.Index == startIndex)
                {
                    decimal sumPmo = 0;
                    for (int p = pr.Index - signalPeriod; p < pr.Index; p++)
                    {
                        PmoResult d = results[p];
                        sumPmo += (decimal)d.Pmo;
                    }
                    pr.Signal = sumPmo / signalPeriod;
                }

                lastSignal = pr.Signal;
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
