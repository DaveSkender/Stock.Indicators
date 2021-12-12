using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE (EXTENDED VERSION)
        /// <include file='./info.xml' path='indicator/type[@name="Extended"]/*' />
        /// 
        public static IEnumerable<SmaExtendedResult> GetSmaExtended<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // initialize
            List<SmaExtendedResult> results = GetSma(quotes, lookbackPeriods)
                .Select(x => new SmaExtendedResult { Date = x.Date, Sma = x.Sma })
                .ToList();

            // roll through quotes
            for (int i = lookbackPeriods - 1; i < results.Count; i++)
            {
                int index = i + 1;
                SmaExtendedResult r = results[i];
                double sma = (double)r.Sma;

                double sumMad = 0;
                double sumMse = 0;
                double? sumMape = 0;

                for (int p = index - lookbackPeriods; p < index; p++)
                {
                    TQuote d = quotesList[p];
                    double close = (double)d.Close;

                    sumMad += Math.Abs(close - sma);
                    sumMse += (close - sma) * (close - sma);

                    sumMape += (close == 0) ? null
                        : Math.Abs(close - sma) / close;
                }

                // mean absolute deviation
                r.Mad = sumMad / lookbackPeriods;

                // mean squared error
                r.Mse = sumMse / lookbackPeriods;

                // mean absolute percent error
                r.Mape = sumMape / lookbackPeriods;
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<SmaExtendedResult> RemoveWarmupPeriods(
            this IEnumerable<SmaExtendedResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Sma != null);

            return results.Remove(removePeriods);
        }
    }
}
