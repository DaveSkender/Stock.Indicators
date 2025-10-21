namespace Skender.Stock.Indicators;

// FCB (STREAM HUB)

/// <summary>
/// Provides methods for calculating the Fractal Chaos Bands (FCB) using a stream hub.
/// </summary>
public class FcbHub
    : StreamHub<IQuote, FcbResult>, IFcb
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="FcbHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="windowSpan">The window span for the calculation.</param>
    internal FcbHub(
        IQuoteProvider<IQuote> provider,
        int windowSpan) : base(provider)
    {
        Fcb.Validate(windowSpan);
        WindowSpan = windowSpan;
        hubName = $"FCB({windowSpan})";

        Reinitialize();
    }
    #endregion

    /// <summary>
    /// Gets the window span for the calculation.
    /// </summary>
    public int WindowSpan { get; init; }

    // state tracking for upper and lower bands
    private decimal? UpperLine { get; set; }
    private decimal? LowerLine { get; set; }

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (FcbResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // check if we can identify fractals (need full window)
        if (i >= 2 * WindowSpan)
        {
            // look at the fractal from windowSpan periods ago
            int fractalIndex = i - WindowSpan;
            IQuote fractalQuote = ProviderCache[fractalIndex];

            // check for bearish fractal (high point) at fractalIndex
            bool isBearishFractal = true;
            for (int p = fractalIndex - WindowSpan; p <= fractalIndex + WindowSpan; p++)
            {
                if (p != fractalIndex)
                {
                    if (ProviderCache[p].High >= fractalQuote.High)
                    {
                        isBearishFractal = false;
                        break;
                    }
                }
            }

            // check for bullish fractal (low point) at fractalIndex
            bool isBullishFractal = true;
            for (int p = fractalIndex - WindowSpan; p <= fractalIndex + WindowSpan; p++)
            {
                if (p != fractalIndex)
                {
                    if (ProviderCache[p].Low <= fractalQuote.Low)
                    {
                        isBullishFractal = false;
                        break;
                    }
                }
            }

            // update lines based on detected fractals
            if (isBearishFractal)
            {
                UpperLine = fractalQuote.High;
            }
            if (isBullishFractal)
            {
                LowerLine = fractalQuote.Low;
            }
        }

        // create result
        FcbResult r = new(
            Timestamp: item.Timestamp,
            UpperBand: UpperLine,
            LowerBand: LowerLine);

        return (r, i);
    }

    /// <summary>
    /// Restores the prior FCB state.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int i = ProviderCache.IndexGte(timestamp);

        // restore prior state
        if (i > 0)
        {
            FcbResult priorResult = Cache[i - 1];
            UpperLine = priorResult.UpperBand;
            LowerLine = priorResult.LowerBand;
        }
        else
        {
            // reset to initial state
            UpperLine = null;
            LowerLine = null;
        }
    }
}


public static partial class Fcb
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FcbHub"/> class.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="windowSpan">The window span for the calculation. Default is 2.</param>
    /// <returns>An instance of <see cref="FcbHub"/>.</returns>
    public static FcbHub ToFcbHub(
       this IQuoteProvider<IQuote> quoteProvider,
       int windowSpan = 2)
           => new(quoteProvider, windowSpan);

    /// <summary>
    /// Creates a Fcb hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="windowSpan">The window span for the calculation. Default is 2.</param>
    /// <returns>An instance of <see cref="FcbHub"/>.</returns>
    public static FcbHub ToFcbHub(
        this IReadOnlyList<IQuote> quotes,
        int windowSpan = 2)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToFcbHub(windowSpan);
    }
}
