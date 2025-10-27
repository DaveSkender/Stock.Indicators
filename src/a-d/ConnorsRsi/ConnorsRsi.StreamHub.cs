namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Connors RSI calculations.
/// </summary>
public class ConnorsRsiHub
    : ChainProvider<IReusable, ConnorsRsiResult>, IConnorsRsi
{
    private readonly string hubName;
    private readonly Queue<double> _gainBuffer;
    private readonly Queue<double> _streakBuffer;
    private double _prevValue = double.NaN;
    private double _streak;
    private int _processedCount;

    // State for RSI of close calculation (Wilder's smoothing)
    private double _avgGain = double.NaN;
    private double _avgLoss = double.NaN;

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

        _gainBuffer = new Queue<double>(rankPeriods + 1);
        _streakBuffer = new Queue<double>(streakPeriods + 1);

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
        double streak = CalculateStreak(item.Value, _processedCount);
        _streakBuffer.Update(StreakPeriods + 1, streak);

        // Calculate RSI of close
        double? rsi = CalculateRsiOfClose(item.Value, _processedCount);

        // Calculate RSI of streak
        double? rsiStreak = CalculateRsiOfStreak(streak, _processedCount);

        // Calculate gain and percent rank
        double gain = CalculateGain(item.Value, _processedCount);
        _gainBuffer.Update(RankPeriods + 1, gain);
        double? percentRank = CalculatePercentRank(gain, _processedCount);

        // Calculate Connors RSI
        double? connorsRsi = CalculateConnorsRsi(rsi, rsiStreak, percentRank, _processedCount);

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
        _processedCount++;

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

    private double? CalculateRsiOfClose(double value, int index)
    {
        if (index < RsiPeriods)
        {
            return null;
        }

        double? rsi = null;

        // Get current gain/loss
        double gain;
        double loss;
        if (!double.IsNaN(value) && !double.IsNaN(_prevValue))
        {
            gain = value > _prevValue ? value - _prevValue : 0;
            loss = value < _prevValue ? _prevValue - value : 0;
        }
        else
        {
            gain = loss = double.NaN;
        }

        // Initialize average gain/loss when needed
        if (index >= RsiPeriods && (double.IsNaN(_avgGain) || double.IsNaN(_avgLoss)))
        {
            double sumGain = 0;
            double sumLoss = 0;

            // Sum gains and losses over lookback period
            for (int p = index - RsiPeriods + 1; p <= index; p++)
            {
                double pPrevVal = ProviderCache[p - 1].Value;
                double pCurrVal = ProviderCache[p].Value;
                double pGain;
                double pLoss;
                if (!double.IsNaN(pCurrVal) && !double.IsNaN(pPrevVal))
                {
                    pGain = pCurrVal > pPrevVal ? pCurrVal - pPrevVal : 0;
                    pLoss = pCurrVal < pPrevVal ? pPrevVal - pCurrVal : 0;
                }
                else
                {
                    pGain = pLoss = double.NaN;
                }

                sumGain += pGain;
                sumLoss += pLoss;
            }

            _avgGain = sumGain / RsiPeriods;
            _avgLoss = sumLoss / RsiPeriods;

            rsi = !double.IsNaN(_avgGain / _avgLoss)
                  ? _avgLoss > 0 ? 100 - (100 / (1 + (_avgGain / _avgLoss))) : 100
                  : null;
        }
        // Calculate RSI incrementally
        else if (index > RsiPeriods && !double.IsNaN(_avgGain) && !double.IsNaN(_avgLoss))
        {
            if (!double.IsNaN(gain))
            {
                _avgGain = ((_avgGain * (RsiPeriods - 1)) + gain) / RsiPeriods;
                _avgLoss = ((_avgLoss * (RsiPeriods - 1)) + loss) / RsiPeriods;

                if (_avgLoss > 0)
                {
                    double rs = _avgGain / _avgLoss;
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
                _avgGain = double.NaN;
                _avgLoss = double.NaN;
            }
        }

        return rsi;
    }

    private double? CalculateRsiOfStreak(double streak, int processedCount)
    {
        // RSI of streak needs StreakPeriods + 2 periods minimum  
        // to produce a final ConnorsRSI value, but RSI itself starts earlier
        if (processedCount < StreakPeriods + 2)
        {
            // Still calculate RSI but return null until we reach the threshold
            // This ensures the state is properly initialized
            if (processedCount >= StreakPeriods)
            {
                // We have enough streaks to calculate RSI, just don't return it yet
                CalculateRsiOfStreakInternal(streak, processedCount);
            }
            return null;
        }

        return CalculateRsiOfStreakInternal(streak, processedCount);
    }

    private double? CalculateRsiOfStreakInternal(double streak, int processedCount)
    {
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
        if (processedCount >= StreakPeriods && (double.IsNaN(_avgStreakGain) || double.IsNaN(_avgStreakLoss)))
        {
            double sumGain = 0;
            double sumLoss = 0;

            // Calculate gains/losses from streak buffer
            // We have StreakPeriods + 1 values in buffer, need to calculate StreakPeriods pairs
            double[] streaks = _streakBuffer.ToArray();

            // For StreakPeriods=2, we need 2 pairs from buffer of size 3
            // pairs: (streaks[1]-streaks[0]), (streaks[2]-streaks[1])
            for (int p = 1; p < streaks.Length; p++)
            {
                double pStreak = streaks[p];
                double pPrevStreak = streaks[p - 1];

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
        else if (processedCount > StreakPeriods && !double.IsNaN(_avgStreakGain) && !double.IsNaN(_avgStreakLoss))
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
        _processedCount = 0;

        // Clear RSI of close state
        _avgGain = double.NaN;
        _avgLoss = double.NaN;

        // Clear RSI of streak state
        _avgStreakGain = double.NaN;
        _avgStreakLoss = double.NaN;

        // Clear buffers
        _gainBuffer.Clear();
        _streakBuffer.Clear();

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
                _processedCount = 1;
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
                _processedCount++;
            }

            // Add to streak buffer (keep last StreakPeriods + 1 values)
            if (p >= targetIndex - StreakPeriods)
            {
                _streakBuffer.Enqueue(_streak);
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
