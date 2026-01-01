namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Parabolic SAR.
/// </summary>
public class ParabolicSarHub
    : ChainHub<IQuote, ParabolicSarResult>, IParabolicSar
{
    private readonly Queue<(double High, double Low)> _buffer;

    // State variables
    private double _accelerationFactor;
    private double _extremePoint;
    private double _priorSar;
    private bool _isRising;
    private bool _isInitialized;
    private bool _firstReversalFound;

    internal ParabolicSarHub(
        IQuoteProvider<IQuote> provider,
        double accelerationStep = 0.02,
        double maxAccelerationFactor = 0.2)
        : this(provider, accelerationStep, maxAccelerationFactor, accelerationStep)
    {
    }
    internal ParabolicSarHub(
        IQuoteProvider<IQuote> provider,
        double accelerationStep,
        double maxAccelerationFactor,
        double initialFactor) : base(provider)
    {
        ParabolicSar.Validate(accelerationStep, maxAccelerationFactor, initialFactor);

        AccelerationStep = accelerationStep;
        MaxAccelerationFactor = maxAccelerationFactor;
        InitialFactor = initialFactor;
        Name = $"PSAR({accelerationStep},{maxAccelerationFactor},{initialFactor})";

        _buffer = new Queue<(double, double)>(2);
        _isInitialized = false;
        _firstReversalFound = false;

        Reinitialize();
    }

    /// <inheritdoc/>
    public double AccelerationStep { get; init; }

    /// <inheritdoc/>
    public double MaxAccelerationFactor { get; init; }

    /// <inheritdoc/>
    public double InitialFactor { get; init; }
    /// <inheritdoc/>
    protected override (ParabolicSarResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double high = (double)item.High;
        double low = (double)item.Low;

        // Skip first quote (initialize state only)
        if (!_isInitialized)
        {
            _accelerationFactor = InitialFactor;
            _extremePoint = high;
            _priorSar = low;
            _isRising = true;  // initial guess
            _isInitialized = true;

            // Ensure prior-quote buffer contains the first quote for next-bar clamps
            _buffer.Update(2, (high, low));

            return (new ParabolicSarResult(item.Timestamp), i);
        }

        bool? isReversal;
        double psar;

        // Was rising
        if (_isRising)
        {
            double sar = _priorSar + (_accelerationFactor * (_extremePoint - _priorSar));

            // SAR cannot be higher than last two lows
            // _buffer contains the PREVIOUS quotes (not including current)
            if (_buffer.Count >= 2)
            {
                (double _, double l1) = _buffer.ElementAt(1);  // i-1
                (double _, double l2) = _buffer.ElementAt(0);  // i-2
                double minLastTwo = Math.Min(l1, l2);
                sar = Math.Min(sar, minLastTwo);
            }

            // Turn down
            if (low < sar)
            {
                isReversal = true;
                psar = _extremePoint;

                _isRising = false;
                _accelerationFactor = InitialFactor;
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
                        _accelerationFactor + AccelerationStep,
                        MaxAccelerationFactor);
                }
            }
        }
        // Was falling
        else
        {
            double sar = _priorSar - (_accelerationFactor * (_priorSar - _extremePoint));

            // SAR cannot be lower than last two highs
            // _buffer contains the PREVIOUS quotes (not including current)
            if (_buffer.Count >= 2)
            {
                (double h1, double _) = _buffer.ElementAt(1);  // i-1
                (double h2, double _) = _buffer.ElementAt(0);  // i-2
                double maxLastTwo = Math.Max(h1, h2);
                sar = Math.Max(sar, maxLastTwo);
            }

            // Turn up
            if (high > sar)
            {
                isReversal = true;
                psar = _extremePoint;

                _isRising = true;
                _accelerationFactor = InitialFactor;
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
                        _accelerationFactor + AccelerationStep,
                        MaxAccelerationFactor);
                }
            }
        }

        // Add result
        // Only output SAR values after the first reversal has been found
        ParabolicSarResult result;

        if (!_firstReversalFound)
        {
            // Before first reversal, output null values
            result = new ParabolicSarResult(item.Timestamp);

            // Check if this IS the first reversal
            if (isReversal == true)
            {
                _firstReversalFound = true;
            }
        }
        else
        {
            // After first reversal, output actual values
            result = new ParabolicSarResult(
                Timestamp: item.Timestamp,
                Sar: psar.NaN2Null(),
                IsReversal: isReversal);
        }

        _priorSar = psar;

        // Update buffer for last two quotes AFTER using it for calculations
        _buffer.Update(2, (high, low));

        return (result, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear state
        _buffer.Clear();
        _isInitialized = false;
        _firstReversalFound = false;

        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        // Rebuild state by replaying history up to timestamp
        int targetIndex = index - 1;

        for (int p = 0; p <= targetIndex; p++)
        {
            IQuote quote = ProviderCache[p];
            double high = (double)quote.High;
            double low = (double)quote.Low;

            if (p == 0)
            {
                // Initialize state with first quote
                _accelerationFactor = InitialFactor;
                _extremePoint = high;
                _priorSar = low;
                _isRising = true;
                _isInitialized = true;
                _buffer.Update(2, (high, low));
                continue;
            }

            // Replay the calculation logic for this quote
            bool isReversal;

            if (_isRising)
            {
                double sar = _priorSar + (_accelerationFactor * (_extremePoint - _priorSar));

                if (_buffer.Count >= 2)
                {
                    (double _, double l1) = _buffer.ElementAt(1);
                    (double _, double l2) = _buffer.ElementAt(0);
                    double minLastTwo = Math.Min(l1, l2);
                    sar = Math.Min(sar, minLastTwo);
                }

                if (low < sar)
                {
                    isReversal = true;
                    _priorSar = _extremePoint;
                    _isRising = false;
                    _accelerationFactor = InitialFactor;
                    _extremePoint = low;
                }
                else
                {
                    isReversal = false;
                    _priorSar = sar;

                    if (high > _extremePoint)
                    {
                        _extremePoint = high;
                        _accelerationFactor = Math.Min(
                            _accelerationFactor + AccelerationStep,
                            MaxAccelerationFactor);
                    }
                }
            }
            else
            {
                double sar = _priorSar - (_accelerationFactor * (_priorSar - _extremePoint));

                if (_buffer.Count >= 2)
                {
                    (double h1, double _) = _buffer.ElementAt(1);
                    (double h2, double _) = _buffer.ElementAt(0);
                    double maxLastTwo = Math.Max(h1, h2);
                    sar = Math.Max(sar, maxLastTwo);
                }

                if (high > sar)
                {
                    isReversal = true;
                    _priorSar = _extremePoint;
                    _isRising = true;
                    _accelerationFactor = InitialFactor;
                    _extremePoint = high;
                }
                else
                {
                    isReversal = false;
                    _priorSar = sar;

                    if (low < _extremePoint)
                    {
                        _extremePoint = low;
                        _accelerationFactor = Math.Min(
                            _accelerationFactor + AccelerationStep,
                            MaxAccelerationFactor);
                    }
                }
            }

            _buffer.Update(2, (high, low));

            // Track first reversal
            if (isReversal && !_firstReversalFound)
            {
                _firstReversalFound = true;
            }
        }
    }
}

public static partial class ParabolicSar
{
    /// <summary>
    /// Creates a Parabolic SAR streaming hub from a quote provider.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="accelerationStep">The acceleration step for the SAR calculation. Default is 0.02.</param>
    /// <param name="maxAccelerationFactor">The maximum acceleration factor for the SAR calculation. Default is 0.2.</param>
    /// <returns>A Parabolic SAR hub.</returns>
    public static ParabolicSarHub ToParabolicSarHub(
        this IQuoteProvider<IQuote> quoteProvider,
        double accelerationStep = 0.02,
        double maxAccelerationFactor = 0.2)
        => new(quoteProvider, accelerationStep, maxAccelerationFactor);

    /// <summary>
    /// Creates a Parabolic SAR streaming hub from a quote provider with custom initial factor.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="accelerationStep">The acceleration step for the SAR calculation.</param>
    /// <param name="maxAccelerationFactor">The maximum acceleration factor for the SAR calculation.</param>
    /// <param name="initialFactor">The initial acceleration factor for the SAR calculation.</param>
    /// <returns>A Parabolic SAR hub.</returns>
    public static ParabolicSarHub ToParabolicSarHub(
        this IQuoteProvider<IQuote> quoteProvider,
        double accelerationStep,
        double maxAccelerationFactor,
        double initialFactor)
        => new(quoteProvider, accelerationStep, maxAccelerationFactor, initialFactor);
}
