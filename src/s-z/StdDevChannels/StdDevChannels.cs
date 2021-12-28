namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STANDARD DEVIATION CHANNELS
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<StdDevChannelsResult> GetStdDevChannels<TQuote>(
            this IEnumerable<TQuote> quotes,
            int? lookbackPeriods = 20,
            double standardDeviations = 2)
            where TQuote : IQuote
        {

            // assume whole quotes when lookback is null
            if (lookbackPeriods is null)
            {
                lookbackPeriods = quotes.Count();
            }

            // check parameter arguments
            ValidateStdDevChannels(quotes, lookbackPeriods, standardDeviations);

            // initialize
            List<SlopeResult> slopeResults = GetSlope(quotes, (int)lookbackPeriods).ToList();

            int size = slopeResults.Count;
            List<StdDevChannelsResult> results = slopeResults
                .Select(x => new StdDevChannelsResult { Date = x.Date })
                .ToList();

            // roll through quotes in reverse
            for (int w = size - 1; w >= lookbackPeriods - 1; w -= (int)lookbackPeriods)
            {
                SlopeResult s = slopeResults[w];
                decimal? width = (decimal?)(standardDeviations * s.StdDev);

                // add regression line (y = mx + b) and channels
                for (int p = w - (int)lookbackPeriods + 1; p <= w; p++)
                {
                    if (p >= 0)
                    {
                        StdDevChannelsResult d = results[p];
                        d.Centerline = (decimal?)(s.Slope * (p + 1) + s.Intercept);
                        d.UpperChannel = d.Centerline + width;
                        d.LowerChannel = d.Centerline - width;

                        d.BreakPoint = (p == w - lookbackPeriods + 1);
                    }
                }
            }

            return results;
        }


        // remove recommended periods
        /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
        ///
        public static IEnumerable<StdDevChannelsResult> RemoveWarmupPeriods(
            this IEnumerable<StdDevChannelsResult> results)
        {
            int removePeriods = results
                .ToList()
                .FindIndex(x => x.UpperChannel != null || x.LowerChannel != null);

            return results.Remove(removePeriods);
        }


        // parameter validation
        private static void ValidateStdDevChannels<TQuote>(
            IEnumerable<TQuote> quotes,
            int? lookbackPeriods,
            double standardDeviations)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                    "Lookback periods must be greater than 1 for Standard Deviation Channels.");
            }

            if (standardDeviations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(standardDeviations), standardDeviations,
                    "Standard Deviations must be greater than 0 for Standard Deviation Channels.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = (int)lookbackPeriods;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for Standard Deviation Channels.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} are required.",
                    qtyHistory, minHistory);

                throw new BadQuotesException(nameof(quotes), message);
            }
        }
    }
}
