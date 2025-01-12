/// <summary>
/// Provides methods for calculating the Connors RSI on a series of quotes.
/// </summary>
public static partial class ConnorsRsi
{
    /// <summary>
    /// Calculates the Connors RSI for a series of quotes.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source list, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The source list of quotes.</param>
    /// <param name="rsiPeriods">The number of periods to use for the RSI calculation. Default is 3.</param>
    /// <param name="streakPeriods">The number of periods to use for the streak calculation. Default is 2.</param>
    /// <param name="rankPeriods">The number of periods to use for the percent rank calculation. Default is 100.</param>
    /// <returns>A read-only list of <see cref="ConnorsRsiResult"/> containing the Connors RSI calculation results.</returns>
    public static IReadOnlyList<ConnorsRsiResult> ToConnorsRsi<T>(
        this IReadOnlyList<T> source,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
        where T : IReusable
    {
        // check parameter arguments
        ArgumentNullException.ThrowIfNull(source);
        Validate(rsiPeriods, streakPeriods, rankPeriods);

        // initialize
        int length = source.Count;
        List<ConnorsRsiResult> results = new(length);

        int startPeriod
            = Math.Max(rsiPeriods, Math.Max(streakPeriods, rankPeriods)) + 2;

        List<ConnorsRsiResult> streakInfo
            = source.CalcStreak(rsiPeriods, rankPeriods);

        // RSI of streak
        IReadOnlyList<RsiResult> rsiStreak = streakInfo
            .Select(si => new QuotePart(si.Timestamp, si.Streak))
            .ToList()
            .ToRsi(streakPeriods);

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

    /// <summary>
    /// Calculates the baseline streak and rank for the Connors RSI.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source list, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The source list of quotes.</param>
    /// <param name="rsiPeriods">The number of periods to use for the RSI calculation.</param>
    /// <param name="rankPeriods">The number of periods to use for the percent rank calculation.</param>
    /// <returns>A list of <see cref="ConnorsRsiResult"/> containing the baseline streak and rank calculation results.</returns>
    private static List<ConnorsRsiResult> CalcStreak<T>(
        this IReadOnlyList<T> source,
        int rsiPeriods,
        int rankPeriods)
        where T : IReusable
    {
        // initialize
        IReadOnlyList<RsiResult> rsiResults = source.ToRsi(rsiPeriods);

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
