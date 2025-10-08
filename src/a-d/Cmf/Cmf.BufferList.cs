namespace Skender.Stock.Indicators;

/// <summary>
/// Chaikin Money Flow (CMF) from incremental quotes.
/// </summary>
public class CmfList : BufferList<CmfResult>, IBufferList, ICmf
{
    private readonly AdlList _adlList;
    private readonly Queue<double> _volumeBuffer;
    private readonly Queue<double?> _mfvBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CmfList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window.</param>
    public CmfList(int lookbackPeriods = 20)
    {
        Cmf.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _adlList = new AdlList();
        _volumeBuffer = new Queue<double>(lookbackPeriods);
        _mfvBuffer = new Queue<double?>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CmfList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public CmfList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to use for the lookback window.
    /// </summary>
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

        // Update buffers
        _volumeBuffer.Update(LookbackPeriods, volume);
        _mfvBuffer.Update(LookbackPeriods, adlResult.MoneyFlowVolume);

        double? cmf = null;

        // Calculate CMF when we have enough data
        if (_volumeBuffer.Count == LookbackPeriods)
        {
            double? sumMfv = 0;
            double sumVol = 0;

            foreach (double? mfv in _mfvBuffer)
            {
                sumMfv += mfv;
            }

            foreach (double vol in _volumeBuffer)
            {
                sumVol += vol;
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
        ClearInternal();
        _adlList.Clear();
        _volumeBuffer.Clear();
        _mfvBuffer.Clear();
    }
}

public static partial class Cmf
{
    /// <summary>
    /// Creates a buffer list for Chaikin Money Flow (CMF) calculations.
    /// </summary>
    public static CmfList ToCmfList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 20)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
