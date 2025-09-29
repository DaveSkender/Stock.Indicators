namespace Skender.Stock.Indicators;

/// <summary>
/// MESA Adaptive Moving Average (MAMA) from incremental reusable values.
/// </summary>
public class MamaList : List<MamaResult>, IMama, IBufferList, IBufferReusable
{
    // State tracking
    private double prevMama = double.NaN;
    private double prevFama = double.NaN;

    // Arrays for algorithm state (using List for dynamic sizing)
    // Note: MAMA requires historical data arrays that grow with each quote
    // Unlike simple moving averages, this cannot use fixed-size buffers
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
    /// Initializes a new instance of the <see cref="MamaList"/> class.
    /// </summary>
    /// <param name="fastLimit">The fast limit for the MAMA calculation.</param>
    /// <param name="slowLimit">The slow limit for the MAMA calculation.</param>
    public MamaList(
        double fastLimit = 0.5,
        double slowLimit = 0.05)
    {
        Mama.Validate(fastLimit, slowLimit);

        FastLimit = fastLimit;
        SlowLimit = slowLimit;
    }

    /// <inheritdoc />
    public double FastLimit { get; init; }

    /// <inheritdoc />
    public double SlowLimit { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        int i = Count;

        // Add value to price array
        pr.Add(value);

        // Initialize arrays for new position (only once per position)
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

        // Skip incalculable periods
        if (i < 5)
        {
            base.Add(new MamaResult(timestamp));
            return;
        }

        double mama;
        double fama;

        // Initialization at index 5 (first calculable period)
        if (double.IsNaN(prevMama))
        {
            double sum = 0;

            // Reset all values for the initialization range
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
        // Normal MAMA calculation for subsequent periods
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

        base.Add(new MamaResult(
            Timestamp: timestamp,
            Mama: mama.NaN2Null(),
            Fama: fama.NaN2Null()));

        prevMama = mama;
        prevFama = fama;
    }

    /// <inheritdoc />
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i].Timestamp, quotes[i].Value);
        }
    }

    /// <inheritdoc />
    public new void Clear()
    {
        base.Clear();

        // Reset state
        prevMama = double.NaN;
        prevFama = double.NaN;

        // Clear all arrays
        pr.Clear();
        sm.Clear();
        dt.Clear();
        pd.Clear();
        q1.Clear();
        i1.Clear();
        q2.Clear();
        i2.Clear();
        re.Clear();
        im.Clear();
        ph.Clear();
    }
}
