namespace Skender.Stock.Indicators;

/// <summary>
/// Volume Weighted Average Price (VWAP) from incremental quote values.
/// </summary>
public class VwapList : BufferList<VwapResult>, IIncrementFromQuote
{
    private readonly bool _autoAnchor;
    private DateTime? _startDate;
    private double _cumVolume;
    private double _cumVolumeTp;

    /// <summary>
    /// Initializes a new instance of the <see cref="VwapList"/> class.
    /// </summary>
    /// <param name="startDate">The start date for VWAP calculation. Quotes before this date will have null VWAP.</param>
    public VwapList(DateTime? startDate = null)
    {
        _autoAnchor = startDate == null;
        _startDate = startDate;
        _cumVolume = 0;
        _cumVolumeTp = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VwapList"/> class with initial quotes.
    /// </summary>
    /// <param name="startDate">The start date for VWAP calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public VwapList(DateTime? startDate, IReadOnlyList<IQuote> quotes)
        : this(startDate)
        => Add(quotes);

    /// <summary>
    /// Gets the start date for the VWAP calculation.
    /// </summary>
    public DateTime? StartDate
    {
        get => _startDate;
        init => _startDate = value;
    }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Set start date to first quote's timestamp if not provided
        if (_startDate == null)
        {
            _startDate = quote.Timestamp;
        }

        double? vwap;

        if (quote.Timestamp >= _startDate.Value)
        {
            double v = (double)quote.Volume;
            double h = (double)quote.High;
            double l = (double)quote.Low;
            double c = (double)quote.Close;

            _cumVolume += v;
            _cumVolumeTp += v * (h + l + c) / 3;

            vwap = _cumVolume != 0 ? _cumVolumeTp / _cumVolume : null;
        }
        else
        {
            vwap = null;
        }

        AddInternal(new VwapResult(quote.Timestamp, vwap));
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
            _startDate = null;
        }
    }
}

public static partial class Vwap
{
    /// <summary>
    /// Creates a buffer list for Volume Weighted Average Price (VWAP) calculations.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="startDate">The start date for VWAP calculation.</param>
    /// <returns>A new <see cref="VwapList"/> instance.</returns>
    public static VwapList ToVwapList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        DateTime startDate)
        where TQuote : IQuote
        => new(startDate) { (IReadOnlyList<IQuote>)quotes };

    /// <summary>
    /// Creates a buffer list for Volume Weighted Average Price (VWAP) calculations
    /// starting from the first quote's timestamp.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote, which must implement <see cref="IQuote"/>.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <returns>A new <see cref="VwapList"/> instance.</returns>
    public static VwapList ToVwapList<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote
        => new(null) { (IReadOnlyList<IQuote>)quotes };
}
