namespace Skender.Stock.Indicators;


/// <summary>
/// Represents a Williams %R stream hub.
/// </summary>
public class WilliamsRHub
    : StreamHub<IQuote, WilliamsResult>, IWilliamsR
{
    #region constructors

    private readonly string hubName;
    private readonly RollingWindowMax<double> _highWindow;
    private readonly RollingWindowMin<double> _lowWindow;

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
        _highWindow = new RollingWindowMax<double>(lookbackPeriods);
        _lowWindow = new RollingWindowMin<double>(lookbackPeriods);

        hubName = $"WILLR({lookbackPeriods})";

        Reinitialize();
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the lookback periods for Williams %R calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    #endregion

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (WilliamsResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Add current high/low to rolling windows
        bool isViable = !double.IsNaN((double)item.High) &&
                       !double.IsNaN((double)item.Low) &&
                       !double.IsNaN((double)item.Close);

        if (isViable)
        {
            _highWindow.Add((double)item.High);
            _lowWindow.Add((double)item.Low);
        }

        // Calculate Williams %R
        double williamsR = double.NaN;
        if (i >= LookbackPeriods - 1 && isViable)
        {
            // Get highest high and lowest low from rolling windows (O(1))
            double highHigh = _highWindow.Max;
            double lowLow = _lowWindow.Min;

            williamsR = highHigh - lowLow != 0
                ? (100 * ((double)item.Close - lowLow) / (highHigh - lowLow)) - 100
                : 0;
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

            // Only add viable quotes to windows
            if (!double.IsNaN((double)quote.High) &&
                !double.IsNaN((double)quote.Low) &&
                !double.IsNaN((double)quote.Close))
            {
                _highWindow.Add((double)quote.High);
                _lowWindow.Add((double)quote.Low);
            }
        }
    }

    #endregion

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

