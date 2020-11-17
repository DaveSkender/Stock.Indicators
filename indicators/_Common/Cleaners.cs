using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Skender.Stock.Indicators
{

    public static class Cleaners
    {
        private static readonly CultureInfo nativeCulture = Thread.CurrentThread.CurrentUICulture;

        public static List<Quote> PrepareHistory(IEnumerable<Quote> history)
        {
            // we cannot rely on date consistency when looking back, so we add an index and sort

            List<Quote> historyList = history.Sort();

            // check for duplicates
            DateTime lastDate = DateTime.MinValue;
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];

                if (lastDate == h.Date)
                {
                    throw new BadHistoryException(
                        string.Format(nativeCulture, "Duplicate date found on {0}.", h.Date));
                }

                lastDate = h.Date;
            }

            return historyList;
        }

        internal static List<Quote> Sort(this IEnumerable<Quote> history)
        {
            List<Quote> historyList = history.OrderBy(x => x.Date).ToList();

            // validate
            if (historyList == null || historyList.Count == 0)
            {
                throw new BadHistoryException(nameof(history), "No historical quotes provided.");
            }

            return historyList;
        }

        internal static List<BasicData> ConvertHistoryToBasic(IEnumerable<Quote> history, string element = "C")
        {
            // elements represents the targeted OHLCV parts, so use "O" to return <Open> as base data, etc.
            // convert to basic data format
            IEnumerable<BasicData> basicData = element switch
            {
                "O" => history.Select(x => new BasicData { Date = x.Date, Value = x.Open }),
                "H" => history.Select(x => new BasicData { Date = x.Date, Value = x.High }),
                "L" => history.Select(x => new BasicData { Date = x.Date, Value = x.Low }),
                "C" => history.Select(x => new BasicData { Date = x.Date, Value = x.Close }),
                "V" => history.Select(x => new BasicData { Date = x.Date, Value = x.Volume }),
                _ => new List<BasicData>(),
            };

            List<BasicData> bdList = basicData.OrderBy(x => x.Date).ToList();

            // validate
            if (bdList == null || bdList.Count == 0)
            {
                throw new BadHistoryException(nameof(history), "No historical quotes provided.");
            }

            return bdList;
        }
    }
}
