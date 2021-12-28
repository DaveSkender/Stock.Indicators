namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // VOLUME WEIGHTED MOVING AVERAGE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<VwmaResult> GetVwma<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // convert quotes
            List<QuoteD> quotesList = quotes.ConvertToList();

            // check parameter arguments
            ValidateVwma(quotes, lookbackPeriods);

            // initialize
            int size = quotesList.Count;
            List<VwmaResult> results = new(size);

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                QuoteD q = quotesList[i];
                int index = i + 1;

                VwmaResult result = new()
                {
                    Date = q.Date
                };

                if (index >= lookbackPeriods)
                {
                    double? sumCl = 0;
                    double? sumVl = 0;
                    for (int p = index - lookbackPeriods; p < index; p++)
                    {
                        QuoteD d = quotesList[p];
                        double? c = d.Close;
                        double? v = d.Volume;

                        sumCl += c * v;
                        sumVl += v;
                    }

                    result.Vwma = sumVl != 0 ? (decimal?)(sumCl / sumVl) : null;
                }

                results.Add(result);
            }


            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<VwmaResult> RemoveWarmupPeriods(
            this IEnumerable<VwmaResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Vwma != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateVwma<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 0 for Vwma.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Vwma.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
