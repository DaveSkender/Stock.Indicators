namespace Skender.Stock.Indicators;

/// <summary>
/// True Range (TR) from incremental quotes.
/// </summary>
public static partial class Tr
{
    /// <summary>
    /// Creates a buffer list for True Range calculations.
    /// </summary>
    public static TrList ToTrBufferList<TQuote>(
        this IReadOnlyList<TQuote> quotes)
        where TQuote : IQuote
    {
        // Input validation
        ArgumentNullException.ThrowIfNull(quotes);

        // Initialize buffer and populate
        TrList bufferList = new();
        
        foreach (TQuote quote in quotes)
        {
            bufferList.Add(quote);
        }
        
        return bufferList;
    }
}

/// <summary>
/// True Range (TR) from incremental quotes.
/// </summary>
public class TrList : List<TrResult>, IBufferList
{
    private readonly Queue<TrBuffer> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrList"/> class.
    /// </summary>
    public TrList()
    {
        _buffer = new Queue<TrBuffer>(2); // Only need current and previous
    }

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
            base.Add(new TrResult(timestamp, null));
            return;
        }

        // get previous, then add current using extension method
        TrBuffer prev = _buffer.Last();
        _buffer.Update(2, curr);

        // calculate True Range
        double tr = Tr.Increment(curr.High, curr.Low, prev.Close);

        base.Add(new TrResult(timestamp, tr));
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
    public new void Clear()
    {
        base.Clear();
        _buffer.Clear();
    }

    /// <summary>
    /// Represents a buffer for True Range calculations.
    /// </summary>
    private readonly struct TrBuffer
    {
        public double High { get; init; }
        public double Low { get; init; }
        public double Close { get; init; }

        public TrBuffer(double high, double low, double close)
        {
            High = high;
            Low = low;
            Close = close;
        }
    }
}