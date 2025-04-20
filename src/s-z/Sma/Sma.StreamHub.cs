namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for Simple Moving Average (SMA) calculations.
/// </summary>
public interface ISma
{
    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    int LookbackPeriods { get; }
}

/// <summary>
/// Provides methods for calculating the Simple Moving Average (SMA) indicator.
/// </summary>
public static partial class Sma
{
    /// <summary>
    /// Creates an SMA hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An SMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    [StreamHub("SMA", "Simple Moving Average", Category.MovingAverage, ChartType.Overlay)]
    public static SmaHub<T> ToSma<T>(
        this IChainProvider<T> chainProvider,

        [Param("Lookback Periods", 2, 250, 20)]
        int lookbackPeriods)
        where T : IReusable
        => new(chainProvider, lookbackPeriods);
}

/// <summary>
/// Represents a hub for Simple Moving Average (SMA) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class SmaHub<TIn>
    : ChainProvider<TIn, SmaResult>, ISma
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="SmaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal SmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"SMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (SmaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // candidate result
        SmaResult r = new(
            Timestamp: item.Timestamp,
            Sma: Sma.Increment(ProviderCache, LookbackPeriods, i).NaN2Null());

        return (r, i);
    }
}
