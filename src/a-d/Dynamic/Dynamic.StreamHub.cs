namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the McGinley Dynamic indicator.
/// </summary>
public class DynamicHub
    : ChainProvider<IReusable, DynamicResult>, IDynamic
{

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="kFactor">The smoothing factor for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods or kFactor are invalid.</exception>
    internal DynamicHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        double kFactor = 0.6) : base(provider)
    {
        MgDynamic.Validate(lookbackPeriods, kFactor);
        LookbackPeriods = lookbackPeriods;
        KFactor = kFactor;
        Name = $"DYNAMIC({lookbackPeriods},{kFactor})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double KFactor { get; init; }
    /// <inheritdoc/>
    protected override (DynamicResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // skip first period
        if (i == 0)
        {
            DynamicResult firstResult = new(
                Timestamp: item.Timestamp,
                Dynamic: null);

            return (firstResult, i);
        }

        // calculate dynamic
        double prevDyn = Cache[i - 1].Dynamic ?? ProviderCache[i - 1].Value;
        double dyn = MgDynamic.Increment(
            LookbackPeriods,
            KFactor,
            newVal: item.Value,
            prevDyn: prevDyn);

        // candidate result
        DynamicResult r = new(
            Timestamp: item.Timestamp,
            Dynamic: dyn.NaN2Null());

        return (r, i);
    }
}

public static partial class MgDynamic
{
    /// <summary>
    /// Creates a Dynamic streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="kFactor">The smoothing factor for the calculation.</param>
    /// <returns>A Dynamic hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods or kFactor are invalid.</exception>
    public static DynamicHub ToDynamicHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods,
        double kFactor = 0.6)
             => new(chainProvider, lookbackPeriods, kFactor);

    /// <summary>
    /// Creates a Dynamic hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="kFactor">The smoothing factor for the calculation.</param>
    /// <returns>An instance of <see cref="DynamicHub"/>.</returns>
    public static DynamicHub ToDynamicHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods,
        double kFactor = 0.6)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDynamicHub(lookbackPeriods, kFactor);
    }

}
