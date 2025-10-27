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
        double streak = CalculateStreak(item.Value, i);
        _streakBuffer.Update(StreakPeriods + 1, streak);

        // Calculate RSI of close
        double? rsi = CalculateRsiOfClose(item.Value, i);

        // Calculate RSI of streak
        double? rsiStreak = CalculateRsiOfStreak(streak, i);

        // Calculate gain and percent rank
        double gain = CalculateGain(item.Value, i);

        // Handle first item - add gain[0] = 0.0 to match Series behavior
        // but don't add the calculated gain which would be NaN
        if (i == 0)
        {
            if (_gainBuffer.Count == 0)
            {
                _gainBuffer.Enqueue(0.0);
            }
        }
        else
        {
            _gainBuffer.Update(RankPeriods + 1, gain);
        }

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

        (double gain, double loss) = Rsi.ComputeGainLoss(value, _prevValue);

        // Initialize average gain/loss when needed
        if (index >= RsiPeriods && (double.IsNaN(_avgGain) || double.IsNaN(_avgLoss)))
        {
            InitializeRsiOfClose(index);
            return Rsi.CalculateRsiValue(_avgGain, _avgLoss);
        }

        // Calculate RSI incrementally
        if (index > RsiPeriods && !double.IsNaN(_avgGain) && !double.IsNaN(_avgLoss))
        {
            return UpdateRsiOfClose(gain, loss);
        }

        return null;
    }

    private void InitializeRsiOfClose(int index)
    {
        double sumGain = 0;
        double sumLoss = 0;

        for (int p = index - RsiPeriods + 1; p <= index; p++)
        {
            (double pGain, double pLoss) = Rsi.ComputeGainLoss(
                ProviderCache[p].Value,
                ProviderCache[p - 1].Value);

            sumGain += pGain;
            sumLoss += pLoss;
        }

        _avgGain = sumGain / RsiPeriods;
        _avgLoss = sumLoss / RsiPeriods;
    }

    private double? UpdateRsiOfClose(double gain, double loss)
    {
        (_avgGain, _avgLoss) = Rsi.ApplyWilderSmoothing(_avgGain, _avgLoss, gain, loss, RsiPeriods);

        if (double.IsNaN(_avgGain))
        {
            return null;
        }

        return Rsi.CalculateRsiValue(_avgGain, _avgLoss);
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
        (double streakGain, double streakLoss) = Rsi.ComputeGainLoss(streak, _prevStreak);

        // Initialize average gain/loss when needed
        if (processedCount >= StreakPeriods && (double.IsNaN(_avgStreakGain) || double.IsNaN(_avgStreakLoss)))
        {
            InitializeRsiOfStreak();
            return Rsi.CalculateRsiValue(_avgStreakGain, _avgStreakLoss);
        }

        // Calculate RSI incrementally
        if (processedCount > StreakPeriods && !double.IsNaN(_avgStreakGain) && !double.IsNaN(_avgStreakLoss))
        {
            return UpdateRsiOfStreak(streakGain, streakLoss);
        }

        return null;
    }

    private void InitializeRsiOfStreak()
    {
        double sumGain = 0;
        double sumLoss = 0;

        double[] streaks = _streakBuffer.ToArray();

        for (int p = 1; p < streaks.Length; p++)
        {
            (double pGain, double pLoss) = Rsi.ComputeGainLoss(streaks[p], streaks[p - 1]);
            sumGain += pGain;
            sumLoss += pLoss;
        }

        _avgStreakGain = sumGain / StreakPeriods;
        _avgStreakLoss = sumLoss / StreakPeriods;
    }

    private double? UpdateRsiOfStreak(double streakGain, double streakLoss)
    {
        (_avgStreakGain, _avgStreakLoss) = Rsi.ApplyWilderSmoothing(
            _avgStreakGain, _avgStreakLoss, streakGain, streakLoss, StreakPeriods);

        if (double.IsNaN(_avgStreakGain))
        {
            return null;
        }

        return Rsi.CalculateRsiValue(_avgStreakGain, _avgStreakLoss);
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
        // Match Series logic: need index > rankPeriods - 1
        if (index <= RankPeriods - 1 || double.IsNaN(gain))
        {
            return null;
        }

        // Check if buffer has enough items
        // After adding gain[index], buffer should have items from gain[0] to gain[index]
        // For index=100 with rankPeriods=100, we need 101 items (gain[0] through gain[100])
        int requiredCount = Math.Min(index + 1, RankPeriods + 1);
        if (_gainBuffer.Count < requiredCount)
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
        ClearState();

        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        int targetIndex = index - 1;
        List<double> allStreaks = RebuildBuffers(targetIndex);

        // Restore Wilder smoothed averages
        RestoreRsiOfCloseState(targetIndex);
        RestoreRsiOfStreakState(allStreaks);
    }

    private void ClearState()
    {
        _prevValue = double.NaN;
        _prevStreak = double.NaN;
        _streak = 0;
        _processedCount = 0;
        _avgGain = double.NaN;
        _avgLoss = double.NaN;
        _avgStreakGain = double.NaN;
        _avgStreakLoss = double.NaN;
        _gainBuffer.Clear();
        _streakBuffer.Clear();
    }

    private List<double> RebuildBuffers(int targetIndex)
    {
        List<double> allStreaks = [];
        const double gain0 = 0.0;
        bool hasGain0 = false;

        for (int p = 0; p <= targetIndex; p++)
        {
            IReusable item = ProviderCache[p];
            double value = item.Value;

            if (p == 0)
            {
                InitializeFirstStreakState(value, allStreaks);
                hasGain0 = true;
            }
            else
            {
                double prevValue = _prevValue; // Save before updating
                UpdateStreakState(value, allStreaks);
                UpdateGainBuffer(value, prevValue, p, targetIndex, hasGain0, gain0);
            }

            UpdateStreakBuffer(p, targetIndex);
        }

        return allStreaks;
    }

    private void InitializeFirstStreakState(double value, List<double> allStreaks)
    {
        _streak = 0;
        _prevValue = value;
        _prevStreak = 0;
        _processedCount = 1;
        allStreaks.Add(_streak);
    }

    private void UpdateStreakState(double value, List<double> allStreaks)
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

        allStreaks.Add(_streak);
        _prevValue = value;
        _prevStreak = _streak;
        _processedCount++;
    }

    private void UpdateGainBuffer(double value, double prevValue, int p, int targetIndex, bool hasGain0, double gain0)
    {
        double gain = double.IsNaN(value) || double.IsNaN(prevValue) || prevValue <= 0
            ? double.NaN
            : (value - prevValue) / prevValue;

        int bufferStartIndex = Math.Max(0, targetIndex - RankPeriods);
        if (p >= bufferStartIndex)
        {
            if (_gainBuffer.Count == 0 && hasGain0 && bufferStartIndex == 0)
            {
                _gainBuffer.Enqueue(gain0);
            }

            _gainBuffer.Enqueue(gain);
        }
    }

    private void UpdateStreakBuffer(int p, int targetIndex)
    {
        if (p >= targetIndex - StreakPeriods)
        {
            _streakBuffer.Enqueue(_streak);
        }
    }

    private void RestoreRsiOfCloseState(int targetIndex)
    {
        if (targetIndex < RsiPeriods)
        {
            return;
        }

        (double sumGain, double sumLoss) = ComputeInitialRsiSums();
        _avgGain = sumGain / RsiPeriods;
        _avgLoss = sumLoss / RsiPeriods;

        ApplyWilderSmoothingForClose(targetIndex);
    }

    private (double sumGain, double sumLoss) ComputeInitialRsiSums()
    {
        double sumGain = 0;
        double sumLoss = 0;

        for (int p = 1; p <= RsiPeriods; p++)
        {
            (double pGain, double pLoss) = Rsi.ComputeGainLoss(
                ProviderCache[p].Value,
                ProviderCache[p - 1].Value);

            sumGain += pGain;
            sumLoss += pLoss;
        }

        return (sumGain, sumLoss);
    }

    private void ApplyWilderSmoothingForClose(int targetIndex)
    {
        for (int p = RsiPeriods + 1; p <= targetIndex; p++)
        {
            (double pGain, double pLoss) = Rsi.ComputeGainLoss(
                ProviderCache[p].Value,
                ProviderCache[p - 1].Value);

            (_avgGain, _avgLoss) = Rsi.ApplyWilderSmoothing(
                _avgGain, _avgLoss, pGain, pLoss, RsiPeriods);

            if (double.IsNaN(_avgGain))
            {
                break;
            }
        }
    }

    private void RestoreRsiOfStreakState(List<double> allStreaks)
    {
        if (allStreaks.Count <= StreakPeriods)
        {
            return;
        }

        (double sumGain, double sumLoss) = ComputeInitialStreakRsiSums(allStreaks);
        _avgStreakGain = sumGain / StreakPeriods;
        _avgStreakLoss = sumLoss / StreakPeriods;

        ApplyWilderSmoothingForStreak(allStreaks);
    }

    private (double sumGain, double sumLoss) ComputeInitialStreakRsiSums(List<double> allStreaks)
    {
        double sumGain = 0;
        double sumLoss = 0;

        for (int p = 1; p <= StreakPeriods; p++)
        {
            (double pGain, double pLoss) = Rsi.ComputeGainLoss(allStreaks[p], allStreaks[p - 1]);
            sumGain += pGain;
            sumLoss += pLoss;
        }

        return (sumGain, sumLoss);
    }

    private void ApplyWilderSmoothingForStreak(List<double> allStreaks)
    {
        for (int p = StreakPeriods + 1; p < allStreaks.Count; p++)
        {
            (double pGain, double pLoss) = Rsi.ComputeGainLoss(allStreaks[p], allStreaks[p - 1]);

            (_avgStreakGain, _avgStreakLoss) = Rsi.ApplyWilderSmoothing(
                _avgStreakGain, _avgStreakLoss, pGain, pLoss, StreakPeriods);

            if (double.IsNaN(_avgStreakGain))
            {
                break;
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
