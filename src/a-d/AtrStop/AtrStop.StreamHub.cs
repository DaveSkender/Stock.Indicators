namespace Skender.Stock.Indicators;

// ATR TRAILING STOP (STREAM HUB)

#region hub interface

public interface IAtrStopHub
{
    int LookbackPeriods { get; }
    double Multiplier { get; }
    EndType EndType { get; }
}
#endregion

public class AtrStopHub<TIn> : QuoteObserver<TIn, AtrStopResult>,
    IResultHub<TIn, AtrStopResult>, IAtrStopHub
    where TIn : IQuote
{
    #region constructors

    internal AtrStopHub(
        IQuoteProvider<TIn> provider,
        int lookbackPeriods,
        double multiplier,
        EndType endType) : base(provider)
    {
        AtrStop.Validate(lookbackPeriods, multiplier);

        LookbackPeriods = lookbackPeriods;
        Multiplier = multiplier;
        EndType = endType;

        Reinitialize();
    }
    #endregion

    public int LookbackPeriods { get; }
    public double Multiplier { get; }
    public EndType EndType { get; }

    // METHODS

    internal override void Add(Act act, TIn newIn, int? index)
    {
        if (newIn is null)
        {
            throw new ArgumentNullException(nameof(newIn));
        }

        bool isBullish = true;
        double? upperBand = null;
        double? lowerBand = null;

        decimal? atrStop = null;
        decimal? buyStop = null;
        decimal? sellStop = null;



        AtrStopResult r = new(
            Timestamp: newIn.Timestamp,
            AtrStop: atrStop,
            BuyStop: buyStop,
            SellStop: sellStop);

        // save and send
        Motify(act, r, null);
    }

    public override string ToString()
        => $"ATR-STOP({LookbackPeriods},{Multiplier},{EndType})";
}
