namespace Skender.Stock.Indicators;

/// <summary>
/// Interface for Exponential Moving Average (EMA) calculations.
/// </summary>
public interface IEma
{
    /// <summary>
    /// Gets the number of periods to look back for the calculation.
    /// </summary>
    int LookbackPeriods { get; }

    /// <summary>
    /// Gets the smoothing factor for the calculation.
    /// </summary>
    double K { get; }
}

/// <summary>
/// Provides methods for calculating the Exponential Moving Average (EMA) indicator.
/// </summary>
public static partial class Ema
{
    /// <summary>
    /// Creates an EMA hub from a chain provider.
    /// </summary>
    /// <typeparam name="T">The type of the reusable data.</typeparam>
    /// <param name="chainProvider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <returns>An EMA hub.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the chain provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    [StreamHub("EMA", "Exponential Moving Average")]
    public static EmaHub<T> ToEma<T>(
        this IChainProvider<T> chainProvider,

        [Param("Lookback Periods", 2, 250, 20)]
        int lookbackPeriods)
        where T : IReusable
        => new(chainProvider, lookbackPeriods);
}

/// <summary>
/// Represents a hub for Exponential Moving Average (EMA) calculations.
/// </summary>
/// <typeparam name="TIn">The type of the input data.</typeparam>
public class EmaHub<TIn>
    : ChainProvider<TIn, EmaResult>, IEma
    where TIn : IReusable
{
    private readonly string hubName;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmaHub{TIn}"/> class.
    /// </summary>
    /// <param name="provider">The chain provider.</param>
    /// <param name="lookbackPeriods">The number of periods to look back for the calculation.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provider is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the lookback periods are invalid.</exception>
    internal EmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Ema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);
        hubName = $"EMA({lookbackPeriods})";

        Reinitialize();
    }

    /// <inheritdoc/>
    public int LookbackPeriods { get; init; }

    /// <inheritdoc/>
    public double K { get; private init; }

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (EmaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.IndexOf(item, true);

        double ema = i >= LookbackPeriods - 1
            ? Cache[i - 1].Ema is not null

                // normal
                ? Ema.Increment(K, Cache[i - 1].Value, item.Value)

                // re/initialize as SMA
                : Sma.Increment(ProviderCache, LookbackPeriods, i)

            // warmup periods are never calculable
            : double.NaN;

        // candidate result
        EmaResult r = new(
            Timestamp: item.Timestamp,
            Ema: ema.NaN2Null());

        return (r, i);
    }
}
