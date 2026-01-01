namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Fisher Transform indicator using a stream hub.
/// </summary>
public class FisherTransformHub
    : ChainHub<IReusable, FisherTransformResult>, IFisherTransform
{

    /// <summary>
    /// State arrays for Fisher Transform algorithm
    /// These arrays grow with each added value to support indexed lookback access
    /// </summary>
    private readonly List<double> xv = []; // value transform (intermediate state)

    /// <summary>
    /// Rolling windows for O(1) price min/max tracking
    /// </summary>
    private readonly RollingWindowMax<double> _priceMaxWindow;
    private readonly RollingWindowMin<double> _priceMinWindow;

    internal FisherTransformHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods = 10) : base(provider)
    {
        FisherTransform.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _priceMaxWindow = new RollingWindowMax<double>(lookbackPeriods);
        _priceMinWindow = new RollingWindowMin<double>(lookbackPeriods);
        Name = $"FISHER({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }
    /// <inheritdoc/>
    protected override (FisherTransformResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        // NOTE: Fisher Transform has internal state (xv).  We maintain state arrays that are
        // truncated on rebuild events.  See RollbackState() override below.

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Ensure arrays have enough capacity
        while (xv.Count <= i)
        {
            xv.Add(0);
        }

        // prefer HL2 when source is an IQuote
        double currentValue = item.Hl2OrValue();

        // Add current price to rolling windows (skip NaN values)
        if (!double.IsNaN(currentValue))
        {
            _priceMaxWindow.Add(currentValue);
            _priceMinWindow.Add(currentValue);
        }

        // Get min/max from rolling windows (O(1)), or fallback to currentValue if window empty
        double maxPrice;
        double minPrice;

        if (_priceMaxWindow.Count > 0)
        {
            maxPrice = _priceMaxWindow.GetMax();
            minPrice = _priceMinWindow.GetMin();
        }
        else
        {
            // Fallback when all values in warmup are NaN
            maxPrice = currentValue;
            minPrice = currentValue;
        }

        double fisher;
        double? trigger = null;

        if (i > 0)
        {
            // calculate current xv
            xv[i] = maxPrice - minPrice != 0
                ? (0.33 * 2 * (((currentValue - minPrice) / (maxPrice - minPrice)) - 0.5))
                      + (0.67 * xv[i - 1])
                : 0d;

            // limit xv to prevent log issues
            xv[i] = xv[i] > 0.99 ? 0.999 : xv[i];
            xv[i] = xv[i] < -0.99 ? -0.999 : xv[i];

            // calculate Fisher Transform
            fisher = DeMath.Atanh(xv[i]) + (0.5d * Cache[i - 1].Fisher);

            trigger = Cache[i - 1].Fisher;
        }
        else
        {
            xv[i] = 0;
            fisher = 0;
        }

        // candidate result
        FisherTransformResult r = new(
            Timestamp: item.Timestamp,
            Fisher: fisher,
            Trigger: trigger);

        return (r, i);
    }

    /// <summary>
    /// Restores the rolling window and xv state up to the specified timestamp.
    /// Clears and rebuilds rolling windows and xv array from ProviderCache for Insert/Remove operations.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear rolling windows
        _priceMaxWindow.Clear();
        _priceMinWindow.Clear();

        int index = ProviderCache.IndexGte(timestamp);

        if (index <= 0)
        {
            xv.Clear();
            return;
        }

        // Truncate xv array to rollback point
        if (index < xv.Count)
        {
            int removeCount = xv.Count - index;
            xv.RemoveRange(index, removeCount);
        }

        // Rebuild rolling windows from ProviderCache
        int targetIndex = index - 1;
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

        for (int p = startIdx; p <= targetIndex; p++)
        {
            double priceValue = ProviderCache[p].Hl2OrValue();

            // Only add non-NaN values to windows
            if (!double.IsNaN(priceValue))
            {
                _priceMaxWindow.Add(priceValue);
                _priceMinWindow.Add(priceValue);
            }
        }
    }
}

public static partial class FisherTransform
{
    /// <summary>
    /// Creates a Fisher Transform streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation. Default is 10.</param>
    /// <returns>A Fisher Transform hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static FisherTransformHub ToFisherTransformHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 10)
             => new(chainProvider, lookbackPeriods);
}
