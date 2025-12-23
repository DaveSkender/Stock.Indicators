namespace Skender.Stock.Indicators;

/// <summary>
/// On-Balance Volume (OBV) from incremental quote values.
/// </summary>
public class ObvList : BufferList<ObvResult>, IIncrementFromQuote
{
    private double _previousClose = double.NaN;
    private double _obvValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObvList"/> class.
    /// </summary>
    public ObvList() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObvList"/> class with initial quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public ObvList(IReadOnlyList<IQuote> quotes)
        : this() => Add(quotes);

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Handle volume direction changes in streaming mode
        if (!double.IsNaN(_previousClose))
        {
            if ((double)quote.Close > _previousClose)
            {
                _obvValue += (double)quote.Volume;
            }
            else if ((double)quote.Close < _previousClose)
            {
                _obvValue -= (double)quote.Volume;
            }
            // No change if quote.Close == _previousClose
        }

        // Add result with current OBV value
        AddInternal(new ObvResult(
            Timestamp: quote.Timestamp,
            Obv: _obvValue));

        // Update previous close for next iteration
        _previousClose = (double)quote.Close;
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
        _previousClose = double.NaN;
        _obvValue = 0;
    }
}

public static partial class Obv
{
    /// <summary>
    /// Creates a buffer list for On-Balance Volume (OBV) calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public static ObvList ToObvList(
        this IReadOnlyList<IQuote> quotes)
        => new() { quotes };
}
