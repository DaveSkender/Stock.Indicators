using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // VOLATILITY SYSTEM (STOP)
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<VolatilityStopResult> GetVolatilityStop<TQuote>(
            this IEnumerable<TQuote> quotes,
            int lookbackPeriods = 7,
            decimal multiplier = 3m)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateVolatilityStop(quotes, lookbackPeriods, multiplier);

            // initialize
            int size = quotesList.Count;
            List<VolatilityStopResult> results = new(size);
            List<AtrResult> atrList = quotes.GetAtr(lookbackPeriods).ToList();

            // initial trend (guess)
            decimal sic = quotesList[0].Close;
            bool isLong = (quotesList[lookbackPeriods - 1].Close > sic);

            for (int i = 0; i < lookbackPeriods; i++)
            {
                TQuote q = quotesList[i];
                sic = isLong ? Math.Max(sic, q.Close) : Math.Min(sic, q.Close);
                results.Add(new VolatilityStopResult() { Date = q.Date });
            }

            // roll through quotes
            for (int i = lookbackPeriods; i < size; i++)
            {
                TQuote q = quotesList[i];

                // average true range × multiplier constant
                decimal arc = (decimal)atrList[i - 1].Atr * multiplier;

                VolatilityStopResult r = new()
                {
                    Date = q.Date,

                    // stop and reverse threshold
                    Sar = isLong ? sic - arc : sic + arc
                };
                results.Add(r);

                // add SAR as separate bands
                if (isLong)
                {
                    r.LowerBand = r.Sar;
                }
                else
                {
                    r.UpperBand = r.Sar;
                }

                // evaluate stop and reverse
                if ((isLong && q.Close < r.Sar) || (!isLong && q.Close > r.Sar))
                {
                    r.IsStop = true;
                    sic = q.Close;
                    isLong = !isLong;
                }
                else
                {
                    r.IsStop = false;

                    // significant close adjustment
                    // extreme favorable close while in trade
                    sic = isLong ? Math.Max(sic, q.Close) : Math.Min(sic, q.Close);
                }
            }

            // remove first trend to stop, since it is a guess
            VolatilityStopResult firstStop = results
                .Where(x => x.IsStop == true)
                .OrderBy(x => x.Date)
                .FirstOrDefault();

            if (firstStop != null)
            {
                int cutIndex = results.IndexOf(firstStop);

                for (int d = 0; d <= cutIndex; d++)
                {
                    VolatilityStopResult r = results[d];
                    r.Sar = null;
                    r.UpperBand = null;
                    r.LowerBand = null;
                    r.IsStop = null;
                }
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<VolatilityStopResult> RemoveWarmupPeriods(
            this IEnumerable<VolatilityStopResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.Sar != null);

            removePeriods = Math.Max(100, removePeriods);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateVolatilityStop<TQuote>(
            IEnumerable<TQuote> quotes,
            int lookbackPeriods,
            decimal multiplier)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 1 for Volatility Stop.");
            }

            if (multiplier <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(multiplier), multiplier,
                    "ATR Multiplier must be greater than 0 for Volatility Stop.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = lookbackPeriods + 100;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Volatility Stop.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least N+250 data points prior to the intended "
                    + "usage date for better precision.", qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
