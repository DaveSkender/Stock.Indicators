namespace Skender.Stock.Indicators;

/// <summary>
/// Rate of Change with Bands (RocWb) from incremental reusable values.
/// </summary>
public class RocWbList : BufferList<RocWbResult>, IIncrementFromChain, IRocWb
{
    private readonly Queue<double> rocBuffer;
    private readonly Queue<double> rocSqBuffer;
    private readonly Queue<double> rocEmaInitBuffer;
    private double prevEma = double.NaN;
    private readonly double k;

    /// <summary>
    /// Initializes a new instance of the <see cref="RocWbList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the ROC calculation.</param>
    /// <param name="emaPeriods">The number of periods for the exponential moving average calculation.</param>
    /// <param name="stdDevPeriods">The number of periods for the standard deviation calculation.</param>
    public RocWbList(int lookbackPeriods, int emaPeriods, int stdDevPeriods)
    {
        RocWb.Validate(lookbackPeriods, emaPeriods, stdDevPeriods);
        LookbackPeriods = lookbackPeriods;
        EmaPeriods = emaPeriods;
        StdDevPeriods = stdDevPeriods;
        rocBuffer = new Queue<double>(lookbackPeriods + 1);
        rocSqBuffer = new Queue<double>(stdDevPeriods);
        rocEmaInitBuffer = new Queue<double>(emaPeriods);
        k = 2d / (emaPeriods + 1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RocWbList"/> class with initial reusable values.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the ROC calculation.</param>
    /// <param name="emaPeriods">The number of periods for the exponential moving average calculation.</param>
    /// <param name="stdDevPeriods">The number of periods for the standard deviation calculation.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public RocWbList(int lookbackPeriods, int emaPeriods, int stdDevPeriods, IReadOnlyList<IReusable> values)
        : this(lookbackPeriods, emaPeriods, stdDevPeriods)
    {
        Add(values);
    }

    /// <summary>
    /// Gets the number of periods to look back for the ROC calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the exponential moving average calculation.
    /// </summary>
    public int EmaPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the standard deviation calculation.
    /// </summary>
    public int StdDevPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Update the ROC buffer
        rocBuffer.Update(LookbackPeriods + 1, value);

        // Calculate ROC
        double roc;
        if (rocBuffer.Count > LookbackPeriods)
        {
            double backValue = rocBuffer.Peek();
            roc = backValue == 0 ? double.NaN : 100d * (value - backValue) / backValue;
        }
        else
        {
            roc = double.NaN;
        }

        // Update the ROC buffer for squared values (RMS calculation)
        double rocSq = roc * roc;
        if (!double.IsNaN(roc))
        {
            rocSqBuffer.Update(StdDevPeriods, rocSq);
        }

        // Calculate EMA of ROC
        double rocEma;
        if (double.IsNaN(prevEma))
        {
            // Accumulate ROC values for initial EMA calculation
            if (!double.IsNaN(roc))
            {
                rocEmaInitBuffer.Update(EmaPeriods, roc);
            }

            // Initialize EMA with SMA when we have enough values
            if (rocEmaInitBuffer.Count >= EmaPeriods)
            {
                double sum = 0;
                foreach (double rocVal in rocEmaInitBuffer)
                {
                    sum += rocVal;
                }

                rocEma = sum / EmaPeriods;
            }
            else
            {
                rocEma = double.NaN;
            }
        }
        else
        {
            // Normal EMA calculation
            rocEma = Ema.Increment(k, prevEma, roc);
        }

        prevEma = rocEma;

        // Calculate RMS deviation bands
        double? rocDev = null;
        if (rocSqBuffer.Count >= StdDevPeriods && !double.IsNaN(roc))
        {
            double sum = 0;
            foreach (double sq in rocSqBuffer)
            {
                sum += sq;
            }

            rocDev = Math.Sqrt(sum / StdDevPeriods).NaN2Null();
        }

        AddInternal(new RocWbResult(
            Timestamp: timestamp,
            Roc: roc.NaN2Null(),
            RocEma: rocEma.NaN2Null(),
            UpperBand: rocDev,
            LowerBand: rocDev.HasValue ? -rocDev.Value : null));
    }

    /// <summary>
    /// Adds a new reusable value to the RocWb list.
    /// </summary>
    /// <param name="value">The reusable value to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <summary>
    /// Adds a list of reusable values to the RocWb list.
    /// </summary>
    /// <param name="values">The list of reusable values to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the values list is null.</exception>
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <summary>
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        rocBuffer.Clear();
        rocSqBuffer.Clear();
        rocEmaInitBuffer.Clear();
        prevEma = double.NaN;
    }
}

public static partial class RocWb
{
    /// <summary>
    /// Creates a buffer list for Rate of Change with Bands (RocWb) calculations.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="lookbackPeriods"></param>
    /// <param name="emaPeriods"></param>
    /// <param name="stdDevPeriods"></param>
    public static RocWbList ToRocWbList(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods = 20,
        int emaPeriods = 5,
        int stdDevPeriods = 5)
        => new(lookbackPeriods, emaPeriods, stdDevPeriods) { source };
}
