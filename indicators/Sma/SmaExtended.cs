using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE (EXTENDED VERSION)
        /// <include file='./info.xml' path='indicators/type[@name="Extended"]/*' />
        /// 
        public static IEnumerable<SmaExtendedResult> GetSmaExtended<TQuote>(
            this IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // initialize
            List<SmaExtendedResult> results = GetSma(history, lookbackPeriod)
                .Select(x => new SmaExtendedResult { Date = x.Date, Sma = x.Sma })
                .ToList();

            // roll through history
            for (int i = lookbackPeriod - 1; i < results.Count; i++)
            {
                SmaExtendedResult r = results[i];
                int index = i + 1;

                decimal sumMad = 0m;
                decimal sumMse = 0m;
                decimal? sumMape = 0m;

                for (int p = index - lookbackPeriod; p < index; p++)
                {
                    TQuote d = historyList[p];
                    sumMad += Math.Abs(d.Close - (decimal)r.Sma);
                    sumMse += (d.Close - (decimal)r.Sma) * (d.Close - (decimal)r.Sma);

                    sumMape += (d.Close == 0) ? null
                        : Math.Abs(d.Close - (decimal)r.Sma) / d.Close;
                }

                // mean absolute deviation
                r.Mad = sumMad / lookbackPeriod;

                // mean squared error
                r.Mse = sumMse / lookbackPeriod;

                // mean absolute percent error
                r.Mape = sumMape / lookbackPeriod;
            }

            return results;
        }


        // prune recommended periods extensions (extended)
        public static IEnumerable<SmaExtendedResult> PruneWarmupPeriods(
            this IEnumerable<SmaExtendedResult> results)
        {
            int prunePeriods = results
                .ToList()
                .FindIndex(x => x.Sma != null);

            return results.Prune(prunePeriods);
        }
    }
}
