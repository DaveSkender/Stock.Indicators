namespace Skender.Stock.Indicators;

/// <summary>
/// Bollinger Bands from incremental reusable values.
/// </summary>
public class BollingerBandsList : List<BollingerBandsResult>, IBufferList, IBufferReusable
{
    private readonly Queue<double> buffer;
    private readonly double standardDeviations;

    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBandsList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <param name="standardDeviations">The number of standard deviations to use for the bands.</param>
    public BollingerBandsList(int lookbackPeriods, double standardDeviations = 2)
    {
        BollingerBands.Validate(lookbackPeriods, standardDeviations);
        LookbackPeriods = lookbackPeriods;
        this.standardDeviations = standardDeviations;
        buffer = new Queue<double>(lookbackPeriods);
    }

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the number of standard deviations for the bands.
    /// </summary>
    public double StandardDeviations => standardDeviations;

    /// <summary>
    /// Adds a new value to the Bollinger Bands list.
    /// </summary>
    /// <param name="timestamp">The timestamp of the value.</param>
    /// <param name="value">The value to add.</param>
    public void Add(DateTime timestamp, double value)
    {
        // Use universal buffer extension method for consistent buffer management
        buffer.Update(LookbackPeriods, value);

        // Calculate Bollinger Bands when we have enough values
        if (buffer.Count == LookbackPeriods)
        {
            // Calculate SMA by summing all values in buffer
            double sum = 0;
            foreach (double val in buffer)
            {
                sum += val;
            }
            double sma = sum / LookbackPeriods;

            // Calculate standard deviation using the same algorithm as the static series
            double[] window = buffer.ToArray();
            double stdDev = window.StdDev();

            // Calculate bands
            double upperBand = sma + (standardDeviations * stdDev);
            double lowerBand = sma - (standardDeviations * stdDev);

            // Calculate derived values
            double? percentB = upperBand - lowerBand == 0 ? null
                : (value - lowerBand) / (upperBand - lowerBand);

            double? zScore = stdDev == 0 ? null : (value - sma) / stdDev;
            double? width = sma == 0 ? null : (upperBand - lowerBand) / sma;

            base.Add(new BollingerBandsResult(
                Timestamp: timestamp,
                Sma: sma,
                UpperBand: upperBand,
                LowerBand: lowerBand,
                PercentB: percentB,
                ZScore: zScore,
                Width: width
            ));
        }
        else
        {
            // Initialization period - return null values
            base.Add(new BollingerBandsResult(timestamp));
        }
    }

    /// <summary>
    /// Adds a new reusable value to the Bollinger Bands list.
    /// </summary>
    /// <param name="value">The reusable value to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the value is null.</exception>
    public void Add(IReusable value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Add(value.Timestamp, value.Value);
    }

    /// <summary>
    /// Adds a list of reusable values to the Bollinger Bands list.
    /// </summary>
    /// <param name="values">The list of reusable values to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the values list is null.</exception>
    public void Add(IReadOnlyList<IReusable> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        for (int i = 0; i < values.Count; i++)
        {
            Add(values[i].Timestamp, values[i].Value);
        }
    }

    /// <summary>
    /// Adds a new quote to the Bollinger Bands list.
    /// </summary>
    /// <param name="quote">The quote to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quote is null.</exception>
    public void Add(IQuote quote)
    {
        ArgumentNullException.ThrowIfNull(quote);
        Add(quote.Timestamp, quote.Value);
    }

    /// <summary>
    /// Adds a list of quotes to the Bollinger Bands list.
    /// </summary>
    /// <param name="quotes">The list of quotes to add.</param>
    /// <exception cref="ArgumentNullException">Thrown when the quotes list is null.</exception>
    public void Add(IReadOnlyList<IQuote> quotes)
    {
        ArgumentNullException.ThrowIfNull(quotes);

        for (int i = 0; i < quotes.Count; i++)
        {
            Add(quotes[i].Timestamp, quotes[i].Value);
        }
    }

    /// <summary>
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public new void Clear()
    {
        base.Clear();
        buffer.Clear();
    }
}

// EXTENSION METHODS
public static partial class BollingerBands
{
    /// <summary>
    /// Creates a buffer list for Bollinger Bands calculations
    /// </summary>
    /// <typeparam name="TQuote">The type that implements IQuote</typeparam>
    /// <param name="quotes">Historical price quotes</param>
    /// <param name="lookbackPeriods">The number of periods to use for the lookback window. Default is 20.</param>
    /// <param name="standardDeviations">The number of standard deviations to use for the bands. Default is 2.</param>
    /// <returns>A BollingerBandsList instance pre-populated with historical data</returns>
    /// <exception cref="ArgumentNullException">Thrown when quotes is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid</exception>
    public static BollingerBandsList ToBollingerBandsBufferList<TQuote>(
        this IReadOnlyList<TQuote> quotes,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        where TQuote : IQuote
    {
        // Input validation
        ArgumentNullException.ThrowIfNull(quotes);
        Validate(lookbackPeriods, standardDeviations);

        // Initialize buffer and populate
        BollingerBandsList bufferList = new(lookbackPeriods, standardDeviations);

        foreach (TQuote quote in quotes)
        {
            bufferList.Add(quote);
        }

        return bufferList;
    }
}
