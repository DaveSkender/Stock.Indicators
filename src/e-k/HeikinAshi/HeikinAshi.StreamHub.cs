namespace Skender.Stock.Indicators;

// HEIKIN-ASHI (STREAM HUB)

/// <summary>
/// Represents a hub for Heikin-Ashi quote transformation.
/// </summary>
public class HeikinAshiHub
    : QuoteProvider<IQuote, HeikinAshiResult>
{
    private readonly string hubName;
    private decimal _prevOpen = decimal.MinValue;
    private decimal _prevClose = decimal.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeikinAshiHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    internal HeikinAshiHub(
        IQuoteProvider<IQuote> provider) : base(provider)
    {
        hubName = "HEIKINASHI";
        Reinitialize();
    }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (HeikinAshiResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        decimal prevOpen;
        decimal prevClose;

        // Get previous values from cache or initialize from first quote
        if (i == 0)
        {
            prevOpen = item.Open;
            prevClose = item.Close;
        }
        else if (i > 0 && Cache.Count > i - 1)
        {
            // Use previous HeikinAshi result from cache
            HeikinAshiResult prev = Cache[i - 1];
            prevOpen = prev.Open;
            prevClose = prev.Close;
        }
        else
        {
            // Fallback to stored state (on mutation rebuilds)
            prevOpen = _prevOpen;
            prevClose = _prevClose;

            // If state is uninitialized, seed from first quote or current item
            if (prevOpen == decimal.MinValue || prevClose == decimal.MinValue)
            {
                if (ProviderCache.Count > 0)
                {
                    IQuote q0 = ProviderCache[0];
                    prevOpen = q0.Open;
                    prevClose = q0.Close;
                }
                else
                {
                    prevOpen = item.Open;
                    prevClose = item.Close;
                }
            }
        }

        // close
        decimal close = (item.Open + item.High + item.Low + item.Close) / 4;

        // open
        decimal open = (prevOpen + prevClose) / 2;

        // high
        decimal high = Math.Max(item.High, Math.Max(open, close));

        // low
        decimal low = Math.Min(item.Low, Math.Min(open, close));

        // Candidate result
        HeikinAshiResult r = new(
            Timestamp: item.Timestamp,
            Open: open,
            High: high,
            Low: low,
            Close: close,
            Volume: item.Volume);

        // save for next iteration (used when building sequentially)
        _prevOpen = open;
        _prevClose = close;

        return (r, i);
    }

    /// <summary>
    /// Restores the state to the last result before or at the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // restore previous open/close markers
        if (Cache.Count != 0)
        {
            bool found = false;
            HeikinAshiResult lastResult = default!;
            for (int j = Cache.Count - 1; j >= 0; j--)
            {
                HeikinAshiResult c = Cache[j];
                if (c.Timestamp <= timestamp)
                {
                    lastResult = c;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                _prevOpen = lastResult.Open;
                _prevClose = lastResult.Close;
                return;
            }
            // else: fall through to seed from first quote/defaults below
        }

        // reset to first quote if no cache
        if (ProviderCache.Count > 0)
        {
            IQuote q0 = ProviderCache[0];
            _prevOpen = q0.Open;
            _prevClose = q0.Close;
        }
        else
        {
            _prevOpen = decimal.MinValue;
            _prevClose = decimal.MinValue;
        }
    }
}

public static partial class HeikinAshi
{
    /// <summary>
    /// Creates a Heikin-Ashi hub from a quote provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <returns>A Heikin-Ashi hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    public static HeikinAshiHub ToHeikinAshiHub(
        this IQuoteProvider<IQuote> quoteProvider)
        => new(quoteProvider);

    /// <summary>
    /// Creates a Heikin-Ashi hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <returns>An instance of <see cref="HeikinAshiHub"/>.</returns>
    public static HeikinAshiHub ToHeikinAshiHub(
        this IReadOnlyList<IQuote> quotes)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToHeikinAshiHub();
    }
}
