namespace Skender.Stock.Indicators;

/// <summary>
/// Commodity Channel Index (CCI) from incremental quote values.
/// </summary>
public class CciList : BufferList<CciResult>, IIncrementFromQuote, ICci
{
    private readonly Queue<double> _buffer; // true price

    /// <summary>
    /// Initializes a new instance of the <see cref="CciList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public CciList(int lookbackPeriods)
    {
        Cci.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<double>(lookbackPeriods);

        Name = $"CCI({lookbackPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CciList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public CciList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods) => Add(quotes);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Calculate typical price
        double tp = ((double)quote.High + (double)quote.Low + (double)quote.Close) / 3d;

        // Update buffer using universal buffer utilities
        _buffer.Update(LookbackPeriods, tp);

        double? cci = null;

        // Calculate CCI when we have enough data
        if (_buffer.Count == LookbackPeriods)
        {
            // Calculate average TP over lookback
            double avgTp = _buffer.Average();

            // Calculate average Deviation over lookback
            double avgDv = 0;
            foreach (double tpValue in _buffer)
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
        _buffer.Clear();
    }
}

public static partial class Cci
{
    /// <summary>
    /// Creates a buffer list for Commodity Channel Index (CCI).
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public static CciList ToCciList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 20)
        => new(lookbackPeriods) { quotes };
}
