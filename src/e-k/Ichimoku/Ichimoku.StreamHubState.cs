namespace Skender.Stock.Indicators;

/// <summary>
/// State object for Ichimoku streaming hub.
/// </summary>
/// <remarks>
/// Ichimoku uses complex rolling window calculations that are difficult to cache efficiently.
/// This implementation uses a minimal state marker to trigger rollback for complex operations.
/// </remarks>
public record IchimokuState() : IHubState;

/// <summary>
/// Streaming hub for Ichimoku Cloud using state management.
/// </summary>
/// <remarks>
/// This implementation caches the rolling window states (Tenkan and Kijun high/low) for efficient updates.
/// State restoration after rollback uses cached window states instead of recalculating.
/// </remarks>
public class IchimokuHubState
    : StreamHubState<IQuote, IchimokuState, IchimokuResult>, IIchimoku
{
    private readonly RollingWindowMax<decimal> tenkanHighWindow;
    private readonly RollingWindowMin<decimal> tenkanLowWindow;
    private readonly RollingWindowMax<decimal> kijunHighWindow;
    private readonly RollingWindowMin<decimal> kijunLowWindow;

    internal IchimokuHubState(
        IQuoteProvider<IQuote> provider,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int senkouOffset,
        int chikouOffset)
        : base(provider)
    {
        Ichimoku.Validate(
            tenkanPeriods,
            kijunPeriods,
            senkouBPeriods,
            senkouOffset,
            chikouOffset);

        TenkanPeriods = tenkanPeriods;
        KijunPeriods = kijunPeriods;
        SenkouBPeriods = senkouBPeriods;
        SenkouOffset = senkouOffset;
        ChikouOffset = chikouOffset;

        Name = $"ICHIMOKU({tenkanPeriods},{kijunPeriods},{senkouBPeriods})-State";

        tenkanHighWindow = new RollingWindowMax<decimal>(tenkanPeriods);
        tenkanLowWindow = new RollingWindowMin<decimal>(tenkanPeriods);
        kijunHighWindow = new RollingWindowMax<decimal>(kijunPeriods);
        kijunLowWindow = new RollingWindowMin<decimal>(kijunPeriods);

        Reinitialize();
    }

    /// <inheritdoc/>
    public int TenkanPeriods { get; init; }

    /// <inheritdoc/>
    public int KijunPeriods { get; init; }

    /// <inheritdoc/>
    public int SenkouBPeriods { get; init; }

    /// <inheritdoc/>
    public int SenkouOffset { get; init; }

    /// <inheritdoc/>
    public int ChikouOffset { get; init; }

    /// <summary>
    /// Handles adding a new quote and updates past results that now have sufficient data for ChikouSpan.
    /// </summary>
    /// <param name="item">New item from provider.</param>
    /// <param name="notify">Whether to notify observers.</param>
    /// <param name="indexHint">Provider index hint.</param>
    public override void OnAdd(IQuote item, bool notify, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        // Add the new item normally
        base.OnAdd(item, notify, indexHint);

        // Update past result that can now have its ChikouSpan calculated
        // This happens when a new quote provides the forward data needed
        if (ChikouOffset > 0 && Cache.Count > ChikouOffset && ProviderCache.Count > ChikouOffset)
        {
            int providerIndex = indexHint ?? ProviderCache.IndexOf(item, true);

            // Find the past result that should be updated
            // It's the result that is ChikouOffset periods before the current item
            int backfillProviderIndex = providerIndex - ChikouOffset;

            if (backfillProviderIndex >= 0 && backfillProviderIndex < ProviderCache.Count)
            {
                // Find the past result in Cache by timestamp (not by index)
                DateTime backfillTimestamp = ProviderCache[backfillProviderIndex].Timestamp;
                int backfillCacheIndex = Cache.IndexOf(backfillTimestamp, false);

                if (backfillCacheIndex >= 0 && backfillCacheIndex < Cache.Count)
                {
                    IchimokuResult pastResult = Cache[backfillCacheIndex];

                    // During normal streaming (notify=true), only update if ChikouSpan is null
                    // During rebuilds (notify=false), always update to ensure correctness
                    if (!notify || pastResult.ChikouSpan is null)
                    {
                        decimal chikouClose = ProviderCache[providerIndex].Close;

                        // Update the past result
                        Cache[backfillCacheIndex] = pastResult with { ChikouSpan = chikouClose };
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    protected override (IchimokuResult result, IchimokuState state, int index)
        ToIndicatorState(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Add current quote to rolling windows
        tenkanHighWindow.Add(item.High);
        tenkanLowWindow.Add(item.Low);
        kijunHighWindow.Add(item.High);
        kijunLowWindow.Add(item.Low);

        // Calculate Tenkan-sen (conversion line)
        decimal? tenkanSen = null;
        if (i >= TenkanPeriods - 1)
        {
            decimal max = tenkanHighWindow.GetMax();
            decimal min = tenkanLowWindow.GetMin();
            tenkanSen = (min + max) / 2;
        }

        // Calculate Kijun-sen (base line)
        decimal? kijunSen = null;
        if (i >= KijunPeriods - 1)
        {
            decimal max = kijunHighWindow.GetMax();
            decimal min = kijunLowWindow.GetMin();
            kijunSen = (min + max) / 2;
        }

        // Calculate Senkou Span A (leading span A)
        decimal? senkouSpanA = null;
        int senkouStartPeriod = Math.Max(
            2 * SenkouOffset,
            Math.Max(TenkanPeriods, KijunPeriods)) - 1;

        if (i >= senkouStartPeriod)
        {
            if (SenkouOffset == 0)
            {
                senkouSpanA = (tenkanSen + kijunSen) / 2;
            }
            else
            {
                IchimokuResult skq = Cache[i - SenkouOffset];
                senkouSpanA = (skq.TenkanSen + skq.KijunSen) / 2;
            }
        }

        // Calculate Senkou Span B (leading span B)
        decimal? senkouSpanB = null;
        if (i >= SenkouOffset + SenkouBPeriods - 1)
        {
            // Calculate max/min over the SenkouB period, offset back
            decimal max = decimal.MinValue;
            decimal min = decimal.MaxValue;

            for (int p = i - SenkouOffset - SenkouBPeriods + 1;
                 p <= i - SenkouOffset; p++)
            {
                IQuote d = ProviderCache[p];

                if (d.High > max)
                {
                    max = d.High;
                }

                if (d.Low < min)
                {
                    min = d.Low;
                }
            }

            senkouSpanB = min == decimal.MaxValue ? null : (min + max) / 2;
        }

        // Calculate Chikou Span (lagging span)
        // This is forward-looking: ChikouSpan at index i uses Close from index i + ChikouOffset
        decimal? chikouSpan = null;
        int chikouIndex = i + ChikouOffset;
        if (chikouIndex < ProviderCache.Count)
        {
            chikouSpan = ProviderCache[chikouIndex].Close;
        }

        // Create result
        IchimokuResult r = new(
            Timestamp: item.Timestamp,
            TenkanSen: tenkanSen,
            KijunSen: kijunSen,
            SenkouSpanA: senkouSpanA,
            SenkouSpanB: senkouSpanB,
            ChikouSpan: chikouSpan);

        // Create minimal state marker
        IchimokuState state = new();

        return (r, state, i);
    }

    /// <inheritdoc/>
    protected override void RestorePreviousState(IchimokuState? previousState)
    {
        // Ichimoku's complex multi-window state makes efficient caching challenging.
        // For all operations (including same-candle updates), we fall back to RollbackState.
        // This is acceptable given Ichimoku's already complex calculation overhead.
        // Simply reset - RollbackState will handle reconstruction.
        tenkanHighWindow.Clear();
        tenkanLowWindow.Clear();
        kijunHighWindow.Clear();
        kijunLowWindow.Clear();
    }

    /// <summary>
    /// Restores the rolling window states up to the specified timestamp for complex operations.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear all windows
        tenkanHighWindow.Clear();
        tenkanLowWindow.Clear();
        kijunHighWindow.Clear();
        kijunLowWindow.Clear();

        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        int targetIndex = index - 1;

        // Rebuild Tenkan windows
        int tenkanStart = Math.Max(0, targetIndex - TenkanPeriods + 1);
        for (int p = tenkanStart; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            tenkanHighWindow.Add(quote.High);
            tenkanLowWindow.Add(quote.Low);
        }

        // Rebuild Kijun windows
        int kijunStart = Math.Max(0, targetIndex - KijunPeriods + 1);
        for (int p = kijunStart; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            kijunHighWindow.Add(quote.High);
            kijunLowWindow.Add(quote.Low);
        }

        // After Remove operations, ChikouSpan values that point past the end need to be nulled
        // Calculate which results might have invalid ChikouSpan values
        if (ChikouOffset > 0 && Cache.Count > 0)
        {
            int firstInvalidIndex = ProviderCache.Count - ChikouOffset;

            // Null out ChikouSpan for results that now reference beyond ProviderCache
            for (int i = Math.Max(0, firstInvalidIndex); i < Cache.Count; i++)
            {
                IchimokuResult result = Cache[i];
                if (result.ChikouSpan is not null)
                {
                    Cache[i] = result with { ChikouSpan = null };
                }
            }
        }
    }
}

public static partial class Ichimoku
{
    /// <summary>
    /// Converts a quote provider to an Ichimoku hub with state management.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <returns>An Ichimoku hub with state management.</returns>
    public static IchimokuHubState ToIchimokuHubState(
        this IQuoteProvider<IQuote> quoteProvider,
        int tenkanPeriods = 9,
        int kijunPeriods = 26,
        int senkouBPeriods = 52)
        => new(quoteProvider, tenkanPeriods, kijunPeriods, senkouBPeriods, kijunPeriods, kijunPeriods);

    /// <summary>
    /// Converts a quote provider to an Ichimoku hub with state management and specified parameters.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="offsetPeriods">The number of periods for the offset.</param>
    /// <returns>An Ichimoku hub with state management.</returns>
    public static IchimokuHubState ToIchimokuHubState(
        this IQuoteProvider<IQuote> quoteProvider,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int offsetPeriods)
        => new(quoteProvider, tenkanPeriods, kijunPeriods, senkouBPeriods, offsetPeriods, offsetPeriods);
}
