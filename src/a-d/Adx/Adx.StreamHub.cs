namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Average Directional Index (ADX).
/// </summary>
public class AdxHub
    : ChainHub<IQuote, AdxResult>, IAdx
{
    internal AdxHub(
        IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods)
        : base(quoteProvider)
    {
        Adx.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        Name = $"ADX({LookbackPeriods})";

        // Initialize state variables
        _isFirstPeriod = true;
        _prevTrs = double.NaN;
        _prevPdm = double.NaN;
        _prevMdm = double.NaN;
        _prevAdx = double.NaN;
        _sumTr = 0;
        _sumPdm = 0;
        _sumMdm = 0;
        _sumDx = 0;

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// State variables
    /// </summary>
    private bool _isFirstPeriod;
    private double _prevHigh;
    private double _prevLow;
    private double _prevClose;
    /// <summary>
    /// smoothed TR
    /// </summary>
    private double _prevTrs;
    /// <summary>
    /// smoothed PDM
    /// </summary>
    private double _prevPdm;
    /// <summary>
    /// smoothed MDM
    /// </summary>
    private double _prevMdm;
    private double _prevAdx;
    private double _sumTr;
    private double _sumPdm;
    private double _sumMdm;
    private double _sumDx;


    /// <inheritdoc/>
    public override string ToString() => Results.Count > 0
        ? $"{Name}({Results[0].Timestamp:d})"
        : Name;

    /// <inheritdoc/>
    protected override (AdxResult result, int index)
        ToIndicator(IQuote item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

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

        // Accumulate TR/PDM/MDM during warmup
        if (i <= LookbackPeriods)
        {
            _sumTr += tr;
            _sumPdm += pdm1;
            _sumMdm += mdm1;
        }

        // Skip calculation until we have enough data
        if (i < LookbackPeriods)
        {
            // Continue accumulation only
        }
        // Calculate smoothed TR/PDM/MDM and directional indicators
        else
        {
            // Initialize smoothed values on first calculation
            if (double.IsNaN(_prevTrs))
            {
                // Initialize with SUM values (not averages) per StaticSeries implementation
                _prevTrs = _sumTr;
                _prevPdm = _sumPdm;
                _prevMdm = _sumMdm;
            }
            // Apply Wilder's smoothing for subsequent periods
            else
            {
                // Wilder's smoothing keeps values on SUM scale
                _prevTrs = _prevTrs - (_prevTrs / LookbackPeriods) + tr;
                _prevPdm = _prevPdm - (_prevPdm / LookbackPeriods) + pdm1;
                _prevMdm = _prevMdm - (_prevMdm / LookbackPeriods) + mdm1;
            }

            // Calculate directional indicators
            pdi = _prevTrs != 0 ? 100 * _prevPdm / _prevTrs : null;
            mdi = _prevTrs != 0 ? 100 * _prevMdm / _prevTrs : null;

            if (pdi.HasValue && mdi.HasValue)
            {
                dx = pdi.Value + mdi.Value != 0
                    ? 100 * Math.Abs(pdi.Value - mdi.Value) / (pdi.Value + mdi.Value)
                    : 0;

                // ADX initialization and calculation
                if (i < (2 * LookbackPeriods))
                {
                    // Accumulate DX values for initial ADX
                    _sumDx += dx.Value;

                    // Calculate initial ADX when we have enough DX values
                    if (double.IsNaN(_prevAdx) && i == (2 * LookbackPeriods) - 1)
                    {
                        _prevAdx = _sumDx / LookbackPeriods;
                        adx = _prevAdx;
                    }
                }
                // Ongoing ADX smoothing
                else
                {
                    _prevAdx = ((_prevAdx * (LookbackPeriods - 1)) + dx.Value) / LookbackPeriods;
                    adx = _prevAdx;

                    // ADXR calculation: average of current ADX and ADX from lookbackPeriods ago
                    // First valid ADXR when: i - lookbackPeriods >= (2 * lookbackPeriods) - 1
                    // Which simplifies to: i >= (3 * lookbackPeriods) - 1
                    int priorAdxIndex = i - LookbackPeriods;
                    if (priorAdxIndex >= (2 * LookbackPeriods) - 1 && priorAdxIndex >= 0 && priorAdxIndex < Results.Count)
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

    /// <summary>
    /// Restore rolling state up to the specified timestamp for accurate rebuilds.
    /// </summary>
    /// <param name="timestamp">Timestamp of record.</param>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset all state
        _isFirstPeriod = true;
        _prevHigh = 0;
        _prevLow = 0;
        _prevClose = 0;
        _prevTrs = double.NaN;
        _prevPdm = double.NaN;
        _prevMdm = double.NaN;
        _prevAdx = double.NaN;
        _sumTr = 0;
        _sumPdm = 0;
        _sumMdm = 0;
        _sumDx = 0;

        if (timestamp <= DateTime.MinValue || ProviderCache.Count == 0)
        {
            return;
        }

        // Find the first index at or after timestamp
        int index = ProviderCache.IndexGte(timestamp);

        if (index <= 0)
        {
            // Rolling back before all data, keep cleared state
            return;
        }

        // We need to rebuild state up to the index before timestamp
        // (since IndexGte gives us first index >= timestamp)
        int targetIndex = index - 1;

        for (int i = 0; i <= targetIndex; i++)
        {
            IQuote item = ProviderCache[i];

            double high = (double)item.High;
            double low = (double)item.Low;
            double close = (double)item.Close;

            if (_isFirstPeriod)
            {
                _prevHigh = high;
                _prevLow = low;
                _prevClose = close;
                _isFirstPeriod = false;
                continue;
            }

            double hmpc = Math.Abs(high - _prevClose);
            double lmpc = Math.Abs(low - _prevClose);
            double hmph = high - _prevHigh;
            double plml = _prevLow - low;

            double tr = Math.Max(high - low, Math.Max(hmpc, lmpc));
            double pdm1 = hmph > plml ? Math.Max(hmph, 0) : 0;
            double mdm1 = plml > hmph ? Math.Max(plml, 0) : 0;

            // Accumulate TR/PDM/MDM during warmup
            if (i <= LookbackPeriods)
            {
                _sumTr += tr;
                _sumPdm += pdm1;
                _sumMdm += mdm1;
            }

            // Skip calculation until we have enough data
            if (i < LookbackPeriods)
            {
                // Continue accumulation only
            }
            // Calculate smoothed TR/PDM/MDM and directional indicators
            else
            {
                // Initialize smoothed values on first calculation
                if (double.IsNaN(_prevTrs))
                {
                    _prevTrs = _sumTr;
                    _prevPdm = _sumPdm;
                    _prevMdm = _sumMdm;
                }
                // Apply Wilder's smoothing for subsequent periods
                else
                {
                    _prevTrs = _prevTrs - (_prevTrs / LookbackPeriods) + tr;
                    _prevPdm = _prevPdm - (_prevPdm / LookbackPeriods) + pdm1;
                    _prevMdm = _prevMdm - (_prevMdm / LookbackPeriods) + mdm1;
                }

                double? pdi = _prevTrs != 0 ? 100 * _prevPdm / _prevTrs : null;
                double? mdi = _prevTrs != 0 ? 100 * _prevMdm / _prevTrs : null;
                if (pdi.HasValue && mdi.HasValue)
                {
                    double dx = pdi.Value + mdi.Value != 0
                        ? 100 * Math.Abs(pdi.Value - mdi.Value) / (pdi.Value + mdi.Value)
                        : 0;

                    // ADX initialization and calculation
                    if (i < (2 * LookbackPeriods))
                    {
                        // Accumulate DX values for initial ADX
                        _sumDx += dx;

                        // Calculate initial ADX when we have enough DX values
                        if (double.IsNaN(_prevAdx) && i == (2 * LookbackPeriods) - 1)
                        {
                            _prevAdx = _sumDx / LookbackPeriods;
                        }
                    }
                    // Ongoing ADX smoothing
                    else
                    {
                        _prevAdx = ((_prevAdx * (LookbackPeriods - 1)) + dx) / LookbackPeriods;
                    }
                }
            }

            _prevHigh = high;
            _prevLow = low;
            _prevClose = close;
        }
    }
}

public static partial class Adx
{
    /// <summary>
    /// Creates a stream hub for ADX indicator calculations.
    /// </summary>
    /// <param name="quoteProvider">The quote provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    public static AdxHub ToAdxHub(
        this IQuoteProvider<IQuote> quoteProvider,
        int lookbackPeriods = 14)
             => new(quoteProvider, lookbackPeriods);
}
