namespace Skender.Stock.Indicators;

// BOLLINGER BANDS (STREAM HUB)

/// <summary>
/// Provides methods for creating Bollinger Bands hubs.
/// </summary>
public static partial class BollingerBands
{
    /// <summary>
    /// Converts the chain provider to a Bollinger Bands hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <param name="standardDeviations">The number of standard deviations.</param>
    /// <returns>A Bollinger Bands hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    public static BollingerBandsHub<TIn> ToBollingerBands<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods = 20,
        double standardDeviations = 2)
        where TIn : IReusable
        => new(chainProvider, lookbackPeriods, standardDeviations);
}

/// <summary>
/// Represents a Bollinger Bands stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class BollingerBandsHub<TIn>
    : ChainProvider<TIn, BollingerBandsResult>
    where TIn : IReusable
{
    #region constructors

    private readonly string hubName;
    private readonly Queue<double> buffer;
    private readonly double standardDeviations;

    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBandsHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <param name="standardDeviations">The number of standard deviations.</param>
    internal BollingerBandsHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods,
        double standardDeviations) : base(provider)
    {
        BollingerBands.Validate(lookbackPeriods, standardDeviations);
        LookbackPeriods = lookbackPeriods;
        this.standardDeviations = standardDeviations;
        hubName = $"BB({lookbackPeriods},{standardDeviations})";
        buffer = new Queue<double>(lookbackPeriods);
    }

    #endregion

    #region properties

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    /// <summary>
    /// Gets the number of standard deviations.
    /// </summary>
    public double StandardDeviations => standardDeviations;

    #endregion

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (BollingerBandsResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        double value = item.Value;
        int index = indexHint ?? ProviderCache.IndexOf(item, true);

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

            return (new BollingerBandsResult(
                Timestamp: item.Timestamp,
                Sma: sma,
                UpperBand: upperBand,
                LowerBand: lowerBand,
                PercentB: percentB,
                ZScore: zScore,
                Width: width
            ), index);
        }
        else
        {
            // Initialization period - return null values
            return (new BollingerBandsResult(item.Timestamp), index);
        }
    }

    #endregion
}
