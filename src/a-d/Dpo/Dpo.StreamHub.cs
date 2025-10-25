namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a Detrended Price Oscillator (DPO) stream hub.
/// Note: DPO requires lookahead, so results are delayed by offset periods.
/// Input and output are not indexed 1:1 - output lags by offset.
/// </summary>
public class DpoHub
    : ChainProvider<IReusable, DpoResult>, IDpo
{
    private readonly string hubName;
    private readonly int offset;
    private readonly SmaList smaList;

    /// <summary>
    /// Initializes a new instance of the <see cref="DpoHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal DpoHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Dpo.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        offset = (lookbackPeriods / 2) + 1;
        hubName = $"DPO({lookbackPeriods})";
        smaList = new SmaList(lookbackPeriods);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <summary>
    /// Override OnAdd to handle DPO's lookahead requirement with delayed emission.
    /// For each new input at position i:
    /// 1. Add an empty placeholder result for position i
    /// 2. Update the result at position i-offset with calculated values (if enough future data exists)
    /// This maintains 1:1 input/output correspondence with the last offset positions remaining empty.
    /// </summary>
    /// <inheritdoc />
    public override void OnAdd(IReusable item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        // Add to SMA calculation
        smaList.Add(item);

        // Get current position
        int i = indexHint ?? ProviderCache.Count - 1;

        // Add empty placeholder for current position
        Cache.Add(new DpoResult(item.Timestamp, null, null));

        // Calculate which earlier position we can now update with actual values
        int targetIndex = i - offset;

        // We can only update targetIndex if we have data at targetIndex + offset (which is position i)
        // This ensures the last offset positions never get values
        if (targetIndex >= 0 && (targetIndex + offset) < ProviderCache.Count)
        {
            // Get current SMA (the "future" SMA for the target position)
            SmaResult currentSma = smaList[^1];

            // Get the value from the target (earlier) position
            IReusable targetItem = ProviderCache[targetIndex];

            // Check if we have valid input value (not NaN from chained indicators)
            double targetValue = targetItem.Value;
            bool hasValidInput = !double.IsNaN(targetValue);

            // Check if SMA is valid (not NaN from propagated null values in chains)
            bool hasValidSma = currentSma.Sma.HasValue && !double.IsNaN(currentSma.Sma.Value);

            double? dpoSma = hasValidSma ? currentSma.Sma : null;
            double? dpoVal = (hasValidSma && hasValidInput)
                ? targetValue - currentSma.Sma!.Value
                : null;

            // Update the placeholder at targetIndex with actual values
            Cache[targetIndex] = new DpoResult(targetItem.Timestamp, dpoVal, dpoSma);
        }
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // When rolling back, we need to clear the SMA state and rebuild it
        int affectedIndex = ProviderCache.IndexGte(timestamp);
        if (affectedIndex < 0)
        {
            base.RollbackState(timestamp);
            return;
        }

        // Due to lookahead, the last offset positions must always be empty.
        // Clear from the EARLIER of: affected index OR first trailing empty position
        int firstTrailingEmpty = ProviderCache.Count - offset;
        int clearFromIndex = Math.Min(affectedIndex, firstTrailingEmpty);

        // Clear SMA list and rebuild up to affectedIndex (for correct SMA calculations during rebuild)
        smaList.Clear();
        for (int i = 0; i < affectedIndex && i < ProviderCache.Count; i++)
        {
            smaList.Add(ProviderCache[i]);
        }

        // Clear Cache from clearFromIndex onwards
        while (Cache.Count > clearFromIndex)
        {
            Cache.RemoveAt(Cache.Count - 1);
        }

        // If clearFromIndex < affectedIndex, add placeholders for the gap
        // These positions are now in the "last offset empty" range
        for (int i = clearFromIndex; i < affectedIndex && i < ProviderCache.Count; i++)
        {
            Cache.Add(new DpoResult(ProviderCache[i].Timestamp, null, null));
        }
    }

    /// <inheritdoc />
    protected override (DpoResult result, int index) ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        // For DPO with delayed emission, Insert operations should trigger rebuild rather than
        // try to insert at a specific position, since the output index doesn't match input index.
        // Returning -1 signals the base class to treat this as Add + Rebuild.

        return (new DpoResult(item.Timestamp, null, null), -1);
    }
}/// <summary>
/// Provides methods for creating DPO hubs.
/// </summary>
public static partial class Dpo
{
    /// <summary>
    /// Creates a DPO streaming hub from a chain provider.
    /// Note: DPO results are delayed by offset periods due to lookahead requirements.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A DPO hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static DpoHub ToDpoHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates a DPO hub from a collection of quotes.
    /// Note: DPO results are delayed by offset periods due to lookahead requirements.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An instance of <see cref="DpoHub"/>.</returns>
    public static DpoHub ToDpoHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToDpoHub(lookbackPeriods);
    }
}
