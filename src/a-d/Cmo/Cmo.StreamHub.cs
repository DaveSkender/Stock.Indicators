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

        // Build up the tick buffer (starting from index 1, since we need previous value)
        if (i > 0)
        {
            double prevValue = ProviderCache[i - 1].Value;
            double currValue = item.Value;

            double tickValue = Math.Abs(currValue - prevValue);
            bool? isUp = double.IsNaN(tickValue) || currValue == prevValue
                ? null
                : currValue > prevValue;

            // Update buffer using universal buffer utilities
            _tickBuffer.Update(LookbackPeriods, (isUp, tickValue));

            // Calculate CMO only when we have enough data
            if (i >= LookbackPeriods)
            {
                cmo = Cmo.PeriodCalculation(_tickBuffer);
            }
        }

        // candidate result
        CmoResult r = new(
            Timestamp: item.Timestamp,
            Cmo: cmo);

        return (r, i);
    }

    /// <summary>
    /// Restores the tick buffer state up to the specified timestamp.
    /// Clears and rebuilds _tickBuffer from ProviderCache for Insert/Remove operations.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear tick buffer
        _tickBuffer.Clear();

        // Find target index in ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        // Rebuild up to the index before the rollback timestamp
        int targetIndex = index - 1;

        // Need at least LookbackPeriods items to rebuild buffer
        // Start from targetIndex - LookbackPeriods + 1, but not before index 1 (we need i-1 for prevValue)
        int startIdx = Math.Max(1, targetIndex + 1 - LookbackPeriods);

        // Rebuild tick buffer from ProviderCache
        for (int p = startIdx; p <= targetIndex; p++)
        {
            double prevValue = ProviderCache[p - 1].Value;
            double currValue = ProviderCache[p].Value;

            double tickValue = Math.Abs(currValue - prevValue);
            bool? isUp = double.IsNaN(tickValue) || currValue == prevValue
                ? null
                : currValue > prevValue;

            _tickBuffer.Enqueue((isUp, tickValue));
        }
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

