namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // FRACTAL CHAOS BANDS
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<FcbResult> GetFcb<TQuote>(
            this IEnumerable<TQuote> quotes,
            int windowSpan = 2)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateFcb(quotes, windowSpan);

            // initialize
            List<FractalResult> fractals = GetFractal(quotes, windowSpan).ToList();
            int size = fractals.Count;
            List<FcbResult> results = new(size);
            decimal? upperLine = null, lowerLine = null;

            // roll through quotes
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


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<FcbResult> RemoveWarmupPeriods(
            this IEnumerable<FcbResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.UpperBand != null || x.LowerBand != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateFcb<TQuote>(
            IEnumerable<TQuote> quotes,
            int windowSpan)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (windowSpan < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(windowSpan), windowSpan,
                    "Window span must be at least 2 for FCB.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = 2 * windowSpan + 1;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for FCB.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
