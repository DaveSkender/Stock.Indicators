using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // PRICE MOMENTUM OSCILLATOR (PMO)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<PmoResult> GetPmo<TQuote>(
            IEnumerable<TQuote> history,
            int timePeriod = 35,
            int smoothingPeriod = 20,
            int signalPeriod = 10)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidatePmo(history, timePeriod, smoothingPeriod, signalPeriod);

            // initialize
            List<PmoResult> results = CalcPmoRocEma(history, timePeriod);
            decimal smoothingConstant = 2m / smoothingPeriod;
            decimal? lastPmo = null;

            // calculate PMO
            int startIndex = timePeriod + smoothingPeriod;

            for (int i = startIndex - 1; i < results.Count; i++)
            {
                PmoResult pr = results[i];
                int index = i + 1;

                if (index > startIndex)
                {
                    pr.Pmo = (pr.RocEma - lastPmo) * smoothingConstant + lastPmo;
                }
                else if (index == startIndex)
                {
                    decimal sumRocEma = 0;
                    for (int p = index - smoothingPeriod; p < index; p++)
                    {
                        PmoResult d = results[p];
                        sumRocEma += (decimal)d.RocEma;
                    }
                    pr.Pmo = sumRocEma / smoothingPeriod;
                }

                lastPmo = pr.Pmo;
            }

            // add Signal
            CalcPmoSignal(results, timePeriod, smoothingPeriod, signalPeriod);

            return results;
        }


        private static List<PmoResult> CalcPmoRocEma<TQuote>(
            IEnumerable<TQuote> history,
            int timePeriod)
            where TQuote : IQuote
        {
            // initialize
            decimal smoothingMultiplier = 2m / timePeriod;
            decimal? lastRocEma = null;
            List<RocResult> roc = GetRoc(history, 1).ToList();
            List<PmoResult> results = new List<PmoResult>();

            int startIndex = timePeriod + 1;

            for (int i = 0; i < roc.Count; i++)
            {
                RocResult r = roc[i];
                int index = i + 1;

                PmoResult result = new PmoResult
                {
                    Date = r.Date
                };

                if (index > startIndex)
                {
                    result.RocEma = r.Roc * smoothingMultiplier + lastRocEma * (1 - smoothingMultiplier);
                }
                else if (index == startIndex)
                {
                    decimal sumRoc = 0;
                    for (int p = index - timePeriod; p < index; p++)
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

            return results;
        }


        private static void CalcPmoSignal(
            List<PmoResult> results,
            int timePeriod,
            int smoothingPeriod,
            int signalPeriod)
        {
            decimal signalConstant = 2m / (signalPeriod + 1);
            decimal? lastSignal = null;

            int startIndex = timePeriod + smoothingPeriod + signalPeriod - 1;

            for (int i = startIndex - 1; i < results.Count; i++)
            {
                PmoResult pr = results[i];
                int index = i + 1;

                if (index > startIndex)
                {
                    pr.Signal = (pr.Pmo - lastSignal) * signalConstant + lastSignal;
                }
                else if (index == startIndex)
                {
                    decimal sumPmo = 0;
                    for (int p = index - signalPeriod; p < index; p++)
                    {
                        PmoResult d = results[p];
                        sumPmo += (decimal)d.Pmo;
                    }
                    pr.Signal = sumPmo / signalPeriod;
                }

                lastSignal = pr.Signal;
            }
        }


        private static void ValidatePmo<TQuote>(
            IEnumerable<TQuote> history,
            int timePeriod,
            int smoothingPeriod,
            int signalPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (timePeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(timePeriod), timePeriod,
                    "Time period must be greater than 1 for PMO.");
            }

            if (smoothingPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothingPeriod), smoothingPeriod,
                    "Smoothing period must be greater than 0 for PMO.");
            }

            if (signalPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriod), signalPeriod,
                    "Signal period must be greater than 0 for PMO.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(timePeriod + smoothingPeriod, Math.Max(2 * timePeriod, timePeriod + 100));
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for PMO.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a several smoothing operations, "
                    + "we recommend you use at least {2} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, minHistory + 250);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
