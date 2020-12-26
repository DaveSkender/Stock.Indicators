using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // WILLIAMS FRACTAL
        public static IEnumerable<FractalResult> GetFractal<TQuote>(
            IEnumerable<TQuote> history)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateFractal(history);

            // initialize
            List<FractalResult> results = new List<FractalResult>(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                FractalResult r = new FractalResult()
                {
                    Date = h.Date
                };

                if (index > 2 && index <= historyList.Count - 2)
                {
                    // bearish signal
                    if (h.High > historyList[i - 2].High
                     && h.High > historyList[i - 1].High
                     && h.High > historyList[i + 1].High
                     && h.High > historyList[i + 2].High)
                    {
                        r.FractalBear = h.High;
                    }

                    // bullish signal
                    if (h.Low < historyList[i - 2].Low
                     && h.Low < historyList[i - 1].Low
                     && h.Low < historyList[i + 1].Low
                     && h.Low < historyList[i + 2].Low)
                    {
                        r.FractalBull = h.Low;
                    }
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateFractal<TQuote>(
            IEnumerable<TQuote> history)
            where TQuote : IQuote
        {

            // check history
            int qtyHistory = history.Count();
            int minHistory = 5;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Fractal.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }

    }
}