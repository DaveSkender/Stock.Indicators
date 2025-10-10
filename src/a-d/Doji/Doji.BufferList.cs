namespace Skender.Stock.Indicators;

/// <summary>
/// Doji candlestick pattern from incremental quotes.
/// </summary>
public class DojiList : BufferList<CandleResult>, IIncrementFromQuote, IDoji
{
    private readonly double maxPriceChangeFraction;

    /// <summary>
    /// Initializes a new instance of the <see cref="DojiList"/> class.
    /// </summary>
    /// <param name="maxPriceChangePercent">Maximum absolute percent difference in open and close price.</param>
    public DojiList(double maxPriceChangePercent = 0.1)
    {
        Doji.Validate(maxPriceChangePercent);
        MaxPriceChangePercent = maxPriceChangePercent;
        maxPriceChangeFraction = maxPriceChangePercent / 100;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DojiList"/> class with initial quotes.
    /// </summary>
    /// <param name="maxPriceChangePercent">Maximum absolute percent difference in open and close price.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public DojiList(double maxPriceChangePercent, IReadOnlyList<IQuote> quotes)
        : this(maxPriceChangePercent)
        => Add(quotes);

    /// <summary>
    /// Gets the maximum price change percent.
    /// </summary>
    public double MaxPriceChangePercent { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;
        decimal? matchPrice = null;
        Match matchType = Match.None;

        // Check for Doji pattern
        if (quote.Open != 0
            && Math.Abs((double)(quote.Close / quote.Open) - 1d) <= maxPriceChangeFraction)
        {
            matchPrice = quote.Close;
            matchType = Match.Neutral;
        }

        AddInternal(new CandleResult(
            timestamp: timestamp,
            quote: quote,
            match: matchType,
            price: matchPrice));
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i]);
        }
    }

    /// <summary>
    /// Clears the list and resets internal state so the instance can be reused.
    /// </summary>
    public override void Clear()
    {
        base.Clear();
    }
}

public static partial class Doji
{
    /// <summary>
    /// Creates a buffer list for Doji candlestick pattern detection.
    /// </summary>
    public static DojiList ToDojiList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        double maxPriceChangePercent = 0.1)
        where TQuote : IQuote
        => new(maxPriceChangePercent) { (IReadOnlyList<IQuote>)quotes };
}
