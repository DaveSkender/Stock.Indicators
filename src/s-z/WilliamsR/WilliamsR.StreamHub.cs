namespace Skender.Stock.Indicators;


/// <summary>
/// Represents a Williams %R stream hub.
/// </summary>
public class WilliamsRHub
    : StreamHub<IQuote, WilliamsResult>, IWilliamsR
{
    #region constructors

    private readonly string hubName;
    private readonly RollingWindowMax<decimal> _highWindow;
    private readonly RollingWindowMin<decimal> _lowWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsRHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The lookback period for Williams %R.</param>
    internal WilliamsRHub(
        IStreamObservable<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        WilliamsR.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        _highWindow = new RollingWindowMax<decimal>(lookbackPeriods);
        _lowWindow = new RollingWindowMin<decimal>(lookbackPeriods);

        hubName = $"WILLR({lookbackPeriods})";

        Reinitialize();
    }

    #endregion constructors

    #region properties

    /// <summary>
    /// Gets the lookback periods for Williams %R calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    #endregion properties

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

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
            decimal highHigh = _highWindow.Max;
            decimal lowLow = _lowWindow.Min;

            // Return NaN when range is zero (undefined %R)
            williamsR = highHigh == lowLow
                ? double.NaN
                : (100 * ((double)item.Close - (double)lowLow) / ((double)highHigh - (double)lowLow)) - 100;
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
    /// <param name="lookbackPeriods">The lookback period for Williams %R.</param>
    /// <returns>A Williams %R hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static WilliamsRHub ToWilliamsRHub(
        this IStreamObservable<IQuote> quoteProvider,
        int lookbackPeriods = 14)
             => new(quoteProvider, lookbackPeriods);

    /// <summary>
    /// Creates a WilliamsR hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The lookback period for Williams %R. Default is 14.</param>
    /// <returns>An instance of <see cref="WilliamsRHub"/>.</returns>
    public static WilliamsRHub ToWilliamsRHub(
        this IReadOnlyList<IQuote> quotes, int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToWilliamsRHub(lookbackPeriods);
    }
}

