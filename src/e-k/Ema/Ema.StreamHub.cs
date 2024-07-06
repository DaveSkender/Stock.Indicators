namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAM HUB)

#region hub interface

public interface IEmaHub
{
    int LookbackPeriods { get; }
    double K { get; }
}
#endregion

public class EmaHub<TIn>
    : ChainHub<TIn, EmaResult>, IEmaHub
    where TIn : struct, IReusable
{
    #region constructors

    public EmaHub(
        ChainProvider<TIn> provider,
        int lookbackPeriods)
        : this(provider, cache: new(), lookbackPeriods) { }

    private EmaHub(
        ChainProvider<TIn> provider,
        StreamCache<EmaResult> cache,
        int lookbackPeriods) : base(provider, cache)
    {
        Ema.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        Reinitialize();
    }
    #endregion

    public int LookbackPeriods { get; }
    public double K { get; }

    // METHODS

    public override string ToString()
        => $"EMA({LookbackPeriods})";

    public override void OnNextNew(TIn newItem)
    {
        double ema;

        int i = Supplier.StreamCache.Position(newItem);

        if (i >= LookbackPeriods - 1)
        {
            IReusable last = StreamCache.ReadCache[i - 1];

            ema = !double.IsNaN(last.Value)

                // normal
                ? Ema.Increment(K, last.Value, newItem.Value)

                // re/initialize
                : Sma.Increment(Supplier.StreamCache.ReadCache, i, LookbackPeriods);
        }

        // warmup periods are never calculable
        else
        {
            ema = double.NaN;
        }

        // candidate result
        EmaResult r = new(
            Timestamp: newItem.Timestamp,
            Ema: ema.NaN2Null());


        // save to cache
        Act act = StreamCache.Modify(Act.AddNew, r);

        // send to observers
        NotifyObservers(act, r);
    }
}
