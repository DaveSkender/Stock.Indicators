namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAM HUB)

public class EmaHub<TIn>
    : ChainProvider<TIn, EmaResult>, IEma
    where TIn : IReusable
{
    #region constructors

    internal EmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Ema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        Reinitialize();
    }
    #endregion

    public int LookbackPeriods { get; init; }
    public double K { get; init; }

    // METHODS

    protected override void Add(TIn item, int? indexHint)
    {
        double ema;

        int i = indexHint ?? ProviderCache.GetIndex(item, true);

        if (i >= LookbackPeriods - 1)
        {
            ema = Cache[i - 1].Ema is not null

                // normal
                ? Ema.Increment(K, Cache[i - 1].Value, item.Value)

                // re/initialize as SMA
                : Sma.Increment(ProviderCache, LookbackPeriods, i);
        }

        // warmup periods are never calculable
        else
        {
            ema = double.NaN;
        }

        // candidate result
        EmaResult r = new(
            Timestamp: item.Timestamp,
            Ema: ema.NaN2Null());

        // save and send
        Motify(r, i);
    }

    public override string ToString()
        => $"EMA({LookbackPeriods})";
}
