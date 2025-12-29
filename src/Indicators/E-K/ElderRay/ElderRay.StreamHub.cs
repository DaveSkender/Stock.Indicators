namespace Skender.Stock.Indicators;

// ELDER RAY (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Elder Ray indicator using a stream hub.
/// </summary>
public class ElderRayHub
    : StreamHub<IQuote, ElderRayResult>, IElderRay
{

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElderRayHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal ElderRayHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        ElderRay.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        hubName = $"ELDER-RAY({lookbackPeriods})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the EMA smoothing constant.
    /// </summary>
    private double K { get; init; }

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (ElderRayResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double ema = i >= LookbackPeriods - 1
            ? i > 0 && Cache[i - 1].Ema is not null

                // normal EMA calculation
                ? Ema.Increment(K, Cache[i - 1].Ema!.Value, (double)item.Close)

                // re/initialize as SMA
                : Sma.Increment(ProviderCache, LookbackPeriods, i)

            // warmup periods are never calculable
            : double.NaN;

        // calculate Elder Ray values
        ElderRayResult r = new(
            Timestamp: item.Timestamp,
            Ema: ema.NaN2Null(),
            BullPower: ema.NaN2Null() is not null ? (double)item.High - ema : null,
            BearPower: ema.NaN2Null() is not null ? (double)item.Low - ema : null);

        return (r, i);
    }
}

public static partial class ElderRay
{
    /// <summary>
    /// Creates an Elder Ray streaming hub from a quote provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation. Default is 13.</param>
    /// <returns>An Elder Ray hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static ElderRayHub ToElderRayHub(
       this IQuoteProvider<IQuote> quoteProvider,
       int lookbackPeriods = 13)
           => new(quoteProvider, lookbackPeriods);

    /// <summary>
    /// Creates an Elder Ray hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation. Default is 13.</param>
    /// <returns>An instance of <see cref="ElderRayHub"/>.</returns>
    public static ElderRayHub ToElderRayHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 13)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToElderRayHub(lookbackPeriods);
    }
}
