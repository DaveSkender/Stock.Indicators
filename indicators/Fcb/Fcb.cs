using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // FRACTAL CHAOS BANDS
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<FcbResult> GetFcb<TQuote>(
            this IEnumerable<TQuote> history,
            int windowSpan = 2)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateFcb(history, windowSpan);

            // initialize
            List<FractalResult> fractals = GetFractal(history, windowSpan).ToList();
            int size = fractals.Count;
            List<FcbResult> results = new(size);
            decimal? upperLine = null, lowerLine = null;

            // roll through history
            for (int i = 0; i < size; i++)
            {
                int index = i + 1;
                FractalResult f = fractals[i];

                FcbResult r = new()
                {
                    Date = f.Date
                };

                if (index >= 2 * windowSpan + 1)
                {
                    FractalResult fp = fractals[i - windowSpan];

                    upperLine = (fp.FractalBear != null) ? fp.FractalBear : upperLine;
                    lowerLine = (fp.FractalBull != null) ? fp.FractalBull : lowerLine;

                    r.UpperBand = upperLine;
                    r.LowerBand = lowerLine;
                }

                results.Add(r);
            }

            return results;
        }


        private static void ValidateFcb<TQuote>(
            IEnumerable<TQuote> history,
            int windowSpan)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (windowSpan < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(windowSpan), windowSpan,
                    "Window span must be at least 2 for FCB.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = 2 * windowSpan + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for FCB.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
