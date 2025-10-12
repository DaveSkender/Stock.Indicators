namespace Skender.Stock.Indicators;

/// <summary>
/// Standard Deviation from incremental reusable values.
/// </summary>
public class StdDevList : BufferList<StdDevResult>, IIncrementFromChain
{
    private readonly Queue<double> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="StdDevList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public StdDevList(int lookbackPeriods = 14)
    {
        StdDev.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<double>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StdDevList"/> class with initial reusable values.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public StdDevList(int lookbackPeriods, IReadOnlyList<IReusable> values)
        : this(lookbackPeriods)
        => Add(values);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Update buffer
        _buffer.Update(LookbackPeriods, value);

        double mean;
        double stdDev;
        double zScore;

        if (_buffer.Count == LookbackPeriods)
        {
            double[] values = new double[LookbackPeriods];
            double sum = 0;
            int n = 0;

            foreach (double val in _buffer)
            {
                values[n] = val;
                sum += val;
                n++;
            }

            mean = sum / LookbackPeriods;

            stdDev = values.StdDev();

            zScore = stdDev == 0 ? double.NaN
                : (value - mean) / stdDev;
        }
        else
        {
            mean = double.NaN;
            stdDev = double.NaN;
            zScore = double.NaN;
        }

        StdDevResult result = new(
            Timestamp: timestamp,
            StdDev: stdDev.NaN2Null(),
            Mean: mean.NaN2Null(),
            ZScore: zScore.NaN2Null());

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
        _buffer.Clear();
    }
}

public static partial class StdDev
{
    /// <summary>
    /// Creates a buffer list for Standard Deviation calculations.
    /// </summary>
    public static StdDevList ToStdDevList<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods = 14)
        where T : IReusable
        => new(lookbackPeriods) { (IReadOnlyList<IReusable>)source };
}
