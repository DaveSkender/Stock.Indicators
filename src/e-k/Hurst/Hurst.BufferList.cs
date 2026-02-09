namespace Skender.Stock.Indicators;

/// <summary>
/// Hurst Exponent from incremental reusable values.
/// </summary>
public class HurstList : BufferList<HurstResult>, IIncrementFromChain, IHurst
{
    private readonly Queue<double> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="HurstList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public HurstList(int lookbackPeriods)
    {
        Hurst.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _buffer = new Queue<double>(lookbackPeriods + 1);

        Name = $"HURST({lookbackPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HurstList"/> class with initial reusable values.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public HurstList(int lookbackPeriods, IReadOnlyList<IReusable> values)
        : this(lookbackPeriods) => Add(values);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        _buffer.Update(LookbackPeriods + 1, value);

        double? h = null;

        // need enough periods to calculate Hurst (lookbackPeriods + 1 values to get lookbackPeriods returns)
        if (_buffer.Count == LookbackPeriods + 1)
        {
            // get evaluation batch - calculate returns from buffer values
            double[] values = new double[LookbackPeriods];
            double[] bufferArray = _buffer.ToArray();

            int x = 0;
            double l = bufferArray[0];

            // skip first value (used as initial l) and calculate returns for the rest
            for (int p = 1; p < bufferArray.Length; p++)
            {
                double ps = bufferArray[p];

                // return values
                values[x] = l != 0 ? (ps / l) - 1 : double.NaN;

                l = ps;
                x++;
            }

            // calculate hurst exponent
            h = Hurst.CalcHurstWindow(values).NaN2Null();
        }

        AddInternal(new HurstResult(timestamp, h));
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

public static partial class Hurst
{
    /// <summary>
    /// Creates a buffer list for Hurst Exponent calculations.
    /// </summary>
    /// <param name="source">Historical reusable values.</param>
    /// <param name="lookbackPeriods">Number of periods for Hurst calculation.</param>
    /// <returns>An initialized <see cref="HurstList" />.</returns>
    public static HurstList ToHurstList(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods = 100)
        => new(lookbackPeriods) { source };
}
