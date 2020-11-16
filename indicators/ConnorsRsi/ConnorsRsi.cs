﻿using System;
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
            List<BasicData> bd = Cleaners.ConvertHistoryToBasic(history, "C").ToList();

            // check parameters
            ValidateConnorsRsi(bd, rsiPeriod, streakPeriod, rankPeriod);

            // initialize
            List<ConnorsRsiResult> results = CalcConnorsRsiBaseline(bd, rsiPeriod, rankPeriod);
            int startPeriod = Math.Max(rsiPeriod, Math.Max(streakPeriod, rankPeriod)) + 2;

            // RSI of streak
            List<BasicData> bdStreak = results
                .Where(x => x.Streak != null)
                .Select(x => new BasicData { Date = x.Date, Value = (decimal)x.Streak })
                .ToList();

            List<RsiResult> rsiStreakResults = CalcRsi(bdStreak, streakPeriod).ToList();

            // compose final results
            for (int p = streakPeriod + 2; p < results.Count; p++)
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


        private static List<ConnorsRsiResult> CalcConnorsRsiBaseline(
            List<BasicData> bd, int rsiPeriod, int rankPeriod)
        {
            List<RsiResult> rsiResults = CalcRsi(bd, rsiPeriod).ToList();
            List<ConnorsRsiResult> results = new List<ConnorsRsiResult>();

            decimal? lastClose = null;
            decimal streak = 0;

            // compose interim results
            for (int i = 0; i < bd.Count; i++)
            {
                BasicData h = bd[i];
                int index = i + 1;

                ConnorsRsiResult result = new ConnorsRsiResult
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
                result.PeriodGain = (decimal)((lastClose <= 0) ? null : (h.Value - lastClose) / lastClose);

                results.Add(result);

                if (index > rankPeriod)
                {
                    int qty = 0;
                    for (int p = index - rankPeriod - 1; p < index; p++)
                    {
                        ConnorsRsiResult r = results[p];
                        if (r.PeriodGain < result.PeriodGain)
                        {
                            qty++;
                        }
                    }

                    result.PercentRank = 100m * qty / rankPeriod;
                }


                lastClose = h.Value;
            }

            return results;
        }


        private static void ValidateConnorsRsi(
            IEnumerable<BasicData> history, int rsiPeriod, int streakPeriod, int rankPeriod)
        {

            // check parameters
            if (rsiPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rsiPeriod), rsiPeriod,
                    "RSI period for Close price must be greater than 1 for ConnorsRsi.");
            }

            if (streakPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(streakPeriod), streakPeriod,
                    "RSI period for Streak must be greater than 1 for ConnorsRsi.");
            }

            if (rankPeriod <= 1)
            {
                throw new ArgumentOutOfRangeException(nameof(rankPeriod), rankPeriod,
                    "Percent Rank period must be greater than 1 for ConnorsRsi.");
            }

            // check history
            int qtyHistory = history.Count();
            int minHistory = Math.Max(rsiPeriod, Math.Max(streakPeriod, rankPeriod + 2));
            if (qtyHistory < minHistory)
            {
                string message = "Insufficient history provided for ConnorsRsi.  " +
                    string.Format(englishCulture,
                    "You provided {0} periods of history when at least {1} is required.  "
                    + "Since this uses a smoothing technique, "
                    + "we recommend you use at least 250 data points prior to the intended "
                    + "usage date for maximum precision.", qtyHistory, minHistory);

                throw new BadHistoryException(nameof(history), message);
            }
        }
    }

}
