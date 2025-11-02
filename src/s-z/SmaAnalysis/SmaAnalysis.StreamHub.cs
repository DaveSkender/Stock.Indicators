namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE ANALYSIS (STREAM HUB)

/// <summary>
/// Represents a Simple Moving Average (SMA) Analysis stream hub.
/// </summary>
public class SmaAnalysisHub
    : ChainProvider<IReusable, SmaAnalysisResult>, ISma
{
    #region fields

    private readonly string hubName;

    #endregion fields

    #region constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SmaAnalysisHub"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    internal SmaAnalysisHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"SMA-ANALYSIS({lookbackPeriods})";

        Reinitialize();
    }

    #endregion constructors

    #region properties

    /// <summary>
    /// Gets the number of lookback periods.
    /// </summary>
    public int LookbackPeriods { get; init; }

    #endregion properties

    #region methods

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (SmaAnalysisResult result, int index)
        ToIndicator(IReusable item, int? indexHint)
    {
        ArgumentNullException.ThrowIfNull(item);

        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        // Calculate SMA and analysis metrics efficiently using a rolling window over ProviderCache
        // This is O(lookbackPeriods) per update (linear in lookbackPeriods)
        // and maintains exact precision with Series implementation
        double? sma = null;
        double? mad = null;
        double? mse = null;
        double? mape = null;

        if (i >= LookbackPeriods - 1)
        {
            double sum = 0;
            bool hasNaN = false;

            for (int p = i - LookbackPeriods + 1; p <= i; p++)
            {
                double value = ProviderCache[p].Value;
                if (double.IsNaN(value))
                {
                    hasNaN = true;
                    break;
                }

                sum += value;
            }

            if (!hasNaN)
            {
                sma = sum / LookbackPeriods;

                // Calculate analysis metrics
                double sumMad = 0;
                double sumMse = 0;
                double sumMape = 0;

                for (int p = i - LookbackPeriods + 1; p <= i; p++)
                {
                    double value = ProviderCache[p].Value;
                    sumMad += Math.Abs(value - sma.Value);
                    sumMse += (value - sma.Value) * (value - sma.Value);

                    const double epsilon = 1e-8;
                    sumMape += Math.Abs(value) < epsilon ? double.NaN : Math.Abs(value - sma.Value) / value;
                }

                mad = (sumMad / LookbackPeriods).NaN2Null();
                mse = (sumMse / LookbackPeriods).NaN2Null();
                mape = (sumMape / LookbackPeriods).NaN2Null();
            }
        }

        // candidate result
        SmaAnalysisResult r = new(
            Timestamp: item.Timestamp,
            Sma: sma,
            Mad: mad,
            Mse: mse,
            Mape: mape);

        return (r, i);
    }

    #endregion methods
}

/// <summary>
/// Provides methods for creating SMA Analysis hubs.
/// </summary>
public static partial class SmaAnalysis
{
    /// <summary>
    /// Converts the chain provider to an SMA Analysis hub.
    /// </summary>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An SMA Analysis hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static SmaAnalysisHub ToSmaAnalysisHub(
        this IChainProvider<IReusable> chainProvider,
        int lookbackPeriods)
             => new(chainProvider, lookbackPeriods);

    /// <summary>
    /// Creates an SMA Analysis hub from a collection of quotes.
    /// </summary>
    /// <param name="quotes">Aggregate OHLCV quote bars, time sorted.</param>
    /// <param name="lookbackPeriods">Quantity of periods in lookback window.</param>
    /// <returns>An instance of <see cref="SmaAnalysisHub"/>.</returns>
    public static SmaAnalysisHub ToSmaAnalysisHub(
        this IReadOnlyList<IQuote> quotes,
        int lookbackPeriods)
    {
        QuoteHub quoteHub = new();
        quoteHub.Add(quotes);
        return quoteHub.ToSmaAnalysisHub(lookbackPeriods);
    }
}
