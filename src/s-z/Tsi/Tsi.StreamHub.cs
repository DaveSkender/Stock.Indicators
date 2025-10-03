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

        // Calculate price change
        double priceChange = item.Value - ProviderCache[i - 1].Value;
        double absChange = Math.Abs(priceChange);

        // Calculate first smoothing (cs1, as1) - EMA of price change
        double cs1 = double.NaN;
        double as1 = double.NaN;

        if (i >= LookbackPeriods)
        {
            if (i > LookbackPeriods)
            {
                // Get previous smoothed values from cache
                // We need to cache these intermediate values for proper calculation
                // For now, we'll recalculate from the data
                cs1 = CalculateCs1(i, priceChange);
                as1 = CalculateAs1(i, absChange);
            }
            else
            {
                // Initialize as SMA
                (cs1, as1) = InitializeFirstSmooth(i);
            }
        }

        // Calculate second smoothing (cs2, as2) - EMA of first smoothed values
        double cs2 = double.NaN;
        double as2 = double.NaN;

        if (!double.IsNaN(cs1) && !double.IsNaN(as1) && i >= LookbackPeriods + SmoothPeriods - 1)
        {
            if (i > LookbackPeriods + SmoothPeriods - 1)
            {
                // Calculate EMA of smoothed values
                cs2 = CalculateCs2(i, cs1);
                as2 = CalculateAs2(i, as1);
            }
            else
            {
                // Initialize as SMA
                (cs2, as2) = InitializeSecondSmooth(i);
            }
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

    private double CalculateCs1(int index, double priceChange)
    {
        // Get previous cs1 value - we need to reconstruct it
        double prevCs1 = ReconstructCs1(index - 1);
        return ((priceChange - prevCs1) * Mult1) + prevCs1;
    }

    private double CalculateAs1(int index, double absChange)
    {
        // Get previous as1 value - we need to reconstruct it
        double prevAs1 = ReconstructAs1(index - 1);
        return ((absChange - prevAs1) * Mult1) + prevAs1;
    }

    private double ReconstructCs1(int index)
    {
        // Reconstruct cs1 at index by calculating from initialization point
        if (index < LookbackPeriods) return double.NaN;

        // Initialize
        double sumC = 0;
        for (int p = 1; p <= LookbackPeriods; p++)
        {
            sumC += ProviderCache[p].Value - ProviderCache[p - 1].Value;
        }
        double cs1 = sumC / LookbackPeriods;

        // Apply EMA from initialization to target index
        for (int i = LookbackPeriods + 1; i <= index; i++)
        {
            double c = ProviderCache[i].Value - ProviderCache[i - 1].Value;
            cs1 = ((c - cs1) * Mult1) + cs1;
        }

        return cs1;
    }

    private double ReconstructAs1(int index)
    {
        // Reconstruct as1 at index by calculating from initialization point
        if (index < LookbackPeriods) return double.NaN;

        // Initialize
        double sumA = 0;
        for (int p = 1; p <= LookbackPeriods; p++)
        {
            sumA += Math.Abs(ProviderCache[p].Value - ProviderCache[p - 1].Value);
        }
        double as1 = sumA / LookbackPeriods;

        // Apply EMA from initialization to target index
        for (int i = LookbackPeriods + 1; i <= index; i++)
        {
            double a = Math.Abs(ProviderCache[i].Value - ProviderCache[i - 1].Value);
            as1 = ((a - as1) * Mult1) + as1;
        }

        return as1;
    }

    private (double cs2, double as2) InitializeSecondSmooth(int index)
    {
        double sumCs = 0;
        double sumAs = 0;

        for (int p = index - SmoothPeriods + 1; p <= index; p++)
        {
            double cs1 = ReconstructCs1(p);
            double as1 = ReconstructAs1(p);
            sumCs += cs1;
            sumAs += as1;
        }

        return (sumCs / SmoothPeriods, sumAs / SmoothPeriods);
    }

    private double CalculateCs2(int index, double cs1)
    {
        // Get previous cs2 value - reconstruct from cached TSI
        double prevCs2 = ReconstructCs2(index - 1);
        return ((cs1 - prevCs2) * Mult2) + prevCs2;
    }

    private double CalculateAs2(int index, double as1)
    {
        // Get previous as2 value - reconstruct from cached TSI
        double prevAs2 = ReconstructAs2(index - 1);
        return ((as1 - prevAs2) * Mult2) + prevAs2;
    }

    private double ReconstructCs2(int index)
    {
        // Reconstruct cs2 at index
        if (index < LookbackPeriods + SmoothPeriods - 1) return double.NaN;

        // Initialize second smooth
        double sumCs = 0;
        for (int p = LookbackPeriods; p < LookbackPeriods + SmoothPeriods; p++)
        {
            sumCs += ReconstructCs1(p);
        }
        double cs2 = sumCs / SmoothPeriods;

        // Apply EMA from initialization to target index
        for (int i = LookbackPeriods + SmoothPeriods; i <= index; i++)
        {
            double cs1 = ReconstructCs1(i);
            cs2 = ((cs1 - cs2) * Mult2) + cs2;
        }

        return cs2;
    }

    private double ReconstructAs2(int index)
    {
        // Reconstruct as2 at index
        if (index < LookbackPeriods + SmoothPeriods - 1) return double.NaN;

        // Initialize second smooth
        double sumAs = 0;
        for (int p = LookbackPeriods; p < LookbackPeriods + SmoothPeriods; p++)
        {
            sumAs += ReconstructAs1(p);
        }
        double as2 = sumAs / SmoothPeriods;

        // Apply EMA from initialization to target index
        for (int i = LookbackPeriods + SmoothPeriods; i <= index; i++)
        {
            double as1 = ReconstructAs1(i);
            as2 = ((as1 - as2) * Mult2) + as2;
        }

        return as2;
    }
}
