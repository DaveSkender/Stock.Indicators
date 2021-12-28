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
        List<BasicD> bdList = quotes.ConvertToBasic(CandlePart.Close);

        // check parameter arguments
        ValidateConnorsRsi(bdList, rsiPeriods, streakPeriods, rankPeriods);

        // initialize
        List<ConnorsRsiResult> results = CalcConnorsRsiBaseline(bdList, rsiPeriods, rankPeriods);
        int startPeriod = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;

        // RSI of streak
        List<BasicD> bdStreak = results
            .Where(x => x.Streak != null)
            .Select(x => new BasicD { Date = x.Date, Value = (double)x.Streak })
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


    // remove recommended periods
    /// <include file='../../_common/Results/info.xml' path='info/type[@name="Prune"]/*' />
    ///
    public static IEnumerable<ConnorsRsiResult> RemoveWarmupPeriods(
        this IEnumerable<ConnorsRsiResult> results)
    {
        int n = results
          .ToList()
          .FindIndex(x => x.ConnorsRsi != null);

        return results.Remove(n);
    }


    // parameter validation
    private static List<ConnorsRsiResult> CalcConnorsRsiBaseline(
        List<BasicD> bdList, int rsiPeriods, int rankPeriods)
    {
        // initialize
        List<RsiResult> rsiResults = CalcRsi(bdList, rsiPeriods);

        int size = bdList.Count;
        List<ConnorsRsiResult> results = new(size);
        double?[] gain = new double?[size];

        double? lastClose = null;
        int streak = 0;

        // compose interim results
        for (int i = 0; i < size; i++)
        {
            BasicD q = bdList[i];
            int index = i + 1;

            ConnorsRsiResult r = new()
            {
                Date = q.Date,
                RsiClose = rsiResults[i].Rsi
            };
            results.Add(r);

            // bypass for first record
            if (lastClose == null)
            {
                lastClose = q.Value;
                continue;
            }

            // streak of up or down
            if (q.Value == lastClose)
            {
                streak = 0;
            }
            else if (q.Value > lastClose)
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
                    : (q.Value - lastClose) / lastClose;

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

                r.PercentRank = 100 * qty / rankPeriods;
            }

            lastClose = q.Value;
        }

        return results;
    }


    private static void ValidateConnorsRsi(
        IEnumerable<BasicD> quotes,
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
                string.Format(EnglishCulture,
                "You provided {0} periods of quotes when at least {1} are required.  "
                + "Since this uses a smoothing technique, "
                + "we recommend you use at least N+150 data points prior to the intended "
                + "usage date for better precision.", qtyHistory, minHistory);

            throw new BadQuotesException(nameof(quotes), message);
        }
    }
}
