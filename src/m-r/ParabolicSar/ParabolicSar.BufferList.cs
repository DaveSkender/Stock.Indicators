namespace Skender.Stock.Indicators;

/// <summary>
/// Parabolic SAR from incremental quote values.
/// </summary>
public class ParabolicSarList : BufferList<ParabolicSarResult>, IIncrementFromQuote
{
    private readonly double _accelerationStep;
    private readonly double _maxAccelerationFactor;
    private readonly double _initialFactor;

    private double _accelerationFactor;
    private double _extremePoint;
    private double _priorSar;
    private bool _isRising;
    private int _processedCount;
    private readonly Queue<(double High, double Low)> _priorQuotes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParabolicSarList"/> class.
    /// </summary>
    /// <param name="accelerationStep">The acceleration step for the SAR calculation.</param>
    /// <param name="maxAccelerationFactor">The maximum acceleration factor for the SAR calculation.</param>
    /// <param name="initialFactor">The initial acceleration factor for the SAR calculation.</param>
    public ParabolicSarList(
        double accelerationStep = 0.02,
        double maxAccelerationFactor = 0.2,
        double? initialFactor = null)
    {
        double initial = initialFactor ?? accelerationStep;
        ParabolicSar.Validate(accelerationStep, maxAccelerationFactor, initial);

        _accelerationStep = accelerationStep;
        _maxAccelerationFactor = maxAccelerationFactor;
        _initialFactor = initial;

        AccelerationStep = accelerationStep;
        MaxAccelerationFactor = maxAccelerationFactor;
        InitialFactor = initial;

        _priorQuotes = new Queue<(double, double)>(2);
        _processedCount = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParabolicSarList"/> class with initial quotes.
    /// </summary>
    /// <param name="accelerationStep">The acceleration step for the SAR calculation.</param>
    /// <param name="maxAccelerationFactor">The maximum acceleration factor for the SAR calculation.</param>
    /// <param name="initialFactor">The initial acceleration factor for the SAR calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public ParabolicSarList(
        double accelerationStep,
        double maxAccelerationFactor,
        double? initialFactor,
        IReadOnlyList<IQuote> quotes)
        : this(accelerationStep, maxAccelerationFactor, initialFactor)
        => Add(quotes);

    /// <summary>
    /// Gets the acceleration step for the SAR calculation.
    /// </summary>
    public double AccelerationStep { get; init; }

    /// <summary>
    /// Gets the maximum acceleration factor for the SAR calculation.
    /// </summary>
    public double MaxAccelerationFactor { get; init; }

    /// <summary>
    /// Gets the initial acceleration factor for the SAR calculation.
    /// </summary>
    public double InitialFactor { get; init; }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);

        double high = (double)quote.High;
        double low = (double)quote.Low;

        // First quote - initialize but don't calculate SAR
        if (_processedCount == 0)
        {
            _accelerationFactor = _initialFactor;
            _extremePoint = high;
            _priorSar = low;
            _isRising = true;

            AddInternal(new ParabolicSarResult(quote.Timestamp));
            _priorQuotes.Update(2, (high, low));
            _processedCount++;
            return;
        }

        bool? isReversal;
        double psar;

        // Rising trend
        if (_isRising)
        {
            double sar = _priorSar + (_accelerationFactor * (_extremePoint - _priorSar));

            // SAR cannot be higher than last two lows (only when we have >= 2 prior quotes)
            if (_processedCount >= 2)
            {
                double minLastTwo;
                if (_priorQuotes.Count == 2)
                {
                    (double High, double Low) q1 = _priorQuotes.ElementAt(0);
                    (double High, double Low) q2 = _priorQuotes.ElementAt(1);
                    minLastTwo = Math.Min(q1.Low, q2.Low);
                }
                else
                {
                    // Only have 1 prior quote
                    (double High, double Low) q1 = _priorQuotes.ElementAt(0);
                    minLastTwo = q1.Low;
                }
                sar = Math.Min(sar, minLastTwo);
            }

            // Turn down (reversal)
            if (low < sar)
            {
                isReversal = true;
                psar = _extremePoint;

                _isRising = false;
                _accelerationFactor = _initialFactor;
                _extremePoint = low;
            }
            // Continue rising
            else
            {
                isReversal = false;
                psar = sar;

                // New high extreme point
                if (high > _extremePoint)
                {
                    _extremePoint = high;
                    _accelerationFactor = Math.Min(
                        _accelerationFactor + _accelerationStep,
                        _maxAccelerationFactor);
                }
            }
        }
        // Falling trend
        else
        {
            double sar = _priorSar - (_accelerationFactor * (_priorSar - _extremePoint));

            // SAR cannot be lower than last two highs (only when we have >= 2 prior quotes)
            if (_processedCount >= 2)
            {
                double maxLastTwo;
                if (_priorQuotes.Count == 2)
                {
                    (double High, double Low) q1 = _priorQuotes.ElementAt(0);
                    (double High, double Low) q2 = _priorQuotes.ElementAt(1);
                    maxLastTwo = Math.Max(q1.High, q2.High);
                }
                else
                {
                    // Only have 1 prior quote
                    (double High, double Low) q1 = _priorQuotes.ElementAt(0);
                    maxLastTwo = q1.High;
                }
                sar = Math.Max(sar, maxLastTwo);
            }

            // Turn up (reversal)
            if (high > sar)
            {
                isReversal = true;
                psar = _extremePoint;

                _isRising = true;
                _accelerationFactor = _initialFactor;
                _extremePoint = high;
            }
            // Continue falling
            else
            {
                isReversal = false;
                psar = sar;

                // New low extreme point
                if (low < _extremePoint)
                {
                    _extremePoint = low;
                    _accelerationFactor = Math.Min(
                        _accelerationFactor + _accelerationStep,
                        _maxAccelerationFactor);
                }
            }
        }

        _priorSar = psar;
        _priorQuotes.Update(2, (high, low));

        AddInternal(new ParabolicSarResult(
            Timestamp: quote.Timestamp,
            Sar: psar,
            IsReversal: isReversal));

        _processedCount++;
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
        _accelerationFactor = _initialFactor;
        _extremePoint = 0;
        _priorSar = 0;
        _isRising = true;
        _processedCount = 0;
        _priorQuotes.Clear();
    }
}

public static partial class ParabolicSar
{
    /// <summary>
    /// Creates a buffer list for Parabolic SAR calculations.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="accelerationStep">The acceleration step for the SAR calculation.</param>
    /// <param name="maxAccelerationFactor">The maximum acceleration factor for the SAR calculation.</param>
    /// <returns>A new <see cref="ParabolicSarList"/> instance.</returns>
    public static ParabolicSarList ToParabolicSarList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        double accelerationStep = 0.02,
        double maxAccelerationFactor = 0.2)
        where TQuote : IQuote
        => new(accelerationStep, maxAccelerationFactor, accelerationStep)
        {
            (IReadOnlyList<IQuote>)quotes
        };

    /// <summary>
    /// Creates a buffer list for Parabolic SAR calculations with explicit initial factor.
    /// </summary>
    /// <typeparam name="TQuote">The type of the quote.</typeparam>
    /// <param name="quotes">The source list of quotes.</param>
    /// <param name="accelerationStep">The acceleration step for the SAR calculation.</param>
    /// <param name="maxAccelerationFactor">The maximum acceleration factor for the SAR calculation.</param>
    /// <param name="initialFactor">The initial acceleration factor for the SAR calculation.</param>
    /// <returns>A new <see cref="ParabolicSarList"/> instance.</returns>
    public static ParabolicSarList ToParabolicSarList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        double accelerationStep,
        double maxAccelerationFactor,
        double initialFactor)
        where TQuote : IQuote
        => new(accelerationStep, maxAccelerationFactor, initialFactor)
        {
            (IReadOnlyList<IQuote>)quotes
        };
}
