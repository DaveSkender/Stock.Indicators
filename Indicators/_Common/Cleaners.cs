using System.Collections.Generic;
using System.Linq;

namespace StockIndicators
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

            // return if already processed
            if (!history.Any(x => x.Index == null))
            {
                return history.OrderBy(x => x.Index).ToList();
            }

            // add index
            int i = 0;
            foreach (Quote h in history.OrderBy(x => x.Date))
            {
                h.Index = i++;

                // TODO: evaluate quote (same date as previous [dup], impossible values, missing values, etc.
            }

            return history.OrderBy(x => x.Index).ToList();
        }
    }
}
