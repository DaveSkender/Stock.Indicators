using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // ZIG ZAG
        public static IEnumerable<ZigZagResult> GetZigZag(IEnumerable<Quote> history, decimal percentChange)
        {

            // clean quotes
            history = Cleaners.PrepareHistory(history);

            // check parameters
            ValidateZigZag(history, percentChange);

            // initialize
            List<ZigZagResult> results = new List<ZigZagResult>();

            // roll through history
            foreach (Quote h in history)
            {

                ZigZagResult result = new ZigZagResult
                {
                    Index = (int)h.Index,
                    Date = h.Date
                };

                // TODO: add algorithm

                results.Add(result);
            }

            return results;
        }


        private static void ValidateZigZag(IEnumerable<Quote> history, decimal percentChange)
        {

            // check parameters
            if (percentChange <= 0)
            {
                throw new BadParameterException("Percent change must be greater than 0 for ZIGZAG.");
            }

            // check histor
            int qtyHistory = history.Count();
            int minHistory = 2;
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for ZIGZAG.  " +
                        string.Format(cultureProvider, 
                        "You provided {0} periods of history when at least {1} is required.", 
                        qtyHistory, minHistory));
            }

        }
    }

}
