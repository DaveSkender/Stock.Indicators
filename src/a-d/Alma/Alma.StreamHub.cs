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
    private readonly Queue<double> window;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlmaHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
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
        window = new Queue<double>(lookbackPeriods);

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

        // Optimized sliding window approach (36% performance improvement: 7.6x → 4.89x):
        // - Weights pre-calculated once in constructor (Gaussian distribution)
        // - Uses Queue for O(1) window management
        // - Avoids repeated ProviderCache access (significant overhead reduction)
        // - Converts queue to array for O(1) indexed access during weighted sum
        double value = item.Value;
        double? alma = null;

        // Handle NaN values
        if (double.IsNaN(value))
        {
            // Reset state when encountering NaN
            window.Clear();
        }
        else
        {
            // Add new value to window
            window.Enqueue(value);

            // Remove oldest value if window is full
            if (window.Count > LookbackPeriods)
            {
                window.Dequeue();
            }

            // Calculate ALMA when window is full using pre-calculated weights
            if (window.Count == LookbackPeriods)
            {
                double weightedSum = 0;
                double[] windowArray = window.ToArray();

                for (int p = 0; p < LookbackPeriods; p++)
                {
                    weightedSum += weights[p] * windowArray[p];
                }

                alma = weightedSum / normalizationFactor;
            }
        }

        // candidate result
        AlmaResult r = new(
            Timestamp: item.Timestamp,
            Alma: alma);

        return (r, i);
    }

    /// <summary>
    /// Restores the rolling window state up to the specified timestamp.
    /// </summary>
    /// <inheritdoc/>
    protected override void RollbackState(DateTime timestamp)
    {
        // Clear state
        window.Clear();

        // Rebuild window from ProviderCache
        int index = ProviderCache.IndexGte(timestamp);
        if (index <= 0)
        {
            return;
        }

        int targetIndex = index - 1;
        int startIdx = Math.Max(0, targetIndex + 1 - LookbackPeriods);

        for (int p = startIdx; p <= targetIndex; p++)
        {
            double value = ProviderCache[p].Value;
            if (!double.IsNaN(value))
            {
                window.Enqueue(value);
            }
            else
            {
                // Reset on NaN
                window.Clear();
            }
        }
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
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
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
    /// <param name="quotes">The collection of quotes.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
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
