namespace Skender.Stock.Indicators;

/// <summary>
/// Volume Weighted Average Price (VWAP) from incremental quotes.
/// </summary>
public class VwapList : BufferList<VwapResult>, IIncrementFromQuote
{
    private readonly DateTime _startDate;
    private double _cumVolume;
    private double _cumVolumeTp;

    /// <summary>
    /// Initializes a new instance of the <see cref="VwapList"/> class.
    /// </summary>
    /// <param name="startDate">The start date for the VWAP calculation.</param>
    public VwapList(DateTime startDate)
    {
        StartDate = startDate;
        _startDate = startDate;
        _cumVolume = 0;
        _cumVolumeTp = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VwapList"/> class with initial quotes.
    /// </summary>
    /// <param name="startDate">The start date for the VWAP calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public VwapList(DateTime startDate, IReadOnlyList<IQuote> quotes)
        : this(startDate)
        => Add(quotes);

    /// <summary>
    /// Gets the start date for the VWAP calculation.
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        double volume = (double)quote.Volume;
        double high = (double)quote.High;
        double low = (double)quote.Low;
        double close = (double)quote.Close;

        double? vwap;

        if (quote.Timestamp >= _startDate)
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
    }
}

public static partial class Vwap
{
    /// <summary>
    /// Creates a buffer list for VWAP calculations.
    /// </summary>
    public static VwapList ToVwapList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        DateTime startDate)
        where TQuote : IQuote
        => new(startDate) { (IReadOnlyList<IQuote>)quotes };

    /// <summary>
    /// Creates a buffer list for VWAP calculations starting from the first quote.
    /// </summary>
    public static VwapList ToVwapList<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote
        => quotes?.Count is null or 0
            ? new VwapList(DateTime.MinValue)
            : new VwapList(quotes[0].Timestamp) { (IReadOnlyList<IQuote>)quotes };
}
