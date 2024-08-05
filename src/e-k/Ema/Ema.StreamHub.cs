namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAM HUB)

public class EmaHub<TIn> : ReusableObserver<TIn, EmaResult>,
    IReusableHub<TIn, EmaResult>, IEma
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

    public override string ToString() => hubName;

    internal override void Add(Act act, TIn newIn, int? index)
    {
        double ema;

        int i = index ?? Provider.GetIndex(newIn, false);

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
        Motify(act, r, i);
    }
}
