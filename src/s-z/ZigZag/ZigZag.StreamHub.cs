namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for generating ZigZag series in a streaming manner.
/// </summary>
/// <remarks>
/// <para>
/// ZigZag is a "repaint-by-design" indicator where values from the last confirmed
/// pivot forward may change as new data arrives, but earlier pivots remain stable.
/// </para>
/// <para>
/// This implementation maintains pivot state and only recalculates from the last
/// confirmed pivot forward (O(k) where k = quotes since last pivot), not the entire
/// series (O(n)).
/// </para>
/// </remarks>
public class ZigZagHub
    : ChainProvider<IQuote, ZigZagResult>
{
    #region fields and constructor

    private readonly string hubName;
    private readonly decimal changeThreshold;

    // Pivot state tracking
    private ZigZagPoint lastPoint = new();
    private ZigZagPoint lastHighPoint = new();
    private ZigZagPoint lastLowPoint = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ZigZagHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="endType">The type of price to use for the end of the pivot.</param>
    /// <param name="percentChange">The percentage change threshold for ZigZag points.</param>
    internal ZigZagHub(
        IChainProvider<IQuote> provider,
        EndType endType,
        decimal percentChange) : base(provider)
    {
        ZigZag.Validate(percentChange);
        EndType = endType;
        PercentChange = percentChange;
        changeThreshold = percentChange / 100m;
        hubName = $"ZIGZAG({endType.ToString().ToUpperInvariant()},{percentChange})";

        Reinitialize();
    }

    #endregion fields and constructor

    /// <inheritdoc/>
    public EndType EndType { get; }

    /// <inheritdoc/>
    public decimal PercentChange { get; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <summary>
    /// Converts provider item into ZigZag result.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ZigZag is a repaint-by-design indicator. Only values from the last confirmed
    /// pivot forward may change with new data; earlier pivots are stable.
    /// </para>
    /// <para>
    /// This implementation maintains pivot state and only recalculates from the last
    /// confirmed pivot forward, not the entire series. This improves from O(n) to O(k)
    /// where k = quotes since last pivot.
    /// </para>
    /// </remarks>
    /// <inheritdoc/>
    protected override (ZigZagResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Initialize on first run or when cache is out of sync
        if (Cache.Count == 0)
        {
            InitializeState();
        }

        // Only recalculate from last pivot forward (repaint-from-anchor pattern)
        RecalculateFromPivot();

        return (Cache[i], i);
    }

    /// <summary>
    /// Initializes pivot state from the beginning of the series.
    /// </summary>
    private void InitializeState()
    {
        if (ProviderCache.Count == 0)
        {
            return;
        }

        IQuote q0 = ProviderCache[0];
        ZigZagEval eval = ZigZag.GetZigZagEval(EndType, 1, q0);

        lastPoint = new ZigZagPoint
        {
            Index = eval.Index,
            Value = q0.Close,
            PointType = "U"
        };

        lastHighPoint = new ZigZagPoint
        {
            Index = eval.Index,
            Value = eval.High,
            PointType = "H"
        };

        lastLowPoint = new ZigZagPoint
        {
            Index = eval.Index,
            Value = eval.Low,
            PointType = "L"
        };

        // Find initial trend
        for (int i = 0; i < ProviderCache.Count; i++)
        {
            IQuote q = ProviderCache[i];
            int index = i + 1;

            eval = ZigZag.GetZigZagEval(EndType, index, q);

            decimal? changeUp = lastLowPoint.Value == 0
                ? null
                : (eval.High - lastLowPoint.Value) / lastLowPoint.Value;

            decimal? changeDn = lastHighPoint.Value == 0
                ? null
                : (lastHighPoint.Value - eval.Low) / lastHighPoint.Value;

            if (changeUp >= changeThreshold && changeUp > changeDn)
            {
                lastPoint.Index = lastLowPoint.Index;
                lastPoint.Value = lastLowPoint.Value;
                lastPoint.PointType = lastLowPoint.PointType;
                break;
            }

            if (changeDn >= changeThreshold && changeDn > changeUp)
            {
                lastPoint.Index = lastHighPoint.Index;
                lastPoint.Value = lastHighPoint.Value;
                lastPoint.PointType = lastHighPoint.PointType;
                break;
            }
        }

        // Add first point
        if (Cache.Count == 0)
        {
            Cache.Add(new ZigZagResult(q0.Timestamp));
        }
    }

    /// <summary>
    /// Recalculates ZigZag values from the last confirmed pivot forward.
    /// </summary>
    private void RecalculateFromPivot()
    {
        if (ProviderCache.Count == 0 || lastPoint.Index == 0)
        {
            return;
        }

        // Remove cache entries from last pivot forward
        int pivotIndex = lastPoint.Index - 1; // Convert to 0-based index
        if (pivotIndex < 0) pivotIndex = 0;

        // Keep entries before pivot, remove from pivot forward
        if (Cache.Count > pivotIndex)
        {
            Cache.RemoveRange(pivotIndex, Cache.Count - pivotIndex);
        }

        // Recalculate from pivot forward using static methods
        while (lastPoint.Index < ProviderCache.Count)
        {
            ZigZagPoint nextPoint = ZigZag.EvaluateNextPoint(
                ProviderCache, EndType, changeThreshold, lastPoint);

            string? lastDirection = lastPoint.PointType;

            // Draw line (and reset last point)
            DrawZigZagLine(lastPoint, nextPoint);

            // Draw retrace line (and reset last high/low point)
            if (lastDirection is not null)
            {
                DrawRetraceLine(lastDirection, lastLowPoint, lastHighPoint, nextPoint);
            }
        }
    }

    /// <summary>
    /// Draws a ZigZag line between two points into the cache.
    /// </summary>
    private void DrawZigZagLine(ZigZagPoint fromPoint, ZigZagPoint toPoint)
    {
        if (toPoint.Index == fromPoint.Index)
        {
            return;
        }

        decimal? increment = (toPoint.Value - fromPoint.Value)
                           / (toPoint.Index - fromPoint.Index);

        // Add new line segment
        for (int i = fromPoint.Index; i < toPoint.Index; i++)
        {
            if (i >= ProviderCache.Count) break;

            IQuote q = ProviderCache[i];
            int index = i + 1;

            ZigZagResult result = new(
                Timestamp: q.Timestamp,
                ZigZag: fromPoint.Index != 1 || index == toPoint.Index
                    ? fromPoint.Value + (increment * (index - fromPoint.Index))
                    : null,
                PointType: index == toPoint.Index
                    ? toPoint.PointType
                    : null);

            Cache.Add(result);
        }

        // Reset lastPoint
        fromPoint.Index = toPoint.Index;
        fromPoint.Value = toPoint.Value;
        fromPoint.PointType = toPoint.PointType;
    }

    /// <summary>
    /// Draws a retrace line between two points into the cache.
    /// </summary>
    private void DrawRetraceLine(
        string lastDirection,
        ZigZagPoint lastLowPt,
        ZigZagPoint lastHighPt,
        ZigZagPoint nextPoint)
    {
        ZigZagPoint priorPoint = new();

        switch (lastDirection)
        {
            case "L":
                priorPoint.Index = lastHighPt.Index;
                priorPoint.Value = lastHighPt.Value;
                lastHighPt.Index = nextPoint.Index;
                lastHighPt.Value = nextPoint.Value;
                break;

            case "H":
                priorPoint.Index = lastLowPt.Index;
                priorPoint.Value = lastLowPt.Value;
                lastLowPt.Index = nextPoint.Index;
                lastLowPt.Value = nextPoint.Value;
                break;
        }

        // Nothing to draw cases
        if (lastDirection == "U" || priorPoint.Index == 1 || nextPoint.Index == priorPoint.Index)
        {
            return;
        }

        decimal? increment = (nextPoint.Value - priorPoint.Value)
                           / (nextPoint.Index - priorPoint.Index);

        // Update existing cache entries with retrace lines
        for (int i = priorPoint.Index - 1; i < nextPoint.Index; i++)
        {
            if (i < 0 || i >= Cache.Count) continue;

            ZigZagResult r = Cache[i];
            int index = i + 1;

            switch (lastDirection)
            {
                case "L":
                    decimal? retraceHigh = priorPoint.Value
                                         + (increment * (index - priorPoint.Index));
                    Cache[i] = r with { RetraceHigh = retraceHigh };
                    break;

                case "H":
                    decimal? retraceLow = priorPoint.Value
                                        + (increment * (index - priorPoint.Index));
                    Cache[i] = r with { RetraceLow = retraceLow };
                    break;
            }
        }
    }

    /// <summary>
    /// Restores pivot state after provider history mutations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Restores pivot state (lastPoint, lastHighPoint, lastLowPoint) from the cache
    /// to enable efficient recalculation from the last pivot forward only.
    /// </para>
    /// </remarks>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Find the last confirmed pivot before the rollback timestamp
        int rollbackIndex = Cache.Count > 0
            ? Cache.FindLastIndex(r => r.Timestamp < timestamp && r.PointType != null)
            : -1;

        if (rollbackIndex < 0)
        {
            // No pivot found, reinitialize from scratch
            lastPoint = new ZigZagPoint();
            lastHighPoint = new ZigZagPoint();
            lastLowPoint = new ZigZagPoint();
            return;
        }

        // Restore lastPoint from the last confirmed pivot
        ZigZagResult pivotResult = Cache[rollbackIndex];
        lastPoint.Index = rollbackIndex + 1; // Convert to 1-based index
        lastPoint.Value = pivotResult.ZigZag ?? 0;
        lastPoint.PointType = pivotResult.PointType;

        // Restore lastHighPoint and lastLowPoint by scanning backwards
        // Find the most recent high pivot
        int highIndex = Cache.FindLastIndex(rollbackIndex,
            r => r.PointType == "H");

        if (highIndex >= 0)
        {
            ZigZagResult highResult = Cache[highIndex];
            lastHighPoint.Index = highIndex + 1;
            lastHighPoint.Value = highResult.ZigZag ?? 0;
            lastHighPoint.PointType = "H";
        }
        else
        {
            // Default to first quote if no high pivot found
            if (ProviderCache.Count > 0)
            {
                ZigZagEval eval = ZigZag.GetZigZagEval(EndType, 1, ProviderCache[0]);
                lastHighPoint.Index = 1;
                lastHighPoint.Value = eval.High;
                lastHighPoint.PointType = "H";
            }
        }

        // Find the most recent low pivot
        int lowIndex = Cache.FindLastIndex(rollbackIndex,
            r => r.PointType == "L");

        if (lowIndex >= 0)
        {
            ZigZagResult lowResult = Cache[lowIndex];
            lastLowPoint.Index = lowIndex + 1;
            lastLowPoint.Value = lowResult.ZigZag ?? 0;
            lastLowPoint.PointType = "L";
        }
        else
        {
            // Default to first quote if no low pivot found
            if (ProviderCache.Count > 0)
            {
                ZigZagEval eval = ZigZag.GetZigZagEval(EndType, 1, ProviderCache[0]);
                lastLowPoint.Index = 1;
                lastLowPoint.Value = eval.Low;
                lastLowPoint.PointType = "L";
            }
        }
    }
}


public static partial class ZigZag
{
    /// <summary>
    /// Converts a chain provider to a ZigZag hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="endType">The type of price to use for the end of the pivot.</param>
    /// <param name="percentChange">The percentage change threshold for ZigZag points.</param>
    /// <returns>A ZigZag hub.</returns>
    public static ZigZagHub ToZigZagHub(
        this IChainProvider<IQuote> chainProvider,
        EndType endType = EndType.Close,
        decimal percentChange = 5)
        => new(chainProvider, endType, percentChange);

    /// <summary>
    /// Creates a ZigZag hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="endType">The type of price to use for the end of the pivot.</param>
    /// <param name="percentChange">The percentage change threshold for ZigZag points.</param>
    /// <returns>An instance of <see cref="ZigZagHub"/>.</returns>
    public static ZigZagHub ToZigZagHub(
        this IReadOnlyList<IQuote> quotes,
        EndType endType = EndType.Close,
        decimal percentChange = 5)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToZigZagHub(endType, percentChange);
    }
}
