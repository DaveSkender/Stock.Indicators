using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // WEIGHTED MOVING AVERAGE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<WmaResult> GetWma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // convert quotes
            List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

            // check parameter arguments
            ValidateWma(quotes, lookbackPeriods);

            // initialize
            List<WmaResult> results = new(bdList.Count);
            double divisor = (lookbackPeriods * (lookbackPeriods + 1)) / 2d;

            // roll through quotes
            for (int i = 0; i < bdList.Count; i++)
            {
                BasicD q = bdList[i];
                int index = i + 1;

                WmaResult result = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
                {
                    double wma = 0;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        BasicD d = bdList[p];
                        wma += (double)d.Value * (lookbackPeriods - (index - p - 1)) / divisor;
                    }

                    result.Wma = (decimal)wma;
                }

                results.Add(result);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<WmaResult> RemoveWarmupPeriods(
            this IEnumerable<WmaResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Wma != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateWma<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for WMA.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for WMA.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
