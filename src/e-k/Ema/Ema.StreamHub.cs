namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAM HUB)

#region hub interface and initializer

public interface IEma
{
    int LookbackPeriods { get; }
    double K { get; }
}

public static partial class Ema
{
    // HUB, from Chain Provider
    public static EmaHub<T> ToEma<T>(
        this IChainProvider<T> chainProvider,
        int lookbackPeriods)
        where T : IReusable
        => new(chainProvider, lookbackPeriods);
}
#endregion

public class EmaHub<TIn>
    : ChainProvider<TIn, EmaResult>, IEma
    where TIn : IReusable
{
    #region constructors

    private readonly string hubName;

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
    #endregion

    public int LookbackPeriods { get; init; }
    public double K { get; init; }

    // METHODS

    /// <inheritdoc/>
    public override string ToString() => hubName;

    /// <inheritdoc/>
    protected override (EmaResult result, int index)
        ToIndicator(TIn item, int? indexHint)
    {
        int i = indexHint ?? ProviderCache.GetIndex(item, true);

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
