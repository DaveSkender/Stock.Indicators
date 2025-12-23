namespace Skender.Stock.Indicators;

/// <summary>
/// Beta coefficient from incremental reusable values.
/// </summary>
public class BetaList : BufferList<BetaResult>, IIncrementFromPairs, IBeta
{
    private readonly Queue<(DateTime Timestamp, double EvalValue, double MrktValue, double EvalReturn, double MrktReturn)> _buffer;
    private double _prevEval;
    private double _prevMrkt;
    private bool _isFirst = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="type">The type of Beta calculation. Default is <see cref="BetaType.Standard"/>.</param>
    public BetaList(int lookbackPeriods = 50, BetaType type = BetaType.Standard)
    {
        Beta.Validate<IReusable>([], [], lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        Type = type;

        _buffer = new Queue<(DateTime, double, double, double, double)>(lookbackPeriods + 1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaList"/> class with initial values.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="type">The type of Beta calculation.</param>
    /// <param name="sourceEval">Initial evaluated asset values to populate the list.</param>
    /// <param name="sourceMrkt">Initial market values to populate the list.</param>
    public BetaList(
        int lookbackPeriods,
        BetaType type,
        IReadOnlyList<IReusable> sourceEval,
        IReadOnlyList<IReusable> sourceMrkt)
        : this(lookbackPeriods, type) => Add(sourceEval, sourceMrkt);

    /// <summary>
    /// Gets the number of periods to look back.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the type of Beta calculation.
    /// </summary>
    public BetaType Type { get; init; }

    /// <summary>
    /// Adds a new pair of values to the Beta list.
    /// </summary>
    /// <param name="timestamp">The timestamp for both values.</param>
    /// <param name="valueA">The evaluated asset value.</param>
    /// <param name="valueB">The market value.</param>
    public void Add(DateTime timestamp, double valueA, double valueB)
    {
        // Calculate returns with division-by-zero guard
        double evalReturn = _isFirst || _prevEval == 0 ? 0 : (valueA / _prevEval) - 1d;
        double mrktReturn = _isFirst || _prevMrkt == 0 ? 0 : (valueB / _prevMrkt) - 1d;

        _prevEval = valueA;
        _prevMrkt = valueB;
        _isFirst = false;

        // Add to buffer using Update utility
        _buffer.Update(LookbackPeriods + 1, (timestamp, valueA, valueB, evalReturn, mrktReturn));

        // Calculate results
        double? beta = null;
        double? betaUp = null;
        double? betaDown = null;
        double? ratio = null;
        double? convexity = null;

        // Need at least lookbackPeriods + 1 values to calculate
        if (_buffer.Count > LookbackPeriods)
        {
            bool calcSd = Type is BetaType.All or BetaType.Standard;
            bool calcUp = Type is BetaType.All or BetaType.Up;
            bool calcDn = Type is BetaType.All or BetaType.Down;

            // Calculate beta variants using returns (skip first return which is 0)
            if (calcSd)
            {
                beta = CalcBetaWindow(BetaType.Standard);
            }

            if (calcDn)
            {
                betaDown = CalcBetaWindow(BetaType.Down);
            }

            if (calcUp)
            {
                betaUp = CalcBetaWindow(BetaType.Up);
            }

            // Ratio and convexity
            if (Type == BetaType.All && betaUp != null && betaDown != null)
            {
                ratio = betaDown != 0 ? betaUp / betaDown : null;
                convexity = (betaUp - betaDown) * (betaUp - betaDown);
            }
        }

        AddInternal(new BetaResult(
            Timestamp: timestamp,
            Beta: beta,
            BetaUp: betaUp,
            BetaDown: betaDown,
            Ratio: ratio,
            Convexity: convexity,
            ReturnsEval: evalReturn,
            ReturnsMrkt: mrktReturn));
    }

    /// <summary>
    /// Adds a new pair of reusable values to the Beta list.
    /// </summary>
    /// <param name="valueA">The evaluated asset value.</param>
    /// <param name="valueB">The market value.</param>
    /// <exception cref="ArgumentNullException">Thrown when valueA or valueB is null.</exception>
    /// <exception cref="InvalidQuotesException">Thrown when timestamps do not match.</exception>
    public void Add(IReusable valueA, IReusable valueB)
    {
        ArgumentNullException.ThrowIfNull(valueA);
        ArgumentNullException.ThrowIfNull(valueB);

        if (valueA.Timestamp != valueB.Timestamp)
        {
            throw new InvalidQuotesException(
                nameof(valueA), valueA.Timestamp,
                "Timestamp sequence does not match.  " +
                "Beta requires matching dates in provided quotes.");
        }

        Add(valueA.Timestamp, valueA.Value, valueB.Value);
    }

    /// <summary>
    /// Adds lists of reusable values to the Beta list.
    /// </summary>
    /// <param name="valuesA">The list of evaluated asset values to add.</param>
    /// <param name="valuesB">The list of market values to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when valuesA or valuesB is null.</exception>
    /// <exception cref="InvalidQuotesException">Thrown when lists have different counts.</exception>
    public void Add(IReadOnlyList<IReusable> valuesA, IReadOnlyList<IReusable> valuesB)
    {
        ArgumentNullException.ThrowIfNull(valuesA);
        ArgumentNullException.ThrowIfNull(valuesB);

        if (valuesA.Count != valuesB.Count)
        {
            throw new InvalidQuotesException(
                nameof(valuesA),
                "Eval quotes should have the same number of Market quotes for Beta.");
        }

        for (int i = 0; i < valuesA.Count; i++)
        {
            Add(valuesA[i], valuesB[i]);
        }
    }

    /// <summary>
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        _buffer.Clear();
        _prevEval = 0;
        _prevMrkt = 0;
        _isFirst = true;
    }

    /// <summary>
    /// Calculates the Beta value for the current window of data.
    /// </summary>
    /// <param name="type">The type of Beta calculation.</param>
    /// <returns>The calculated Beta value.</returns>
    private double? CalcBetaWindow(BetaType type)
    {
        // note: BetaType.All is ineligible for this method

        // Initialize
        double? beta = null;
        List<double> dataA = new(LookbackPeriods);
        List<double> dataB = new(LookbackPeriods);

        // Use returns from index 1 onwards (skip first which is 0)
        int index = 0;
        foreach ((DateTime _, double _, double _, double evalReturn, double mrktReturn) in _buffer)
        {
            if (index > 0) // Skip first return which is 0
            {
                if (type is BetaType.Standard
                || (type is BetaType.Down && mrktReturn < 0)
                || (type is BetaType.Up && mrktReturn > 0))
                {
                    dataA.Add(mrktReturn);
                    dataB.Add(evalReturn);
                }
            }

            index++;
        }

        if (dataA.Count == 0)
        {
            return beta;
        }

        // Calculate correlation, covariance, and variance
        // Use the timestamp from the most recent buffer entry
        DateTime periodTimestamp = _buffer.Last().Timestamp;
        CorrResult c = Correlation.PeriodCorrelation(
            periodTimestamp,
            [.. dataA],
            [.. dataB]);

        // Calculate beta
        if (c.VarianceA != 0)
        {
            beta = (c.Covariance / c.VarianceA).NaN2Null();
        }

        return beta;
    }
}

/// <summary>
/// EXTENSION METHODS
/// </summary>
public static partial class Beta
{
    /// <summary>
    /// Creates a buffer list for Beta calculations.
    /// </summary>
    /// <param name="sourceEval">The evaluated asset values.</param>
    /// <param name="sourceMrkt">The market values.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="type">The type of Beta calculation.</param>
    /// <returns>A BetaList instance pre-populated with historical data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when sourceEval or sourceMrkt is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static BetaList ToBetaList(
        this IReadOnlyList<IReusable> sourceEval,
        IReadOnlyList<IReusable> sourceMrkt,
        int lookbackPeriods = 50,
        BetaType type = BetaType.Standard)
        => new(
            lookbackPeriods,
            type,
            sourceEval,
            sourceMrkt);
}
