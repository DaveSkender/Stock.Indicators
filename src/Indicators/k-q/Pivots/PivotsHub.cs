namespace FacioQuo.Stock.Indicators;

/// <summary>
/// Streaming hub for Pivot Points using a stream hub.
/// </summary>
public class PivotsHub
    : StreamHub<IBar, PivotsResult>, IPivots
{
    internal PivotsHub(
        IBarProvider<IBar> provider,
        int leftSpan,
        int rightSpan,
        int maxTrendPeriods,
        EndType endType) : base(provider)
    {
        Pivots.Validate(leftSpan, rightSpan, maxTrendPeriods);

        LeftSpan = leftSpan;
        RightSpan = rightSpan;
        MaxTrendPeriods = maxTrendPeriods;
        EndType = endType;
        Name = $"PIVOTS({leftSpan},{rightSpan},{maxTrendPeriods},{endType.ToString().ToUpperInvariant()})";

        // Validate cache size for warmup requirements
        int requiredWarmup = leftSpan + rightSpan + 1;
        ValidateCacheSize(requiredWarmup, Name);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LeftSpan { get; init; }

    /// <inheritdoc/>
    public int RightSpan { get; init; }

    /// <inheritdoc/>
    public int MaxTrendPeriods { get; init; }

    /// <inheritdoc/>
    public EndType EndType { get; init; }

    /// <inheritdoc/>
    /// <remarks>
    /// A pivot is decided <see cref="RightSpan"/> bars after its own bar, once its right wing
    /// exists. Deciding one repaints only the trend segment back to the previous pivot.
    /// </remarks>
    public override void OnAdd(IBar item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (CacheLock)
        {
            base.OnAdd(item, notify, indexHint);

            int i = indexHint ?? ProviderCache.IndexOf(item, true);
            int k = i - RightSpan;

            if (k < LeftSpan || k >= Cache.Count)
            {
                return;
            }

            (decimal? highPoint, decimal? lowPoint) = FindPivot(k);
            Cache[k] = Cache[k] with { HighPoint = highPoint, LowPoint = lowPoint };

            int highStart = highPoint is null ? k : PaintSegment(k, isHigh: true);
            int lowStart = lowPoint is null ? k : PaintSegment(k, isHigh: false);
            int repaintedFrom = Math.Min(highStart, lowStart);

            if (notify && (highPoint is not null || lowPoint is not null) && repaintedFrom < Cache.Count - 1)
            {
                NotifyObserversOnRebuild(Cache[repaintedFrom].Timestamp);
            }
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// A change at a bar can move a pivot up to <see cref="RightSpan"/> bars earlier, and that
    /// pivot's trend segment reaches back up to <see cref="MaxTrendPeriods"/> bars more, so the
    /// rebuild starts that far back.
    /// </remarks>
    public override void Rebuild(DateTime fromTimestamp)
    {
        lock (CacheLock)
        {
            int fromIndex = Cache.IndexGte(fromTimestamp);
            int start = (fromIndex < 0 ? Cache.Count : fromIndex) - RightSpan - MaxTrendPeriods;

            base.Rebuild(start <= 0 ? DateTime.MinValue : Cache[start].Timestamp);
        }
    }

    /// <inheritdoc/>
    protected override (PivotsResult result, int index)
        ToIndicator(IBar item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // undecided until its right wing arrives; OnAdd fills it in
        PivotsResult result = new(
            Timestamp: item.Timestamp,
            HighPoint: null,
            LowPoint: null,
            HighLine: null,
            LowLine: null,
            HighTrend: null,
            LowTrend: null);

        return (result, i);
    }

    private (decimal? highPoint, decimal? lowPoint) FindPivot(int k)
    {
        IBar center = ProviderCache[k];
        decimal evalHigh = EndType == EndType.Close ? center.Close : center.High;
        decimal evalLow = EndType == EndType.Close ? center.Close : center.Low;
        bool isHigh = true;
        bool isLow = true;

        for (int p = k - LeftSpan; p <= k + RightSpan; p++)
        {
            if (p == k)
            {
                continue;
            }

            IBar wing = ProviderCache[p];

            if (evalHigh <= (EndType == EndType.Close ? wing.Close : wing.High))
            {
                isHigh = false;
            }

            if (evalLow >= (EndType == EndType.Close ? wing.Close : wing.Low))
            {
                isLow = false;
            }
        }

        return (isHigh ? evalHigh : null, isLow ? evalLow : null);
    }

    /// <summary>
    /// Repaints the trend segment ending at the pivot at <paramref name="k"/>, as Series does.
    /// </summary>
    /// <remarks>
    /// The segment starts at the previous pivot of the same side when it lies within
    /// <see cref="MaxTrendPeriods"/> bars and differs in value; an older one has expired.
    /// </remarks>
    /// <param name="k">Index of the newly decided pivot.</param>
    /// <param name="isHigh">Whether to paint the high trend rather than the low.</param>
    /// <returns>The first repainted index, or <paramref name="k"/> when nothing was repainted.</returns>
    private int PaintSegment(int k, bool isHigh)
    {
        decimal? point = isHigh ? Cache[k].HighPoint : Cache[k].LowPoint;
        int j = k - 1;

        while (j >= Math.Max(0, k - MaxTrendPeriods)
            && (isHigh ? Cache[j].HighPoint : Cache[j].LowPoint) is null)
        {
            j--;
        }

        decimal? prior = j >= 0 && j >= k - MaxTrendPeriods
            ? isHigh ? Cache[j].HighPoint : Cache[j].LowPoint
            : null;

        if (prior is null || prior == point)
        {
            return k;
        }

        decimal? incr = (point - prior) / (k - j);

        if (isHigh)
        {
            PivotTrend trend = point > prior ? PivotTrend.Hh : PivotTrend.Lh;
            Cache[j] = Cache[j] with { HighLine = prior };

            for (int t = j + 1; t <= k; t++)
            {
                Cache[t] = Cache[t] with { HighLine = point + (incr * (t - k)), HighTrend = trend };
            }
        }
        else
        {
            PivotTrend trend = point > prior ? PivotTrend.Hl : PivotTrend.Ll;
            Cache[j] = Cache[j] with { LowLine = prior };

            for (int t = j + 1; t <= k; t++)
            {
                Cache[t] = Cache[t] with { LowLine = point + (incr * (t - k)), LowTrend = trend };
            }
        }

        return j;
    }
}

public static partial class Pivots
{
    /// <summary>
    /// Creates a Pivots streaming hub from a bar provider.
    /// </summary>
    /// <param name="barProvider">Bar provider.</param>
    /// <param name="leftSpan">Number of periods to the left of the pivot point. Default is 2.</param>
    /// <param name="rightSpan">Number of periods to the right of the pivot point. Default is 2.</param>
    /// <param name="maxTrendPeriods">Maximum number of periods for trend calculation. Default is 20.</param>
    /// <param name="endType">Type of end point for the pivot calculation. Default is <see cref="EndType.HighLow"/>.</param>
    /// <returns>An instance of <see cref="PivotsHub"/>.</returns>
    public static PivotsHub ToPivotsHub(
        this IBarProvider<IBar> barProvider,
        int leftSpan = 2,
        int rightSpan = 2,
        int maxTrendPeriods = 20,
        EndType endType = EndType.HighLow)
    {
        ArgumentNullException.ThrowIfNull(barProvider);
        return new(barProvider, leftSpan, rightSpan, maxTrendPeriods, endType);
    }
}
