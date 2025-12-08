namespace Skender.Stock.Indicators;

/// <summary>
/// Bollinger Bands from incremental reusable values.
/// </summary>
public class BollingerBandsList : BufferList<BollingerBandsResult>, IIncrementFromChain, IBollingerBands
{
    private readonly Queue<double> buffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBandsList"/> class.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">The number of standard deviations to use for the bands.</param>
    public BollingerBandsList(int lookbackPeriods, double standardDeviations = 2)
    {
        BollingerBands.Validate(lookbackPeriods, standardDeviations);
        LookbackPeriods = lookbackPeriods;
        StandardDeviations = standardDeviations;
        buffer = new Queue<double>(lookbackPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBandsList"/> class with initial reusable values.
    /// </summary>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">The number of standard deviations to use for the bands.</param>
    /// <param name="values">Initial reusable values to populate the list.</param>
    public BollingerBandsList(int lookbackPeriods, double standardDeviations, IReadOnlyList<IReusable> values)
        : this(lookbackPeriods, standardDeviations) => Add(values);

    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the number of standard deviations for the bands.
    /// </summary>
    public double StandardDeviations { get; }

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
            double upperBand = sma + (StandardDeviations * stdDev);
            double lowerBand = sma - (StandardDeviations * stdDev);

            // Calculate derived values
            double? percentB = upperBand == lowerBand ? null
                : (value - lowerBand) / (upperBand - lowerBand);

            double? zScore = stdDev == 0 ? null : (value - sma) / stdDev;
            double? width = sma == 0 ? null : (upperBand - lowerBand) / sma;

            AddInternal(new BollingerBandsResult(
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
            AddInternal(new BollingerBandsResult(timestamp));
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
    /// Clears the list and resets internal buffers so the instance can be reused.
    /// </summary>
    public override void Clear()
    {
        base.Clear();
        buffer.Clear();
    }
}

/// <summary>
/// EXTENSION METHODS
/// </summary>
public static partial class BollingerBands
{
    /// <summary>
    /// Creates a buffer list for Bollinger Bands calculations
    /// </summary>
    /// <param name="source">Time-series values</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="standardDeviations">The number of standard deviations to use for the bands.</param>
    /// <returns>A BollingerBandsList instance pre-populated with historical data</returns>
    /// <exception cref="ArgumentNullException">Thrown when source is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid</exception>
    public static BollingerBandsList ToBollingerBandsList(
        this IReadOnlyList<IReusable> source,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        => new(lookbackPeriods, standardDeviations, source);
}
