namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a stream hub for calculating the Chandelier Exit.
/// </summary>
public class ChandelierHub
    : StreamHub<IQuote, ChandelierResult>, IChandelier
{
    private readonly string hubName;
    private readonly AtrHub atrHub;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChandelierHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short).</param>
    internal ChandelierHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods,
        double multiplier,
        Direction type) : base(provider)
    {
        Chandelier.Validate(lookbackPeriods, multiplier);

        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;
        Type = type;

        string typeName = type.ToString().ToUpperInvariant();
        hubName = FormattableString.Invariant(
            $"CHEXIT({lookbackPeriods},{multiplier},{typeName})");

        // Initialize internal ATR hub to maintain streaming state
        atrHub = provider.ToAtrHub(lookbackPeriods);

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of periods to use for the lookback window.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the ATR multiplier.
    /// </summary>
    public double Multiplier { get; init; }

    /// <summary>
    /// Gets the direction type (Long or Short).
    /// </summary>
    public Direction Type { get; init; }

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (ChandelierResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // handle warmup periods
        if (i < LookbackPeriods)
        {
            return (new ChandelierResult(item.Timestamp, null), i);
        }

        // use cached ATR result from internal hub (O(1) lookup)
        // System invariant: atrHub.Results[i] must exist because atrHub subscribes
        // to the same provider and processes updates synchronously before this hub.
        // This bounds check defends against edge cases during initialization/rebuild.
        if (i >= atrHub.Results.Count)
        {
            throw new InvalidOperationException(
                $"ATR hub synchronization error: expected ATR result at index {i}, "
                + $"but atrHub.Results.Count is {atrHub.Results.Count}. "
                + "This indicates a state synchronization issue between chained hubs.");
        }

        double? atr = atrHub.Results[i].Atr;

        if (atr is null)
        {
            return (new ChandelierResult(item.Timestamp, null), i);
        }

        // find max high or min low in lookback period
        double? exit;

        switch (Type)
        {
            case Direction.Long:
                double maxHigh = double.MinValue;
                for (int p = i + 1 - LookbackPeriods; p <= i; p++)
                {
                    double high = (double)ProviderCache[p].High;
                    if (high > maxHigh)
                    {
                        maxHigh = high;
                    }
                }

                exit = maxHigh - (atr.Value * Multiplier);
                break;

            case Direction.Short:
                double minLow = double.MaxValue;
                for (int p = i + 1 - LookbackPeriods; p <= i; p++)
                {
                    double low = (double)ProviderCache[p].Low;
                    if (low < minLow)
                    {
                        minLow = low;
                    }
                }

                exit = minLow + (atr.Value * Multiplier);
                break;

            default:
                throw new InvalidOperationException($"Unknown direction type: {Type}");
        }

        ChandelierResult r = new(
            Timestamp: item.Timestamp,
            ChandelierExit: exit);

        return (r, i);
    }
}

/// <summary>
/// Provides methods for calculating the Chandelier Exit using a stream hub.
/// </summary>
public static partial class Chandelier
{
    /// <summary>
    /// Creates a Chandelier Exit streaming hub from a quotes provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window. Default is 22.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR. Default is 3.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short). Default is Long.</param>
    /// <returns>An instance of <see cref="ChandelierHub"/>.</returns>
    public static ChandelierHub ToChandelierHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 22,
        double multiplier = 3,
        Direction type = Direction.Long)
             => new(quoteProvider, lookbackPeriods, multiplier, type);

    /// <summary>
    /// Creates a Chandelier Exit hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window. Default is 22.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR. Default is 3.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short). Default is Long.</param>
    /// <returns>An instance of <see cref="ChandelierHub"/>.</returns>
    public static ChandelierHub ToChandelierHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 22,
        double multiplier = 3,
        Direction type = Direction.Long)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToChandelierHub(lookbackPeriods, multiplier, type);
    }
}
