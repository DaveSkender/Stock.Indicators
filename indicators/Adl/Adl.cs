using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ACCUMULATION/DISTRIBUTION LINE
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<AdlResult> GetAdl<TQuote>(
            this IEnumerable<TQuote> quotes,
            int? smaPeriods = null)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateAdl(quotes, smaPeriods);

            // initialize
            List<AdlResult> results = new(quotesList.Count);
            decimal prevAdl = 0;

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                decimal mfm = (q.High == q.Low) ? 0 : ((q.Close - q.Low) - (q.High - q.Close)) / (q.High - q.Low);
                decimal mfv = mfm * q.Volume;
                decimal adl = mfv + prevAdl;

                AdlResult result = new()
                {
                    Date = q.Date,
                    MoneyFlowMultiplier = mfm,
                    MoneyFlowVolume = mfv,
                    Adl = adl
                };
                results.Add(result);

                prevAdl = adl;

                // optional SMA
                if (smaPeriods != null && index >= smaPeriods)
                {
                    decimal sumSma = 0m;
                    for (int p = index - (int)smaPeriods; p < index; p++)
                    {
                        sumSma += results[p].Adl;
                    }

                    result.AdlSma = sumSma / smaPeriods;
                }
            }

            return results;
        }


        // convert to quotes
        /// <include file='../_Common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        /// 
        public static IEnumerable<Quote> ConvertToQuotes(
            this IEnumerable<AdlResult> results)
        {
            return results
              .Select(x => new Quote
              {
                  Date = x.Date,
                  Open = x.Adl,
                  High = x.Adl,
                  Low = x.Adl,
                  Close = x.Adl,
                  Volume = x.Adl
              })
              .ToList();
        }


        // parameter validation
        private static void ValidateAdl<TQuote>(
            IEnumerable<TQuote> quotes,
            int? smaPeriods)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (smaPeriods is not null and <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(smaPeriods), smaPeriods,
                    "SMA periods must be greater than 0 for ADL.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message = string.Format(EnglishCulture,
                    "Insufficient quotes provided for Accumulation/Distribution Line.  " +
                    "You provided {0} periods of quotes when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
