namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for generating Renko chart series using ATR (Average True Range) in a streaming manner.
/// </summary>
/// <remarks>
/// <para>
/// The streaming implementation differs from the series implementation:
/// </para>
/// <list type="bullet">
/// <item>Series: Calculates ATR from ALL quotes and uses the final ATR value for brick sizing</item>
/// <item>Stream: Locks in ATR value after initial period (atrPeriods) and uses it for all subsequent bricks</item>
/// </list>
/// <para>
/// This means streaming results may differ from series results because the brick size
/// is determined earlier in the data stream. This is intentional and necessary for 
/// real-time streaming scenarios where future data is not available.
/// </para>
/// </remarks>
public class RenkoAtrHub
    : QuoteProvider<IQuote, RenkoResult>
{
    #region constructors

    private readonly string hubName;
    private RenkoHub? renkoHub;
    private QuoteHub? internalQuoteHub;
    private decimal? fixedBrickSize;  // lock in brick size once calculated

    /// <summary>
    /// Initializes a new instance of the <see cref="RenkoAtrHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="atrPeriods">The number of periods for calculating ATR.</param>
    /// <param name="endType">The type of price to use for the end of the brick.</param>
    internal RenkoAtrHub(
        IQuoteProvider<IQuote> provider,
        int atrPeriods,
        EndType endType) : base(provider)
    {
        Atr.Validate(atrPeriods);
        AtrPeriods = atrPeriods;
        EndType = endType;
        hubName = $"RENKO-ATR({atrPeriods},{endType.ToString().ToUpperInvariant()})";

        Reinitialize();
    }
    #endregion constructors

    /// <summary>
    /// RenkoAtr hub settings. Since it can produce 0 or many bricks per quote,
    /// the default 1:1 in/out is not used and must be skipped to prevent
    /// same-date triggered rebuilds when caching.
    /// </summary>
    public override BinarySettings Properties { get; init; } = new(0b00000010);  // custom

    /// <summary>
    /// Gets the number of periods for calculating ATR.
    /// </summary>
    public int AtrPeriods { get; }

    /// <summary>
    /// Gets the price candle end type used to determine when threshold
    /// is met to generate new bricks.
    /// </summary>
    public EndType EndType { get; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    public override void OnAdd(IQuote item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        ToIndicator(item, notify, indexHint);
    }

    /// <inheritdoc />
    protected override (RenkoResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
        => throw new InvalidOperationException(); // not used

    /// <summary>
    /// Restores the RenkoAtr state to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // clean up existing renko hub
        if (renkoHub is not null)
        {
            renkoHub.Unsubscribe();
            renkoHub = null;
        }

        // clean up internal quote hub
        internalQuoteHub = null;

        // reset brick size to force recalculation
        fixedBrickSize = null;
    }

    /// <summary>
    /// custom: build 0 to many bricks per quote using ATR-based brick size.
    /// Uses last available ATR value as brick size, consistent with Series implementation.
    /// </summary>
    /// <param name="item">Item to process</param>
    /// <param name="notify">Whether to notify observers</param>
    /// <param name="indexHint">Optional index hint for performance</param>
    /// <exception cref="InvalidOperationException">Thrown when the operation is invalid for the current context</exception>
    private void ToIndicator(IQuote item, bool notify, int? indexHint)
    {
        int providerIndex = indexHint
            ?? throw new InvalidOperationException($"{nameof(indexHint)} cannot be empty");

        // nothing to do until we have enough data for ATR
        if (providerIndex < AtrPeriods)
        {
            return;
        }

        // calculate and lock in brick size on first opportunity
        if (fixedBrickSize is null)
        {
            // calculate ATR for current data
            List<IQuote> quotesList = ProviderCache
                .Take(providerIndex + 1)
                .ToList();

            IReadOnlyList<AtrResult> atrResults = quotesList
                .ToQuoteDList()
                .CalcAtr(AtrPeriods);

            // get last ATR value for brick size (matching Series behavior)
            AtrResult? lastAtr = atrResults[^1];
            decimal brickSize = (decimal?)lastAtr?.Atr ?? 0;

            // nothing to do if no brick size available
            if (brickSize == 0)
            {
                return;
            }

            // lock in this brick size for all future calculations
            fixedBrickSize = brickSize;
        }

        // initialize or reinitialize renko hub with fixed brick size
        if (renkoHub is null && fixedBrickSize is not null)
        {
            // create new quote provider with all current data
            internalQuoteHub = new QuoteHub();
            for (int i = 0; i <= providerIndex; i++)
            {
                internalQuoteHub.Add(ProviderCache[i]);
            }

            // create Renko hub with fixed ATR-based brick size
            renkoHub = internalQuoteHub.ToRenkoHub(fixedBrickSize.Value, EndType);

            // clear and rebuild cache with all bricks
            Cache.Clear();
            foreach (RenkoResult brick in renkoHub.Results)
            {
                AppendCache(brick, notify);
            }
        }
        // hub already initialized, just process the new quote
        else if (internalQuoteHub is not null && renkoHub is not null)
        {
            int beforeCount = renkoHub.Results.Count;

            // add the new quote to the internal hub
            internalQuoteHub.Add(item);

            // copy any new bricks that were generated
            for (int i = beforeCount; i < renkoHub.Results.Count; i++)
            {
                AppendCache(renkoHub.Results[i], notify);
            }
        }
    }
}


public static partial class RenkoAtr
{
    /// <summary>
    /// Converts a quote provider to a RenkoAtr hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="atrPeriods">The number of periods for calculating ATR.</param>
    /// <param name="endType">The price candle end type to use as the brick threshold.</param>
    /// <returns>A RenkoAtr hub.</returns>
    public static RenkoAtrHub ToRenkoAtrHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int atrPeriods = 14,
        EndType endType = EndType.Close)
        => new(quoteProvider, atrPeriods, endType);

    /// <summary>
    /// Creates a RenkoAtr hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="atrPeriods">The number of periods for calculating ATR.</param>
    /// <param name="endType">The price candle end type to use as the brick threshold.</param>
    /// <returns>An instance of <see cref="RenkoAtrHub"/>.</returns>
    public static RenkoAtrHub ToRenkoAtrHub(
        this IReadOnlyList<IQuote> quotes,
        int atrPeriods = 14,
        EndType endType = EndType.Close)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToRenkoAtrHub(atrPeriods, endType);
    }
}
