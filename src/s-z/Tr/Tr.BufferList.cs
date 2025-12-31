namespace Skender.Stock.Indicators;

/// <summary>
/// True Range (TR) from incremental quotes.
/// </summary>
public class TrList : BufferList<TrResult>, IIncrementFromQuote  // TR has no interface members
{
    private readonly Queue<(double High, double Low, double Close)> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrList"/> class.
    /// </summary>
    public TrList() => _buffer = new Queue<(double, double, double)>(2); // Only need current and previous

    /// <summary>
    /// Initializes a new instance of the <see cref="TrList"/> class with initial quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public TrList(IReadOnlyList<IQuote> quotes)
        : this() => Add(quotes);

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;

        (double High, double Low, double Close) curr = (
            (double)quote.High,
            (double)quote.Low,
            (double)quote.Close);

        // skip first period
        if (Count == 0)
        {
            _buffer.Update(2, curr);
            AddInternal(new TrResult(timestamp, null));
            return;
        }

        // get previous, then add current using extension method
        (double _, double _, double PrevClose) = _buffer.Last();
        _buffer.Update(2, curr);

        // calculate True Range
        double tr = Tr.Increment(curr.High, curr.Low, PrevClose);

        AddInternal(new TrResult(timestamp, tr));
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

public static partial class Tr
{
    /// <summary>
    /// Creates a buffer list for True Range calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public static TrList ToTrList(
        this IReadOnlyList<IQuote> quotes)
        => new() { quotes };
}
