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
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public MfiList(int lookbackPeriods = 14)
    {
        Mfi.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _buffer = new Queue<(double, double, int)>(lookbackPeriods);
        _quotesProcessed = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MfiList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public MfiList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
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
        int direction;
        if (_prevTruePrice == null || truePrice == _prevTruePrice)
        {
            direction = 0;
        }
        else if (truePrice > _prevTruePrice)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

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
    public static MfiList ToMfiList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 14)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
