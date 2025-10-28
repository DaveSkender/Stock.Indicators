namespace Skender.Stock.Indicators;

/// <summary>
/// Standard Deviation Channels from incremental reusable values.
/// </summary>
/// <remarks>
/// <para>
/// <b>Exception to BufferUtilities Pattern:</b> StdDevChannels does not use the standard
/// <see cref="BufferListUtilities"/> extension methods (Update/UpdateWithDequeue) due to
/// its dependency on internal <see cref="SlopeList"/> calculations.
/// </para>
/// <para>
/// <b>Algorithmic Requirements:</b>
/// <list type="bullet">
/// <item>Requires complete <see cref="SlopeList"/> history for slope and regression calculations</item>
/// <item>Processes data in reverse windows (most recent backward by lookbackPeriods steps)</item>
/// <item>Calculates regression channels using slope/intercept from specific window endpoints</item>
/// <item>Maintains global index offset to ensure correct calculations after list pruning</item>
/// </list>
/// </para>
/// <para>
/// <b>Why Queue&lt;T&gt; Cannot Be Used:</b> The standard <see cref="Queue{T}"/> data structure
/// used by BufferUtilities cannot maintain the complete SlopeList needed for the reverse window
/// iteration pattern that characterizes Standard Deviation Channels.
/// </para>
/// <para>
/// <b>Memory Management:</b> This implementation uses an internal <see cref="SlopeList"/> that
/// is synchronized with the main result list during pruning operations.
/// </para>
/// </remarks>
public class StdDevChannelsList : BufferList<StdDevChannelsResult>, IIncrementFromChain
{
    private readonly SlopeList slopeList;
    private readonly int lookbackPeriods;
    private readonly double stdDeviations;
    /// <summary>
    /// Tracks how many items have been removed from the beginning
    /// </summary>
    private int globalIndexOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="StdDevChannelsList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="stdDeviations">The number of standard deviations for the channel width.</param>
    public StdDevChannelsList(int lookbackPeriods = 20, double stdDeviations = 2)
    {
        StdDevChannels.Validate(lookbackPeriods, stdDeviations);
        this.lookbackPeriods = lookbackPeriods;
        this.stdDeviations = stdDeviations;
        LookbackPeriods = lookbackPeriods;
        StdDeviations = stdDeviations;
        slopeList = new SlopeList(lookbackPeriods);
        globalIndexOffset = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StdDevChannelsList"/> class with initial reusable values.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="stdDeviations">The number of standard deviations for the channel width.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public StdDevChannelsList(int lookbackPeriods, double stdDeviations, IReadOnlyList<IReusable> values)
        : this(lookbackPeriods, stdDeviations)
    {
        Add(values);
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the number of standard deviations for the channel width.
    /// </summary>
    public double StdDeviations { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Add to slope list
        slopeList.Add(timestamp, value);

        // Add initial empty result
        AddInternal(new StdDevChannelsResult(timestamp));

        // Update channel values for the current window if we have enough data
        UpdateChannelValues();
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
        slopeList.Clear();
        globalIndexOffset = 0;
    }

    /// <summary>
    /// Overrides list pruning to also prune the internal SlopeList and update the global index offset.
    /// </summary>
    protected override void PruneList()
    {
        int countBeforePrune = Count;

        // Sync MaxListSize before pruning so both lists prune to the same size
        slopeList.MaxListSize = MaxListSize;

        // Prune the base list
        base.PruneList();

        int countAfterPrune = Count;

        // Update offset by the number of items removed
        globalIndexOffset += countBeforePrune - countAfterPrune;
    }

    /// <summary>
    /// Updates channel values following the reverse iteration pattern.
    /// This matches the Series implementation behavior where channels are calculated
    /// starting from the most recent data point and working backwards by lookbackPeriods.
    /// </summary>
    private void UpdateChannelValues()
    {
        int currentCount = Count;

        // Only update when we have at least lookbackPeriods items
        if (currentCount < lookbackPeriods)
        {
            return;
        }

        // First, clear all channel values (we'll recalculate only the valid windows)
        for (int p = 0; p < currentCount; p++)
        {
            StdDevChannelsResult existing = this[p];
            if (existing.Centerline is not null || existing.BreakPoint)
            {
                StdDevChannelsResult cleared = new(existing.Timestamp);
                UpdateInternal(p, cleared);
            }
        }

        // Process windows in reverse order, starting from the last complete window
        // This matches the series implementation: for (int i = length - 1; i >= lookbackPeriods - 1; i -= lookbackPeriods)
        for (int i = currentCount - 1; i >= lookbackPeriods - 1; i -= lookbackPeriods)
        {
            SlopeResult s = slopeList[i];
            double? width = stdDeviations * s.StdDev;

            // Calculate regression line and channels for this window
            for (int p = i - lookbackPeriods + 1; p <= i; p++)
            {
                if (p < 0)
                {
                    continue;
                }

                StdDevChannelsResult existing = this[p];

                // Calculate centerline: y = mx + b
                // Use global index (accounting for pruned items) for x
                int globalIndex = globalIndexOffset + p;
                double? c = (s.Slope * (globalIndex + 1)) + s.Intercept;

                // Create updated result with channels
                StdDevChannelsResult updated = existing with {
                    Centerline = c,
                    UpperChannel = c + width,
                    LowerChannel = c - width,
                    BreakPoint = p == i - lookbackPeriods + 1
                };

                UpdateInternal(p, updated);
            }
        }
    }
}

public static partial class StdDevChannels
{
    /// <summary>
    /// Creates a buffer list for Standard Deviation Channels calculations.
    /// </summary>
    /// <param name="source">Collection of input values, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="stdDeviations"></param>
    public static StdDevChannelsList ToStdDevChannelsList(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods = 20,
        double stdDeviations = 2)
        => new(lookbackPeriods, stdDeviations) { source };
}
