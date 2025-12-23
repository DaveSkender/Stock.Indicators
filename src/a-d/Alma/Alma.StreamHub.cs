namespace Skender.Stock.Indicators;

/// <summary>
/// Streaming hub for Arnaud Legoux Moving Average (ALMA) calculations.
/// </summary>
public class AlmaHub
    : ChainProvider<IReusable, AlmaResult>, IAlma
{
    private readonly string hubName;
    private readonly double[] weights;
    private readonly double normalizationFactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlmaHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="offset">The offset for the ALMA calculation.</param>
    /// <param name="sigma">The sigma for the ALMA calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    internal AlmaHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods,
        double offset,
        double sigma) : base(provider)
    {
        Alma.Validate(lookbackPeriods, offset, sigma);
        LookbackPeriods = lookbackPeriods;
        Offset = offset;
        Sigma = sigma;
        hubName = $"ALMA({lookbackPeriods},{offset},{sigma})";

        // Pre-calculate weights once for O(1) application per quote
        double m = offset * (lookbackPeriods - 1);
        double s = lookbackPeriods / sigma;

        weights = new double[lookbackPeriods];
        double norm = 0;

        for (int i = 0; i < lookbackPeriods; i++)
        {
            double wt = Math.Exp(-((i - m) * (i - m)) / (2 * s * s));
            weights[i] = wt;
            norm += wt;
        }

        normalizationFactor = norm;

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double Offset { get; init; }

    /// <inheritdoc/>
    public double Sigma { get; init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (AlmaResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate ALMA efficiently using pre-calculated weights and a rolling window
        // This is O(lookbackPeriods) which is constant for a given configuration
        // Weight calculation is done once in constructor, not on every quote
        double? alma = null;

        if (i >= LookbackPeriods - 1)
        {
            double weightedSum = 0;
            bool hasNaN = false;

            for (int p = 0; p < LookbackPeriods; p++)
            {
                int sourceIndex = i - LookbackPeriods + 1 + p;
                double value = ProviderCache[sourceIndex].Value;

                if (double.IsNaN(value))
                {
                    hasNaN = true;
                    break;
                }

                weightedSum += weights[p] * value;
            }

            alma = hasNaN ? null : weightedSum / normalizationFactor;
        }

        // candidate result
        AlmaResult r = new(
            Timestamp: item.Timestamp,
            Alma: alma);

        return (r, i);
    }
}

/// <summary>
/// Provides methods for calculating the Arnaud Legoux Moving Average (ALMA) indicator.
/// </summary>
public static partial class Alma
{
    /// <summary>
    /// Creates an ALMA streaming hub from a chain provider.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="offset">The offset for the ALMA calculation. Default is 0.85.</param>
    /// <param name="sigma">The sigma for the ALMA calculation. Default is 6.</param>
    /// <returns>An ALMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the parameters are invalid.</exception>
    public static AlmaHub ToAlmaHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
        => new(chainProvider, lookbackPeriods, offset, sigma);

    /// <summary>
    /// Creates an ALMA hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <param name="offset">The offset for the ALMA calculation. Default is 0.85.</param>
    /// <param name="sigma">The sigma for the ALMA calculation. Default is 6.</param>
    /// <returns>An instance of <see cref="AlmaHub"/>.</returns>
    public static AlmaHub ToAlmaHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods = 9,
        double offset = 0.85,
        double sigma = 6)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToAlmaHub(lookbackPeriods, offset, sigma);
    }
}
