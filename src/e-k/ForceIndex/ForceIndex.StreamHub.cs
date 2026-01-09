namespace Skender.Stock.Indicators;

/// <summary>
/// Provides streaming hub for Force Index calculations.
/// </summary>
public class ForceIndexHub
    : ChainHub<IReusable, ForceIndexResult>, IForceIndex
{
    private readonly double _k;
    private double _sumRawFi;

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

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public override string ToString() => Name;

    /// <inheritdoc />
    protected override (ForceIndexResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double fi = double.NaN;

        // skip first period (need prior quote for delta)
        if (i > 0)
        {
            // get current and previous quotes
            IQuote currentQuote = (IQuote)ProviderCache[i];
            IQuote previousQuote = (IQuote)ProviderCache[i - 1];

            // calculate raw Force Index
            double rawFi = (double)currentQuote.Volume
                * ((double)currentQuote.Close - (double)previousQuote.Close);

            if (i >= LookbackPeriods)
            {
                // Check if previous result has a valid ForceIndex for incremental update
                if (Cache[i - 1].ForceIndex is not null)
                {
                    // Incremental O(1) EMA update
                    fi = Ema.Increment(_k, Cache[i - 1].Value, rawFi);
                }
                else
                {
                    // First EMA value - use accumulated sum for O(1) SMA calculation
                    _sumRawFi += rawFi;
                    fi = _sumRawFi / LookbackPeriods;
                }
            }
            else
            {
                // Warmup period - accumulate raw Force Index values
                _sumRawFi += rawFi;
            }
        }

        ForceIndexResult result = new(
            Timestamp: item.Timestamp,
            ForceIndex: fi.NaN2Null());

        return (result, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset sum - will be recalculated during rebuild
        _sumRawFi = 0;

        // Rebuild the rolling sum from cache
        int lastIndex = Cache.Count - 1;
        if (lastIndex < 0)
        {
            return;
        }

        // Determine how many values to sum based on where we are
        int endWarmup = Math.Min(LookbackPeriods - 1, lastIndex);

        for (int i = 1; i <= endWarmup; i++)
        {
            IQuote curr = (IQuote)ProviderCache[i];
            IQuote prev = (IQuote)ProviderCache[i - 1];
            _sumRawFi += (double)curr.Volume * ((double)curr.Close - (double)prev.Close);
        }
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
}
