namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Double Exponential Moving Average (DEMA) indicator.
/// </summary>
public static partial class Dema
{
    /// <summary>
    /// Creates a DEMA hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A DEMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static DemaHub<T> ToDema<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods = 14)
        where T : IReusable
        => new(chainProvider, lookbackPeriods);
}

/// <summary>
/// Represents a hub for Double Exponential Moving Average (DEMA) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class DemaHub<TIn>
    : ChainProvider<TIn, DemaResult>, IDema
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="DemaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal DemaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Dema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        hubName = $"DEMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double K { get; private init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (DemaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? dema = null;

        if (i >= LookbackPeriods - 1)
        {
            // Calculate DEMA following the same algorithm as StaticSeries
            // For StreamHub, we need to compute EMA1 and EMA2 on-demand
            
            double ema1, ema2;

            if (i == LookbackPeriods - 1)
            {
                // First calculable period - initialize with SMA
                ema1 = ema2 = Sma.Increment(ProviderCache, LookbackPeriods, i);
            }
            else
            { 
                // Get previous DEMA result to extract EMA state
                DemaResult? prevResult = Cache.Count > 0 ? Cache[i - 1] : null;

                if (prevResult?.Dema.HasValue == true)
                {
                    // We need to recalculate the previous EMA1 and EMA2 values
                    // This is not ideal for performance but matches the pattern
                    double prevEma1 = CalculateEmaAtIndex(i - 1);
                    double prevEma2 = CalculateEma2AtIndex(i - 1);

                    // Calculate current EMA values
                    ema1 = Ema.Increment(K, prevEma1, item.Value);
                    ema2 = Ema.Increment(K, prevEma2, ema1);
                }
                else
                {
                    // Fallback to SMA initialization
                    ema1 = ema2 = Sma.Increment(ProviderCache, LookbackPeriods, i);
                }
            }

            dema = Dema.Calculate(ema1, ema2);
        }

        // candidate result
        DemaResult r = new(
            Timestamp: item.Timestamp,
            Dema: dema);

        return (r, i);
    }

    /// <summary>
    /// Calculates EMA1 value at a specific index by recomputing from SMA start.
    /// </summary>
    private double CalculateEmaAtIndex(int index)
    {
        if (index < LookbackPeriods - 1) return double.NaN;

        // Start with SMA for the first calculable period
        double ema = Sma.Increment(ProviderCache, LookbackPeriods, LookbackPeriods - 1);

        // Incrementally calculate EMA up to the requested index
        for (int j = LookbackPeriods; j <= index; j++)
        {
            ema = Ema.Increment(K, ema, ProviderCache[j].Value);
        }

        return ema;
    }

    /// <summary>
    /// Calculates EMA2 value at a specific index (EMA of EMA1 values).
    /// </summary>
    private double CalculateEma2AtIndex(int index)
    {
        if (index < LookbackPeriods - 1) return double.NaN;

        // First, calculate the initial EMA1 values for the SMA period
        List<double> ema1Values = new();
        
        // Build initial EMA1 values
        double ema1 = Sma.Increment(ProviderCache, LookbackPeriods, LookbackPeriods - 1);
        ema1Values.Add(ema1);

        for (int j = LookbackPeriods; j <= index; j++)
        {
            ema1 = Ema.Increment(K, ema1, ProviderCache[j].Value);
            ema1Values.Add(ema1);
        }

        // Now calculate EMA2 from the EMA1 values
        double ema2 = ema1Values[0]; // Start with first EMA1 value

        for (int j = 1; j < ema1Values.Count; j++)
        {
            ema2 = Ema.Increment(K, ema2, ema1Values[j]);
        }

        return ema2;
    }
}