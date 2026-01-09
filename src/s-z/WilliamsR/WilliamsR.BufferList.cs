namespace Skender.Stock.Indicators;

/// <summary>
/// Williams %R from incremental quote values.
/// </summary>
public class WilliamsRList : BufferList<WilliamsResult>, IIncrementFromQuote, IWilliamsR
{
    private readonly Queue<(double High, double Low)> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsRList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public WilliamsRList(
        int lookbackPeriods = 14)
    {
        WilliamsR.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<(double, double)>(lookbackPeriods);

        Name = $"WILLIAMSR({lookbackPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsRList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public WilliamsRList(
        int lookbackPeriods,
        IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods) => Add(quotes);

    /// <inheritdoc />
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
        // Update rolling buffer using BufferListUtilities with consolidated tuple
        _buffer.Update(LookbackPeriods, (high, low));

        // Calculate Williams %R when we have enough data
        double? williamsR = null;
        if (_buffer.Count == LookbackPeriods)
        {
            double highHigh = double.MinValue;
            double lowLow = double.MaxValue;

            foreach ((double High, double Low) in _buffer)
            {
                if (High > highHigh)
                {
                    highHigh = High;
                }

                if (Low < lowLow)
                {
                    lowLow = Low;
                }
            }

            // Apply boundary clamping to ensure -100 ≤ WilliamsR ≤ 0
            // This prevents floating-point precision errors at boundaries
            if (highHigh == lowLow)
            {
                williamsR = -100.0;
            }
            else if (close >= highHigh)
            {
                williamsR = 0.0;
            }
            else if (close <= lowLow)
            {
                williamsR = -100.0;
            }
            else
            {
                williamsR = (100.0 * (close - lowLow) / (highHigh - lowLow)) - 100.0;
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
        base.Clear();
        _buffer.Clear();
    }
}
