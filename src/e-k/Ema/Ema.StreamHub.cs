namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAM HUB)

public class EmaHub<TIn> : ReusableObserver<TIn, EmaResult>,
    IReusableHub<TIn, EmaResult>, IEma
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

    internal override void Add(TIn newIn, int? index)
    {
        double ema;

        int i = index ?? Provider.GetIndex(newIn, true);

        if (i >= LookbackPeriods - 1)
        {
            ema = Cache[i - 1].Ema is not null

                // normal
                ? Ema.Increment(K, Cache[i - 1].Value, newIn.Value)

                // re/initialize as SMA
                : Sma.Increment(Provider.Results, LookbackPeriods, i);
        }

        // warmup periods are never calculable
        else
        {
            ema = double.NaN;
        }

        // candidate result
        EmaResult r = new(
            Timestamp: newIn.Timestamp,
            Ema: ema.NaN2Null());

        // save and send
        Motify(r, i);
    }

    public override string ToString()
        => $"EMA({LookbackPeriods})";
}
