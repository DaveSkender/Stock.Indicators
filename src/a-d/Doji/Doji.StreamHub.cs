namespace Skender.Stock.Indicators;

// DOJI CANDLESTICK PATTERN (STREAM HUB)

/// <summary>
/// Provides methods for identifying Doji candlestick patterns.
/// </summary>
public class DojiHub<TIn>
    : StreamHub<TIn, CandleResult>, IDoji
    where TIn : IQuote
{
    private readonly string hubName;
    private readonly double _maxPriceChangePercentDecimal;

    /// <summary>
    /// Initializes a new instance of the <see cref="DojiHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="maxPriceChangePercent">Maximum absolute percent difference in open and close price.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the maxPriceChangePercent is invalid.</exception>
    internal DojiHub(
        IStreamObservable<TIn> provider,
        double maxPriceChangePercent) : base(provider)
    {
        Doji.Validate(maxPriceChangePercent);
        MaxPriceChangePercent = maxPriceChangePercent;
        _maxPriceChangePercentDecimal = maxPriceChangePercent / 100;
        hubName = $"DOJI({maxPriceChangePercent:F1})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public double MaxPriceChangePercent { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (CandleResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        decimal? matchPrice = null;
        Match matchType = Match.None;

        // Check for Doji pattern
        if (item.Open != 0
            && Math.Abs((double)(item.Close / item.Open) - 1d) <= _maxPriceChangePercentDecimal)
        {
            matchPrice = item.Close;
            matchType = Match.Neutral;
        }

        // Candidate result
        CandleResult r = new(
            timestamp: item.Timestamp,
            quote: item,
            match: matchType,
            price: matchPrice);

        return (r, i);
    }
}


public static partial class Doji
{
    /// <summary>
    /// Creates a Doji hub from a quote provider.
    /// </summary>
    /// <typeparam name="T">The type of the quote data.</typeparam>
    /// <param name="provider">The quote provider.</param>
    /// <param name="maxPriceChangePercent">Maximum absolute percent difference in open and close price. Default is 0.1.</param>
    /// <returns>A Doji hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the maxPriceChangePercent is invalid.</exception>
    public static DojiHub<T> ToDojiHub<T>(
        this IStreamObservable<T> provider,
        double maxPriceChangePercent = 0.1)
        where T : IQuote
        => new(provider, maxPriceChangePercent);

    /// <summary>
    /// Creates a Doji hub from a collection of quotes.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="maxPriceChangePercent">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="DojiHub{TQuote}"/>.</returns>
    public static DojiHub<TQuote> ToDojiHub<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        double maxPriceChangePercent = 0.1)
        where TQuote : IQuote
    {
        QuoteHub<TQuote> quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDojiHub(maxPriceChangePercent);
    }

}
