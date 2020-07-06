using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ON-BALANCE VOLUME
        public static IEnumerable<AdlResult> GetAdl(IEnumerable<Quote> history)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check parameters
            ValidateAdl(history);

            // initialize
            List<AdlResult> results = new List<AdlResult>();
            decimal prevAdl = 0;

            // get money flow multiplier
            foreach (Quote h in history)
            {
                decimal mfm = (h.High == h.Low) ? 0 : ((h.Close - h.Low) - (h.High - h.Close)) / (h.High - h.Low);
                decimal mfv = mfm * h.Volume;
                decimal adl = mfv + prevAdl;

                AdlResult result = new AdlResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    MoneyFlowMultiplier = mfm,
                    MoneyFlowVolume = mfv,
                    Adl = adl
                };
                results.Add(result);

                prevAdl = adl;
            }

            return results;
        }


        private static void ValidateAdl(IEnumerable<Quote> history)
        {

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Accumulation Distribution Line.  " +
                        string.Format("You provided {0} periods of history when at least {1} is required.", qtyHistory, minHistory));
            }

        }
    }

}
