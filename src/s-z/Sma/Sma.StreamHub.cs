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

    internal SmaHub(
        IChainProvider<TIn> provider,
        int lookbackPeriods) : base(provider)
    {
        Sma.Validate(lookbackPeriods);
        LookbackPeriods = lookbackPeriods;

        Reinitialize();
    }
    #endregion

    public int LookbackPeriods { get; init; }

    // METHODS

    internal override void Add(TIn newIn, int? index)
    {
        int i = index ?? Provider.GetIndex(newIn, true);

        // candidate result
        SmaResult r = new(
            Timestamp: newIn.Timestamp,
            Sma: Sma.Increment(Provider.Results, LookbackPeriods, i).NaN2Null());

        // save and send
        Motify(r, i);
    }

    public override string ToString()
        => $"SMA({LookbackPeriods})";
}
