namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a hub for Marubozu candlestick pattern detection.
/// </summary>
public class MarubozuHub
    : StreamHub<IQuote, CandleResult>, IMarubozu
{
    private readonly double _minBodyPercentDecimal;

    internal MarubozuHub(
        IStreamObservable<IQuote> provider,
        double minBodyPercent) : base(provider)
    {
        Marubozu.Validate(minBodyPercent);
        MinBodyPercent = minBodyPercent;
        _minBodyPercentDecimal = minBodyPercent / 100;
        Name = $"MARUBOZU({minBodyPercent:F1})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public double MinBodyPercent { get; init; }
    /// <inheritdoc/>
    protected override (CandleResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
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

public static partial class Marubozu
{
    /// <summary>
    /// Creates a Marubozu hub from a quote provider.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="minBodyPercent">The minimum body percentage to qualify as a Marubozu. Default is 95.</param>
    /// <returns>A Marubozu hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the minBodyPercent is invalid.</exception>
    public static MarubozuHub ToMarubozuHub(
        this IStreamObservable<IQuote> provider,
        double minBodyPercent = 95)
        => new(provider, minBodyPercent);

    /// <summary>
    /// Creates a Marubozu hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="minBodyPercent">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="MarubozuHub"/>.</returns>
    public static MarubozuHub ToMarubozuHub(
        this IReadOnlyList<IQuote> quotes,
        double minBodyPercent = 95)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToMarubozuHub(minBodyPercent);
    }
}
