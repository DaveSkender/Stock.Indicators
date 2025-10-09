namespace Skender.Stock.Indicators;

/// <summary>
/// Beta coefficient from incremental reusable values.
/// </summary>
public class BetaList : BufferList<BetaResult>
{
    private readonly Queue<(DateTime Timestamp, double EvalValue, double MrktValue, double EvalReturn, double MrktReturn)> _buffer;
    private double _prevEval;
    private double _prevMrkt;
    private bool _isFirst = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <param name="type">The type of Beta calculation. Default is <see cref="BetaType.Standard"/>.</param>
    public BetaList(int lookbackPeriods = 50, BetaType type = BetaType.Standard)
    {
        Beta.Validate<ISeries>([], [], lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        Type = type;

        _buffer = new Queue<(DateTime, double, double, double, double)>(lookbackPeriods + 1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaList"/> class with initial values.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <param name="type">The type of Beta calculation.</param>
    /// <param name="sourceEval">Initial evaluated asset values to populate the list.</param>
    /// <param name="sourceMrkt">Initial market values to populate the list.</param>
    public BetaList(
        int lookbackPeriods,
        BetaType type,
        IReadOnlyList<IReusable> sourceEval,
        IReadOnlyList<IReusable> sourceMrkt)
        : this(lookbackPeriods, type)
        => Add(sourceEval, sourceMrkt);

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
    /// <param name="evalValue">The evaluated asset value.</param>
    /// <param name="mrktValue">The market value.</param>
    public void Add(DateTime timestamp, double evalValue, double mrktValue)
    {
        // Calculate returns
        double evalReturn = _isFirst ? 0 : (evalValue / _prevEval) - 1d;
        double mrktReturn = _isFirst ? 0 : (mrktValue / _prevMrkt) - 1d;

        _prevEval = evalValue;
        _prevMrkt = mrktValue;
        _isFirst = false;

        // Add to buffer using Update utility
        _buffer.Update(LookbackPeriods + 1, (timestamp, evalValue, mrktValue, evalReturn, mrktReturn));

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
            ReturnsEval: evalReturn,
            ReturnsMrkt: mrktReturn,
            Beta: beta,
            BetaUp: betaUp,
            BetaDown: betaDown,
            Ratio: ratio,
            Convexity: convexity
        ));
    }

    /// <summary>
    /// Adds a new pair of reusable values to the Beta list.
    /// </summary>
    /// <param name="eval">The evaluated asset value.</param>
    /// <param name="mrkt">The market value.</param>
    /// <exception cref="ArgumentNullException">Thrown when eval or mrkt is null.</exception>
    /// <exception cref="InvalidQuotesException">Thrown when timestamps do not match.</exception>
    public void Add(IReusable eval, IReusable mrkt)
    {
        ArgumentNullException.ThrowIfNull(eval);
        ArgumentNullException.ThrowIfNull(mrkt);

        if (eval.Timestamp != mrkt.Timestamp)
        {
            throw new InvalidQuotesException(
                nameof(eval), eval.Timestamp,
                "Timestamp sequence does not match.  " +
                "Beta requires matching dates in provided quotes.");
        }

        Add(eval.Timestamp, eval.Value, mrkt.Value);
    }

    /// <summary>
    /// Adds lists of reusable values to the Beta list.
    /// </summary>
    /// <param name="sourceEval">The list of evaluated asset values to add.</param>
    /// <param name="sourceMrkt">The list of market values to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when sourceEval or sourceMrkt is null.</exception>
    /// <exception cref="InvalidQuotesException">Thrown when lists have different counts.</exception>
    public void Add(IReadOnlyList<IReusable> sourceEval, IReadOnlyList<IReusable> sourceMrkt)
    {
        ArgumentNullException.ThrowIfNull(sourceEval);
        ArgumentNullException.ThrowIfNull(sourceMrkt);

        if (sourceEval.Count != sourceMrkt.Count)
        {
            throw new InvalidQuotesException(
                nameof(sourceEval),
                "Eval quotes should have the same number of Market quotes for Beta.");
        }

        for (int i = 0; i < sourceEval.Count; i++)
        {
            Add(sourceEval[i], sourceMrkt[i]);
        }
    }

    /// <summary>
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public override void Clear()
    {
        ClearInternal();
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

        if (dataA.Count <= 0)
        {
            return beta;
        }

        // Calculate correlation, covariance, and variance
        CorrResult c = Correlation.PeriodCorrelation(
            default,
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

// EXTENSION METHODS
public static partial class Beta
{
    /// <summary>
    /// Creates a buffer list for Beta calculations.
    /// </summary>
    /// <typeparam name="T">The type that implements IReusable.</typeparam>
    /// <param name="sourceEval">The evaluated asset values.</param>
    /// <param name="sourceMrkt">The market values.</param>
    /// <param name="lookbackPeriods">The number of periods to look back. Default is 50.</param>
    /// <param name="type">The type of Beta calculation. Default is <see cref="BetaType.Standard"/>.</param>
    /// <returns>A BetaList instance pre-populated with historical data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when sourceEval or sourceMrkt is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static BetaList ToBetaList<T>(
        this IReadOnlyList<T> sourceEval,
        IReadOnlyList<T> sourceMrkt,
        int lookbackPeriods = 50,
        BetaType type = BetaType.Standard)
        where T : IReusable
    {
        ArgumentNullException.ThrowIfNull(sourceEval);
        ArgumentNullException.ThrowIfNull(sourceMrkt);

        Beta.Validate(sourceEval, sourceMrkt, lookbackPeriods);

        return new(
            lookbackPeriods,
            type,
            (IReadOnlyList<IReusable>)sourceEval,
            (IReadOnlyList<IReusable>)sourceMrkt);
    }
}
