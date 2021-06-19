using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // GATOR OSCILLATOR
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<GatorResult> GetGator<TQuote>(
            this IEnumerable<TQuote> history)
            where TQuote : IQuote
        {

            // check parameter arguments
            ValidateGator(history);

            // initialize
            List<GatorResult> results = GetAlligator(history)
                .Select(x => new GatorResult
                {
                    Date = x.Date,

                    Upper = ((x.Jaw - x.Teeth) is null) ? null :
                    Math.Abs(x.Jaw.Value - x.Teeth.Value),

                    Lower = ((x.Teeth - x.Lips) is null) ? null :
                    -Math.Abs(x.Teeth.Value - x.Lips.Value)
                })
                .ToList();

            // roll through history
            for (int i = 13; i < results.Count; i++)
            {
                GatorResult r = results[i];
                GatorResult p = results[i - 1];

                // directional information
                r.UpperIsExpanding = p.Upper is not null ? (r.Upper > p.Upper) : null;
                r.LowerIsExpanding = p.Lower is not null ? (r.Lower < p.Lower) : null;
            }

            return results;
        }

        private static void ValidateGator<TQuote>(
            IEnumerable<TQuote> history)
            where TQuote : IQuote
        {
            // check history
            int qtyHistory = history.Count();

            // static values for traditional Gator Oscillator with max lookback of 13
            int minHistory = 115;
            int recHistory = 265;

            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for Gator Oscillator.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least {2} data points prior to the intended "
                    + "usage date for better precision.",
                    qtyHistory, minHistory, recHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }
}
