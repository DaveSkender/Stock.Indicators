namespace Skender.Stock.Indicators;

/// <summary>
/// Provides methods for calculating the Triple Exponential Moving Average (TEMA) indicator.
/// </summary>
public static partial class Tema
{
    /// <summary>
    /// Creates a TEMA hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>A TEMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    public static TemaHub<T> ToTema<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods = 20)
        where T : IReusable
        => new(chainProvider, lookbackPeriods);
}

/// <summary>
/// Represents a hub for Triple Exponential Moving Average (TEMA) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class TemaHub<TIn>
    : ChainProvider<TIn, TemaResult>, ITema
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal TemaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Tema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        hubName = $"TEMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double K { get; private init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (TemaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double? tema = null;

        if (i >= LookbackPeriods - 1)
        {
            // Need to calculate triple EMA - we'll use the static series logic here
            // for each calculation step since we need three levels of EMA

            double ema1, ema2, ema3;

            // Check if this is the first calculable period (initialize as SMA)
            if (i == LookbackPeriods - 1 || Cache[i - 1].Tema is null)
            {
                // Initialize all EMAs as SMA for the first calculation
                double sum = 0;
                for (int p = i - LookbackPeriods + 1; p <= i; p++)
                {
                    sum += ProviderCache[p].Value;
                }
                ema1 = ema2 = ema3 = sum / LookbackPeriods;
            }
            else
            {
                // Get previous EMA values - we need to track them somehow
                // For StreamHub we'll recalculate from the series using available data
                // This is less efficient but maintains accuracy

                // Calculate EMA1 (first level)
                ema1 = Ema.Increment(K, GetPreviousEma1(i - 1), item.Value);

                // Calculate EMA2 (second level - EMA of EMA1)
                ema2 = Ema.Increment(K, GetPreviousEma2(i - 1), ema1);

                // Calculate EMA3 (third level - EMA of EMA2)
                ema3 = Ema.Increment(K, GetPreviousEma3(i - 1), ema2);
            }

            // Calculate TEMA: 3*EMA1 - 3*EMA2 + EMA3
            tema = (3 * ema1) - (3 * ema2) + ema3;
        }

        // candidate result
        TemaResult r = new(
            Timestamp: item.Timestamp,
            Tema: tema);

        return (r, i);
    }

    // Helper methods to get previous EMA values
    // Note: This is a simplified approach - a more efficient implementation
    // would maintain state for all three EMA levels
    private double GetPreviousEma1(int index)
    {
        if (index < 0 || Cache[index].Tema is null) return double.NaN;

        // Recalculate EMA1 for previous point
        // This is inefficient but maintains correctness for StreamHub pattern
        double sum = 0;
        int startIdx = Math.Max(0, index - LookbackPeriods + 1);
        for (int i = startIdx; i <= index; i++)
        {
            sum += ProviderCache[i].Value;
        }

        if (index == LookbackPeriods - 1)
        {
            return sum / LookbackPeriods; // SMA for first calculation
        }

        // For subsequent calculations, use EMA formula recursively
        double prevEma1 = index > LookbackPeriods - 1 ? GetPreviousEma1(index - 1) : sum / LookbackPeriods;
        return Ema.Increment(K, prevEma1, ProviderCache[index].Value);
    }

    private double GetPreviousEma2(int index)
    {
        if (index < 0 || Cache[index].Tema is null) return double.NaN;

        double ema1 = GetPreviousEma1(index);

        if (index == LookbackPeriods - 1)
        {
            return ema1; // Initialize as same value
        }

        double prevEma2 = index > LookbackPeriods - 1 ? GetPreviousEma2(index - 1) : ema1;
        return Ema.Increment(K, prevEma2, ema1);
    }

    private double GetPreviousEma3(int index)
    {
        if (index < 0 || Cache[index].Tema is null) return double.NaN;

        double ema2 = GetPreviousEma2(index);

        if (index == LookbackPeriods - 1)
        {
            return ema2; // Initialize as same value
        }

        double prevEma3 = index > LookbackPeriods - 1 ? GetPreviousEma3(index - 1) : ema2;
        return Ema.Increment(K, prevEma3, ema2);
    }
}
