namespace Skender.Stock.Indicators;

// ENDPOINT MOVING AVERAGE (STREAM HUB)

/// <summary>
/// Provides methods for creating EPMA hubs.
/// </summary>
public static partial class Epma
{
    /// <summary>
    /// Converts the chain provider to an EPMA hub.
    /// </summary>
    /// <typeparam name="TIn">The type of the input.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    /// <returns>An EPMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static EpmaHub<TIn> ToEpma<TIn>(
        this IChainProvider<TIn> chainProvider,
        int lookbackPeriods)
        where TIn : IReusable
        => new(chainProvider, lookbackPeriods);
}

/// <summary>
/// Represents an Endpoint Moving Average (EPMA) stream hub.
/// </summary>
/// <typeparam name="TIn">The type of the input.</typeparam>
public class EpmaHub<TIn>
    : ChainProvider<TIn, EpmaResult>, IEpma
    where TIn : IReusable
{
    #region constructors

    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="EpmaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of lookback periods.</param>
    internal EpmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Epma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"EPMA({lookbackPeriods})";

        Reinitialize();
    }
    #endregion

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (EpmaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // candidate result
        EpmaResult r = new(
            Timestamp: item.Timestamp,
            Epma: Increment(ProviderCache, LookbackPeriods, i).NaN2Null());

        return (r, i);
    }

    /// <summary>
    /// Calculates EPMA increment for the current position using linear regression.
    /// </summary>
    /// <param name="source">The source data provider cache.</param>
    /// <param name="lookbackPeriods">The number of periods to look back.</param>
    /// <param name="endIndex">The current index position to evaluate.</param>
    /// <returns>The EPMA value or double.NaN if incalculable.</returns>
    private static double Increment<T>(
        IReadOnlyList<T> source,
        int lookbackPeriods,
        int endIndex)
        where T : IReusable
    {
        if (endIndex < lookbackPeriods - 1 || endIndex >= source.Count)
        {
            return double.NaN;
        }

        // Calculate linear regression for the lookback window
        int startIndex = endIndex - lookbackPeriods + 1;

        // Calculate averages
        double sumX = 0;
        double sumY = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            sumX += i + 1d; // X values are 1, 2, 3, ..., n
            sumY += source[startIndex + i].Value;
        }

        double avgX = sumX / lookbackPeriods;
        double avgY = sumY / lookbackPeriods;

        // Least squares method
        double sumSqX = 0;
        double sumSqXy = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            double devX = (i + 1d) - avgX;
            double devY = source[startIndex + i].Value - avgY;

            sumSqX += devX * devX;
            sumSqXy += devX * devY;
        }

        if (sumSqX == 0)
        {
            return double.NaN;
        }

        double slope = sumSqXy / sumSqX;
        double intercept = avgY - (slope * avgX);

        // EPMA calculation: slope * (endpoint_index + 1) + intercept
        // The endpoint index for EPMA calculation is the lookback period
        double epma = (slope * lookbackPeriods) + intercept;

        return epma;
    }
}
