namespace Skender.Stock.Indicators;

/// <summary>
/// True Range (TR) from incremental quotes.
/// </summary>
public class TrList : BufferList<TrResult>, IBufferList
{
    private readonly Queue<TrBuffer> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrList"/> class.
    /// </summary>
    public TrList()
    {
        _buffer = new Queue<TrBuffer>(2); // Only need current and previous
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrList"/> class with initial quotes.
    /// </summary>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public TrList(IReadOnlyList<IQuote> quotes)
        : this()
        => Add(quotes);

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        DateTime timestamp = quote.Timestamp;

        TrBuffer curr = new(
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
        TrBuffer prev = _buffer.Last();
        _buffer.Update(2, curr);

        // calculate True Range
        double tr = Tr.Increment(curr.High, curr.Low, prev.Close);

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
        ClearInternal();
        _buffer.Clear();
    }

    /// <summary>
    /// Represents a buffer for True Range calculations.
    /// </summary>
    private readonly struct TrBuffer(double high, double low, double close)
    {
        public double High { get; init; } = high;
        public double Low { get; init; } = low;
        public double Close { get; init; } = close;
    }
}

public static partial class Tr
{
    /// <summary>
    /// Creates a buffer list for True Range calculations.
    /// </summary>
    public static TrList ToTrList<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote
        => new() { (IReadOnlyList<IQuote>)quotes };
}
