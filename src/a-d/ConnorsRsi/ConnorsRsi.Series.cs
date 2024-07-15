namespace Skender.Stock.Indicators;

// CONNORS RSI (SERIES)

public static partial class Indicator
{
    private static List<ConnorsRsiResult> CalcConnorsRsi<T>(
        this List<T> source,
        int rsiPeriods,
        int streakPeriods,
        int rankPeriods)
        where T : IReusable
    {
        // check parameter arguments
        ConnorsRsi.Validate(rsiPeriods, streakPeriods, rankPeriods);

        // initialize
        int length = source.Count;
        List<ConnorsRsiResult> results = new(length);

        int startPeriod
            = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;

        List<ConnorsRsiResult> streakInfo
            = source.CalcStreak(rsiPeriods, rankPeriods);

        // RSI of streak
        List<QuotePart> reStreak = streakInfo
            .Select(si => new QuotePart(si.Timestamp, si.Streak))
            .ToList();

        List<RsiResult> rsiStreak = reStreak.CalcRsi(streakPeriods);

        // compose final results
        for (int i = 0; i < length; i++)
        {
            if (i >= streakPeriods + 2)
            {
                ConnorsRsiResult sInfo = streakInfo[i];
                RsiResult sRsi = rsiStreak[i];

                double? crsi = null;

                if (i >= startPeriod - 1)
                {
                    crsi = (sInfo.Rsi + sRsi.Rsi + sInfo.PercentRank) / 3;
                }

                results.Add(sInfo with {
                    ConnorsRsi = crsi,
                    RsiStreak = sRsi.Rsi
                });
            }

            // warmup periods
            else
            {
                ConnorsRsiResult sInfo = streakInfo[i];
                results.Add(sInfo);
            }
        }

        return results;
    }

    // calculate baseline streak and rank
    private static List<ConnorsRsiResult> CalcStreak<T>(
        this List<T> source,
        int rsiPeriods,
        int rankPeriods)
        where T : IReusable
    {
        // initialize
        List<RsiResult> rsiResults = CalcRsi(source, rsiPeriods);

        int length = source.Count;
        List<ConnorsRsiResult> results = new(length);
        double[] gain = new double[length];

        double streak = 0;
        double prevPrice = double.NaN;

        // roll through source values
        for (int i = 0; i < length; i++)
        {
            T s = source[i];
            double? percentRank = null;

            // bypass for first record
            if (i == 0)
            {
                prevPrice = s.Value;
                results.Add(new(s.Timestamp, 0));
                continue;
            }

            // streak of up or down
            if (double.IsNaN(s.Value) || double.IsNaN(prevPrice))
            {
                streak = double.NaN;
            }
            else if (s.Value > prevPrice)
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
            else if (s.Value < prevPrice)
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
            else
            {
                streak = 0;
            }

            // percentile rank
            gain[i] = double.IsNaN(s.Value) || double.IsNaN(prevPrice) || prevPrice <= 0
                    ? double.NaN
                    : (s.Value - prevPrice) / prevPrice;

            if (i > rankPeriods - 1 && !double.IsNaN(gain[i]))
            {
                int qty = 0;
                bool isViableRank = true;
                for (int p = i - rankPeriods; p <= i; p++)
                {
                    // rank is not viable if there
                    // are incalculable gain values
                    if (double.IsNaN(gain[p]))
                    {
                        isViableRank = false;
                        break;
                    }

                    if (gain[p] < gain[i])
                    {
                        qty++;
                    }
                }

                percentRank = isViableRank ? 100 * qty / rankPeriods : null;
            }

            results.Add(new ConnorsRsiResult(
                Timestamp: s.Timestamp,
                Streak: streak,
                Rsi: rsiResults[i].Rsi,
                PercentRank: percentRank));

            prevPrice = s.Value;
        }

        return results;
    }
}
