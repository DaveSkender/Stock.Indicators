namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Connors RSI indicator in a streaming context.
/// </summary>
public class ConnorsRsiHub
    : ChainProvider<IReusable, ConnorsRsiResult>, IConnorsRsi
{
    private readonly string hubName;
    private readonly RsiHub rsiHub;
    private readonly Queue<double> streakBuffer;
    private readonly Queue<double> gainBuffer;
    private double streak;
    private double prevValue;
    private double streakAvgGain;
    private double streakAvgLoss;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnorsRsiHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="rsiPeriods">The number of periods to use for the RSI calculation. Default is 3.</param>
    /// <param name="streakPeriods">The number of periods to use for the streak calculation. Default is 2.</param>
    /// <param name="rankPeriods">The number of periods to use for the percent rank calculation. Default is 100.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    internal ConnorsRsiHub(
        IChainProvider<IReusable> provider,
        int rsiPeriods,
        int streakPeriods,
        int rankPeriods) : base(provider)
    {
        ConnorsRsi.Validate(rsiPeriods, streakPeriods, rankPeriods);
        RsiPeriods = rsiPeriods;
        StreakPeriods = streakPeriods;
        RankPeriods = rankPeriods;

        hubName = $"CRSI({rsiPeriods},{streakPeriods},{rankPeriods})";

        // Create internal hub for price RSI
        rsiHub = provider.ToRsiHub(rsiPeriods);

        // Initialize state
        streakBuffer = new Queue<double>();
        gainBuffer = new Queue<double>(rankPeriods + 1);
        streak = 0;
        prevValue = double.NaN;
        streakAvgGain = double.NaN;
        streakAvgLoss = double.NaN;

        Reinitialize();
    }

    /// <inheritdoc/>
    public int RsiPeriods { get; init; }

    /// <inheritdoc/>
    public int StreakPeriods { get; init; }

    /// <inheritdoc/>
    public int RankPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (ConnorsRsiResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Get RSI from embedded hub
        double? rsi = rsiHub.Results[i].Rsi;

        // Calculate streak
        double currentValue = item.Value;
        double currentStreak = 0;

        if (i == 0)
        {
            prevValue = currentValue;
            currentStreak = 0;
            streakBuffer.Enqueue(currentStreak);
        }
        else
        {
            // Calculate streak of up or down
            if (double.IsNaN(currentValue) || double.IsNaN(prevValue))
            {
                streak = double.NaN;
            }
            else if (currentValue > prevValue)
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
            else if (currentValue < prevValue)
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

            currentStreak = streak;
            streakBuffer.Enqueue(currentStreak);
        }

        // Calculate RSI of streak (manual Wilder's smoothing)
        double? rsiStreak = null;

        if (i >= StreakPeriods + 2)
        {
            double prevStreak = streakBuffer.ElementAt(streakBuffer.Count - 2);
            double streakGain = currentStreak > prevStreak ? currentStreak - prevStreak : 0;
            double streakLoss = currentStreak < prevStreak ? prevStreak - currentStreak : 0;

            // Initialize or update streak RSI averages
            // Need at least StreakPeriods + 1 streak values to calculate first RSI
            if (i == StreakPeriods + 2 && (double.IsNaN(streakAvgGain) || double.IsNaN(streakAvgLoss)))
            {
                // Initial SMA calculation over last StreakPeriods + 1 values
                double sumGain = 0;
                double sumLoss = 0;

                // Calculate gain/loss for last StreakPeriods pairs
                // We need pairs from index (i - StreakPeriods) to i
                int startIdx = i - StreakPeriods;
                for (int p = startIdx; p < i; p++)
                {
                    double s1 = streakBuffer.ElementAt(p);
                    double s2 = streakBuffer.ElementAt(p + 1);
                    sumGain += s2 > s1 ? s2 - s1 : 0;
                    sumLoss += s2 < s1 ? s1 - s2 : 0;
                }

                streakAvgGain = sumGain / StreakPeriods;
                streakAvgLoss = sumLoss / StreakPeriods;

                rsiStreak = streakAvgLoss > 0
                    ? 100 - (100 / (1 + (streakAvgGain / streakAvgLoss)))
                    : 100;
            }
            else if (i > StreakPeriods + 2 && !double.IsNaN(streakAvgGain) && !double.IsNaN(streakAvgLoss))
            {
                // Wilder's smoothing (EMA-style update)
                streakAvgGain = ((streakAvgGain * (StreakPeriods - 1)) + streakGain) / StreakPeriods;
                streakAvgLoss = ((streakAvgLoss * (StreakPeriods - 1)) + streakLoss) / StreakPeriods;

                rsiStreak = streakAvgLoss > 0
                    ? 100 - (100 / (1 + (streakAvgGain / streakAvgLoss)))
                    : 100;
            }
        }

        // Calculate gain for percent rank
        double gain = double.IsNaN(currentValue) || double.IsNaN(prevValue) || prevValue <= 0
            ? double.NaN
            : (currentValue - prevValue) / prevValue;

        gainBuffer.Enqueue(gain);
        if (gainBuffer.Count > RankPeriods + 1)
        {
            gainBuffer.Dequeue();
        }

        // Calculate percent rank
        double? percentRank = null;
        if (i >= RankPeriods && !double.IsNaN(gain))
        {
            int qty = 0;
            bool isViableRank = true;

            foreach (double g in gainBuffer)
            {
                if (double.IsNaN(g))
                {
                    isViableRank = false;
                    break;
                }

                if (g < gain)
                {
                    qty++;
                }
            }

            percentRank = isViableRank ? 100.0 * qty / RankPeriods : null;
        }

        // Calculate ConnorsRsi
        double? connorsRsi = null;
        int startPeriod = Math.Max(RsiPeriods, Math.Max(StreakPeriods, RankPeriods)) + 2;

        if (i >= startPeriod - 1 && rsi.HasValue && rsiStreak.HasValue && percentRank.HasValue)
        {
            connorsRsi = (rsi.Value + rsiStreak.Value + percentRank.Value) / 3;
        }

        // Update previous value
        prevValue = currentValue;

        // Candidate result
        ConnorsRsiResult r = new(
            Timestamp: item.Timestamp,
            Streak: currentStreak,
            Rsi: rsi,
            RsiStreak: rsiStreak,
            PercentRank: percentRank,
            ConnorsRsi: connorsRsi);

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset state
        streak = 0;
        prevValue = double.NaN;
        streakAvgGain = double.NaN;
        streakAvgLoss = double.NaN;
        gainBuffer.Clear();
        streakBuffer.Clear();

        // Restore state from cache up to the timestamp
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0) return;

        int targetIndex = index - 1;

        // Replay values to restore state
        for (int i = 0; i <= targetIndex; i++)
        {
            IReusable item = ProviderCache[i];
            double value = item.Value;

            // Restore streak
            if (i == 0)
            {
                prevValue = value;
                streak = 0;
                streakBuffer.Enqueue(0);
            }
            else
            {
                // Calculate streak
                if (double.IsNaN(value) || double.IsNaN(prevValue))
                {
                    streak = double.NaN;
                }
                else if (value > prevValue)
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
                else if (value < prevValue)
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

                streakBuffer.Enqueue(streak);

                // Restore streak RSI state
                if (i >= StreakPeriods + 2)
                {
                    double prevStreak = streakBuffer.ElementAt(streakBuffer.Count - 2);
                    double streakGain = streak > prevStreak ? streak - prevStreak : 0;
                    double streakLoss = streak < prevStreak ? prevStreak - streak : 0;

                    if (i == StreakPeriods + 2 && (double.IsNaN(streakAvgGain) || double.IsNaN(streakAvgLoss)))
                    {
                        // Initial SMA calculation
                        double sumGain = 0;
                        double sumLoss = 0;

                        int startIdx = i - StreakPeriods;
                        for (int p = startIdx; p < i; p++)
                        {
                            double s1 = streakBuffer.ElementAt(p);
                            double s2 = streakBuffer.ElementAt(p + 1);
                            sumGain += s2 > s1 ? s2 - s1 : 0;
                            sumLoss += s2 < s1 ? s1 - s2 : 0;
                        }

                        streakAvgGain = sumGain / StreakPeriods;
                        streakAvgLoss = sumLoss / StreakPeriods;
                    }
                    else if (i > StreakPeriods + 2 && !double.IsNaN(streakAvgGain) && !double.IsNaN(streakAvgLoss))
                    {
                        // Wilder's smoothing
                        streakAvgGain = ((streakAvgGain * (StreakPeriods - 1)) + streakGain) / StreakPeriods;
                        streakAvgLoss = ((streakAvgLoss * (StreakPeriods - 1)) + streakLoss) / StreakPeriods;
                    }
                }

                // Restore gain buffer
                double gain = double.IsNaN(value) || double.IsNaN(prevValue) || prevValue <= 0
                    ? double.NaN
                    : (value - prevValue) / prevValue;

                gainBuffer.Enqueue(gain);
                if (gainBuffer.Count > RankPeriods + 1)
                {
                    gainBuffer.Dequeue();
                }

                prevValue = value;
            }
        }
    }
}


public static partial class ConnorsRsi
{
    /// <summary>
    /// Creates a ConnorsRsi streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="rsiPeriods">The number of periods to use for the RSI calculation. Default is 3.</param>
    /// <param name="streakPeriods">The number of periods to use for the streak calculation. Default is 2.</param>
    /// <param name="rankPeriods">The number of periods to use for the percent rank calculation. Default is 100.</param>
    /// <returns>A ConnorsRsi hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    public static ConnorsRsiHub ToConnorsRsiHub(
        this IChainProvider<IReusable> chainProvider,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
        => new(chainProvider, rsiPeriods, streakPeriods, rankPeriods);

    /// <summary>
    /// Creates a ConnorsRsi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="rsiPeriods">The number of periods to use for the RSI calculation. Default is 3.</param>
    /// <param name="streakPeriods">The number of periods to use for the streak calculation. Default is 2.</param>
    /// <param name="rankPeriods">The number of periods to use for the percent rank calculation. Default is 100.</param>
    /// <returns>An instance of <see cref="ConnorsRsiHub"/>.</returns>
    public static ConnorsRsiHub ToConnorsRsiHub(
        this IReadOnlyList<IQuote> quotes,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToConnorsRsiHub(rsiPeriods, streakPeriods, rankPeriods);
    }
}
