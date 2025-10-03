namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the True Strength Index (TSI) indicator.
/// </summary>
public static partial class Tsi
{
    /// <summary>
    /// Creates a TSI hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback calculation. Default is 25.</param>
    /// <param name="smoothPeriods">The number of periods for the smoothing calculation. Default is 13.</param>
    /// <param name="signalPeriods">The number of periods for the signal calculation. Default is 7.</param>
    /// <returns>A TSI hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    public static TsiHub<T> ToTsi<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods = 25,
        int smoothPeriods = 13,
        int signalPeriods = 7)
        where T : IReusable
        => new(chainProvider, lookbackPeriods, smoothPeriods, signalPeriods);
}

/// <summary>
/// Represents a hub for TSI (True Strength Index) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class TsiHub<TIn>
    : ChainProvider<TIn, TsiResult>, ITsi
    where TIn : IReusable
{
    private readonly string hubName;

    // State variables for incremental calculations
    private double lastCs1 = double.NaN;
    private double lastAs1 = double.NaN;
    private double lastCs2 = double.NaN;
    private double lastAs2 = double.NaN;

    /// <summary>
    /// Initializes a new instance of the <see cref="TsiHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods for the lookback calculation.</param>
    /// <param name="smoothPeriods">The number of periods for the smoothing calculation.</param>
    /// <param name="signalPeriods">The number of periods for the signal calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any of the parameters are invalid.</exception>
    internal TsiHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods,
        int smoothPeriods,
        int signalPeriods) : base(provider)
    {
        Tsi.Validate(lookbackPeriods, smoothPeriods, signalPeriods);
        LookbackPeriods = lookbackPeriods;
        SmoothPeriods = smoothPeriods;
        SignalPeriods = signalPeriods;

        Mult1 = 2d / (lookbackPeriods + 1);
        Mult2 = 2d / (smoothPeriods + 1);
        MultS = 2d / (signalPeriods + 1);

        hubName = $"TSI({lookbackPeriods},{smoothPeriods},{signalPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public int SmoothPeriods { get; init; }

    /// <inheritdoc/>
    public int SignalPeriods { get; init; }

    /// <summary>
    /// Gets the smoothing factor for the first smoothing.
    /// </summary>
    public double Mult1 { get; private init; }

    /// <summary>
    /// Gets the smoothing factor for the second smoothing.
    /// </summary>
    public double Mult2 { get; private init; }

    /// <summary>
    /// Gets the smoothing factor for the signal line.
    /// </summary>
    public double MultS { get; private init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (TsiResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Skip first period (need previous value for price change)
        if (i == 0)
        {
            TsiResult r0 = new(
                Timestamp: item.Timestamp,
                Tsi: null,
                Signal: null);
            return (r0, i);
        }

        // If out-of-order change occurred, backfill from previous cached state
        if (i > 0 && Cache.Count > i && Cache[i - 1].Tsi is not null
            && (double.IsNaN(lastCs1) || double.IsNaN(lastCs2)))
        {
            RollbackState(item.Timestamp);
        }

        // Calculate price change
        double priceChange = item.Value - ProviderCache[i - 1].Value;
        double absChange = Math.Abs(priceChange);

        // Calculate first smoothing (cs1, as1)
        double cs1 = double.NaN;
        double as1 = double.NaN;

        if (i >= LookbackPeriods)
        {
            if (i > LookbackPeriods && !double.IsNaN(lastCs1))
            {
                // Normal EMA calculation
                cs1 = ((priceChange - lastCs1) * Mult1) + lastCs1;
                as1 = ((absChange - lastAs1) * Mult1) + lastAs1;
            }
            else
            {
                // Initialize as SMA
                (cs1, as1) = InitializeFirstSmooth(i);
            }

            lastCs1 = cs1;
            lastAs1 = as1;
        }

        // Calculate second smoothing (cs2, as2)
        double cs2 = double.NaN;
        double as2 = double.NaN;

        if (!double.IsNaN(cs1) && !double.IsNaN(as1) && i >= LookbackPeriods + SmoothPeriods - 1)
        {
            if (i > LookbackPeriods + SmoothPeriods - 1 && !double.IsNaN(lastCs2))
            {
                // Normal EMA calculation
                cs2 = ((cs1 - lastCs2) * Mult2) + lastCs2;
                as2 = ((as1 - lastAs2) * Mult2) + lastAs2;
            }
            else
            {
                // Initialize as SMA
                (cs2, as2) = InitializeSecondSmooth(i);
            }

            lastCs2 = cs2;
            lastAs2 = as2;
        }

        // Calculate TSI
        double tsi = !double.IsNaN(cs2) && !double.IsNaN(as2) && as2 != 0
            ? 100d * (cs2 / as2)
            : double.NaN;

        // Calculate Signal line
        double signal;
        if (SignalPeriods > 1 && !double.IsNaN(tsi))
        {
            int minSignalIndex = LookbackPeriods + SmoothPeriods + SignalPeriods - 2;
            if (i >= minSignalIndex && (i == minSignalIndex || Cache[i - 1].Signal is null))
            {
                // Initialize signal as SMA of TSI values
                double sum = tsi;
                for (int j = i - SignalPeriods + 1; j < i; j++)
                {
                    sum += Cache[j].Tsi ?? double.NaN;
                }
                signal = sum / SignalPeriods;
            }
            else
            {
                // Calculate signal EMA normally
                signal = Ema.Increment(MultS, i > 0 ? Cache[i - 1].Signal ?? double.NaN : double.NaN, tsi);
            }
        }
        else if (SignalPeriods == 1)
        {
            signal = tsi;
        }
        else
        {
            signal = double.NaN;
        }

        // Candidate result
        TsiResult r = new(
            Timestamp: item.Timestamp,
            Tsi: tsi.NaN2Null(),
            Signal: signal.NaN2Null());

        return (r, i);
    }

    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        int i = ProviderCache.IndexGte(timestamp);
        if (i > LookbackPeriods + SmoothPeriods - 1)
        {
            // Replay from the change point in the timeline
            // Initialize state from scratch and replay through to the rollback point
            lastCs1 = lastAs1 = lastCs2 = lastAs2 = double.NaN;

            // Replay from first valid point to rebuild state
            for (int j = 1; j <= i - 1; j++)
            {
                if (j >= LookbackPeriods)
                {
                    double c = ProviderCache[j].Value - ProviderCache[j - 1].Value;
                    double a = Math.Abs(c);

                    if (double.IsNaN(lastCs1))
                    {
                        (lastCs1, lastAs1) = InitializeFirstSmooth(j);
                    }
                    else
                    {
                        lastCs1 = ((c - lastCs1) * Mult1) + lastCs1;
                        lastAs1 = ((a - lastAs1) * Mult1) + lastAs1;
                    }
                }

                if (j >= LookbackPeriods + SmoothPeriods - 1 && !double.IsNaN(lastCs1))
                {
                    if (double.IsNaN(lastCs2))
                    {
                        (lastCs2, lastAs2) = InitializeSecondSmooth(j);
                    }
                    else
                    {
                        lastCs2 = ((lastCs1 - lastCs2) * Mult2) + lastCs2;
                        lastAs2 = ((lastAs1 - lastAs2) * Mult2) + lastAs2;
                    }
                }
            }
        }
        else
        {
            lastCs1 = lastAs1 = lastCs2 = lastAs2 = double.NaN;
        }
    }

    private (double cs1, double as1) InitializeFirstSmooth(int index)
    {
        double sumC = 0;
        double sumA = 0;

        for (int p = index - LookbackPeriods + 1; p <= index; p++)
        {
            if (p > 0)
            {
                double c = ProviderCache[p].Value - ProviderCache[p - 1].Value;
                sumC += c;
                sumA += Math.Abs(c);
            }
        }

        return (sumC / LookbackPeriods, sumA / LookbackPeriods);
    }

    private (double cs2, double as2) InitializeSecondSmooth(int index)
    {
        double sumCs = 0;
        double sumAs = 0;

        for (int p = index - SmoothPeriods + 1; p <= index; p++)
        {
            // For second smooth initialization, we need the cs1/as1 values at each point
            // These must be calculated incrementally during the rollback/replay
            if (p > 0)
            {
                double c = ProviderCache[p].Value - ProviderCache[p - 1].Value;
                double a = Math.Abs(c);

                // Calculate cs1/as1 for this point
                double cs1Temp, as1Temp;
                if (p == LookbackPeriods)
                {
                    // Initialize
                    double sumC = 0, sumA = 0;
                    for (int q = p - LookbackPeriods + 1; q <= p; q++)
                    {
                        if (q > 0)
                        {
                            sumC += ProviderCache[q].Value - ProviderCache[q - 1].Value;
                            sumA += Math.Abs(ProviderCache[q].Value - ProviderCache[q - 1].Value);
                        }
                    }
                    cs1Temp = sumC / LookbackPeriods;
                    as1Temp = sumA / LookbackPeriods;
                }
                else
                {
                    // Use incrementally calculated values from state
                    // This is called during initialization, so we can't rely on lastCs1
                    // Need to calculate from scratch
                    cs1Temp = 0;
                    as1Temp = 0;
                    double tempCs1 = double.NaN, tempAs1 = double.NaN;
                    for (int q = 1; q <= p; q++)
                    {
                        if (q >= LookbackPeriods)
                        {
                            double cq = ProviderCache[q].Value - ProviderCache[q - 1].Value;
                            double aq = Math.Abs(cq);
                            if (double.IsNaN(tempCs1))
                            {
                                double sumC = 0, sumA = 0;
                                for (int r = q - LookbackPeriods + 1; r <= q; r++)
                                {
                                    if (r > 0)
                                    {
                                        sumC += ProviderCache[r].Value - ProviderCache[r - 1].Value;
                                        sumA += Math.Abs(ProviderCache[r].Value - ProviderCache[r - 1].Value);
                                    }
                                }
                                tempCs1 = sumC / LookbackPeriods;
                                tempAs1 = sumA / LookbackPeriods;
                            }
                            else
                            {
                                tempCs1 = ((cq - tempCs1) * Mult1) + tempCs1;
                                tempAs1 = ((aq - tempAs1) * Mult1) + tempAs1;
                            }
                        }
                    }
                    cs1Temp = tempCs1;
                    as1Temp = tempAs1;
                }

                sumCs += cs1Temp;
                sumAs += as1Temp;
            }
        }

        return (sumCs / SmoothPeriods, sumAs / SmoothPeriods);
    }
}
