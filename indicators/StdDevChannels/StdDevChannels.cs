using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // STANDARD DEVIATION CHANNELS
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<StdDevChannelsResult> GetStdDevChannels<TQuote>(
            IEnumerable<TQuote> history,
            int? lookbackPeriod = 20,
            decimal standardDeviations = 2)
            where TQuote : IQuote
        {

            // assume whole history when lookback is null
            if (lookbackPeriod is null)
            {
                lookbackPeriod = history.Count();
            }

            // check parameter arguments
            ValidateStdDevChannels(history, lookbackPeriod, standardDeviations);

            // initialize
            List<SlopeResult> slopeResults = GetSlope(history, (int)lookbackPeriod).ToList();

            int size = slopeResults.Count;
            List<StdDevChannelsResult> results = slopeResults
                .Select(x => new StdDevChannelsResult { Date = x.Date })
                .ToList();

            // roll through history in reverse
            for (int w = size - 1; w >= lookbackPeriod - 1; w -= (int)lookbackPeriod)
            {
                SlopeResult s = slopeResults[w];

                // add regression line (y = mx + b) and channels
                for (int p = w - (int)lookbackPeriod + 1; p <= w; p++)
                {
                    if (p >= 0)
                    {
                        StdDevChannelsResult d = results[p];
                        d.Centerline = s.Slope * (p + 1) + s.Intercept;

                        decimal width = standardDeviations * (decimal)s.StdDev;
                        d.UpperChannel = d.Centerline + width;
                        d.LowerChannel = d.Centerline - width;

                        d.BreakPoint = (p == w - lookbackPeriod + 1);
                    }
                }
            }

            return results;
        }


        private static void ValidateStdDevChannels<TQuote>(
            IEnumerable<TQuote> history,
            int? lookbackPeriod,
            decimal standardDeviations)
            where TQuote : IQuote
        {

            // check parameter arguments
            if (lookbackPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(lookbackPeriod), lookbackPeriod,
                    "Lookback period must be greater than 1 for Standard Deviation Channels.");
            }

            if (standardDeviations <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(standardDeviations), standardDeviations,
                    "Standard Deviations must be greater than 0 for Standard Deviation Channels.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = (int)lookbackPeriod;
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Standard Deviation Channels.  " +
                    string.Format(EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.",
                    qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
