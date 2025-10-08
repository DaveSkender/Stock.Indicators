namespace Skender.Stock.Indicators;

// ADX (STREAM HUB)

public static partial class Adx
{
    /// <summary>
    /// Creates a stream hub for ADX indicator calculations.
    /// </summary>
    public static AdxHub<TIn> ToAdxHub<TIn>(
        this IQuoteProvider<TIn> quoteProvider,
        int lookbackPeriods = 14)
        where TIn : IQuote
        => new(quoteProvider, lookbackPeriods);
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
    /// <param name="quoteProvider">The stream observable provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    internal AdxHub(
        IQuoteProvider<TIn> quoteProvider,
        int lookbackPeriods)
        : base(quoteProvider)
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

        Reinitialize();
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
    public override string ToString() => Results.Count > 0
        ? $"{hubName}({Results[0].Timestamp:d})"
        : hubName;

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
        double? dx = null;
        double? adx = null;
        double? adxr = null; // Average Directional Movement Rating (ADX rating)

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
            // Initialize with SUM values (not averages) per StaticSeries implementation
            _prevTrs = _sumTr;
            _prevPdm = _sumPdm;
            _prevMdm = _sumMdm;

            pdi = _prevTrs != 0 ? 100 * _prevPdm / _prevTrs : null;
            mdi = _prevTrs != 0 ? 100 * _prevMdm / _prevTrs : null;

            if (pdi.HasValue && mdi.HasValue)
            {
                dx = pdi.Value + mdi.Value != 0
                    ? 100 * Math.Abs(pdi.Value - mdi.Value) / (pdi.Value + mdi.Value)
                    : 0;
                _sumDx = dx.Value;
            }
        }
        else if (i < (2 * LookbackPeriods) - 1)
        {
            // Smoothed values calculation
            // Wilder's smoothing keeps values on SUM scale
            _prevTrs = _prevTrs - (_prevTrs / LookbackPeriods) + tr;
            _prevPdm = _prevPdm - (_prevPdm / LookbackPeriods) + pdm1;
            _prevMdm = _prevMdm - (_prevMdm / LookbackPeriods) + mdm1;

            pdi = _prevTrs != 0 ? 100 * _prevPdm / _prevTrs : null;
            mdi = _prevTrs != 0 ? 100 * _prevMdm / _prevTrs : null;

            if (pdi.HasValue && mdi.HasValue)
            {
                dx = pdi.Value + mdi.Value != 0
                    ? 100 * Math.Abs(pdi.Value - mdi.Value) / (pdi.Value + mdi.Value)
                    : 0;
                _sumDx += dx.Value;
            }
        }
        else if (i == (2 * LookbackPeriods) - 1)
        {
            // First ADX calculation
            // Final smoothing before initial ADX
            _prevTrs = _prevTrs - (_prevTrs / LookbackPeriods) + tr;
            _prevPdm = _prevPdm - (_prevPdm / LookbackPeriods) + pdm1;
            _prevMdm = _prevMdm - (_prevMdm / LookbackPeriods) + mdm1;

            pdi = _prevTrs != 0 ? 100 * _prevPdm / _prevTrs : null;
            mdi = _prevTrs != 0 ? 100 * _prevMdm / _prevTrs : null;

            if (pdi.HasValue && mdi.HasValue)
            {
                dx = pdi.Value + mdi.Value != 0
                    ? 100 * Math.Abs(pdi.Value - mdi.Value) / (pdi.Value + mdi.Value)
                    : 0;
                _sumDx += dx.Value;

                _prevAdx = _sumDx / LookbackPeriods;
                adx = _prevAdx;
            }
        }
        else
        {
            // Subsequent ADX calculations
            _prevTrs = _prevTrs - (_prevTrs / LookbackPeriods) + tr;
            _prevPdm = _prevPdm - (_prevPdm / LookbackPeriods) + pdm1;
            _prevMdm = _prevMdm - (_prevMdm / LookbackPeriods) + mdm1;

            pdi = _prevTrs != 0 ? 100 * _prevPdm / _prevTrs : null;
            mdi = _prevTrs != 0 ? 100 * _prevMdm / _prevTrs : null;

            if (pdi.HasValue && mdi.HasValue)
            {
                dx = pdi.Value + mdi.Value != 0
                    ? 100 * Math.Abs(pdi.Value - mdi.Value) / (pdi.Value + mdi.Value)
                    : 0;

                _prevAdx = ((_prevAdx * (LookbackPeriods - 1)) + dx.Value) / LookbackPeriods;
                adx = _prevAdx;

                // ADXR becomes available once we have an ADX value from (lookbackPeriods - 1) periods earlier
                // Static series: i >= 3*lookbackPeriods - 2 (because first ADX at index 2*lookback -1)
                int firstAdxIndex = (2 * LookbackPeriods) - 1;
                int firstAdxrIndex = (3 * LookbackPeriods) - 2; // matches series implementation expectation
                if (i >= firstAdxrIndex)
                {
                    int priorAdxIndex = i - LookbackPeriods + 1; // same offset as static series
                    if (priorAdxIndex >= 0 && priorAdxIndex < Results.Count)
                    {
                        double? priorAdx = Results[priorAdxIndex].Adx;
                        if (priorAdx.HasValue && adx.HasValue)
                        {
                            adxr = (adx.Value + priorAdx.Value) / 2d;
                        }
                    }
                }
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
            dx,
            adx,
            adxr);

        return (result, i);
    }
}
