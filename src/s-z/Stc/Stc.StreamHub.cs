namespace Skender.Stock.Indicators;

// SCHAFF TREND CYCLE (STREAM HUB)

/// <summary>
/// Internal record for storing MACD intermediate values in STC calculation.
/// </summary>
/// <param name="FastEma">Fast EMA value.</param>
/// <param name="SlowEma">Slow EMA value.</param>
/// <param name="Macd">MACD value (FastEma - SlowEma).</param>
internal record StcMacdState(double FastEma, double SlowEma, double Macd);

/// <summary>
/// Provides methods for creating Schaff Trend Cycle (STC) streaming hubs.
/// </summary>
public class StcHub
    : ChainProvider<IReusable, StcResult>
{
    #region fields

    private readonly string hubName;
    private readonly RollingWindowMax<double> _macdHighWindow;
    private readonly RollingWindowMin<double> _macdLowWindow;
    private readonly Queue<double> _rawKBuffer;
    private readonly List<StcMacdState> _macdCache;
    private readonly double _fastK;
    private readonly double _slowK;

    #endregion fields

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StcHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="cyclePeriods">The number of periods for the cycle.</param>
    /// <param name="fastPeriods">The number of fast periods for the MACD calculation.</param>
    /// <param name="slowPeriods">The number of slow periods for the MACD calculation.</param>
    internal StcHub(
        IChainProvider<IReusable> provider,
        int cyclePeriods,
        int fastPeriods,
        int slowPeriods) : base(provider)
    {
        Stc.Validate(cyclePeriods, fastPeriods, slowPeriods);
        CyclePeriods = cyclePeriods;
        FastPeriods = fastPeriods;
        SlowPeriods = slowPeriods;

        // MACD smoothing factors
        _fastK = 2d / (fastPeriods + 1);
        _slowK = 2d / (slowPeriods + 1);

        // Stochastic rolling windows for MACD values
        _macdHighWindow = new RollingWindowMax<double>(cyclePeriods);
        _macdLowWindow = new RollingWindowMin<double>(cyclePeriods);

        // Buffer for raw %K smoothing (smoothPeriods = 3)
        _rawKBuffer = new Queue<double>(3);

        // Cache for MACD intermediate values
        _macdCache = [];

        hubName = $"STC({cyclePeriods},{fastPeriods},{slowPeriods})";

        Reinitialize();
    }

    #endregion constructors

    #region properties

    /// <inheritdoc/>
    public int CyclePeriods { get; init; }

    /// <inheritdoc/>
    public int FastPeriods { get; init; }

    /// <inheritdoc/>
    public int SlowPeriods { get; init; }

    #endregion properties

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (StcResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate MACD Fast EMA
        double fastEma = i >= FastPeriods - 1
            ? i > 0 && _macdCache.Count > 0 && _macdCache.Count > i - 1
                ? Ema.Increment(_fastK, _macdCache[i - 1].FastEma, item.Value)
                : Sma.Increment(ProviderCache, FastPeriods, i)
            : double.NaN;

        // Calculate MACD Slow EMA
        double slowEma = i >= SlowPeriods - 1
            ? i > 0 && _macdCache.Count > 0 && _macdCache.Count > i - 1
                ? Ema.Increment(_slowK, _macdCache[i - 1].SlowEma, item.Value)
                : Sma.Increment(ProviderCache, SlowPeriods, i)
            : double.NaN;

        // Calculate MACD value
        double macd = fastEma - slowEma;

        // Store MACD state in cache
        if (_macdCache.Count == i)
        {
            _macdCache.Add(new StcMacdState(fastEma, slowEma, macd));
        }
        else if (_macdCache.Count > i)
        {
            _macdCache[i] = new StcMacdState(fastEma, slowEma, macd);
        }

        // Add MACD to rolling windows for stochastic calculation (only if not NaN)
        if (!double.IsNaN(macd))
        {
            _macdHighWindow.Add(macd);
            _macdLowWindow.Add(macd);
        }

        // Calculate raw %K from MACD values
        double rawK = double.NaN;
        if (i >= SlowPeriods + CyclePeriods - 2)
        {
            if (!double.IsNaN(macd) && _macdHighWindow.Count >= CyclePeriods && _macdLowWindow.Count >= CyclePeriods)
            {
                double highHigh = _macdHighWindow.Max;
                double lowLow = _macdLowWindow.Min;

                rawK = highHigh - lowLow != 0
                     ? 100 * (macd - lowLow) / (highHigh - lowLow)
                     : 0;
            }
        }

        // Buffer raw %K for smoothing (only if valid)
        if (!double.IsNaN(rawK))
        {
            _rawKBuffer.Enqueue(rawK);
            if (_rawKBuffer.Count > 3)
            {
                _rawKBuffer.Dequeue();
            }
        }

        // Calculate smoothed %K (STC value) using SMA with smoothPeriods=3
        double? stc = null;
        if (i >= SlowPeriods + CyclePeriods && _rawKBuffer.Count >= 3)
        {
            double sum = 0;
            int validCount = 0;
            foreach (double rawKValue in _rawKBuffer)
            {
                if (!double.IsNaN(rawKValue))
                {
                    sum += rawKValue;
                    validCount++;
                }
            }

            if (validCount == 3)
            {
                stc = sum / 3;
            }
        }

        StcResult result = new(
            Timestamp: item.Timestamp,
            Stc: stc);

        return (result, i);
    }

    /// <summary>
    /// Restores the MACD and stochastic state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear rolling windows and buffer
        _macdHighWindow.Clear();
        _macdLowWindow.Clear();
        _rawKBuffer.Clear();

        // Rebuild from ProviderCache up to the rollback point
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        int targetIndex = index - 1;

        // Rebuild MACD cache up to target
        _macdCache.Clear();
        for (int p = 0; p <= targetIndex; p++)
        {
            IReusable item = ProviderCache[p];

            // Calculate Fast EMA
            double fastEma = p >= FastPeriods - 1
                ? p > 0 && _macdCache.Count > 0
                    ? Ema.Increment(_fastK, _macdCache[p - 1].FastEma, item.Value)
                    : Sma.Increment(ProviderCache, FastPeriods, p)
                : double.NaN;

            // Calculate Slow EMA
            double slowEma = p >= SlowPeriods - 1
                ? p > 0 && _macdCache.Count > 0
                    ? Ema.Increment(_slowK, _macdCache[p - 1].SlowEma, item.Value)
                    : Sma.Increment(ProviderCache, SlowPeriods, p)
                : double.NaN;

            double macd = fastEma - slowEma;
            _macdCache.Add(new StcMacdState(fastEma, slowEma, macd));
        }

        // Rebuild MACD rolling windows for cycle periods
        int startIdx = Math.Max(0, targetIndex + 1 - CyclePeriods);
        for (int p = startIdx; p <= targetIndex; p++)
        {
            double macdValue = _macdCache[p].Macd;
            if (!double.IsNaN(macdValue))
            {
                _macdHighWindow.Add(macdValue);
                _macdLowWindow.Add(macdValue);
            }
        }

        // Prefill raw %K buffer (smoothPeriods = 3)
        if (targetIndex >= SlowPeriods + CyclePeriods - 2)
        {
            int kStart = Math.Max(SlowPeriods + CyclePeriods - 2, targetIndex + 1 - 3);
            for (int p = kStart; p <= targetIndex; p++)
            {
                // Calculate raw %K for this position
                int rStart = Math.Max(0, p + 1 - CyclePeriods);
                double hh = double.NegativeInfinity;
                double ll = double.PositiveInfinity;

                for (int r = rStart; r <= p; r++)
                {
                    double macdAtR = _macdCache[r].Macd;
                    if (macdAtR > hh)
                    {
                        hh = macdAtR;
                    }

                    if (macdAtR < ll)
                    {
                        ll = macdAtR;
                    }
                }

                double macdAtP = _macdCache[p].Macd;
                double rawAtP = (hh - ll) != 0 ? 100 * (macdAtP - ll) / (hh - ll) : 0;
                _rawKBuffer.Enqueue(rawAtP);
            }
        }
    }

    #endregion methods
}


public static partial class Stc
{
    /// <summary>
    /// Creates a Schaff Trend Cycle (STC) streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="cyclePeriods">The number of periods for the cycle. Default is 10.</param>
    /// <param name="fastPeriods">The number of fast periods for the MACD calculation. Default is 23.</param>
    /// <param name="slowPeriods">The number of slow periods for the MACD calculation. Default is 50.</param>
    /// <returns>A STC hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    public static StcHub ToStcHub(
        this IChainProvider<IReusable> chainProvider,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
        => new(chainProvider, cyclePeriods, fastPeriods, slowPeriods);

    /// <summary>
    /// Creates a Stc hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="cyclePeriods">Parameter for the calculation.</param>
    /// <param name="fastPeriods">Parameter for the calculation.</param>
    /// <param name="slowPeriods">Parameter for the calculation.</param>
    /// <returns>An instance of <see cref="StcHub"/>.</returns>
    public static StcHub ToStcHub(
        this IReadOnlyList<IQuote> quotes,
        int cyclePeriods = 10,
        int fastPeriods = 23,
        int slowPeriods = 50)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToStcHub(cyclePeriods, fastPeriods, slowPeriods);
    }
}
