namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for creating OBV hubs.
/// </summary>
public class ObvHub : ChainProvider<IQuote, ObvResult>
{
    internal ObvHub(
        IQuoteProvider<IQuote> provider) : base(provider)
    {
        Name = "OBV";
        Reinitialize();
    }

    /// <inheritdoc/>
    protected override (ObvResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Get previous close and OBV values for calculation
        double prevClose = i > 0 ? (double)ProviderCache[i - 1].Close : double.NaN;
        double prevObv = i > 0 ? Cache[i - 1].Value : 0;

        // Calculate OBV using the Increment method
        ObvResult r = Obv.Increment(
            item.Timestamp,
            (double)item.Close,
            (double)item.Volume,
            prevClose,
            prevObv);

        return (r, i);
    }
}

public static partial class Obv
{
    /// <summary>
    /// Converts the quote provider to an OBV hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <returns>An OBV hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    public static ObvHub ToObvHub(
        this IQuoteProvider<IQuote> quoteProvider)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider);
    }

    /// <summary>
    /// Creates an Obv hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>An instance of <see cref="ObvHub"/>.</returns>
    public static ObvHub ToObvHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToObvHub();
    }

}
