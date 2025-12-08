namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a Williams %R stream hub.
/// </summary>
public class WilliamsRHub
    : StreamHub<IQuote, WilliamsResult>, IWilliamsR
{

    private readonly string hubName;
    private readonly RollingWindowMax<double> _highWindow;
    private readonly RollingWindowMin<double> _lowWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsRHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
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

    /// <summary>
    /// Gets the lookback periods for Williams %R calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (WilliamsResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double o = (double)item.Open;
        double h = (double)item.High;
        double c = (double)item.Close;

        _highWindow.Add(h);
        _lowWindow.Add(l);

        // Calculate Williams %R
        // Williams %R is Fast Stochastic - 100
        double williamsR = double.NaN;
        if (i >= LookbackPeriods - 1)
        {
            // Get highest high and lowest low from rolling windows (O(1))
            double highHigh = _highWindow.GetMax();
            double lowLow = _lowWindow.GetMin();

            // Return NaN when range is zero (undefined %R)
            williamsR = highHigh - lowLow != 0
                ? -100.0 * (c - lowLow) / (highHigh - lowLow)
                : double.NaN;
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

            _highWindow.Add((double)quote.High);
            _lowWindow.Add((double)quote.Low);
        }
    }

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

    /// <summary>
    /// Creates a WilliamsR hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="WilliamsRHub"/>.</returns>
    public static WilliamsRHub ToWilliamsRHub(
        this IReadOnlyList<IQuote> quotes, int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToWilliamsRHub(lookbackPeriods);
    }
}
