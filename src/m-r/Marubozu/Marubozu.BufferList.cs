namespace Skender.Stock.Indicators;

/// <summary>
/// Marubozu candlestick pattern from incremental quotes.
/// </summary>
public class MarubozuList : BufferList<CandleResult>, IIncrementFromQuote, IMarubozu
{
    private readonly double minBodyPercent;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarubozuList"/> class.
    /// </summary>
    /// <param name="minBodyPercent">The minimum body percentage to qualify as a Marubozu. Default is 95.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="minBodyPercent"/> is invalid.</exception>
    public MarubozuList(double minBodyPercent = 95)
    {
        Marubozu.Validate(minBodyPercent);
        MinBodyPercent = minBodyPercent;
        this.minBodyPercent = minBodyPercent / 100;

        Name = $"MARUBOZU({minBodyPercent})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MarubozuList"/> class with initial quotes.
    /// </summary>
    /// <param name="minBodyPercent">The minimum body percentage to qualify as a Marubozu.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public MarubozuList(double minBodyPercent, IReadOnlyList<IQuote> quotes)
        : this(minBodyPercent) => Add(quotes);

    /// <inheritdoc />
    public double MinBodyPercent { get; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;
        CandleProperties candle = quote.ToCandle();

        decimal? matchPrice = null;
        Match matchType = Match.None;

        // check for current signal
        if (candle.BodyPct >= minBodyPercent)
        {
            matchPrice = quote.Close;
            matchType = candle.IsBullish ? Match.BullSignal : Match.BearSignal;
        }

        CandleResult result = new(
            timestamp: timestamp,
            candle: candle,
            match: matchType,
            price: matchPrice);

        AddInternal(result);
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

    /// <inheritdoc />
    public override void Clear() => base.Clear();
}

public static partial class Marubozu
{
    /// <summary>
    /// Creates a buffer list for Marubozu candlestick pattern calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="minBodyPercent">Minimum body percent threshold</param>
    public static MarubozuList ToMarubozuList(
        this IReadOnlyList<IQuote> quotes,
        double minBodyPercent = 95)
        => new(minBodyPercent) { quotes };
}
