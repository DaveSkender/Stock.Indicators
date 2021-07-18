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
        public static IEnumerable<TQuote> Validate<TQuote>(this IEnumerable<TQuote> quotes)
            where TQuote : IQuote
        {
            // we cannot rely on date consistency when looking back, so we add an index and sort

            List<TQuote> quotesList = quotes.Sort();

            // check for duplicates
            DateTime lastDate = DateTime.MinValue;
            for (int i = 0; i < quotesList.Count; i++)
            {
                TQuote q = quotesList[i];

                if (lastDate == q.Date)
                {
                    throw new BadQuotesException(
                        string.Format(NativeCulture, "Duplicate date found on {0}.", q.Date));
                }

                lastDate = q.Date;
            }

            return quotesList;
        }

        // aggregation (quantization)
        public static IEnumerable<Quote> Aggregate<TQuote>(
            this IEnumerable<TQuote> quotes,
            PeriodSize newSize)
            where TQuote : IQuote
        {

            // handle no quotes scenario
            if (quotes == null || !quotes.Any())
            {
                return new List<Quote>();
            }

            // parameter validation
            TimeSpan newPeriod = newSize.ToTimeSpan();

            if (newPeriod == TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(newSize), newSize,
                    "Historical quotes Aggregation must use a New Size value of at least " +
                    "one minute and not more than one week.");
            }

            // return aggregation
            return quotes
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
        internal static List<TQuote> Sort<TQuote>(this IEnumerable<TQuote> quotes)
            where TQuote : IQuote
        {
            List<TQuote> quotesList = quotes.OrderBy(x => x.Date).ToList();

            // validate
            return quotesList == null || quotesList.Count == 0
                ? throw new BadQuotesException(nameof(quotes), "No historical quotes provided.")
                : quotesList;
        }

        // convert to basic
        internal static List<BasicData> ConvertToBasic<TQuote>(
            this IEnumerable<TQuote> quotes, string element = "C")
            where TQuote : IQuote
        {
            // elements represents the targeted OHLCV parts, so use "O" to return <Open> as base data, etc.
            // convert to basic data format
            IEnumerable<BasicData> basicData = element switch
            {
                "O" => quotes.Select(x => new BasicData { Date = x.Date, Value = x.Open }),
                "H" => quotes.Select(x => new BasicData { Date = x.Date, Value = x.High }),
                "L" => quotes.Select(x => new BasicData { Date = x.Date, Value = x.Low }),
                "C" => quotes.Select(x => new BasicData { Date = x.Date, Value = x.Close }),
                "V" => quotes.Select(x => new BasicData { Date = x.Date, Value = x.Volume }),
                _ => new List<BasicData>(),
            };

            List<BasicData> bdList = basicData.OrderBy(x => x.Date).ToList();

            // validate
            return bdList == null || bdList.Count == 0
                ? throw new BadQuotesException(nameof(quotes), "No historical quotes provided.")
                : bdList;
        }

    }
}
