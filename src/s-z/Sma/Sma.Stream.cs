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

        RebuildCache();

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
        int i;
        double sma;

        List<(DateTime _, double value)> supplier = ChainSupplier.Chain;

        // handle deletes
        if (value.act == Act.Delete)
        {
            i = Cache.FindIndex(value.date);
            sma = Cache[i].Sma.Null2NaN();
        }

        // handle new values
        else
        {
            // calculate incremental value
            i = supplier.FindIndex(value.date);

            // source unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching source history not found on arrival.");
            }

            // normal
            else if (i >= LookbackPeriods - 1)
            {
                double sum = 0;
                for (int w = i - LookbackPeriods + 1; w <= i; w++)
                {
                    sum += supplier[w].value;
                }

                sma = sum / LookbackPeriods;
            }

            // warmup periods are never calculable
            else
            {
                sma = double.NaN;
            }
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
        NotifyObservers(act, r);

        // update forward values
        if (act != Act.AddNew && i < supplier.Count - 1)
        {
            // cascade updates gracefully
            int next = act == Act.Delete ? i : i + 1;
            (DateTime d, double v) = supplier[next];
            OnNext((Act.Update, d, v));
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
