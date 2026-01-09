namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for creating VWMA hubs.
/// </summary>
public class VwmaHub
    : ChainHub<IReusable, VwmaResult>, IVwma
{
    internal VwmaHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        Vwma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"VWMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }


    /// <inheritdoc />
    public override string ToString() => Name;

    /// <inheritdoc />
    protected override (VwmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int index = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate VWMA efficiently using a rolling window over ProviderCache
        // This is O(lookbackPeriods) which is constant for a given configuration
        // and maintains exact precision with Series implementation
        double? vwma = null;

        if (index >= LookbackPeriods - 1)
        {
            double priceVolumeSum = 0;
            double volumeSum = 0;

            for (int p = index - LookbackPeriods + 1; p <= index; p++)
            {
                IQuote quote = (IQuote)ProviderCache[p];
                double price = (double)quote.Close;
                double volume = (double)quote.Volume;

                priceVolumeSum += price * volume;
                volumeSum += volume;
            }

            vwma = volumeSum != 0 ? priceVolumeSum / volumeSum : null;
        }

        VwmaResult result = new(
            Timestamp: item.Timestamp,
            Vwma: vwma);

        return (result, index);
    }
}

public static partial class Vwma
{
    /// <summary>
    /// Converts the quote provider to a VWMA hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A VWMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static VwmaHub ToVwmaHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, lookbackPeriods);
    }
}
