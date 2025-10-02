namespace Skender.Stock.Indicators;

/// <summary>
/// Chande Momentum Oscillator (CMO) from incremental reusable values.
/// </summary>
public class CmoList : BufferList<CmoResult>, IBufferReusable, ICmo
{
    private readonly Queue<(bool? isUp, double value)> _tickBuffer;
    private double _prevValue = double.NaN;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="CmoList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public CmoList(int lookbackPeriods)
    {
        Cmo.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _tickBuffer = new Queue<(bool? isUp, double value)>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CmoList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public CmoList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        double? cmo = null;

        // First value - just initialize
        if (!_isInitialized)
        {
            _prevValue = value;
            _isInitialized = true;
        }
        else
        {
            // Determine tick direction and size
            (bool? isUp, double tickValue) tick = (null, Math.Abs(value - _prevValue));

            tick.isUp = double.IsNaN(tick.tickValue) || value == _prevValue
                ? null
                : value > _prevValue;

            // Update buffer using universal buffer utilities
            _tickBuffer.Update(LookbackPeriods, tick);

            // Calculate CMO when we have enough data
            if (_tickBuffer.Count == LookbackPeriods)
            {
                double sH = 0;
                double sL = 0;
                bool hasNaN = false;

                foreach ((bool? isUp, double pDiff) in _tickBuffer)
                {
                    if (double.IsNaN(pDiff))
                    {
                        hasNaN = true;
                        break;
                    }

                    // up
                    if (isUp == true)
                    {
                        sH += pDiff;
                    }
                    // down
                    else
                    {
                        sL += pDiff;
                    }
                }

                if (!hasNaN)
                {
                    cmo = sH + sL != 0
                        ? (100 * (sH - sL) / (sH + sL)).NaN2Null()
                        : null;
                }
            }

            _prevValue = value;
        }

        AddInternal(new CmoResult(timestamp, cmo));
    }

    /// <inheritdoc />
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i].Timestamp, quotes[i].Value);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        ClearInternal();
        _tickBuffer.Clear();
        _prevValue = double.NaN;
        _isInitialized = false;
    }
}
