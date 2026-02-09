namespace Skender.Stock.Indicators;

/// <summary>
/// Heikin-Ashi from incremental quote values.
/// </summary>
public class HeikinAshiList : BufferList<HeikinAshiResult>, IIncrementFromQuote
{
    private decimal _prevOpen = decimal.MinValue;
    private decimal _prevClose = decimal.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="HeikinAshiList"/> class.
    /// </summary>
    public HeikinAshiList()
    {
        // HeikinAshi doesn't require any parameters or initialization
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeikinAshiList"/> class with initial quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public HeikinAshiList(IReadOnlyList<IQuote> quotes)
        : this() => Add(quotes);

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Initialize on first quote
        if (_prevOpen == decimal.MinValue)
        {
            _prevOpen = quote.Open;
            _prevClose = quote.Close;
        }

        // close
        decimal close = (quote.Open + quote.High + quote.Low + quote.Close) / 4;

        // open
        decimal open = (_prevOpen + _prevClose) / 2;

        // high
        decimal high = Math.Max(quote.High, Math.Max(open, close));

        // low
        decimal low = Math.Min(quote.Low, Math.Min(open, close));

        AddInternal(new HeikinAshiResult(
            Timestamp: quote.Timestamp,
            Open: open,
            High: high,
            Low: low,
            Close: close,
            Volume: quote.Volume));

        // save for next iteration
        _prevOpen = open;
        _prevClose = close;
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
        _prevOpen = decimal.MinValue;
        _prevClose = decimal.MinValue;
    }
}

public static partial class HeikinAshi
{
    /// <summary>
    /// Creates a buffer list for Heikin-Ashi calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public static HeikinAshiList ToHeikinAshiList(
        this IReadOnlyList<IQuote> quotes)
        => new() { quotes };
}
