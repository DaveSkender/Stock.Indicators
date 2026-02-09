namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Accumulation/Distribution Line (ADL).
/// </summary>
public class AdlHub : ChainHub<IQuote, AdlResult>
{
    internal AdlHub(IQuoteProvider<IQuote> provider)
        : base(provider)
    {
        Name = "ADL";
        Reinitialize();
    }

    /// <inheritdoc/>
    protected override (AdlResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // candidate result
        AdlResult r = Adl.Increment(
            item.Timestamp,
            item.High,
            item.Low,
            item.Close,
            item.Volume,
            i > 0 ? Cache[i - 1].Value : 0);

        return (r, i);
    }

    /// <inheritdoc/>
    public override string ToString() => Cache.Count == 0 ? "ADL" : $"ADL({Cache[0].Timestamp:d})";
}

/// <summary>
/// Provides methods for the Accumulation/Distribution Line (ADL) indicator.
/// </summary>
public static partial class Adl
{
    /// <summary>
    /// Creates an AdlHub that is subscribed to an IQuoteProvider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    public static AdlHub ToAdlHub(
        this IQuoteProvider<IQuote> quoteProvider)
        => new(quoteProvider);
}
