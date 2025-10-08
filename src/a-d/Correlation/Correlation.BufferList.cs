namespace Skender.Stock.Indicators;

/// <summary>
/// Correlation from incremental paired reusable values.
/// Note: This indicator requires two synchronized series (A and B).
/// </summary>
public class CorrelationList : BufferList<CorrResult>, ICorrelation
{
    private readonly Queue<double> valueABuffer;
    private readonly Queue<double> valueBBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public CorrelationList(int lookbackPeriods)
    {
        Correlation.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        valueABuffer = new Queue<double>(lookbackPeriods);
        valueBBuffer = new Queue<double>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationList"/> class with initial series.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="seriesA">First series to populate the list.</param>
    /// <param name="seriesB">Second series to populate the list.</param>
    public CorrelationList(
        int lookbackPeriods,
        IReadOnlyList<IReusable> seriesA,
        IReadOnlyList<IReusable> seriesB)
        : this(lookbackPeriods)
        => Add(seriesA, seriesB);

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
        // Update buffers
        valueABuffer.Update(LookbackPeriods, valueA);
        valueBBuffer.Update(LookbackPeriods, valueB);

        // Calculate correlation when we have enough data
        CorrResult result;

        if (valueABuffer.Count == LookbackPeriods)
        {
            double[] dataA = [.. valueABuffer];
            double[] dataB = [.. valueBBuffer];

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
    /// <param name="itemA">The item from series A.</param>
    /// <param name="itemB">The item from series B.</param>
    /// <exception cref="ArgumentNullException">Thrown when either item is null.</exception>
    /// <exception cref="InvalidQuotesException">Thrown when timestamps don't match.</exception>
    public void Add(IReusable itemA, IReusable itemB)
    {
        ArgumentNullException.ThrowIfNull(itemA);
        ArgumentNullException.ThrowIfNull(itemB);

        if (itemA.Timestamp != itemB.Timestamp)
        {
            throw new InvalidQuotesException(
                nameof(itemA), itemA.Timestamp,
                "Timestamp sequence does not match. " +
                "Correlation requires matching dates in provided histories.");
        }

        Add(itemA.Timestamp, itemA.Value, itemB.Value);
    }

    /// <summary>
    /// Adds two synchronized lists of reusable values.
    /// </summary>
    /// <param name="seriesA">The first series.</param>
    /// <param name="seriesB">The second series.</param>
    /// <exception cref="ArgumentNullException">Thrown when either series is null.</exception>
    /// <exception cref="ArgumentException">Thrown when series have different lengths.</exception>
    public void Add(IReadOnlyList<IReusable> seriesA, IReadOnlyList<IReusable> seriesB)
    {
        ArgumentNullException.ThrowIfNull(seriesA);
        ArgumentNullException.ThrowIfNull(seriesB);

        if (seriesA.Count != seriesB.Count)
        {
            throw new ArgumentException(
                "Series A and Series B must have the same number of items.",
                nameof(seriesB));
        }

        for (int i = 0; i < seriesA.Count; i++)
        {
            Add(seriesA[i], seriesB[i]);
        }
    }

    /// <summary>
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public override void Clear()
    {
        ClearInternal();
        valueABuffer.Clear();
        valueBBuffer.Clear();
    }
}

public static partial class Correlation
{
    /// <summary>
    /// Creates a buffer list for Correlation calculations from two synchronized series.
    /// </summary>
    public static CorrelationList ToCorrelationList<T>(
        this IReadOnlyList<T> seriesA,
        IReadOnlyList<T> seriesB,
        int lookbackPeriods)
        where T : IReusable
    {
        // Cast to IReadOnlyList<IReusable> for the constructor
        IReadOnlyList<IReusable> castSeriesA = seriesA.Cast<IReusable>().ToList();
        IReadOnlyList<IReusable> castSeriesB = seriesB.Cast<IReusable>().ToList();

        return new(lookbackPeriods, castSeriesA, castSeriesB);
    }

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
