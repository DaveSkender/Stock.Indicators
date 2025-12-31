namespace Skender.Stock.Indicators;

/// <summary>
/// Elder Ray from incremental quote values.
/// </summary>
public class ElderRayList : BufferList<ElderRayResult>, IIncrementFromQuote, IElderRay
{
    private readonly EmaList _emaList;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElderRayList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="lookbackPeriods"/> is invalid.</exception>
    public ElderRayList(
        int lookbackPeriods = 13
    )
    {
        ElderRay.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        _emaList = new EmaList(lookbackPeriods);

        Name = $"ELDERRAY({lookbackPeriods})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElderRayList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public ElderRayList(
        int lookbackPeriods,
        IReadOnlyList<IQuote> quotes
    )
        : this(lookbackPeriods) => Add(quotes);

    /// <inheritdoc />
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // update EMA
        _emaList.Add(quote);
        EmaResult emaResult = _emaList[^1];

        // calculate Elder Ray
        AddInternal(new ElderRayResult(
            Timestamp: quote.Timestamp,
            Ema: emaResult.Ema,
            BullPower: (double)quote.High - emaResult.Ema,
            BearPower: (double)quote.Low - emaResult.Ema));
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
        _emaList.Clear();
    }
}

public static partial class ElderRay
{
    /// <summary>
    /// Creates a buffer list for Elder Ray calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    public static ElderRayList ToElderRayList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 13)
        => new(lookbackPeriods) { quotes };
}
