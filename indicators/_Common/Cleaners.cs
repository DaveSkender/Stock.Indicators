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

            List<Quote> historyList = history?.OrderBy(x => x.Date).ToList();

            if (historyList == null || historyList.Count == 0)
            {
                throw new BadHistoryException("No historical quotes provided.");
            }

            // return if already processed (no missing indexes)
            if (!historyList.Any(x => x.Index == null) && historyList[0].Index == 1)
            {
                return historyList;
            }

            // add index and check for errors
            DateTime lastDate = DateTime.MinValue;
            for (int i = 0; i < historyList.Count; i++)
            {
                Quote h = historyList[i];
                h.Index = i + 1;

                if (lastDate == h.Date)
                {
                    throw new BadHistoryException(
                        string.Format(nativeCulture, "Duplicate date found on {0}.", h.Date));
                }

                lastDate = h.Date;
            }

            return historyList;
        }


        internal static List<BasicData> PrepareBasicData(IEnumerable<BasicData> basicData)
        {
            // we cannot rely on date consistency when looking back, so we add an index and sort

            List<BasicData> bdList = basicData?.OrderBy(x => x.Date).ToList();

            if (bdList == null || bdList.Count == 0)
            {
                throw new BadHistoryException("No historical quotes provided.");
            }

            // return if already processed (no missing indexes)
            if (!bdList.Any(x => x.Index == null))
            {
                return bdList;
            }

            // add index and check for errors
            DateTime lastDate = DateTime.MinValue;
            for (int i = 0; i < bdList.Count; i++)
            {
                BasicData d = bdList[i];
                d.Index = i + 1;

                if (lastDate == d.Date)
                {
                    throw new BadHistoryException(
                        string.Format(nativeCulture, "Duplicate date found on {0}.", d.Date));
                }

                lastDate = d.Date;
            }

            return bdList;
        }


        internal static IEnumerable<BasicData> ConvertHistoryToBasic(IEnumerable<Quote> history, string element = "C")
        {
            // elements represents the targeted OHLCV parts, so use "O" to return <Open> as base data, etc.

            // convert to basic data format
            IEnumerable<BasicData> basicData = element switch
            {
                "O" => history.Select(x => new BasicData { Index = x.Index, Date = x.Date, Value = x.Open }),
                "H" => history.Select(x => new BasicData { Index = x.Index, Date = x.Date, Value = x.High }),
                "L" => history.Select(x => new BasicData { Index = x.Index, Date = x.Date, Value = x.Low }),
                "C" => history.Select(x => new BasicData { Index = x.Index, Date = x.Date, Value = x.Close }),
                "V" => history.Select(x => new BasicData { Index = x.Index, Date = x.Date, Value = x.Volume }),
                _ => new List<BasicData>(),
            };

            return PrepareBasicData(basicData);
        }
    }
}
