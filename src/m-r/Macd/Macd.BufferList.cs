namespace Skender.Stock.Indicators;

/// <summary>
/// MACD (Moving Average Convergence Divergence) from incremental reusable values.
/// </summary>
public class MacdList : List<MacdResult>, IMacd, IBufferQuote, IBufferReusable
{
    private double lastEmaFast = double.NaN;
    private double lastEmaSlow = double.NaN;
    private double lastEmaMacd = double.NaN;
    
    private readonly double kFast;
    private readonly double kSlow;
    private readonly double kMacd;

    // Store input values for proper SMA initialization
    private readonly List<double> _inputValues = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="MacdList"/> class.
    /// </summary>
    /// <param name="fastPeriods">The number of periods for the fast EMA. Default is 12.</param>
    /// <param name="slowPeriods">The number of periods for the slow EMA. Default is 26.</param>
    /// <param name="signalPeriods">The number of periods for the signal line. Default is 9.</param>
    public MacdList(int fastPeriods = 12, int slowPeriods = 26, int signalPeriods = 9)
    {
        Macd.Validate(fastPeriods, slowPeriods, signalPeriods);
        
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;
        SignalPeriods = signalPeriods;

        // calculate smoothing factors
        kFast = 2d / (fastPeriods + 1);
        kSlow = 2d / (slowPeriods + 1);
        kMacd = 2d / (signalPeriods + 1);
    }

    /// <summary>
    /// Gets the number of periods for the fast EMA.
    /// </summary>
    public int FastPeriods { get; }

    /// <summary>
    /// Gets the number of periods for the slow EMA.
    /// </summary>
    public int SlowPeriods { get; }

    /// <summary>
    /// Gets the number of periods for the signal line.
    /// </summary>
    public int SignalPeriods { get; }

    /// <summary>
    /// Adds a new value to the MACD list.
    /// </summary>
    /// <param name="timestamp">The timestamp of the value.</param>
    /// <param name="value">The value to add.</param>
    public void Add(DateTime timestamp, double value)
    {
        int i = Count;
        _inputValues.Add(value);

        // Fast EMA
        double emaFast
            = i >= FastPeriods - 1 && i > 0 && this[i - 1].FastEma is null
            ? CalculateSma(FastPeriods, i)
            : Ema.Increment(kFast, lastEmaFast, value);

        lastEmaFast = emaFast;

        // Slow EMA
        double emaSlow
            = i >= SlowPeriods - 1 && i > 0 && this[i - 1].SlowEma is null
            ? CalculateSma(SlowPeriods, i)
            : Ema.Increment(kSlow, lastEmaSlow, value);

        lastEmaSlow = emaSlow;

        // MACD line
        double macd = emaFast - emaSlow;

        // Signal line
        double signal;

        if (i >= SignalPeriods + SlowPeriods - 2 && i > 0 && this[i - 1].Signal is null)
        {
            // Initialize signal with SMA of MACD values
            double sum = macd;
            for (int p = i - SignalPeriods + 1; p < i; p++)
            {
                sum += this[p].Value; // MacdResult.Value is the MACD value (IReusable.Value)
            }
            signal = sum / SignalPeriods;
        }
        else
        {
            signal = Ema.Increment(kMacd, lastEmaMacd, macd);
        }

        lastEmaMacd = signal;

        // Histogram
        double histogram = macd - signal;

        // Add result to list
        base.Add(new MacdResult(
            timestamp,
            macd.NaN2Null(),
            signal.NaN2Null(),
            histogram.NaN2Null(),
            i >= FastPeriods - 1 ? emaFast.NaN2Null() : null,
            i >= SlowPeriods - 1 ? emaSlow.NaN2Null() : null));
    }

    /// <summary>
    /// Calculates a simple moving average for EMA initialization.
    /// </summary>
    /// <param name="periods">The number of periods.</param>
    /// <param name="currentIndex">The current index.</param>
    /// <returns>The simple moving average.</returns>
    private double CalculateSma(int periods, int currentIndex)
    {
        // Calculate SMA using stored input values
        double sum = 0;
        for (int i = currentIndex - periods + 1; i <= currentIndex; i++)
        {
            sum += _inputValues[i];
        }
        return sum / periods;
    }

    /// <summary>
    /// Adds a new reusable value to the MACD list.
    /// </summary>
    /// <param name="value">The reusable value to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <summary>
    /// Adds a list of reusable values to the MACD list.
    /// </summary>
    /// <param name="values">The list of reusable values to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the values list is null.</exception>
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <summary>
    /// Adds a new quote to the MACD list.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quote is null.</exception>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    /// <summary>
    /// Adds a list of quotes to the MACD list.
    /// </summary>
    /// <param name="quotes">The list of quotes to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i].Timestamp, quotes[i].Value);
        }
    }
}