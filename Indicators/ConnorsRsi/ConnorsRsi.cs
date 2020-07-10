using System;
using System.Collections.Generic;
using System.Linq;

namespace Skender.Stock.Indicators
{
    public static partial class Indicator
    {
        // CONNORS RSI
        public static IEnumerable<ConnorsRsiResult> GetConnorsRsi(
            IEnumerable<Quote> history, int rsiPeriod = 3, int streakPeriod = 2, int rankPeriod = 100)
        {

            // convert history to basic format
            IEnumerable<BasicData> bd = Cleaners.ConvertHistoryToBasic(history, "C");

            // check parameters
            ValidateConnorsRsi(bd, rsiPeriod, streakPeriod, rankPeriod);

            // initialize
            List<ConnorsRsiResult> results = new List<ConnorsRsiResult>();
            IEnumerable<RsiResult> rsiResults = CalcRsi(bd, rsiPeriod);
            int startPeriod = Math.Max(rsiPeriod, Math.Max(streakPeriod, rankPeriod)) + 2;

            decimal? lastClose = null;
            decimal streak = 0;

            // compose interim results
            foreach (BasicData h in bd)
            {
                ConnorsRsiResult result = new ConnorsRsiResult
                {
                    Index = (int)h.Index,
                    Date = h.Date,
                    RsiClose = rsiResults.Where(x => x.Index == h.Index).FirstOrDefault().Rsi
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
                result.PeriodGain = (decimal)((lastClose <= 0) ? null : (h.Value - lastClose) / lastClose);

                if (h.Index > rankPeriod)
                {
                    IEnumerable<ConnorsRsiResult> period = results
                        .Where(x => x.Index >= (h.Index - rankPeriod) && x.Index < h.Index);

                    result.PercentRank = (decimal)100 * period
                        .Where(x => x.PeriodGain < result.PeriodGain).Count() / rankPeriod;
                }

                results.Add(result);
                lastClose = h.Value;
            }

            // RSI of streak
            List<BasicData> bdStreak = results
                .Where(x => x.Streak != null)
                .Select(x => new BasicData { Index = null, Date = x.Date, Value = (decimal)x.Streak })
                .ToList();

            IEnumerable<RsiResult> rsiStreakResults = CalcRsi(bdStreak, streakPeriod);

            // compose final results
            foreach (ConnorsRsiResult r in results.Where(x => x.Index >= streakPeriod + 2))
            {
                r.RsiStreak = rsiStreakResults
                    .Where(x => x.Index == r.Index - 1)
                    .FirstOrDefault()
                    .Rsi;

                if (r.Index >= startPeriod)
                {
                    r.ConnorsRsi = (r.RsiClose + r.RsiStreak + r.PercentRank) / 3;
                }
            }

            return results;
        }


        private static void ValidateConnorsRsi(
            IEnumerable<BasicData> basicData, int rsiPeriod, int streakPeriod, int rankPeriod)
        {

            // check parameters
            if (rsiPeriod <= 1)
            {
                throw new BadParameterException("RSI period for Close price must be greater than 1 for ConnorsRsi.");
            }

            if (streakPeriod <= 1)
            {
                throw new BadParameterException("RSI period for Streak must be greater than 1 for ConnorsRsi.");
            }

            if (rankPeriod <= 1)
            {
                throw new BadParameterException("Percent Rank period must be greater than 1 for ConnorsRsi.");
            }


            // check history
            int qtyHistory = basicData.Count();
            int minHistory = Math.Max(rsiPeriod, Math.Max(streakPeriod, rankPeriod + 2));
            if (qtyHistory < minHistory)
            {
                throw new BadHistoryException("Insufficient history provided for ConnorsRsi.  " +
                        string.Format("You provided {0} periods of history when at least {1} is required.  "
                          + "Since this uses a smoothing technique, "
                          + "we recommend you use at least 250 data points prior to the intended "
                          + "usage date for maximum precision.", qtyHistory, minHistory));
            }
        }
    }

}
