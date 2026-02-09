namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Choppiness Index (CHOP) on a series of quotes.
/// </summary>
public class ChopHub
    : ChainHub<IQuote, ChopResult>, IChop
{
    private readonly RollingWindowMax<double> _trueHighWindow;
    private readonly RollingWindowMin<double> _trueLowWindow;
    private readonly Queue<double> _trueRangeBuffer;
    private double _sumTrueRange;

    internal ChopHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        Chop.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _trueHighWindow = new RollingWindowMax<double>(lookbackPeriods);
        _trueLowWindow = new RollingWindowMin<double>(lookbackPeriods);
        _trueRangeBuffer = new Queue<double>(lookbackPeriods);
        _sumTrueRange = 0;
        Name = $"CHOP({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }
    /// <inheritdoc/>
    protected override (ChopResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // skip first period (no previous close)
        if (i == 0)
        {
            return (new ChopResult(item.Timestamp, null), i);
        }

        double? chop = null;

        // Calculate true high, true low, and true range for current period
        double prevClose = (double)ProviderCache[i - 1].Close;
        double trueHigh = Math.Max((double)item.High, prevClose);
        double trueLow = Math.Min((double)item.Low, prevClose);
        double trueRange = trueHigh - trueLow;

        // Add to rolling windows
        _trueHighWindow.Add(trueHigh);
        _trueLowWindow.Add(trueLow);

        // Update sum of true range using rolling buffer
        double? dequeuedTR = _trueRangeBuffer.UpdateWithDequeue(LookbackPeriods, trueRange);
        if (dequeuedTR.HasValue)
        {
            _sumTrueRange = _sumTrueRange - dequeuedTR.Value + trueRange;
        }
        else
        {
            _sumTrueRange += trueRange;
        }

        // calculate CHOP when we have enough data
        if (i >= LookbackPeriods)
        {
            // Get max/min from rolling windows (O(1))
            double maxTrueHigh = _trueHighWindow.GetMax();
            double minTrueLow = _trueLowWindow.GetMin();
            double range = maxTrueHigh - minTrueLow;

            // calculate CHOP
            if (range != 0)
            {
                chop = 100 * (Math.Log(_sumTrueRange / range) / Math.Log(LookbackPeriods));
            }
        }

        // candidate result
        ChopResult r = new(
            Timestamp: item.Timestamp,
            Chop: chop);

        return (r, i);
    }

    /// <summary>
    /// Restores the rolling window state up to the specified timestamp.
    /// Clears and rebuilds rolling windows and true range buffer from ProviderCache for Insert/Remove operations.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear rolling windows and buffer
        _trueHighWindow.Clear();
        _trueLowWindow.Clear();
        _trueRangeBuffer.Clear();
        _sumTrueRange = 0;

        // Find target index in ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index == -1)
        {
            index = ProviderCache.Count;
        }

        if (index <= 0)
        {
            return;
        }

        // Rebuild up to the index before the rollback timestamp
        int targetIndex = index - 1;
        int startIdx = Math.Max(1, targetIndex + 1 - LookbackPeriods);

        // Rebuild rolling windows and buffer from ProviderCache
        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote current = ProviderCache[p];
            double prevClose = (double)ProviderCache[p - 1].Close;

            double trueHigh = Math.Max((double)current.High, prevClose);
            double trueLow = Math.Min((double)current.Low, prevClose);
            double trueRange = trueHigh - trueLow;

            _trueHighWindow.Add(trueHigh);
            _trueLowWindow.Add(trueLow);
            _trueRangeBuffer.Enqueue(trueRange);
            _sumTrueRange += trueRange;
        }
    }
}

public static partial class Chop
{
    /// <summary>
    /// Creates a Choppiness Index (CHOP) streaming hub from a quote provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A ChopHub instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static ChopHub ToChopHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 14)
             => new(quoteProvider, lookbackPeriods);
}
