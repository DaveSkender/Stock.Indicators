using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STOCHASTIC MOMENTUM INDEX
        /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
        /// 
        public static IEnumerable<SmiResult> GetSmi<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int firstSmoothPeriods,
            int secondSmoothPeriods,
            int signalPeriods = 3
            )
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateSmi(
                quotes,
                lookbackPeriods,
                firstSmoothPeriods,
                secondSmoothPeriods,
                signalPeriods);

            // initialize
            int size = quotesList.Count;
            List<SmiResult> results = new(size);

            decimal k1 = 2m / (firstSmoothPeriods + 1m);
            decimal k2 = 2m / (secondSmoothPeriods + 1m);
            decimal kS = 2m / (signalPeriods + 1m);

            decimal lastSmEma1 = 0m;
            decimal lastSmEma2 = 0m;
            decimal lastHlEma1 = 0m;
            decimal lastHlEma2 = 0m;
            decimal lastSignal = 0m;

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                SmiResult r = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
                {
                    decimal HH = decimal.MinValue;
                    decimal LL = decimal.MaxValue;

                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        TQuote x = quotesList[p];

                        if (x.High > HH)
                        {
                            HH = x.High;
                        }

                        if (x.Low < LL)
                        {
                            LL = x.Low;
                        }
                    }

                    decimal sm = q.Close - 0.5m * (HH + LL);
                    decimal hl = HH - LL;

                    // initialize last EMA values
                    if (index == lookbackPeriods)
                    {
                        lastSmEma1 = sm;
                        lastSmEma2 = lastSmEma1;
                        lastHlEma1 = hl;
                        lastHlEma2 = lastHlEma1;
                    }

                    // first smoothing
                    decimal smEma1 = lastSmEma1 + k1 * (sm - lastSmEma1);
                    decimal hlEma1 = lastHlEma1 + k1 * (hl - lastHlEma1);

                    // second smoothing
                    decimal smEma2 = lastSmEma2 + k2 * (smEma1 - lastSmEma2);
                    decimal hlEma2 = lastHlEma2 + k2 * (hlEma1 - lastHlEma2);

                    // stochastic momentum index
                    r.Smi = 100 * (smEma2 / (0.5m * hlEma2));

                    // initialize signal line
                    if (index == lookbackPeriods)
                    {
                        lastSignal = (decimal)r.Smi;
                    }

                    // signal line
                    r.Signal = lastSignal + kS * (r.Smi - lastSignal);

                    // carryover values
                    lastSmEma1 = smEma1;
                    lastSmEma2 = smEma2;
                    lastHlEma1 = hlEma1;
                    lastHlEma2 = hlEma2;
                    lastSignal = (decimal)r.Signal;
                }
                results.Add(r);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<SmiResult> RemoveWarmupPeriods(
            this IEnumerable<SmiResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Smi != null);

            return results.Remove(removePeriods + 2 + 100);
        }


        // parameter validation
        private static void ValidateSmi<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int firstSmoothPeriods,
            int secondSmoothPeriods,
            int signalPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for SMI.");
            }

            if (firstSmoothPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(firstSmoothPeriods), firstSmoothPeriods,
                    "Smoothing periods must be greater than 0 for SMI.");
            }

            if (secondSmoothPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(secondSmoothPeriods), secondSmoothPeriods,
                    "Smoothing periods must be greater than 0 for SMI.");
            }

            if (signalPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalPeriods), signalPeriods,
                    "Signal periods must be greater than 0 for SMI.");
            }


            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Stochastic.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
