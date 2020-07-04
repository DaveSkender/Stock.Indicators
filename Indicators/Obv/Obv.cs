using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ON-BALANCE VOLUME
        public static IEnumerable<ObvResult> GetObv(IEnumerable<Quote> history)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check parameters
            ValidateObv(history);

            // initialize
            List<ObvResult> results = new List<ObvResult>();

            decimal? prevClose = null;
            long obv = 0;

            foreach (Quote h in history)
            {
                if(prevClose == null || h.Close == prevClose)
                {
                    // no change to OBV
                }
                else if(h.Close > prevClose)
                {
                    obv += h.Volume;
                }
                else if(h.Close < prevClose)
                {
                    obv -= h.Volume;
                }

                ObvResult result = new ObvResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    Obv = obv
                };
                results.Add(result);

                prevClose = h.Close;
            }

            return results;
        }


        private static void ValidateObv(IEnumerable<Quote> history)
        {

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for Heikin-Ashi.  " +
                        string.Format("You provided {0} periods of history when at least {1} is required.", qtyHistory, minHistory));
            }

        }
    }

}
