using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // SIMPLE MOVING AVERAGE
        public static IEnumerable<SmaResult> GetSma<TQuote>(
            IEnumerable<TQuote> history,
            int lookbackPeriod,
            bool extended = false)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateSma(history, lookbackPeriod);

            // initialize
            List<SmaResult> results = new List<SmaResult>(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                SmaResult result = new SmaResult
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

                    // add optional extended values
                    if (extended)
                    {
                        decimal sumMad = 0m;
                        decimal sumMse = 0m;
                        decimal? sumMape = 0m;

                        for (int p = index - lookbackPeriod; p < index; p++)
                        {
                            TQuote d = historyList[p];
                            sumMad += Math.Abs(d.Close - (decimal)result.Sma);
                            sumMse += (d.Close - (decimal)result.Sma) * (d.Close - (decimal)result.Sma);

                            sumMape += (d.Close == 0) ? null
                                : Math.Abs(d.Close - (decimal)result.Sma) / d.Close;
                        }

                        // mean absolute deviation
                        result.Mad = sumMad / lookbackPeriod;

                        // mean squared error
                        result.Mse = sumMse / lookbackPeriod;

                        // mean absolute percent error
                        result.Mape = sumMape / lookbackPeriod;
                    }
                }

                results.Add(result);
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
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }

    }
}