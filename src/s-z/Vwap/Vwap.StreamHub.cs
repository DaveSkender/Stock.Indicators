namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Volume Weighted Average Price (VWAP).
/// </summary>
public class VwapHub : ChainHub<IQuote, VwapResult>
{
    private readonly bool _autoAnchor;
    private double _cumVolume;
    private double _cumVolumeTp;

    internal VwapHub(
        IQuoteProvider<IQuote> provider,
        DateTime? startDate = null)
        : base(provider)
    {
        StartDate = startDate;
        _autoAnchor = (startDate ?? default) == default;
        Name = "VWAP";
        Reinitialize();
    }

    /// <inheritdoc/>
    public DateTime? StartDate { get; private set; }

    /// <inheritdoc/>
    protected override (VwapResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Set start date to first quote's timestamp if auto-anchoring
        if (StartDate == null)
        {
            StartDate = item.Timestamp;
        }

        double volume = (double)item.Volume;
        double high = (double)item.High;
        double low = (double)item.Low;
        double close = (double)item.Close;

        double? vwap;

        if (item.Timestamp >= StartDate.Value)
        {
            _cumVolume += volume;
            _cumVolumeTp += volume * (high + low + close) / 3;

            vwap = _cumVolume != 0 ? _cumVolumeTp / _cumVolume : null;
        }
        else
        {
            vwap = null;
        }

        VwapResult r = new(
            Timestamp: item.Timestamp,
            Vwap: vwap);

        return (r, i);
    }

    /// <summary>
    /// Restores the cumulative state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear cumulative state
        _cumVolume = 0;
        _cumVolumeTp = 0;

        // Reset start date if auto-anchoring
        if (_autoAnchor)
        {
            StartDate = null;
        }

        // Rebuild cumulative state from ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        int targetIndex = index - 1;

        for (int p = 0; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];

            // Set start date to first quote if auto-anchoring
            if (StartDate == null)
            {
                StartDate = quote.Timestamp;
            }

            if (quote.Timestamp >= StartDate.Value)
            {
                double volume = (double)quote.Volume;
                double high = (double)quote.High;
                double low = (double)quote.Low;
                double close = (double)quote.Close;

                _cumVolume += volume;
                _cumVolumeTp += volume * (high + low + close) / 3;
            }
        }
    }

    /// <inheritdoc/>
    public override string ToString() =>
        StartDate.HasValue ? $"VWAP({StartDate.Value:d})" : "VWAP";
}

/// <summary>
/// Provides extension methods for creating VWAP hubs.
/// </summary>
public static partial class Vwap
{
    /// <summary>
    /// Creates a VWAP streaming hub from a quotes provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="startDate">The start date for VWAP calculation. If null, auto-anchors to first quote.</param>
    /// <returns>A VWAP streaming hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the quote provider is null.</exception>
    public static VwapHub ToVwapHub(
        this IQuoteProvider<IQuote> quoteProvider,
        DateTime? startDate = null)
    {
        ArgumentNullException.ThrowIfNull(quoteProvider);
        return new VwapHub(quoteProvider, startDate);
    }
}
