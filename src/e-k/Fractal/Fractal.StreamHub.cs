namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Williams Fractal indicator using a stream hub.
/// </summary>
public class FractalHub
    : StreamHub<IQuote, FractalResult>, IFractal
{
    internal FractalHub(
        IQuoteProvider<IQuote> provider,
        int windowSpan,
        EndType endType) : this(provider, windowSpan, windowSpan, endType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FractalHub"/> class with different left and right spans.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="leftSpan">The number of periods to look back for the calculation.</param>
    /// <param name="rightSpan">The number of periods to look forward for the calculation.</param>
    /// <param name="endType">The type of price to use for the calculation.</param>
    internal FractalHub(
        IQuoteProvider<IQuote> provider,
        int leftSpan,
        int rightSpan,
        EndType endType) : base(provider)
    {
        // Validate both spans independently
        Fractal.Validate(leftSpan);
        Fractal.Validate(rightSpan);

        LeftSpan = leftSpan;
        RightSpan = rightSpan;
        EndType = endType;
        Name = $"FRACTAL({leftSpan},{rightSpan},{endType.ToString().ToUpperInvariant()})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LeftSpan { get; init; }

    /// <inheritdoc/>
    public int RightSpan { get; init; }

    /// <inheritdoc/>
    public EndType EndType { get; init; }

    /// <inheritdoc/>
    public override void OnAdd(IQuote item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (CacheLock)
        {
            // First, add the current item
            base.OnAdd(item, notify, indexHint);

            // For forward-looking indicators, we need to recalculate the position
            // that now has sufficient right context (i - RightSpan)
            int i = indexHint ?? ProviderCache.IndexOf(item, true);
            int targetIndex = i - RightSpan;

            // Only recalculate if the target position is valid and needs calculation
            if (targetIndex >= LeftSpan && targetIndex < Cache.Count)
            {
                FractalResult existing = Cache[targetIndex];

                // Only recalculate if the result was previously null (insufficient context)
                if (existing.FractalBear is null && existing.FractalBull is null)
                {
                    (FractalResult updated, int _) = ToIndicator(ProviderCache[targetIndex], targetIndex);
                    Cache[targetIndex] = updated;

                    // Notify observers if a fractal was actually found and notify is enabled
                    if (notify && (updated.FractalBear is not null || updated.FractalBull is not null))
                    {
                        NotifyObserversOnRebuild(updated.Timestamp);
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    protected override (FractalResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // For Fractal, we calculate the value for the CURRENT quote (at index i)
        // but we need to check if we have sufficient left AND right context
        // Series logic: i + 1 > leftSpan && i + 1 <= length - rightSpan

        int length = ProviderCache.Count;
        decimal? fractalBear = null;
        decimal? fractalBull = null;

        // Can we calculate fractal for the quote at index i?
        // We need LeftSpan quotes before it and RightSpan quotes after it
        if (i + 1 > LeftSpan && i + 1 <= length - RightSpan)
        {
            IQuote center = ProviderCache[i];
            bool isHigh = true;
            bool isLow = true;

            decimal evalHigh = EndType == EndType.Close ? center.Close : center.High;
            decimal evalLow = EndType == EndType.Close ? center.Close : center.Low;

            // Compare center with wings (both left and right)
            for (int p = i - LeftSpan; p <= i + RightSpan; p++)
            {
                // Skip center
                if (p == i)
                {
                    continue;
                }

                IQuote wing = ProviderCache[p];
                decimal wingHigh = EndType == EndType.Close ? wing.Close : wing.High;
                decimal wingLow = EndType == EndType.Close ? wing.Close : wing.Low;

                if (evalHigh <= wingHigh)
                {
                    isHigh = false;
                }

                if (evalLow >= wingLow)
                {
                    isLow = false;
                }
            }

            fractalBear = isHigh ? evalHigh : null;
            fractalBull = isLow ? evalLow : null;
        }

        // Create result for the current quote
        FractalResult result = new(ProviderCache[i].Timestamp, fractalBear, fractalBull);

        return (result, i);
    }
}

public static partial class Fractal
{
    /// <summary>
    /// Creates a Fractal hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="windowSpan">The window span used for both left and right spans.</param>
    /// <param name="endType">The price end type to use.</param>
    /// <returns>An instance of <see cref="FractalHub"/>.</returns>
    public static FractalHub ToFractalHub(
       this IQuoteProvider<IQuote> quoteProvider,
       int windowSpan = 2,
       EndType endType = EndType.HighLow)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, windowSpan, endType);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FractalHub"/> class with different left and right spans.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="leftSpan">The number of periods to look back for the calculation.</param>
    /// <param name="rightSpan">The number of periods to look forward for the calculation.</param>
    /// <param name="endType">The type of price to use for the calculation. Default is <see cref="EndType.HighLow"/>.</param>
    /// <returns>An instance of <see cref="FractalHub"/>.</returns>
    public static FractalHub ToFractalHub(
       this IQuoteProvider<IQuote> quoteProvider,
       int leftSpan,
       int rightSpan,
       EndType endType = EndType.HighLow)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new(quoteProvider, leftSpan, rightSpan, endType);
    }
}
