namespace Skender.Stock.Indicators;

// ICHIMOKU CLOUD (STREAM HUB)

/// <summary>
/// Represents a stream hub for calculating the Ichimoku Cloud indicator.
/// </summary>
/// <inheritdoc cref="IIchimoku"/>
public class IchimokuHub
    : StreamHub<IQuote, IchimokuResult>, IIchimoku
{
    #region fields

    private readonly string hubName;
    private readonly RollingWindowMax<decimal> tenkanHighWindow;
    private readonly RollingWindowMin<decimal> tenkanLowWindow;
    private readonly RollingWindowMax<decimal> kijunHighWindow;
    private readonly RollingWindowMin<decimal> kijunLowWindow;

    #endregion fields

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IchimokuHub"/> class.
    /// </summary>
    /// <param name="provider">The quote provider.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="senkouOffset">The number of periods for the Senkou offset.</param>
    /// <param name="chikouOffset">The number of periods for the Chikou offset.</param>
    internal IchimokuHub(
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

        hubName = $"ICHIMOKU({tenkanPeriods},{kijunPeriods},{senkouBPeriods})";

        tenkanHighWindow = new RollingWindowMax<decimal>(tenkanPeriods);
        tenkanLowWindow = new RollingWindowMin<decimal>(tenkanPeriods);
        kijunHighWindow = new RollingWindowMax<decimal>(kijunPeriods);
        kijunLowWindow = new RollingWindowMin<decimal>(kijunPeriods);

        Reinitialize();
    }

    #endregion constructors

    #region properties

    /// <summary>
    /// Gets the number of periods for the Tenkan-sen (conversion line).
    /// </summary>
    public int TenkanPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the Kijun-sen (base line).
    /// </summary>
    public int KijunPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the Senkou Span B (leading span B).
    /// </summary>
    public int SenkouBPeriods { get; init; }

    /// <summary>
    /// Gets the number of periods for the Senkou offset.
    /// </summary>
    public int SenkouOffset { get; init; }

    /// <summary>
    /// Gets the number of periods for the Chikou offset.
    /// </summary>
    public int ChikouOffset { get; init; }

    #endregion properties

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

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
            // The newly added result is at Cache.Count - 1 (let's call this currentIndex)
            // It completes ChikouSpan for the result at currentIndex - ChikouOffset
            int currentCacheIndex = Cache.Count - 1;
            int backfillCacheIndex = currentCacheIndex - ChikouOffset;

            if (backfillCacheIndex >= 0 && backfillCacheIndex < Cache.Count)
            {
                IchimokuResult pastResult = Cache[backfillCacheIndex];

                // Only update if ChikouSpan is currently null
                // and we have the corresponding data in ProviderCache
                if (pastResult.ChikouSpan is null)
                {
                    // Assuming Cache and ProviderCache have matching indices
                    // (they should, as they're both sorted by timestamp)
                    int pastProviderIndex = backfillCacheIndex;
                    int chikouProviderIndex = pastProviderIndex + ChikouOffset;

                    if (chikouProviderIndex < ProviderCache.Count)
                    {
                        decimal chikouClose = ProviderCache[chikouProviderIndex].Close;

                        // Update the past result
                        Cache[backfillCacheIndex] = new(
                            Timestamp: pastResult.Timestamp,
                            TenkanSen: pastResult.TenkanSen,
                            KijunSen: pastResult.KijunSen,
                            SenkouSpanA: pastResult.SenkouSpanA,
                            SenkouSpanB: pastResult.SenkouSpanB,
                            ChikouSpan: chikouClose);
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    protected override (IchimokuResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
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

        return (r, i);
    }

    /// <summary>
    /// Restores the rolling window states up to the specified timestamp.
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
    }

    #endregion methods
}

public static partial class Ichimoku
{
    /// <summary>
    /// Converts a quote provider to an Ichimoku hub.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <returns>An Ichimoku hub.</returns>
    public static IchimokuHub ToIchimokuHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int tenkanPeriods = 9,
        int kijunPeriods = 26,
        int senkouBPeriods = 52)
        => new(quoteProvider, tenkanPeriods, kijunPeriods, senkouBPeriods, kijunPeriods, kijunPeriods);

    /// <summary>
    /// Converts a quote provider to an Ichimoku hub with specified parameters.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="offsetPeriods">The number of periods for the offset.</param>
    /// <returns>An Ichimoku hub.</returns>
    public static IchimokuHub ToIchimokuHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int offsetPeriods)
        => new(quoteProvider, tenkanPeriods, kijunPeriods, senkouBPeriods, offsetPeriods, offsetPeriods);

    /// <summary>
    /// Creates an Ichimoku hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <returns>An instance of <see cref="IchimokuHub"/>.</returns>
    public static IchimokuHub ToIchimokuHub(
        this IReadOnlyList<IQuote> quotes,
        int tenkanPeriods = 9,
        int kijunPeriods = 26,
        int senkouBPeriods = 52)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToIchimokuHub(tenkanPeriods, kijunPeriods, senkouBPeriods);
    }

    /// <summary>
    /// Creates an Ichimoku hub from a collection of quotes with specified parameters.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="tenkanPeriods">The number of periods for the Tenkan-sen (conversion line).</param>
    /// <param name="kijunPeriods">The number of periods for the Kijun-sen (base line).</param>
    /// <param name="senkouBPeriods">The number of periods for the Senkou Span B (leading span B).</param>
    /// <param name="offsetPeriods">The number of periods for the offset.</param>
    /// <returns>An instance of <see cref="IchimokuHub"/>.</returns>
    public static IchimokuHub ToIchimokuHub(
        this IReadOnlyList<IQuote> quotes,
        int tenkanPeriods,
        int kijunPeriods,
        int senkouBPeriods,
        int offsetPeriods)
    {
        ArgumentNullException.ThrowIfNull(quotes);
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToIchimokuHub(tenkanPeriods, kijunPeriods, senkouBPeriods, offsetPeriods);
    }
}
