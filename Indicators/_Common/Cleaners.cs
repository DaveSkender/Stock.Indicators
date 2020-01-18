using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static class Cleaners
    {

        public static List<Quote> PrepareHistory(IEnumerable<Quote> history)
        {
            // we cannot rely on date consistency when looking back, so we add an index and sort

            if (history == null || !history.Any())
            {
                return new List<Quote>();
            }

            // return if already processed (no missing indexes)
            if (!history.Any(x => x.Index == null))
            {
                return history.OrderBy(x => x.Index).ToList();
            }

            // add index and check for errors
            int i = 1;
            DateTime lastDate = DateTime.MinValue;
            foreach (Quote h in history.OrderBy(x => x.Date))
            {
                h.Index = i++;

                if (lastDate == h.Date)
                {
                    throw new BadHistoryException(string.Format("Duplicate date found: {0}.", h.Date));
                }

                lastDate = h.Date;

                // TODO: more error evaluation (impossible values, missing values, etc.)
            }

            return history.OrderBy(x => x.Index).ToList();
        }
    }
}
