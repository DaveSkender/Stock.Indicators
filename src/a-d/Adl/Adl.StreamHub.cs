namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for the Accumulation/Distribution Line (ADL) indicator.
/// </summary>
public static partial class Adl
{
    /// <summary>
    /// Converts the quote provider to an ADL hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input quote.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <returns>An instance of <see cref="AdlHub{TIn}"/>.</returns>
    public static AdlHub<TIn> ToAdl<TIn>(
        this IQuoteProvider<TIn> quoteProvider)
        where TIn : IQuote
        => new(quoteProvider);
}

/// <summary>
/// Represents a hub for the Accumulation/Distribution Line (ADL) indicator.
/// </summary>
/// <typeparam name="TIn">The type of the input quote.</typeparam>
public class AdlHub<TIn> : ChainProvider<TIn, AdlResult>
    where TIn : IQuote
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdlHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    internal AdlHub(IQuoteProvider<TIn> provider)
        : base(provider)
    {
        Reinitialize();
    }

    /// <inheritdoc/>
    protected override (AdlResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
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
