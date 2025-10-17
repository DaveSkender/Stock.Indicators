namespace Skender.Stock.Indicators;

// MARUBOZU CANDLESTICK PATTERN (STREAM HUB)

/// <summary>
/// Provides methods for identifying Marubozu candlestick patterns.
/// </summary>
public static partial class Marubozu
{
    /// <summary>
    /// Creates a Marubozu hub from a quote provider.
    /// </summary>
    /// <typeparam name="T">The type of the quote data.</typeparam>
    /// <param name="provider">The quote provider.</param>
    /// <param name="minBodyPercent">The minimum body percentage to qualify as a Marubozu. Default is 95.</param>
    /// <returns>A Marubozu hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the minBodyPercent is invalid.</exception>
    public static MarubozuHub<T> ToMarubozuHub<T>(
        this IStreamObservable<T> provider,
        double minBodyPercent = 95)
        where T : IQuote
        => new(provider, minBodyPercent);
}

/// <summary>
/// Represents a hub for Marubozu candlestick pattern detection.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class MarubozuHub<TIn>
    : StreamHub<TIn, CandleResult>, IMarubozu
    where TIn : IQuote
{
    private readonly string hubName;
    private readonly double _minBodyPercentDecimal;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarubozuHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="minBodyPercent">The minimum body percentage to qualify as a Marubozu.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the minBodyPercent is invalid.</exception>
    internal MarubozuHub(
        IStreamObservable<TIn> provider,
        double minBodyPercent) : base(provider)
    {
        Marubozu.Validate(minBodyPercent);
        MinBodyPercent = minBodyPercent;
        _minBodyPercentDecimal = minBodyPercent / 100;
        hubName = $"MARUBOZU({minBodyPercent:F1})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public double MinBodyPercent { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (CandleResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        CandleProperties candle = item.ToCandle();
        decimal? matchPrice = null;
        Match matchType = Match.None;

        // check for current signal
        if (candle.BodyPct >= _minBodyPercentDecimal)
        {
            matchPrice = item.Close;
            matchType = candle.IsBullish ? Match.BullSignal : Match.BearSignal;
        }

        // Candidate result
        CandleResult r = new(
            timestamp: item.Timestamp,
            candle: candle,
            match: matchType,
            price: matchPrice);

        return (r, i);
    }
}
