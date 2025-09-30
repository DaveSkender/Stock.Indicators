namespace Skender.Stock.Indicators;

/// <summary>
/// MESA Adaptive Moving Average (MAMA) from incremental reusable values.
/// </summary>
public class MamaList : List<MamaResult>, IMama, IBufferList, IBufferReusable
{
    // Price data storage for recalculation
    private readonly List<double> pr = new(); // price

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
        // Add the raw quote data
        pr.Add(value);

        // Create a list of QuotePart values (which implement IReusable)
        var values = pr.Select((p, i) => new QuotePart(
            i < Count ? this[i].Timestamp : timestamp,
            p)).ToList();

        var results = values.ToMama(FastLimit, SlowLimit);

        // Clear the current results
        base.Clear();

        // Add all the recalculated results
        foreach (var result in results)
        {
            base.Add(result);
        }
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
        pr.Clear();
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
