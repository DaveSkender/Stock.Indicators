namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for the Money Flow Index (MFI) indicator.
/// </summary>
public class MfiHub : ChainProvider<IQuote, MfiResult>, IMfi
{
    private readonly string hubName;
    private readonly Queue<(double TruePrice, double MoneyFlow, int Direction)> _buffer;
    private double? _prevTruePrice;

    /// <summary>
    /// Initializes a new instance of the <see cref="MfiHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    internal MfiHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Mfi.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        hubName = $"MFI({lookbackPeriods})";
        _buffer = new Queue<(double, double, int)>(lookbackPeriods);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (MfiResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate true price
        double truePrice = ((double)item.High + (double)item.Low + (double)item.Close) / 3;

        // Calculate raw money flow
        double moneyFlow = truePrice * (double)item.Volume;

        // Determine direction
        int direction = _prevTruePrice == null || truePrice == _prevTruePrice
            ? 0
            : truePrice > _prevTruePrice ? 1 : -1;

        // If buffer is full, remove oldest item
        if (_buffer.Count == LookbackPeriods)
        {
            _buffer.Dequeue();
        }

        // Add new item to buffer
        _buffer.Enqueue((truePrice, moneyFlow, direction));

        // Update previous true price
        _prevTruePrice = truePrice;

        // Calculate MFI when we have enough data
        double? mfi = null;
        if (i >= LookbackPeriods)
        {
            // Recalculate sums from buffer to avoid floating point accumulation errors
            double sumPosMFs = 0;
            double sumNegMFs = 0;

            foreach ((double _, double mf, int dir) in _buffer)
            {
                if (dir == 1)
                {
                    sumPosMFs += mf;
                }
                else if (dir == -1)
                {
                    sumNegMFs += mf;
                }
            }

            if (sumNegMFs != 0)
            {
                double mfRatio = sumPosMFs / sumNegMFs;
                mfi = 100 - (100 / (1 + mfRatio));
            }
            else
            {
                // Handle no negative case
                mfi = 100;
            }
        }

        MfiResult r = new(
            Timestamp: item.Timestamp,
            Mfi: mfi);

        return (r, i);
    }

    /// <summary>
    /// Restores the buffer state up to the specified timestamp.
    /// Clears and rebuilds buffer from ProviderCache for Insert/Remove operations.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear state
        _buffer.Clear();
        _prevTruePrice = null;

        // Find target index in ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index == -1)
        {
            index = ProviderCache.Count;
        }
        if (index <= 0)
        {
            return;
        }

        // Rebuild up to the index before the rollback timestamp
        int targetIndex = index - 1;

        // Rebuild buffer from ProviderCache
        // We need to start from targetIndex - LookbackPeriods to rebuild the buffer
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

        for (int p = startIdx; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];

            // Calculate true price
            double truePrice = ((double)quote.High + (double)quote.Low + (double)quote.Close) / 3;

            // Calculate raw money flow
            double moneyFlow = truePrice * (double)quote.Volume;

            // Determine direction
            int direction = _prevTruePrice == null || truePrice == _prevTruePrice
                ? 0
                : truePrice > _prevTruePrice ? 1 : -1;

            // Add to buffer (no need to dequeue since we're rebuilding)
            _buffer.Enqueue((truePrice, moneyFlow, direction));

            // Update previous true price
            _prevTruePrice = truePrice;
        }
    }
}


public static partial class Mfi
{
    /// <summary>
    /// Converts the quote provider to an MFI hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods. Default is 14.</param>
    /// <returns>An MFI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static MfiHub ToMfiHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 14)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, lookbackPeriods);
    }

    /// <summary>
    /// Creates an Mfi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">The number of lookback periods. Default is 14.</param>
    /// <returns>An instance of <see cref="MfiHub"/>.</returns>
    public static MfiHub ToMfiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToMfiHub(lookbackPeriods);
    }
}
