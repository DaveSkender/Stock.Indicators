using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // RATE OF CHANGE (ROC)
        /// <include file='./info.xml' path='indicator/type[@name="Main"]/*' />
        /// 
        public static IEnumerable<RocResult> GetRoc<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int? smaPeriods = null)
            where TQuote : IQuote
        {

            // convert quotes
            List<QuoteD> quotesList = quotes.ConvertToList();

            // check parameter arguments
            ValidateRoc(quotes, lookbackPeriods, smaPeriods);

            // initialize
            List<RocResult> results = new(quotesList.Count);

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                QuoteD q = quotesList[i];
                int index = i + 1;

                RocResult result = new()
                {
                    Date = q.Date
                };

                if (index > lookbackPeriods)
                {
                    QuoteD back = quotesList[index - lookbackPeriods - 1];

                    result.Roc = (back.Close == 0) ? null
                        : 100d * (q.Close - back.Close) / back.Close;
                }

                results.Add(result);

                // optional SMA
                if (smaPeriods != null && index >= lookbackPeriods + smaPeriods)
                {
                    double? sumSma = 0;
                    for (int p = index - (int)smaPeriods; p < index; p++)
                    {
                        sumSma += results[p].Roc;
                    }

                    result.RocSma = sumSma / smaPeriods;
                }
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<RocResult> RemoveWarmupPeriods(
            this IEnumerable<RocResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Roc != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateRoc<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            int? smaPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for ROC.");
            }

            if (smaPeriods is not null and <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                    "SMA periods must be greater than 0 for ROC.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for ROC.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
