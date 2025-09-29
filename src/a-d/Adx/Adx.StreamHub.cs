namespace Skender.Stock.Indicators;

// ADX (STREAM HUB)

public static partial class Adx
{
    /// <summary>
    /// Creates a stream hub for ADX indicator calculations.
    /// </summary>
    public static AdxHub<TIn> ToAdx<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods = 14)
        where TIn : IQuote
        => new(
            chainProvider,
            lookbackPeriods);
}

/// <summary>
/// Represents a stream hub for calculating the Average Directional Index (ADX).
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
/// <inheritdoc cref="IAdx"/>
public class AdxHub<TIn>
   : StreamHub<TIn, AdxResult>, IAdx
   where TIn : IQuote
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdxHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    internal AdxHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods)
        : base(provider)
    {
        Adx.Validate(lookbackPeriods);
        
        LookbackPeriods = lookbackPeriods;
        hubName = $"ADX({LookbackPeriods})";

        // Initialize state variables
        _isFirstPeriod = true;
        _sumTr = 0;
        _sumPdm = 0;
        _sumMdm = 0;
        _sumDx = 0;
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    // State variables
    private bool _isFirstPeriod;
    private double _prevHigh;
    private double _prevLow;
    private double _prevClose;
    private double _prevTrs; // smoothed TR
    private double _prevPdm; // smoothed PDM
    private double _prevMdm; // smoothed MDM
    private double _prevAdx;
    private double _sumTr;
    private double _sumPdm;
    private double _sumMdm;
    private double _sumDx;

    #endregion

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (AdxResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? throw new ArgumentNullException(nameof(indexHint));

        double high = (double)item.High;
        double low = (double)item.Low;
        double close = (double)item.Close;

        // Handle first period
        if (_isFirstPeriod)
        {
            _prevHigh = high;
            _prevLow = low;
            _prevClose = close;
            _isFirstPeriod = false;

            return (new AdxResult(item.Timestamp), i);
        }

        // Calculate True Range and directional movement
        double hmpc = Math.Abs(high - _prevClose);
        double lmpc = Math.Abs(low - _prevClose);
        double hmph = high - _prevHigh;
        double plml = _prevLow - low;

        double tr = Math.Max(high - low, Math.Max(hmpc, lmpc));
        double pdm1 = hmph > plml ? Math.Max(hmph, 0) : 0;
        double mdm1 = plml > hmph ? Math.Max(plml, 0) : 0;

        double? pdi = null;
        double? mdi = null;
        double? adx = null;

        if (i < LookbackPeriods)
        {
            // Accumulation phase
            _sumTr += tr;
            _sumPdm += pdm1;
            _sumMdm += mdm1;
        }
        else if (i == LookbackPeriods)
        {
            // First calculated values
            _sumTr += tr;
            _sumPdm += pdm1;
            _sumMdm += mdm1;

            _prevTrs = _sumTr / LookbackPeriods;
            _prevPdm = _sumPdm / LookbackPeriods;
            _prevMdm = _sumMdm / LookbackPeriods;

            pdi = _prevTrs != 0 ? 100 * _prevPdm / _prevTrs : null;
            mdi = _prevTrs != 0 ? 100 * _prevMdm / _prevTrs : null;

            if (pdi.HasValue && mdi.HasValue)
            {
                double dx = pdi.Value + mdi.Value != 0 
                    ? 100 * Math.Abs(pdi.Value - mdi.Value) / (pdi.Value + mdi.Value) 
                    : 0;
                _sumDx = dx;
            }
        }
        else if (i < 2 * LookbackPeriods - 1)
        {
            // Smoothed values calculation
            _prevTrs = ((_prevTrs * (LookbackPeriods - 1)) + tr) / LookbackPeriods;
            _prevPdm = ((_prevPdm * (LookbackPeriods - 1)) + pdm1) / LookbackPeriods;
            _prevMdm = ((_prevMdm * (LookbackPeriods - 1)) + mdm1) / LookbackPeriods;

            pdi = _prevTrs != 0 ? 100 * _prevPdm / _prevTrs : null;
            mdi = _prevTrs != 0 ? 100 * _prevMdm / _prevTrs : null;

            if (pdi.HasValue && mdi.HasValue)
            {
                double dx = pdi.Value + mdi.Value != 0 
                    ? 100 * Math.Abs(pdi.Value - mdi.Value) / (pdi.Value + mdi.Value) 
                    : 0;
                _sumDx += dx;
            }
        }
        else if (i == 2 * LookbackPeriods - 1)
        {
            // First ADX calculation
            _prevTrs = ((_prevTrs * (LookbackPeriods - 1)) + tr) / LookbackPeriods;
            _prevPdm = ((_prevPdm * (LookbackPeriods - 1)) + pdm1) / LookbackPeriods;
            _prevMdm = ((_prevMdm * (LookbackPeriods - 1)) + mdm1) / LookbackPeriods;

            pdi = _prevTrs != 0 ? 100 * _prevPdm / _prevTrs : null;
            mdi = _prevTrs != 0 ? 100 * _prevMdm / _prevTrs : null;

            if (pdi.HasValue && mdi.HasValue)
            {
                double dx = pdi.Value + mdi.Value != 0 
                    ? 100 * Math.Abs(pdi.Value - mdi.Value) / (pdi.Value + mdi.Value) 
                    : 0;
                _sumDx += dx;

                _prevAdx = _sumDx / LookbackPeriods;
                adx = _prevAdx;
            }
        }
        else
        {
            // Subsequent ADX calculations
            _prevTrs = ((_prevTrs * (LookbackPeriods - 1)) + tr) / LookbackPeriods;
            _prevPdm = ((_prevPdm * (LookbackPeriods - 1)) + pdm1) / LookbackPeriods;
            _prevMdm = ((_prevMdm * (LookbackPeriods - 1)) + mdm1) / LookbackPeriods;

            pdi = _prevTrs != 0 ? 100 * _prevPdm / _prevTrs : null;
            mdi = _prevTrs != 0 ? 100 * _prevMdm / _prevTrs : null;

            if (pdi.HasValue && mdi.HasValue)
            {
                double dx = pdi.Value + mdi.Value != 0 
                    ? 100 * Math.Abs(pdi.Value - mdi.Value) / (pdi.Value + mdi.Value) 
                    : 0;

                _prevAdx = ((_prevAdx * (LookbackPeriods - 1)) + dx) / LookbackPeriods;
                adx = _prevAdx;
            }
        }

        // Update previous values
        _prevHigh = high;
        _prevLow = low;
        _prevClose = close;

        AdxResult result = new(
            item.Timestamp,
            pdi,
            mdi,
            adx);

        return (result, i);
    }
}