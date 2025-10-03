namespace Skender.Stock.Indicators;

/// <summary>
/// Williams %R from incremental quote values.
/// </summary>
public class WilliamsRList : BufferList<WilliamsResult>, IWilliamsR, IBufferList
{
    private readonly Queue<double> _highBuffer;
    private readonly Queue<double> _lowBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsRList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    public WilliamsRList(
        int lookbackPeriods = 14)
    {
        WilliamsR.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _highBuffer = new Queue<double>(lookbackPeriods);
        _lowBuffer = new Queue<double>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsRList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public WilliamsRList(
        int lookbackPeriods,
        IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the lookback periods for Williams %R calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, (double)quote.High, (double)quote.Low, (double)quote.Close);
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

    /// <summary>
    /// Adds a new quote data point for Williams %R calculation.
    /// </summary>
    /// <param name="timestamp">The timestamp of the data point.</param>
    /// <param name="high">The high price.</param>
    /// <param name="low">The low price.</param>
    /// <param name="close">The close price.</param>
    public void Add(DateTime timestamp, double high, double low, double close)
    {
        // Update rolling buffers using BufferUtilities
        _highBuffer.Update(LookbackPeriods, high);
        _lowBuffer.Update(LookbackPeriods, low);

        // Calculate Williams %R when we have enough data
        double? williamsR = null;
        if (_highBuffer.Count == LookbackPeriods)
        {
            double highHigh = _highBuffer.Max();
            double lowLow = _lowBuffer.Min();

            if (highHigh - lowLow != 0)
            {
                williamsR = -100.0 * (highHigh - close) / (highHigh - lowLow);
            }
            else
            {
                williamsR = 0;
            }
        }

        // Add result to the list
        AddInternal(new WilliamsResult(
            Timestamp: timestamp,
            WilliamsR: williamsR));
    }

    /// <inheritdoc />
    public override void Clear()
    {
        ClearInternal();
        _highBuffer.Clear();
        _lowBuffer.Clear();
    }
}
