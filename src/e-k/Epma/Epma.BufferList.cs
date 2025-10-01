namespace Skender.Stock.Indicators;

/// <summary>
/// Endpoint Moving Average (EPMA) from incremental reusable values.
/// </summary>
public class EpmaList : List<EpmaResult>, IEpma, IBufferList, IBufferReusable
{
    private readonly Queue<double> _buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="EpmaList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    public EpmaList(int lookbackPeriods)
    {
        Epma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        _buffer = new Queue<double>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EpmaList"/> class with initial quotes.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="quotes">Initial quotes to populate the list.</param>
    public EpmaList(int lookbackPeriods, IReadOnlyList<IQuote> quotes)
        : this(lookbackPeriods)
    {
        Add(quotes);
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc />
    public void Add(DateTime timestamp, double value)
    {
        // Use universal buffer extension method for consistent buffer management
        _buffer.Update(LookbackPeriods, value);

        // Calculate EPMA when we have enough values
        double? epma = null;
        if (_buffer.Count == LookbackPeriods)
        {
            // Calculate linear regression (slope and intercept) for the buffer
            (double? slope, double? intercept) = CalculateLinearRegression();

            if (slope.HasValue && intercept.HasValue)
            {
                // EPMA calculation: slope * (current_index + 1) + intercept
                // The current index for endpoint calculation is the buffer count
                epma = (slope.Value * LookbackPeriods) + intercept.Value;

                // Apply null handling for NaN values
                epma = epma.Value.NaN2Null();
            }
        }

        base.Add(new EpmaResult(timestamp, epma));
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
        Add(quote.Timestamp, (double)quote.Close);
    }

    /// <inheritdoc />
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i]);
        }
    }

    /// <inheritdoc />
    public new void Clear()
    {
        _buffer.Clear();
        base.Clear();
    }

    /// <summary>
    /// Calculates the linear regression (slope and intercept) for the current buffer values.
    /// This implements the same least squares method as the static Slope implementation.
    /// </summary>
    /// <returns>A tuple containing the slope and intercept values.</returns>
    private (double? slope, double? intercept) CalculateLinearRegression()
    {
        if (_buffer.Count < LookbackPeriods)
        {
            return (null, null);
        }

        // Convert buffer to array for easier indexing
        double[] values = _buffer.ToArray();
        int periods = values.Length;

        // Calculate averages
        double sumX = 0;
        double sumY = 0;

        for (int i = 0; i < periods; i++)
        {
            sumX += i + 1d; // X values are 1, 2, 3, ..., n
            sumY += values[i];
        }

        double avgX = sumX / periods;
        double avgY = sumY / periods;

        // Least squares method
        double sumSqX = 0;
        double sumSqXy = 0;

        for (int i = 0; i < periods; i++)
        {
            double devX = (i + 1d) - avgX;
            double devY = values[i] - avgY;

            sumSqX += devX * devX;
            sumSqXy += devX * devY;
        }

        double? slope = (sumSqXy / sumSqX).NaN2Null();
        double? intercept = (avgY - (slope * avgX)).NaN2Null();

        return (slope, intercept);
    }
}

public static partial class Epma
{
    /// <summary>
    /// Creates a buffer list for Endpoint Moving Average (EPMA) calculations.
    /// </summary>
    public static EpmaList ToEpmaList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods)
        where TQuote : IQuote
        => new(lookbackPeriods) { (IReadOnlyList<IQuote>)quotes };
}
