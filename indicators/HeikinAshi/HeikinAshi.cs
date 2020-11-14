using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // HEIKIN-ASHI
        public static IEnumerable<HeikinAshiResult> GetHeikinAshi(IEnumerable<Quote> history)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check parameters
            ValidateHeikinAshi(history);

            // initialize
            List<HeikinAshiResult> results = new List<HeikinAshiResult>();

            decimal? prevOpen = null;
            decimal? prevClose = null;

            foreach (Quote h in history)
            {

                // close
                decimal close = (h.Open + h.High + h.Low + h.Close) / 4;

                // open
                decimal open = (prevOpen == null) ? (h.Open + h.Close) / 2 : (decimal)(prevOpen + prevClose) / 2;

                // high
                decimal[] arrH = { h.High, open, close };
                decimal high = arrH.Max();

                // low
                decimal[] arrL = { h.Low, open, close };
                decimal low = arrL.Min();


                HeikinAshiResult result = new HeikinAshiResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    Open = open,
                    High = high,
                    Low = low,
                    Close = close
                };
                results.Add(result);

                // save for next iteration
                prevOpen = open;
                prevClose = close;
            }

            return results;
        }


        private static void ValidateHeikinAshi(IEnumerable<Quote> history)
        {

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Heikin-Ashi.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }

        }
    }

}
