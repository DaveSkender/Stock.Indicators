namespace Skender.Stock.Indicators;

/// <summary>
/// State for ConnorsRSI calculations using StreamHubState pattern.
/// </summary>
/// <param name="Streak">Current streak value</param>
/// <param name="PrevValue">Previous close value</param>
/// <param name="StreakAvgGain">Average gain for streak RSI</param>
/// <param name="StreakAvgLoss">Average loss for streak RSI</param>
public record ConnorsRsiState(
    double Streak,
    double PrevValue,
    double StreakAvgGain,
    double StreakAvgLoss) : IHubState;

/// <summary>
/// Streaming hub for Connors RSI using state management.
/// </summary>
public class ConnorsRsiHubState
    : ChainHubState<IReusable, ConnorsRsiState, ConnorsRsiResult>, IConnorsRsi
{
    private readonly RsiHub rsiHub;
    private readonly List<double> streakBuffer;
    private readonly Queue<double> gainBuffer;
    private double streak;
    private double prevValue;
    private double streakAvgGain;
    private double streakAvgLoss;

    internal ConnorsRsiHubState(
        IChainProvider<IReusable> provider,
        int rsiPeriods,
        int streakPeriods,
        int rankPeriods) : base(provider)
    {
        ConnorsRsi.Validate(rsiPeriods, streakPeriods, rankPeriods);
        RsiPeriods = rsiPeriods;
        StreakPeriods = streakPeriods;
        RankPeriods = rankPeriods;

        Name = $"CRSI({rsiPeriods},{streakPeriods},{rankPeriods})";

        rsiHub = provider.ToRsiHub(rsiPeriods);
        streakBuffer = [];
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
    protected override void RestorePreviousState(ConnorsRsiState? previousState)
    {
        if (previousState is null)
        {
            streak = 0;
            prevValue = double.NaN;
            streakAvgGain = double.NaN;
            streakAvgLoss = double.NaN;
            gainBuffer.Clear();
            streakBuffer.Clear();
        }
        else
        {
            streak = previousState.Streak;
            prevValue = previousState.PrevValue;
            streakAvgGain = previousState.StreakAvgGain;
            streakAvgLoss = previousState.StreakAvgLoss;
        }
    }

    /// <inheritdoc/>
    protected override (ConnorsRsiResult result, ConnorsRsiState state, int index)
        ToIndicatorState(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? rsi = rsiHub.Results[i].Rsi;
        double currentValue = item.Value;
        double currentStreak;

        if (i == 0)
        {
            prevValue = currentValue;
            currentStreak = 0;
            streakBuffer.Add(currentStreak);
        }
        else
        {
            if (double.IsNaN(currentValue) || double.IsNaN(prevValue))
            {
                streak = double.NaN;
            }
            else if (currentValue > prevValue)
            {
                streak = streak >= 0 ? streak + 1 : 1;
            }
            else if (currentValue < prevValue)
            {
                streak = streak <= 0 ? streak - 1 : -1;
            }
            else
            {
                streak = 0;
            }

            currentStreak = streak;
            streakBuffer.Add(currentStreak);
        }

        double? rsiStreak = null;
        const int rsiWarmupOffset = 2;

        if (i > 0 && !double.IsNaN(currentStreak))
        {
            double prevStreak = streakBuffer[i - 1];

            if (!double.IsNaN(prevStreak))
            {
                double streakGain = currentStreak > prevStreak ? currentStreak - prevStreak : 0;
                double streakLoss = currentStreak < prevStreak ? prevStreak - currentStreak : 0;

                if (i >= StreakPeriods && (double.IsNaN(streakAvgGain) || double.IsNaN(streakAvgLoss)))
                {
                    double sumGain = 0, sumLoss = 0;
                    bool hasValidStreaks = true;

                    for (int p = i - StreakPeriods + 1; p <= i; p++)
                    {
                        double s1 = streakBuffer[p - 1];
                        double s2 = streakBuffer[p];

                        if (double.IsNaN(s1) || double.IsNaN(s2))
                        {
                            hasValidStreaks = false;
                            break;
                        }

                        sumGain += s2 > s1 ? s2 - s1 : 0;
                        sumLoss += s2 < s1 ? s1 - s2 : 0;
                    }

                    if (hasValidStreaks)
                    {
                        streakAvgGain = sumGain / StreakPeriods;
                        streakAvgLoss = sumLoss / StreakPeriods;

                        if (i >= StreakPeriods + rsiWarmupOffset)
                        {
                            rsiStreak = streakAvgLoss > 0
                                ? 100 - (100 / (1 + (streakAvgGain / streakAvgLoss)))
                                : 100;
                        }
                    }
                }
                else if (i > StreakPeriods && !double.IsNaN(streakAvgGain) && !double.IsNaN(streakAvgLoss))
                {
                    streakAvgGain = ((streakAvgGain * (StreakPeriods - 1)) + streakGain) / StreakPeriods;
                    streakAvgLoss = ((streakAvgLoss * (StreakPeriods - 1)) + streakLoss) / StreakPeriods;

                    if (i >= StreakPeriods + rsiWarmupOffset)
                    {
                        rsiStreak = streakAvgLoss > 0
                            ? 100 - (100 / (1 + (streakAvgGain / streakAvgLoss)))
                            : 100;
                    }
                }
            }
        }

        double gain = double.IsNaN(currentValue) || double.IsNaN(prevValue) || prevValue <= 0
            ? double.NaN
            : (currentValue - prevValue) / prevValue;

        gainBuffer.Enqueue(gain);
        if (gainBuffer.Count > RankPeriods + 1)
        {
            gainBuffer.Dequeue();
        }

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

        double? connorsRsi = null;
        int startPeriod = Math.Max(RsiPeriods, Math.Max(StreakPeriods, RankPeriods)) + rsiWarmupOffset;

        if (i >= startPeriod - 1 && rsi.HasValue && rsiStreak.HasValue && percentRank.HasValue)
        {
            connorsRsi = (rsi.Value + rsiStreak.Value + percentRank.Value) / 3;
        }

        prevValue = currentValue;

        ConnorsRsiResult r = new(
            Timestamp: item.Timestamp,
            Streak: currentStreak,
            Rsi: rsi,
            RsiStreak: rsiStreak,
            PercentRank: percentRank,
            ConnorsRsi: connorsRsi);

        ConnorsRsiState currentState = new(streak, prevValue, streakAvgGain, streakAvgLoss);
        return (r, currentState, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        base.RollbackState(timestamp);

        streak = 0;
        prevValue = double.NaN;
        streakAvgGain = double.NaN;
        streakAvgLoss = double.NaN;
        gainBuffer.Clear();
        streakBuffer.Clear();

        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        RestoreStateIfNeeded(index - 1);
    }

    private void RestoreStateIfNeeded(int targetIndex)
    {
        for (int i = 0; i <= targetIndex; i++)
        {
            IReusable item = ProviderCache[i];
            double value = item.Value;

            if (i == 0)
            {
                prevValue = value;
                streak = 0;
                streakBuffer.Add(0);
            }
            else
            {
                if (double.IsNaN(value) || double.IsNaN(prevValue))
                {
                    streak = double.NaN;
                }
                else if (value > prevValue)
                {
                    streak = streak >= 0 ? streak + 1 : 1;
                }
                else if (value < prevValue)
                {
                    streak = streak <= 0 ? streak - 1 : -1;
                }
                else
                {
                    streak = 0;
                }

                streakBuffer.Add(streak);

                if (i > 0 && !double.IsNaN(streak))
                {
                    double prevStreak = streakBuffer[i - 1];

                    if (!double.IsNaN(prevStreak))
                    {
                        double streakGain = streak > prevStreak ? streak - prevStreak : 0;
                        double streakLoss = streak < prevStreak ? prevStreak - streak : 0;

                        if (i >= StreakPeriods && (double.IsNaN(streakAvgGain) || double.IsNaN(streakAvgLoss)))
                        {
                            double sumGain = 0, sumLoss = 0;
                            bool hasValidStreaks = true;

                            for (int p = i - StreakPeriods + 1; p <= i; p++)
                            {
                                double s1 = streakBuffer[p - 1];
                                double s2 = streakBuffer[p];

                                if (double.IsNaN(s1) || double.IsNaN(s2))
                                {
                                    hasValidStreaks = false;
                                    break;
                                }

                                sumGain += s2 > s1 ? s2 - s1 : 0;
                                sumLoss += s2 < s1 ? s1 - s2 : 0;
                            }

                            if (hasValidStreaks)
                            {
                                streakAvgGain = sumGain / StreakPeriods;
                                streakAvgLoss = sumLoss / StreakPeriods;
                            }
                        }
                        else if (i > StreakPeriods && !double.IsNaN(streakAvgGain) && !double.IsNaN(streakAvgLoss))
                        {
                            streakAvgGain = ((streakAvgGain * (StreakPeriods - 1)) + streakGain) / StreakPeriods;
                            streakAvgLoss = ((streakAvgLoss * (StreakPeriods - 1)) + streakLoss) / StreakPeriods;
                        }
                    }
                }

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
    /// Creates a ConnorsRsi streaming hub with state management from a chain provider.
    /// </summary>
    public static ConnorsRsiHubState ToConnorsRsiHubState(
        this IChainProvider<IReusable> chainProvider,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
        => new(chainProvider, rsiPeriods, streakPeriods, rankPeriods);
}
