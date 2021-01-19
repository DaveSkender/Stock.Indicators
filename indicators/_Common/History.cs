using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Skender.Stock.Indicators
{
    // HISTORICAL QUOTES

    public interface IQuote
    {
        public DateTime Date { get; }
        public decimal Open { get; }
        public decimal High { get; }
        public decimal Low { get; }
        public decimal Close { get; }
        public decimal Volume { get; }
    }

    [Serializable]
    public class Quote : IQuote
    {
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public decimal Volume { get; set; }
    }

    [Serializable]
    internal class BasicData
    {
        internal DateTime Date { get; set; }
        internal decimal Value { get; set; }
    }


    public static class HistoricalQuotes
    {
        private static readonly CultureInfo NativeCulture = Thread.CurrentThread.CurrentUICulture;

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

        internal static List<TQuote> Sort<TQuote>(this IEnumerable<TQuote> history)
            where TQuote : IQuote
        {
            List<TQuote> historyList = history.OrderBy(x => x.Date).ToList();

            // validate
            return historyList == null || historyList.Count == 0
                ? throw new BadHistoryException(nameof(history), "No historical quotes provided.")
                : historyList;
        }

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


    // for backwards compatibility only
    // TODO: remove in v2
    public static class Cleaners
    {

        public static IEnumerable<TQuote> ValidateHistory<TQuote>(IEnumerable<TQuote> history)
            where TQuote : IQuote
        {
            return history.Validate();
        }
    }
}
