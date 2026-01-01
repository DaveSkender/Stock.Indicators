namespace Skender.Stock.Indicators;

/// <summary>
/// Chaikin Money Flow (CMF) from incremental quotes.
/// </summary>
public class CmfList : BufferList<CmfResult>, IIncrementFromQuote, ICmf
{
    private readonly AdlList _adlList;
    private readonly Queue<(double Volume, double? Mfv)> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CmfList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public CmfList(int lookbackPeriods = 20)
    {
        Cmf.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _adlList = [];
        _buffer = new Queue<(double, double?)>(lookbackPeriods);

        Name = $"CMF({lookbackPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CmfList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public CmfList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods) => Add(quotes);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;
        double volume = (double)quote.Volume;

        // Calculate ADL
        _adlList.Add(quote);
        AdlResult adlResult = _adlList[^1];

        // Update buffer with consolidated tuple
        _buffer.Update(LookbackPeriods, (volume, adlResult.MoneyFlowVolume));

        double? cmf = null;

        // Calculate CMF when we have enough data
        if (_buffer.Count == LookbackPeriods)
        {
            double? sumMfv = 0;
            double sumVol = 0;

            foreach ((double Volume, double? Mfv) in _buffer)
            {
                sumMfv += Mfv;
                sumVol += Volume;
            }

            double? avgMfv = sumMfv / LookbackPeriods;
            double avgVol = sumVol / LookbackPeriods;

            if (avgVol != 0)
            {
                cmf = avgMfv / avgVol;
            }
        }

        AddInternal(new CmfResult(
            timestamp,
            adlResult.MoneyFlowMultiplier,
            adlResult.MoneyFlowVolume,
            cmf));
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
        _adlList.Clear();
        _buffer.Clear();
    }
}

public static partial class Cmf
{
    /// <summary>
    /// Creates a buffer list for Chaikin Money Flow (CMF) calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    public static CmfList ToCmfList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
        => new(lookbackPeriods) { quotes };
}
