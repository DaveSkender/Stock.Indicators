namespace Skender.Stock.Indicators;

/// <summary>
/// Arnaud Legoux Moving Average (ALMA) from incremental reusable values.
/// </summary>
public class AlmaList : BufferList<AlmaResult>, IIncrementFromChain, IAlma
{
    private readonly Queue<double> _buffer;
    private readonly double[] _weight;
    private readonly double _norm;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="offset">The offset for the ALMA calculation. Default is 0.85.</param>
    /// <param name="sigma">The sigma for the ALMA calculation. Default is 6.</param>
    public AlmaList(
        int lookbackPeriods,
        double offset = 0.85,
        double sigma = 6)
    {
        Alma.Validate(lookbackPeriods, offset, sigma);

        LookbackPeriods = lookbackPeriods;
        Offset = offset;
        Sigma = sigma;

        _buffer = new Queue<double>(lookbackPeriods);

        // Pre-calculate weights and normalization factor for efficiency
        double m = offset * (lookbackPeriods - 1);
        double s = lookbackPeriods / sigma;

        _weight = new double[lookbackPeriods];
        double norm = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            double wt = Math.Exp(-((i - m) * (i - m)) / (2 * s * s));
            _weight[i] = wt;
            norm += wt;
        }

        _norm = norm;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlmaList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="offset">The offset for the ALMA calculation. Default is 0.85.</param>
    /// <param name="sigma">The sigma for the ALMA calculation. Default is 6.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public AlmaList(
        int lookbackPeriods,
        double offset,
        double sigma,
        IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods, offset, sigma)
        => Add(quotes);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the offset parameter for the ALMA calculation.
    /// </summary>
    public double Offset { get; init; }

    /// <summary>
    /// Gets the sigma parameter for the ALMA calculation.
    /// </summary>
    public double Sigma { get; init; }




    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Use universal buffer extension method for consistent buffer management
        _buffer.Update(LookbackPeriods, value);

        double? alma = null;

        // Calculate ALMA when we have enough values
        if (_buffer.Count == LookbackPeriods)
        {
            double weightedSum = 0;
            int n = 0;

            // Apply weights to buffer values
            foreach (double bufferValue in _buffer)
            {
                weightedSum += _weight[n] * bufferValue;
                n++;
            }

            alma = weightedSum / _norm;
        }

        AddInternal(new AlmaResult(timestamp, alma));
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
    public override void Clear()
    {
        base.Clear();
        _buffer.Clear();
    }
}

public static partial class Alma
{
    /// <summary>
    /// Creates a buffer list for Arnaud Legoux Moving Average (ALMA) calculations.
    /// </summary>
    public static AlmaList ToAlmaList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods,
        double offset = 0.85,
        double sigma = 6)
        where TQuote : IQuote
        => new(lookbackPeriods, offset, sigma) { (IReadOnlyList<IQuote>)quotes };
}
