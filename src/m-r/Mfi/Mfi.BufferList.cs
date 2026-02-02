namespace Skender.Stock.Indicators;

/// <summary>
/// Money Flow Index (MFI) from incremental quotes.
/// </summary>
public class MfiList : BufferList<MfiResult>, IIncrementFromQuote
{
    private readonly Queue<(double TruePrice, double MoneyFlow, int Direction)> _buffer;
    private double? _prevTruePrice;
    private int _quotesProcessed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MfiList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public MfiList(int lookbackPeriods = 14)
    {
        Mfi.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _buffer = new Queue<(double, double, int)>(lookbackPeriods);
        _quotesProcessed = 0;

        Name = $"MFI({lookbackPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MfiList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public MfiList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods) => Add(quotes);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;

        // Calculate true price
        double truePrice = ((double)quote.High + (double)quote.Low + (double)quote.Close) / 3;

        // Calculate raw money flow
        double moneyFlow = truePrice * (double)quote.Volume;

        // Determine direction
        int direction = _prevTruePrice == null || truePrice == _prevTruePrice
            ? 0 :
            truePrice > _prevTruePrice ? 1 : -1;

        // Update buffer
        _buffer.Update(LookbackPeriods, (truePrice, moneyFlow, direction));
        _quotesProcessed++;

        // Calculate MFI when we have enough data
        // Need lookbackPeriods + 1 total quotes (at least one full buffer cycle)
        double? mfi = null;
        if (_quotesProcessed > LookbackPeriods)
        {
            double sumPosMFs = 0;
            double sumNegMFs = 0;

            foreach ((double _, double mf, int dir) in _buffer)
            {
                if (dir == 1)
                {
                    sumPosMFs += mf;
                }
                else if (dir == -1)
                {
                    sumNegMFs += mf;
                }
            }

            // Calculate MFI
            if (sumNegMFs != 0)
            {
                double mfRatio = sumPosMFs / sumNegMFs;
                mfi = 100 - (100 / (1 + mfRatio));
            }
            else
            {
                mfi = 100;
            }
        }

        _prevTruePrice = truePrice;

        AddInternal(new MfiResult(timestamp, mfi));
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
        _prevTruePrice = null;
        _quotesProcessed = 0;
    }
}

public static partial class Mfi
{
    /// <summary>
    /// Creates a buffer list for Money Flow Index (MFI) calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    public static MfiList ToMfiList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 14)
        => new(lookbackPeriods) { quotes };
}
