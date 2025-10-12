namespace Skender.Stock.Indicators;

/// <summary>
/// Money Flow Index (MFI) from incremental quotes.
/// </summary>
public class MfiList : BufferList<MfiResult>, IIncrementFromQuote
{
    private readonly Queue<(double TruePrice, double MoneyFlow, int Direction)> _buffer;
    private double? _prevTruePrice;

    /// <summary>
    /// Initializes a new instance of the <see cref="MfiList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public MfiList(int lookbackPeriods)
    {
        Mfi.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _buffer = new Queue<(double, double, int)>(lookbackPeriods);
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

        // Calculate true price (HLC/3)
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
        else // truePrice < _prevTruePrice
        {
            direction = -1;
        }

        // Update buffer
        _buffer.Update(LookbackPeriods, (truePrice, moneyFlow, direction));

        double? mfi = null;

        // Calculate MFI when we have enough data
        // We need lookbackPeriods + 1 total values because the first value establishes baseline direction
        if (Count >= LookbackPeriods && _buffer.Count == LookbackPeriods)
        {
            double sumPosMFs = 0;
            double sumNegMFs = 0;

            foreach ((double TruePrice, double MoneyFlow, int Direction) item in _buffer)
            {
                if (item.Direction == 1)
                {
                    sumPosMFs += item.MoneyFlow;
                }
                else if (item.Direction == -1)
                {
                    sumNegMFs += item.MoneyFlow;
                }
            }

            // Calculate MFI normally
            if (sumNegMFs != 0)
            {
                double mfRatio = sumPosMFs / sumNegMFs;
                mfi = 100 - (100 / (1 + mfRatio));
            }
            // Handle no negative case
            else
            {
                mfi = 100;
            }
        }

        AddInternal(new MfiResult(
            Timestamp: quote.Timestamp,
            Mfi: mfi));

        _prevTruePrice = truePrice;
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
