using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CHOPPINESS INDEX
        /// <include file='./info.xml' path='indicator/*' />
        ///
        public static IEnumerable<ChopResult> GetChop<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 14)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateChop(quotesList, lookbackPeriods);

            // initialize
            decimal sum;
            decimal high;
            decimal low;
            decimal range;

            int size = quotesList.Count;
            List<ChopResult> results = new(size);
            decimal[] trueHigh = new decimal[size];
            decimal[] trueLow = new decimal[size];
            decimal[] trueRange = new decimal[size];

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                ChopResult r = new()
                {
                    Date = quotesList[i].Date
                };
                results.Add(r);

                if (i > 0)
                {
                    trueHigh[i] = Math.Max(quotesList[i].High, quotesList[i - 1].Close);
                    trueLow[i] = Math.Min(quotesList[i].Low, quotesList[i - 1].Close);
                    trueRange[i] = trueHigh[i] - trueLow[i];

                    // calculate CHOP

                    if (i >= lookbackPeriods)
                    {
                        // reset measurements
                        sum = trueRange[i];
                        high = trueHigh[i];
                        low = trueLow[i];

                        // iterate over lookback window
                        for (int j = 1; j < lookbackPeriods; j++)
                        {
                            sum += trueRange[i - j];
                            high = Math.Max(high, trueHigh[i - j]);
                            low = Math.Min(low, trueLow[i - j]);
                        }

                        range = high - low;

                        // calculate CHOP
                        if (range != 0)
                        {
                            r.Chop = (decimal)(100 * (Math.Log((double)(sum / range)) / Math.Log(lookbackPeriods)));
                        }
                    }
                }
            }
            return results;
        }


        // remove recommended periods
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<ChopResult> RemoveWarmupPeriods(
            this IEnumerable<ChopResult> results)
        {
            int removePeriods = results
               .ToList()
               .FindIndex(x => x.Chop != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateChop<TQuote>(
            List<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote

        {
            // check parameter arguments
            if (lookbackPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 1 for CHOP.");
            }

            // check quotes
            int qtyHistory = quotes.Count;
            int minHistory = lookbackPeriods + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for CHOP.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
