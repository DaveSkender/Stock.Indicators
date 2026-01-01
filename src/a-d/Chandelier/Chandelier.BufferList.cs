namespace Skender.Stock.Indicators;

/// <summary>
/// Chandelier Exit from incremental quotes.
/// </summary>
public class ChandelierList : BufferList<ChandelierResult>, IIncrementFromQuote, IChandelier
{
    private readonly AtrList _atrList;
    private readonly Queue<(double High, double Low)> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChandelierList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="type"/> is invalid.</exception>
    public ChandelierList(int lookbackPeriods = 22, double multiplier = 3, Direction type = Direction.Long)
    {
        Chandelier.Validate(lookbackPeriods, multiplier);
        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;
        Type = type;

        _atrList = new AtrList(lookbackPeriods);
        _buffer = new Queue<(double, double)>(lookbackPeriods);

        Name = $"CHANDELIER({22}, {3}, {Direction.Long})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChandelierList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier to apply to the ATR.</param>
    /// <param name="type">The type of Chandelier Exit to calculate (Long or Short).</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public ChandelierList(int lookbackPeriods, double multiplier, Direction type, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods, multiplier, type) => Add(quotes);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public double Multiplier { get; init; }

    /// <inheritdoc />
    public Direction Type { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;
        double high = (double)quote.High;
        double low = (double)quote.Low;

        // Calculate ATR
        _atrList.Add(quote);
        AtrResult atrResult = _atrList[^1];

        // Update buffer with consolidated tuple
        _buffer.Update(LookbackPeriods, (high, low));

        double? exit = null;

        // Calculate exit when we have enough data
        if (_buffer.Count == LookbackPeriods && atrResult.Atr.HasValue)
        {
            double atr = atrResult.Atr.Value;

            switch (Type)
            {
                case Direction.Long:
                    double maxHigh = double.MinValue;
                    foreach ((double High, double Low) in _buffer)
                    {
                        if (High > maxHigh)
                        {
                            maxHigh = High;
                        }
                    }

                    exit = maxHigh - (atr * Multiplier);
                    break;

                case Direction.Short:
                    double minLow = double.MaxValue;
                    foreach ((double High, double Low) in _buffer)
                    {
                        if (Low < minLow)
                        {
                            minLow = Low;
                        }
                    }

                    exit = minLow + (atr * Multiplier);
                    break;

                default:
                    throw new InvalidOperationException($"Unknown direction type: {Type}");
            }
        }

        AddInternal(new ChandelierResult(timestamp, exit));
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
        _atrList.Clear();
        _buffer.Clear();
    }
}

public static partial class Chandelier
{
    /// <summary>
    /// Creates a buffer list for Chandelier Exit calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">Multiplier for calculation</param>
    /// <param name="type">Chandelier type</param>
    public static ChandelierList ToChandelierList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 22,
        double multiplier = 3,
        Direction type = Direction.Long)
        => new(lookbackPeriods, multiplier, type) { quotes };
}
