namespace Skender.Stock.Indicators;

/// <summary>
/// Fractal Chaos Bands (FCB) from incremental quote values.
/// </summary>
public class FcbList : BufferList<FcbResult>, IBufferList
{
    private readonly Queue<Quote> _quoteBuffer;
    private readonly int _windowSpan;
    private decimal? _upperLine;
    private decimal? _lowerLine;

    /// <summary>
    /// Initializes a new instance of the <see cref="FcbList"/> class.
    /// </summary>
    /// <param name="windowSpan">The window span for the calculation. Default is 2.</param>
    public FcbList(
        int windowSpan = 2
    )
    {
        Fcb.Validate(windowSpan);
        WindowSpan = windowSpan;

        _windowSpan = windowSpan;
        _quoteBuffer = new Queue<Quote>(2 * windowSpan + 1);
        _upperLine = null;
        _lowerLine = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FcbList"/> class with initial quotes.
    /// </summary>
    /// <param name="windowSpan">The window span for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public FcbList(
        int windowSpan,
        IReadOnlyList<IQuote> quotes
    )
        : this(windowSpan)
        => Add(quotes);

    /// <summary>
    /// Gets the window span for the calculation.
    /// </summary>
    public int WindowSpan { get; init; }


    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        Quote q = new()
        {
            Timestamp = quote.Timestamp,
            Open = quote.Open,
            High = quote.High,
            Low = quote.Low,
            Close = quote.Close,
            Volume = quote.Volume
        };

        // maintain buffer of quotes needed for fractal calculation
        _quoteBuffer.Update(2 * _windowSpan + 1, q);

        // check if we can identify fractals
        if (_quoteBuffer.Count >= 2 * _windowSpan + 1)
        {
            // get quotes as list for fractal detection
            List<Quote> bufferList = _quoteBuffer.ToList();
            int midIndex = _windowSpan;
            Quote midQuote = bufferList[midIndex];

            // check for bearish fractal (high point)
            bool isBearishFractal = true;
            for (int i = 0; i < bufferList.Count; i++)
            {
                if (i != midIndex && bufferList[i].High >= midQuote.High)
                {
                    isBearishFractal = false;
                    break;
                }
            }

            // check for bullish fractal (low point)
            bool isBullishFractal = true;
            for (int i = 0; i < bufferList.Count; i++)
            {
                if (i != midIndex && bufferList[i].Low <= midQuote.Low)
                {
                    isBullishFractal = false;
                    break;
                }
            }

            // update lines based on fractals detected windowSpan periods ago
            if (Count >= _windowSpan)
            {
                if (isBearishFractal)
                {
                    _upperLine = midQuote.High;
                }
                if (isBullishFractal)
                {
                    _lowerLine = midQuote.Low;
                }
            }
        }

        // add result
        AddInternal(new FcbResult(
            Timestamp: quote.Timestamp,
            UpperBand: _upperLine,
            LowerBand: _lowerLine));
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
        _quoteBuffer.Clear();
        _upperLine = null;
        _lowerLine = null;
    }
}

public static partial class Fcb
{
    /// <summary>
    /// Creates a buffer list for Fractal Chaos Bands (FCB) calculations.
    /// </summary>
    public static FcbList ToFcbList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int windowSpan = 2)
        where TQuote : IQuote
        => new(windowSpan) { (IReadOnlyList<IQuote>)quotes };
}
