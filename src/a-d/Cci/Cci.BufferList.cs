namespace Skender.Stock.Indicators;

/// <summary>
/// Commodity Channel Index (CCI) from incremental quote values.
/// </summary>
public class CciList : BufferList<CciResult>, IIncrementFromQuote, ICci
{
    private readonly Queue<double> _tpBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="CciList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    public CciList(int lookbackPeriods)
    {
        Cci.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _tpBuffer = new Queue<double>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CciList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public CciList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods) => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Calculate typical price
        double tp = ((double)quote.High + (double)quote.Low + (double)quote.Close) / 3d;

        // Update buffer using universal buffer utilities
        _tpBuffer.Update(LookbackPeriods, tp);

        double? cci = null;

        // Calculate CCI when we have enough data
        if (_tpBuffer.Count == LookbackPeriods)
        {
            // Calculate average TP over lookback
            double avgTp = 0;
            foreach (double tpValue in _tpBuffer)
            {
                avgTp += tpValue;
            }

            avgTp /= LookbackPeriods;

            // Calculate average Deviation over lookback
            double avgDv = 0;
            foreach (double tpValue in _tpBuffer)
            {
                avgDv += Math.Abs(avgTp - tpValue);
            }

            avgDv /= LookbackPeriods;

            // Calculate CCI
            cci = avgDv == 0 ? null
                : ((tp - avgTp) / (0.015 * avgDv)).NaN2Null();
        }

        AddInternal(new CciResult(quote.Timestamp, cci));
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
        _tpBuffer.Clear();
    }
}
