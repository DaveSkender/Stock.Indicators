namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Fractal Chaos Bands (FCB) using a stream hub.
/// </summary>
public class FcbHub
    : StreamHub<IQuote, FcbResult>, IFcb
{
    internal FcbHub(
        IQuoteProvider<IQuote> provider,
        int windowSpan) : base(provider)
    {
        Fcb.Validate(windowSpan);
        WindowSpan = windowSpan;
        Name = $"FCB({windowSpan})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int WindowSpan { get; init; }

    /// <summary>
    /// state tracking for upper and lower bands
    /// </summary>
    private decimal? UpperLine { get; set; }
    private decimal? LowerLine { get; set; }

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
                if (p != fractalIndex && ProviderCache[p].High >= fractalQuote.High)
                {
                    isBearishFractal = false;
                    break;
                }
            }

            // check for bullish fractal (low point) at fractalIndex
            bool isBullishFractal = true;
            for (int p = fractalIndex - WindowSpan; p <= fractalIndex + WindowSpan; p++)
            {
                if (p != fractalIndex && ProviderCache[p].Low <= fractalQuote.Low)
                {
                    isBullishFractal = false;
                    break;
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
    /// Creates a Fractal Chaos Bands (FCB) hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="windowSpan">The window span used for fractal detection.</param>
    /// <returns>An instance of <see cref="FcbHub"/>.</returns>
    public static FcbHub ToFcbHub(
       this IQuoteProvider<IQuote> quoteProvider,
       int windowSpan = 2)
           => new(quoteProvider, windowSpan);
}
