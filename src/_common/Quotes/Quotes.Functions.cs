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
        /// <include file='./info.xml' path='info/type[@name="Validate"]/*' />
        /// 
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
        /// <include file='./info.xml' path='info/type[@name="Aggregate"]/*' />
        /// 
        public static IEnumerable<Quote> Aggregate<TQuote>(
            this IEnumerable<TQuote> quotes,
            PeriodSize newSize)
            where TQuote : IQuote
        {
            // parameter conversion
            TimeSpan newTimeSpan = newSize.ToTimeSpan();

            // convert
            return quotes.Aggregate(newTimeSpan);
        }

        // aggregation (quantization) using TimeSpan
        /// <include file='./info.xml' path='info/type[@name="AggregateTimeSpan"]/*' />
        /// 
        public static IEnumerable<Quote> Aggregate<TQuote>(
            this IEnumerable<TQuote> quotes,
            TimeSpan timeSpan)
            where TQuote : IQuote
        {

            // handle no quotes scenario
            if (quotes == null || !quotes.Any())
            {
                return new List<Quote>();
            }

            if (timeSpan <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeSpan), timeSpan,
                    "Historical quotes Aggregation must use a new size value " +
                    "that is greater than zero (0).");
            }

            // return aggregation
            return quotes
                    .OrderBy(x => x.Date)
                    .GroupBy(x => x.Date.RoundDown(timeSpan))
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
            this IEnumerable<TQuote> quotes, CandlePart element = CandlePart.Close)
            where TQuote : IQuote
        {
            // elements represents the targeted OHLCV parts, so use "O" to return <Open> as base data, etc.
            // convert to basic data format
            IEnumerable<BasicData> basicData = element switch
            {
                CandlePart.Open => quotes.Select(x => new BasicData { Date = x.Date, Value = x.Open }),
                CandlePart.High => quotes.Select(x => new BasicData { Date = x.Date, Value = x.High }),
                CandlePart.Low => quotes.Select(x => new BasicData { Date = x.Date, Value = x.Low }),
                CandlePart.Close => quotes.Select(x => new BasicData { Date = x.Date, Value = x.Close }),
                CandlePart.Volume => quotes.Select(x => new BasicData { Date = x.Date, Value = x.Volume }),
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
