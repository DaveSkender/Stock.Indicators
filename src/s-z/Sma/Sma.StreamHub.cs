namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAM HUB)

/// <summary>
/// Provides methods for creating SMA hubs.
/// </summary>
public static partial class Sma
{
    /// <summary>
    /// Converts the chain provider to an SMA hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>An SMA hub.</returns>
    public static SmaHub<TIn> ToSma<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods)
        where TIn : IReusable
        => new(chainProvider, lookbackPeriods);
}

/// <summary>
/// Represents a Simple Moving Average (SMA) stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class SmaHub<TIn> : IndicatorStreamHubBase<TIn, SmaResult>, ISinglePeriodIndicator
    where TIn : IReusable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SmaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal SmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider, "SMA", lookbackPeriods)
    {
        LookbackPeriods = lookbackPeriods;
        
        // Initialize with validation
        Initialize();
    }

    /// <inheritdoc/>
    public override int LookbackPeriods { get; }

    /// <inheritdoc/>
    protected override void ValidateParameters()
    {
        IndicatorUtilities.ValidateLookbackPeriods(LookbackPeriods, "SMA");
    }

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
