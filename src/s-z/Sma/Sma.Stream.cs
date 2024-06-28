namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAMING)

public partial class Sma<TIn> : AbstractChainInChainOut<TIn, SmaResult>, ISma
    where TIn : struct, IReusable
{
    #region CONSTRUCTORS

    public Sma(
        IChainProvider<TIn> provider,
        int lookbackPeriods)
        : base(provider)
    {
        SmaUtilities.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;

        // subscribe to chain provider
        Subscription = provider != null
           ? provider.Subscribe(this)
           : throw new ArgumentNullException(nameof(provider));
    }
    #endregion

    # region PROPERTIES

    public int LookbackPeriods { get; private set; }
    #endregion

    # region METHODS

    // string label
    public override string ToString()
        => $"SMA({LookbackPeriods})";

    // handle chain arrival
    protected override void OnNextArrival(Act act, TIn inbound)
    {
        int i;
        double sma;

        // handle deletes
        if (act == Act.Delete)
        {
            i = Cache.FindIndex(inbound.Timestamp);
            sma = Cache[i].Sma.Null2NaN();
        }

        // handle new values
        else
        {
            // calculate incremental value
            i = Provider.FindIndex(inbound.Timestamp);

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
                    sum += ProviderCache[w].Value;
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
        SmaResult r = new(
            Timestamp: inbound.Timestamp,
            Sma: sma.NaN2Null());

        // save to cache
        act = ModifyCache(act, r);

        // send to observers
        NotifyObservers(act, r);

        // cascade update forward values (recursively)
        if (act != Act.AddNew && i < ProviderCache.Count - 1)
        {
            int next = act == Act.Delete ? i : i + 1;
            TIn value = ProviderCache[next];
            OnNextArrival(Act.Update, value);
        }
    }
    #endregion
}
