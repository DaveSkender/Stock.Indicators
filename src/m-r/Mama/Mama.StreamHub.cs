namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the MESA Adaptive Moving Average (MAMA) indicator.
/// </summary>
public static partial class Mama
{
    /// <summary>
    /// Creates a MAMA hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="fastLimit">The fast limit for the MAMA calculation.</param>
    /// <param name="slowLimit">The slow limit for the MAMA calculation.</param>
    /// <returns>A MAMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the limits are invalid.</exception>
    public static MamaHub<T> ToMama<T>(
        this IChainProvider<T> chainProvider,
        double fastLimit = 0.5,
        double slowLimit = 0.05)
        where T : IReusable
        => new(chainProvider, fastLimit, slowLimit);
}

/// <summary>
/// Represents a hub for MESA Adaptive Moving Average (MAMA) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class MamaHub<TIn>
    : ChainProvider<TIn, MamaResult>, IMama
    where TIn : IReusable
{
    private readonly string hubName;

    // State tracking
    private double prevMama = double.NaN;
    private double prevFama = double.NaN;

    // Arrays for algorithm state (using List for dynamic sizing)
    // Note: MAMA requires historical data arrays that grow with each quote
    private readonly List<double> pr = new(); // price
    private readonly List<double> sm = new(); // smooth
    private readonly List<double> dt = new(); // detrender
    private readonly List<double> pd = new(); // period

    private readonly List<double> q1 = new(); // quadrature
    private readonly List<double> i1 = new(); // in-phase

    private readonly List<double> q2 = new(); // adj. quadrature
    private readonly List<double> i2 = new(); // adj. in-phase

    private readonly List<double> re = new();
    private readonly List<double> im = new();

    private readonly List<double> ph = new(); // phase

    /// <summary>
    /// Initializes a new instance of the <see cref="MamaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="fastLimit">The fast limit for the MAMA calculation.</param>
    /// <param name="slowLimit">The slow limit for the MAMA calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the limits are invalid.</exception>
    internal MamaHub(
        IChainProvider<TIn> provider,
        double fastLimit,
        double slowLimit) : base(provider)
    {
        Mama.Validate(fastLimit, slowLimit);
        FastLimit = fastLimit;
        SlowLimit = slowLimit;
        hubName = $"MAMA({fastLimit},{slowLimit})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public double FastLimit { get; init; }

    /// <inheritdoc/>
    public double SlowLimit { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (MamaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Add value to price array
        if (i >= pr.Count)
        {
            pr.Add(item.Value);

            // Initialize arrays for new position
            sm.Add(0);
            dt.Add(0);
            pd.Add(0);
            q1.Add(0);
            i1.Add(0);
            q2.Add(0);
            i2.Add(0);
            re.Add(0);
            im.Add(0);
            ph.Add(0);
        }
        else
        {
            pr[i] = item.Value;
        }

        double mama = double.NaN;
        double fama = double.NaN;

        // Skip incalculable periods
        if (i >= 5)
        {
            // Initialization
            if (double.IsNaN(prevMama))
            {
                double sum = 0;
                for (int p = i - 5; p <= i; p++)
                {
                    pd[p] = 0;
                    sm[p] = 0;
                    dt[p] = 0;

                    i1[p] = 0;
                    q1[p] = 0;
                    i2[p] = 0;
                    q2[p] = 0;

                    re[p] = 0;
                    im[p] = 0;

                    ph[p] = 0;

                    sum += pr[p];
                }

                mama = fama = sum / 6;
            }
            // Normal MAMA calculation
            else
            {
                double adj = (0.075 * pd[i - 1]) + 0.54;

                // Smooth and detrender
                sm[i] = ((4 * pr[i]) + (3 * pr[i - 1]) + (2 * pr[i - 2]) + pr[i - 3]) / 10;

                if (i >= 6)
                {
                    dt[i] = ((0.0962 * sm[i]) + (0.5769 * sm[i - 2]) - (0.5769 * sm[i - 4]) - (0.0962 * sm[i - 6])) * adj;
                }

                // In-phase and quadrature
                if (i >= 6)
                {
                    q1[i] = ((0.0962 * dt[i]) + (0.5769 * dt[i - 2]) - (0.5769 * dt[i - 4]) - (0.0962 * dt[i - 6])) * adj;
                    i1[i] = dt[i - 3];
                }

                // Advance the phases by 90 degrees
                double jI = 0;
                double jQ = 0;

                if (i >= 6)
                {
                    jI = ((0.0962 * i1[i]) + (0.5769 * i1[i - 2]) - (0.5769 * i1[i - 4]) - (0.0962 * i1[i - 6])) * adj;
                    jQ = ((0.0962 * q1[i]) + (0.5769 * q1[i - 2]) - (0.5769 * q1[i - 4]) - (0.0962 * q1[i - 6])) * adj;
                }

                // Phasor addition for 3-bar averaging
                i2[i] = i1[i] - jQ;
                q2[i] = q1[i] + jI;

                if (i > 0)
                {
                    i2[i] = (0.2 * i2[i]) + (0.8 * i2[i - 1]);  // smoothing it
                    q2[i] = (0.2 * q2[i]) + (0.8 * q2[i - 1]);
                }

                // Homodyne discriminator
                if (i > 0)
                {
                    re[i] = (i2[i] * i2[i - 1]) + (q2[i] * q2[i - 1]);
                    im[i] = (i2[i] * q2[i - 1]) - (q2[i] * i2[i - 1]);
                }

                if (i > 0)
                {
                    re[i] = (0.2 * re[i]) + (0.8 * re[i - 1]);  // smoothing it
                    im[i] = (0.2 * im[i]) + (0.8 * im[i - 1]);
                }

                // Calculate period
                pd[i] = im[i] != 0 && re[i] != 0
                    ? 2 * Math.PI / Math.Atan(im[i] / re[i])
                    : 0;

                // Adjust period to thresholds
                if (i > 0)
                {
                    pd[i] = pd[i] > 1.5 * pd[i - 1] ? 1.5 * pd[i - 1] : pd[i];
                    pd[i] = pd[i] < 0.67 * pd[i - 1] ? 0.67 * pd[i - 1] : pd[i];
                }

                pd[i] = pd[i] < 6 ? 6 : pd[i];
                pd[i] = pd[i] > 50 ? 50 : pd[i];

                // Smooth the period
                if (i > 0)
                {
                    pd[i] = (0.2 * pd[i]) + (0.8 * pd[i - 1]);
                }

                // Determine phase position
                ph[i] = i1[i] != 0 ? Math.Atan(q1[i] / i1[i]) * 180 / Math.PI : 0;

                // Change in phase
                double delta = Math.Max(i > 0 ? ph[i - 1] - ph[i] : 1, 1);

                // Adaptive alpha value
                double alpha = Math.Max(FastLimit / delta, SlowLimit);

                // Final indicators
                mama = (alpha * pr[i]) + ((1d - alpha) * prevMama);
                fama = (0.5 * alpha * mama) + ((1d - (0.5 * alpha)) * prevFama);
            }

            prevMama = mama;
            prevFama = fama;
        }

        // candidate result
        MamaResult r = new(
            Timestamp: item.Timestamp,
            Mama: mama.NaN2Null(),
            Fama: fama.NaN2Null());

        return (r, i);
    }
}