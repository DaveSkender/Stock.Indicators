namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAM HUB)

#region hub interface

public interface IEmaHub
{
    int LookbackPeriods { get; }
    double K { get; }
}
#endregion

public class EmaHub<TIn> : ReusableObserver<TIn, EmaResult>,
    IReusableHub<TIn, EmaResult>, IEmaHub
    where TIn : IReusable
{
    #region constructors

    public EmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
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

    public override void Add(TIn newIn)
    {
        if (newIn is null)
        {
            throw new ArgumentNullException(nameof(newIn));
        }

        double ema;

        int i = Supplier.ExactIndex(newIn);

        if (i >= LookbackPeriods - 1)
        {
            IReusable last = Cache[i - 1];

            ema = !double.IsNaN(last.Value)

                // normal
                ? Ema.Increment(K, last.Value, newIn.Value)

                // re/initialize
                : Sma.Increment(Supplier.Results, i, LookbackPeriods);
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


        // save to cache
        Act act = Modify(Act.AddNew, r);

        // send to observers
        NotifyObservers(act, r);
    }

    public override string ToString()
        => $"EMA({LookbackPeriods})";
}
