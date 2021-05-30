using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Skender.Stock.Indicators
{
    // HISTORICAL QUOTES FUNCTIONS (GENERAL)

    public static class HistoricalQuotes
    {
        private static readonly CultureInfo NativeCulture = Thread.CurrentThread.CurrentUICulture;

        // validation
        public static IEnumerable<TQuote> Validate<TQuote>(this IEnumerable<TQuote> history)
            where TQuote : IQuote
        {
            // we cannot rely on date consistency when looking back, so we add an index and sort

            List<TQuote> historyList = history.Sort();

            // check for duplicates
            DateTime lastDate = DateTime.MinValue;
            for (int i = 0; i < historyList.Count; i++)
            {
                TQuote h = historyList[i];

                if (lastDate == h.Date)
                {
                    throw new BadHistoryException(
                        string.Format(NativeCulture, "Duplicate date found on {0}.", h.Date));
                }

                lastDate = h.Date;
            }

            return historyList;
        }

        // quantization
        public static IEnumerable<Quote> Aggregate<TQuote>(
            this IEnumerable<TQuote> history,
            PeriodSize newSize)
            where TQuote : IQuote
        {
            TimeSpan newPeriod = newSize.ToTimeSpan();

            return

                // handle no history scenario
                history == null || !history.Any() ? new List<Quote>()

                // validate parameters
                : newPeriod == TimeSpan.Zero ?

                throw new ArgumentOutOfRangeException(nameof(newSize), newSize,
                    "History Aggregation must use a New Size value of at least " +
                    "one minute and not more than one week.")

                // return aggregation
                : history
                    .OrderBy(x => x.Date)
                    .GroupBy(x => x.Date.RoundDown(newPeriod))
                    .Select(x => new Quote
                    {
                        Date = x.Key,
                        Open = x.First().Open,
                        High = x.Max(t => t.High),
                        Low = x.Min(t => t.Low),
                        Close = x.Last().Close,
                        Volume = x.Sum(t => t.Volume)
                    });
        }

        // sort
        internal static List<TQuote> Sort<TQuote>(this IEnumerable<TQuote> history)
            where TQuote : IQuote
        {
            List<TQuote> historyList = history.OrderBy(x => x.Date).ToList();

            // validate
            return historyList == null || historyList.Count == 0
                ? throw new BadHistoryException(nameof(history), "No historical quotes provided.")
                : historyList;
        }

        // convert to basic
        internal static List<BasicData> ConvertToBasic<TQuote>(
            this IEnumerable<TQuote> history, string element = "C")
            where TQuote : IQuote
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
            return bdList == null || bdList.Count == 0
                ? throw new BadHistoryException(nameof(history), "No historical quotes provided.")
                : bdList;
        }

    }
}
