using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // HEIKIN-ASHI
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<HeikinAshiResult> GetHeikinAshi<TQuote>(
            IEnumerable<TQuote> history)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateHeikinAshi(history);

            // initialize
            List<HeikinAshiResult> results = new(historyList.Count);

            decimal? prevOpen = null;
            decimal? prevClose = null;

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];

                // close
                decimal close = (h.Open + h.High + h.Low + h.Close) / 4;

                // open
                decimal open = (prevOpen == null) ? (h.Open + h.Close) / 2
                    : (decimal)(prevOpen + prevClose) / 2;

                // high
                decimal[] arrH = { h.High, open, close };
                decimal high = arrH.Max();

                // low
                decimal[] arrL = { h.Low, open, close };
                decimal low = arrL.Min();


                HeikinAshiResult result = new()
                {
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


        private static void ValidateHeikinAshi<TQuote>(
            IEnumerable<TQuote> history)
            where TQuote : IQuote
        {

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Heikin-Ashi.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
