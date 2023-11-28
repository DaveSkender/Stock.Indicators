namespace Skender.Stock.Indicators;

// SIMPLE MOVING AVERAGE (STREAMING)

public class Sma<TResult> : ChainProvider<SmaResult>,
    IObserver<(Act act, DateTime date, double price)>
    where TResult : IReusableResult, new()
{
    // fields
    private readonly IDisposable? unsubscriber;

    // constructor
    public Sma(
        ChainProvider<TResult> provider,
        int lookbackPeriods)
    {
        ChainSupplier = provider;
        LookbackPeriods = lookbackPeriods;

        Initialize();

        // subscribe to chain provider
        unsubscriber = provider != null
         ? provider.Subscribe(this)
         : throw new ArgumentNullException(nameof(provider));
    }

    // PROPERTIES

    private ChainProvider<TResult> ChainSupplier { get; set; }

    private int LookbackPeriods { get; set; }

    // METHODS

    // handle chain arrival
    public virtual void OnNext((Act act, DateTime date, double price) value)
    {
        // candidate result
        SmaResult r = new()
        {
            Date = value.date,
            Sma = Increment(value.date).NaN2Null()
        };

        // save to cache
        this.CacheWithAction(value.act, r);

        // send to observers
        NotifyObservers(value.act, r);
    }

    private double Increment(DateTime newDate)
    {
        int i = ChainSupplier.Cache.FindIndex(newDate);

        // normal
        if (i >= LookbackPeriods - 1)
        {
            double sum = 0;
            for (int w = i - LookbackPeriods + 1; w <= i; w++)
            {
                sum += ChainSupplier.Cache[w].Value;
            }

            return sum / LookbackPeriods;
        }

        // warmup periods
        if (i >= 0)
        {
            return double.NaN;
        }

        // i == -1 when source value not found
        throw new InvalidOperationException("Basis not found.");
    }

    public void OnCompleted() => Unsubscribe();

    public void Unsubscribe() => unsubscriber?.Dispose();

    // re/initialize my cache, from provider cache
    private void Initialize()
    {
        ResetCache();  // clears my cache (and notifies my observers)

        // current provider cache
        List<TResult> inbound = ChainSupplier.Cache;

        // replay provider quotes
        for (int i = 0; i < inbound.Count; i++)
        {
            TResult r = inbound[i];
            OnNext((Act.AddNew, r.Date, r.Value));
        }
    }
}
