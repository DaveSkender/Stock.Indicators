using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ULCER INDEX (UI)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<UlcerIndexResult> GetUlcerIndex<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 14)
            where TQuote : IQuote
        {

            // convert quotes
            List<QuoteD> quotesList = quotes.ConvertToList();

            // check parameter arguments
            ValidateUlcer(quotes, lookbackPeriods);

            // initialize
            List<UlcerIndexResult> results = new(quotesList.Count);

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                QuoteD q = quotesList[i];
                int index = i + 1;

                UlcerIndexResult result = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
                {
                    double? sumSquared = 0;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        QuoteD d = quotesList[p];
                        int dIndex = p + 1;

                        double maxClose = 0;
                        for (int s = index - lookbackPeriods; s < dIndex; s++)
                        {
                            QuoteD dd = quotesList[s];
                            if (dd.Close > maxClose)
                            {
                                maxClose = dd.Close;
                            }
                        }

                        double? percentDrawdown = (maxClose == 0) ? null
                            : 100 * (double)((d.Close - maxClose) / maxClose);

                        sumSquared += percentDrawdown * percentDrawdown;
                    }

                    result.UI = (sumSquared == null) ? null
                        : Math.Sqrt((double)sumSquared / lookbackPeriods);
                }
                results.Add(result);
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<UlcerIndexResult> RemoveWarmupPeriods(
            this IEnumerable<UlcerIndexResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.UI != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateUlcer<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Ulcer Index.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Ulcer Index.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
