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
            this IEnumerable<TQuote> quotes,
            int windowSpan = 2)
            where TQuote : IQuote
        {

            // sort quotes
            List<TQuote> quotesList = quotes.Sort();

            // check parameter arguments
            ValidateFractal(quotes, windowSpan);

            // initialize
            List<FractalResult> results = new(quotesList.Count);

            // roll through quotes
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];
                int index = i + 1;

                FractalResult r = new()
                {
                    Date = q.Date
                };

                if (index > windowSpan && index <= quotesList.Count - windowSpan)
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
                        TQuote d = quotesList[p];

                        if (q.High <= d.High)
                        {
                            isHigh = false;
                        }

                        if (q.Low >= d.Low)
                        {
                            isLow = false;
                        }
                    }

                    // bearish signal
                    if (isHigh)
                    {
                        r.FractalBear = q.High;
                    }

                    // bullish signal
                    if (isLow)
                    {
                        r.FractalBull = q.Low;
                    }
                }

                results.Add(r);
            }

            return results;
        }


        // parameter validation
        private static void ValidateFractal<TQuote>(
            IEnumerable<TQuote> quotes,
            int windowSpan)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (windowSpan < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(windowSpan), windowSpan,
                    "Window span must be at least 2 for Fractal.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = 2 * windowSpan + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Fractal.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
