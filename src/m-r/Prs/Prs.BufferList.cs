namespace Skender.Stock.Indicators;

/// <summary>
/// Price Relative Strength (PRS) from incremental paired reusable values.
/// Note: This indicator requires two synchronized series (Eval and Base).
/// </summary>
public class PrsList : BufferList<PrsResult>, IIncrementFromPairs, IPrs
{
    private readonly Queue<(double ValueEval, double ValueBase)> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrsList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods for the PRS% lookback calculation. Use int.MinValue for no lookback calculation.</param>
    public PrsList(int lookbackPeriods = int.MinValue)
    {
        // Validate lookback periods
        if (lookbackPeriods is <= 0 and not int.MinValue)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Price Relative Strength.");
        }

        LookbackPeriods = lookbackPeriods;

        // Only need buffer if lookbackPeriods is positive
        int bufferSize = lookbackPeriods > 0 ? lookbackPeriods + 1 : 1;
        _buffer = new Queue<(double, double)>(bufferSize);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PrsList"/> class with initial series.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods for the PRS% lookback calculation.</param>
    /// <param name="sourceEval">First series (evaluation) to populate the list.</param>
    /// <param name="sourceBase">Second series (base) to populate the list.</param>
    public PrsList(
        int lookbackPeriods,
        IReadOnlyList<IReusable> sourceEval,
        IReadOnlyList<IReusable> sourceBase)
        : this(lookbackPeriods)
    {
        Add(sourceEval, sourceBase);
    }

    /// <summary>
    /// Gets the number of periods for the PRS% lookback calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Adds a pair of values from two synchronized series.
    /// </summary>
    /// <param name="timestamp">The timestamp for this pair of values.</param>
    /// <param name="valueA">The value from evaluation series.</param>
    /// <param name="valueB">The value from base series.</param>
    public void Add(DateTime timestamp, double valueA, double valueB)
    {
        // Update buffer with current values
        _buffer.Update(LookbackPeriods > 0 ? LookbackPeriods + 1 : 1, (valueA, valueB));

        // Calculate PRS
        double? prs = valueB == 0
            ? null
            : (valueA / valueB).NaN2Null();

        // Calculate PRS%
        double? prsPercent = null;

        if (LookbackPeriods > 0 && _buffer.Count > LookbackPeriods)
        {
            (double evalOld, double baseOld) = _buffer.Peek();

            if (baseOld != 0 && evalOld != 0)
            {
                double? pctBase = (valueB - baseOld) / baseOld;
                double? pctEval = (valueA - evalOld) / evalOld;

                prsPercent = (pctEval - pctBase).NaN2Null();
            }
        }

        PrsResult result = new(
            Timestamp: timestamp,
            Prs: prs,
            PrsPercent: prsPercent);

        AddInternal(result);
    }

    /// <summary>
    /// Adds a pair of reusable values from two synchronized series.
    /// </summary>
    /// <param name="valueA">The item from evaluation series.</param>
    /// <param name="valueB">The item from base series.</param>
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
                "Price Relative Strength requires matching dates in provided histories.");
        }

        Add(valueA.Timestamp, valueA.Value, valueB.Value);
    }

    /// <summary>
    /// Adds two synchronized lists of reusable values.
    /// </summary>
    /// <param name="valuesA">The evaluation series.</param>
    /// <param name="valuesB">The base series.</param>
    /// <exception cref="ArgumentNullException">Thrown when either series is null.</exception>
    /// <exception cref="ArgumentException">Thrown when series have different lengths.</exception>
    public void Add(IReadOnlyList<IReusable> valuesA, IReadOnlyList<IReusable> valuesB)
    {
        ArgumentNullException.ThrowIfNull(valuesA);
        ArgumentNullException.ThrowIfNull(valuesB);

        if (valuesA.Count != valuesB.Count)
        {
            throw new ArgumentException(
                "Eval series and Base series must have the same number of items.",
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

public static partial class Prs
{
    /// <summary>
    /// Creates a buffer list for PRS calculations from two synchronized series.
    /// </summary>
    /// <param name="sourceEval">Eval quote series</param>
    /// <param name="sourceBase">Base quote series for comparison</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    public static PrsList ToPrsList(
        this IReadOnlyList<IReusable> sourceEval,
        IReadOnlyList<IReusable> sourceBase,
        int lookbackPeriods = int.MinValue)
        => new(lookbackPeriods, sourceEval, sourceBase);
}
