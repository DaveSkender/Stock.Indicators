using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CONNORS RSI
        /// <include file='./info.xml' path='indicator/*' />
        /// 
        public static IEnumerable<ConnorsRsiResult> GetConnorsRsi<TQuote>(
            this IEnumerable<TQuote> quotes,
            int rsiPeriods = 3,
            int streakPeriods = 2,
            int rankPeriods = 100)
            where TQuote : IQuote
        {

            // convert quotes to basic format
            List<BasicData> bdList = quotes.ConvertToBasic("C");

            // check parameter arguments
            ValidateConnorsRsi(bdList, rsiPeriods, streakPeriods, rankPeriods);

            // initialize
            List<ConnorsRsiResult> results = CalcConnorsRsiBaseline(bdList, rsiPeriods, rankPeriods);
            int startPeriod = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;

            // RSI of streak
            List<BasicData> bdStreak = results
                .Where(x => x.Streak != null)
                .Select(x => new BasicData { Date = x.Date, Value = (decimal)x.Streak })
                .ToList();

            List<RsiResult> rsiStreakResults = CalcRsi(bdStreak, streakPeriods).ToList();

            // compose final results
            for (int p = streakPeriods + 2; p < results.Count; p++)
            {
                ConnorsRsiResult r = results[p];
                RsiResult k = rsiStreakResults[p - 1];

                r.RsiStreak = k.Rsi;

                if (p + 1 >= startPeriod)
                {
                    r.ConnorsRsi = (r.RsiClose + r.RsiStreak + r.PercentRank) / 3;
                }
            }

            return results;
        }


        // prune recommended periods extensions
        public static IEnumerable<ConnorsRsiResult> PruneWarmupPeriods(
            this IEnumerable<ConnorsRsiResult> results)
        {
            int n = results
              .ToList()
              .FindIndex(x => x.ConnorsRsi != null);

            return results.Prune(n);
        }


        // parameter validation
        private static List<ConnorsRsiResult> CalcConnorsRsiBaseline(
            List<BasicData> bdList, int rsiPeriods, int rankPeriods)
        {
            // initialize
            List<RsiResult> rsiResults = CalcRsi(bdList, rsiPeriods).ToList();

            int size = bdList.Count;
            List<ConnorsRsiResult> results = new(size);
            decimal?[] gain = new decimal?[size];

            decimal? lastClose = null;
            decimal streak = 0;

            // compose interim results
            for (int i = 0; i < size; i++)
            {
                BasicData h = bdList[i];
                int index = i + 1;

                ConnorsRsiResult result = new()
                {
                    Date = h.Date,
                    RsiClose = rsiResults[i].Rsi
                };

                // bypass for first record
                if (lastClose == null)
                {
                    lastClose = h.Value;
                    results.Add(result);
                    continue;
                }

                // streak of up or down
                if (h.Value == lastClose)
                {
                    streak = 0;
                }
                else if (h.Value > lastClose)
                {
                    if (streak >= 0)
                    {
                        streak++;
                    }
                    else
                    {
                        streak = 1;
                    }
                }
                else // h.Value < lastClose
                {
                    if (streak <= 0)
                    {
                        streak--;
                    }
                    else
                    {
                        streak = -1;
                    }
                }

                result.Streak = streak;

                // percentile rank
                gain[i] = (lastClose == 0) ? null
                    : (decimal)((lastClose <= 0) ? null : (h.Value - lastClose) / lastClose);

                results.Add(result);

                if (index > rankPeriods)
                {
                    int qty = 0;
                    for (int p = index - rankPeriods - 1; p < index; p++)
                    {
                        if (gain[p] < gain[i])
                        {
                            qty++;
                        }
                    }

                    result.PercentRank = 100m * qty / rankPeriods;
                }


                lastClose = h.Value;
            }

            return results;
        }


        private static void ValidateConnorsRsi(
            IEnumerable<BasicData> quotes,
            int rsiPeriods,
            int streakPeriods,
            int rankPeriods)
        {

            // check parameter arguments
            if (rsiPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rsiPeriods), rsiPeriods,
                    "RSI period for Close price must be greater than 1 for ConnorsRsi.");
            }

            if (streakPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(streakPeriods), streakPeriods,
                    "RSI period for Streak must be greater than 1 for ConnorsRsi.");
            }

            if (rankPeriods <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rankPeriods), rankPeriods,
                    "Percent Rank periods must be greater than 1 for ConnorsRsi.");
            }

            // check quotes
            int qtyHistory = quotes.Count();
            int minHistory = Math.Max(rsiPeriods + 100, Math.Max(streakPeriods, rankPeriods + 2));
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient quotes provided for ConnorsRsi.  " +
                    string.Format(
                        EnglishCulture,
                    "You provided {0} periods of quotes when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least N+150 data points prior to the intended "
                    + "usage date for better precision.", qtyHistory, minHistory);

                throw new BadHistoryException(nameof(quotes), message);
            }
        }
    }
}
