using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // WILLIAMS FRACTAL
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<FractalResult> GetFractal<TQuote>(
            this IEnumerable<TQuote> history,
            int windowSpan = 2)
            where TQuote : IQuote
        {

            // sort history
            List<TQuote> historyList = history.Sort();

            // check parameter arguments
            ValidateFractal(history, windowSpan);

            // initialize
            List<FractalResult> results = new(historyList.Count);

            // roll through history
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];
                int index = i + 1;

                FractalResult r = new()
                {
                    Date = h.Date
                };

                if (index > windowSpan && index <= historyList.Count - windowSpan)
                {
                    bool isHigh = true;
                    bool isLow = true;

                    for (int p = i - windowSpan; p <= i + windowSpan; p++)
                    {
                        // skip current period
                        if (p == i)
                        {
                            continue;
                        }

                        // evaluate "wings"
                        TQuote d = historyList[p];

                        if (h.High <= d.High)
                        {
                            isHigh = false;
                        }

                        if (h.Low >= d.Low)
                        {
                            isLow = false;
                        }
                    }

                    // bearish signal
                    if (isHigh)
                    {
                        r.FractalBear = h.High;
                    }

                    // bullish signal
                    if (isLow)
                    {
                        r.FractalBull = h.Low;
                    }
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateFractal<TQuote>(
            IEnumerable<TQuote> history,
            int windowSpan)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (windowSpan < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(windowSpan), windowSpan,
                    "Window span must be at least 2 for Fractal.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2 * windowSpan + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Fractal.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
