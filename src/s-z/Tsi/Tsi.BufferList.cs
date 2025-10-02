namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the True Strength Index (TSI) indicator.
/// </summary>
public static partial class Tsi
{
    /// <summary>
    /// Creates a TSI buffer list from reusable values.
    /// </summary>
    /// <typeparam name="T">The type of the source data, which must implement <see cref="IReusable"/>.</typeparam>
    /// <param name="source">The list of source data.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback calculation. Default is 25.</param>
    /// <param name="smoothPeriods">The number of periods for the smoothing calculation. Default is 13.</param>
    /// <param name="signalPeriods">The number of periods for the signal calculation. Default is 7.</param>
    /// <returns>A TSI buffer list.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the source list is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    public static TsiList ToTsiList<T>(
        this IReadOnlyList<T> source,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
        where T : IReusable
        => new(lookbackPeriods, smoothPeriods, signalPeriods) { (IReadOnlyList<IReusable>)source };
}

/// <summary>
/// True Strength Index (TSI) from incremental reusable values.
/// </summary>
public class TsiList : BufferList<TsiResult>, IBufferReusable, ITsi
{
    private readonly Queue<double> _lookbackBufferC;  // price change buffer
    private readonly Queue<double> _lookbackBufferA;  // absolute change buffer
    private readonly Queue<double> _smoothBufferC;    // first smooth change buffer
    private readonly Queue<double> _smoothBufferA;    // first smooth abs change buffer
    private readonly Queue<double> _signalBuffer;     // signal line buffer
    
    private double _lookbackBufferSumC;
    private double _lookbackBufferSumA;
    private double _smoothBufferSumC;
    private double _smoothBufferSumA;
    private double _signalBufferSum;
    private int _tsiCount;  // Count of TSI values calculated
    
    private double? _lastPriceChange;
    private double? _lastCs1;
    private double? _lastAs1;
    private double? _lastCs2;
    private double? _lastAs2;
    private double? _lastSignal;

    /// <summary>
    /// Initializes a new instance of the <see cref="TsiList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods for the lookback calculation.</param>
    /// <param name="smoothPeriods">The number of periods for the smoothing calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal calculation.</param>
    public TsiList(
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
    {
        Tsi.Validate(lookbackPeriods, smoothPeriods, signalPeriods);
        LookbackPeriods = lookbackPeriods;
        SmoothPeriods = smoothPeriods;
        SignalPeriods = signalPeriods;

        Mult1 = 2d / (lookbackPeriods + 1);
        Mult2 = 2d / (smoothPeriods + 1);
        MultS = 2d / (signalPeriods + 1);

        _lookbackBufferC = new Queue<double>(lookbackPeriods);
        _lookbackBufferA = new Queue<double>(lookbackPeriods);
        _smoothBufferC = new Queue<double>(smoothPeriods);
        _smoothBufferA = new Queue<double>(smoothPeriods);
        _signalBuffer = new Queue<double>(signalPeriods);

        _lookbackBufferSumC = 0;
        _lookbackBufferSumA = 0;
        _smoothBufferSumC = 0;
        _smoothBufferSumA = 0;
        _signalBufferSum = 0;
        _tsiCount = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TsiList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods for the lookback calculation.</param>
    /// <param name="smoothPeriods">The number of periods for the smoothing calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public TsiList(
        int lookbackPeriods,
        int smoothPeriods,
        int signalPeriods,
        IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods, smoothPeriods, signalPeriods)
        => Add(quotes);

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public int SmoothPeriods { get; init; }

    /// <inheritdoc/>
    public int SignalPeriods { get; init; }

    /// <summary>
    /// Gets the smoothing factor for the first smoothing.
    /// </summary>
    public double Mult1 { get; private init; }

    /// <summary>
    /// Gets the smoothing factor for the second smoothing.
    /// </summary>
    public double Mult2 { get; private init; }

    /// <summary>
    /// Gets the smoothing factor for the signal line.
    /// </summary>
    public double MultS { get; private init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Skip first period (need previous value for price change)
        if (Count == 0)
        {
            _lastPriceChange = value;
            AddInternal(new TsiResult(timestamp));
            return;
        }

        // Calculate price change
        double priceChange = value - _lastPriceChange!.Value;
        double absChange = Math.Abs(priceChange);
        _lastPriceChange = value;

        // Update lookback buffers using BufferUtilities
        double? dequeuedC = _lookbackBufferC.UpdateWithDequeue(LookbackPeriods, priceChange);
        double? dequeuedA = _lookbackBufferA.UpdateWithDequeue(LookbackPeriods, absChange);
        
        if (dequeuedC.HasValue)
        {
            _lookbackBufferSumC = _lookbackBufferSumC - dequeuedC.Value + priceChange;
            _lookbackBufferSumA = _lookbackBufferSumA - dequeuedA.Value + absChange;
        }
        else
        {
            _lookbackBufferSumC += priceChange;
            _lookbackBufferSumA += absChange;
        }

        // Calculate first smoothing (cs1, as1)
        double? cs1 = null;
        double? as1 = null;
        
        if (Count >= LookbackPeriods)
        {
            if (_lastCs1 is null)
            {
                // Initialize as SMA
                cs1 = _lookbackBufferSumC / LookbackPeriods;
                as1 = _lookbackBufferSumA / LookbackPeriods;
            }
            else
            {
                // Normal EMA calculation
                cs1 = ((priceChange - _lastCs1.Value) * Mult1) + _lastCs1.Value;
                as1 = ((absChange - _lastAs1!.Value) * Mult1) + _lastAs1.Value;
            }
            
            _lastCs1 = cs1;
            _lastAs1 = as1;
        }

        // Calculate second smoothing (cs2, as2)
        double? cs2 = null;
        double? as2 = null;
        
        if (cs1.HasValue && as1.HasValue)
        {
            // Update smooth buffers
            double? dequeuedCs = _smoothBufferC.UpdateWithDequeue(SmoothPeriods, cs1.Value);
            double? dequeuedAs = _smoothBufferA.UpdateWithDequeue(SmoothPeriods, as1.Value);
            
            if (dequeuedCs.HasValue)
            {
                _smoothBufferSumC = _smoothBufferSumC - dequeuedCs.Value + cs1.Value;
                _smoothBufferSumA = _smoothBufferSumA - dequeuedAs!.Value + as1.Value;
            }
            else
            {
                _smoothBufferSumC += cs1.Value;
                _smoothBufferSumA += as1.Value;
            }

            if (_smoothBufferC.Count >= SmoothPeriods)
            {
                if (_lastCs2 is null)
                {
                    // Initialize as SMA
                    cs2 = _smoothBufferSumC / SmoothPeriods;
                    as2 = _smoothBufferSumA / SmoothPeriods;
                }
                else
                {
                    // Normal EMA calculation
                    cs2 = ((cs1.Value - _lastCs2.Value) * Mult2) + _lastCs2.Value;
                    as2 = ((as1.Value - _lastAs2!.Value) * Mult2) + _lastAs2.Value;
                }
                
                _lastCs2 = cs2;
                _lastAs2 = as2;
            }
        }

        // Calculate TSI
        double? tsi = null;
        if (cs2.HasValue && as2.HasValue && as2.Value != 0)
        {
            tsi = 100d * (cs2.Value / as2.Value);
            _tsiCount++;
        }

        // Calculate Signal line
        double? signal = null;
        if (tsi.HasValue)
        {
            if (SignalPeriods > 1)
            {
                // Update signal buffer
                double? dequeuedSignal = _signalBuffer.UpdateWithDequeue(SignalPeriods, tsi.Value);
                
                if (dequeuedSignal.HasValue)
                {
                    _signalBufferSum = _signalBufferSum - dequeuedSignal.Value + tsi.Value;
                }
                else
                {
                    _signalBufferSum += tsi.Value;
                }

                // Initialize signal when we have signalPeriods or more TSI values
                if (_lastSignal is null && _tsiCount >= SignalPeriods)
                {
                    // Initialize as SMA
                    signal = _signalBufferSum / SignalPeriods;
                }
                // Continue with EMA after initialization
                else if (_lastSignal is not null)
                {
                    // Normal EMA calculation
                    signal = ((tsi.Value - _lastSignal.Value) * MultS) + _lastSignal.Value;
                }
                
                if (signal.HasValue)
                {
                    _lastSignal = signal;
                }
            }
            else if (SignalPeriods == 1)
            {
                signal = tsi.Value;
            }
        }

        // Add result
        TsiResult result = new(
            Timestamp: timestamp,
            Tsi: tsi,
            Signal: signal);

        AddInternal(result);
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
        _lookbackBufferC.Clear();
        _lookbackBufferA.Clear();
        _smoothBufferC.Clear();
        _smoothBufferA.Clear();
        _signalBuffer.Clear();
        _lookbackBufferSumC = 0;
        _lookbackBufferSumA = 0;
        _smoothBufferSumC = 0;
        _smoothBufferSumA = 0;
        _signalBufferSum = 0;
        _tsiCount = 0;
        _lastPriceChange = null;
        _lastCs1 = null;
        _lastAs1 = null;
        _lastCs2 = null;
        _lastAs2 = null;
        _lastSignal = null;
    }
}
