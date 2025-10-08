namespace Skender.Stock.Indicators;

/// <summary>
/// Beta coefficient from incremental reusable values.
/// </summary>
public class BetaList : BufferList<BetaResult>
{
    private readonly Queue<(DateTime Timestamp, double EvalValue, double MrktValue)> _buffer;
    private readonly List<double> _evalReturns;
    private readonly List<double> _mrktReturns;
    private double _prevEval;
    private double _prevMrkt;
    private bool _isFirst = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="BetaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <param name="type">The type of Beta calculation. Default is <see cref="BetaType.Standard"/>.</param>
    public BetaList(int lookbackPeriods = 20, BetaType type = BetaType.Standard)
    {
        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Beta.");
        }

        LookbackPeriods = lookbackPeriods;
        Type = type;

        _buffer = new Queue<(DateTime, double, double)>(lookbackPeriods + 1);
        _evalReturns = new List<double>(lookbackPeriods + 1);
        _mrktReturns = new List<double>(lookbackPeriods + 1);
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
    /// <param name="evalTimestamp">The timestamp of the evaluated asset.</param>
    /// <param name="evalValue">The evaluated asset value.</param>
    /// <param name="mrktTimestamp">The timestamp of the market.</param>
    /// <param name="mrktValue">The market value.</param>
    /// <exception cref="InvalidQuotesException">Thrown when timestamps do not match.</exception>
    public void Add(DateTime evalTimestamp, double evalValue, DateTime mrktTimestamp, double mrktValue)
    {
        if (evalTimestamp != mrktTimestamp)
        {
            throw new InvalidQuotesException(
                nameof(evalTimestamp), evalTimestamp,
                "Timestamp sequence does not match.  " +
                "Beta requires matching dates in provided quotes.");
        }

        // Calculate returns
        double evalReturn = _isFirst ? 0 : (evalValue / _prevEval) - 1d;
        double mrktReturn = _isFirst ? 0 : (mrktValue / _prevMrkt) - 1d;

        _prevEval = evalValue;
        _prevMrkt = mrktValue;
        _isFirst = false;

        // Add to buffer
        _buffer.Enqueue((evalTimestamp, evalValue, mrktValue));
        _evalReturns.Add(evalReturn);
        _mrktReturns.Add(mrktReturn);

        // Maintain buffer size
        if (_buffer.Count > LookbackPeriods + 1)
        {
            _buffer.Dequeue();
            _evalReturns.RemoveAt(0);
            _mrktReturns.RemoveAt(0);
        }

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
            Timestamp: evalTimestamp,
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
    public void Add(IReusable eval, IReusable mrkt)
    {
        ArgumentNullException.ThrowIfNull(eval);
        ArgumentNullException.ThrowIfNull(mrkt);
        Add(eval.Timestamp, eval.Value, mrkt.Timestamp, mrkt.Value);
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
        _evalReturns.Clear();
        _mrktReturns.Clear();
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
        for (int p = 1; p < _evalReturns.Count; p++)
        {
            double a = _mrktReturns[p];
            double b = _evalReturns[p];

            if (type is BetaType.Standard
            || (type is BetaType.Down && a < 0)
            || (type is BetaType.Up && a > 0))
            {
                dataA.Add(a);
                dataB.Add(b);
            }
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
    /// <param name="lookbackPeriods">The number of periods to look back. Default is 20.</param>
    /// <param name="type">The type of Beta calculation. Default is <see cref="BetaType.Standard"/>.</param>
    /// <returns>A BetaList instance pre-populated with historical data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when sourceEval or sourceMrkt is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static BetaList ToBetaList<T>(
        this IReadOnlyList<T> sourceEval,
        IReadOnlyList<T> sourceMrkt,
        int lookbackPeriods = 20,
        BetaType type = BetaType.Standard)
        where T : IReusable
    {
        ArgumentNullException.ThrowIfNull(sourceEval);
        ArgumentNullException.ThrowIfNull(sourceMrkt);

        if (lookbackPeriods <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lookbackPeriods), lookbackPeriods,
                "Lookback periods must be greater than 0 for Beta.");
        }

        if (sourceEval.Count != sourceMrkt.Count)
        {
            throw new InvalidQuotesException(
                nameof(sourceEval),
                "Eval quotes should have the same number of Market quotes for Beta.");
        }

        return new(
            lookbackPeriods,
            type,
            (IReadOnlyList<IReusable>)sourceEval,
            (IReadOnlyList<IReusable>)sourceMrkt);
    }
}
