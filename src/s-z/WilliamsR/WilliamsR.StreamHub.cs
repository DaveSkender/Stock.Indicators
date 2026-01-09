namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a Williams %R stream hub.
/// </summary>
public class WilliamsRHub
    : StreamHub<IQuote, WilliamsResult>, IWilliamsR
{
    #region constructors

    private readonly RollingWindowMax<decimal> _highWindow;
    private readonly RollingWindowMin<decimal> _lowWindow;

    internal WilliamsRHub(
        IStreamObservable<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        WilliamsR.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        _highWindow = new RollingWindowMax<decimal>(lookbackPeriods);
        _lowWindow = new RollingWindowMin<decimal>(lookbackPeriods);

        Name = $"WILLR({lookbackPeriods})";

        Reinitialize();
    }

    #endregion constructors

    #region properties

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    #endregion properties

    #region methods
    /// <inheritdoc/>
    protected override (WilliamsResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Add current high/low to rolling windows (only require High/Low, not Close)
        bool hasHL = !double.IsNaN((double)item.High) &&
                     !double.IsNaN((double)item.Low);
        bool hasClose = !double.IsNaN((double)item.Close);

        if (hasHL)
        {
            _highWindow.Add(item.High);
            _lowWindow.Add(item.Low);
        }

        // Calculate Williams %R
        double williamsR = double.NaN;
        if (i >= LookbackPeriods - 1 && hasHL && hasClose)
        {
            // Get highest high and lowest low from rolling windows (O(1))
            decimal highHigh = _highWindow.GetMax();
            decimal lowLow = _lowWindow.GetMin();

            // Apply boundary clamping to ensure -100 ≤ WilliamsR ≤ 0
            // This prevents floating-point precision errors at boundaries
            if (highHigh == lowLow)
            {
                williamsR = -100.0;
            }
            else if (item.Close >= highHigh)
            {
                williamsR = 0.0;
            }
            else if (item.Close <= lowLow)
            {
                williamsR = -100.0;
            }
            else
            {
                williamsR = (100 * ((double)item.Close - (double)lowLow) / ((double)highHigh - (double)lowLow)) - 100;
            }
        }

        WilliamsResult result = new(
            Timestamp: item.Timestamp,
            WilliamsR: williamsR.NaN2Null());

        return (result, i);
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
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

        // Rebuild rolling windows from ProviderCache
        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];

            // Only require High/Low to rebuild windows (not Close)
            if (!double.IsNaN((double)quote.High) &&
                !double.IsNaN((double)quote.Low))
            {
                _highWindow.Add(quote.High);
                _lowWindow.Add(quote.Low);
            }
        }
    }

    #endregion methods

}

public static partial class WilliamsR
{
    /// <summary>
    /// Converts the quote provider to a Williams %R hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A Williams %R hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static WilliamsRHub ToWilliamsRHub(
        this IStreamObservable<IQuote> quoteProvider,
        int lookbackPeriods = 14)
             => new(quoteProvider, lookbackPeriods);
}
