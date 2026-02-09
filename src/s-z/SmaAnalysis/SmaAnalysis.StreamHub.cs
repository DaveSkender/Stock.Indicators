namespace Skender.Stock.Indicators;

/// <summary>
/// Represents a Simple Moving Average (SMA) Analysis stream hub.
/// </summary>
public class SmaAnalysisHub
    : ChainHub<IReusable, SmaAnalysisResult>, ISma
{
    internal SmaAnalysisHub(
        IChainProvider<IReusable> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        Name = $"SMA-ANALYSIS({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }
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
            double smaValue = Sma.Increment(ProviderCache, LookbackPeriods, i);

            if (!double.IsNaN(smaValue))
            {
                sma = smaValue;

                // Calculate analysis metrics
                double sumMad = 0;
                double sumMse = 0;
                double sumMape = 0;

                for (int p = i - LookbackPeriods + 1; p <= i; p++)
                {
                    double value = ProviderCache[p].Value;
                    sumMad += Math.Abs(value - sma.Value);
                    sumMse += (value - sma.Value) * (value - sma.Value);

                    sumMape += value != 0 ? Math.Abs(value - sma.Value) / value : double.NaN;
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
}
