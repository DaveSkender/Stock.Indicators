namespace Skender.Stock.Indicators;

/// <summary>
/// Relative Strength Index (RSI) from incremental reusable values.
/// </summary>
public class RsiList : BufferList<RsiResult>, IIncrementFromChain, IRsi
{
    private readonly Queue<(double Gain, double Loss)> _buffer;
    private double _avgGain = double.NaN;
    private double _avgLoss = double.NaN;
    private double _prevValue = double.NaN;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="RsiList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public RsiList(int lookbackPeriods)
    {
        Rsi.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<(double, double)>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RsiList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public RsiList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }




    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        double gain = 0;
        double loss = 0;

        // Calculate gain/loss for current period
        if (!_isInitialized)
        {
            _prevValue = value;
            _isInitialized = true;
        }
        else if (!double.IsNaN(value) && !double.IsNaN(_prevValue))
        {
            gain = value > _prevValue ? value - _prevValue : 0;
            loss = value < _prevValue ? _prevValue - value : 0;
        }
        else
        {
            gain = loss = double.NaN;
        }

        // Update buffer using universal buffer utilities with consolidated tuple
        _buffer.Update(LookbackPeriods, (gain, loss));

        double? rsi = null;
        _prevValue = value;

        // Calculate RSI when we have enough data points
        // We need at least lookbackPeriods + 1 total values (because first value is just for reference)
        int totalValuesProcessed = Count + 1; // +1 because we haven't added to base list yet

        // Re/initialize average gain/loss (first calculation after reaching lookback periods)
        if (totalValuesProcessed >= LookbackPeriods + 1 && (double.IsNaN(_avgGain) || double.IsNaN(_avgLoss)))
        {
            double sumGain = 0;
            double sumLoss = 0;

            foreach ((double Gain, double Loss) in _buffer)
            {
                sumGain += Gain;
                sumLoss += Loss;
            }

            _avgGain = sumGain / LookbackPeriods;
            _avgLoss = sumLoss / LookbackPeriods;

            rsi = !double.IsNaN(_avgGain / _avgLoss)
                  ? _avgLoss > 0 ? 100 - (100 / (1 + (_avgGain / _avgLoss))) : 100
                  : null;
        }
        // Calculate RSI using exponential smoothing for subsequent periods
        else if (totalValuesProcessed > LookbackPeriods + 1)
        {
            _avgGain = ((_avgGain * (LookbackPeriods - 1)) + gain) / LookbackPeriods;
            _avgLoss = ((_avgLoss * (LookbackPeriods - 1)) + loss) / LookbackPeriods;

            if (_avgLoss > 0)
            {
                double rs = _avgGain / _avgLoss;
                rsi = 100 - (100 / (1 + rs));
            }
            else
            {
                rsi = 100;
            }
        }

        AddInternal(new RsiResult(timestamp, rsi));
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
        base.Clear();
        _buffer.Clear();
        _avgGain = double.NaN;
        _avgLoss = double.NaN;
        _prevValue = double.NaN;
        _isInitialized = false;
    }
}

public static partial class Rsi
{
    /// <summary>
    /// Creates a buffer list for Relative Strength Index (RSI) calculations.
    /// </summary>
    public static RsiList ToRsiList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
