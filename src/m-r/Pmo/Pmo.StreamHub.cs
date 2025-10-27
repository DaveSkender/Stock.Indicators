namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Price Momentum Oscillator (PMO) indicator.
/// </summary>
public class PmoHub
    : ChainProvider<IReusable, PmoResult>, IPmo
{
    private readonly string hubName;
    private readonly double smoothingConstant1;
    private readonly double smoothingConstant2;
    private readonly double smoothingConstant3;

    private double prevRocEma;
    private double prevPmo;
    private double prevSignal;

    /// <summary>
    /// Initializes a new instance of the <see cref="PmoHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="timePeriods">The number of periods for the time span.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    internal PmoHub(
        IChainProvider<IReusable> provider,
        int timePeriods,
        int smoothPeriods,
        int signalPeriods) : base(provider)
    {
        Pmo.Validate(timePeriods, smoothPeriods, signalPeriods);
        TimePeriods = timePeriods;
        SmoothPeriods = smoothPeriods;
        SignalPeriods = signalPeriods;

        smoothingConstant1 = 2d / smoothPeriods;
        smoothingConstant2 = 2d / timePeriods;
        smoothingConstant3 = 2d / (signalPeriods + 1);

        hubName = $"PMO({timePeriods},{smoothPeriods},{signalPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int TimePeriods { get; init; }

    /// <inheritdoc/>
    public int SmoothPeriods { get; init; }

    /// <inheritdoc/>
    public int SignalPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (PmoResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double currentValue = item.Value;
        double prevValue = i > 0 ? ProviderCache[i - 1].Value : double.NaN;

        // Calculate rate of change (ROC)
        double roc = prevValue == 0 ? double.NaN : 100 * ((currentValue / prevValue) - 1);

        // Calculate ROC EMA (first smoothing with timePeriods)
        double rocEma = i >= TimePeriods
            ? i > 0 && !double.IsNaN(prevRocEma)
                // Calculate EMA normally
                ? prevRocEma + (smoothingConstant2 * (roc - prevRocEma))
                // Initialize as SMA of ROC
                : InitRocEma(i)
            // warmup periods are never calculable
            : double.NaN;

        double rocEmaScaled = rocEma * 10;
        prevRocEma = rocEma;

        // Calculate PMO (second smoothing with smoothPeriods)
        double pmoValue = i >= SmoothPeriods + TimePeriods - 1
            ? i > 0 && !double.IsNaN(prevPmo)
                // Calculate EMA normally
                ? prevPmo + (smoothingConstant1 * (rocEmaScaled - prevPmo))
                // Initialize as SMA of ROC EMA scaled
                : InitPmo(i)
            // warmup periods are never calculable
            : double.NaN;

        prevPmo = pmoValue;

        // Calculate Signal (third smoothing with signalPeriods)
        double signalValue;
        if (i >= SignalPeriods + SmoothPeriods + TimePeriods - 2 && (i == 0 || Cache[i - 1].Signal is null))
        {
            // Initialize signal as SMA of PMO values from Cache
            double sum = pmoValue;
            for (int j = i - SignalPeriods + 1; j < i; j++)
            {
                sum += Cache[j].Value;
            }
            signalValue = sum / SignalPeriods;
        }
        else
        {
            // Calculate signal EMA normally
            signalValue = Ema.Increment(smoothingConstant3, i > 0 ? Cache[i - 1].Signal ?? double.NaN : double.NaN, pmoValue);
        }

        prevSignal = signalValue;

        // Candidate result
        PmoResult r = new(
            Timestamp: item.Timestamp,
            Pmo: pmoValue.NaN2Null(),
            Signal: signalValue.NaN2Null());

        return (r, i);
    }

    private double InitRocEma(int endIndex)
    {
        double sum = 0;
        for (int p = endIndex - TimePeriods + 1; p <= endIndex; p++)
        {
            double pCurrVal = ProviderCache[p].Value;
            double pPrevVal = p > 0 ? ProviderCache[p - 1].Value : double.NaN;
            double pRoc = pPrevVal == 0 ? double.NaN : 100 * ((pCurrVal / pPrevVal) - 1);
            sum += pRoc;
        }
        return sum / TimePeriods;
    }

    private double InitPmo(int endIndex)
    {
        // Need to rebuild ROC EMA values for this window
        double sum = 0;
        double tempPrevRocEma = double.NaN;

        for (int p = endIndex - SmoothPeriods + 1; p <= endIndex; p++)
        {
            double pCurrVal = ProviderCache[p].Value;
            double pPrevVal = p > 0 ? ProviderCache[p - 1].Value : double.NaN;
            double pRoc = pPrevVal == 0 ? double.NaN : 100 * ((pCurrVal / pPrevVal) - 1);

            double pRocEma;
            if (double.IsNaN(tempPrevRocEma) && p >= TimePeriods)
            {
                pRocEma = InitRocEma(p);
            }
            else if (!double.IsNaN(tempPrevRocEma))
            {
                pRocEma = tempPrevRocEma + (smoothingConstant2 * (pRoc - tempPrevRocEma));
            }
            else
            {
                pRocEma = double.NaN;
            }

            tempPrevRocEma = pRocEma;
            sum += pRocEma * 10;
        }

        return sum / SmoothPeriods;
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Reset state - will be recalculated during rebuild
        prevRocEma = double.NaN;
        prevPmo = double.NaN;
        prevSignal = double.NaN;
    }
}


public static partial class Pmo
{
    /// <summary>
    /// Creates a PMO streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="timePeriods">The number of periods for the time span.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <returns>A PMO hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    public static PmoHub ToPmoHub(
        this IChainProvider<IReusable> chainProvider,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10)
        => new(chainProvider, timePeriods, smoothPeriods, signalPeriods);

    /// <summary>
    /// Creates a PMO hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="timePeriods">The number of periods for the time span.</param>
    /// <param name="smoothPeriods">The number of periods for smoothing.</param>
    /// <param name="signalPeriods">The number of periods for the signal line.</param>
    /// <returns>An instance of <see cref="PmoHub"/>.</returns>
    public static PmoHub ToPmoHub(
        this IReadOnlyList<IQuote> quotes,
        int timePeriods = 35,
        int smoothPeriods = 20,
        int signalPeriods = 10)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToPmoHub(timePeriods, smoothPeriods, signalPeriods);
    }
}
