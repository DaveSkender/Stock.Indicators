namespace Skender.Stock.Indicators;

/// <summary>
/// Schaff Trend Cycle from incremental reusable values.
/// </summary>
public class StcList : BufferList<StcResult>, IIncrementFromChain
{
    private readonly MacdList _macdList;
    private readonly StochList _stochList;

    /// <summary>
    /// Initializes a new instance of the <see cref="StcList"/> class.
    /// </summary>
    /// <param name="cyclePeriods">The number of periods for the cycle calculation.</param>
    /// <param name="fastPeriods">The number of periods for the fast MA.</param>
    /// <param name="slowPeriods">The number of periods for the slow MA.</param>
    public StcList(
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
    {
        Stc.Validate(cyclePeriods, fastPeriods, slowPeriods);

        CyclePeriods = cyclePeriods;
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;

        _macdList = new MacdList(fastPeriods, slowPeriods, 1);
        _stochList = new StochList(cyclePeriods, 3, 3, 3, 2, MaType.SMA);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StcList"/> class with initial reusable values.
    /// </summary>
    /// <param name="cyclePeriods">The number of periods for the cycle calculation.</param>
    /// <param name="fastPeriods">The number of periods for the fast MA.</param>
    /// <param name="slowPeriods">The number of periods for the slow MA.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public StcList(
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods,
        IReadOnlyList<IReusable> values)
        : this(cyclePeriods, fastPeriods, slowPeriods)
    {
        Add(values);
    }

    /// <summary>
    /// Gets the number of periods for the cycle calculation.
    /// </summary>
    public int CyclePeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the fast MA.
    /// </summary>
    public int FastPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the slow MA.
    /// </summary>
    public int SlowPeriods { get; init; }




    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Add to MACD list
        _macdList.Add(timestamp, value);

        // Get the latest MACD result
        MacdResult macdResult = _macdList[^1];

        // If MACD has a value, feed it to the Stochastic calculation
        // MACD value is used as high, low, and close for stochastic calculation
        if (macdResult.Macd.HasValue)
        {
            double macdValue = macdResult.Macd.Value;
            _stochList.Add(timestamp, macdValue, macdValue, macdValue);
        }

        // Create STC result from the Stochastic result
        StcResult result;
        if (_stochList.Count > 0)
        {
            StochResult stochResult = _stochList[^1];
            result = new StcResult(
                Timestamp: timestamp,
                Stc: stochResult.Oscillator);
        }
        else
        {
            result = new StcResult(timestamp, null);
        }

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
        _macdList.Clear();
        _stochList.Clear();
    }
}

public static partial class Stc
{
    /// <summary>
    /// Creates a buffer list for Schaff Trend Cycle calculations.
    /// </summary>
    /// <param name="source">The source list of reusable values.</param>
    /// <param name="cyclePeriods">The number of periods for the cycle calculation.</param>
    /// <param name="fastPeriods">The number of periods for the fast MA.</param>
    /// <param name="slowPeriods">The number of periods for the slow MA.</param>
    /// <returns>A buffer list for Schaff Trend Cycle calculations.</returns>
    public static StcList ToStcList(
        this IReadOnlyList<IReusable> source,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        => new(cyclePeriods, fastPeriods, slowPeriods) { source };
}
