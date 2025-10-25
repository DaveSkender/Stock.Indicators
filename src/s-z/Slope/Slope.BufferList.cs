namespace Skender.Stock.Indicators;

/// <summary>
/// Slope and Linear Regression from incremental reusable values.
/// </summary>
public class SlopeList : BufferList<SlopeResult>, IIncrementFromChain
{
    private readonly Queue<double> buffer;
    private readonly int lookbackPeriods;
    /// <summary>
    /// Tracks how many items have been removed from the beginning
    /// </summary>
    private int globalIndexOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="SlopeList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public SlopeList(int lookbackPeriods)
    {
        Slope.Validate(lookbackPeriods);
        this.lookbackPeriods = lookbackPeriods;
        LookbackPeriods = lookbackPeriods;
        buffer = new Queue<double>(lookbackPeriods);
        globalIndexOffset = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SlopeList"/> class with initial reusable values.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public SlopeList(int lookbackPeriods, IReadOnlyList<IReusable> values)
        : this(lookbackPeriods)
    {
        Add(values);
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Update the rolling buffer
        buffer.Update(lookbackPeriods, value);

        // During initialization period
        if (buffer.Count < lookbackPeriods)
        {
            AddInternal(new SlopeResult(timestamp));
            return;
        }

        // The current global index (0-based, but we use 1-based for X values)
        int currentIndex = globalIndexOffset + Count;

        // Calculate slope and related metrics using indices from the window
        double sumX = 0;
        double sumY = 0;
        int relativeIndex = 0;

        // Get averages for period
        // X values are global indices + 1: (currentIndex - lookbackPeriods + 2) to (currentIndex + 1)
        foreach (double bufferValue in buffer)
        {
            int globalIndex = currentIndex - lookbackPeriods + 1 + relativeIndex;
            sumX += globalIndex + 1d;
            sumY += bufferValue;
            relativeIndex++;
        }

        double avgX = sumX / lookbackPeriods;
        double avgY = sumY / lookbackPeriods;

        // Least squares method
        double sumSqX = 0;
        double sumSqY = 0;
        double sumSqXy = 0;
        relativeIndex = 0;

        foreach (double bufferValue in buffer)
        {
            int globalIndex = currentIndex - lookbackPeriods + 1 + relativeIndex;
            double devX = (globalIndex + 1d) - avgX;
            double devY = bufferValue - avgY;

            sumSqX += devX * devX;
            sumSqY += devY * devY;
            sumSqXy += devX * devY;
            relativeIndex++;
        }

        double? slope = (sumSqXy / sumSqX).NaN2Null();
        double? intercept = (avgY - (slope * avgX)).NaN2Null();

        // Calculate Standard Deviation and R-Squared
        double stdDevX = Math.Sqrt(sumSqX / lookbackPeriods);
        double stdDevY = Math.Sqrt(sumSqY / lookbackPeriods);

        double? rSquared = null;

        if (stdDevX * stdDevY != 0)
        {
            double arrr = sumSqXy / (stdDevX * stdDevY) / lookbackPeriods;
            rSquared = (arrr * arrr).NaN2Null();
        }

        // Write result (Line will be calculated after we have the current slope)
        SlopeResult result = new(
            Timestamp: timestamp,
            Slope: slope,
            Intercept: intercept,
            StdDev: stdDevY.NaN2Null(),
            RSquared: rSquared,
            Line: null);

        AddInternal(result);

        // Update Line values for the last lookbackPeriods results
        // This is legitimate historical repaint matching Series behavior
        UpdateLineValues();
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
        buffer.Clear();
        globalIndexOffset = 0;
    }

    /// <summary>
    /// Overrides list pruning to update the global index offset.
    /// </summary>
    protected override void PruneList()
    {
        int countBeforePrune = Count;
        base.PruneList();
        int countAfterPrune = Count;

        // Update offset by the number of items removed
        globalIndexOffset += countBeforePrune - countAfterPrune;
    }

    /// <summary>
    /// Updates Line values for the last lookbackPeriods results using the most recent slope/intercept.
    /// This is legitimate historical repaint behavior matching the Series implementation.
    /// </summary>
    private void UpdateLineValues()
    {
        // Only update if we have enough results
        if (Count < lookbackPeriods)
        {
            return;
        }

        // Get the most recent result (just added) which has the current slope/intercept
        SlopeResult lastResult = this[Count - 1];

        // First, nullify Line for all results that are NOT in the last lookbackPeriods
        for (int p = 0; p < Count - lookbackPeriods; p++)
        {
            SlopeResult existing = this[p];
            if (existing.Line is not null)
            {
                SlopeResult updated = existing with { Line = null };
                UpdateInternal(p, updated);
            }
        }

        // Then, update Line for the last lookbackPeriods results
        // Using global indices (globalIndex + 1) like the series implementation
        int startIndex = Count - lookbackPeriods;
        for (int p = startIndex; p < Count; p++)
        {
            SlopeResult existing = this[p];

            // Calculate Line: y = mx + b, using global index (offset + p + 1)
            int globalIndex = globalIndexOffset + p;
            decimal? line = (decimal?)((lastResult.Slope * (globalIndex + 1)) + lastResult.Intercept).NaN2Null();

            SlopeResult updated = existing with { Line = line };
            UpdateInternal(p, updated);
        }
    }
}

public static partial class Slope
{
    /// <summary>
    /// Creates a buffer list for Slope calculations.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="lookbackPeriods"></param>
    public static SlopeList ToSlopeList(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods = 14)
        => new(lookbackPeriods) { source };
}
