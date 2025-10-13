namespace Skender.Stock.Indicators;

// TRUE RANGE (STREAM HUB)

public class TrHub<TIn>
    : ChainProvider<TIn, TrResult>
    where TIn : IQuote
{
    #region constructors

    private const string hubName = "TRUE RANGE";

    /// <summary>
    /// Initializes a new instance of the <see cref="TrHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    internal TrHub(IQuoteProvider<TIn> provider)
        : base(provider)
    {
        Reinitialize();
    }
    #endregion

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (TrResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // skip first period
        if (i == 0)
        {
            return (new TrResult(item.Timestamp, null), i);
        }

        TIn prev = ProviderCache[i - 1];

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
    /// <typeparam name="TIn">The type of quote.</typeparam>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <returns>A True Range (TR) hub.</returns>
    public static TrHub<TIn> ToTrHub<TIn>(
        this IQuoteProvider<TIn> quoteProvider)
        where TIn : IQuote
        => new(quoteProvider);

    /// <summary>
    /// Creates a Tr hub from a collection of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The collection of quotes.</param>
    /// <returns>An instance of <see cref="TrHub{TQuote}"/>.</returns>
    public static TrHub<TQuote> ToTrHub<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote
    {
        QuoteHub<TQuote> quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToTrHub();
    }

}
