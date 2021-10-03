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

            TQuote first = quotesList[0];

            bool isBullish = true;
            decimal sicBull = first.Close; // close high
            decimal sicBear = first.Close; // close low
            decimal? bearStop = null;
            decimal? bullStop = null;

            // roll through quotes
            for (int i = 0; i < size; i++)
            {
                TQuote q = quotesList[i];

                VolatilityStopResult r = new()
                {
                    Date = q.Date
                };

                if (i >= lookbackPeriods)
                {
                    // initialize values for range
                    decimal arc = (decimal)atrList[i].Atr * multiplier;
                    decimal prevClose = quotesList[i - 1].Close;

                    // potential stops
                    decimal bearEval = sicBear + arc;
                    decimal bullEval = sicBull - arc;

                    // initial trend (guess)
                    if (i == lookbackPeriods)
                    {
                        isBullish = (q.Close <= bearEval);

                        bearStop = bearEval;
                        bullStop = bullEval;
                    }

                    // new upper band
                    if (bearEval < bearStop || prevClose > bearStop)
                    {
                        bearStop = bearEval;
                    }

                    // new lower band
                    if (bullEval > bullStop || prevClose < bullStop)
                    {
                        bullStop = bullEval;
                    }

                    // SAR
                    if (q.Close <= ((isBullish) ? bullStop : bearStop))
                    {
                        r.Sar = bearStop;
                        r.UpperBand = bearStop;
                        r.IsStop = isBullish ? true : null;
                        isBullish = false;
                    }
                    else
                    {
                        r.Sar = bullStop;
                        r.LowerBand = bullStop;
                        r.IsStop = !isBullish ? true : null;
                        isBullish = true;
                    }
                }

                // next SIC
                sicBull = q.Close > sicBull ? q.Close : sicBull;
                sicBear = q.Close < sicBear ? q.Close : sicBear;
            }

            // remove first trend to stop, since it is an invalid guess
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
