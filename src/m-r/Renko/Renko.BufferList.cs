namespace Skender.Stock.Indicators;

/// <summary>
/// Renko Chart from incremental quote values.
/// </summary>
public class RenkoList : BufferList<RenkoResult>, IIncrementFromQuote, IRenko
{
    /// <summary>
    /// State tracking
    /// </summary>
    private RenkoResult _lastBrick;
    private decimal _h = decimal.MinValue;
    private decimal _l = decimal.MaxValue;
    private decimal _sumV;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenkoList"/> class.
    /// </summary>
    /// <param name="brickSize">The size of each Renko brick.</param>
    /// <param name="endType">The price candle end type to use as the brick threshold.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="endType"/> is invalid.</exception>
    public RenkoList(
        decimal brickSize,
        EndType endType = EndType.Close
    )
    {
        Renko.Validate(brickSize);
        BrickSize = brickSize;
        EndType = endType;

        _lastBrick = new(
            default,
            Open: default,
            High: default,
            Low: default,
            Close: default,
            Volume: default,
            IsUp: false);

        Name = $"RENKO({brickSize}, {EndType.Close})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RenkoList"/> class with initial quotes.
    /// </summary>
    /// <param name="brickSize">The size of each Renko brick.</param>
    /// <param name="endType">The price candle end type to use as the brick threshold.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public RenkoList(
        decimal brickSize,
        EndType endType,
        IReadOnlyList<IQuote> quotes
    )
        : this(brickSize, endType) => Add(quotes);

    /// <inheritdoc/>
    public decimal BrickSize { get; init; }

    /// <inheritdoc/>
    public EndType EndType { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Initialize baseline brick on first quote
        if (!_isInitialized)
        {
            int decimals = BrickSize.GetDecimalPlaces();
            decimal baseline = Math.Round(quote.Close, Math.Max(decimals - 1, 0));

            _lastBrick = new(
                quote.Timestamp,
                Open: baseline,
                High: 0,
                Low: 0,
                Close: baseline,
                Volume: 0,
                IsUp: false);

            _isInitialized = true;
            return;
        }

        // Track high/low/volume between bricks
        _h = Math.Max(_h, quote.High);
        _l = Math.Min(_l, quote.Low);
        _sumV += quote.Volume;

        // Determine new brick quantity
        int newBrickQty = Renko.GetNewBrickQuantity(quote, _lastBrick, BrickSize, EndType);
        int absBrickQty = Math.Abs(newBrickQty);
        bool isUp = newBrickQty >= 0;

        // Add new brick(s) - can add more than one brick!
        for (int b = 0; b < absBrickQty; b++)
        {
            decimal o;
            decimal c;
            decimal v = _sumV / absBrickQty;

            if (isUp)
            {
                o = Math.Max(_lastBrick.Open, _lastBrick.Close);
                c = o + BrickSize;
            }
            else
            {
                o = Math.Min(_lastBrick.Open, _lastBrick.Close);
                c = o - BrickSize;
            }

            RenkoResult r = new(quote.Timestamp, o, _h, _l, c, v, isUp);

            AddInternal(r);
            _lastBrick = r;
        }

        // Reset high/low/volume tracking
        if (absBrickQty != 0)
        {
            _h = decimal.MinValue;
            _l = decimal.MaxValue;
            _sumV = 0;
        }
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
        _lastBrick = new(
            default,
            Open: default,
            High: default,
            Low: default,
            Close: default,
            Volume: default,
            IsUp: false);
        _h = decimal.MinValue;
        _l = decimal.MaxValue;
        _sumV = 0;
        _isInitialized = false;
    }
}

public static partial class Renko
{
    /// <summary>
    /// Creates a buffer list for Renko Chart calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="brickSize">The size of each Renko brick.</param>
    /// <param name="endType">The price candle end type to use as the brick threshold.</param>
    /// <returns>A buffer list for Renko Chart calculations.</returns>
    public static RenkoList ToRenkoList(
        this IReadOnlyList<IQuote> quotes,
        decimal brickSize,
        EndType endType = EndType.Close)
        => new(brickSize, endType) { quotes };
}
