namespace Skender.Stock.Indicators;

/// <summary>
/// MESA Adaptive Moving Average (MAMA) from incremental reusable values.
/// </summary>
public class MamaList : BufferList<MamaResult>, IBufferReusable, IMama
{
    // Internal state arrays matching StaticSeries implementation
    // These arrays grow with each added value to support indexed lookback access
    private readonly List<double> pr = []; // price (HL2 when quote)
    private readonly List<double> sm = []; // smooth
    private readonly List<double> dt = []; // detrender
    private readonly List<double> pd = []; // period
    private readonly List<double> q1 = []; // quadrature
    private readonly List<double> i1 = []; // in-phase
    private readonly List<double> q2 = []; // adj. quadrature
    private readonly List<double> i2 = []; // adj. in-phase
    private readonly List<double> re = [];
    private readonly List<double> im = [];
    private readonly List<double> ph = []; // phase

    private double prevMama = double.NaN;
    private double prevFama = double.NaN;

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

    /// <summary>
    /// Initializes a new instance of the <see cref="MamaList"/> class with initial quotes.
    /// </summary>
    /// <param name="fastLimit">The fast limit for the MAMA calculation.</param>
    /// <param name="slowLimit">The slow limit for the MAMA calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public MamaList(
        double fastLimit,
        double slowLimit,
        IReadOnlyList<IQuote> quotes)
        : this(fastLimit, slowLimit)
        => Add(quotes);

    /// <inheritdoc />
    public double FastLimit { get; init; }

    /// <inheritdoc />
    public double SlowLimit { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        int i = pr.Count; // new index position

        // append base arrays with placeholders
        pr.Add(value);
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

        double mama;
        double fama;

        // skip until we have 6 price points (index 0..5)
        if (i < 5)
        {
            AddInternal(new MamaResult(timestamp));
            return;
        }

        // initialization at first calculable index
        if (double.IsNaN(prevMama))
        {
            double sum = 0;
            for (int p = i - 5; p <= i; p++) { sum += pr[p]; }
            mama = fama = sum / 6d;
            prevMama = mama;
            prevFama = fama;
            AddInternal(new MamaResult(timestamp, mama, fama));
            return;
        }

        // normal MAMA calculation (mirrors StaticSeries & StreamHub)
        double adj = (0.075 * pd[i - 1]) + 0.54;

        // Smooth & detrender
        sm[i] = ((4 * pr[i]) + (3 * pr[i - 1]) + (2 * pr[i - 2]) + pr[i - 3]) / 10d;
        dt[i] = ((0.0962 * sm[i]) + (0.5769 * sm[i - 2]) - (0.5769 * sm[i - 4]) - (0.0962 * sm[i - 6])) * adj;

        // In-phase & quadrature
        q1[i] = ((0.0962 * dt[i]) + (0.5769 * dt[i - 2]) - (0.5769 * dt[i - 4]) - (0.0962 * dt[i - 6])) * adj;
        i1[i] = dt[i - 3];

        // Advance phases by 90 degrees
        double jI = ((0.0962 * i1[i]) + (0.5769 * i1[i - 2]) - (0.5769 * i1[i - 4]) - (0.0962 * i1[i - 6])) * adj;
        double jQ = ((0.0962 * q1[i]) + (0.5769 * q1[i - 2]) - (0.5769 * q1[i - 4]) - (0.0962 * q1[i - 6])) * adj;

        // Phasor addition for 3-bar averaging
        i2[i] = i1[i] - jQ;
        q2[i] = q1[i] + jI;
        i2[i] = (0.2 * i2[i]) + (0.8 * i2[i - 1]);
        q2[i] = (0.2 * q2[i]) + (0.8 * q2[i - 1]);

        // Homodyne discriminator
        re[i] = (i2[i] * i2[i - 1]) + (q2[i] * q2[i - 1]);
        im[i] = (i2[i] * q2[i - 1]) - (q2[i] * i2[i - 1]);
        re[i] = (0.2 * re[i]) + (0.8 * re[i - 1]);
        im[i] = (0.2 * im[i]) + (0.8 * im[i - 1]);

        // Period calculation & constraints
        pd[i] = im[i] != 0 && re[i] != 0 ? 2 * Math.PI / Math.Atan(im[i] / re[i]) : 0d;
        pd[i] = pd[i] > 1.5 * pd[i - 1] ? 1.5 * pd[i - 1] : pd[i];
        pd[i] = pd[i] < 0.67 * pd[i - 1] ? 0.67 * pd[i - 1] : pd[i];
        pd[i] = pd[i] < 6 ? 6 : pd[i];
        pd[i] = pd[i] > 50 ? 50 : pd[i];
        pd[i] = (0.2 * pd[i]) + (0.8 * pd[i - 1]);

        // Phase & delta
        ph[i] = i1[i] != 0 ? Math.Atan(q1[i] / i1[i]) * 180d / Math.PI : 0d;
        double delta = Math.Max(ph[i - 1] - ph[i], 1d);

        // Adaptive alpha & final outputs
        double alpha = Math.Max(FastLimit / delta, SlowLimit);
        mama = (alpha * pr[i]) + ((1d - alpha) * prevMama);
        fama = (0.5 * alpha * mama) + ((1d - (0.5 * alpha)) * prevFama);

        prevMama = mama;
        prevFama = fama;

        AddInternal(new MamaResult(timestamp, mama.NaN2Null(), fama.NaN2Null()));
    }

    /// <inheritdoc />
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        // prefer HL2 when source is an IQuote to match StaticSeries behavior
        double val = value is IQuote q ? (double)(q.High + q.Low) / 2d : value.Value;
        Add(value.Timestamp, val);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            IReusable v = values[i];
            double val = v is IQuote q ? (double)(q.High + q.Low) / 2d : v.Value;
            Add(v.Timestamp, val);
        }
    }

    /// <inheritdoc />
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        // Calculate HL2 for MAMA algorithm (as per MAMA specification)
        double hl2 = (double)(quote.High + quote.Low) / 2;
        Add(quote.Timestamp, hl2);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            IQuote quote = quotes[i];
            // Calculate HL2 for MAMA algorithm (as per MAMA specification)
            double hl2 = (double)(quote.High + quote.Low) / 2;
            Add(quote.Timestamp, hl2);
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        ClearInternal();
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
        prevMama = double.NaN;
        prevFama = double.NaN;
    }
}

public static partial class Mama
{
    /// <summary>
    /// Creates a buffer list for MESA Adaptive Moving Average (MAMA) calculations.
    /// </summary>
    public static MamaList ToMamaList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        double fastLimit = 0.5,
        double slowLimit = 0.05)
        where TQuote : IQuote
        => new(fastLimit, slowLimit) { (IReadOnlyList<IQuote>)quotes };
}
