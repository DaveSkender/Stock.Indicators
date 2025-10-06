namespace Skender.Stock.Indicators;

/// <summary>
/// On-Balance Volume (OBV) from incremental quote values.
/// </summary>
public class ObvList : BufferList<ObvResult>, IBufferList
{
    private double _previousClose = double.NaN;
    private const int DefaultMaxListSize = (int)(0.9 * int.MaxValue);
    private double _obvValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObvList"/> class.
    /// </summary>
    public ObvList()
    {
        MaxListSize = DefaultMaxListSize;
        // OBV doesn't require any parameters or initialization
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObvList"/> class with initial quotes.
    /// </summary>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public ObvList(IReadOnlyList<IQuote> quotes)
        : this()
        => Add(quotes);


    /// <summary>
    /// Gets or sets the maximum size of the result list before pruning occurs.
    /// When the list exceeds this size, older results are removed. Default is 90% of int.MaxValue.
    /// </summary>
    public int MaxListSize { get; init; }


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
        PruneList();

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
        ClearInternal();
        _previousClose = double.NaN;
        _obvValue = 0;
    }

    /// <summary>
    /// Prunes the result list to prevent unbounded memory growth.
    /// </summary>
    private void PruneList()
    {
        if (Count < MaxListSize)
        {
            return;
        }

        // Remove oldest results while keeping the list under MaxListSize
        while (Count >= MaxListSize)
        {
            RemoveAtInternal(0);
        }
    }
}
