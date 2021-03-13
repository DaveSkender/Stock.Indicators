using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE
        /// <include file='./info.xml' path='indicators/type[@name="Main"]/*' />
        /// 
        public static IEnumerable<SmaResult> GetSma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateSma(history, lookbackPeriod);

            // initialize
            List<SmaResult> results = new(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                SmaResult result = new()
                {
                    Date = h.Date
                };

                if (index >= lookbackPeriod)
                {
                    decimal sumSma = 0m;
                    for (int p = index - lookbackPeriod; p < index; p++)
                    {
                        TQuote d = historyList[p];
                        sumSma += d.Close;
                    }

                    result.Sma = sumSma / lookbackPeriod;
                }

                results.Add(result);
            }

            return results;
        }

        // EXTENDED
        /// <include file='./info.xml' path='indicators/type[@name="Extended"]/*' />
        /// 
        public static IEnumerable<SmaExtendedResult> GetSmaExtended<TQuote>(
            IEnumerable<TQuote> history,
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

        private static void ValidateSma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 0 for SMA.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for SMA.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
