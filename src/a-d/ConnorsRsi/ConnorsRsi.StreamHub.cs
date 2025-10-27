namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Connors RSI calculations.
/// </summary>
public class ConnorsRsiHub
    : ChainProvider<IReusable, ConnorsRsiResult>, IConnorsRsi
{
    private readonly string hubName;
    private readonly RsiHub _rsiClose;
    private readonly Queue<double> _gainBuffer;
    private double _prevValue = double.NaN;
    private double _streak;

    // State for RSI of streak calculation (Wilder's smoothing)
    private double _avgStreakGain = double.NaN;
    private double _avgStreakLoss = double.NaN;
    private double _prevStreak = double.NaN;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnorsRsiHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation on close prices.</param>
    /// <param name="streakPeriods">The number of periods for the RSI calculation on streak.</param>
    /// <param name="rankPeriods">The number of periods for the percent rank calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any periods are invalid.</exception>
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

        // Create internal RSI hub for close price
        _rsiClose = provider.ToRsiHub(rsiPeriods);

        _gainBuffer = new Queue<double>(rankPeriods + 1);

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

        // Calculate streak
        double streak = CalculateStreak(item.Value, i);

        // Get RSI of close
        RsiResult rsiCloseResult = _rsiClose.Results[i];
        double? rsi = rsiCloseResult.Rsi;

        // Calculate RSI of streak
        double? rsiStreak = CalculateRsiOfStreak(streak, i);

        // Calculate gain and percent rank
        double gain = CalculateGain(item.Value, i);
        _gainBuffer.Update(RankPeriods + 1, gain);
        double? percentRank = CalculatePercentRank(gain, i);

        // Calculate Connors RSI
        double? connorsRsi = CalculateConnorsRsi(rsi, rsiStreak, percentRank, i);

        // candidate result
        ConnorsRsiResult r = new(
            Timestamp: item.Timestamp,
            Streak: streak,
            Rsi: rsi,
            RsiStreak: rsiStreak,
            PercentRank: percentRank,
            ConnorsRsi: connorsRsi);

        // Update state for next iteration
        _prevValue = item.Value;
        _prevStreak = streak;

        return (r, i);
    }

    private double CalculateStreak(double value, int index)
    {
        if (index == 0)
        {
            _streak = 0;
            return _streak;
        }

        if (double.IsNaN(value) || double.IsNaN(_prevValue))
        {
            _streak = double.NaN;
        }
        else if (value > _prevValue)
        {
            _streak = _streak >= 0 ? _streak + 1 : 1;
        }
        else if (value < _prevValue)
        {
            _streak = _streak <= 0 ? _streak - 1 : -1;
        }
        else
        {
            _streak = 0;
        }

        return _streak;
    }

    private double? CalculateRsiOfStreak(double streak, int index)
    {
        if (index < StreakPeriods)
        {
            return null;
        }

        double? rsi = null;

        // Get current gain/loss from streak
        double streakGain;
        double streakLoss;
        if (!double.IsNaN(streak) && !double.IsNaN(_prevStreak))
        {
            streakGain = streak > _prevStreak ? streak - _prevStreak : 0;
            streakLoss = streak < _prevStreak ? _prevStreak - streak : 0;
        }
        else
        {
            streakGain = streakLoss = double.NaN;
        }

        // Initialize average gain/loss when needed
        if (index >= StreakPeriods && (double.IsNaN(_avgStreakGain) || double.IsNaN(_avgStreakLoss)))
        {
            double sumGain = 0;
            double sumLoss = 0;

            // Calculate streaks from cache for initial period
            List<double> streaks = new();
            for (int p = 0; p <= index; p++)
            {
                double pValue = ProviderCache[p].Value;
                double pPrevValue = p > 0 ? ProviderCache[p - 1].Value : double.NaN;

                double pStreak;
                if (p == 0)
                {
                    pStreak = 0;
                }
                else
                {
                    double prevStreakValue = streaks[p - 1];
                    if (double.IsNaN(pValue) || double.IsNaN(pPrevValue))
                    {
                        pStreak = double.NaN;
                    }
                    else if (pValue > pPrevValue)
                    {
                        pStreak = prevStreakValue >= 0 ? prevStreakValue + 1 : 1;
                    }
                    else if (pValue < pPrevValue)
                    {
                        pStreak = prevStreakValue <= 0 ? prevStreakValue - 1 : -1;
                    }
                    else
                    {
                        pStreak = 0;
                    }
                }
                streaks.Add(pStreak);
            }

            // Calculate gains/losses from streaks
            for (int p = index - StreakPeriods + 1; p <= index; p++)
            {
                double pStreak = streaks[p];
                double pPrevStreak = p > 0 ? streaks[p - 1] : double.NaN;

                double pGain;
                double pLoss;
                if (!double.IsNaN(pStreak) && !double.IsNaN(pPrevStreak))
                {
                    pGain = pStreak > pPrevStreak ? pStreak - pPrevStreak : 0;
                    pLoss = pStreak < pPrevStreak ? pPrevStreak - pStreak : 0;
                }
                else
                {
                    pGain = pLoss = double.NaN;
                }

                sumGain += pGain;
                sumLoss += pLoss;
            }

            _avgStreakGain = sumGain / StreakPeriods;
            _avgStreakLoss = sumLoss / StreakPeriods;

            rsi = !double.IsNaN(_avgStreakGain / _avgStreakLoss)
                  ? _avgStreakLoss > 0 ? 100 - (100 / (1 + (_avgStreakGain / _avgStreakLoss))) : 100
                  : null;
        }
        // Calculate RSI incrementally
        else if (index > StreakPeriods && !double.IsNaN(_avgStreakGain) && !double.IsNaN(_avgStreakLoss))
        {
            if (!double.IsNaN(streakGain))
            {
                _avgStreakGain = ((_avgStreakGain * (StreakPeriods - 1)) + streakGain) / StreakPeriods;
                _avgStreakLoss = ((_avgStreakLoss * (StreakPeriods - 1)) + streakLoss) / StreakPeriods;

                if (_avgStreakLoss > 0)
                {
                    double rs = _avgStreakGain / _avgStreakLoss;
                    rsi = 100 - (100 / (1 + rs));
                }
                else
                {
                    rsi = 100;
                }
            }
            else
            {
                // Reset state if we hit NaN
                _avgStreakGain = double.NaN;
                _avgStreakLoss = double.NaN;
            }
        }

        return rsi;
    }

    private double CalculateGain(double value, int index)
    {
        if (index == 0)
        {
            return double.NaN;
        }

        return double.IsNaN(value) || double.IsNaN(_prevValue) || _prevValue <= 0
            ? double.NaN
            : (value - _prevValue) / _prevValue;
    }

    private double? CalculatePercentRank(double gain, int index)
    {
        if (index < RankPeriods || double.IsNaN(gain))
        {
            return null;
        }

        int qty = 0;
        foreach (double g in _gainBuffer)
        {
            if (double.IsNaN(g))
            {
                return null;
            }

            if (g < gain)
            {
                qty++;
            }
        }

        return 100.0 * qty / RankPeriods;
    }

    private double? CalculateConnorsRsi(double? rsi, double? rsiStreak, double? percentRank, int index)
    {
        int startPeriod = Math.Max(RsiPeriods, Math.Max(StreakPeriods, RankPeriods)) + 2;

        return index >= startPeriod - 1 && rsi.HasValue && rsiStreak.HasValue && percentRank.HasValue
            ? (rsi.Value + rsiStreak.Value + percentRank.Value) / 3
            : null;
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear streak state - will be recalculated during rebuild
        _prevValue = double.NaN;
        _prevStreak = double.NaN;
        _streak = 0;

        // Clear RSI of streak state
        _avgStreakGain = double.NaN;
        _avgStreakLoss = double.NaN;

        // Clear gain buffer
        _gainBuffer.Clear();

        // Rebuild state from ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0) return;

        int targetIndex = index - 1;

        // Rebuild streak and gain buffer state
        for (int p = 0; p <= targetIndex; p++)
        {
            IReusable item = ProviderCache[p];
            double value = item.Value;

            // Recalculate streak
            if (p == 0)
            {
                _streak = 0;
                _prevValue = value;
                _prevStreak = 0;
            }
            else
            {
                if (double.IsNaN(value) || double.IsNaN(_prevValue))
                {
                    _streak = double.NaN;
                }
                else if (value > _prevValue)
                {
                    _streak = _streak >= 0 ? _streak + 1 : 1;
                }
                else if (value < _prevValue)
                {
                    _streak = _streak <= 0 ? _streak - 1 : -1;
                }
                else
                {
                    _streak = 0;
                }

                // Recalculate gain
                double gain = double.IsNaN(value) || double.IsNaN(_prevValue) || _prevValue <= 0
                    ? double.NaN
                    : (value - _prevValue) / _prevValue;

                // Add to gain buffer (limited to buffer size)
                if (p >= targetIndex - RankPeriods)
                {
                    _gainBuffer.Enqueue(gain);
                }

                _prevValue = value;
                _prevStreak = _streak;
            }
        }
    }
}

public static partial class ConnorsRsi
{
    /// <summary>
    /// Creates a Connors RSI streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation on close prices.</param>
    /// <param name="streakPeriods">The number of periods for the RSI calculation on streak.</param>
    /// <param name="rankPeriods">The number of periods for the percent rank calculation.</param>
    /// <returns>A Connors RSI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any periods are invalid.</exception>
    public static ConnorsRsiHub ToConnorsRsiHub(
        this IChainProvider<IReusable> chainProvider,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
        => new(chainProvider, rsiPeriods, streakPeriods, rankPeriods);

    /// <summary>
    /// Creates a Connors RSI hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation on close prices.</param>
    /// <param name="streakPeriods">The number of periods for the RSI calculation on streak.</param>
    /// <param name="rankPeriods">The number of periods for the percent rank calculation.</param>
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
