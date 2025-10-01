namespace Skender.Stock.Indicators;

/// <summary>
/// T3 Moving Average from incremental reusable values.
/// </summary>
public class T3List : List<T3Result>, IT3, IBufferList, IBufferReusable
{
    // State for six-layer EMA calculations
    private double _lastE1 = double.NaN;
    private double _lastE2 = double.NaN;
    private double _lastE3 = double.NaN;
    private double _lastE4 = double.NaN;
    private double _lastE5 = double.NaN;
    private double _lastE6 = double.NaN;

    /// <summary>
    /// Initializes a new instance of the <see cref="T3List"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="volumeFactor">The volume factor for the calculation.</param>
    public T3List(
        int lookbackPeriods = 5,
        double volumeFactor = 0.7
    )
    {
        T3.Validate(lookbackPeriods, volumeFactor);
        LookbackPeriods = lookbackPeriods;
        VolumeFactor = volumeFactor;
        K = 2d / (lookbackPeriods + 1);

        double a = volumeFactor;
        C1 = -a * a * a;
        C2 = (3 * a * a) + (3 * a * a * a);
        C3 = (-6 * a * a) - (3 * a) - (3 * a * a * a);
        C4 = 1 + (3 * a) + (a * a * a) + (3 * a * a);
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double VolumeFactor { get; init; }

    /// <inheritdoc/>
    public double K { get; private init; }

    /// <inheritdoc/>
    public double C1 { get; private init; }

    /// <inheritdoc/>
    public double C2 { get; private init; }

    /// <inheritdoc/>
    public double C3 { get; private init; }

    /// <inheritdoc/>
    public double C4 { get; private init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        double e1;
        double e2;
        double e3;
        double e4;
        double e5;
        double e6;

        // re/seed values on first data point
        if (double.IsNaN(_lastE6))
        {
            e1 = e2 = e3 = e4 = e5 = e6 = value;
        }
        // normal T3 calculation with six layers
        else
        {
            e1 = _lastE1 + (K * (value - _lastE1));
            e2 = _lastE2 + (K * (e1 - _lastE2));
            e3 = _lastE3 + (K * (e2 - _lastE3));
            e4 = _lastE4 + (K * (e3 - _lastE4));
            e5 = _lastE5 + (K * (e4 - _lastE5));
            e6 = _lastE6 + (K * (e5 - _lastE6));
        }

        // calculate T3 with coefficients
        double t3 = (C1 * e6) + (C2 * e5) + (C3 * e4) + (C4 * e3);

        base.Add(new T3Result(
            timestamp,
            t3.NaN2Null()));

        // store state for next calculation
        _lastE1 = e1;
        _lastE2 = e2;
        _lastE3 = e3;
        _lastE4 = e4;
        _lastE5 = e5;
        _lastE6 = e6;
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
        _lastE1 = double.NaN;
        _lastE2 = double.NaN;
        _lastE3 = double.NaN;
        _lastE4 = double.NaN;
        _lastE5 = double.NaN;
        _lastE6 = double.NaN;
    }
}

public static partial class T3
{
    /// <summary>
    /// Creates a buffer list for T3 calculations.
    /// </summary>
    public static T3List ToT3List<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 5,
        double volumeFactor = 0.7)
        where TQuote : IQuote
        => new(lookbackPeriods, volumeFactor) { (IReadOnlyList<IQuote>)quotes };
}
