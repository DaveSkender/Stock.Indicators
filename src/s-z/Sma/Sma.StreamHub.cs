namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAM HUB)

#region hub interface
public interface ISmaHub
{
    int LookbackPeriods { get; }
}
#endregion

public class SmaHub<TIn> : ReusableObserver<TIn, SmaResult>,
    IReusableHub<TIn, SmaResult>, ISmaHub
    where TIn : IReusable
{
    #region constructors

    public SmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        Reinitialize();
    }
    #endregion

    public int LookbackPeriods { get; }

    // METHODS

    public override void Add(TIn newIn)
    {
        if (newIn is null)
        {
            throw new ArgumentNullException(nameof(newIn));
        }

        int i = Supplier.ExactIndex(newIn);

        // candidate result
        SmaResult r = new(
            Timestamp: newIn.Timestamp,
            Sma: Sma.Increment(Supplier.Results, i, LookbackPeriods).NaN2Null());

        // save to cache
        Act act = Modify(Act.AddNew, r);

        // send to observers
        NotifyObservers(act, r);
    }

    public override string ToString()
        => $"SMA({LookbackPeriods})";
}
