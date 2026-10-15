namespace FacioQuo.Stock.Indicators;

/// <summary>
/// Stochastic RSI from incremental reusable values.
/// </summary>
public class StochRsiList : BufferList<StochRsiResult>, IIncrementFromChain
{
    private readonly RsiList _rsiList;
    private readonly StochList _stochList;

    // rows seen since construction or Clear; Count shrinks when the list prunes
    private int _rowsSeen;

    /// <summary>
    /// Initializes a new instance of the <see cref="StochRsiList"/> class.
    /// </summary>
    /// <param name="rsiPeriods">Number of periods for the RSI calculation.</param>
    /// <param name="stochPeriods">Number of periods for the Stochastic calculation.</param>
    /// <param name="signalPeriods">Number of periods for the signal line.</param>
    /// <param name="smoothPeriods">Number of periods for smoothing.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="smoothPeriods"/> is invalid.</exception>
    public StochRsiList(
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods = 1)
    {
        StochRsi.Validate(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods);

        RsiPeriods = rsiPeriods;
        StochPeriods = stochPeriods;
        SignalPeriods = signalPeriods;
        SmoothPeriods = smoothPeriods;

        _rsiList = new RsiList(rsiPeriods);
        _stochList = new StochList(stochPeriods, signalPeriods, smoothPeriods, 3, 2, MaType.SMA);

        Name = FormattableString.Invariant($"STOCHRSI({rsiPeriods}, {stochPeriods}, {signalPeriods}, {smoothPeriods})");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StochRsiList"/> class with initial reusable values.
    /// </summary>
    /// <param name="rsiPeriods">Number of periods for the RSI calculation.</param>
    /// <param name="stochPeriods">Number of periods for the Stochastic calculation.</param>
    /// <param name="signalPeriods">Number of periods for the signal line.</param>
    /// <param name="smoothPeriods">Number of periods for smoothing.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public StochRsiList(
        int rsiPeriods,
        int stochPeriods,
        int signalPeriods,
        int smoothPeriods,
        IReadOnlyList<IReusable> values)
        : this(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods) => Add(values);

    /// <inheritdoc />
    public int RsiPeriods { get; init; }

    /// <inheritdoc />
    public int StochPeriods { get; init; }

    /// <inheritdoc />
    public int SignalPeriods { get; init; }

    /// <inheritdoc />
    public int SmoothPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Add to RSI list
        _rsiList.Add(timestamp, value);

        // Series runs Stoch over every row, warmup NaN included, so its windows line up by index
        double rsi = _rsiList[^1].Rsi.Null2NaN();
        _stochList.Add(timestamp, rsi, rsi, rsi);

        StochResult stoch = _stochList[^1];

        // Series leaves the first rsiPeriods + stochPeriods - 1 rows empty
        StochRsiResult result = _rowsSeen++ >= RsiPeriods + StochPeriods - 1
            ? new(timestamp, stoch.Oscillator, stoch.Signal)
            : new(timestamp);

        AddInternal(result);
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
        _rsiList.Clear();
        _stochList.Clear();
        _rowsSeen = 0;
    }

    /// <inheritdoc />
    protected override void PruneList()
    {
        // inner lists only need the newest rows; bound them with the outer list
        _rsiList.MaxListSize = Math.Max(RsiPeriods + 1, MaxListSize + 1);
        _stochList.MaxListSize = Math.Max(Math.Max(StochPeriods, Math.Max(SmoothPeriods, SignalPeriods)) + 1, MaxListSize + 1);

        base.PruneList();
    }
}

public static partial class StochRsi
{
    /// <summary>
    /// Creates a buffer list for Stochastic RSI calculations.
    /// </summary>
    /// <param name="source">Source list of reusable values.</param>
    /// <param name="rsiPeriods">Number of periods for RSI calculation.</param>
    /// <param name="stochPeriods">Number of periods for Stochastic calculation.</param>
    /// <param name="signalPeriods">Number of periods for the signal line.</param>
    /// <param name="smoothPeriods">Number of periods for smoothing.</param>
    /// <returns>A buffer list for Stochastic RSI calculations.</returns>
    public static StochRsiList ToStochRsiList(
        this IReadOnlyList<IReusable> source,
        int rsiPeriods = 14,
        int stochPeriods = 14,
        int signalPeriods = 3,
        int smoothPeriods = 1)
        => new(rsiPeriods, stochPeriods, signalPeriods, smoothPeriods) { source };
}
