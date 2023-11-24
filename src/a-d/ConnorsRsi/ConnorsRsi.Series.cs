namespace Skender.Stock.Indicators;

// CONNORS RSI (SERIES)

public static partial class Indicator
{
    internal static List<ConnorsRsiResult> CalcConnorsRsi(
        this List<(DateTime, double)> tpList,
        int rsiPeriods,
        int streakPeriods,
        int rankPeriods)
    {
        // check parameter arguments
        ConnorsRsi.Validate(rsiPeriods, streakPeriods, rankPeriods);

        // initialize
        List<ConnorsRsiResult> results = tpList.CalcStreak(rsiPeriods, rankPeriods);
        int startPeriod = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;
        int length = results.Count;

        // RSI of streak
        List<(DateTime Date, double Streak)> bdStreak = results
            .Select(x => (x.Date, x.Streak))
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
    private static List<ConnorsRsiResult> CalcStreak(
        this List<(DateTime, double)> tpList,
        int rsiPeriods,
        int rankPeriods)
    {
        // initialize
        List<RsiResult> rsiResults = CalcRsi(tpList, rsiPeriods);

        int length = tpList.Count;
        List<ConnorsRsiResult> results = new(length);
        double[] gain = new double[length];

        double prevPrice = double.NaN;
        double streak = 0;

        // compose interim results
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double price) = tpList[i];

            ConnorsRsiResult r = new(date)
            {
                Rsi = rsiResults[i].Rsi
            };
            results.Add(r);

            // bypass for first record
            if (i == 0)
            {
                prevPrice = price;
                continue;
            }

            // streak of up or down
            if (double.IsNaN(price) || double.IsNaN(prevPrice))
            {
                streak = double.NaN;
            }
            else if (price > prevPrice)
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
            else if (price < prevPrice)
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
            gain[i] = double.IsNaN(price) || double.IsNaN(prevPrice) || prevPrice <= 0
                    ? double.NaN
                    : (price - prevPrice) / prevPrice;

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

            prevPrice = price;
        }

        return results;
    }
}
