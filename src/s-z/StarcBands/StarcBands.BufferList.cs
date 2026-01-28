namespace Skender.Stock.Indicators;

/// <summary>
/// STARC Bands from incremental quotes.
/// </summary>
public class StarcBandsList : BufferList<StarcBandsResult>, IIncrementFromQuote, IStarcBands
{
    private readonly SmaList _smaList;
    private readonly AtrList _atrList;

    /// <summary>
    /// Initializes a new instance of the <see cref="StarcBandsList"/> class.
    /// </summary>
    /// <param name="smaPeriods">The number of periods for the Simple Moving Average (SMA).</param>
    /// <param name="multiplier">The multiplier for the Average True Range (ATR).</param>
    /// <param name="atrPeriods">The number of periods for the ATR calculation.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="atrPeriods"/> is invalid.</exception>
    public StarcBandsList(
        int smaPeriods = 5,
        double multiplier = 2,
        int atrPeriods = 10)
    {
        StarcBands.Validate(smaPeriods, multiplier, atrPeriods);
        SmaPeriods = smaPeriods;
        Multiplier = multiplier;
        AtrPeriods = atrPeriods;

        _smaList = new SmaList(smaPeriods);
        _atrList = new AtrList(atrPeriods);

        Name = $"STARCBANDS({smaPeriods}, {multiplier}, {atrPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StarcBandsList"/> class with initial quotes.
    /// </summary>
    /// <param name="smaPeriods">The number of periods for the Simple Moving Average (SMA).</param>
    /// <param name="multiplier">The multiplier for the Average True Range (ATR).</param>
    /// <param name="atrPeriods">The number of periods for the ATR calculation.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public StarcBandsList(
        int smaPeriods,
        double multiplier,
        int atrPeriods,
        IReadOnlyList<IQuote> quotes)
        : this(smaPeriods, multiplier, atrPeriods) => Add(quotes);

    /// <inheritdoc />
    public int SmaPeriods { get; init; }

    /// <inheritdoc />
    public double Multiplier { get; init; }

    /// <inheritdoc />
    public int AtrPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;

        // Add quote to both component lists
        _smaList.Add(quote);
        _atrList.Add(quote);

        // Get the latest results from each component
        SmaResult smaResult = _smaList[^1];
        AtrResult atrResult = _atrList[^1];

        // Calculate STARC Bands
        double? upperBand = null;
        double? centerline = smaResult.Sma;
        double? lowerBand = null;

        if (smaResult.Sma.HasValue && atrResult.Atr.HasValue)
        {
            upperBand = smaResult.Sma.Value + (Multiplier * atrResult.Atr.Value);
            lowerBand = smaResult.Sma.Value - (Multiplier * atrResult.Atr.Value);
        }

        AddInternal(new StarcBandsResult(
            Timestamp: timestamp,
            UpperBand: upperBand,
            Centerline: centerline,
            LowerBand: lowerBand));
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
        _smaList.Clear();
        _atrList.Clear();
    }
}

public static partial class StarcBands
{
    /// <summary>
    /// Creates a buffer list for STARC Bands calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="smaPeriods">Number of periods for the simple moving average</param>
    /// <param name="multiplier">Multiplier for calculation</param>
    /// <param name="atrPeriods">Number of periods for ATR calculation</param>
    public static StarcBandsList ToStarcBandsList(
        this IReadOnlyList<IQuote> quotes,
        int smaPeriods = 5,
        double multiplier = 2,
        int atrPeriods = 10)
        => new(smaPeriods, multiplier, atrPeriods) { quotes };
}
