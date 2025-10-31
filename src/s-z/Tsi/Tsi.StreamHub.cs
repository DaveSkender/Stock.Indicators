namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the True Strength Index (TSI) indicator.
/// </summary>
public class TsiHub
    : ChainProvider<IReusable, TsiResult>, ITsi
{
    private readonly string hubName;
    private readonly double mult1;  // smoothing constant for first EMA (lookbackPeriods)
    private readonly double mult2;  // smoothing constant for second EMA (smoothPeriods)
    private readonly double multS;  // smoothing constant for signal EMA (signalPeriods)

    /// <summary>
    /// Initializes a new instance of the <see cref="TsiHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback calculation.</param>
    /// <param name="smoothPeriods">The number of periods for the smoothing calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    internal TsiHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        int smoothPeriods,
        int signalPeriods) : base(provider)
    {
        Tsi.Validate(lookbackPeriods, smoothPeriods, signalPeriods);
        LookbackPeriods = lookbackPeriods;
        SmoothPeriods = smoothPeriods;
        SignalPeriods = signalPeriods;

        mult1 = 2d / (lookbackPeriods + 1);
        mult2 = 2d / (smoothPeriods + 1);
        multS = 2d / (signalPeriods + 1);

        hubName = $"TSI({lookbackPeriods},{smoothPeriods},{signalPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public int SmoothPeriods { get; init; }

    /// <inheritdoc/>
    public int SignalPeriods { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (TsiResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Skip first period (no previous value for change calculation)
        if (i == 0)
        {
            return (new TsiResult(item.Timestamp), i);
        }

        double currentValue = item.Value;
        double prevValue = ProviderCache[i - 1].Value;

        // Price change
        double change = currentValue - prevValue;
        double absChange = Math.Abs(change);

        // Get intermediate smoothing states
        (double cs1, double as1) = CalculateFirstSmoothing(i, change, absChange);
        (double cs2, double as2) = CalculateSecondSmoothing(i, cs1, as1);

        // Calculate TSI
        double tsi = as2 != 0
            ? 100d * (cs2 / as2)
            : double.NaN;

        // Calculate signal line
        double signal = CalculateSignal(i, tsi);

        // Candidate result
        TsiResult r = new(
            Timestamp: item.Timestamp,
            Tsi: tsi.NaN2Null(),
            Signal: signal.NaN2Null());

        return (r, i);
    }

    private (double cs1, double as1) CalculateFirstSmoothing(int index, double change, double absChange)
    {
        // Get previous first smoothing values from cache (if available)
        double prevCs1 = double.NaN;
        double prevAs1 = double.NaN;

        if (index > 1)
        {
            // Need to recalculate cs1/as1 from previous index
            (prevCs1, prevAs1) = RecalculateFirstSmoothingAtIndex(index - 1);
        }

        // re/initialize first smoothing
        if (double.IsNaN(prevCs1) && index >= LookbackPeriods)
        {
            double sumC = 0;
            double sumA = 0;
            for (int p = index - LookbackPeriods + 1; p <= index; p++)
            {
                double pValue = ProviderCache[p].Value;
                double pPrevValue = ProviderCache[p - 1].Value;
                double pChange = pValue - pPrevValue;
                sumC += pChange;
                sumA += Math.Abs(pChange);
            }
            return (sumC / LookbackPeriods, sumA / LookbackPeriods);
        }
        // normal first smoothing
        else if (!double.IsNaN(prevCs1))
        {
            double cs1 = ((change - prevCs1) * mult1) + prevCs1;
            double as1 = ((absChange - prevAs1) * mult1) + prevAs1;
            return (cs1, as1);
        }

        return (double.NaN, double.NaN);
    }

    private (double cs1, double as1) RecalculateFirstSmoothingAtIndex(int index)
    {
        if (index == 0)
        {
            return (double.NaN, double.NaN);
        }

        double tempCs1 = double.NaN;
        double tempAs1 = double.NaN;

        for (int p = 1; p <= index; p++)
        {
            double pValue = ProviderCache[p].Value;
            double pPrevValue = ProviderCache[p - 1].Value;
            double pChange = pValue - pPrevValue;
            double pAbsChange = Math.Abs(pChange);

            if (double.IsNaN(tempCs1) && p >= LookbackPeriods)
            {
                double sumC = 0;
                double sumA = 0;
                for (int j = p - LookbackPeriods + 1; j <= p; j++)
                {
                    double jValue = ProviderCache[j].Value;
                    double jPrevValue = ProviderCache[j - 1].Value;
                    sumC += jValue - jPrevValue;
                    sumA += Math.Abs(jValue - jPrevValue);
                }
                tempCs1 = sumC / LookbackPeriods;
                tempAs1 = sumA / LookbackPeriods;
            }
            else if (!double.IsNaN(tempCs1))
            {
                tempCs1 = ((pChange - tempCs1) * mult1) + tempCs1;
                tempAs1 = ((pAbsChange - tempAs1) * mult1) + tempAs1;
            }
        }

        return (tempCs1, tempAs1);
    }

    private (double cs2, double as2) CalculateSecondSmoothing(int index, double cs1, double as1)
    {
        // Get previous second smoothing values from cache (if available)
        double prevCs2 = double.NaN;
        double prevAs2 = double.NaN;

        if (index > 1)
        {
            // Need to recalculate cs2/as2 from previous index
            (prevCs2, prevAs2) = RecalculateSecondSmoothingAtIndex(index - 1);
        }

        // re/initialize second smoothing
        if (double.IsNaN(prevCs2) && index >= SmoothPeriods && !double.IsNaN(cs1))
        {
            double sumCs = 0;
            double sumAs = 0;
            for (int p = index - SmoothPeriods + 1; p <= index; p++)
            {
                // Recalculate cs1/as1 for this window
                (double pCs1, double pAs1) = RecalculateFirstSmoothingAtIndex(p);
                sumCs += pCs1;
                sumAs += pAs1;
            }
            return (sumCs / SmoothPeriods, sumAs / SmoothPeriods);
        }
        // normal second smoothing
        else if (!double.IsNaN(prevCs2))
        {
            double cs2 = ((cs1 - prevCs2) * mult2) + prevCs2;
            double as2 = ((as1 - prevAs2) * mult2) + prevAs2;
            return (cs2, as2);
        }

        return (double.NaN, double.NaN);
    }

    private (double cs2, double as2) RecalculateSecondSmoothingAtIndex(int index)
    {
        if (index == 0)
        {
            return (double.NaN, double.NaN);
        }

        double tempCs2 = double.NaN;
        double tempAs2 = double.NaN;

        for (int p = 1; p <= index; p++)
        {
            (double pCs1, double pAs1) = RecalculateFirstSmoothingAtIndex(p);

            if (double.IsNaN(tempCs2) && p >= SmoothPeriods && !double.IsNaN(pCs1))
            {
                double sumCs = 0;
                double sumAs = 0;
                for (int j = p - SmoothPeriods + 1; j <= p; j++)
                {
                    (double jCs1, double jAs1) = RecalculateFirstSmoothingAtIndex(j);
                    sumCs += jCs1;
                    sumAs += jAs1;
                }
                tempCs2 = sumCs / SmoothPeriods;
                tempAs2 = sumAs / SmoothPeriods;
            }
            else if (!double.IsNaN(tempCs2))
            {
                tempCs2 = ((pCs1 - tempCs2) * mult2) + tempCs2;
                tempAs2 = ((pAs1 - tempAs2) * mult2) + tempAs2;
            }
        }

        return (tempCs2, tempAs2);
    }

    private double CalculateSignal(int index, double tsi)
    {
        if (SignalPeriods > 1)
        {
            double prevSignal = index > 0 ? Cache[index - 1].Signal ?? double.NaN : double.NaN;

            // re/initialize signal
            if (double.IsNaN(prevSignal) && index > SignalPeriods)
            {
                double sum = tsi;
                for (int p = index - SignalPeriods + 1; p < index; p++)
                {
                    sum += Cache[p].Tsi.Null2NaN();
                }
                return sum / SignalPeriods;
            }
            // normal signal
            else if (!double.IsNaN(prevSignal))
            {
                return ((tsi - prevSignal) * multS) + prevSignal;
            }
        }
        else if (SignalPeriods == 1)
        {
            return tsi;
        }

        return double.NaN;
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // No member state to roll back - all state is derived from cache
    }
}


public static partial class Tsi
{
    /// <summary>
    /// Creates a TSI streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback calculation.</param>
    /// <param name="smoothPeriods">The number of periods for the smoothing calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal calculation.</param>
    /// <returns>A TSI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    public static TsiHub ToTsiHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
        => new(chainProvider, lookbackPeriods, smoothPeriods, signalPeriods);

    /// <summary>
    /// Creates a TSI hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback calculation.</param>
    /// <param name="smoothPeriods">The number of periods for the smoothing calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal calculation.</param>
    /// <returns>An instance of <see cref="TsiHub"/>.</returns>
    public static TsiHub ToTsiHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToTsiHub(lookbackPeriods, smoothPeriods, signalPeriods);
    }
}
