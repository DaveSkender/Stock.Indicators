namespace Skender.Stock.Indicators;

/// <summary>
/// ATR Trailing Stop from incremental quote values.
/// </summary>
public class AtrStopList : BufferList<AtrStopResult>, IIncrementFromQuote, IAtrStop
{
    private readonly AtrList _atrList;
    private bool _isBullish;
    private double _upperBand;
    private double _lowerBand;
    private double _previousClose;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtrStopList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <param name="endType">The candle threshold point to use for reversals.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="endType"/> is invalid.</exception>
    public AtrStopList(
        int lookbackPeriods = 21,
        double multiplier = 3,
        EndType endType = EndType.Close)
    {
        AtrStop.Validate(lookbackPeriods, multiplier);
        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;
        EndType = endType;

        _atrList = new AtrList(lookbackPeriods);
        _isBullish = true;
        _upperBand = double.MaxValue;
        _lowerBand = double.MinValue;
        _isInitialized = false;

        Name = $"ATRSTOP({21}, {3}, {EndType.Close})";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtrStopList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <param name="endType">The candle threshold point to use for reversals.</param>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    public AtrStopList(
        int lookbackPeriods,
        double multiplier,
        EndType endType,
        IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods, multiplier, endType) => Add(quotes);

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double Multiplier { get; init; }

    /// <inheritdoc/>
    public EndType EndType { get; init; }

    /// <summary>
    /// Adds a new quote to the AtrStop list.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quote is null.</exception>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        // Add to ATR list to get ATR calculation
        _atrList.Add(quote);
        AtrResult atrResult = _atrList[^1];

        // Handle warmup periods - need LookbackPeriods values for initial ATR
        if (_atrList.Count < LookbackPeriods + 1)
        {
            _previousClose = (double)quote.Close;
            AddInternal(new AtrStopResult(Timestamp: quote.Timestamp));
            return;
        }

        double close = (double)quote.Close;
        double high = (double)quote.High;
        double low = (double)quote.Low;

        // Initialize direction on first evaluation
        if (!_isInitialized)
        {
            _isBullish = close >= _previousClose;
            _isInitialized = true;
        }

        double atr = atrResult.Atr ?? double.NaN;

        // Calculate potential bands
        double upperEval;
        double lowerEval;

        if (EndType == EndType.Close)
        {
            upperEval = close + (Multiplier * atr);
            lowerEval = close - (Multiplier * atr);
        }
        else
        {
            upperEval = high + (Multiplier * atr);
            lowerEval = low - (Multiplier * atr);
        }

        // New upper band: can only go down, or reverse
        if (upperEval < _upperBand || _previousClose > _upperBand)
        {
            _upperBand = upperEval;
        }

        // New lower band: can only go up, or reverse
        if (lowerEval > _lowerBand || _previousClose < _lowerBand)
        {
            _lowerBand = lowerEval;
        }

        // Determine stop based on direction
        AtrStopResult result;

        // Upper band (short / buy-to-stop)
        if (close <= (_isBullish ? _lowerBand : _upperBand))
        {
            _isBullish = false;

            result = new AtrStopResult(
                Timestamp: quote.Timestamp,
                AtrStop: _upperBand,
                BuyStop: _upperBand,
                SellStop: null,
                Atr: atr);
        }
        // Lower band (long / sell-to-stop)
        else
        {
            _isBullish = true;

            result = new AtrStopResult(
                Timestamp: quote.Timestamp,
                AtrStop: _lowerBand,
                BuyStop: null,
                SellStop: _lowerBand,
                Atr: atr);
        }

        _previousClose = close;
        AddInternal(result);
    }

    /// <summary>
    /// Adds a list of quotes to the AtrStop list.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i]);
        }
    }

    /// <summary>
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        _atrList.Clear();
        _isBullish = true;
        _upperBand = double.MaxValue;
        _lowerBand = double.MinValue;
        _previousClose = 0;
        _isInitialized = false;
    }
    /// <summary>
    /// Removes oldest results from both the outer list and the nested ATR list
    /// when the list exceeds <see cref="BufferList{TResult}.MaxListSize"/>.
    /// This prevents unbounded growth of the auxiliary ATR cache.
    /// </summary>
    protected override void PruneList()
    {
        // Synchronize the nested ATR list's MaxListSize with the outer list
        // Keep enough ATR history to support calculations (LookbackPeriods + 1)
        // plus room for the current MaxListSize
        int minAtrSize = LookbackPeriods + 1;
        _atrList.MaxListSize = Math.Max(minAtrSize, MaxListSize + 1);

        // Call base implementation to prune the outer result list
        base.PruneList();
    }
}

/// <summary>
/// EXTENSION METHODS
/// </summary>
public static partial class AtrStop
{
    /// <summary>
    /// Creates a buffer list for ATR Trailing Stop calculations.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="multiplier">The multiplier for the ATR.</param>
    /// <param name="endType">The candle threshold point to use for reversals.</param>
    /// <returns>An AtrStopList instance pre-populated with historical data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when quotes is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static AtrStopList ToAtrStopList(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 21,
        double multiplier = 3,
        EndType endType = EndType.Close)
        => new(lookbackPeriods, multiplier, endType) { quotes };
}
