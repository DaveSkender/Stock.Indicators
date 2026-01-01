namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Donchian Channels using a stream hub.
/// </summary>
public static partial class Donchian
{
    /// <summary>
    /// Creates a Donchian Channels streaming hub from a quotes provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="DonchianHub"/>.</returns>
    public static DonchianHub ToDonchianHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 20)
        => new(quoteProvider, lookbackPeriods);
}

/// <summary>
/// Streaming hub for Donchian Channels.
/// </summary>
public class DonchianHub
    : StreamHub<IQuote, DonchianResult>, IDonchian
{
    private readonly RollingWindowMax<decimal> _highWindow;
    private readonly RollingWindowMin<decimal> _lowWindow;

    internal DonchianHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        Donchian.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        Name = $"DONCHIAN({lookbackPeriods})";
        _highWindow = new RollingWindowMax<decimal>(lookbackPeriods);
        _lowWindow = new RollingWindowMin<decimal>(lookbackPeriods);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }
    /// <inheritdoc/>
    protected override (DonchianResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // handle warmup periods
        // Note: Donchian looks at PRIOR periods (not including current)
        // We need LookbackPeriods items in the window before we can calculate
        if (i < LookbackPeriods)
        {
            // Build up the window during warmup
            _highWindow.Add(item.High);
            _lowWindow.Add(item.Low);
            return (new DonchianResult(item.Timestamp), i);
        }

        // Get highest high and lowest low from rolling windows (O(1))
        // This gives us the max/min of the PRIOR LookbackPeriods items
        decimal upperBand = _highWindow.GetMax();
        decimal lowerBand = _lowWindow.GetMin();
        decimal centerline = (upperBand + lowerBand) / 2m;
        decimal? width = centerline == 0 ? null : (upperBand - lowerBand) / centerline;

        // Add current item to windows AFTER getting the result
        // This maintains the invariant that windows contain the prior LookbackPeriods items
        _highWindow.Add(item.High);
        _lowWindow.Add(item.Low);

        DonchianResult r = new(
            Timestamp: item.Timestamp,
            UpperBand: upperBand,
            Centerline: centerline,
            LowerBand: lowerBand,
            Width: width);

        return (r, i);
    }

    /// <summary>
    /// Restores the rolling window state up to the specified timestamp.
    /// Clears and rebuilds rolling windows from ProviderCache for Insert/Remove operations.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear rolling windows
        _highWindow.Clear();
        _lowWindow.Clear();

        // Find target index in ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        // Rebuild up to the index before the rollback timestamp
        int targetIndex = index - 1;
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

        // Rebuild rolling windows from ProviderCache
        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            _highWindow.Add(quote.High);
            _lowWindow.Add(quote.Low);
        }
    }
}
