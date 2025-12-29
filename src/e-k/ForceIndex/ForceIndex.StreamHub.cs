namespace Skender.Stock.Indicators;

// FORCE INDEX (STREAM HUB)

/// <summary>
/// Provides methods for creating Force Index hubs.
/// </summary>
public class ForceIndexHub
    : ChainProvider<IReusable, ForceIndexResult>, IForceIndex
{
    private readonly double _k;
    private double _sumRawFi;
    private double? _prevFi;

    /// <summary>
    /// Initializes a new instance of the <see cref="ForceIndexHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    internal ForceIndexHub(
        IQuoteProvider<IQuote> provider,
        int lookbackPeriods) : base(provider)
    {
        ForceIndex.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _k = 2d / (lookbackPeriods + 1);
        Name = $"FORCE({lookbackPeriods})";

        Reinitialize();
    }

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    // METHODS

    /// <inheritdoc />
    public override string ToString() => Name;

    /// <inheritdoc />
    protected override (ForceIndexResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int index = indexHint ?? ProviderCache.IndexOf(item, true);

        double? fi = null;

        // skip first period
        if (index > 0)
        {
            // get current and previous quotes
            IQuote currentQuote = (IQuote)ProviderCache[index];
            IQuote previousQuote = (IQuote)ProviderCache[index - 1];

            // calculate raw Force Index
            double rawFi = (double)currentQuote.Volume * ((double)currentQuote.Close - (double)previousQuote.Close);

            // Check if we can use incremental update (sequential processing)
            bool canIncrement = Cache.Count > index
                && _prevFi.HasValue
                && Cache[index - 1].ForceIndex.HasValue;

            if (canIncrement && index > LookbackPeriods)
            {
                // Sequential processing - incremental O(1) EMA update
                fi = _prevFi + (_k * (rawFi - _prevFi));
                _prevFi = fi;
            }
            else
            {
                // Rebuild from ProviderCache (after rollback or non-sequential access)
                _sumRawFi = 0;
                _prevFi = null;

                // Recalculate sum and EMA from start
                for (int j = 1; j <= index; j++)
                {
                    IQuote curr = (IQuote)ProviderCache[j];
                    IQuote prev = (IQuote)ProviderCache[j - 1];
                    double jRawFi = (double)curr.Volume * ((double)curr.Close - (double)prev.Close);

                    if (j <= LookbackPeriods)
                    {
                        // Accumulate sum for initialization
                        _sumRawFi += jRawFi;

                        // First EMA value - use SMA of raw FI
                        if (j == LookbackPeriods)
                        {
                            fi = _sumRawFi / LookbackPeriods;
                            _prevFi = fi;
                        }
                    }
                    else
                    {
                        // Apply EMA updates
                        if (_prevFi.HasValue)
                        {
                            fi = _prevFi + (_k * (jRawFi - _prevFi));
                            _prevFi = fi;
                        }
                    }
                }
            }
        }

        ForceIndexResult result = new(
            Timestamp: item.Timestamp,
            ForceIndex: fi);

        return (result, index);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset state - will be recalculated during rebuild
        _sumRawFi = double.NaN;
        _prevFi = null;
    }
}

public static partial class ForceIndex
{
    /// <summary>
    /// Converts the quote provider to a Force Index hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>A Force Index hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static ForceIndexHub ToForceIndexHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 2)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, lookbackPeriods);
    }

    /// <summary>
    /// Creates a Force Index hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="ForceIndexHub"/>.</returns>
    public static ForceIndexHub ToForceIndexHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 2)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToForceIndexHub(lookbackPeriods);
    }

}
