namespace Skender.Stock.Indicators;

/// <summary>
/// Correlation from incremental paired reusable values.
/// Note: This indicator requires two synchronized series (A and B).
/// </summary>
public class CorrelationList : BufferList<CorrResult>, IIncrementFromPairs, ICorrelation
{
    private readonly Queue<(double ValueA, double ValueB)> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public CorrelationList(int lookbackPeriods)
    {
        Correlation.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<(double, double)>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationList"/> class with initial series.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="valuesA">First series to populate the list.</param>
    /// <param name="valuesB">Second series to populate the list.</param>
    public CorrelationList(
        int lookbackPeriods,
        IReadOnlyList<IReusable> valuesA,
        IReadOnlyList<IReusable> valuesB)
        : this(lookbackPeriods)
        => Add(valuesA, valuesB);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Adds a pair of values from two synchronized series.
    /// </summary>
    /// <param name="timestamp">The timestamp for this pair of values.</param>
    /// <param name="valueA">The value from series A.</param>
    /// <param name="valueB">The value from series B.</param>
    public void Add(DateTime timestamp, double valueA, double valueB)
    {
        // Update buffer with consolidated tuple
        _buffer.Update(LookbackPeriods, (valueA, valueB));

        // Calculate correlation when we have enough data
        CorrResult result;

        if (_buffer.Count == LookbackPeriods)
        {
            (double ValueA, double ValueB)[] bufferArray = _buffer.ToArray();
            double[] dataA = new double[bufferArray.Length];
            double[] dataB = new double[bufferArray.Length];

            for (int i = 0; i < bufferArray.Length; i++)
            {
                dataA[i] = bufferArray[i].ValueA;
                dataB[i] = bufferArray[i].ValueB;
            }

            result = Correlation.PeriodCorrelation(timestamp, dataA, dataB);
        }
        else
        {
            result = new(Timestamp: timestamp);
        }

        AddInternal(result);
    }

    /// <summary>
    /// Adds a pair of reusable values from two synchronized series.
    /// </summary>
    /// <param name="valueA">The item from series A.</param>
    /// <param name="valueB">The item from series B.</param>
    /// <exception cref="ArgumentNullException">Thrown when either item is null.</exception>
    /// <exception cref="InvalidQuotesException">Thrown when timestamps don't match.</exception>
    public void Add(IReusable valueA, IReusable valueB)
    {
        ArgumentNullException.ThrowIfNull(valueA);
        ArgumentNullException.ThrowIfNull(valueB);

        if (valueA.Timestamp != valueB.Timestamp)
        {
            throw new InvalidQuotesException(
                nameof(valueA), valueA.Timestamp,
                "Timestamp sequence does not match. " +
                "Correlation requires matching dates in provided histories.");
        }

        Add(valueA.Timestamp, valueA.Value, valueB.Value);
    }

    /// <summary>
    /// Adds two synchronized lists of reusable values.
    /// </summary>
    /// <param name="valuesA">The first series.</param>
    /// <param name="valuesB">The second series.</param>
    /// <exception cref="ArgumentNullException">Thrown when either series is null.</exception>
    /// <exception cref="ArgumentException">Thrown when series have different lengths.</exception>
    public void Add(IReadOnlyList<IReusable> valuesA, IReadOnlyList<IReusable> valuesB)
    {
        ArgumentNullException.ThrowIfNull(valuesA);
        ArgumentNullException.ThrowIfNull(valuesB);

        if (valuesA.Count != valuesB.Count)
        {
            throw new ArgumentException(
                "Series A and Series B must have the same number of items.",
                nameof(valuesB));
        }

        for (int i = 0; i < valuesA.Count; i++)
        {
            Add(valuesA[i], valuesB[i]);
        }
    }

    /// <summary>
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        _buffer.Clear();
    }
}

public static partial class Correlation
{
    /// <summary>
    /// Creates a buffer list for Correlation calculations from two synchronized series.
    /// </summary>
    public static CorrelationList ToCorrelationList(
        this IReadOnlyList<IReusable> valuesA,
        IReadOnlyList<IReusable> valuesB,
        int lookbackPeriods)
        => new(lookbackPeriods, valuesA, valuesB);

    /// <summary>
    /// Validates the parameters for correlation calculations.
    /// </summary>
    internal static void Validate(int lookbackPeriods)
    {
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Correlation.");
        }
    }
}
