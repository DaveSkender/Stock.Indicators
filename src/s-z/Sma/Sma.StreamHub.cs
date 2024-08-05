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

    private readonly string hubName;
    
    internal SmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;
        hubName = $"SMA({lookbackPeriods})";

        Reinitialize();
    }
    #endregion

    public int LookbackPeriods { get; init; }

    // METHODS

    public override string ToString() => hubName;

    internal override void Add(Act act, TIn newIn, int? index)
    {
        int i = index ?? Provider.GetIndex(newIn, false);

        // candidate result
        SmaResult r = new(
            Timestamp: newIn.Timestamp,
            Sma: Sma.Increment(Provider.Results, i, LookbackPeriods).NaN2Null());

        // save and send
        Motify(act, r, i);
    }
}
