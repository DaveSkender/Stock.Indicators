namespace Skender.Stock.Indicators;

// EXPONENTIAL MOVING AVERAGE (STREAMING)

public class Ema<TIn>
    : AbstractChainInChainOut<TIn, EmaResult>, IEma
    where TIn : struct, IReusable
{
    #region CONSTRUCTORS

    public Ema(
        IChainProvider<TIn> provider,
        int lookbackPeriods)
        : base(provider)
    {
        EmaUtilities.Validate(lookbackPeriods);

        LookbackPeriods = lookbackPeriods;
        K = 2d / (lookbackPeriods + 1);

        // subscribe to provider
        Subscription = provider != null
           ? provider.Subscribe(this)
           : throw new ArgumentNullException(nameof(provider));
    }
    #endregion

    # region PROPERTIES

    public int LookbackPeriods { get; }
    public double K { get; }
    #endregion

    # region METHODS

    // string label
    public override string ToString()
        => $"EMA({LookbackPeriods})";

    // handle chain arrival
    protected override void OnNextArrival(Act act, TIn inbound)
    {
        int i;
        double ema;

        // handle deletes
        if (act is Act.Delete)
        {
            i = Cache.FindIndex(inbound.Timestamp);
            ema = Cache[i].Ema.Null2NaN();
        }

        // handle new values
        else
        {
            i = Provider.FindIndex(inbound.Timestamp);

            // source unexpectedly not found
            if (i == -1)
            {
                throw new InvalidOperationException(
                    "Matching source history not found on arrival.");
            }

            // normal

            if (i >= LookbackPeriods - 1)
            {
                IReusable last = Cache[i - 1];  // prior EMA

                // normal
                if (!double.IsNaN(last.Value))
                {
                    ema = EmaUtilities.Increment(K, last.Value, inbound.Value);
                }

                // set first value (normal) or reset
                // when prior EMA was incalculable
                else
                {
                    double sum = 0;
                    for (int w = i - LookbackPeriods + 1; w <= i; w++)
                    {
                        sum += Provider.Results[w].Value;
                    }

                    ema = sum / LookbackPeriods;
                }
            }

            // warmup periods are never calculable
            else
            {
                ema = double.NaN;
            }
        }

        // candidate result
        EmaResult r = new(
            Timestamp: inbound.Timestamp,
            Ema: ema.NaN2Null());

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
