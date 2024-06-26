namespace Skender.Stock.Indicators;

// CONNORS RSI (SERIES)

public static partial class Indicator
{
    internal static List<ConnorsRsiResult> CalcConnorsRsi<T>(
        this List<T> source,
        int rsiPeriods,
        int streakPeriods,
        int rankPeriods)
        where T : IReusableResult
    {
        // check parameter arguments
        ConnorsRsi.Validate(rsiPeriods, streakPeriods, rankPeriods);

        // initialize
        List<ConnorsRsiResult> results = source.CalcStreak(rsiPeriods, rankPeriods);
        int startPeriod = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;
        int length = results.Count;

        // RSI of streak
        List<Reusable> bdStreak = results
            .Select(x => new Reusable(x.Timestamp, x.Streak))
            .ToList();

        List<RsiResult> rsiStreak = CalcRsi(bdStreak, streakPeriods);

        // compose final results
        for (int p = streakPeriods + 2; p < length; p++)
        {
            ConnorsRsiResult r = results[p];
            RsiResult k = rsiStreak[p];

            r.RsiStreak = k.Rsi;

            if (p >= startPeriod - 1)
            {
                r.ConnorsRsi = (r.Rsi + r.RsiStreak + r.PercentRank) / 3;
            }
        }

        return results;
    }

    // calculate baseline streak and rank
    private static List<ConnorsRsiResult> CalcStreak<T>(
        this List<T> source,
        int rsiPeriods,
        int rankPeriods)
        where T : IReusableResult
    {
        // initialize
        List<RsiResult> rsiResults = CalcRsi(source, rsiPeriods);

        int length = source.Count;
        List<ConnorsRsiResult> results = new(length);
        double[] gain = new double[length];

        double prevPrice = double.NaN;
        double streak = 0;

        // compose interim results
        for (int i = 0; i < length; i++)
        {
            var s = source[i];

            ConnorsRsiResult r = new() {
                Timestamp = s.Timestamp,
                Rsi = rsiResults[i].Rsi
            };
            results.Add(r);

            // bypass for first record
            if (i == 0)
            {
                prevPrice = s.Value;
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

            r.Streak = streak;

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

                r.PercentRank = isViableRank ? 100 * qty / rankPeriods : null;
            }

            prevPrice = s.Value;
        }

        return results;
    }
}
