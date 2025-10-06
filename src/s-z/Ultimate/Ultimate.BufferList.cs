namespace Skender.Stock.Indicators;

/// <summary>
/// Ultimate Oscillator from incremental quotes.
/// </summary>
public class UltimateList : BufferList<UltimateResult>, IUltimate, IBufferList
{
    private readonly Queue<double> _bpBuffer;
    private readonly Queue<double> _trBuffer;
    private double _previousClose;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="UltimateList"/> class.
    /// </summary>
    /// <param name="shortPeriods">The number of short periods.</param>
    /// <param name="middlePeriods">The number of middle periods.</param>
    /// <param name="longPeriods">The number of long periods.</param>
    public UltimateList(
        int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
    {
        Ultimate.Validate(shortPeriods, middlePeriods, longPeriods);
        ShortPeriods = shortPeriods;
        MiddlePeriods = middlePeriods;
        LongPeriods = longPeriods;

        _bpBuffer = new Queue<double>(longPeriods);
        _trBuffer = new Queue<double>(longPeriods);
        _isInitialized = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UltimateList"/> class with initial quotes.
    /// </summary>
    /// <param name="shortPeriods">The number of short periods.</param>
    /// <param name="middlePeriods">The number of middle periods.</param>
    /// <param name="longPeriods">The number of long periods.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public UltimateList(
        int shortPeriods,
        int middlePeriods,
        int longPeriods,
        IReadOnlyList<IQuote> quotes)
        : this(shortPeriods, middlePeriods, longPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of short periods.
    /// </summary>
    public int ShortPeriods { get; init; }

    /// <summary>
    /// Gets the number of middle periods.
    /// </summary>
    public int MiddlePeriods { get; init; }

    /// <summary>
    /// Gets the number of long periods.
    /// </summary>
    public int LongPeriods { get; init; }




    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;
        double high = (double)quote.High;
        double low = (double)quote.Low;
        double close = (double)quote.Close;

        // Handle first period - no calculation possible
        if (!_isInitialized)
        {
            _previousClose = close;
            _isInitialized = true;

            AddInternal(new UltimateResult(timestamp, null));
            PruneList();
            return;
        }

        // Calculate buying pressure and true range
        double bp = close - Math.Min(low, _previousClose);
        double tr = Math.Max(high, _previousClose) - Math.Min(low, _previousClose);

        // Update buffers
        _bpBuffer.Update(LongPeriods, bp);
        _trBuffer.Update(LongPeriods, tr);

        double? ultimate = null;

        // Calculate Ultimate Oscillator when we have enough data
        if (_bpBuffer.Count >= LongPeriods)
        {
            double sumBp1 = 0;  // short period
            double sumBp2 = 0;  // middle period
            double sumBp3 = 0;  // long period

            double sumTr1 = 0;  // short period
            double sumTr2 = 0;  // middle period
            double sumTr3 = 0;  // long period

            // Convert queues to arrays for indexed access
            double[] bpArray = _bpBuffer.ToArray();
            double[] trArray = _trBuffer.ToArray();

            int bufferLength = bpArray.Length;

            // Calculate sums for all three periods
            for (int i = 0; i < bufferLength; i++)
            {
                // Long period includes all values
                sumBp3 += bpArray[i];
                sumTr3 += trArray[i];

                // Middle period includes more recent values
                if (i >= bufferLength - MiddlePeriods)
                {
                    sumBp2 += bpArray[i];
                    sumTr2 += trArray[i];
                }

                // Short period includes most recent values
                if (i >= bufferLength - ShortPeriods)
                {
                    sumBp1 += bpArray[i];
                    sumTr1 += trArray[i];
                }
            }

            // Calculate averages (avoid division by zero)
            double avg1 = sumTr1 == 0 ? double.NaN : sumBp1 / sumTr1;
            double avg2 = sumTr2 == 0 ? double.NaN : sumBp2 / sumTr2;
            double avg3 = sumTr3 == 0 ? double.NaN : sumBp3 / sumTr3;

            // Calculate Ultimate Oscillator with weighted average
            ultimate = (100d * ((4d * avg1) + (2d * avg2) + avg3) / 7d).NaN2Null();
        }

        AddInternal(new UltimateResult(timestamp, ultimate));
        _previousClose = close;
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
        _bpBuffer.Clear();
        _trBuffer.Clear();
        _previousClose = 0;
        _isInitialized = false;
    }
}

public static partial class Ultimate
{
    /// <summary>
    /// Creates a buffer list for Ultimate Oscillator calculations.
    /// </summary>
    public static UltimateList ToUltimateList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int shortPeriods = 7,
        int middlePeriods = 14,
        int longPeriods = 28)
        where TQuote : IQuote
        => new(shortPeriods, middlePeriods, longPeriods) { (IReadOnlyList<IQuote>)quotes };
}
