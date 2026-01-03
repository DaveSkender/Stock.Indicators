namespace Skender.Stock.Indicators;

/// <summary>
/// TDI-GM from incremental reusable values.
/// </summary>
public class TdiGmList : BufferList<TdiGmResult>, IIncrementFromChain, ITdiGm
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TdiGmList"/> class.
    /// </summary>
    /// <param name="rsiPeriod">The RSI period.</param>
    /// <param name="bandLength">The band length.</param>
    /// <param name="fastLength">The fast length.</param>
    /// <param name="slowLength">The slow length.</param>
    public TdiGmList(
         int rsiPeriod = 21,
        int bandLength = 34,
        int fastLength = 2,
        int slowLength = 7
    )
    {
        TdiGm.Validate(rsiPeriod, bandLength, fastLength, slowLength);
        RsiPeriod = rsiPeriod;
        BandLength = bandLength;
        FastLength = fastLength;
        SlowLength = slowLength;

        Name = $"TdiGm({21}, {34}, {2}, {7})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TdiGmList"/> class with initial reusable values.
    /// </summary>
    /// <param name="rsiPeriod">The RSI period.</param>
    /// <param name="bandLength">The band length.</param>
    /// <param name="fastLength">The fast length.</param>
    /// <param name="slowLength">The slow length.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public TdiGmList(
        int rsiPeriod,
        int bandLength,
        int fastLength,
        int slowLength,
        IReadOnlyList<IReusable> values
    )
        : this(rsiPeriod, bandLength, fastLength, slowLength) => Add(values);

    public int RsiPeriod { get; init; }
    public int BandLength { get; init; }
    public int FastLength { get; init; }
    public int SlowLength { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {

    }

    /// <inheritdoc />
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
    }
}

public static partial class TdiGm
{
    /// <summary>
    /// Creates a buffer list for TDI-GM calculations.
    /// </summary>
    /// <param name="source">The source list of reusable values.</param>
    /// <param name="rsiPeriod">The RSI period.</param>
    /// <param name="bandLength">The band length.</param>
    /// <param name="fastLength">The fast length.</param>
    /// <param name="slowLength">The slow length.</param>
    /// <returns>A buffer list for TDI-GM calculations.</returns>
    public static TdiGmList ToTdiGmList(
        this IReadOnlyList<IReusable> source,
        int rsiPeriod = 21,
        int bandLength = 34,
        int fastLength = 2,
        int slowLength = 7)
        => new(rsiPeriod, bandLength, fastLength, slowLength) { source };
}
