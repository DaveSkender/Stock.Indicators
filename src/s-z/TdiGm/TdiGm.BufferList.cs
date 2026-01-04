namespace Skender.Stock.Indicators;

/// <summary>
/// TDI-GM from incremental reusable values.
/// </summary>
public class TdiGmList : BufferList<TdiGmResult>, IIncrementFromChain, ITdiGm
{
    private readonly RsiList _rsiList;
    private readonly SmaList _middleBandList;
    private readonly StdDevList _stdDevList;
    private readonly SmaList _fastMaList;
    private readonly SmaList _slowMaList;

    /// <summary>
    /// Initializes a new instance of the <see cref="TdiGmList"/> class.
    /// </summary>
    /// <param name="rsiPeriod">The RSI period.</param>
    /// <param name="bandLength">The band length.</param>
    /// <param name="fastLength">The fast length.</param>
    /// <param name="slowLength">The slow length.</param>
    public TdiGmList(
        int rsiPeriod = 21,
        int bandLength = 34,
        int fastLength = 2,
        int slowLength = 7
    )
    {
        TdiGm.Validate(rsiPeriod, bandLength, fastLength, slowLength);
        RsiPeriod = rsiPeriod;
        BandLength = bandLength;
        FastLength = fastLength;
        SlowLength = slowLength;

        // Initialize internal buffer lists for incremental processing
        _rsiList = new RsiList(rsiPeriod);
        _middleBandList = new SmaList(bandLength);
        _stdDevList = new StdDevList(bandLength);
        _fastMaList = new SmaList(fastLength);
        _slowMaList = new SmaList(slowLength);

        Name = $"TdiGm({rsiPeriod}, {bandLength}, {fastLength}, {slowLength})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TdiGmList"/> class with initial reusable values.
    /// </summary>
    /// <param name="rsiPeriod">The RSI period.</param>
    /// <param name="bandLength">The band length.</param>
    /// <param name="fastLength">The fast length.</param>
    /// <param name="slowLength">The slow length.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public TdiGmList(
        int rsiPeriod,
        int bandLength,
        int fastLength,
        int slowLength,
        IReadOnlyList<IReusable> values
    )
        : this(rsiPeriod, bandLength, fastLength, slowLength) => Add(values);

    public int RsiPeriod { get; init; }
    public int BandLength { get; init; }
    public int FastLength { get; init; }
    public int SlowLength { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Add value to RSI calculation
        _rsiList.Add(timestamp, value);

        // Get the latest RSI result
        RsiResult rsiResult = _rsiList[^1];

        // If RSI is available, feed it through the other indicators
        if (rsiResult.Rsi.HasValue)
        {
            double rsiValue = rsiResult.Rsi.Value;

            // Add RSI value to all the downstream calculations
            _middleBandList.Add(timestamp, rsiValue);
            _stdDevList.Add(timestamp, rsiValue);
            _fastMaList.Add(timestamp, rsiValue);
            _slowMaList.Add(timestamp, rsiValue);
        }
        else
        {
            // Add NaN to maintain alignment
            _middleBandList.Add(timestamp, double.NaN);
            _stdDevList.Add(timestamp, double.NaN);
            _fastMaList.Add(timestamp, double.NaN);
            _slowMaList.Add(timestamp, double.NaN);
        }

        // Get the latest results from each list
        SmaResult middleBandResult = _middleBandList[^1];
        StdDevResult stdDevResult = _stdDevList[^1];
        SmaResult fastMaResult = _fastMaList[^1];
        SmaResult slowMaResult = _slowMaList[^1];

        // Calculate bands
        double? upper = null;
        double? lower = null;
        double? middle = null;

        if (middleBandResult.Sma != null && stdDevResult.StdDev != null)
        {
            double ma = middleBandResult.Sma.Value;
            double stdDev = stdDevResult.StdDev.Value;
            double offset = 1.6185 * stdDev;

            upper = ma + offset;
            lower = ma - offset;
            middle = ma;
        }

        // Create and add the result
        AddInternal(
            new TdiGmResult {
                Timestamp = timestamp,
                Upper = upper,
                Lower = lower,
                Middle = middle,
                Fast = fastMaResult.Sma,
                Slow = slowMaResult.Sma
            });
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
    public override void Clear()
    {
        base.Clear();
        _rsiList.Clear();
        _middleBandList.Clear();
        _stdDevList.Clear();
        _fastMaList.Clear();
        _slowMaList.Clear();
    }
}

public static partial class TdiGm
{
    /// <summary>
    /// Creates a buffer list for TDI-GM calculations.
    /// </summary>
    /// <param name="source">The source list of reusable values.</param>
    /// <param name="rsiPeriod">The RSI period.</param>
    /// <param name="bandLength">The band length.</param>
    /// <param name="fastLength">The fast length.</param>
    /// <param name="slowLength">The slow length.</param>
    /// <returns>A buffer list for TDI-GM calculations.</returns>
    public static TdiGmList ToTdiGmList(
        this IReadOnlyList<IReusable> source,
        int rsiPeriod = 21,
        int bandLength = 34,
        int fastLength = 2,
        int slowLength = 7)
        => new(rsiPeriod, bandLength, fastLength, slowLength) { source };
}
