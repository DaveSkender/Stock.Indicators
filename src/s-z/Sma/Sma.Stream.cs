namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAMING)

public partial class Sma : ChainObserver<SmaResult>, ISma
{
    // constructor
    public Sma(
        ChainProvider provider,
        int lookbackPeriods)
        : base(provider)
    {
        Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;

        Initialize();

        // subscribe to chain provider
        unsubscriber = provider != null
           ? provider.Subscribe(this)
           : throw new ArgumentNullException(nameof(provider));
    }

    // PROPERTIES

    public int LookbackPeriods { get; private set; }

    // METHODS

    // handle chain arrival
    public override void OnNext((Act act, DateTime date, double price) value)
    {
        // determine incremental value
        double sma;

        int i = ChainSupplier.Chain.FindIndex(value.date);

        // source unexpectedly not found
        if (i == -1)
        {
            throw new InvalidOperationException("Matching source history not found.");
        }

        // normal
        else if (i >= LookbackPeriods - 1)
        {
            double sum = 0;
            for (int w = i - LookbackPeriods + 1; w <= i; w++)
            {
                sum += ChainSupplier.Chain[w].Value;
            }

            sma = sum / LookbackPeriods;
        }

        // warmup periods are never calculable
        else
        {
            sma = double.NaN;
        }

        // candidate result
        SmaResult r = new()
        {
            Timestamp = value.date,
            Sma = sma.NaN2Null()
        };

        // save to cache
        Act act = CacheChainorPerAction(value.act, r, sma);

        // send to observers
        if (act == Act.AddNew)
        {
            NotifyObservers(act, r);
        }

        // rebuild cache from this point forward
        else
        {
            // note: intuitively an update would be more proficient than delete and replay; however,
            // given the chain reaction of observer rebuilds, delete and rebuild is the most humane.

            ClearCache(r.Timestamp);
            RebuildCache(r.Timestamp);
        }
    }

    // delete cache between index values
    // usually called from inherited ClearCache(fromDate)
    internal override void ClearCache(int fromIndex, int toIndex)
    {
        // delete and deliver instruction,
        // in reverse order to prevent recompositions

        for (int i = toIndex; i >= fromIndex; i--)
        {
            SmaResult r = Cache[i];
            Act act = CacheChainorPerAction(Act.Delete, r, double.NaN);
            NotifyObservers(act, r);
        }
    }
}
