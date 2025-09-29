namespace Skender.Stock.Indicators;

// VOLUME WEIGHTED MOVING AVERAGE (STREAM HUB)

/// <summary>
/// Provides methods for creating VWMA hubs.
/// </summary>
public static partial class Vwma
{
    /// <summary>
    /// Converts the chain provider to a VWMA hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>A VWMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static VwmaHub<TIn> ToVwma<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods)
        where TIn : IQuote
    {
        ArgumentNullException.ThrowIfNull(chainProvider);
        return new(chainProvider, lookbackPeriods);
    }
}

/// <summary>
/// Represents a Volume Weighted Moving Average (VWMA) stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class VwmaHub<TIn>
    : ChainProvider<TIn, VwmaResult>, IVwma
    where TIn : IQuote
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="VwmaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal VwmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Vwma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"VWMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    // METHODS

    /// <inheritdoc />
    public override string ToString() => hubName;

    /// <inheritdoc />
    protected override (VwmaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int index = indexHint ?? ProviderCache.IndexOf(item, true);

        double vwma = double.NaN;

        if (index + 1 >= LookbackPeriods)
        {
            double priceVolumeSum = 0;
            double volumeSum = 0;

            for (int p = index + 1 - LookbackPeriods; p <= index; p++)
            {
                TIn quote = ProviderCache[p];
                double price = (double)quote.Close;
                double volume = (double)quote.Volume;

                priceVolumeSum += price * volume;
                volumeSum += volume;
            }

            vwma = volumeSum != 0 ? priceVolumeSum / volumeSum : double.NaN;
        }

        VwmaResult result = new(
            Timestamp: item.Timestamp,
            Vwma: vwma.NaN2Null());

        return (result, index);
    }
}
