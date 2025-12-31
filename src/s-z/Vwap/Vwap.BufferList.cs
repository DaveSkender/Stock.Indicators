namespace Skender.Stock.Indicators;

/// <summary>
/// Volume Weighted Average Price (VWAP) from incremental quotes.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="VwapList"/> class.
/// </remarks>
/// <param name="startDate">The start date for VWAP calculation. If null, auto-anchors to first quote.</param>
public class VwapList(DateTime? startDate = null) : BufferList<VwapResult>, IIncrementFromQuote
{
    private readonly bool _autoAnchor = (startDate ?? default) == default;
    private double _cumVolume;
    private double _cumVolumeTp;

    /// <summary>
    /// Initializes a new instance of the <see cref="VwapList"/> class with initial quotes.
    /// </summary>
    /// <param name="startDate">The start date for VWAP calculation. If null, auto-anchors to first quote.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public VwapList(DateTime? startDate, IReadOnlyList<IQuote> quotes)
        : this(startDate) => Add(quotes);

    /// <inheritdoc />
    public DateTime? StartDate { get; private set; } = startDate;

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Set start date to first quote's timestamp if auto-anchoring
        if (StartDate == null)
        {
            StartDate = quote.Timestamp;
        }

        double volume = (double)quote.Volume;
        double high = (double)quote.High;
        double low = (double)quote.Low;
        double close = (double)quote.Close;

        double? vwap;

        if (quote.Timestamp >= StartDate.Value)
        {
            _cumVolume += volume;
            _cumVolumeTp += volume * (high + low + close) / 3;

            vwap = _cumVolume != 0 ? _cumVolumeTp / _cumVolume : null;
        }
        else
        {
            vwap = null;
        }

        AddInternal(new VwapResult(
            Timestamp: quote.Timestamp,
            Vwap: vwap));
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i]);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        _cumVolume = 0;
        _cumVolumeTp = 0;

        // Reset _startDate to null if in auto-anchor mode
        if (_autoAnchor)
        {
            StartDate = null;
        }
    }
}

public static partial class Vwap
{
    /// <summary>
    /// Creates a buffer list for VWAP calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="startDate">Starting date for calculation</param>
    public static VwapList ToVwapList(
        this IReadOnlyList<IQuote> quotes,
        DateTime startDate)
        => new(startDate) { quotes };

    /// <summary>
    /// Creates a buffer list for VWAP calculations starting from the first quote.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public static VwapList ToVwapList(
        this IReadOnlyList<IQuote> quotes)
        => new(null) { quotes };
}
