namespace Skender.Stock.Indicators;

/// <inheritdoc />
public class TrHub
    : ChainHub<IQuote, TrResult>
{
    internal TrHub(IQuoteProvider<IQuote> provider)
        : base(provider)
    {
        Name = "TRUE RANGE";
        Reinitialize();
    }

    /// <inheritdoc/>
    protected override (TrResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // skip first period
        if (i == 0)
        {
            return (new TrResult(item.Timestamp, null), i);
        }

        IQuote prev = ProviderCache[i - 1];

        // candidate result
        TrResult r = new(
            item.Timestamp,
            Tr.Increment(
                (double)item.High,
                (double)item.Low,
                (double)prev.Close));

        return (r, i);
    }
}

public static partial class Tr
{
    /// <summary>
    /// Converts a quote provider to a True Range (TR) hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <returns>A True Range (TR) hub.</returns>
    public static TrHub ToTrHub(
        this IQuoteProvider<IQuote> quoteProvider)
             => new(quoteProvider);
}
