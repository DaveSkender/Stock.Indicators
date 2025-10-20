namespace Skender.Stock.Indicators;


/// <summary>
/// Streaming hub for Chande Momentum Oscillator (CMO) calculations.
/// </summary>
public class CmoHub
    : ChainProvider<IReusable, CmoResult>, ICmo
{
    private readonly string hubName;
    private readonly Queue<(bool? isUp, double value)> _tickBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CmoHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal CmoHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Cmo.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"CMO({lookbackPeriods})";
        _tickBuffer = new Queue<(bool? isUp, double value)>(lookbackPeriods);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (CmoResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? cmo = null;

        if (i >= LookbackPeriods)
        {
            // Check if we can use incremental update (sequential processing)
            // We can do this if:
            // 1. Cache has the previous result (i-1)
            // 2. Buffer has the right number of items
            // 3. Previous result had a value (wasn't recalculated)
            bool canIncrement = Cache.Count > i
                && _tickBuffer.Count == LookbackPeriods
                && Cache[i - 1].Cmo.HasValue;

            if (canIncrement)
            {
                // Sequential processing - incremental O(1) update
                double prevValue = ProviderCache[i - 1].Value;
                double currValue = item.Value;

                double tickValue = Math.Abs(currValue - prevValue);
                bool? isUp = double.IsNaN(tickValue) || currValue == prevValue
                    ? null
                    : currValue > prevValue;

                // Update buffer using universal buffer utilities
                _tickBuffer.Update(LookbackPeriods, (isUp, tickValue));
            }
            else
            {
                // Rebuild buffer from ProviderCache (O(lookbackPeriods))
                _tickBuffer.Clear();

                // Build tick buffer from provider cache
                // Start from i - LookbackPeriods + 1 to get exactly LookbackPeriods ticks
                int startPosition = i - LookbackPeriods + 1;
                for (int k = startPosition; k <= i; k++)
                {
                    double prevValue = ProviderCache[k - 1].Value;
                    double currValue = ProviderCache[k].Value;

                    double tickValue = Math.Abs(currValue - prevValue);
                    bool? isUp = double.IsNaN(tickValue) || currValue == prevValue
                        ? null
                        : currValue > prevValue;

                    _tickBuffer.Enqueue((isUp, tickValue));
                }
            }

            // Calculate CMO
            cmo = Cmo.PeriodCalculation(_tickBuffer);
        }
        else
        {
            // Not enough data yet - clear buffer
            _tickBuffer.Clear();
        }

        // candidate result
        CmoResult r = new(
            Timestamp: item.Timestamp,
            Cmo: cmo);

        return (r, i);
    }

}


public static partial class Cmo
{
    /// <summary>
    /// Creates a CMO streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A CMO hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static CmoHub ToCmoHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 14)
        => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a Cmo hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An instance of <see cref="CmoHub"/>.</returns>
    public static CmoHub ToCmoHub(
        this IReadOnlyList<IQuote> quotes, int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToCmoHub(lookbackPeriods);
    }
}

