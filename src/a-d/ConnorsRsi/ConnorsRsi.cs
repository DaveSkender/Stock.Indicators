namespace Skender.Stock.Indicators;

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
        // convert quotes
        List<(DateTime Date, double Value)>? tpList = quotes.ToBasicTuple(CandlePart.Close);

        // check parameter arguments
        ValidateConnorsRsi(rsiPeriods, streakPeriods, rankPeriods);

        // initialize
        List<ConnorsRsiResult> results = CalcConnorsRsiBaseline(tpList, rsiPeriods, rankPeriods);
        int startPeriod = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;

        // RSI of streak
        List<(DateTime Date, double Streak)> bdStreak = results
            .Remove(Math.Min(results.Count, 1))
            .Select(x => (x.Date, (double)x.Streak))
            .ToList();

        List<RsiResult> rsiStreakResults = CalcRsi(bdStreak, streakPeriods);

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

    // parameter validation
    private static List<ConnorsRsiResult> CalcConnorsRsiBaseline(
        List<(DateTime Date, double Streak)> tpList, int rsiPeriods, int rankPeriods)
    {
        // initialize
        List<RsiResult> rsiResults = CalcRsi(tpList, rsiPeriods);

        int length = tpList.Count;
        List<ConnorsRsiResult> results = new(length);
        double?[] gain = new double?[length];

        double? lastClose = null;
        int streak = 0;

        // compose interim results
        for (int i = 0; i < length; i++)
        {
            (DateTime date, double value) = tpList[i];

            ConnorsRsiResult r = new()
            {
                Date = date,
                RsiClose = rsiResults[i].Rsi
            };
            results.Add(r);

            // bypass for first record
            if (lastClose == null)
            {
                lastClose = value;
                continue;
            }

            // streak of up or down
            if (value == lastClose)
            {
                streak = 0;
            }
            else if (value > lastClose)
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

            r.Streak = streak;

            // percentile rank
            gain[i] = (lastClose <= 0) ? null
                    : (value - lastClose) / lastClose;

            if (i + 1 > rankPeriods)
            {
                int qty = 0;
                for (int p = i - rankPeriods; p <= i; p++)
                {
                    if (gain[p] < gain[i])
                    {
                        qty++;
                    }
                }

                r.PercentRank = 100 * qty / rankPeriods;
            }

            lastClose = value;
        }

        return results;
    }

    private static void ValidateConnorsRsi(
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
    }
}
