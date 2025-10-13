namespace Skender.Stock.Indicators;

/// <summary>
/// Connors RSI from incremental reusable values.
/// </summary>
public class ConnorsRsiList : BufferList<ConnorsRsiResult>, IIncrementFromChain
{
    private readonly RsiList _rsiClose;
    private readonly RsiList _rsiStreak;
    private readonly Queue<double> _gainBuffer;
    private double _prevValue = double.NaN;
    private double _streak;
    private int _processedCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnorsRsiList"/> class.
    /// </summary>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation on close prices.</param>
    /// <param name="streakPeriods">The number of periods for the RSI calculation on streak.</param>
    /// <param name="rankPeriods">The number of periods for the percent rank calculation.</param>
    public ConnorsRsiList(
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
    {
        ConnorsRsi.Validate(rsiPeriods, streakPeriods, rankPeriods);

        RsiPeriods = rsiPeriods;
        StreakPeriods = streakPeriods;
        RankPeriods = rankPeriods;

        _rsiClose = new RsiList(rsiPeriods);
        _rsiStreak = new RsiList(streakPeriods);
        _gainBuffer = new Queue<double>(rankPeriods + 1);
        _processedCount = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnorsRsiList"/> class with initial values.
    /// </summary>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation on close prices.</param>
    /// <param name="streakPeriods">The number of periods for the RSI calculation on streak.</param>
    /// <param name="rankPeriods">The number of periods for the percent rank calculation.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public ConnorsRsiList(
        int rsiPeriods,
        int streakPeriods,
        int rankPeriods,
        IReadOnlyList<IReusable> values)
        : this(rsiPeriods, streakPeriods, rankPeriods)
        => Add(values);

    /// <summary>
    /// Gets the number of periods for the RSI calculation on close prices.
    /// </summary>
    public int RsiPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the RSI calculation on streak.
    /// </summary>
    public int StreakPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the percent rank calculation.
    /// </summary>
    public int RankPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Calculate streak
        if (_processedCount == 0)
        {
            _streak = 0;
            _prevValue = value;
        }
        else
        {
            _streak = double.IsNaN(value) || double.IsNaN(_prevValue)
                ? double.NaN
                : value > _prevValue
                    ? _streak >= 0 ? _streak + 1 : 1
                    : value < _prevValue
                        ? _streak <= 0 ? _streak - 1 : -1
                        : 0;
        }

        // Calculate RSI on close
        _rsiClose.Add(timestamp, value);
        double? rsi = _rsiClose[^1].Rsi;

        // Calculate RSI on streak (but only populate after warmup)
        _rsiStreak.Add(timestamp, _streak);
        double? rsiStreak = _processedCount >= StreakPeriods + 2
            ? _rsiStreak[^1].Rsi
            : null;

        // Calculate gain and percent rank
        double gain = double.IsNaN(value) || double.IsNaN(_prevValue) || _prevValue <= 0
            ? double.NaN
            : (value - _prevValue) / _prevValue;

        _gainBuffer.Update(RankPeriods + 1, gain);

        double? percentRank = null;
        if (_processedCount >= RankPeriods && !double.IsNaN(gain))
        {
            int qty = 0;
            bool isViableRank = true;

            foreach (double g in _gainBuffer)
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

        // Calculate Connors RSI
        int startPeriod = Math.Max(RsiPeriods, Math.Max(StreakPeriods, RankPeriods)) + 2;
        double? connorsRsi = null;

        if (_processedCount >= startPeriod - 1 && rsi.HasValue && rsiStreak.HasValue && percentRank.HasValue)
        {
            connorsRsi = (rsi.Value + rsiStreak.Value + percentRank.Value) / 3;
        }

        AddInternal(new ConnorsRsiResult(
            Timestamp: timestamp,
            Streak: _streak,
            Rsi: rsi,
            RsiStreak: rsiStreak,
            PercentRank: percentRank,
            ConnorsRsi: connorsRsi));

        _prevValue = value;
        _processedCount++;
    }

    /// <inheritdoc />
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        _rsiClose.Clear();
        _rsiStreak.Clear();
        _gainBuffer.Clear();
        _prevValue = double.NaN;
        _streak = 0;
        _processedCount = 0;
    }
}

public static partial class ConnorsRsi
{
    /// <summary>
    /// Creates a buffer list for Connors RSI calculations.
    /// </summary>
    /// <typeparam name="T">The type of the reusable value.</typeparam>
    /// <param name="source">The source list of reusable values.</param>
    /// <param name="rsiPeriods">The number of periods for the RSI calculation on close prices.</param>
    /// <param name="streakPeriods">The number of periods for the RSI calculation on streak.</param>
    /// <param name="rankPeriods">The number of periods for the percent rank calculation.</param>
    /// <returns>A new <see cref="ConnorsRsiList"/> instance.</returns>
    public static ConnorsRsiList ToConnorsRsiList<T>(
        this IReadOnlyList<T> source,
        int rsiPeriods = 3,
        int streakPeriods = 2,
        int rankPeriods = 100)
        where T : IReusable
        => new(rsiPeriods, streakPeriods, rankPeriods)
        {
            (IReadOnlyList<IReusable>)source
        };
}
