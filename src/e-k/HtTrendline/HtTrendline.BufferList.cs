namespace Skender.Stock.Indicators;

/// <summary>
/// Hilbert Transform Instantaneous Trendline (HTL) from incremental reusable values.
/// </summary>
/// <remarks>
/// <para>
/// <b>Exception to BufferUtilities Pattern:</b> HtTrendline does not use the standard
/// <see cref="BufferListUtilities"/> extension methods (Update/UpdateWithDequeue) due to
/// the unique complexity of the Hilbert Transform algorithm.
/// </para>
/// <para>
/// <b>Algorithmic Requirements:</b>
/// <list type="bullet">
/// <item>Maintains 12 parallel state arrays (pr, sp, dt, pd, q1, i1, q2, i2, re, im, sd, it)
/// for different phases of the Hilbert Transform calculation</item>
/// <item>Requires indexed lookback access up to 6 periods (e.g., <c>sp[i - 6]</c>)
/// for phase calculations</item>
/// <item>Requires dynamic lookback based on dominant cycle period (dcPeriods) which can be up to 50 periods</item>
/// </list>
/// </para>
/// <para>
/// <b>Why Queue&lt;T&gt; Cannot Be Used:</b> The standard <see cref="Queue{T}"/> data structure
/// used by BufferUtilities does not provide indexed access to historical values, which is
/// essential for the Hilbert Transform algorithm's phase and period calculations.
/// </para>
/// <para>
/// <b>Memory Management:</b> This implementation uses <see cref="List{T}"/> arrays to maintain
/// calculation state with automatic pruning to prevent unbounded growth:
/// <list type="bullet">
/// <item>State arrays are pruned at 1000 items, keeping minimum 51 periods for calculations</item>
/// <item>Result list is pruned at <see cref="BufferList{HtlResult}.MaxListSize"/> (default 90% of int.MaxValue)</item>
/// </list>
/// </para>
/// </remarks>
public class HtTrendlineList : BufferList<HtlResult>, IIncrementFromChain
{
    /// <summary>
    /// Internal state arrays matching StaticSeries implementation
    /// These arrays grow with each added value to support indexed lookback access
    /// </summary>
    private readonly List<double> pr = []; // price (HL2 when quote)
    private readonly List<double> sp = []; // smooth price
    private readonly List<double> dt = []; // detrender
    private readonly List<double> pd = []; // period
    private readonly List<double> q1 = []; // quadrature
    private readonly List<double> i1 = []; // in-phase
    private readonly List<double> q2 = []; // adj. quadrature
    private readonly List<double> i2 = []; // adj. in-phase
    private readonly List<double> re = []; // real
    private readonly List<double> im = []; // imaginary
    private readonly List<double> sd = []; // smooth period
    private readonly List<double> it = []; // instantaneous trend

    /// <summary>
    /// Initializes a new instance of the <see cref="HtTrendlineList"/> class.
    /// </summary>
    public HtTrendlineList()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HtTrendlineList"/> class with initial reusable values.
    /// </summary>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public HtTrendlineList(IReadOnlyList<IReusable> values)
        : this()
    {
        Add(values);
    }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        int i = Count; // current index
        pr.Add(value);

        // initialization period (first 6 bars)
        if (i < 6)
        {
            sp.Add(0);
            dt.Add(0);
            q1.Add(0);
            i1.Add(0);
            q2.Add(0);
            i2.Add(0);
            re.Add(0);
            im.Add(0);
            pd.Add(0);
            sd.Add(0);

            AddInternal(new HtlResult(
                Timestamp: timestamp,
                DcPeriods: null,
                Trendline: value.NaN2Null(),
                SmoothPrice: null));
        }
        else
        {
            double adj = (0.075 * pd[i - 1]) + 0.54;

            // smooth and detrender
            sp.Add(((4 * pr[i]) + (3 * pr[i - 1]) + (2 * pr[i - 2]) + pr[i - 3]) / 10);
            dt.Add(((0.0962 * sp[i]) + (0.5769 * sp[i - 2]) - (0.5769 * sp[i - 4]) - (0.0962 * sp[i - 6])) * adj);

            // in-phase and quadrature
            q1.Add(((0.0962 * dt[i]) + (0.5769 * dt[i - 2]) - (0.5769 * dt[i - 4]) - (0.0962 * dt[i - 6])) * adj);
            i1.Add(dt[i - 3]);

            // advance the phases by 90 degrees
            double jI = ((0.0962 * i1[i]) + (0.5769 * i1[i - 2]) - (0.5769 * i1[i - 4]) - (0.0962 * i1[i - 6])) * adj;
            double jQ = ((0.0962 * q1[i]) + (0.5769 * q1[i - 2]) - (0.5769 * q1[i - 4]) - (0.0962 * q1[i - 6])) * adj;

            // phasor addition for 3-bar averaging
            i2.Add(i1[i] - jQ);
            q2.Add(q1[i] + jI);

            i2[i] = (0.2 * i2[i]) + (0.8 * i2[i - 1]);  // smoothing it
            q2[i] = (0.2 * q2[i]) + (0.8 * q2[i - 1]);

            // homodyne discriminator
            re.Add((i2[i] * i2[i - 1]) + (q2[i] * q2[i - 1]));
            im.Add((i2[i] * q2[i - 1]) - (q2[i] * i2[i - 1]));

            re[i] = (0.2 * re[i]) + (0.8 * re[i - 1]);  // smoothing it
            im[i] = (0.2 * im[i]) + (0.8 * im[i - 1]);

            // calculate period
            double pdValue = im[i] != 0 && re[i] != 0
                ? 2 * Math.PI / Math.Atan(im[i] / re[i])
                : 0d;

            // adjust period to thresholds
            pdValue = pdValue > 1.5 * pd[i - 1] ? 1.5 * pd[i - 1] : pdValue;
            pdValue = pdValue < 0.67 * pd[i - 1] ? 0.67 * pd[i - 1] : pdValue;
            pdValue = pdValue < 6d ? 6d : pdValue;
            pdValue = pdValue > 50d ? 50d : pdValue;

            // smooth the period
            pdValue = (0.2 * pdValue) + (0.8 * pd[i - 1]);
            pd.Add(pdValue);

            double sdValue = (0.33 * pd[i]) + (0.67 * sd[i - 1]);
            sd.Add(sdValue);

            // smooth dominant cycle period
            int dcPeriods = (int)(double.IsNaN(sd[i]) ? 0 : sd[i] + 0.5);
            double sumPr = 0;
            for (int d = i - dcPeriods + 1; d <= i; d++)
            {
                if (d >= 0)
                {
                    sumPr += pr[d];
                }
                else
                {
                    // handle insufficient lookback quotes (trim scope)
                    dcPeriods--;
                }
            }

            double itValue = dcPeriods > 0 ? sumPr / dcPeriods : pr[i];
            it.Add(itValue);

            // final indicators
            double? trendline;
            if (i >= 11 && it.Count >= 4) // 12th bar and have enough it values
            {
                int itIdx = it.Count - 1; // current index in it array
                trendline = (((4 * it[itIdx]) + (3 * it[itIdx - 1]) + (2 * it[itIdx - 2]) + it[itIdx - 3]) / 10d).NaN2Null();
            }
            else
            {
                trendline = pr[i].NaN2Null();
            }

            double? smoothPrice = (((4 * pr[i]) + (3 * pr[i - 1]) + (2 * pr[i - 2]) + pr[i - 3]) / 10d).NaN2Null();

            AddInternal(new HtlResult(
                Timestamp: timestamp,
                DcPeriods: dcPeriods > 0 ? dcPeriods : null,
                Trendline: trendline,
                SmoothPrice: smoothPrice));
        }

        // Prune state arrays if they grow too large (keep minimum 51 for max lookback)
        PruneStateArrays();
    }

    private const int MaxBufferSize = 1000;
    private const int MinBufferSize = 51; // Max lookback needed

    /// <summary>
    /// Prunes the internal state arrays to prevent unbounded memory growth.
    /// Removes older data while preserving the minimum required periods for calculations.
    /// </summary>
    private void PruneStateArrays()
    {
        if (pr.Count <= MaxBufferSize)
        {
            return;
        }

        // Calculate how many items to remove
        // Keep at least MinBufferSize (51) elements for lookback calculations
        int removeCount = pr.Count - MinBufferSize;

        if (removeCount > 0)
        {
            RemoveStateRange(removeCount);
        }
    }

    /// <inheritdoc />
    protected override void PruneList()
    {
        int overflow = Count - MaxListSize;

        if (overflow > 0)
        {
            int removable = Math.Min(overflow, Math.Max(0, pr.Count - MinBufferSize));

            if (removable > 0)
            {
                RemoveStateRange(removable);
            }
        }

        base.PruneList();
    }

    private void RemoveStateRange(int removeCount)
    {
        pr.RemoveRange(0, removeCount);
        sp.RemoveRange(0, removeCount);
        dt.RemoveRange(0, removeCount);
        q1.RemoveRange(0, removeCount);
        i1.RemoveRange(0, removeCount);
        q2.RemoveRange(0, removeCount);
        i2.RemoveRange(0, removeCount);
        re.RemoveRange(0, removeCount);
        im.RemoveRange(0, removeCount);
        pd.RemoveRange(0, removeCount);
        sd.RemoveRange(0, removeCount);
        it.RemoveRange(0, removeCount);
    }

    /// <inheritdoc />
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        // prefer HL2 when source is an IQuote to match StaticSeries behavior
        Add(value.Timestamp, value.Hl2OrValue());
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            // prefer HL2 when source is an IQuote to match StaticSeries behavior
            Add(values[i].Timestamp, values[i].Hl2OrValue());
        }
    }

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        pr.Clear();
        sp.Clear();
        dt.Clear();
        q1.Clear();
        i1.Clear();
        q2.Clear();
        i2.Clear();
        re.Clear();
        im.Clear();
        pd.Clear();
        sd.Clear();
        it.Clear();
    }
}

public static partial class HtTrendline
{
    /// <summary>
    /// Creates a buffer list for Hilbert Transform Instantaneous Trendline (HTL) calculations.
    /// </summary>
    /// <param name="source">Collection of input values, time sorted.</param>
    public static HtTrendlineList ToHtTrendlineList(
        this IReadOnlyList<IReusable> source)
        => new() { source };
}
